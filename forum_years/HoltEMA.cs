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
    /// the Holt trend-adjusted EMA richard@movethemarkets.com
    /// </summary>
    [Description("the Holt trend-adjusted EMA richard@movethemarkets.com")]
    public class HoltEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha = 20.000; // Default setting for Alpha
            private double gamma = 20; // Default setting for Gamma
			private double a2, g2;
			private double b;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "HEMA"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			a2 = alpha; g2 = gamma;
			if(a2>1.0) a2 = 2.0/(a2+1.0);
			if(g2>1.0) g2 = 2.0/(g2+1.0);
			b = 0;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {  Value.Set(Input[0]); return; }
			
			Value.Set( Value[1] + b + a2*(Input[0] - Value[1] - b) );
			b = b + g2*(Value[0]-Value[1]-b);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HEMA
        {
            get { return Values[0]; }
        }

        [Description("EMA speed")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { alpha = Math.Max(1, value); }
        }

        [Description("Trend Adjustment speed")]
        [GridCategory("Parameters")]
        public double Gamma
        {
            get { return gamma; }
            set { gamma = Math.Max(1, value); }
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
        private HoltEMA[] cacheHoltEMA = null;

        private static HoltEMA checkHoltEMA = new HoltEMA();

        /// <summary>
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public HoltEMA HoltEMA(double alpha, double gamma)
        {
            return HoltEMA(Input, alpha, gamma);
        }

        /// <summary>
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public HoltEMA HoltEMA(Data.IDataSeries input, double alpha, double gamma)
        {
            if (cacheHoltEMA != null)
                for (int idx = 0; idx < cacheHoltEMA.Length; idx++)
                    if (Math.Abs(cacheHoltEMA[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cacheHoltEMA[idx].Gamma - gamma) <= double.Epsilon && cacheHoltEMA[idx].EqualsInput(input))
                        return cacheHoltEMA[idx];

            lock (checkHoltEMA)
            {
                checkHoltEMA.Alpha = alpha;
                alpha = checkHoltEMA.Alpha;
                checkHoltEMA.Gamma = gamma;
                gamma = checkHoltEMA.Gamma;

                if (cacheHoltEMA != null)
                    for (int idx = 0; idx < cacheHoltEMA.Length; idx++)
                        if (Math.Abs(cacheHoltEMA[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cacheHoltEMA[idx].Gamma - gamma) <= double.Epsilon && cacheHoltEMA[idx].EqualsInput(input))
                            return cacheHoltEMA[idx];

                HoltEMA indicator = new HoltEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.Gamma = gamma;
                Indicators.Add(indicator);
                indicator.SetUp();

                HoltEMA[] tmp = new HoltEMA[cacheHoltEMA == null ? 1 : cacheHoltEMA.Length + 1];
                if (cacheHoltEMA != null)
                    cacheHoltEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHoltEMA = tmp;
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
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HoltEMA HoltEMA(double alpha, double gamma)
        {
            return _indicator.HoltEMA(Input, alpha, gamma);
        }

        /// <summary>
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Indicator.HoltEMA HoltEMA(Data.IDataSeries input, double alpha, double gamma)
        {
            return _indicator.HoltEMA(input, alpha, gamma);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HoltEMA HoltEMA(double alpha, double gamma)
        {
            return _indicator.HoltEMA(Input, alpha, gamma);
        }

        /// <summary>
        /// the Holt trend-adjusted EMA richard@movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Indicator.HoltEMA HoltEMA(Data.IDataSeries input, double alpha, double gamma)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HoltEMA(input, alpha, gamma);
        }
    }
}
#endregion
