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
    /// sorted window
    /// </summary>
    [Description("sorted window")]
    public class zSortedWindow : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int length = 10; // Default setting for Length
		    private double[] prices;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Median"));
            Overlay				= true;
        }
		protected override void OnStartUp() {
			prices = new double[length];
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar > length) {
				var todel = Input[length];
				var toadd = Input[0];
				var endIndx = length - 1;

				for(int i = 0; i < length; ++i) {
				  if(prices[i] == todel) {
				    if( (i == endIndx) ||
						 (prices[i+1] >= toadd) ) {
						prices[i] = toadd;
						break;
					}
					prices[i] = prices[i+1];
					todel = prices[i];
				  } 
				  else if(prices[i] > toadd) {
				    var tmp = prices[i];
					prices[i] = toadd;
					toadd = tmp;
				  }
				}
			}
			else if(CurrentBar < length) { 
			 	Median.Set(Input[0]);	return;
			}
			else { // (CurrentBar == length) {
				for(int j = 0; j < length; j++) {
						prices[j] = Input[j];
				}
				Array.Sort(prices);
			} 
			
			if((length & 1) == 1) Median.Set(prices[length/2]);
			else Median.Set(0.5*(prices[length/2]+prices[length/2-1]));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Median
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double[] Window
        {
            get { return prices; }
        }

        [Description("length")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(2, value); }
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
        private zSortedWindow[] cachezSortedWindow = null;

        private static zSortedWindow checkzSortedWindow = new zSortedWindow();

        /// <summary>
        /// sorted window
        /// </summary>
        /// <returns></returns>
        public zSortedWindow zSortedWindow(int length)
        {
            return zSortedWindow(Input, length);
        }

        /// <summary>
        /// sorted window
        /// </summary>
        /// <returns></returns>
        public zSortedWindow zSortedWindow(Data.IDataSeries input, int length)
        {
            if (cachezSortedWindow != null)
                for (int idx = 0; idx < cachezSortedWindow.Length; idx++)
                    if (cachezSortedWindow[idx].Length == length && cachezSortedWindow[idx].EqualsInput(input))
                        return cachezSortedWindow[idx];

            lock (checkzSortedWindow)
            {
                checkzSortedWindow.Length = length;
                length = checkzSortedWindow.Length;

                if (cachezSortedWindow != null)
                    for (int idx = 0; idx < cachezSortedWindow.Length; idx++)
                        if (cachezSortedWindow[idx].Length == length && cachezSortedWindow[idx].EqualsInput(input))
                            return cachezSortedWindow[idx];

                zSortedWindow indicator = new zSortedWindow();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                zSortedWindow[] tmp = new zSortedWindow[cachezSortedWindow == null ? 1 : cachezSortedWindow.Length + 1];
                if (cachezSortedWindow != null)
                    cachezSortedWindow.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezSortedWindow = tmp;
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
        /// sorted window
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSortedWindow zSortedWindow(int length)
        {
            return _indicator.zSortedWindow(Input, length);
        }

        /// <summary>
        /// sorted window
        /// </summary>
        /// <returns></returns>
        public Indicator.zSortedWindow zSortedWindow(Data.IDataSeries input, int length)
        {
            return _indicator.zSortedWindow(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// sorted window
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSortedWindow zSortedWindow(int length)
        {
            return _indicator.zSortedWindow(Input, length);
        }

        /// <summary>
        /// sorted window
        /// </summary>
        /// <returns></returns>
        public Indicator.zSortedWindow zSortedWindow(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zSortedWindow(input, length);
        }
    }
}
#endregion
