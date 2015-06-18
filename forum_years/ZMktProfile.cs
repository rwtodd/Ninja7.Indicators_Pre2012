#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region support
namespace rwtMP {
  public class Histogram {
	 private static char[] _alphabet = { 'a','b','c','d','e','f','g','h','i','j','k','l',
		                                 'm','n','o','p','q','r','s','t','u','v','w','x','y','z',
		                                 'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
		                                 'O','P','Q','R','S','T','U','V','W','X','Y','Z' };
	
	 private StringBuilder[] tpos;
	 private double topPrice;
	 private double granularity;  // same as the quantize factor below
	 public int StartBar;
	 public double HighestLevel;
	 public double LowestLevel;
	 private bool preloaded;  // did we start out with data in us?
	 private bool starise; // do we turn previous ones into *'s?
	
	 private static StringBuilder starify(string orig) {
		 var ans = new StringBuilder();
		 for(int i = 0; i < orig.Length; ++i) {
		   if(orig[i] != '*') ans.Append('*');	
		 }
		 return ans;
	 }
	
	
	 public Histogram(double g, int sb, Histogram oldh, bool stars) {
	    granularity = g;
		StartBar = sb;
		oldbn = -1;
		starise = stars;
		
		if(oldh == null) {
			preloaded = false;
			topPrice = -1;
			tpos = new StringBuilder[20];
			HighestLevel = Double.MinValue;
			LowestLevel = Double.MaxValue;
		} else {
			preloaded = true;
			tpos = new StringBuilder[oldh.tpos.Length];
			
			bool first = true;
			int highestnonempty = -1;
			int lowestnonempty = -1;
			int i = 0;
			for(int j = 0; j < tpos.Length; ++j) {
			   if(oldh.tpos[j] == null) {
				  if(!first) ++i;
				  continue;
			   }
			
			   StringBuilder builder = null;
			   if(starise) builder = Histogram.starify(oldh.tpos[j].ToString());
			   else builder = new StringBuilder(oldh.tpos[j].ToString());
			   
			   if(builder.Length > 0) {
				  tpos[i++] = builder;
				  if(first) {
				     highestnonempty = j;
					 first = false;
				  }
				  lowestnonempty = j;
			   } else {
				  if(!first) ++i;
			   } 
				
			}
			// now fixup the topprice and length...
			topPrice = oldh.topPrice - highestnonempty*granularity;
			HighestLevel = topPrice;
			LowestLevel = topPrice - (lowestnonempty - highestnonempty)*granularity;
			Array.Resize<StringBuilder>(ref tpos,lowestnonempty - highestnonempty + 1);	
		}
	 }
	
	    private static char charForNum(int num) {
		   if(num < 52) return _alphabet[num];
		   num = num % 52;
		   return _alphabet[num];
		}
		
		private int oldbn;
		private void clearBarNum(int bn, char c) {
			if(!preloaded || starise || (bn <= oldbn))  return;
			bool first = true;
			
			int highestnonempty = -1;
			int lowestnonempty = -1;
			
			for(int i = 0; i < tpos.Length; ++i) {
				var tpo = tpos[i];
				if(tpo == null) continue;
				for(int j = 0; j < tpo.Length; ++j) {
				  if(tpo[j] == c) {
				     if(tpo.Length > 1) {
						tpo.Remove(j,1);
					 }
					 else {
						tpos[i] = null;
					 }
					 break;
				  }
				}
				if(tpos[i] != null) {
					if(first) {
						highestnonempty = i;
						first = false;
					}
					lowestnonempty = i;
				}
			}
			
			// now adjust the highest and lowest...
			if(highestnonempty > -1) HighestLevel = topPrice - highestnonempty*granularity;
			if(lowestnonempty > -1) LowestLevel = topPrice - lowestnonempty*granularity;
			
			// make sure we only do this once per level.
			oldbn = bn;
		}
		
