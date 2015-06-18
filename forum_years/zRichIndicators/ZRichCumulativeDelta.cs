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
    public class ZRichCumulativeDelta : Indicator
    {
        #region Variables
        // Wizard generated variables
		 	private int widthOverride = -1;
			private SolidBrush paintBrush;
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false; 
        }

		
		protected override void OnStartUp() {
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");
			
            paintBrush = new SolidBrush( Color.FromArgb( 255, Color.Black ) );
		}

		protected override void OnTermination() {
			base.OnTermination();
			if(paintBrush != null) paintBrush.Dispose();	
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var ed = extdat.getExtraData(0,this.Bars,CurrentBar);
			if(ed != null) {
				Value.Set(ed.dClose);
			} else {
				Value.Set(0);	
			}
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
		    var ed = extdat.getExtraData(CurrentBar-indx,this.Bars,CurrentBar);
			if(ed != null) {
		  		min = Math.Min(min,ed.dLow);
		  		max = Math.Max(max,ed.dHigh);
			} else {
			    min = Math.Min(min,0);
				max = Math.Max(max,0);
			}
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
			
	int x, yHigh, yClose, yOpen, yLow;	
	
	for(int indx=firstBar;indx <= lastBar; ++indx) {
	
		if((indx <= CurrentBar) && (indx >= 1)) {		
			x = ChartControl.GetXByBarIdx(BarsArray[0],indx);
		    var ed = extdat.getExtraData(CurrentBar-indx,this.Bars,CurrentBar);
			if(ed != null) {
				yHigh = ChartControl.GetYByValue(this,ed.dHigh);
				yClose = ChartControl.GetYByValue(this,ed.dClose);
				yOpen = ChartControl.GetYByValue(this,ed.dOpen);
				yLow = ChartControl.GetYByValue(this,ed.dLow);
		    } else {
			    yHigh = 0; yClose = 0; yOpen = 0; yLow = 0;	
			}
			
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
        [Description("Override for Bar Width (-1 = no)")]
        [Category("Parameters")]
        public int  WidthOverride
        {
            get { return widthOverride; }
            set { widthOverride = value; }
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
        private ZRichCumulativeDelta[] cacheZRichCumulativeDelta = null;

        private static ZRichCumulativeDelta checkZRichCumulativeDelta = new ZRichCumulativeDelta();

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public ZRichCumulativeDelta ZRichCumulativeDelta(int widthOverride)
        {
            return ZRichCumulativeDelta(Input, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public ZRichCumulativeDelta ZRichCumulativeDelta(Data.IDataSeries input, int widthOverride)
        {
            if (cacheZRichCumulativeDelta != null)
                for (int idx = 0; idx < cacheZRichCumulativeDelta.Length; idx++)
                    if (cacheZRichCumulativeDelta[idx].WidthOverride == widthOverride && cacheZRichCumulativeDelta[idx].EqualsInput(input))
                        return cacheZRichCumulativeDelta[idx];

            lock (checkZRichCumulativeDelta)
            {
                checkZRichCumulativeDelta.WidthOverride = widthOverride;
                widthOverride = checkZRichCumulativeDelta.WidthOverride;

                if (cacheZRichCumulativeDelta != null)
                    for (int idx = 0; idx < cacheZRichCumulativeDelta.Length; idx++)
                        if (cacheZRichCumulativeDelta[idx].WidthOverride == widthOverride && cacheZRichCumulativeDelta[idx].EqualsInput(input))
                            return cacheZRichCumulativeDelta[idx];

                ZRichCumulativeDelta indicator = new ZRichCumulativeDelta();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.WidthOverride = widthOverride;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichCumulativeDelta[] tmp = new ZRichCumulativeDelta[cacheZRichCumulativeDelta == null ? 1 : cacheZRichCumulativeDelta.Length + 1];
                if (cacheZRichCumulativeDelta != null)
                    cacheZRichCumulativeDelta.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichCumulativeDelta = tmp;
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
        public Indicator.ZRichCumulativeDelta ZRichCumulativeDelta(int widthOverride)
        {
            return _indicator.ZRichCumulativeDelta(Input, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichCumulativeDelta ZRichCumulativeDelta(Data.IDataSeries input, int widthOverride)
        {
            return _indicator.ZRichCumulativeDelta(input, widthOverride);
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
        public Indicator.ZRichCumulativeDelta ZRichCumulativeDelta(int widthOverride)
        {
            return _indicator.ZRichCumulativeDelta(Input, widthOverride);
        }

        /// <summary>
        /// Plots the cumulative delta as bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichCumulativeDelta ZRichCumulativeDelta(Data.IDataSeries input, int widthOverride)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichCumulativeDelta(input, widthOverride);
        }
    }
}
#endregion
