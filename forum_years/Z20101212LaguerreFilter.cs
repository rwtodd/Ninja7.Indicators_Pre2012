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
    /// Laguerre
    /// </summary>
    [Description("Laguerre")]
    public class Z20101212LaguerreFilter : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double gamma = 0.600; // Default setting for Gamma
            private int len = 4; // Default setting for Length
		    private double[] l0,l1;
		    private int lastSeenBar = -1;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Avg"));
            Overlay				= true;
        }
		
		protected override void OnStartUp() {
			l0 = new double[len];
			l1 = new double[len];
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar <= 1) {
			  for(int i = 0; i < len; ++i) {
				l0[i] = Input[0];
				l1[i] = Input[0];
				lastSeenBar = CurrentBar;
			  }
			}
			
			if(CurrentBar != lastSeenBar) {
		      for(int i = 0; i < len; ++i) 
			    l1[i] = l0[i]; // remember previous bar value...

			  lastSeenBar = CurrentBar;
			}

			l0[0] = (1-gamma)*Input[0] + gamma*l1[0];
			var avg = l0[0];
			for(int i = 1; i < len; ++i) { 
				l0[i] = -gamma*l0[i-1] + l1[i-1] + gamma*l1[i];
				avg += l0[i];
			}
	
			avg /= len;
			
            Value.Set(avg);
        }
		
		public double filtered(int barsAgo) {
		   return l0[barsAgo];	
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Avg
        {
            get { return Values[0]; }
        }

        [Description("Gamma")]
        [GridCategory("Parameters")]
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        [Description("Number of bars to calculate")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return len; }
            set { len = Math.Max(2, value); }
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
        private Z20101212LaguerreFilter[] cacheZ20101212LaguerreFilter = null;

        private static Z20101212LaguerreFilter checkZ20101212LaguerreFilter = new Z20101212LaguerreFilter();

        /// <summary>
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreFilter Z20101212LaguerreFilter(double gamma, int length)
        {
            return Z20101212LaguerreFilter(Input, gamma, length);
        }

        /// <summary>
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreFilter Z20101212LaguerreFilter(Data.IDataSeries input, double gamma, int length)
        {
            if (cacheZ20101212LaguerreFilter != null)
                for (int idx = 0; idx < cacheZ20101212LaguerreFilter.Length; idx++)
                    if (Math.Abs(cacheZ20101212LaguerreFilter[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreFilter[idx].Length == length && cacheZ20101212LaguerreFilter[idx].EqualsInput(input))
                        return cacheZ20101212LaguerreFilter[idx];

            lock (checkZ20101212LaguerreFilter)
            {
                checkZ20101212LaguerreFilter.Gamma = gamma;
                gamma = checkZ20101212LaguerreFilter.Gamma;
                checkZ20101212LaguerreFilter.Length = length;
                length = checkZ20101212LaguerreFilter.Length;

                if (cacheZ20101212LaguerreFilter != null)
                    for (int idx = 0; idx < cacheZ20101212LaguerreFilter.Length; idx++)
                        if (Math.Abs(cacheZ20101212LaguerreFilter[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreFilter[idx].Length == length && cacheZ20101212LaguerreFilter[idx].EqualsInput(input))
                            return cacheZ20101212LaguerreFilter[idx];

                Z20101212LaguerreFilter indicator = new Z20101212LaguerreFilter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Gamma = gamma;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20101212LaguerreFilter[] tmp = new Z20101212LaguerreFilter[cacheZ20101212LaguerreFilter == null ? 1 : cacheZ20101212LaguerreFilter.Length + 1];
                if (cacheZ20101212LaguerreFilter != null)
                    cacheZ20101212LaguerreFilter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20101212LaguerreFilter = tmp;
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
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreFilter Z20101212LaguerreFilter(double gamma, int length)
        {
            return _indicator.Z20101212LaguerreFilter(Input, gamma, length);
        }

        /// <summary>
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreFilter Z20101212LaguerreFilter(Data.IDataSeries input, double gamma, int length)
        {
            return _indicator.Z20101212LaguerreFilter(input, gamma, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreFilter Z20101212LaguerreFilter(double gamma, int length)
        {
            return _indicator.Z20101212LaguerreFilter(Input, gamma, length);
        }

        /// <summary>
        /// Laguerre
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreFilter Z20101212LaguerreFilter(Data.IDataSeries input, double gamma, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20101212LaguerreFilter(input, gamma, length);
        }
    }
}
#endregion
