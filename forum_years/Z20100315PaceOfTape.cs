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
    /// cunparis' indicator...
    /// </summary>
    [Description("cunparis' indicator...")]
    public class Z20100315PaceOfTape : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double period = 30; // Default setting for Period
            private int threshold = 1500; // Default setting for Threshold
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.SkyBlue), PlotStyle.Bar, "Normal"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Salmon), PlotStyle.Bar, "High"));
            CalculateOnBarClose	= true;
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int pace = 0;
			int i = 0;
			while(i < CurrentBar) {
			  TimeSpan ts = Time[0] - Time[i];
			  if(ts.TotalSeconds < period) pace += Bars.Period.Value;
			  else break;
			  ++i;	
			}
			if(pace >= threshold) 
				High.Set(pace);
		    else 
				Normal.Set(pace);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Normal
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries High
        {
            get { return Values[1]; }
        }

        [Description("How long in seconds to look back")]
        [GridCategory("Parameters")]
        public double Period
        {
            get { return period; }
            set { period = Math.Max(0, value); }
        }

        [Description("How many ticks per period is considered high")]
        [GridCategory("Parameters")]
        public int Threshold
        {
            get { return threshold; }
            set { threshold = Math.Max(1, value); }
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
        private Z20100315PaceOfTape[] cacheZ20100315PaceOfTape = null;

        private static Z20100315PaceOfTape checkZ20100315PaceOfTape = new Z20100315PaceOfTape();

        /// <summary>
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        public Z20100315PaceOfTape Z20100315PaceOfTape(double period, int threshold)
        {
            return Z20100315PaceOfTape(Input, period, threshold);
        }

        /// <summary>
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        public Z20100315PaceOfTape Z20100315PaceOfTape(Data.IDataSeries input, double period, int threshold)
        {
            if (cacheZ20100315PaceOfTape != null)
                for (int idx = 0; idx < cacheZ20100315PaceOfTape.Length; idx++)
                    if (Math.Abs(cacheZ20100315PaceOfTape[idx].Period - period) <= double.Epsilon && cacheZ20100315PaceOfTape[idx].Threshold == threshold && cacheZ20100315PaceOfTape[idx].EqualsInput(input))
                        return cacheZ20100315PaceOfTape[idx];

            lock (checkZ20100315PaceOfTape)
            {
                checkZ20100315PaceOfTape.Period = period;
                period = checkZ20100315PaceOfTape.Period;
                checkZ20100315PaceOfTape.Threshold = threshold;
                threshold = checkZ20100315PaceOfTape.Threshold;

                if (cacheZ20100315PaceOfTape != null)
                    for (int idx = 0; idx < cacheZ20100315PaceOfTape.Length; idx++)
                        if (Math.Abs(cacheZ20100315PaceOfTape[idx].Period - period) <= double.Epsilon && cacheZ20100315PaceOfTape[idx].Threshold == threshold && cacheZ20100315PaceOfTape[idx].EqualsInput(input))
                            return cacheZ20100315PaceOfTape[idx];

                Z20100315PaceOfTape indicator = new Z20100315PaceOfTape();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100315PaceOfTape[] tmp = new Z20100315PaceOfTape[cacheZ20100315PaceOfTape == null ? 1 : cacheZ20100315PaceOfTape.Length + 1];
                if (cacheZ20100315PaceOfTape != null)
                    cacheZ20100315PaceOfTape.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100315PaceOfTape = tmp;
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
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100315PaceOfTape Z20100315PaceOfTape(double period, int threshold)
        {
            return _indicator.Z20100315PaceOfTape(Input, period, threshold);
        }

        /// <summary>
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100315PaceOfTape Z20100315PaceOfTape(Data.IDataSeries input, double period, int threshold)
        {
            return _indicator.Z20100315PaceOfTape(input, period, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100315PaceOfTape Z20100315PaceOfTape(double period, int threshold)
        {
            return _indicator.Z20100315PaceOfTape(Input, period, threshold);
        }

        /// <summary>
        /// cunparis' indicator...
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100315PaceOfTape Z20100315PaceOfTape(Data.IDataSeries input, double period, int threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100315PaceOfTape(input, period, threshold);
        }
    }
}
#endregion
