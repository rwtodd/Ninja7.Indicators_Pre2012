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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class Z20100527FastMIN : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 5; // Default setting for Period
        // User defined variables (add any user defined variables below)
		    private int barsOld = 0;
		    private double minval = -1;
		    private int lastBarSeen = -1;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "MIN"));
            Overlay				= true;
        }
		
		
		private void fullSearch(int len) {
				minval = Input[0];
				barsOld = 0;
			  	for(int i = 1; i < len; ++i) {
					if(minval > Input[i]) {
					    minval = Input[i];
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
				
				// check to see if we have a new minimum...
				if(Input[0] <= minval) {
				  minval = Input[0];
				  barsOld = 0;
				}
				
				// must do a full search if the min val fell off the list...
				if(barsOld >= period) {
				  fullSearch(Math.Min(CurrentBar,period));	
				}
				
			} else {
			    // got to get bootstrapped...
				minval = Input[0];
				barsOld = 0;
			}						
			
            Value.Set(minval);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MIN
        {
            get { return Values[0]; }
        }

        [Description("Period")]
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
        private Z20100527FastMIN[] cacheZ20100527FastMIN = null;

        private static Z20100527FastMIN checkZ20100527FastMIN = new Z20100527FastMIN();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Z20100527FastMIN Z20100527FastMIN(int period)
        {
            return Z20100527FastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Z20100527FastMIN Z20100527FastMIN(Data.IDataSeries input, int period)
        {
            if (cacheZ20100527FastMIN != null)
                for (int idx = 0; idx < cacheZ20100527FastMIN.Length; idx++)
                    if (cacheZ20100527FastMIN[idx].Period == period && cacheZ20100527FastMIN[idx].EqualsInput(input))
                        return cacheZ20100527FastMIN[idx];

            lock (checkZ20100527FastMIN)
            {
                checkZ20100527FastMIN.Period = period;
                period = checkZ20100527FastMIN.Period;

                if (cacheZ20100527FastMIN != null)
                    for (int idx = 0; idx < cacheZ20100527FastMIN.Length; idx++)
                        if (cacheZ20100527FastMIN[idx].Period == period && cacheZ20100527FastMIN[idx].EqualsInput(input))
                            return cacheZ20100527FastMIN[idx];

                Z20100527FastMIN indicator = new Z20100527FastMIN();
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

                Z20100527FastMIN[] tmp = new Z20100527FastMIN[cacheZ20100527FastMIN == null ? 1 : cacheZ20100527FastMIN.Length + 1];
                if (cacheZ20100527FastMIN != null)
                    cacheZ20100527FastMIN.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100527FastMIN = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100527FastMIN Z20100527FastMIN(int period)
        {
            return _indicator.Z20100527FastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100527FastMIN Z20100527FastMIN(Data.IDataSeries input, int period)
        {
            return _indicator.Z20100527FastMIN(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100527FastMIN Z20100527FastMIN(int period)
        {
            return _indicator.Z20100527FastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100527FastMIN Z20100527FastMIN(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100527FastMIN(input, period);
        }
    }
}
#endregion
