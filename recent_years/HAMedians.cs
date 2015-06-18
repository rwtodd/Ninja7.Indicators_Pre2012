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
    /// Heikan-Ashi bar levels
    /// </summary>
    [Description("Heikan-Ashi bar levels")]
    public class HAMedians : Indicator
    {
        #region Variables
        // Wizard generated variables
		private double alpha = 0.9;
		private double haopen, haclose;
        // User defined variables (add any user defined variables below)
		private class dexpma {
		  private double ema1, ema2;
		  private double atauconst;
		  private double alpha;
		  private int tau;
		  public dexpma(double a, int t) /* alpha, tau */ {
				alpha = a; tau = t;
			    atauconst = (alpha*tau)/(1.0-alpha);
		  }
		  public void init(double val) {
		    ema1 = val; ema2 = val;	
		  }
		  public double next(double val) {
			ema1 = ema1 + alpha*(val-ema1);
			ema2 = ema2 + alpha*(ema1-ema2);
			return  (2.0+atauconst)*ema1 - (1.0+atauconst)*ema2;
		  }
		};

		private dexpma highs, lows, opens, closes;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "HAMedian"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			if(alpha != 1.0) {
				opens = new dexpma(alpha,1);
				highs = new dexpma(alpha,1);
				lows = new dexpma(alpha,1);
				closes = new dexpma(alpha,1);			
			}
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(alpha == 1) {
					haopen = (CurrentBar > 0)? ((haopen + haclose) * 0.5) : Open[0]; 
					haclose = (High[0]+Low[0]+Close[0]+Open[0])*0.25;

					var hahigh = Math.Max(High[0],haopen);
					var halow = Math.Min(Low[0],haopen);
					
					HAMedian.Set((hahigh + halow) * 0.5);
			} else {
				
					if(CurrentBar == 0) {
						opens.init(Open[0]);
						closes.init(Close[0]);
						highs.init(High[0]);
						lows.init(Low[0]);
						haopen = Open[0];
						haclose = (High[0]+Low[0]+Close[0]+Open[0])*0.25;
						HAMedian.Set((High[0]+Low[0])*0.5);
						return;
					}

					var curhigh = highs.next(High[0]);
					var curlow = lows.next(Low[0]);
					var curopen = opens.next(Open[0]);
					var curclose = closes.next(Close[0]);
					haopen = (haopen + haclose) * 0.5;
					haclose = (curhigh+curlow+curopen+curclose)*0.25;
					var hahigh = Math.Max(curhigh,haopen);
					var halow = Math.Min(curlow,haopen);
					HAMedian.Set((hahigh + halow)*0.5);
			}
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HAMedian
        {
            get { return Values[0]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool UpBar
        {
            get { return ((haopen <= haclose) || 
					      ( (Close[0] > Open[0]) || (Close[0] == High[0])) 
					     ); 
			}
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool DnBar
        {
            get { return ((haopen >= haclose) ||
			        	  ( (Close[0] < Open[0]) || (Close[0] == Low[0]) )
					);
		    }
        }
		
        [Description("alpha for smoothing")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { if(value > 1) { value = 2.0/(1.0+value); }
			      alpha = value; }
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
        private HAMedians[] cacheHAMedians = null;

        private static HAMedians checkHAMedians = new HAMedians();

        /// <summary>
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        public HAMedians HAMedians(double alpha)
        {
            return HAMedians(Input, alpha);
        }

        /// <summary>
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        public HAMedians HAMedians(Data.IDataSeries input, double alpha)
        {
            if (cacheHAMedians != null)
                for (int idx = 0; idx < cacheHAMedians.Length; idx++)
                    if (Math.Abs(cacheHAMedians[idx].Alpha - alpha) <= double.Epsilon && cacheHAMedians[idx].EqualsInput(input))
                        return cacheHAMedians[idx];

            lock (checkHAMedians)
            {
                checkHAMedians.Alpha = alpha;
                alpha = checkHAMedians.Alpha;

                if (cacheHAMedians != null)
                    for (int idx = 0; idx < cacheHAMedians.Length; idx++)
                        if (Math.Abs(cacheHAMedians[idx].Alpha - alpha) <= double.Epsilon && cacheHAMedians[idx].EqualsInput(input))
                            return cacheHAMedians[idx];

                HAMedians indicator = new HAMedians();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                Indicators.Add(indicator);
                indicator.SetUp();

                HAMedians[] tmp = new HAMedians[cacheHAMedians == null ? 1 : cacheHAMedians.Length + 1];
                if (cacheHAMedians != null)
                    cacheHAMedians.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHAMedians = tmp;
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
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HAMedians HAMedians(double alpha)
        {
            return _indicator.HAMedians(Input, alpha);
        }

        /// <summary>
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        public Indicator.HAMedians HAMedians(Data.IDataSeries input, double alpha)
        {
            return _indicator.HAMedians(input, alpha);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HAMedians HAMedians(double alpha)
        {
            return _indicator.HAMedians(Input, alpha);
        }

        /// <summary>
        /// Heikan-Ashi bar levels
        /// </summary>
        /// <returns></returns>
        public Indicator.HAMedians HAMedians(Data.IDataSeries input, double alpha)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HAMedians(input, alpha);
        }
    }
}
#endregion
