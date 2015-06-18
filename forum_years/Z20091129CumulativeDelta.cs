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
    /// Plots the cumulative delta as bars.
    /// </summary>
    [Description("Plots the cumulative delta as bars.")]
    public class Z20091129CumulativeDelta : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double minSize = 0; // Default setting for MinSize
            private double maxSize = 99999999; // Default setting for MaxSize
            private VolumeStatsMode calcMethod = VolumeStatsMode.UpDnTick; // Default setting for CalcMethod
			private bool calcWithTicks=false;
		    private bool bundleTrades = false;
		    private long bundleMilliseconds = 150;

		 	private int widthOverride = -1;
			private SolidBrush paintBrush;
        // User defined variables (add any user defined variables below)
		    private Z20091129VolumeStats volstats;
		    private DataSeries dsOpen,dsClose,dsHigh,dsLow;
		    private int lastBar;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false; 
			volstats = null;
        }

		
		protected override void OnStartUp() {
			dsOpen = new DataSeries(this,MaximumBarsLookBack.Infinite);
            dsClose = new DataSeries(this,MaximumBarsLookBack.Infinite);
            dsHigh= new DataSeries(this,MaximumBarsLookBack.Infinite);
            dsLow = new DataSeries(this,MaximumBarsLookBack.Infinite);
			lastBar = -1;
			
			
            paintBrush = new SolidBrush( Color.FromArgb( 255, Color.Black ) );
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(volstats == null) volstats = Z20091129VolumeStats(bundleMilliseconds,bundleTrades,calcMethod,maxSize,minSize);
			
			double curDelta = (calcWithTicks?(volstats.UpTradeCount[0] - volstats.DnTradeCount[0]):volstats.OneBarDelta[0]);

			if(CurrentBar > 1) {
			  dsClose.Set(dsClose[1]+curDelta);
			} else {
			  dsClose.Set(curDelta);	
			}
             
			if(lastBar != CurrentBar) {
			  
			  lastBar = CurrentBar;
				
			  dsOpen.Set(dsClose[0]);
			  dsHigh.Set(dsClose[0]);
			  dsLow.Set(dsClose[0]);
			}
			
			if(dsClose[0] > dsHigh[0]) dsHigh.Set(dsClose[0]);
			if(dsClose[0] < dsLow[0]) dsLow.Set(dsClose[0]);
			Value.Set(dsClose[0]);
        }

	#region plotStuff	
		
/// </summary>
/// <param name="chartControl"></param>
/// <param name="min"></param>
/// <param name="max"></param>
public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
{
    if(Bars==null) return;
	int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
	int firstBar = FirstBarIndexPainted;
	min = Double.MaxValue;
	max = Double.MinValue;
	for(int indx = firstBar; indx<=lastBar;++indx) {
		if((indx <= CurrentBar) && (indx >= 1)) {
		  double thislow = dsLow.Get(indx);
		  double thishigh = dsHigh.Get(indx);
		  if(!double.IsNaN(thislow)) min = Math.Min(min,thislow);
		  if(!double.IsNaN(thishigh)) max = Math.Max(max,thishigh);
		}
	}
	if((max-min)<1) { --min; ++max; }
}

/// <summary>
///
/// </summary>
/// <param name="graphics"></param>
/// <param name="bounds"></param>
/// <param name="min"></param>
/// <param name="max"></param>
public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
{
    if(Bars==null) return;
	int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
	int firstBar = FirstBarIndexPainted;

    int barWidth = ((widthOverride > 0)?widthOverride:ChartControl.BarWidth);
	
	Pen outlinePen = ChartControl.ChartStyle.Pen;
	
    int bars = ChartControl.BarsPainted;

			
	int x, yHigh, yClose, yOpen, yLow;	
	SolidBrush direction;			
	SolidBrush up = new SolidBrush(ChartControl.ChartStyle.UpColor);
	SolidBrush dn = new SolidBrush(ChartControl.ChartStyle.DownColor);
	
	for(int indx=firstBar;indx <= lastBar; ++indx) {
	
		if((indx <= CurrentBar) && (indx >= 1)) {
		
			x = ChartControl.GetXByBarIdx(BarsArray[0],indx);
			yHigh = ChartControl.GetYByValue(this,dsHigh.Get(indx));
			yClose = ChartControl.GetYByValue(this,dsClose.Get(indx));
			yOpen = ChartControl.GetYByValue(this,dsOpen.Get(indx));
			yLow = ChartControl.GetYByValue(this,dsLow.Get(indx));
		
			if(yClose < yOpen) paintBrush.Color = ChartControl.ChartStyle.UpColor;
			else paintBrush.Color = ChartControl.ChartStyle.DownColor;
						
			graphics.DrawLine(ChartControl.ChartStyle.Pen2,x,yHigh,x,yLow);
			if(yClose==yOpen)
				graphics.DrawLine(outlinePen,x-barWidth-outlinePen.Width,yClose,x+barWidth+outlinePen.Width,yClose);
			else {
		        graphics.FillRectangle(paintBrush,x-barWidth-outlinePen.Width,Math.Min(yClose,yOpen),2*(barWidth+outlinePen.Width),Math.Abs(yClose-yOpen));
				graphics.DrawRectangle(outlinePen,x-barWidth-outlinePen.Width,Math.Min(yClose,yOpen),2*(barWidth+outlinePen.Width),Math.Abs(yClose-yOpen));			  	
			}
		}
		
	}
}

