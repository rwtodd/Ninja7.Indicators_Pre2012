#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// adx vma
    /// </summary>
    [Description("adx vma")]
    public class zADXVMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int aDXLen = 12; // Default setting for ADXLen
            private double fastSpeed = 1.2; // Default setting for FastSpeed
            private double slowestSpeed = 50.000; // Default setting for SlowestSpeed
            private int windowLength = 10; // Default setting for WindowLength
			private bool useJustInput = false;
			private RWT_MA.MAType adxType = RWT_MA.MAType.EMA;
			private RWT_MA.MAType dmType = RWT_MA.MAType.EMA;
        // User defined variables (add any user defined variables below)
			private ZADX myadx;
			private zStochastic mystoch;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "ADXVMA"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
		   myadx = this.ZADX(aDXLen,adxType,1.0, RWT_HA.SecondaryOHLC.NONE, (useJustInput?RWT_HA.PrimaryOHLC.INPUTS:RWT_HA.PrimaryOHLC.BARS), aDXLen, dmType);
		   mystoch = this.zStochastic(myadx.ADX,windowLength,true);
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar == 0) {
			  ADXVMA.Set(Input[0]);
			  return;
			}
			
			var curStoch = mystoch[0];
			var emaAlpha = 2.0/(fastSpeed + (1.0 - curStoch) * (slowestSpeed - fastSpeed) + 1.0);
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            ADXVMA.Set(ADXVMA[1] + emaAlpha*(Input[0]-ADXVMA[1]));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ADXVMA
        {
            get { return Values[0]; }
        }

        [Description("length of ADX")]
        [GridCategory("Parameters")]
        public int ADXLen
        {
            get { return aDXLen; }
            set { aDXLen = Math.Max(1, value); }
        }

        [Description("fastest ema ")]
        [GridCategory("Parameters")]
        public double FastSpeed
        {
            get { return fastSpeed; }
            set { fastSpeed = Math.Max(1, value); }
        }

        [Description("slowest ema")]
        [GridCategory("Parameters")]
        public double SlowestSpeed
        {
            get { return slowestSpeed; }
            set { slowestSpeed = Math.Max(2.000, value); }
        }
        [Description("Use Just Inputs, or OHLCBars?")]
        [GridCategory("Parameters")]
        public bool UseJustInputs
        {
            get { return useJustInput; }
            set { useJustInput = value; }
        }

        [Description("Length of ADX Window")]
        [GridCategory("Parameters")]
        public int WindowLength
        {
            get { return windowLength; }
            set { windowLength = Math.Max(3, value); }
        }
        [Description("smoothing")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType ADXType
		{
			get { return adxType; } set { adxType = value; }	
		}
        [Description("smoothing")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType DMType
		{
			get { return dmType; } set { dmType = value; }	
		}
		
		
		
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private zADXVMA[] cachezADXVMA = null;

        private static zADXVMA checkzADXVMA = new zADXVMA();

        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        public zADXVMA zADXVMA(int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            return zADXVMA(Input, aDXLen, aDXType, dMType, fastSpeed, slowestSpeed, useJustInputs, windowLength);
        }

        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        public zADXVMA zADXVMA(Data.IDataSeries input, int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            if (cachezADXVMA != null)
                for (int idx = 0; idx < cachezADXVMA.Length; idx++)
                    if (cachezADXVMA[idx].ADXLen == aDXLen && cachezADXVMA[idx].ADXType == aDXType && cachezADXVMA[idx].DMType == dMType && Math.Abs(cachezADXVMA[idx].FastSpeed - fastSpeed) <= double.Epsilon && Math.Abs(cachezADXVMA[idx].SlowestSpeed - slowestSpeed) <= double.Epsilon && cachezADXVMA[idx].UseJustInputs == useJustInputs && cachezADXVMA[idx].WindowLength == windowLength && cachezADXVMA[idx].EqualsInput(input))
                        return cachezADXVMA[idx];

            lock (checkzADXVMA)
            {
                checkzADXVMA.ADXLen = aDXLen;
                aDXLen = checkzADXVMA.ADXLen;
                checkzADXVMA.ADXType = aDXType;
                aDXType = checkzADXVMA.ADXType;
                checkzADXVMA.DMType = dMType;
                dMType = checkzADXVMA.DMType;
                checkzADXVMA.FastSpeed = fastSpeed;
                fastSpeed = checkzADXVMA.FastSpeed;
                checkzADXVMA.SlowestSpeed = slowestSpeed;
                slowestSpeed = checkzADXVMA.SlowestSpeed;
                checkzADXVMA.UseJustInputs = useJustInputs;
                useJustInputs = checkzADXVMA.UseJustInputs;
                checkzADXVMA.WindowLength = windowLength;
                windowLength = checkzADXVMA.WindowLength;

                if (cachezADXVMA != null)
                    for (int idx = 0; idx < cachezADXVMA.Length; idx++)
                        if (cachezADXVMA[idx].ADXLen == aDXLen && cachezADXVMA[idx].ADXType == aDXType && cachezADXVMA[idx].DMType == dMType && Math.Abs(cachezADXVMA[idx].FastSpeed - fastSpeed) <= double.Epsilon && Math.Abs(cachezADXVMA[idx].SlowestSpeed - slowestSpeed) <= double.Epsilon && cachezADXVMA[idx].UseJustInputs == useJustInputs && cachezADXVMA[idx].WindowLength == windowLength && cachezADXVMA[idx].EqualsInput(input))
                            return cachezADXVMA[idx];

                zADXVMA indicator = new zADXVMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXLen = aDXLen;
                indicator.ADXType = aDXType;
                indicator.DMType = dMType;
                indicator.FastSpeed = fastSpeed;
                indicator.SlowestSpeed = slowestSpeed;
                indicator.UseJustInputs = useJustInputs;
                indicator.WindowLength = windowLength;
                Indicators.Add(indicator);
                indicator.SetUp();

                zADXVMA[] tmp = new zADXVMA[cachezADXVMA == null ? 1 : cachezADXVMA.Length + 1];
                if (cachezADXVMA != null)
                    cachezADXVMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezADXVMA = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zADXVMA zADXVMA(int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            return _indicator.zADXVMA(Input, aDXLen, aDXType, dMType, fastSpeed, slowestSpeed, useJustInputs, windowLength);
        }

        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        public Indicator.zADXVMA zADXVMA(Data.IDataSeries input, int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            return _indicator.zADXVMA(input, aDXLen, aDXType, dMType, fastSpeed, slowestSpeed, useJustInputs, windowLength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zADXVMA zADXVMA(int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            return _indicator.zADXVMA(Input, aDXLen, aDXType, dMType, fastSpeed, slowestSpeed, useJustInputs, windowLength);
        }

        /// <summary>
        /// adx vma
        /// </summary>
        /// <returns></returns>
        public Indicator.zADXVMA zADXVMA(Data.IDataSeries input, int aDXLen, RWT_MA.MAType aDXType, RWT_MA.MAType dMType, double fastSpeed, double slowestSpeed, bool useJustInputs, int windowLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zADXVMA(input, aDXLen, aDXType, dMType, fastSpeed, slowestSpeed, useJustInputs, windowLength);
        }
    }
}
#endregion
