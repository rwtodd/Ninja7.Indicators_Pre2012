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
    public class zFastMIN : Indicator
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
				minval = Input[len-1];
				barsOld = len - 1;
			  	for(int i = (len - 2); i >= 0; --i) {
					if(minval >= Input[i]) {
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
        private zFastMIN[] cachezFastMIN = null;

        private static zFastMIN checkzFastMIN = new zFastMIN();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public zFastMIN zFastMIN(int period)
        {
            return zFastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public zFastMIN zFastMIN(Data.IDataSeries input, int period)
        {
            if (cachezFastMIN != null)
                for (int idx = 0; idx < cachezFastMIN.Length; idx++)
                    if (cachezFastMIN[idx].Period == period && cachezFastMIN[idx].EqualsInput(input))
                        return cachezFastMIN[idx];

            lock (checkzFastMIN)
            {
                checkzFastMIN.Period = period;
                period = checkzFastMIN.Period;

                if (cachezFastMIN != null)
                    for (int idx = 0; idx < cachezFastMIN.Length; idx++)
                        if (cachezFastMIN[idx].Period == period && cachezFastMIN[idx].EqualsInput(input))
                            return cachezFastMIN[idx];

                zFastMIN indicator = new zFastMIN();
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

                zFastMIN[] tmp = new zFastMIN[cachezFastMIN == null ? 1 : cachezFastMIN.Length + 1];
                if (cachezFastMIN != null)
                    cachezFastMIN.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezFastMIN = tmp;
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
        public Indicator.zFastMIN zFastMIN(int period)
        {
            return _indicator.zFastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.zFastMIN zFastMIN(Data.IDataSeries input, int period)
        {
            return _indicator.zFastMIN(input, period);
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
        public Indicator.zFastMIN zFastMIN(int period)
        {
            return _indicator.zFastMIN(Input, period);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.zFastMIN zFastMIN(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zFastMIN(input, period);
        }
    }
}
#endregion