#endregion
		
        #region Properties
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
		
        [Description("Override for Bar Width (-1 = no)")]
        [Category("Parameters")]
        public int  WidthOverride
        {
            get { return widthOverride; }
            set { widthOverride = value; }
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
        [Description("If true, calculates on tick count, rather than volume count.")]
        [Category("Parameters")]
        public bool CalcWithTicks
        {
            get { return calcWithTicks; }
            set { calcWithTicks = value; }
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
        private Z20091129CumulativeDelta[] cacheZ20091129CumulativeDelta = null;

        private static Z20091129CumulativeDelta checkZ20091129CumulativeDelta = new Z20091129CumulativeDelta();

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Z20091129CumulativeDelta Z20091129CumulativeDelta(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            return Z20091129CumulativeDelta(Input, bundleMilliseconds, bundleTrades, calcMethod, calcWithTicks, maxSize, minSize, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Z20091129CumulativeDelta Z20091129CumulativeDelta(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            if (cacheZ20091129CumulativeDelta != null)
                for (int idx = 0; idx < cacheZ20091129CumulativeDelta.Length; idx++)
                    if (cacheZ20091129CumulativeDelta[idx].BundleMilliseconds == bundleMilliseconds && cacheZ20091129CumulativeDelta[idx].BundleTrades == bundleTrades && cacheZ20091129CumulativeDelta[idx].CalcMethod == calcMethod && cacheZ20091129CumulativeDelta[idx].CalcWithTicks == calcWithTicks && Math.Abs(cacheZ20091129CumulativeDelta[idx].MaxSize - maxSize) <= double.Epsilon && Math.Abs(cacheZ20091129CumulativeDelta[idx].MinSize - minSize) <= double.Epsilon && cacheZ20091129CumulativeDelta[idx].WidthOverride == widthOverride && cacheZ20091129CumulativeDelta[idx].EqualsInput(input))
                        return cacheZ20091129CumulativeDelta[idx];

            lock (checkZ20091129CumulativeDelta)
            {
                checkZ20091129CumulativeDelta.BundleMilliseconds = bundleMilliseconds;
                bundleMilliseconds = checkZ20091129CumulativeDelta.BundleMilliseconds;
                checkZ20091129CumulativeDelta.BundleTrades = bundleTrades;
                bundleTrades = checkZ20091129CumulativeDelta.BundleTrades;
                checkZ20091129CumulativeDelta.CalcMethod = calcMethod;
                calcMethod = checkZ20091129CumulativeDelta.CalcMethod;
                checkZ20091129CumulativeDelta.CalcWithTicks = calcWithTicks;
                calcWithTicks = checkZ20091129CumulativeDelta.CalcWithTicks;
                checkZ20091129CumulativeDelta.MaxSize = maxSize;
                maxSize = checkZ20091129CumulativeDelta.MaxSize;
                checkZ20091129CumulativeDelta.MinSize = minSize;
                minSize = checkZ20091129CumulativeDelta.MinSize;
                checkZ20091129CumulativeDelta.WidthOverride = widthOverride;
                widthOverride = checkZ20091129CumulativeDelta.WidthOverride;

                if (cacheZ20091129CumulativeDelta != null)
                    for (int idx = 0; idx < cacheZ20091129CumulativeDelta.Length; idx++)
                        if (cacheZ20091129CumulativeDelta[idx].BundleMilliseconds == bundleMilliseconds && cacheZ20091129CumulativeDelta[idx].BundleTrades == bundleTrades && cacheZ20091129CumulativeDelta[idx].CalcMethod == calcMethod && cacheZ20091129CumulativeDelta[idx].CalcWithTicks == calcWithTicks && Math.Abs(cacheZ20091129CumulativeDelta[idx].MaxSize - maxSize) <= double.Epsilon && Math.Abs(cacheZ20091129CumulativeDelta[idx].MinSize - minSize) <= double.Epsilon && cacheZ20091129CumulativeDelta[idx].WidthOverride == widthOverride && cacheZ20091129CumulativeDelta[idx].EqualsInput(input))
                            return cacheZ20091129CumulativeDelta[idx];

                Z20091129CumulativeDelta indicator = new Z20091129CumulativeDelta();
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
                indicator.CalcWithTicks = calcWithTicks;
                indicator.MaxSize = maxSize;
                indicator.MinSize = minSize;
                indicator.WidthOverride = widthOverride;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20091129CumulativeDelta[] tmp = new Z20091129CumulativeDelta[cacheZ20091129CumulativeDelta == null ? 1 : cacheZ20091129CumulativeDelta.Length + 1];
                if (cacheZ20091129CumulativeDelta != null)
                    cacheZ20091129CumulativeDelta.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20091129CumulativeDelta = tmp;
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
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091129CumulativeDelta Z20091129CumulativeDelta(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            return _indicator.Z20091129CumulativeDelta(Input, bundleMilliseconds, bundleTrades, calcMethod, calcWithTicks, maxSize, minSize, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091129CumulativeDelta Z20091129CumulativeDelta(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            return _indicator.Z20091129CumulativeDelta(input, bundleMilliseconds, bundleTrades, calcMethod, calcWithTicks, maxSize, minSize, widthOverride);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091129CumulativeDelta Z20091129CumulativeDelta(long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            return _indicator.Z20091129CumulativeDelta(Input, bundleMilliseconds, bundleTrades, calcMethod, calcWithTicks, maxSize, minSize, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091129CumulativeDelta Z20091129CumulativeDelta(Data.IDataSeries input, long bundleMilliseconds, bool bundleTrades, VolumeStatsMode calcMethod, bool calcWithTicks, double maxSize, double minSize, int widthOverride)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091129CumulativeDelta(input, bundleMilliseconds, bundleTrades, calcMethod, calcWithTicks, maxSize, minSize, widthOverride);
        }
    }
}
#endregion
