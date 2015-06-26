//
// Rich Range Bars
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
    public class RichRange : BarsType, rwt.IExtendedData
  {

     private List<rwt.ExtendedData> extdat;

        private bool uptick;
        private double prevClose;
        private rwt.ExtendedData curdat;
        private double tickSize;

      // for noise removal
        private bool lastBarUP;
        private DateTime lastBarTime;

        // for int mvmt....
        private double[] prices;  
        private bool lastMoveUp;  

        private static bool registered = Data.BarsType.Register(new RichRange());

    public rwt.ExtendedData getExtraData(int barsBack, Data.Bars bars, int cb) {
        var idx = extdat.Count - bars.Count + cb - barsBack;
        if(idx < 0)  return null;
        return extdat[idx];
    }


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
      if((curdat == null) || (time < curdat.dt)) {
          extdat.Clear();
          extdat.TrimExcess();
          curdat = null;
          tickSize  = bars.Instrument.MasterInstrument.TickSize;
      }
      if (bars.Count == 0 || bars.IsNewSession(time, isRealtime)) {
        AddBar(bars, open, high, low, close, time, volume, isRealtime);
        lastBarTime = time;
        RWTAddExtDat(time,close,volume);
      }
      else
      {
        Data.Bar  bar     = (Bar) bars.Get(bars.Count - 1); 
        //double    tickSize  = bars.Instrument.MasterInstrument.TickSize;
        double    rangeValue  = Math.Floor(10000000.0 * (double) bars.Period.Value * tickSize) / 10000000.0;
        if(curdat == null) RWTAddExtDat(time,close,0);
        curdat.dt = time;

        if( (bars.Instrument.MasterInstrument.Compare(close, bar.Low + rangeValue) > 0)  &&
                   (lastBarUP || ((time.Ticks - lastBarTime.Ticks)  > 10000000L)) )
        {
          bool  isFirstNewBar = true;
          double  newClose    = bar.Low + rangeValue; // every bar closes either with high or low
          UpdateBar(bars, bar.Open, newClose, bar.Low, newClose, time, 0, isRealtime);
          curdat.addLevel(1); 

          // if still gap, fill with phantom bars
          double newBarOpen = newClose + tickSize;
          while (bars.Instrument.MasterInstrument.Compare(close, newClose) > 0)
          {
            newClose = Math.Min(close, newBarOpen + rangeValue);
            AddBar(bars, newBarOpen, newClose, newBarOpen, newClose, time, isFirstNewBar ? volume : 1, isRealtime);
            RWTAddExtDat(time, newClose, (isFirstNewBar?volume:0) );

            newBarOpen = newClose + tickSize;
            isFirstNewBar = false;
          }
          prices[1] = newClose;  
          lastMoveUp = true;  
          lastBarUP = true;
          lastBarTime = time;
        }
        else if( (bars.Instrument.MasterInstrument.Compare(bar.High - rangeValue, close) > 0) &&
                        ((!lastBarUP) || ((time.Ticks - lastBarTime.Ticks)  > 10000000L)) )
        {
          bool  isFirstNewBar = true;
          double  newClose    = bar.High - rangeValue; // every bar closes either with high or low
          UpdateBar(bars, bar.Open, bar.High, newClose, newClose, time, 0, isRealtime);
          curdat.addLevel(-1);

          // if still gap, fill with phantom bars
          double newBarOpen = newClose - tickSize;
          while (bars.Instrument.MasterInstrument.Compare(newClose, close) > 0)
          {
            newClose = Math.Max(close, newBarOpen - rangeValue);
            AddBar(bars, newBarOpen, newBarOpen, newClose, newClose, time, isFirstNewBar ? volume : 1, isRealtime);
            RWTAddExtDat(time, newClose, (isFirstNewBar?volume:0) );

            newBarOpen = newClose - tickSize;
            isFirstNewBar = false;
          }
          prices[0] = newClose; 
          lastMoveUp = false; 
          lastBarUP = false;
          lastBarTime = time;

        }
        else {
          UpdateBar(bars, open, (close > bar.High ? close : bar.High), (close < bar.Low ? close : bar.Low), close, time, volume, isRealtime);
          RWTUpdateCounts(close); 
          RWTUpdateCurdat(close,volume);
        }
      }
      bars.LastPrice = close;

      // update the extended data...
      prevClose = close;
    }

        private void RWTAddExtDat(DateTime tm, double close, long volume) {
              curdat = new rwt.ExtendedData(tm,curdat);
              extdat.Add(curdat);
              if(volume > 0) RWTUpdateCurdat(close,volume);
        }

        private void RWTUpdateCounts(double close) {  
             if((close != prices[0]) && (close != prices[1])) {
                 if(close < prices[0]) {
                    curdat.addLevel(-1);  
                    prices[0] = close;
                    lastMoveUp = false;
                 } else if(close > prices[1]) {
                    curdat.addLevel(1);
                    prices[1] = close;
                    lastMoveUp = true;
                 } else if(lastMoveUp) {
                    prices[0] = close;
                 } else {
                    prices[1] = close;
                 }
             }

        }

    private void RWTUpdateCurdat(double close, long volume) {
        if ((close > prevClose) || ((close == prevClose) && uptick)) {
          uptick = true;
          curdat.addVolume(close, volume, tickSize);
        }
        else if((close < prevClose) || ((close == prevClose) && !uptick)) {
          uptick = false;
          curdat.addVolume(close,-volume, tickSize);
        }
    }

    public override void ApplyDefaults(Gui.Chart.BarsData barsData)
    {
      barsData.DaysBack = 3;
      barsData.Period.Value = 4;
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
              return new RichRange();
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
                    get { return "RichRange"; }
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
    public RichRange() 
                  : base(PeriodType.Final4)
    {
        extdat = new List<rwt.ExtendedData>();
        uptick = true;
        prevClose = 0;
        curdat = null;
        prices = new double[2];
        lastMoveUp = false;
        tickSize = 0;
        lastBarUP = true;
        lastBarTime = DateTime.Now;
    }

    /// <summary>
    /// </summary>
    /// <param name="period"></param>
    /// <returns></returns>
    public override string ToString(Period period)
    {
      return "RichRange " + period.Value.ToString() + " Ticks";
    }
  }

}