		private int tposIndex(double price) {
           if(topPrice < 0) {
              topPrice = price + 10*granularity;
           }

		   int rawIndex = (int)(Math.Round((topPrice - price)/granularity));

		   if((rawIndex >= 0) && (rawIndex < tpos.Length))
			  return rawIndex;

           int oldlen = tpos.Length;
		
		    // need to shift things around...
		   if(rawIndex > 0) {
              // add to the end...
              int newlen = Math.Max(rawIndex + 1,tpos.Length+25);
              Array.Resize< StringBuilder >(ref tpos, newlen);                   
              Array.Clear(tpos,oldlen,newlen-oldlen);
              return rawIndex;
            } else {
              // add to the front...
				int newlen = Math.Max(tpos.Length - rawIndex, tpos.Length+25);
				topPrice += (newlen - tpos.Length)*granularity;
				StringBuilder[] bigger = new StringBuilder[newlen];
				Array.Clear(bigger,0,newlen-tpos.Length);
				Array.Copy(tpos,0,bigger,newlen - tpos.Length,tpos.Length);
				tpos = bigger;
      	    }
		    return (int)(Math.Round((topPrice - price)/granularity));				
		}
		
		public void add(double price, int barnum) {
		   // price should be quantized
		   // barnum should be zero based
			
		   char c = Histogram.charForNum(barnum);			
		   clearBarNum(barnum,c);
			
		   if(price > HighestLevel) HighestLevel = price;
		   if(price < LowestLevel) LowestLevel = price;
		
		   int idx = tposIndex(price);
		   if(tpos[idx] == null) {
			 tpos[idx] = new StringBuilder();
			 tpos[idx].Append(c);
		   } else {
			 var tpo = tpos[idx];
			 var size = tpo.Length;
			 if((size == 0) || 
				 (tpo[size - 1] != c) ) {
				tpo.Append(c);
			 }			
		   }		   
		}
	
		public string getString(double price) {
		   // price should be quantized
		   int idx = tposIndex(price);
		   if(tpos[idx] == null) return "";
		   return tpos[idx].ToString();
		}
  }
}
#endregion support

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// a market profile indicator
    /// </summary>
    [Description("a market profile indicator")]
    public class ZMktProfile : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int quantize = 1; // Default setting for Quantize
            private int barsPerHistogram = 20; // Default setting for BarsPerHistogram
		    private bool overlap = false; 
		    private bool starify = false;
        // User defined variables (add any user defined variables below)
			private double quantizeFactor;
		    private List<rwtMP.Histogram> hists;
		    private int lastSeen;
		    private double lastPriceSeen;
			private int barnum;
		
			private Font mpFont;
			private SolidBrush backBrush;
			private SolidBrush paintBrush;	
			private StringFormat leftFormat;
        #endregion

		private double quantizePrice(double price) {
			//Print(price + " rounds to " + quantizeFactor*Math.Round(price/quantizeFactor, MidpointRounding.AwayFromZero)); 
		  	return quantizeFactor*Math.Round(price/quantizeFactor, MidpointRounding.AwayFromZero);
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
			CalculateOnBarClose = false;
        }

		protected override void OnStartUp() {
			quantizeFactor = TickSize*quantize;	
			lastSeen = -1;
			hists = new List<rwtMP.Histogram>();
			barnum = -1;
			lastPriceSeen = -1;
			backBrush = new SolidBrush(ChartControl.BackColor);
			paintBrush = new SolidBrush(ChartControl.ForeColor);
		    leftFormat = new StringFormat();
    		leftFormat.Alignment = StringAlignment.Near;
			leftFormat.LineAlignment = StringAlignment.Center;
			leftFormat.Trimming = StringTrimming.None;
			leftFormat.FormatFlags |= StringFormatFlags.NoClip;
			leftFormat.FormatFlags |= StringFormatFlags.NoWrap;		
			bigrect = new RectangleF(0,0,0,0);
			fontHeight = -1;
		}

		protected override void OnTermination() {
			base.OnTermination();
			if(paintBrush != null) paintBrush.Dispose();	
			if(leftFormat != null) leftFormat.Dispose();
			if(backBrush != null) backBrush.Dispose();
			if(mpFont != null) mpFont.Dispose();
		}
		
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			rwtMP.Histogram h = null;
			if(lastSeen != CurrentBar) {
			  ++barnum;
			  if((barnum == 0) || 
				 ((barsPerHistogram > 0) && (barnum >= barsPerHistogram)) || 
				 Bars.FirstBarOfSession) {
				// need a new histogram...
				rwtMP.Histogram oldh = null;
				if(overlap && (hists.Count > 0)) oldh = hists[hists.Count-1];
				h = new rwtMP.Histogram(quantizeFactor,CurrentBar,oldh,starify);
			    hists.Add(h);
				barnum = 0;
			  } else {
				// use the one we already have
				h = hists[hists.Count-1];
				hists.Add(h);
			  }
			
			  // add the whole bar...
			  var lowbound = (Low[0] - (TickSize/2.0));
			  for(double p = High[0]; p >= lowbound; p -= TickSize) {				
				 h.add(quantizePrice(p),barnum);
			  }
			
			  lastSeen = CurrentBar;	
			  lastPriceSeen = Close[0];
			  return;
			}
			
			// add in everything since the last price...
			h = hists[hists.Count-1];
			var loopstart = Math.Max(Close[0],lastPriceSeen);
			var loopend = Math.Min(Close[0],lastPriceSeen);
			if(loopstart != loopend) {
				var looplowbound = loopend - (TickSize/2.0);
				for(double p = loopstart; p >= looplowbound; p -= TickSize) {
					h.add(quantizePrice(p),barnum);
			   	}
			}
			lastPriceSeen = Close[0];
			//Print(CurrentBar + " cb and " + hists.Count + " hists");
			//Print(h.getString(quantizePrice(Close[0])));
        }

		#region plotstuff
		
		public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
		{	
			if(Bars==null) return;
			int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
			int firstBar = FirstBarIndexPainted;
			min = Double.MaxValue;
			max = Double.MinValue;

			for(int indx = firstBar; indx<=lastBar;++indx) {
				if((indx <= CurrentBar) && (indx >= 1)) {
					min = Math.Min(min,Bars.GetLow(indx));
					max = Math.Max(max,Bars.GetHigh(indx));
				}
			}
						
			if(max < 0) return;
			
			// extend the top by 1 tick, to make room for text
			max = max + quantizeFactor;
			min = min - quantizeFactor;
		}		
		
		#endregion plotstuff

		private RectangleF bigrect;
		private double fontHeight;
		
