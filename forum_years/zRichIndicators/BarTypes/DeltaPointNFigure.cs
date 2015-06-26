// 
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
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
  public class PnFBarsType1 : BarsType, rwt.IExtendedData
  {
    private List<rwt.ExtendedData> extdat;
     private double prevClose;
     private double tickSize;
     private bool uptick;
     private rwt.ExtendedData curdat;
     private double[] prices;  
     private bool lastMoveUp; 

    private static bool   registered    = Data.BarsType.Register(new PnFBarsType1());
    private bool upbar = true;
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
    public override void Add(Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isRealtime)
    {
      if ((curdat == null) || (time < curdat.dt))
            {
                extdat.Clear();
                extdat.TrimExcess();
                curdat = null;
                tickSize = bars.Instrument.MasterInstrument.TickSize;
            } 
      if (bars.Count == 0){
		     AddBar(bars, open, high, low, close, time, volume, isRealtime);
         RWTAddExtDat(time, close, volume);
      }
      else
      {
        Data.Bar  bar     = (Bar) bars.Get(bars.Count - 1); 
        if (curdat == null) RWTAddExtDat(time, close, 0);
        curdat.dt = time;
			  if(upbar && ((curdat.dHigh - curdat.dClose) >= bars.Period.Value) ) {
               AddBar(bars, close, close, close, close, time, volume,isRealtime);
               RWTAddExtDat(time,close,volume);
               upbar = false;
        }       
			  else if(!upbar && ((curdat.dClose - curdat.dLow) >= bars.Period.Value)){
              AddBar(bars, close, close,close,close,time, volume,isRealtime);
               RWTAddExtDat(time,close,volume);
               upbar = true;
        } 
        else {
               UpdateBar(bars,open,high,low,close,time,volume,isRealtime); // regular update...
			         RWTUpdateCounts(close);
               RWTUpdateCurdat(close, volume);
        }

      }
      bars.LastPrice = close;

      // update the extended data...
      prevClose = close;
    }

    private void RWTAddExtDat(DateTime tm, double close, long volume)
        {
            curdat = new rwt.ExtendedData(tm, curdat);
            extdat.Add(curdat);
            if (volume > 0) { 
               RWTUpdateCounts(close);
               RWTUpdateCurdat(close, volume);
            }
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

       /// <summary>
        /// </summary>
        /// <param name="barsData"></param>
        public override void ApplyDefaults(Gui.Chart.BarsData barsData)
        {
            barsData.DaysBack = 3;
            barsData.Period.Value = 3;
        }
     private void RWTUpdateCurdat(double close, long volume)
        {
            if ((close > prevClose) || ((close == prevClose) && uptick))
            {
                uptick = true;
                curdat.addVolume(close,volume,tickSize);
            }
            else if ((close < prevClose) || ((close == prevClose) && !uptick))
            {
                uptick = false;
                curdat.addVolume(close,-volume,tickSize);
            }
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
        public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
        {
            return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
        }

        /// <summary>
        /// Here is how you restrict the selectable chart styles by bars type
        /// </summary>
        public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
        {
            get
            {
                return new Gui.Chart.ChartStyleType[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
        Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
        Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
        Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
        Gui.Chart.ChartStyleType.Final4 };
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new PnFBarsType1();
        }

        /// <summary>
        /// </summary>
        public PnFBarsType1()
            : base(PeriodType.Custom4)
        {
            extdat = new List<rwt.ExtendedData>();
            uptick = true;
            prevClose = 0;
            curdat = null;
            prices = new double[2];
            lastMoveUp = false;
            tickSize = 0;
        }

        /// <summary>
        /// </summary>
        public override int DefaultValue
        {
            get { return 3; }
        }

        /// <summary>
        /// </summary>
        public override string DisplayName
        {
            get { return "Delta PnF"; }
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
        public override double GetPercentComplete(Bars bars, DateTime now)
        {
            return 0.5;
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
        /// <param name="period"></param>
        /// <returns></returns>
        public override string ToString(Period period)
        {
            return string.Format("DeltaPnF {0} Reversal", period.Value);
        }
  }
}
