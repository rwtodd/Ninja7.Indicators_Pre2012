// 
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
//
#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

#endregion

// This namespace holds all bars types. Do not change it.
namespace NinjaTrader.Data
{
    /// <summary>
    /// </summary>
    public class VolDensType : BarsType, rwt.IExtendedData
    {
    private List<rwt.ExtendedData> extdat;
        private bool uptick;
        private double prevClose;
        private rwt.ExtendedData curdat;

        // for int mvmt....
        private double[] prices;  
        private bool lastMoveUp; 
        private double tickSize;
    
        private static bool registered = Data.BarsType.Register(new VolDensType());
    
    public rwt.ExtendedData getExtraData(int barsBack, Data.Bars bars, int cb)
        {
            var idx = extdat.Count - bars.Count + cb - barsBack;
            if (idx < 0) return null;
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
             AddBar(bars, open, high, low, close, time.Date, volume, isRealtime);
             RWTAddExtDat(time, close, volume);
           }
           else
           {
              if (curdat == null) RWTAddExtDat(time, close, 0);
              Bar bar = (Bar)bars.Get(bars.Count - 1);

              double tall  = 1 + ((bar.High - bar.Low) / bars.Instrument.MasterInstrument.TickSize);
              double denom = tall;

              if (tall < 2) denom += 99999;
              if (tall > 10) denom -= ((tall - 10) / 2.0);
              if (tall > 20) denom -= ((tall - 20) / 3.0);
              if( ((double)(bar.Volume + volume))/denom >= ((double)(bars.Period.Value)) ) 
              {
                AddBar(bars,close,close,close,close,time,volume,isRealtime);
                RWTAddExtDat(time, close, volume);
              }
              else {
                UpdateBar(bars, open, high, low, close, time, volume, isRealtime);
                curdat.dt=time;
                RWTUpdateCurdat(close, volume);
              }
           }
           bars.LastPrice = close;
           prevClose = close;
        }
    
    private void RWTAddExtDat(DateTime tm, double close, long volume)
        {
            curdat = new rwt.ExtendedData(tm, curdat);
            extdat.Add(curdat);
            if(volume > 0) { 
                 RWTUpdateCounts(close); 
                 RWTUpdateCurdat(close,volume);
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

    private void RWTUpdateCurdat(double close, long volume) {
        if ((close > prevClose) || ((close == prevClose) && uptick)) {
          uptick = true;
          curdat.addVolume(close,volume,tickSize);
        }
        else if((close < prevClose) || ((close == prevClose) && !uptick)) {
          uptick = false;
          curdat.addVolume(close,-volume,tickSize);
        }
    }


        /// <summary>
        /// </summary>
        /// <param name="barsData"></param>
        public override void ApplyDefaults(Gui.Chart.BarsData barsData)
        {
            barsData.DaysBack = 3;
            barsData.Period.Value = 10;
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
            return new VolDensType();
        }
        /// <summary>
        /// </summary>
        public override int DefaultValue
        {
            get { return 10; }
        }

        /// <summary>
        /// </summary>
        public override string DisplayName
        {
            get { return "RichVDense"; }
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
        public VolDensType()
            : base(PeriodType.Custom7)
        {
            uptick = true;
            prevClose = -1;
            curdat = null;
            extdat = new List<rwt.ExtendedData>();
            prices = new double[2];
            lastMoveUp = false;
            tickSize = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public override string ToString(Period period)
        {
            return string.Format("RichVDense {0} Per Tick", period.Value);
        }
    }
}
