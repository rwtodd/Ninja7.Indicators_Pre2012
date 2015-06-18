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
    /// Richard Todd www.movethemarkets.com efficiently tracks sorted price over a rolling window.  
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  ")]
    public class Z20091120SortedWindow : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int length = 10; // Default setting for Length
        // User defined variables (add any user defined variables below)
		    private double[] prices;
		    private double[] oldprices;
			private int lastSeen;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Median"));
            // CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= true;
			prices = new double[length];
			oldprices = null;
			lastSeen = -1;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < length) return;
			if(CurrentBar == length) {
 			  for(int j= 0; j < length; ++j) {
			    prices[j] = Input[j];	
			  }
			
  			  Array.Sort(prices);
			  return;
			}
			
			if(!CalculateOnBarClose) {
			  if(oldprices == null)	oldprices = (double[])prices.Clone();
			  if(CurrentBar != lastSeen) {
			     lastSeen = CurrentBar;
			     // this is a new bar, so copy prices to oldprices...
				 prices.CopyTo(oldprices,0);
			  } else {
				// this is the same bar as the last tick... copy over prices with oldprices...
				oldprices.CopyTo(prices,0);
			  }
			}
						
			double todel = Input[length];
			double toadd = Input[0];
			double sum = 0;
			for(int i = 0; i < length; ++i) {
			  if(prices[i] == todel) {
			     if( (i == (length - 1)) ||
					 (prices[i+1] >= toadd) ) {
				   prices[i] = toadd;
				   break;
				 }
				 prices[i] = prices[i+1];
				 todel = prices[i];
			  } else if(prices[i] > toadd) {
			    double tmp = prices[i];
				prices[i] = toadd;
				toadd = tmp;
			  }
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
        public double[] SortedValues
        {
            get { return prices; }
        }

        [Description("how many bars is the window?")]
        [Category("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
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
        private Z20091120SortedWindow[] cacheZ20091120SortedWindow = null;

        private static Z20091120SortedWindow checkZ20091120SortedWindow = new Z20091120SortedWindow();

        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        public Z20091120SortedWindow Z20091120SortedWindow(int length)
        {
            return Z20091120SortedWindow(Input, length);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        public Z20091120SortedWindow Z20091120SortedWindow(Data.IDataSeries input, int length)
        {
            checkZ20091120SortedWindow.Length = length;
            length = checkZ20091120SortedWindow.Length;

            if (cacheZ20091120SortedWindow != null)
                for (int idx = 0; idx < cacheZ20091120SortedWindow.Length; idx++)
                    if (cacheZ20091120SortedWindow[idx].Length == length && cacheZ20091120SortedWindow[idx].EqualsInput(input))
                        return cacheZ20091120SortedWindow[idx];

            Z20091120SortedWindow indicator = new Z20091120SortedWindow();
            indicator.BarsRequired = BarsRequired;
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Length = length;
            indicator.SetUp();

            Z20091120SortedWindow[] tmp = new Z20091120SortedWindow[cacheZ20091120SortedWindow == null ? 1 : cacheZ20091120SortedWindow.Length + 1];
            if (cacheZ20091120SortedWindow != null)
                cacheZ20091120SortedWindow.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheZ20091120SortedWindow = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091120SortedWindow Z20091120SortedWindow(int length)
        {
            return _indicator.Z20091120SortedWindow(Input, length);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091120SortedWindow Z20091120SortedWindow(Data.IDataSeries input, int length)
        {
            return _indicator.Z20091120SortedWindow(input, length);
        }

    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091120SortedWindow Z20091120SortedWindow(int length)
        {
            return _indicator.Z20091120SortedWindow(Input, length);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com efficiently tracks sorted input over a rolling window.  
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091120SortedWindow Z20091120SortedWindow(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091120SortedWindow(input, length);
        }

    }
}
#endregion
