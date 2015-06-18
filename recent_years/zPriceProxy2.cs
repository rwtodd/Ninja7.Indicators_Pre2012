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
    public class zPriceProxy2 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int lookback = 5; // Default setting for Lookback
			private double tolerance = 1;
			private RWT_HA.PrimaryOHLC primType = RWT_HA.PrimaryOHLC.BARS;

			private RWT_HA.SecondaryOHLC smoothType = RWT_HA.SecondaryOHLC.HULLEMA;
			private double smoothArg = 7;
		
			private RWT_MA.MAType filterType = RWT_MA.MAType.MEDIANFILT;
			private double filterArg = 5;
		
			private Color upColor = Color.Green;
			private Color dnColor = Color.Red;
			private bool dirUp;
			private double lastFilt; // keep track of previous filter value...
			private bool onBasicBars;
			private double actualTolerance;
			private double lastProxy;
		
        // User defined variables (add any user defined variables below)
			private RWT_HA.OHLC bars;
			private RWT_MA.LINREGSLOPE linreg;
			private RWT_MA.MovingAverage filter;
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
		    onBasicBars = primType == RWT_HA.PrimaryOHLC.BARS;

			var b1 = RWT_HA.OHLCFactory.createPrimary(primType,Open,High,Low,Close,Input);
			var b2 = RWT_HA.OHLCFactory.createSecondary(b1,smoothType,smoothArg);
			bars = new RWT_HA.Heiken_Ashi(b2);
			lastFilt = 0.5*(bars.High+bars.Low);
			lastProxy = lastFilt;
			
			filter = RWT_MA.MAFactory.create(filterType,filterArg);
			filter.init(lastFilt);

			
			linreg = new RWT_MA.LINREGSLOPE((double)lookback);
			linreg.init(lastFilt);
			
			dirUp = true;
			
			actualTolerance = tolerance;
			if(onBasicBars) {
				actualTolerance *= TickSize;	
			}
		}
		
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			bars.update();
			var nextFilt = filter.next( (bars.High+bars.Low)*0.5 );
			var slope = linreg.next(nextFilt);
	
			if( (nextFilt > lastFilt) &&
				(slope <= 0) ) {
			  ProxyLine.Set(lastProxy);
			} else if( (nextFilt < lastFilt) &&
				       (slope >= 0)) {
			  ProxyLine.Set(lastProxy);			
			} else {
				
			  // either everything is flat, or the
			  // med and slope agree...
			  bool assumeUp = (slope > 0);
			  if(slope == 0) {
				if(nextFilt != lastFilt) {
				  assumeUp = (nextFilt > lastFilt);
				} else {
				  assumeUp = dirUp;	
				}
			  }
			   	
			  if(assumeUp) {
			    ProxyLine.Set( Math.Max(lastProxy,nextFilt) );	
			  } else {
				ProxyLine.Set( Math.Min(lastProxy,nextFilt) );
			  }
			
		
			}
			
			var newDirUp = dirUp;
			if(ProxyLine[0] > lastProxy) newDirUp = true;
			else if(ProxyLine[0] < lastProxy) newDirUp = false;
 			
			bool upbar = bars.Close > bars.Open;
			bool dnbar = bars.Close < bars.Open;
			
			// if we're based on bars, use the actual bars themselves, too...
			if(onBasicBars) {
				upbar = upbar || (Close[0] > Open[0]) || (High[0] == Close[0]);	
				dnbar = dnbar || (Close[0] < Open[0]) || (Low[0] == Close[0]);	
			}
			
			double upRefLoc =0;
			double dnRefLoc =0;
			if(onBasicBars) {
				upRefLoc = High[0];
				dnRefLoc = Low[0];
			} else {
				upRefLoc = Input[0];
				dnRefLoc = Input[0];
			}
			
			// exceptions that hold the proxy in place for a bit...
			if(  (!newDirUp && dirUp && upbar) ||
				 (newDirUp && !dirUp && dnbar) ||
				 ( !newDirUp && dirUp && (lastProxy <= (upRefLoc+actualTolerance)) ) ||
                 ( newDirUp && !dirUp && (lastProxy >= (dnRefLoc-actualTolerance))) ||
				 ((ProxyLine[0] > lastProxy) && ( dnbar || (upRefLoc < ProxyLine[0])) ) ||
				 ((ProxyLine[0] < lastProxy) && ( upbar || (dnRefLoc > ProxyLine[0])) )
			  ) {
			  newDirUp = dirUp;
			  ProxyLine.Set(lastProxy);
			}
		    
			PlotColors[0][0] = (newDirUp?upColor:dnColor);
			dirUp = newDirUp;
			lastFilt = nextFilt;
			lastProxy = ProxyLine[0];
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
        public double Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }
		
        [Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public double SmoothArg
        {
            get { return smoothArg; }
            set { smoothArg = value; }
        }

		[Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public RWT_HA.SecondaryOHLC SmoothType
        {
            get { return smoothType; }
            set { smoothType = value; }
        }

		[Description("Input Type")]
        [GridCategory("Parameters")]
        public RWT_HA.PrimaryOHLC PrimaryType
        {
            get { return primType; }
            set { primType = value; }
        }

        [Description("Filter constant")]
        [GridCategory("Parameters")]
        public double FilterArg
        {
            get { return filterArg; }
            set { filterArg = value; }
        }

		[Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType FilterType
        {
            get { return filterType; }
            set { filterType = value; }
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
        private zPriceProxy2[] cachezPriceProxy2 = null;

        private static zPriceProxy2 checkzPriceProxy2 = new zPriceProxy2();

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public zPriceProxy2 zPriceProxy2(double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            return zPriceProxy2(Input, filterArg, filterType, lookback, primaryType, smoothArg, smoothType, tolerance);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public zPriceProxy2 zPriceProxy2(Data.IDataSeries input, double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            if (cachezPriceProxy2 != null)
                for (int idx = 0; idx < cachezPriceProxy2.Length; idx++)
                    if (Math.Abs(cachezPriceProxy2[idx].FilterArg - filterArg) <= double.Epsilon && cachezPriceProxy2[idx].FilterType == filterType && cachezPriceProxy2[idx].Lookback == lookback && cachezPriceProxy2[idx].PrimaryType == primaryType && Math.Abs(cachezPriceProxy2[idx].SmoothArg - smoothArg) <= double.Epsilon && cachezPriceProxy2[idx].SmoothType == smoothType && Math.Abs(cachezPriceProxy2[idx].Tolerance - tolerance) <= double.Epsilon && cachezPriceProxy2[idx].EqualsInput(input))
                        return cachezPriceProxy2[idx];

            lock (checkzPriceProxy2)
            {
                checkzPriceProxy2.FilterArg = filterArg;
                filterArg = checkzPriceProxy2.FilterArg;
                checkzPriceProxy2.FilterType = filterType;
                filterType = checkzPriceProxy2.FilterType;
                checkzPriceProxy2.Lookback = lookback;
                lookback = checkzPriceProxy2.Lookback;
                checkzPriceProxy2.PrimaryType = primaryType;
                primaryType = checkzPriceProxy2.PrimaryType;
                checkzPriceProxy2.SmoothArg = smoothArg;
                smoothArg = checkzPriceProxy2.SmoothArg;
                checkzPriceProxy2.SmoothType = smoothType;
                smoothType = checkzPriceProxy2.SmoothType;
                checkzPriceProxy2.Tolerance = tolerance;
                tolerance = checkzPriceProxy2.Tolerance;

                if (cachezPriceProxy2 != null)
                    for (int idx = 0; idx < cachezPriceProxy2.Length; idx++)
                        if (Math.Abs(cachezPriceProxy2[idx].FilterArg - filterArg) <= double.Epsilon && cachezPriceProxy2[idx].FilterType == filterType && cachezPriceProxy2[idx].Lookback == lookback && cachezPriceProxy2[idx].PrimaryType == primaryType && Math.Abs(cachezPriceProxy2[idx].SmoothArg - smoothArg) <= double.Epsilon && cachezPriceProxy2[idx].SmoothType == smoothType && Math.Abs(cachezPriceProxy2[idx].Tolerance - tolerance) <= double.Epsilon && cachezPriceProxy2[idx].EqualsInput(input))
                            return cachezPriceProxy2[idx];

                zPriceProxy2 indicator = new zPriceProxy2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FilterArg = filterArg;
                indicator.FilterType = filterType;
                indicator.Lookback = lookback;
                indicator.PrimaryType = primaryType;
                indicator.SmoothArg = smoothArg;
                indicator.SmoothType = smoothType;
                indicator.Tolerance = tolerance;
                Indicators.Add(indicator);
                indicator.SetUp();

                zPriceProxy2[] tmp = new zPriceProxy2[cachezPriceProxy2 == null ? 1 : cachezPriceProxy2.Length + 1];
                if (cachezPriceProxy2 != null)
                    cachezPriceProxy2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezPriceProxy2 = tmp;
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
        public Indicator.zPriceProxy2 zPriceProxy2(double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            return _indicator.zPriceProxy2(Input, filterArg, filterType, lookback, primaryType, smoothArg, smoothType, tolerance);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.zPriceProxy2 zPriceProxy2(Data.IDataSeries input, double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            return _indicator.zPriceProxy2(input, filterArg, filterType, lookback, primaryType, smoothArg, smoothType, tolerance);
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
        public Indicator.zPriceProxy2 zPriceProxy2(double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            return _indicator.zPriceProxy2(Input, filterArg, filterType, lookback, primaryType, smoothArg, smoothType, tolerance);
        }

        /// <summary>
        /// price proxy
        /// </summary>
        /// <returns></returns>
        public Indicator.zPriceProxy2 zPriceProxy2(Data.IDataSeries input, double filterArg, RWT_MA.MAType filterType, int lookback, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType, double tolerance)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zPriceProxy2(input, filterArg, filterType, lookback, primaryType, smoothArg, smoothType, tolerance);
        }
    }
}
#endregion
