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
    /// volume stats
    /// </summary>
    [Description("volume stats")]
    public class ZRichVolStats : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool combinedvol = false; // Default setting for combined
			private bool displaygraphical = false;
			private bool displayvolprofile = false;
			private bool displaycandles = true;
			private bool displaytotals = true;

		// support for previous session profile
		    private bool useprevsession = false;
			private int lastSessionBarNum = 0;
		
		// vars for putting traded amounts on the right...
		    private bool displaytradedamounts = false;
		    private double[] tradedPrices;
		    private long[] tradedAmounts;
		    private int taIdx;
			private int lastBarSeen;
		    private long lastVolume;
		    private bool rt;
		
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
	
			private SolidBrush paintBrush;
			private StringFormat centerFormat, leftFormat, rightFormat;
			private SolidBrush backBrush;
			private Pen linpen;
		
		// for the session volume profile...
		    private long[] vprofile;
		    private double topPrice;
		
		// the colors...
			Color upcolor = Color.Green;
			Color dncolor = Color.Red;
			Color neutcolor = Color.Black;
			Color profcolor = Color.Blue;
			int profcolorAlpha = 20;
			Color sessionProfileColor = Color.Blue;
		
		// drawing support
		private RectangleF bigrect;
		private RectangleF lilrect;
		private RectangleF totalsrect;
		private float tickHeight;
		private int barWidth;
		private Font deltaFont;
		private Font totalsFont;
		private double chartedMin;
		private double chartedMax;

		// button control
		private System.Windows.Forms.ToolStrip strip = null;
		private System.Windows.Forms.ToolStripButton button = null;
		private System.Windows.Forms.ToolStripButton button2 = null;
		
        #endregion

		#region volprofileStuff
		// grab the index of a price in our profile data....
		private int vprofIndex(double price) {
           if(topPrice < 0) {
              topPrice = price + 10*TickSize;
           }

		   int rawIndex = (int)(Math.Round((topPrice - price)/TickSize));

		   if((rawIndex >= 0) && (rawIndex < vprofile.Length))
			  return rawIndex;

           int oldlen = vprofile.Length;
		
		    // need to shift things around...
		   if(rawIndex > 0) {
              // add to the end...
              int newlen = Math.Max(rawIndex + 1,vprofile.Length+25);
              Array.Resize<long>(ref vprofile, newlen);                   
              Array.Clear(vprofile,oldlen,newlen-oldlen);
              return rawIndex;
            } else {
              // add to the front...
				int newlen = Math.Max(vprofile.Length - rawIndex, vprofile.Length+25);
				topPrice += (newlen - vprofile.Length)*TickSize;
				long[] bigger = new long[newlen];
				Array.Clear(bigger,0,newlen-vprofile.Length);
				Array.Copy(vprofile,0,bigger,newlen - vprofile.Length,vprofile.Length);
				vprofile = bigger;
      	    }
		    return (int)(Math.Round((topPrice - price)/TickSize));				
		 }
		
		private void vprofClear() {  
		    vprofile = new long[20];
		    topPrice = -1;
		}

		private void addApproximateData(double highPrice, double lowPrice, double vol) {
			long avgvol = (long)(vol / ( (highPrice - lowPrice)/TickSize ));
			for(double p = highPrice; p >= lowPrice; p -= TickSize) {
				var idx = vprofIndex(p);
			    //Print("Added approxdata... "+(avgvol)+" at price "+p+" idx "+idx); // DEBUG
				vprofile[idx] += avgvol;
			}
		}
		
		private void addExtendedData(rwt.ExtendedData ed, double highPrice, double lowPrice) {
		   	long put, pdt;
			for(double p = highPrice; p >= lowPrice; p -= TickSize) {
			   	ed.getUpDnTicksAtPrice(TickSize,p,out put, out pdt);				
				var idx = vprofIndex(p);
			    //Print("Added extradata... "+(put+pdt)+" at price "+p+" idx "+idx); // DEBUG
				vprofile[idx] += (put+pdt);
			}
		}
		#endregion volprofileStuff
		
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
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");
			
            paintBrush = new SolidBrush( ChartControl.ForeColor );
			backBrush = new SolidBrush(ChartControl.BackColor);
		    centerFormat = new StringFormat();
    		centerFormat.Alignment = StringAlignment.Center;
			centerFormat.LineAlignment = StringAlignment.Center;
			centerFormat.Trimming = StringTrimming.None;
			centerFormat.FormatFlags |= StringFormatFlags.NoClip;
			centerFormat.FormatFlags |= StringFormatFlags.NoWrap;
		    leftFormat = new StringFormat();
    		leftFormat.Alignment = StringAlignment.Near;
			leftFormat.LineAlignment = StringAlignment.Center;
			leftFormat.Trimming = StringTrimming.None;
			leftFormat.FormatFlags |= StringFormatFlags.NoClip;
			leftFormat.FormatFlags |= StringFormatFlags.NoWrap;
		    rightFormat = new StringFormat();
    		rightFormat.Alignment = StringAlignment.Far;
			rightFormat.LineAlignment = StringAlignment.Center;
			rightFormat.Trimming = StringTrimming.None;
			rightFormat.FormatFlags |= StringFormatFlags.NoClip;
			rightFormat.FormatFlags |= StringFormatFlags.NoWrap;
			linpen = new Pen(paintBrush,2);
			tradedPrices = new double[2];
			tradedAmounts = new long[2];
			taIdx = 0;
			lastBarSeen = -1;
			lastVolume = 0;
			if(displayvolprofile) vprofClear();
			chartedMin = -1;
			chartedMax = -1;
		    tickHeight = -1;
            barWidth = -1;
		    deltaFont = null;
		    totalsFont = null;
			sessionProfileColor = Color.FromArgb(profcolorAlpha,profcolor);
			bigrect = new RectangleF(0,0,0,0);
			lilrect = new RectangleF(0,0,0,0);
			totalsrect = new RectangleF(0,0,0,0);
			rt = false;
			lastSessionBarNum = 0;
			if(displaycandles) {
				System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
				if(coll.Length > 0) {
					if(displaygraphical) {
					button = new System.Windows.Forms.ToolStripButton("Graphics");
					} else {
					button = new System.Windows.Forms.ToolStripButton("Text");	
					}
					if(combinedvol) {
						button2 = new System.Windows.Forms.ToolStripButton("Combined");	
					} else {
						button2 = new System.Windows.Forms.ToolStripButton("Split");
					}
					button.Click += switchDisplay;	
					button2.Click += switchCombined;
					strip = (System.Windows.Forms.ToolStrip)coll[0];
					strip.Items.Add(button);  
					strip.Items.Add(button2);
				}         
			}
		}
		
		private void switchDisplay(object s, EventArgs e) {
  			if(button==null) return;
			displaygraphical = !displaygraphical;
			if(displaygraphical) {
				button.Text = "Graphics";	
			} else {
				button.Text = "Text";
			}
		}
		private void switchCombined(object s, EventArgs e) {
  			if(button2==null) return;
			combinedvol = !combinedvol;
			if(combinedvol) {
				button2.Text = "Combined";	
			} else {
				button2.Text = "Split";
			}
		}

		protected override void OnTermination() {
			base.OnTermination();
			if(paintBrush != null) paintBrush.Dispose();	
			if(centerFormat != null) centerFormat.Dispose();
			if(leftFormat != null) leftFormat.Dispose();
			if(rightFormat != null) rightFormat.Dispose();
			if(backBrush != null) backBrush.Dispose();
			if(linpen != null) linpen.Dispose();
			if(totalsFont != null) totalsFont.Dispose();
			if(deltaFont != null) deltaFont.Dispose();
			if((strip != null) && (button != null)) {
					strip.Items.Remove(button);
					button.Dispose();
			}
			if((strip != null) && (button2 != null)) {
					strip.Items.Remove(button2);
					button2.Dispose();
			}
			strip = null;
			button = null;
			button2 = null;
			base.OnTermination();
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
			if(CurrentBar != lastBarSeen) {
				lastBarSeen = CurrentBar;
				
				// update session's volume profile...
				if(displayvolprofile) {
					if(useprevsession) {
						if(Bars.FirstBarOfSession) {
							vprofClear();
							for(int bn = CurrentBar - lastSessionBarNum; bn >= 1; --bn) {
								addApproximateData(High[bn],Low[bn],Volume[bn]);
							}
						    lastSessionBarNum = CurrentBar;	// remember for next session
						}
					} else {
						if(Bars.FirstBarOfSession) {
						    vprofClear();	
						} else {
							var ed = extdat.getExtraData(1,Bars,CurrentBar);
							if(ed != null) {
								addExtendedData(ed, High[1], Low[1]);	
							}
						}
					}
				}
				
				if(displaytradedamounts) {
					lastVolume = 0;	
				}
			} else {
			    rt = true;	
			}

			if(displaytradedamounts) {	
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
			
			if(displaytotals) {
				// extend the bottom by enough ticks to make a reasonable area for the 
				// data zone...
				var cwidth = Math.Min(18,Math.Max(ChartControl.BarSpace/4.0,2.0));
				var cheight = 5*1.25*cwidth; // assume 5:4 height/width in font, and we need 5 lines
				// derivation:
				//  we need cheight pix.   ticks_needed = pix_needed/pix per tick
				//  pix per tick ==  numpix / numticks, so
				//  x = pix_needed*(numticks+x)/numpix
				//  x*numpix = pix_needed *numticks + pix_needed*x
				//  x(numpix - pix_needed) = pix_needed*numticks
				//   x = pix_needed*numticks / (numpix - pix_needed)
				var denom = canht - cheight;
				if(denom == 0) denom = 1;
				min = min -  ((cheight*((max-min)/TickSize)) / denom)*TickSize;		
			}
		}
		
public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
    if((Bars==null) || (chartedMax < 0)) return;
	backBrush.Color = ChartControl.BackColor;
	
	int lastBar = Math.Min(LastBarIndexPainted,Bars.Count - 1);
	int firstBar = FirstBarIndexPainted;
	
	var th =  ChartControl.GetYByValue(this,chartedMax) - ChartControl.GetYByValue(this,chartedMax+TickSize);
	float halfHeight = th/2f;

	// a few heights and widths to set...
	bigrect.Height = th;
	totalsrect.Width = ChartControl.BarSpace;	
	bigrect.Width = Math.Max(totalsrect.Width - (combinedvol?5:0),1);
	lilrect.Height =th;
	lilrect.Width = bigrect.Width/2.0f - 3;	

	#region setupFonts
    if((ChartControl.BarSpace != barWidth) || (th != tickHeight)) {
	   // need to recompute the right fonts, then...
	   //Print("New TickHeight! " + th + " vs. "+tickHeight); // DEBUG
	   barWidth = ChartControl.BarSpace;
	   tickHeight = th;
	   if(deltaFont != null) deltaFont.Dispose(); 
	   if(totalsFont != null) totalsFont.Dispose();
	   deltaFont = null;
	   totalsFont = null;
	   float totwidth = Math.Min(Math.Max(totalsrect.Width/4,2),18);
	   totalsFont =  new Font(FontFamily.GenericMonospace,totwidth,GraphicsUnit.Pixel);	
	   float delwidth = Math.Max(bigrect.Width/6,2);
	   deltaFont =  new Font(FontFamily.GenericMonospace,delwidth,GraphicsUnit.Pixel);
	   if(deltaFont.Height > th) {
			deltaFont.Dispose();
			deltaFont = new Font(FontFamily.GenericMonospace,Math.Max(bigrect.Height,2),GraphicsUnit.Pixel);
	   }
	}
	totalsrect.Height = totalsFont.Height;	
	#endregion setupFonts
	
	// set up the dividing line for the summary information
	#region setupdivider
	double linprice = chartedMin;
	float linlvl = ChartControl.GetYByValue(this,linprice-0.5*TickSize);
	if(displaytotals) {
		// put the linlvl at the bottom, if that's lower than just below the 
		// lowest chart price.
		var fromBottom = bounds.Bottom - totalsFont.Height*4.5f;
		//Print("linlvl vs. frombottom: "+linlvl+" "+fromBottom);
		if(fromBottom > linlvl) {
		linlvl = fromBottom;
		linprice = Bars.Instrument.MasterInstrument.Round2TickSize(min +  TickSize*(-1+(totalsFont.Height*4.5f)/tickHeight));
		while(ChartControl.GetYByValue(this,linprice) < linlvl) linprice -= TickSize;
		while(ChartControl.GetYByValue(this,linprice) > linlvl) linprice += TickSize;
		linprice = Bars.Instrument.MasterInstrument.Round2TickSize(linprice);
		}
	} else {
	  linprice = -1;
	  linlvl = ChartControl.GetYByValue(this,min) + 999;
	}
	//Print("Final: "+linlvl);	
	#endregion setupdivider
	
	// if we are using the graphical display, find the largest volume
	#region findlargestvol
	long mvol = 1;
	if(displaycandles && displaygraphical) {
		for(int indx=firstBar;indx <= lastBar; ++indx) {
			if((indx <= CurrentBar) && (indx >= 1)) {		
		  	    var ed = extdat.getExtraData(CurrentBar-indx,this.Bars,CurrentBar); 
				if(ed != null) {
					mvol = Math.Max(mvol,ed.findLargestLevel(combinedvol));
				}
			}
		}
	}
	#endregion findlargestvol
	
	if(displaytotals || displaycandles) {
		for(int indx=firstBar;indx <= lastBar; ++indx) {
			if((indx <= CurrentBar) && (indx >= 1)) {		
				var ed = extdat.getExtraData(CurrentBar-indx,this.Bars,CurrentBar); 
				if(ed != null) {
					// set up for the bar's  location
					#region barXYlocation
					int x = ChartControl.GetXByBarIdx(BarsArray[0],indx);
					totalsrect.X = x - 0.5f*barWidth;
					bigrect.X = totalsrect.X + (combinedvol?5:0);
					#endregion barXYlocation
																			
					// draw on the candle
					#region drawcandle			
					if(displaycandles) {	
						double miny = Bars.GetLow(indx);
						double maxy = Bars.GetHigh(indx);							
						var candleY = ChartControl.GetYByValue(this,maxy);
						var candleH = ChartControl.GetYByValue(this,miny) - candleY;
						
						graphics.FillRectangle(backBrush,bigrect.X,candleY-bigrect.Height,bigrect.Width,candleH+2*bigrect.Height);
						
						var openCoord = ChartControl.GetYByValue(this,Bars.GetOpen(indx));
						var closeCoord = ChartControl.GetYByValue(this,Bars.GetClose(indx));				
						paintBrush.Color = ChartControl.ChartStyle.Pen2.Color; // candle wick color...
						var stemX = (combinedvol?(bigrect.X-4):(bigrect.X+lilrect.Width+2));
						graphics.FillRectangle(paintBrush,stemX,candleY,2,candleH);
						if(openCoord == closeCoord) {
							graphics.FillRectangle(paintBrush,stemX-1,openCoord-1,4,2);
						} else if(openCoord > closeCoord) {
							paintBrush.Color = ChartControl.UpColor;
							graphics.FillRectangle(paintBrush,stemX-1,closeCoord,4,openCoord-closeCoord);					
						} else {
							paintBrush.Color = ChartControl.DownColor;
							graphics.FillRectangle(paintBrush,stemX-1,openCoord,4,closeCoord-openCoord);					
						}
						
						// now paint the bar prices...
						#region barpainting
						long put, pdt;  // price upticks, price downticks
						var ynext = ChartControl.GetYByValue(this,maxy)-halfHeight;
						var lowboundary =  (miny-TickSize/2);
						for(double cury = maxy; cury > lowboundary; cury -= TickSize) {
							ed.getUpDnTicksAtPrice(TickSize,cury,out put, out pdt);
							
							bigrect.Y = ynext;
							ynext = ChartControl.GetYByValue(this,cury-TickSize)-halfHeight;
							bigrect.Height = ynext - bigrect.Y;
							lilrect.Y = bigrect.Y;
							lilrect.Height = bigrect.Height;
							
							if(combinedvol) {
							var cvol = put + pdt;
							if(cvol < 1) cvol = 1;
							var ratio  = ((float)put)/((float)cvol);
							if(ratio >= 0.75) paintBrush.Color = upcolor;
							else if(ratio <= 0.25) paintBrush.Color = dncolor;
							else paintBrush.Color = neutcolor;
							if(displaygraphical) {
								var w = ((float)cvol)*bigrect.Width/((float)mvol);
								graphics.FillRectangle(paintBrush,bigrect.X,bigrect.Y,w,bigrect.Height);
							} else {
								graphics.DrawString(cvol.ToString(),deltaFont,paintBrush,bigrect,leftFormat);
							}
							} else {
							paintBrush.Color = dncolor;			
							lilrect.X = bigrect.X;	
							if(displaygraphical) {
								var w = ((float)pdt)*lilrect.Width/((float)mvol);
								graphics.FillRectangle(paintBrush,lilrect.X + lilrect.Width - w,
														lilrect.Y,w,lilrect.Height);
							} else {
								graphics.DrawString(pdt.ToString(),deltaFont,paintBrush,lilrect,rightFormat);							
							}
							paintBrush.Color = upcolor;	
							lilrect.X += lilrect.Width + 6;					
							if(displaygraphical) {
								var w = ((float)put)*lilrect.Width/((float)mvol);
								graphics.FillRectangle(paintBrush,lilrect.X,lilrect.Y,w,lilrect.Height);
							} else {
								graphics.DrawString(put.ToString(),deltaFont,paintBrush,lilrect,leftFormat);
							}
							}
						}
						#endregion barpainting	
					}
					#endregion drawcandle				

					// draw totals at the bottom
					#region drawtotals
					if(displaytotals) {
						double denom = Math.Max(1,ed.UpTicks + ed.DnTicks);
						
						paintBrush.Color = upcolor;
						totalsrect.Y = linlvl;
						int pctage = (int)(Math.Round((double)ed.UpTicks*100.0/denom));
						graphics.DrawString(String.Format("{0}%",pctage),totalsFont,paintBrush,totalsrect,centerFormat);
						totalsrect.Y = totalsrect.Y + totalsFont.Height;
						graphics.DrawString(ed.UpTicks.ToString(),totalsFont,paintBrush,totalsrect,centerFormat);
										
						paintBrush.Color = dncolor;			
						totalsrect.Y = totalsrect.Y + totalsFont.Height;
						graphics.DrawString(ed.DnTicks.ToString(),totalsFont,paintBrush,totalsrect,centerFormat);		
						totalsrect.Y = totalsrect.Y + totalsFont.Height;
						graphics.DrawString(String.Format("{0}%",100-pctage),totalsFont,paintBrush,totalsrect,centerFormat);
					}
					#endregion drawtotals
					
			    } 
				
			}
		}
	}

	// draw the dividing line
	#region drawdivider
	if(displaytotals) {
		linpen.Color = neutcolor;
		graphics.DrawLine(linpen,bounds.Left,linlvl,bounds.Right,linlvl);
		//Print("totalsFont height *4 = "+(4*totalsFont.Height));
	}
	#endregion drawdivider
	
	// now display the volume profile, if needed...
	#region dispvolprofile
	if(displayvolprofile) {
		var edcbar = extdat.getExtraData(0,Bars,CurrentBar);
		if(edcbar != null) {
			var dispmax = Bars.Instrument.MasterInstrument.Round2TickSize(max);
			var dispmin = Bars.Instrument.MasterInstrument.Round2TickSize(Math.Max(linprice,min));
			var idxhigh = vprofIndex(dispmax);
			var idxlow = vprofIndex(dispmin);
			long maxvprof = 1;
			
			// figure out where the current bar fits in...
			int idxcbarh = idxhigh + (int)Math.Round((dispmax - Bars.GetHigh(CurrentBar))/TickSize);
			int idxcbarl = idxhigh + (int)Math.Round((dispmax - Bars.GetLow(CurrentBar))/TickSize);
		    long stored, recentup, recentdn;
			for(int i = idxhigh; i <= idxlow; ++i) {
				stored = vprofile[i];
				if((!useprevsession) && (i >= idxcbarh) && (i <= idxcbarl)) {
				    edcbar.getUpDnTicksAtPrice(TickSize, dispmax-((i-idxhigh)*TickSize),out recentup, out recentdn);
				   stored += (recentup + recentdn);
				}
				if(stored > maxvprof) {  maxvprof=stored; }
			}
			
			paintBrush.Color = sessionProfileColor;
			
			var profY2 = ChartControl.GetYByValue(this,dispmax) - halfHeight;
			for(int i = idxhigh; i <= idxlow; ++i) {
				var profY = profY2;
				profY2 = ChartControl.GetYByValue(this,dispmax - (i-idxhigh+1)*TickSize) - halfHeight;
				if(profY2 > linlvl) profY2 = linlvl;
				if(profY == profY2) continue;
				stored = vprofile[i];
				if((!useprevsession) && (i >= idxcbarh) && (i <= idxcbarl)) {
				   edcbar.getUpDnTicksAtPrice(TickSize, dispmax-((i-idxhigh)*TickSize),out recentup, out recentdn);
				   stored += (recentup + recentdn);
				}
				if(stored > 0) {
					graphics.FillRectangle(paintBrush,bounds.Left,profY,stored*(bounds.Width/2)/maxvprof,profY2-profY);
				}
			}
		}
	}
	#endregion dispvolprofile
		
    // now paint the tradedamounts, if needed....
	#region tradedamts
		if(displaytradedamounts && (lastBar == CurrentBar)) {
			paintBrush.Color = neutcolor;
			lilrect.X = ChartControl.GetXByBarIdx(BarsArray[0],lastBar) + barWidth;
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

        [Description("combine up and downticks into one volume amt?")]
        [GridCategory("Parameters")]
        public bool CombinedVolume
        {
            get { return combinedvol; }
            set { combinedvol = value; }
        }
        [Description("display data as a graph?")]
        [GridCategory("Parameters")]
        public bool DisplayGraphical
        {
            get { return displaygraphical; }
            set { displaygraphical = value; }
        }
		[Description("display data over the candles?")]
        [GridCategory("Parameters")]
        public bool DisplayOverCandles
        {
            get { return displaycandles; }
            set { displaycandles = value; }
        }
        [Description("display traded amounts to the right?")]
        [GridCategory("Parameters")]
        public bool DisplayTradedAmounts	
        {
            get { return displaytradedamounts; }
            set { displaytradedamounts = value; }
        }
        [Description("display volume profile on the left?")]
        [GridCategory("Parameters")]
        public bool DisplaySessionProfile	
        {
            get { return displayvolprofile; }
            set { displayvolprofile = value; }
        }
		[Description("display volume profile from previous session?")]
        [GridCategory("Parameters")]
        public bool UsePreviousSessionProfile	
        {
            get { return useprevsession; }
            set { useprevsession = value; }
        }
        [Description("display totals on the bottom?")]
        [GridCategory("Parameters")]
        public bool DisplayTotals	
        {
            get { return displaytotals; }
            set { displaytotals = value; }
        }
        [Description("upcolor")]
        [GridCategory("Parameters")]
        public Color ColorUpInfo	
        {
            get { return upcolor; }
            set { upcolor = value; }
        }
        [Browsable(false)]
        public string upcolorSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upcolor); }
           set { upcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("dncolor")]
        [GridCategory("Parameters")]
        public Color ColorDownInfo	
        {
            get { return dncolor; }
            set { dncolor = value; }
        }
        [Browsable(false)]
        public string dncolorSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dncolor); }
           set { dncolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
		
		[Description("session profile base color")]
        [GridCategory("Parameters")]
        public Color ColorSessionProfile	
        {
            get { return profcolor; }
            set { profcolor = value; }
        }
        [Browsable(false)]
        public string profcolorSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(profcolor); }
           set { profcolor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
		
		[Description("opacity (1 to 255) for session profile")]
        [GridCategory("Parameters")]
        public int ColorSessionAlpha
        {
            get { return profcolorAlpha; }
            set { profcolorAlpha = value; }
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
        private ZRichVolStats[] cacheZRichVolStats = null;

        private static ZRichVolStats checkZRichVolStats = new ZRichVolStats();

        /// <summary>
        /// volume stats
        /// </summary>
        /// <returns></returns>
        public ZRichVolStats ZRichVolStats(Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            return ZRichVolStats(Input, colorDownInfo, colorNeutralInfo, colorSessionAlpha, colorSessionProfile, colorUpInfo, combinedVolume, displayGraphical, displayOverCandles, displaySessionProfile, displayTotals, displayTradedAmounts, usePreviousSessionProfile);
        }

        /// <summary>
        /// volume stats
        /// </summary>
        /// <returns></returns>
        public ZRichVolStats ZRichVolStats(Data.IDataSeries input, Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            if (cacheZRichVolStats != null)
                for (int idx = 0; idx < cacheZRichVolStats.Length; idx++)
                    if (cacheZRichVolStats[idx].ColorDownInfo == colorDownInfo && cacheZRichVolStats[idx].ColorNeutralInfo == colorNeutralInfo && cacheZRichVolStats[idx].ColorSessionAlpha == colorSessionAlpha && cacheZRichVolStats[idx].ColorSessionProfile == colorSessionProfile && cacheZRichVolStats[idx].ColorUpInfo == colorUpInfo && cacheZRichVolStats[idx].CombinedVolume == combinedVolume && cacheZRichVolStats[idx].DisplayGraphical == displayGraphical && cacheZRichVolStats[idx].DisplayOverCandles == displayOverCandles && cacheZRichVolStats[idx].DisplaySessionProfile == displaySessionProfile && cacheZRichVolStats[idx].DisplayTotals == displayTotals && cacheZRichVolStats[idx].DisplayTradedAmounts == displayTradedAmounts && cacheZRichVolStats[idx].UsePreviousSessionProfile == usePreviousSessionProfile && cacheZRichVolStats[idx].EqualsInput(input))
                        return cacheZRichVolStats[idx];

            lock (checkZRichVolStats)
            {
                checkZRichVolStats.ColorDownInfo = colorDownInfo;
                colorDownInfo = checkZRichVolStats.ColorDownInfo;
                checkZRichVolStats.ColorNeutralInfo = colorNeutralInfo;
                colorNeutralInfo = checkZRichVolStats.ColorNeutralInfo;
                checkZRichVolStats.ColorSessionAlpha = colorSessionAlpha;
                colorSessionAlpha = checkZRichVolStats.ColorSessionAlpha;
                checkZRichVolStats.ColorSessionProfile = colorSessionProfile;
                colorSessionProfile = checkZRichVolStats.ColorSessionProfile;
                checkZRichVolStats.ColorUpInfo = colorUpInfo;
                colorUpInfo = checkZRichVolStats.ColorUpInfo;
                checkZRichVolStats.CombinedVolume = combinedVolume;
                combinedVolume = checkZRichVolStats.CombinedVolume;
                checkZRichVolStats.DisplayGraphical = displayGraphical;
                displayGraphical = checkZRichVolStats.DisplayGraphical;
                checkZRichVolStats.DisplayOverCandles = displayOverCandles;
                displayOverCandles = checkZRichVolStats.DisplayOverCandles;
                checkZRichVolStats.DisplaySessionProfile = displaySessionProfile;
                displaySessionProfile = checkZRichVolStats.DisplaySessionProfile;
                checkZRichVolStats.DisplayTotals = displayTotals;
                displayTotals = checkZRichVolStats.DisplayTotals;
                checkZRichVolStats.DisplayTradedAmounts = displayTradedAmounts;
                displayTradedAmounts = checkZRichVolStats.DisplayTradedAmounts;
                checkZRichVolStats.UsePreviousSessionProfile = usePreviousSessionProfile;
                usePreviousSessionProfile = checkZRichVolStats.UsePreviousSessionProfile;

                if (cacheZRichVolStats != null)
                    for (int idx = 0; idx < cacheZRichVolStats.Length; idx++)
                        if (cacheZRichVolStats[idx].ColorDownInfo == colorDownInfo && cacheZRichVolStats[idx].ColorNeutralInfo == colorNeutralInfo && cacheZRichVolStats[idx].ColorSessionAlpha == colorSessionAlpha && cacheZRichVolStats[idx].ColorSessionProfile == colorSessionProfile && cacheZRichVolStats[idx].ColorUpInfo == colorUpInfo && cacheZRichVolStats[idx].CombinedVolume == combinedVolume && cacheZRichVolStats[idx].DisplayGraphical == displayGraphical && cacheZRichVolStats[idx].DisplayOverCandles == displayOverCandles && cacheZRichVolStats[idx].DisplaySessionProfile == displaySessionProfile && cacheZRichVolStats[idx].DisplayTotals == displayTotals && cacheZRichVolStats[idx].DisplayTradedAmounts == displayTradedAmounts && cacheZRichVolStats[idx].UsePreviousSessionProfile == usePreviousSessionProfile && cacheZRichVolStats[idx].EqualsInput(input))
                            return cacheZRichVolStats[idx];

                ZRichVolStats indicator = new ZRichVolStats();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorDownInfo = colorDownInfo;
                indicator.ColorNeutralInfo = colorNeutralInfo;
                indicator.ColorSessionAlpha = colorSessionAlpha;
                indicator.ColorSessionProfile = colorSessionProfile;
                indicator.ColorUpInfo = colorUpInfo;
                indicator.CombinedVolume = combinedVolume;
                indicator.DisplayGraphical = displayGraphical;
                indicator.DisplayOverCandles = displayOverCandles;
                indicator.DisplaySessionProfile = displaySessionProfile;
                indicator.DisplayTotals = displayTotals;
                indicator.DisplayTradedAmounts = displayTradedAmounts;
                indicator.UsePreviousSessionProfile = usePreviousSessionProfile;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichVolStats[] tmp = new ZRichVolStats[cacheZRichVolStats == null ? 1 : cacheZRichVolStats.Length + 1];
                if (cacheZRichVolStats != null)
                    cacheZRichVolStats.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichVolStats = tmp;
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
        /// volume stats
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichVolStats ZRichVolStats(Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            return _indicator.ZRichVolStats(Input, colorDownInfo, colorNeutralInfo, colorSessionAlpha, colorSessionProfile, colorUpInfo, combinedVolume, displayGraphical, displayOverCandles, displaySessionProfile, displayTotals, displayTradedAmounts, usePreviousSessionProfile);
        }

        /// <summary>
        /// volume stats
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichVolStats ZRichVolStats(Data.IDataSeries input, Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            return _indicator.ZRichVolStats(input, colorDownInfo, colorNeutralInfo, colorSessionAlpha, colorSessionProfile, colorUpInfo, combinedVolume, displayGraphical, displayOverCandles, displaySessionProfile, displayTotals, displayTradedAmounts, usePreviousSessionProfile);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// volume stats
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichVolStats ZRichVolStats(Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            return _indicator.ZRichVolStats(Input, colorDownInfo, colorNeutralInfo, colorSessionAlpha, colorSessionProfile, colorUpInfo, combinedVolume, displayGraphical, displayOverCandles, displaySessionProfile, displayTotals, displayTradedAmounts, usePreviousSessionProfile);
        }

        /// <summary>
        /// volume stats
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichVolStats ZRichVolStats(Data.IDataSeries input, Color colorDownInfo, Color colorNeutralInfo, int colorSessionAlpha, Color colorSessionProfile, Color colorUpInfo, bool combinedVolume, bool displayGraphical, bool displayOverCandles, bool displaySessionProfile, bool displayTotals, bool displayTradedAmounts, bool usePreviousSessionProfile)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichVolStats(input, colorDownInfo, colorNeutralInfo, colorSessionAlpha, colorSessionProfile, colorUpInfo, combinedVolume, displayGraphical, displayOverCandles, displaySessionProfile, displayTotals, displayTradedAmounts, usePreviousSessionProfile);
        }
    }
}
#endregion
