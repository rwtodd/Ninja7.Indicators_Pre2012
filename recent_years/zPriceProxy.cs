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
    /// price proxy
    /// </summary>
    [Description("price proxy")]
    public class zPriceProxy : Indicator
    {
        #region Variables
        // Wizard generated variables
			private HAMedians ham;
			private double hAAlpha = 1.0;
            private int lookback = 5; // Default setting for Lookback
            private int windowLength = 3; // Default setting for WindowLength
			private int tolerance = 1;
			private Color upColor = Color.Green;
			private Color dnColor = Color.Red;
			private zSortedWindow med;
			private bool dirUp;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "ProxyLine"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
				ham = HAMedians(hAAlpha);	
				med = zSortedWindow(ham.Value,windowLength);
		}
		
		private double calcSlope(int Period) {
			double	sumX	= (double) Period * (Period - 1) * 0.5;
			double	divisor = sumX * sumX - (double) Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			double	sumXY	= 0;

			double suminput = 0;
			for (int count = 0; count < Period && CurrentBar - count >= 0; count++) {
				sumXY += count * med[count];
				suminput += med[count];
			}
			
            return  ((double)Period * sumXY - sumX * suminput) / divisor;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			if(CurrentBar < (windowLength+1)) {
				ProxyLine.Set(med[0]); 
				dirUp = true;
				return; 
			}
		
			var slope = calcSlope(lookback);
			if( (med[0] > med[1]) &&
				(slope <= 0) ) {
			  ProxyLine.Set(ProxyLine[1]);
			} else if( (med[0] < med[1]) &&
				       (slope >= 0)) {
			  ProxyLine.Set(ProxyLine[1]);			
			} else {
				
			  // either everything is flat, or the
			  // med and slope agree...
			  bool assumeUp = (slope > 0);
			  if(slope == 0) {
				if(med[0] != med[1]) {
				  assumeUp = (med[0] > med[1]);
				} else {
				  assumeUp = dirUp;	
				}
			  }
			   	
			  if(assumeUp) {
			    ProxyLine.Set( Math.Max(ProxyLine[1],med[0]) );	
			  } else {
				ProxyLine.Set( Math.Min(ProxyLine[1],med[0]) );
			  }
			
		
			}
			
			var newDirUp = dirUp;
			if(ProxyLine[0] > ProxyLine[1]) newDirUp = true;
			else if(ProxyLine[0] < ProxyLine[1]) newDirUp = false;
 			
			bool upbar = ham.UpBar; // ( (Close[0] > Open[0]) || (Close[0] == High[0]));
			bool dnbar = ham.DnBar; // ( (Close[0] < Open[0]) || (Close[0] == Low[0]) );
			
			// exceptions that hold the proxy in place for a bit...
			if(  (!newDirUp && dirUp && upbar) ||
				 (newDirUp && !dirUp && dnbar) ||
				 ( !newDirUp && dirUp && (ProxyLine[1] <= (High[0]+tolerance*TickSize)) ) ||
                 ( newDirUp && !dirUp && (ProxyLine[1] >= (Low[0]-tolerance*TickSize))) ||
				 ((ProxyLine[0] > ProxyLine[1]) && ( dnbar || (High[0] < ProxyLine[0])) ) ||
				 ((ProxyLine[0] < ProxyLine[1]) && ( upbar || (Low[0]  > ProxyLine[0])) )
			  ) {
			  newDirUp = dirUp;
			  ProxyLine.Set(ProxyLine[1]);
			}
		    
			PlotColors[0][0] = (newDirUp?upColor:dnColor);
			dirUp = newDirUp;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ProxyLine
        {
            get { return Values[0]; }
        }

        [Description("lookback")]
        [GridCategory("Parameters")]
        public int Lookback
        {
            get { return lookback; }
            set { lookback = Math.Max(1, value); }
        }
        [Description("tolerance")]
        [GridCategory("Parameters")]
        public int Tolerance
        {
            get { return tolerance; }
            set { tolerance = Math.Max(1, value); }
        }
        [Description("alpha for smoothing")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return hAAlpha; }
            set { if(value > 1) { value = 2.0/(1.0+value); }
			      hAAlpha = value; }
        }

        [Description("window length")]
        [GridCategory("Parameters")]
        public int WindowLength
        {
            get { return windowLength; }
            set { windowLength = Math.Max(2, value); }
        }
		
      [XmlIgnore]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Down color")]
        public Color DnColor
        {
            get { return dnColor; }
            set { dnColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string DnColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(dnColor); }
            set { dnColor = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Up color")]
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string UpColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(upColor); }
            set { upColor = Gui.Design.SerializableColor.FromString(value); }
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
        private zPriceProxy[] cachezPriceProxy = null;

        private static zPriceProxy checkzPriceProxy = new zPriceProxy();

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public zPriceProxy zPriceProxy(double alpha, int lookback, int tolerance, int windowLength)
        {
            return zPriceProxy(Input, alpha, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public zPriceProxy zPriceProxy(Data.IDataSeries input, double alpha, int lookback, int tolerance, int windowLength)
        {
            if (cachezPriceProxy != null)
                for (int idx = 0; idx < cachezPriceProxy.Length; idx++)
                    if (Math.Abs(cachezPriceProxy[idx].Alpha - alpha) <= double.Epsilon && cachezPriceProxy[idx].Lookback == lookback && cachezPriceProxy[idx].Tolerance == tolerance && cachezPriceProxy[idx].WindowLength == windowLength && cachezPriceProxy[idx].EqualsInput(input))
                        return cachezPriceProxy[idx];

            lock (checkzPriceProxy)
            {
                checkzPriceProxy.Alpha = alpha;
                alpha = checkzPriceProxy.Alpha;
                checkzPriceProxy.Lookback = lookback;
                lookback = checkzPriceProxy.Lookback;
                checkzPriceProxy.Tolerance = tolerance;
                tolerance = checkzPriceProxy.Tolerance;
                checkzPriceProxy.WindowLength = windowLength;
                windowLength = checkzPriceProxy.WindowLength;

                if (cachezPriceProxy != null)
                    for (int idx = 0; idx < cachezPriceProxy.Length; idx++)
                        if (Math.Abs(cachezPriceProxy[idx].Alpha - alpha) <= double.Epsilon && cachezPriceProxy[idx].Lookback == lookback && cachezPriceProxy[idx].Tolerance == tolerance && cachezPriceProxy[idx].WindowLength == windowLength && cachezPriceProxy[idx].EqualsInput(input))
                            return cachezPriceProxy[idx];

                zPriceProxy indicator = new zPriceProxy();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.Lookback = lookback;
                indicator.Tolerance = tolerance;
                indicator.WindowLength = windowLength;
                Indicators.Add(indicator);
                indicator.SetUp();

                zPriceProxy[] tmp = new zPriceProxy[cachezPriceProxy == null ? 1 : cachezPriceProxy.Length + 1];
                if (cachezPriceProxy != null)
                    cachezPriceProxy.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezPriceProxy = tmp;
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
        /// price proxy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zPriceProxy zPriceProxy(double alpha, int lookback, int tolerance, int windowLength)
        {
            return _indicator.zPriceProxy(Input, alpha, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.zPriceProxy zPriceProxy(Data.IDataSeries input, double alpha, int lookback, int tolerance, int windowLength)
        {
            return _indicator.zPriceProxy(input, alpha, lookback, tolerance, windowLength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zPriceProxy zPriceProxy(double alpha, int lookback, int tolerance, int windowLength)
        {
            return _indicator.zPriceProxy(Input, alpha, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.zPriceProxy zPriceProxy(Data.IDataSeries input, double alpha, int lookback, int tolerance, int windowLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zPriceProxy(input, alpha, lookback, tolerance, windowLength);
        }
    }
}
#endregion
