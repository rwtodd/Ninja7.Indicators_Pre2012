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
    public class Z20090722PriceProxy : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int tolerance = 1; // Default setting for Tolerance
		    private int lookback = 5;
		    private int windowLength = 3;
		
        // User defined variables (add any user defined variables below)
		    private Z20091120SortedWindow med;
		    private DataSeries maval;
		    private BoolSeries dirup;
		
            private double colorParam = 1; // Default setting for ColorParam
		    private Colorizer3Method colorMethod = Colorizer3Method.RisingFalling;
		    private Z20091221Colorizer3 colorizer;


        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "UpLine"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "DnLine"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Yellow), PlotStyle.Line, "NeutLine"));
            //CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			maval = new DataSeries(this);
			dirup = new BoolSeries(this);
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 3;
			Plots[2].Pen.Width = 3;
			colorizer = null;
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
			if(colorizer == null) {
			  colorizer = Z20091221Colorizer3(maval,colorMethod,colorParam);
			  med = Z20091120SortedWindow(Median,windowLength);
			}
			
			if(CurrentBar < (windowLength+1)) {
				maval.Set(Median[0]); 
				dirup.Set(true); 
				return; 
			}
			
			double slope = calcSlope(lookback);
			
			if( (med[0] > med[1]) &&
				(slope <= 0) ) {
			  maval.Set(maval[1]);
			} else if( (med[0] < med[1]) &&
				       (slope >= 0)) {
			  maval.Set(maval[1]);			
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
			    maval.Set( Math.Max(maval[1],med[0]) );	
			  } else {
				maval.Set( Math.Min(maval[1],med[0]) );
			  }
			
			  /* if( (maval[1] == maval[2]) &&
				  (med[1] != med[2]) ) {
				  maval.Set(Median[0]);
				  med.Set(Median[0]);
				} */
			}
			
			if(maval[0] > maval[1]) dirup.Set(true);
			else if(maval[0] < maval[1]) dirup.Set(false);
 			else dirup.Set(dirup[1]);			
			
			bool upbar = ( (Close[0] > Open[0]) || (Close[0] == High[0]));
			bool dnbar = ( (Close[0] < Open[0]) || (Close[0] == Low[0]) );
			
			if(!dirup[0] && dirup[1] && upbar) {
			  dirup.Set(true);
			  maval.Set(maval[1]);
			}
			
			if(dirup[0] && !dirup[1] && dnbar) {
			  dirup.Set(false);
			  maval.Set(maval[1]);
			}
			
			if(!dirup[0] && dirup[1] && (maval[1] <= (High[0]+tolerance*TickSize))) {
			  dirup.Set(true);
			  maval.Set(maval[1]);
			}
			
			if(dirup[0] && !dirup[1] && (maval[1] >= (Low[0]-tolerance*TickSize))) {
			  dirup.Set(false);
			  maval.Set(maval[1]);
			}
			
			if( (maval[0] > maval[1]) && dnbar) {
			  maval.Set(maval[1]);	
			}
			if( (maval[0] < maval[1]) && upbar) {
			  maval.Set(maval[1]);	
			}
						
			colorizer.drawColor(this,0,1,2);
			
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
            get { return maval; }
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
		
        [Description("Method of the 3-Color Colorizer")]
        [Category("Parameters")]
        public Colorizer3Method ColorMethod
        {
            get { return colorMethod; }
            set { colorMethod = value; }
        }

        [Description("Input to the 3-Color Colorizer")]
        [Category("Parameters")]
        public double ColorParam
        {
            get { return colorParam; }
            set { colorParam = value; }
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
        private Z20090722PriceProxy[] cacheZ20090722PriceProxy = null;

        private static Z20090722PriceProxy checkZ20090722PriceProxy = new Z20090722PriceProxy();

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Z20090722PriceProxy Z20090722PriceProxy(Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            return Z20090722PriceProxy(Input, colorMethod, colorParam, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Z20090722PriceProxy Z20090722PriceProxy(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            checkZ20090722PriceProxy.ColorMethod = colorMethod;
            colorMethod = checkZ20090722PriceProxy.ColorMethod;
            checkZ20090722PriceProxy.ColorParam = colorParam;
            colorParam = checkZ20090722PriceProxy.ColorParam;
            checkZ20090722PriceProxy.Lookback = lookback;
            lookback = checkZ20090722PriceProxy.Lookback;
            checkZ20090722PriceProxy.Tolerance = tolerance;
            tolerance = checkZ20090722PriceProxy.Tolerance;
            checkZ20090722PriceProxy.WindowLength = windowLength;
            windowLength = checkZ20090722PriceProxy.WindowLength;

            if (cacheZ20090722PriceProxy != null)
                for (int idx = 0; idx < cacheZ20090722PriceProxy.Length; idx++)
                    if (cacheZ20090722PriceProxy[idx].ColorMethod == colorMethod && Math.Abs(cacheZ20090722PriceProxy[idx].ColorParam - colorParam) <= double.Epsilon && cacheZ20090722PriceProxy[idx].Lookback == lookback && cacheZ20090722PriceProxy[idx].Tolerance == tolerance && cacheZ20090722PriceProxy[idx].WindowLength == windowLength && cacheZ20090722PriceProxy[idx].EqualsInput(input))
                        return cacheZ20090722PriceProxy[idx];

            Z20090722PriceProxy indicator = new Z20090722PriceProxy();
            indicator.BarsRequired = BarsRequired;
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.ColorMethod = colorMethod;
            indicator.ColorParam = colorParam;
            indicator.Lookback = lookback;
            indicator.Tolerance = tolerance;
            indicator.WindowLength = windowLength;
            indicator.SetUp();

            Z20090722PriceProxy[] tmp = new Z20090722PriceProxy[cacheZ20090722PriceProxy == null ? 1 : cacheZ20090722PriceProxy.Length + 1];
            if (cacheZ20090722PriceProxy != null)
                cacheZ20090722PriceProxy.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheZ20090722PriceProxy = tmp;
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
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20090722PriceProxy Z20090722PriceProxy(Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20090722PriceProxy(Input, colorMethod, colorParam, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20090722PriceProxy Z20090722PriceProxy(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20090722PriceProxy(input, colorMethod, colorParam, lookback, tolerance, windowLength);
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
        public Indicator.Z20090722PriceProxy Z20090722PriceProxy(Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            return _indicator.Z20090722PriceProxy(Input, colorMethod, colorParam, lookback, tolerance, windowLength);
        }

        /// <summary>
        /// The price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20090722PriceProxy Z20090722PriceProxy(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam, int lookback, int tolerance, int windowLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20090722PriceProxy(input, colorMethod, colorParam, lookback, tolerance, windowLength);
        }

    }
}
#endregion
