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
    /// laguerre macd
    /// </summary>
    [Description("laguerre macd")]
    public class Z20101212LaguerreMACD : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int fastLen = 2; // Default setting for FastLen
            private int slowLen = 4; // Default setting for SlowLen
            private double gamma = 0.6000; // Default setting for Gamma
            private int trigger = 7; // Default setting for Trigger
		    private Z20101212LaguerreFilter lf = null;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "MACDVal"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "TriggerVal"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Bar, "HistogramVal"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            Overlay				= false;
        }

        protected override void OnStartUp() {
			if(fastLen > slowLen) {
			  int tmp = fastLen;
			  fastLen=slowLen;
			  slowLen = tmp;
			}
		  	lf = Z20101212LaguerreFilter(Input,gamma,slowLen);
		}

		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			lf.Update();
			
			double favg = 0;
			double savg = 0;
			for(int i = 0; i < fastLen; ++i) 
				favg += lf.filtered(i);
			savg = favg;
			for(int i = fastLen; i < slowLen; ++i)
				savg += lf.filtered(i);
			
			savg /= slowLen;
			favg /= fastLen;
			
			double diff = favg - savg;
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            MACDVal.Set(diff);
			if(CurrentBar > 1) {
               TriggerVal.Set( TriggerVal[1] + (2.0/(trigger+1.0))*(diff - TriggerVal[1]) );
               HistogramVal.Set(diff - TriggerVal[0]);
			} else {
			   TriggerVal.Set(diff);
			   HistogramVal.Set(0);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MACDVal
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TriggerVal
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HistogramVal
        {
            get { return Values[2]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int FastLen
        {
            get { return fastLen; }
            set { fastLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SlowLen
        {
            get { return slowLen; }
            set { slowLen = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Gamma
        {
            get { return gamma; }
            set { gamma = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Trigger
        {
            get { return trigger; }
            set { trigger = Math.Max(1, value); }
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
        private Z20101212LaguerreMACD[] cacheZ20101212LaguerreMACD = null;

        private static Z20101212LaguerreMACD checkZ20101212LaguerreMACD = new Z20101212LaguerreMACD();

        /// <summary>
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreMACD Z20101212LaguerreMACD(int fastLen, double gamma, int slowLen, int trigger)
        {
            return Z20101212LaguerreMACD(Input, fastLen, gamma, slowLen, trigger);
        }

        /// <summary>
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreMACD Z20101212LaguerreMACD(Data.IDataSeries input, int fastLen, double gamma, int slowLen, int trigger)
        {
            if (cacheZ20101212LaguerreMACD != null)
                for (int idx = 0; idx < cacheZ20101212LaguerreMACD.Length; idx++)
                    if (cacheZ20101212LaguerreMACD[idx].FastLen == fastLen && Math.Abs(cacheZ20101212LaguerreMACD[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreMACD[idx].SlowLen == slowLen && cacheZ20101212LaguerreMACD[idx].Trigger == trigger && cacheZ20101212LaguerreMACD[idx].EqualsInput(input))
                        return cacheZ20101212LaguerreMACD[idx];

            lock (checkZ20101212LaguerreMACD)
            {
                checkZ20101212LaguerreMACD.FastLen = fastLen;
                fastLen = checkZ20101212LaguerreMACD.FastLen;
                checkZ20101212LaguerreMACD.Gamma = gamma;
                gamma = checkZ20101212LaguerreMACD.Gamma;
                checkZ20101212LaguerreMACD.SlowLen = slowLen;
                slowLen = checkZ20101212LaguerreMACD.SlowLen;
                checkZ20101212LaguerreMACD.Trigger = trigger;
                trigger = checkZ20101212LaguerreMACD.Trigger;

                if (cacheZ20101212LaguerreMACD != null)
                    for (int idx = 0; idx < cacheZ20101212LaguerreMACD.Length; idx++)
                        if (cacheZ20101212LaguerreMACD[idx].FastLen == fastLen && Math.Abs(cacheZ20101212LaguerreMACD[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreMACD[idx].SlowLen == slowLen && cacheZ20101212LaguerreMACD[idx].Trigger == trigger && cacheZ20101212LaguerreMACD[idx].EqualsInput(input))
                            return cacheZ20101212LaguerreMACD[idx];

                Z20101212LaguerreMACD indicator = new Z20101212LaguerreMACD();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FastLen = fastLen;
                indicator.Gamma = gamma;
                indicator.SlowLen = slowLen;
                indicator.Trigger = trigger;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20101212LaguerreMACD[] tmp = new Z20101212LaguerreMACD[cacheZ20101212LaguerreMACD == null ? 1 : cacheZ20101212LaguerreMACD.Length + 1];
                if (cacheZ20101212LaguerreMACD != null)
                    cacheZ20101212LaguerreMACD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20101212LaguerreMACD = tmp;
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
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreMACD Z20101212LaguerreMACD(int fastLen, double gamma, int slowLen, int trigger)
        {
            return _indicator.Z20101212LaguerreMACD(Input, fastLen, gamma, slowLen, trigger);
        }

        /// <summary>
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreMACD Z20101212LaguerreMACD(Data.IDataSeries input, int fastLen, double gamma, int slowLen, int trigger)
        {
            return _indicator.Z20101212LaguerreMACD(input, fastLen, gamma, slowLen, trigger);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreMACD Z20101212LaguerreMACD(int fastLen, double gamma, int slowLen, int trigger)
        {
            return _indicator.Z20101212LaguerreMACD(Input, fastLen, gamma, slowLen, trigger);
        }

        /// <summary>
        /// laguerre macd
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreMACD Z20101212LaguerreMACD(Data.IDataSeries input, int fastLen, double gamma, int slowLen, int trigger)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20101212LaguerreMACD(input, fastLen, gamma, slowLen, trigger);
        }
    }
}
#endregion
