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

public enum VolumeStatsMode { BidAsk, UpDnTick, PrevMedian, PrevClose }

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// A helper indicator for tracking volume stats
    /// </summary>
    [Description("A helper indicator for tracking volume stats")]
    public class Z20091129VolumeStats : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double minSize = 0; // Default setting for MinSize
            private double maxSize = 99999999; // Default setting for MaxSize
            private VolumeStatsMode calcMethod = VolumeStatsMode.UpDnTick; // Default setting for CalcMethod
        // User defined variables (add any user defined variables below)
		    private double myTickSize = 0;
		
		    // make the raw data available to other indicators...
		    private IntSeries upTradeCount, dnTradeCount;
		    private DataSeries upTradeVol, dnTradeVol;
		
		    // tracking variables for Mode=UpDnTick method
		    private bool firstTick;
		    private bool uptick;
		    private double prevClose;
		    private double	prevVol;
		    private int lastBar;

			// for bundling purposes...
		    private bool bundleTrades = false;
		    private long bundleMilliseconds = 150;
			private System.Diagnostics.Stopwatch timeSinceUpVol;
		    private System.Diagnostics.Stopwatch timeSinceDnVol;
		    private double storedUpVol;
		    private double storedDnVol;
		    private int storedUpTicks;
		    private int storedDnTicks;
		    private double summedUpTradePrice;
		    private double summedDnTradePrice;
			private int upTradeComponents = 0;
			private double upTradeAvgPrice = -1;
			private double upTradeSize = 0;
			private int dnTradeComponents = 0;
			private double dnTradeAvgPrice = -1;
			private double dnTradeSize = 0;
		
			double lastAsk;
			double lastBid;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "OneBarDelta"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;

        }

		protected override void OnStartUp() {
		    // init data series
			upTradeVol = new DataSeries(this);
			dnTradeVol = new DataSeries(this);
			upTradeCount = new IntSeries(this);
			dnTradeCount = new IntSeries(this);

			firstTick = true;

			// init BidAsk mode vars
			lastBid = -1;
			lastAsk = -1;
			
			// init UpDnTick mode vars
			lastBar = -1;
			uptick = true;
			prevClose = -1;
			prevVol = 0;	
			
			// store off the TickSize
			myTickSize = TickSize;
			
			// initialize bundling stuff
			if(bundleTrades) {
				timeSinceUpVol = new System.Diagnostics.Stopwatch(); 
				timeSinceDnVol = new System.Diagnostics.Stopwatch(); 
				storedUpVol = 0;
				storedDnVol = 0;
				storedUpTicks = 0;
				storedDnTicks = 0;
				summedUpTradePrice = 0;
				summedDnTradePrice = 0;
			    upTradeComponents = 0;
			    upTradeAvgPrice = -1;
			    upTradeSize = 0;
			    dnTradeComponents = 0;
			    dnTradeAvgPrice = -1;
			    dnTradeSize = 0;
		    }
	
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar != lastBar)
			{
				prevVol = 0;
				lastBar = CurrentBar;
				upTradeCount.Set(0);
				dnTradeCount.Set(0);
				upTradeVol.Set(0);
				dnTradeVol.Set(0);
			}
			
		    double tradeVol = Volume[0] - prevVol;
			bool weHaveUpPressure = false;
			bool weHaveDnPressure = false;
			
   			if (!CalculateOnBarClose && !Historical && !firstTick) {

			  switch(calcMethod) {
				#region CalcMethodStuff
				case VolumeStatsMode.UpDnTick:
				  // ***********************************************
				  // the UpDnTick calculation method....
				  // ***********************************************
				    if ((Close[0] > prevClose) || ((Close[0] == prevClose) && uptick)) {
					  uptick = true;
					  weHaveUpPressure = true;
			        }
				    else if((Close[0] < prevClose) || ((Close[0] == prevClose) && !uptick)) {
				  	  uptick = false;
					  weHaveDnPressure = true;
			        }
				  break;
				case VolumeStatsMode.PrevMedian:
				  // ***********************************************
				  // the PrevMedian calculation method....
				  // ***********************************************
					double oldMedian = ((CurrentBar<2)?Median[0]:Median[1]);

					if(Close[0] > oldMedian) {
						weHaveUpPressure = true;
					} else if(Close[0] < oldMedian) {
						weHaveDnPressure = true;
					}
					break;
				case VolumeStatsMode.PrevClose:
				  // ***********************************************
				  // the PrevClose calculation method....
				  // ***********************************************
					double oldClose = ((CurrentBar<2)?Close[0]:Close[1]);

					if(Close[0] > oldClose) {
						weHaveUpPressure = true;
					} else if(Close[0] < oldClose) {
						weHaveDnPressure = true;
					}
					break;
					
				case VolumeStatsMode.BidAsk:
				  // ***********************************************
				  // the BidAsk calculation method....
				  // ***********************************************
				    if (Close[0] >= lastAsk) {
						weHaveUpPressure = true;
			        }
				    else if (Close[0] <= lastBid) {
						weHaveDnPressure = true;
			        }
				  break;
				default:
					break;
				#endregion
			  }
						
     	      prevVol = Volume[0];
			  prevClose = Close[0];
			}
			else
			{
			    upTradeCount.Set(0);
			    dnTradeCount.Set(0);
			    upTradeVol.Set(0);
			    dnTradeVol.Set(0);
			    if(!Historical) firstTick = false;
			    prevVol = Volume[0];
				prevClose = Close[0];
			}			
			
			// ********************************************
			// do the bundling, and reporting....
			// ********************************************
			upTradeComponents = 0;
			upTradeAvgPrice = -1;
			upTradeSize = 0;
			dnTradeComponents = 0;
			dnTradeAvgPrice = -1;
			dnTradeSize = 0;
			
			#region BundlingStuff
			if(bundleTrades) {
			   // if either of the timers have gone far enough to
			   // separate a tick, then publish it...
			   if(timeSinceUpVol.ElapsedMilliseconds > bundleMilliseconds) {
				 // report the trade if it's of the right size...
			     if((storedUpVol >= minSize) && (storedUpVol <= maxSize)) {
				    upTradeVol.Set(upTradeVol[0]+storedUpVol);
					upTradeCount.Set(upTradeCount[0]+1);
					upTradeComponents = storedUpTicks;
					upTradeAvgPrice = summedUpTradePrice/storedUpVol;
					upTradeSize = storedUpVol;
				 }
				 // no matter what, clear the trade out...
	             storedUpVol = 0;
			     storedUpTicks = 0;
				 summedUpTradePrice = 0;
				 timeSinceUpVol.Reset();
			   }
			   if(timeSinceDnVol.ElapsedMilliseconds > bundleMilliseconds) {
				 // report the trade if it's of the right size...
			     if((storedDnVol >= minSize) && (storedDnVol <= maxSize)) {
				    dnTradeVol.Set(dnTradeVol[0]+storedDnVol);
					dnTradeCount.Set(dnTradeCount[0]+1);
					dnTradeComponents = storedDnTicks;
					dnTradeAvgPrice = summedDnTradePrice/storedDnVol;
					dnTradeSize = storedDnVol;
				  }
				  // no matter what, clear the trade out...
	              storedDnVol = 0;
			      storedDnTicks = 0;
				  summedDnTradePrice = 0;
				  timeSinceDnVol.Reset();
			   }
				
  			  if(weHaveUpPressure) {				
				storedUpVol += tradeVol;
				++storedUpTicks;
				summedUpTradePrice += (Close[0]*tradeVol);
				timeSinceUpVol.Reset();
				timeSinceUpVol.Start();
			  }
			  if(weHaveDnPressure) {
				storedDnVol += tradeVol;
				++storedDnTicks;						
				summedDnTradePrice += (Close[0]*tradeVol);
				timeSinceDnVol.Reset();
				timeSinceDnVol.Start();
			  }
			}
			else {
			  // not bundling trades...
			  if((tradeVol >= minSize) && (tradeVol <= maxSize)) {
			     if(weHaveUpPressure) {
  			        upTradeComponents = 1;
			        upTradeAvgPrice = Close[0];
			        upTradeSize = tradeVol;

					upTradeCount.Set(upTradeCount[0]+1);
					upTradeVol.Set(upTradeVol[0] + tradeVol);
			     }
			     if(weHaveDnPressure) {
  			        dnTradeComponents = 1;
			        dnTradeAvgPrice = Close[0];
			        dnTradeSize = tradeVol;

					dnTradeCount.Set(dnTradeCount[0]+1);
					dnTradeVol.Set(dnTradeVol[0] + tradeVol);
				 }
			  }
			}
			#endregion  				
			
			// plot the 1-bar delta
            OneBarDelta.Set(upTradeVol[0] - dnTradeVol[0]);
        }

		protected override void OnMarketData(MarketDataEventArgs e) 
		{
            if (e.MarketDataType == MarketDataType.Bid) {
               if (e.Price > lastBid) {
                  lastBid = e.Price;
                  lastAsk = e.Price + myTickSize;
               }
            } else if (e.MarketDataType == MarketDataType.Ask) {
                if (e.Price < lastAsk) {
                   lastAsk = e.Price;
                   lastBid = e.Price - myTickSize;
                }
            }
		}
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries OneBarDelta
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public IntSeries UpTradeCount
        {
            get { return upTradeCount; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public IntSeries DnTradeCount
        {
            get { return dnTradeCount; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpTradeVol
        {
            get { return upTradeVol; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DnTradeVol
        {
            get { return dnTradeVol; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double UpTradeAvgPrice
        {
            get { return upTradeAvgPrice; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double UpTradeSize
        {
            get { return upTradeSize; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public int UpTradeComponents
        {
            get { return upTradeComponents; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool UpTrade
        {
            get { return upTradeComponents > 0; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double DnTradeAvgPrice
        {
            get { return dnTradeAvgPrice; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public double DnTradeSize
        {
            get { return dnTradeSize; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public int DnTradeComponents
        {
            get { return dnTradeComponents; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool DnTrade
        {
            get { return dnTradeComponents > 0; }
        }

		
		
        [Description("Minimum trade size to count")]
        [Category("Parameters")]
        public double MinSize
        {
            get { return minSize; }
            set { minSize = Math.Max(0, value); }
        }
        [Description("Bundle the trades that happen close together?")]
        [Category("Parameters")]
        public bool  BundleTrades
        {
            get { return bundleTrades; }
            set { bundleTrades = value; }
        }
        [Description("If bundling, how fast should trades be coming in?")]
        [Category("Parameters")]
        public long  BundleMilliseconds
        {
            get { return bundleMilliseconds; }
            set { bundleMilliseconds = Math.Max(0,value); }
        }

        [Description("Maximum trade size to count")]
        [Category("Parameters")]
        public double MaxSize
        {
            get { return maxSize; }
            set { maxSize = Math.Max(0.000, value); }
        }

        [Description("What method should we use to classify ticks?")]
        [Category("Parameters")]
        public VolumeStatsMode CalcMethod
        {
            get { return calcMethod; }
            set { calcMethod = value; }
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
        private Z20091129VolumeStats[] cacheZ20091129VolumeStats = null;

        private static Z20091129VolumeStats checkZ20091129VolumeStats = new Z20091129VolumeStats();

        /// <summary>
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        public Z20091129VolumeStats Z20091129VolumeStats(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            return Z20091129VolumeStats(Input, bundleMilliseconds, bundleTrades, calcMethod, maxSize, minSize);
        }

        /// <summary>
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        public Z20091129VolumeStats Z20091129VolumeStats(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            if (cacheZ20091129VolumeStats != null)
                for (int idx = 0; idx < cacheZ20091129VolumeStats.Length; idx++)
                    if (cacheZ20091129VolumeStats[idx].BundleMilliseconds == bundleMilliseconds && cacheZ20091129VolumeStats[idx].BundleTrades == bundleTrades && cacheZ20091129VolumeStats[idx].CalcMethod == calcMethod && Math.Abs(cacheZ20091129VolumeStats[idx].MaxSize - maxSize) <= double.Epsilon && Math.Abs(cacheZ20091129VolumeStats[idx].MinSize - minSize) <= double.Epsilon && cacheZ20091129VolumeStats[idx].EqualsInput(input))
                        return cacheZ20091129VolumeStats[idx];

            lock (checkZ20091129VolumeStats)
            {
                checkZ20091129VolumeStats.BundleMilliseconds = bundleMilliseconds;
                bundleMilliseconds = checkZ20091129VolumeStats.BundleMilliseconds;
                checkZ20091129VolumeStats.BundleTrades = bundleTrades;
                bundleTrades = checkZ20091129VolumeStats.BundleTrades;
                checkZ20091129VolumeStats.CalcMethod = calcMethod;
                calcMethod = checkZ20091129VolumeStats.CalcMethod;
                checkZ20091129VolumeStats.MaxSize = maxSize;
                maxSize = checkZ20091129VolumeStats.MaxSize;
                checkZ20091129VolumeStats.MinSize = minSize;
                minSize = checkZ20091129VolumeStats.MinSize;

                if (cacheZ20091129VolumeStats != null)
                    for (int idx = 0; idx < cacheZ20091129VolumeStats.Length; idx++)
                        if (cacheZ20091129VolumeStats[idx].BundleMilliseconds == bundleMilliseconds && cacheZ20091129VolumeStats[idx].BundleTrades == bundleTrades && cacheZ20091129VolumeStats[idx].CalcMethod == calcMethod && Math.Abs(cacheZ20091129VolumeStats[idx].MaxSize - maxSize) <= double.Epsilon && Math.Abs(cacheZ20091129VolumeStats[idx].MinSize - minSize) <= double.Epsilon && cacheZ20091129VolumeStats[idx].EqualsInput(input))
                            return cacheZ20091129VolumeStats[idx];

                Z20091129VolumeStats indicator = new Z20091129VolumeStats();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BundleMilliseconds = bundleMilliseconds;
                indicator.BundleTrades = bundleTrades;
                indicator.CalcMethod = calcMethod;
                indicator.MaxSize = maxSize;
                indicator.MinSize = minSize;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20091129VolumeStats[] tmp = new Z20091129VolumeStats[cacheZ20091129VolumeStats == null ? 1 : cacheZ20091129VolumeStats.Length + 1];
                if (cacheZ20091129VolumeStats != null)
                    cacheZ20091129VolumeStats.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20091129VolumeStats = tmp;
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
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091129VolumeStats Z20091129VolumeStats(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            return _indicator.Z20091129VolumeStats(Input, bundleMilliseconds, bundleTrades, calcMethod, maxSize, minSize);
        }

        /// <summary>
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091129VolumeStats Z20091129VolumeStats(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            return _indicator.Z20091129VolumeStats(input, bundleMilliseconds, bundleTrades, calcMethod, maxSize, minSize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091129VolumeStats Z20091129VolumeStats(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            return _indicator.Z20091129VolumeStats(Input, bundleMilliseconds, bundleTrades, calcMethod, maxSize, minSize);
        }

        /// <summary>
        /// A helper indicator for tracking volume stats
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091129VolumeStats Z20091129VolumeStats(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, double maxSize, double minSize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091129VolumeStats(input, bundleMilliseconds, bundleTrades, calcMethod, maxSize, minSize);
        }
    }
}
#endregion
