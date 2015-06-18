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
    /// The price proxy
    /// </summary>
    [Description("The price proxy")]
    public class Z20131018ProxyOneLine : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int tolerance = 1; // Default setting for Tolerance
		    private int lookback = 5;
		    private int windowLength = 3;
		
        // User defined variables (add any user defined variables below)
		    private Z20091120SortedWindow med;
		    private BoolSeries dirup;
		


        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "ProxLine"));
            //CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			dirup = new BoolSeries(this);
			Plots[0].Pen.Width = 3;
			med = null;
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
			if(med == null) {
			  med = Z20091120SortedWindow(Median,windowLength);
			}
			
			if(CurrentBar < (windowLength+1)) {
				Value.Set(Median[0]); 
				dirup.Set(true); 
				return; 
			}
			
			double slope = calcSlope(lookback);
			
			if( (med[0] > med[1]) &&
				(slope <= 0) ) {
			  Value.Set(Value[1]);
			} else if( (med[0] < med[1]) &&
				       (slope >= 0)) {
			  Value.Set(Value[1]);			
			} else {
				
			  // either everything is flat, or the
			  // med and slope agree...
			  bool assumeUp = (slope > 0);
			  if(slope == 0) {
				if(med[0] != med[1]) {
				  assumeUp = (med[0] > med[1]);
				} else {
				  assumeUp = dirup[1];	
				}
			  }
			   	
			  if(assumeUp) {
			    Value.Set( Math.Max(Value[1],med[0]) );	
			  } else {
				Value.Set( Math.Min(Value[1],med[0]) );
			  }
			
			  /* if( (Value[1] == Value[2]) &&
				  (med[1] != med[2]) ) {
				  Value.Set(Median[0]);
				  med.Set(Median[0]);
				} */
			}
			
			if(Value[0] > Value[1]) dirup.Set(true);
			else if(Value[0] < Value[1]) dirup.Set(false);
 			else dirup.Set(dirup[1]);			
			
			bool upbar = ( (Close[0] > Open[0]) || (Close[0] == High[0]));
			bool dnbar = ( (Close[0] < Open[0]) || (Close[0] == Low[0]) );
			
			if(!dirup[0] && dirup[1] && upbar) {
			  dirup.Set(true);
			  Value.Set(Value[1]);
			}
			
			if(dirup[0] && !dirup[1] && dnbar) {
			  dirup.Set(false);
			  Value.Set(Value[1]);
			}
			
			if(!dirup[0] && dirup[1] && (Value[1] <= (High[0]+tolerance*TickSize))) {
			  dirup.Set(true);
			  Value.Set(Value[1]);
			}
			
			if(dirup[0] && !dirup[1] && (Value[1] >= (Low[0]-tolerance*TickSize))) {
			  dirup.Set(false);
			  Value.Set(Value[1]);
			}
			
			if( (Value[0] > Value[1]) && dnbar) {
			  Value.Set(Value[1]);	
			}
			if( (Value[0] < Value[1]) && upbar) {
			  Value.Set(Value[1]);	
			}
						
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpLine
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DnLine
        {
            get { return Values[1]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries NeutLine
        {
            get { return Values[2]; }
        }
		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ProxyVal
        {
            get { return Value; }
        }

		
        [Description("Tolerance in Ticks")]
        [Category("Parameters")]
        public int Tolerance
        {
            get { return tolerance; }
            set { tolerance = Math.Max(0, value); }
        }
        [Description("Lookback Period")]
        [Category("Parameters")]
        public int Lookback
        {
            get { return lookback; }
            set { lookback = Math.Max(1, value); }
        }
		
		[Description("Window Length")]
        [Category("Parameters")]
        public int WindowLength
        {
            get { return windowLength; }
            set { windowLength = Math.Max(1, value); }
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
        private Z20131018ProxyOneLine[] cacheZ20131018ProxyOneLine = null;

        private static Z20131018ProxyOneLine checkZ20131018ProxyOneLine = new Z20131018ProxyOneLine();

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Z20131018ProxyOneLine Z20131018ProxyOneLine(int lookback, int tolerance, int windowLength)
        {
            return Z20131018ProxyOneLine(Input, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Z20131018ProxyOneLine Z20131018ProxyOneLine(Data.IDataSeries input, int lookback, int tolerance, int windowLength)
        {
            if (cacheZ20131018ProxyOneLine != null)
                for (int idx = 0; idx < cacheZ20131018ProxyOneLine.Length; idx++)
                    if (cacheZ20131018ProxyOneLine[idx].Lookback == lookback && cacheZ20131018ProxyOneLine[idx].Tolerance == tolerance && cacheZ20131018ProxyOneLine[idx].WindowLength == windowLength && cacheZ20131018ProxyOneLine[idx].EqualsInput(input))
                        return cacheZ20131018ProxyOneLine[idx];

            lock (checkZ20131018ProxyOneLine)
            {
                checkZ20131018ProxyOneLine.Lookback = lookback;
                lookback = checkZ20131018ProxyOneLine.Lookback;
                checkZ20131018ProxyOneLine.Tolerance = tolerance;
                tolerance = checkZ20131018ProxyOneLine.Tolerance;
                checkZ20131018ProxyOneLine.WindowLength = windowLength;
                windowLength = checkZ20131018ProxyOneLine.WindowLength;

                if (cacheZ20131018ProxyOneLine != null)
                    for (int idx = 0; idx < cacheZ20131018ProxyOneLine.Length; idx++)
                        if (cacheZ20131018ProxyOneLine[idx].Lookback == lookback && cacheZ20131018ProxyOneLine[idx].Tolerance == tolerance && cacheZ20131018ProxyOneLine[idx].WindowLength == windowLength && cacheZ20131018ProxyOneLine[idx].EqualsInput(input))
                            return cacheZ20131018ProxyOneLine[idx];

                Z20131018ProxyOneLine indicator = new Z20131018ProxyOneLine();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Lookback = lookback;
                indicator.Tolerance = tolerance;
                indicator.WindowLength = windowLength;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20131018ProxyOneLine[] tmp = new Z20131018ProxyOneLine[cacheZ20131018ProxyOneLine == null ? 1 : cacheZ20131018ProxyOneLine.Length + 1];
                if (cacheZ20131018ProxyOneLine != null)
                    cacheZ20131018ProxyOneLine.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20131018ProxyOneLine = tmp;
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
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20131018ProxyOneLine Z20131018ProxyOneLine(int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20131018ProxyOneLine(Input, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20131018ProxyOneLine Z20131018ProxyOneLine(Data.IDataSeries input, int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20131018ProxyOneLine(input, lookback, tolerance, windowLength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20131018ProxyOneLine Z20131018ProxyOneLine(int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20131018ProxyOneLine(Input, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20131018ProxyOneLine Z20131018ProxyOneLine(Data.IDataSeries input, int lookback, int tolerance, int windowLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20131018ProxyOneLine(input, lookback, tolerance, windowLength);
        }
    }
}
#endregion