public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
    if((Bars==null) || (max < 0)) return;
	backBrush.Color = ChartControl.BackColor;
	paintBrush.Color = ChartControl.AxisColor;
	
	int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
	int firstBar = FirstBarIndexPainted;
	
	var th =  ChartControl.GetYByValue(this,max) - ChartControl.GetYByValue(this,max+quantizeFactor);
	float halfHeight = th/2f;

	// a few heights and widths to set...
	bigrect.Height = th;
	bigrect.Width = 1;//barsPerHistogram*ChartControl.BarWidth;	
 
	#region setupFont
    if(th != fontHeight) {
	   // need to recompute the right fonts, then...
	   //Print("New TickHeight! " + th + " vs. "+tickHeight); // DEBUG
	   if(mpFont != null) mpFont.Dispose();
	   mpFont = new Font(FontFamily.GenericMonospace,Math.Max(bigrect.Height,2),GraphicsUnit.Pixel);
	   fontHeight = th;
	}
	#endregion setupFont
		//graphics.FillRectangle(backBrush,bounds); // just clear it...
		graphics.FillRectangle(backBrush,bounds.X,bounds.Y,bounds.Width,bounds.Height-5); // just clear it...
	
		for(int indx=firstBar;indx <= lastBar; ++indx) {
			if((indx <= CurrentBar) && (indx >= 1)) {
				var h = this.hists[indx];
				if((h != null) && (h.StartBar == indx)) {
					// set up for the bar's  location
					bigrect.X = ChartControl.GetXByBarIdx(BarsArray[0],indx);
					var ynext = ChartControl.GetYByValue(this,h.HighestLevel)-halfHeight;
					var lowboundary =  (h.LowestLevel-quantizeFactor/2.0);
					for(double cury = h.HighestLevel; cury > lowboundary; cury -= quantizeFactor) {
							var letters = h.getString(cury);							
							bigrect.Y = ynext;
							ynext = ChartControl.GetYByValue(this,cury-quantizeFactor)-halfHeight;
							bigrect.Height = ynext - bigrect.Y;
							graphics.DrawString(letters,mpFont,paintBrush,bigrect,leftFormat);
			    	}	 
				
				}
			}
		

		}
}
		
        #region Properties

        [Description("number of ticks to group together")]
        [GridCategory("Parameters")]
        public int Quantize
        {
            get { return quantize; }
            set { quantize = Math.Max(1, value); }
        }

        [Description("number of bars to group together (0 for full sessions)")]
        [GridCategory("Parameters")]
        public int BarsPerHistogram
        {
            get { return barsPerHistogram; }
            set { barsPerHistogram = Math.Max(0, value); }
        }
        [Description("display sliding window of current bar?")]
        [GridCategory("Parameters")]
        public bool OverlapBars
        {
            get { return overlap; }
            set { overlap = value; }
        }
        [Description("starify the previous bar?")]
        [GridCategory("Parameters")]
        public bool Starify
        {
            get { return starify; }
            set { starify = value; }
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
        private ZMktProfile[] cacheZMktProfile = null;

        private static ZMktProfile checkZMktProfile = new ZMktProfile();

        /// <summary>
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        public ZMktProfile ZMktProfile(int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            return ZMktProfile(Input, barsPerHistogram, overlapBars, quantize, starify);
        }

        /// <summary>
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        public ZMktProfile ZMktProfile(Data.IDataSeries input, int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            if (cacheZMktProfile != null)
                for (int idx = 0; idx < cacheZMktProfile.Length; idx++)
                    if (cacheZMktProfile[idx].BarsPerHistogram == barsPerHistogram && cacheZMktProfile[idx].OverlapBars == overlapBars && cacheZMktProfile[idx].Quantize == quantize && cacheZMktProfile[idx].Starify == starify && cacheZMktProfile[idx].EqualsInput(input))
                        return cacheZMktProfile[idx];

            lock (checkZMktProfile)
            {
                checkZMktProfile.BarsPerHistogram = barsPerHistogram;
                barsPerHistogram = checkZMktProfile.BarsPerHistogram;
                checkZMktProfile.OverlapBars = overlapBars;
                overlapBars = checkZMktProfile.OverlapBars;
                checkZMktProfile.Quantize = quantize;
                quantize = checkZMktProfile.Quantize;
                checkZMktProfile.Starify = starify;
                starify = checkZMktProfile.Starify;

                if (cacheZMktProfile != null)
                    for (int idx = 0; idx < cacheZMktProfile.Length; idx++)
                        if (cacheZMktProfile[idx].BarsPerHistogram == barsPerHistogram && cacheZMktProfile[idx].OverlapBars == overlapBars && cacheZMktProfile[idx].Quantize == quantize && cacheZMktProfile[idx].Starify == starify && cacheZMktProfile[idx].EqualsInput(input))
                            return cacheZMktProfile[idx];

                ZMktProfile indicator = new ZMktProfile();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarsPerHistogram = barsPerHistogram;
                indicator.OverlapBars = overlapBars;
                indicator.Quantize = quantize;
                indicator.Starify = starify;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZMktProfile[] tmp = new ZMktProfile[cacheZMktProfile == null ? 1 : cacheZMktProfile.Length + 1];
                if (cacheZMktProfile != null)
                    cacheZMktProfile.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZMktProfile = tmp;
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
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZMktProfile ZMktProfile(int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            return _indicator.ZMktProfile(Input, barsPerHistogram, overlapBars, quantize, starify);
        }

        /// <summary>
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZMktProfile ZMktProfile(Data.IDataSeries input, int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            return _indicator.ZMktProfile(input, barsPerHistogram, overlapBars, quantize, starify);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZMktProfile ZMktProfile(int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            return _indicator.ZMktProfile(Input, barsPerHistogram, overlapBars, quantize, starify);
        }

        /// <summary>
        /// a market profile indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.ZMktProfile ZMktProfile(Data.IDataSeries input, int barsPerHistogram, bool overlapBars, int quantize, bool starify)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZMktProfile(input, barsPerHistogram, overlapBars, quantize, starify);
        }
    }
}
#endregion
