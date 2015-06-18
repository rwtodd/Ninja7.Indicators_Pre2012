//
// PointEauxBars
//
//
#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
#endregion

// This namespace holds all bars types. Do not change it.
namespace NinjaTrader.Data
{
  /// <summary>
  /// </summary>
    public class PointEauxType : BarsType
  {

       
        
        private bool tlb; // three-line-break
        private double period;

        // for noise removal
        private bool lastBarUP;
        private DateTime lastBarTime;

        private double tickSize;
        private static bool registered = Data.BarsType.Register(new PointEauxType());


    /// <summary>
    /// </summary>
    /// <param name="bars"></param>
    /// <param name="open"></param>
    /// <param name="high"></param>
    /// <param name="low"></param>
    /// <param name="close"></param>
    /// <param name="time"></param>
    /// <param name="volume"></param>
    /// <param name="isRealtime"></param>
    public override void Add(Data.Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isRealtime)
        {
            if (tickSize == 0)
            {
                tickSize = bars.Instrument.MasterInstrument.TickSize;
                if(bars.Period.Value > 100) {
                  tlb = true;
                  period = bars.Period.Value - 100;
                }  else {
                  tlb = false;
                  period = bars.Period.Value;
                }
            } 
            if (bars.Count == 0 || bars.IsNewSession(time, isRealtime)) {
                AddBar(bars, open, high, low, close, time, volume, isRealtime);
                lastBarTime = time;
            }
            else
            {
                Data.Bar bar = (Bar)bars.Get(bars.Count - 1);
                double rangeValue = Math.Floor(10000000.0 * period * tickSize) / 10000000.0;

                double reverseRange = rangeValue*(tlb?3.0:1.0);

                if( (bars.Instrument.MasterInstrument.Compare(close, bar.Open + (lastBarUP?rangeValue:reverseRange)) > 0) &&
                    (lastBarUP || ((time.Ticks - lastBarTime.Ticks)  > 10000000L)) )
                {
                    bool isFirstNewBar = true;
                    double newBarOpen = bar.Open + (lastBarUP?rangeValue:reverseRange);
                    double newClose = Math.Min(close, newBarOpen + rangeValue);
                    UpdateBar(bars, bar.Open, newBarOpen, bar.Low, newBarOpen, time, 0, isRealtime);
                    do
                    {
                        AddBar(bars, newBarOpen, newClose, newBarOpen, newClose, time, isFirstNewBar ? volume : 0, isRealtime);
                        if (bars.Instrument.MasterInstrument.Compare(close, newClose) == 0) break;
                        newBarOpen = newClose;
                        newClose = Math.Min(close, newBarOpen + rangeValue);
                        isFirstNewBar = false;
                    }
                    while (true);

                  
                    lastBarUP = true;
                    lastBarTime = time;
                }
                else
                    if((bars.Instrument.MasterInstrument.Compare(bar.Open - (lastBarUP?reverseRange:rangeValue), close) > 0) &&
                        ((!lastBarUP) || ((time.Ticks - lastBarTime.Ticks)  > 10000000L)) )
                    {
                        bool isFirstNewBar = true;
                        double newBarOpen = bar.Open -  (lastBarUP?reverseRange:rangeValue);
                        double newClose = Math.Max(close, newBarOpen - rangeValue);
                        UpdateBar(bars, bar.Open, bar.High, newBarOpen, newBarOpen, time, 0, isRealtime);
                        do
                        {
                            AddBar(bars, newBarOpen, newBarOpen, newClose, newClose, time, isFirstNewBar ? volume : 0, isRealtime);
                            if (bars.Instrument.MasterInstrument.Compare(close, newClose) == 0) break;
                            newBarOpen = newClose;
                            newClose = Math.Max(close, newBarOpen - rangeValue);
                            isFirstNewBar = false;
                        }
                        while (true);

              
                        lastBarUP = false;
                        lastBarTime = time;
                    }
                    else
                    {
                        UpdateBar(bars, bar.Open, Math.Max(bar.High,close), Math.Min(bar.Low,close), close, time, volume, isRealtime);
                    }
            }
            bars.LastPrice = close;

           
        }

    public override void ApplyDefaults(Gui.Chart.BarsData barsData)
    {
      barsData.DaysBack   = 3;
      barsData.Period.Value = 2;
    }

    /// <summary>
    /// </summary>
    public override PeriodType BuiltFrom
    {
      get { return PeriodType.Tick; }
    }

    /// <summary>
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public override string ChartDataBoxDate(DateTime time)
    {
      return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
    }

    /// <summary>
    /// </summary>
    /// <param name="chartControl"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public override string ChartLabel(NinjaTrader.Gui.Chart.ChartControl chartControl, DateTime time)
    {
      return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
    }

    /// <summary>
    /// Here is how you restrict the selectable chart styles by bars type
    /// </summary>
    public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
    {
      get { return new Gui.Chart.ChartStyleType[] { Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
        Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
        Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
        Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
        Gui.Chart.ChartStyleType.Final4 }; }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
            return new PointEauxType();
    }

    /// <summary>
    /// </summary>
    public override int DefaultValue
    { 
      get { return 4; }
    }

    /// <summary>
    /// </summary>
    public override string DisplayName
    {
            get { return "RichEaux"; }
    }

    /// <summary>
    /// </summary>
    /// <param name="period"></param>
    /// <param name="barsBack"></param>
    /// <returns></returns>
    public override int GetInitialLookBackDays(Period period, int barsBack)
    { 
      return 1;
    } 
      
    /// <summary>
    /// </summary>
    public override double GetPercentComplete(Data.Bars bars, DateTime now)
    {
      throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
    }

    /// <summary>
    /// </summary>
    /// <param name="propertyDescriptor"></param>
    /// <param name="period"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, Period period, Attribute[] attributes)
    {
      PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, period, attributes);

      // here is how you remove properties not needed for that particular bars type
      properties.Remove(properties.Find("BasePeriodType", true));
      properties.Remove(properties.Find("BasePeriodValue", true));
      properties.Remove(properties.Find("PointAndFigurePriceType", true));
      properties.Remove(properties.Find("ReversalType", true));
      properties.Remove(properties.Find("Value2", true));

      // here is how you change the display name of the property on the properties grid
      Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

      return properties;
    }

    /// <summary>
    /// </summary>
    public override bool IsIntraday
    {
      get { return true; }
    }

    /// <summary>
    /// </summary>
    public PointEauxType()
            : base(PeriodType.Custom9)
    {
            tickSize = 0;
            lastBarUP = true;
            lastBarTime = DateTime.Now;
            tlb = false;
            period = -1;
    }

    /// <summary>
    /// </summary>
    /// <param name="period"></param>
    /// <returns></returns>
    public override string ToString(Period period)
    {
//      return period.Value.ToString() + "RichEaux " + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
      return "RichEaux " + period.Value.ToString() + " Ticks";
    }
  }

}
