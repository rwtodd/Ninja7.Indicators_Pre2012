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
    /// Faster MAX
    /// </summary>
    [Description("Faster MAX")]
    public class Z20100527FastMAX : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 5; // Default setting for Period
        // User defined variables (add any user defined variables below)
		    private int barsOld = 0;
		    private double maxval = -1;
		    private int lastBarSeen = -1;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "MAX"));
            Overlay				= true;
        }

		private void fullSearch(int len) {
				maxval = Input[0];
				barsOld = 0;
			  	for(int i = 1; i < len; ++i) {
					if(maxval < Input[i]) {
					    maxval = Input[i];
						barsOld = i;	
					}
				}				
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar > 1) {
				
				// notice when our current max got a bar older...
				if(lastBarSeen != CurrentBar) {
					++barsOld;
					lastBarSeen = CurrentBar;
				}
				
				// check to see if we have a new max...
				if(Input[0] >= maxval) {
				  maxval = Input[0];
				  barsOld = 0;
				}
				
				// must do a full search if the max val fell off the list...
				if(barsOld >= period) {
				  fullSearch(Math.Min(CurrentBar,period));	
				}
				
			} else {
			    // got to get bootstrapped...
				maxval = Input[0];
				barsOld = 0;
			}
							
            Value.Set(maxval);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MAX
        {
            get { return Values[0]; }
        }

        [Description("How far back to look")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
        private Z20100527FastMAX[] cacheZ20100527FastMAX = null;

        private static Z20100527FastMAX checkZ20100527FastMAX = new Z20100527FastMAX();

        /// <summary>
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        public Z20100527FastMAX Z20100527FastMAX(int period)
        {
            return Z20100527FastMAX(Input, period);
        }

        /// <summary>
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        public Z20100527FastMAX Z20100527FastMAX(Data.IDataSeries input, int period)
        {
            if (cacheZ20100527FastMAX != null)
                for (int idx = 0; idx < cacheZ20100527FastMAX.Length; idx++)
                    if (cacheZ20100527FastMAX[idx].Period == period && cacheZ20100527FastMAX[idx].EqualsInput(input))
                        return cacheZ20100527FastMAX[idx];

            lock (checkZ20100527FastMAX)
            {
                checkZ20100527FastMAX.Period = period;
                period = checkZ20100527FastMAX.Period;

                if (cacheZ20100527FastMAX != null)
                    for (int idx = 0; idx < cacheZ20100527FastMAX.Length; idx++)
                        if (cacheZ20100527FastMAX[idx].Period == period && cacheZ20100527FastMAX[idx].EqualsInput(input))
                            return cacheZ20100527FastMAX[idx];

                Z20100527FastMAX indicator = new Z20100527FastMAX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100527FastMAX[] tmp = new Z20100527FastMAX[cacheZ20100527FastMAX == null ? 1 : cacheZ20100527FastMAX.Length + 1];
                if (cacheZ20100527FastMAX != null)
                    cacheZ20100527FastMAX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100527FastMAX = tmp;
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
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100527FastMAX Z20100527FastMAX(int period)
        {
            return _indicator.Z20100527FastMAX(Input, period);
        }

        /// <summary>
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100527FastMAX Z20100527FastMAX(Data.IDataSeries input, int period)
        {
            return _indicator.Z20100527FastMAX(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100527FastMAX Z20100527FastMAX(int period)
        {
            return _indicator.Z20100527FastMAX(Input, period);
        }

        /// <summary>
        /// Faster MAX
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100527FastMAX Z20100527FastMAX(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100527FastMAX(input, period);
        }
    }
}
#endregion
