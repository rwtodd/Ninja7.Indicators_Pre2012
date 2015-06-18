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
    /// trade activity at bid and ask
    /// </summary>
    [Description("trade activity at bid and ask")]
    public class ZRichActivity : Indicator
    {
        #region Variables
        // Wizard generated variables
		
		// vars for putting traded amounts on the right...
		    private double[] tradedPrices;
		    private long[] tradedAmounts;
		    private int taIdx;
			private int lastBarSeen;
		    private long lastVolume;
		    private bool rt;
		
			private SolidBrush paintBrush;
			private StringFormat rightFormat;
		
			private int padding = 3;
		// the colors...
			private Color neutcolor = Color.Black;
		
		// drawing support
		private RectangleF lilrect;
		private float tickHeight;
		private int barWidth;
		private Font deltaFont;
		private double chartedMin;
		private double chartedMax;
		
        #endregion
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {	
            Overlay				= true;
			CalculateOnBarClose = false;
        }
		protected override void OnStartUp() {
			base.OnStartUp();
			
            paintBrush = new SolidBrush( neutcolor );
		    rightFormat = new StringFormat();
    		rightFormat.Alignment = StringAlignment.Far;
			rightFormat.LineAlignment = StringAlignment.Center;
			rightFormat.Trimming = StringTrimming.None;
			rightFormat.FormatFlags |= StringFormatFlags.NoClip;
			rightFormat.FormatFlags |= StringFormatFlags.NoWrap;
			tradedPrices = new double[2];
			tradedAmounts = new long[2];
			taIdx = 0;
			lastBarSeen = -1;
			lastVolume = 0;
			chartedMin = -1;
			chartedMax = -1;
		    tickHeight = -1;
            barWidth = -1;
		    deltaFont = null;
			lilrect = new RectangleF(0,0,0,0);
			rt = false;
		}
		
		protected override void OnTermination() {
			if(paintBrush != null) paintBrush.Dispose();	
			if(rightFormat != null) rightFormat.Dispose();
			if(deltaFont != null) deltaFont.Dispose();
			base.OnTermination();
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			if(CurrentBar != lastBarSeen) {
				lastBarSeen = CurrentBar;
								
				lastVolume = 0;	
			} else {
			    rt = true;	
			}

			var c = Close[0];
			var v = ((long)(Volume[0])) - lastVolume;
			if(c == tradedPrices[0]) {
				if(rt) tradedAmounts[0] += v;
			} else if(c == tradedPrices[1]) {
				if(rt) tradedAmounts[1] += v;
			} else {
			   if(c > tradedPrices[0]) {
			      taIdx = 0;
			   } else if(c < tradedPrices[1]) {
				  taIdx = 1;
			   } else {
			     ++taIdx; if(taIdx > 1) taIdx = 0;
			   }
			   // update traded prices...
			   tradedPrices[taIdx] = c;
			   tradedAmounts[taIdx] = (rt?v:0);
			}
			lastVolume = ((long)Volume[0]);
        }
		
		//#region plotstuff
		
		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{	
			if(Bars==null) return;
			int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
			int firstBar = FirstBarIndexPainted;
			min = Double.MaxValue;
			max = Double.MinValue;

			var canht = ChartControl.CanvasBottom - ChartControl.CanvasTop;

			for(int indx = firstBar; indx<=lastBar;++indx) {
				if((indx <= CurrentBar) && (indx >= 1)) {
					min = Math.Min(min,Bars.GetLow(indx));
					max = Math.Max(max,Bars.GetHigh(indx));
				}
			}
			
			chartedMin = min; // store off the charted values
			chartedMax = max;
			
			if(max < 0) return;
			
			// extend the top by 1 tick, to make room for text
			max = max + TickSize;
			
		}
		
public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
    if((Bars==null) || (chartedMax < 0)) return;
	
	int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
	int firstBar = FirstBarIndexPainted;
	
	var th =  ChartControl.GetYByValue(this,chartedMax) - ChartControl.GetYByValue(this,chartedMax+TickSize);
	float halfHeight = th/2f;

	#region setupFonts
    if((ChartControl.BarSpace != barWidth) || (th != tickHeight)) {
	   // need to recompute the right fonts, then...
	   //Print("New TickHeight! " + th + " vs. "+tickHeight); // DEBUG
		
	   barWidth = ChartControl.BarSpace;
	   tickHeight = th;
	   lilrect.Width = barWidth/2-3;
	   lilrect.Height = th;
	   if(deltaFont != null) deltaFont.Dispose(); 
	   deltaFont = null;
//	   float delwidth = Math.Max(barWidth/6,2);
//	   deltaFont =  new Font(FontFamily.GenericMonospace,delwidth,GraphicsUnit.Pixel);
//	   if(deltaFont.Height > th) {
//			deltaFont.Dispose();
			deltaFont = new Font(FontFamily.GenericMonospace,Math.Max(th,2),GraphicsUnit.Pixel);
//	   }
	}
	#endregion setupFonts
	
    // now paint the tradedamounts, if needed....
	#region tradedamts
		if(lastBar == CurrentBar) {
			lilrect.X = ChartControl.GetXByBarIdx(BarsArray[0],lastBar) + barWidth*padding;
			if(tradedPrices[0] > 0) {
			    lilrect.Y = ChartControl.GetYByValue(this,tradedPrices[0]) - halfHeight;
			    graphics.DrawString(tradedAmounts[0].ToString(),deltaFont,paintBrush,lilrect,rightFormat);
			}
			if(tradedPrices[1] > 0) {
				lilrect.Y = ChartControl.GetYByValue(this,tradedPrices[1]) - halfHeight;
			    graphics.DrawString(tradedAmounts[1].ToString(),deltaFont,paintBrush,lilrect,rightFormat);
			}
		}
	#endregion tradedamts

}

		//#endregion 

        #region Properties	
		[Description("padding")]
        [GridCategory("Parameters")]
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }		
		[Description("neutcolor")]
        [GridCategory("Parameters")]
        public Color ColorNeutralInfo	
        {
            get { return neutcolor; }
            set { neutcolor = value; }
        }
        [Browsable(false)]
        public string neutcolorSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(neutcolor); }
           set { neutcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private ZRichActivity[] cacheZRichActivity = null;

        private static ZRichActivity checkZRichActivity = new ZRichActivity();

        /// <summary>
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        public ZRichActivity ZRichActivity(Color colorNeutralInfo, int padding)
        {
            return ZRichActivity(Input, colorNeutralInfo, padding);
        }

        /// <summary>
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        public ZRichActivity ZRichActivity(Data.IDataSeries input, Color colorNeutralInfo, int padding)
        {
            if (cacheZRichActivity != null)
                for (int idx = 0; idx < cacheZRichActivity.Length; idx++)
                    if (cacheZRichActivity[idx].ColorNeutralInfo == colorNeutralInfo && cacheZRichActivity[idx].Padding == padding && cacheZRichActivity[idx].EqualsInput(input))
                        return cacheZRichActivity[idx];

            lock (checkZRichActivity)
            {
                checkZRichActivity.ColorNeutralInfo = colorNeutralInfo;
                colorNeutralInfo = checkZRichActivity.ColorNeutralInfo;
                checkZRichActivity.Padding = padding;
                padding = checkZRichActivity.Padding;

                if (cacheZRichActivity != null)
                    for (int idx = 0; idx < cacheZRichActivity.Length; idx++)
                        if (cacheZRichActivity[idx].ColorNeutralInfo == colorNeutralInfo && cacheZRichActivity[idx].Padding == padding && cacheZRichActivity[idx].EqualsInput(input))
                            return cacheZRichActivity[idx];

                ZRichActivity indicator = new ZRichActivity();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorNeutralInfo = colorNeutralInfo;
                indicator.Padding = padding;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichActivity[] tmp = new ZRichActivity[cacheZRichActivity == null ? 1 : cacheZRichActivity.Length + 1];
                if (cacheZRichActivity != null)
                    cacheZRichActivity.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichActivity = tmp;
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
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichActivity ZRichActivity(Color colorNeutralInfo, int padding)
        {
            return _indicator.ZRichActivity(Input, colorNeutralInfo, padding);
        }

        /// <summary>
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichActivity ZRichActivity(Data.IDataSeries input, Color colorNeutralInfo, int padding)
        {
            return _indicator.ZRichActivity(input, colorNeutralInfo, padding);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichActivity ZRichActivity(Color colorNeutralInfo, int padding)
        {
            return _indicator.ZRichActivity(Input, colorNeutralInfo, padding);
        }

        /// <summary>
        /// trade activity at bid and ask
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichActivity ZRichActivity(Data.IDataSeries input, Color colorNeutralInfo, int padding)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichActivity(input, colorNeutralInfo, padding);
        }
    }
}
#endregion
