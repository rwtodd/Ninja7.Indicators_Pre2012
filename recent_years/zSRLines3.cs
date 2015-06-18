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

namespace rwt {
  class SRLine {
	private double wAverage;
	private double sWeights;
	private double sTotalWeight;
	private int age;
	private bool enabled;
	private bool simpleSMA;
	private int whichPlot;
	private double initialValue;
	private bool fromHigh;
	
	public SRLine(bool simple, int wP, bool fromH) {
	  wAverage = 0.0;
	  sTotalWeight = 0.0;
	  sWeights = 0.0;
	  initialValue = 0.0;
	  enabled = false;
	  age = 0;
	  simpleSMA = simple;
	  whichPlot = wP;
	  fromHigh = fromH;
	}
	
	private void reset() {
	  wAverage = 0.0;
	  sTotalWeight = 0.0;
	  sWeights = 0.0;
	  age = 0;
	  initialValue = 0.0;
	  enabled = true;
	}
	
	public bool Enabled {
	  get { return enabled; }
	  set { if(value) { reset(); } 
	        enabled = value; 
		}
	}
	
	public double Location {
		get { return wAverage; } 
	}
	
	public int WhichPlot {
	    get { return whichPlot; }	
	}
		
	public int Age {
	  get { return age; }	
	}
	public bool isOlder(SRLine other) {  return age > other.age; }
	
	public void newBar(double v, double p, double h, double l) {
	  ++age;
	  if(enabled) {
		var weight = (simpleSMA?1.0:v);
		sTotalWeight += weight;
		sWeights += (p * weight);
		wAverage = ( sWeights/sTotalWeight );
		if(age==1) { 
			initialValue = (fromHigh)?h:l; 
		}
		if(fromHigh && (initialValue < h)) enabled = false;
		if(!fromHigh && (initialValue > l)) enabled = false;
	  }	  
	}
	
  }
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// SR lines based on swing highs and lows
    /// </summary>
    [Description("SR lines based on swing highs and lows")]
    public class zSRLines3 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int swingStrength = 7; // Default setting for SwingStrength
			private rwt.SRLine[] slines = null;
		    private rwt.SRLine[] rlines = null;
		    private zFastMAX mx = null;
		    private zFastMIN mn = null;
		    private double mxSeen, mnSeen;
		    private int barsAgo = 0;
		    private bool lookForHigh = true;
		    private int plotWithin = 50; // how many ticks to plot within
		    private double tolerance = 0.0; // how many points to plot within
			private string TBName = "FADER";
			private System.Windows.Forms.ToolStrip strip = null;
			private RWT.ZControlHost tsch = null;
 			private System.Windows.Forms.TrackBar tbar = null;
		
		    private delegate void newBarHandler(double v, double p, double h, double l);
		    private event newBarHandler newBar;
		
        // User defined variables (add any user defined variables below)
        #endregion

		private rwt.SRLine availableLine(bool fromHigh) {
		  var lines = (fromHigh?rlines:slines);
		  rwt.SRLine oldest = null;
		  foreach(rwt.SRLine l in lines) {			
			 if(oldest == null) oldest = l;
			
			 if(!oldest.Enabled) {
			   if(!l.Enabled && l.isOlder(oldest)) { oldest = l; }
			 } else {
			   if(!l.Enabled) oldest = l;
			   else if(l.isOlder(oldest)) oldest = l;
			 }
		  }
		  oldest.Enabled = false;
		  return oldest;		
		}
				
		private void addLine(int ba /*bars ago*/, bool swingHigh) {			
			// locate an entry in the array...
			rwt.SRLine l = availableLine(swingHigh);
			l.Enabled = true;
			
			// backfill the data and plot...
			if(CurrentBar > ba) Values[l.WhichPlot].Reset(ba+1);
			for(int i = ba; i >= 0; --i) {
			  l.newBar(Volume[i],Median[i],High[i],Low[i]);
			  Values[l.WhichPlot].Set(i,l.Location);
			}
		}
		
		protected override void OnStartUp() {
		   	slines = new rwt.SRLine[4];
			rlines = new rwt.SRLine[4];
			for(int i = 0; i < slines.Length; ++i)
			{
				slines[i] = new rwt.SRLine((Bars.BarsType.PeriodType == PeriodType.Volume),i,false);
				newBar += slines[i].newBar;
				rlines[i] = new rwt.SRLine((Bars.BarsType.PeriodType == PeriodType.Volume),i+4,true);
				newBar += rlines[i].newBar;
			}
			mx = zFastMAX(High,swingStrength);
			mn = zFastMIN(Low,swingStrength);
			lookForHigh = true;
			mxSeen = -1;
			mnSeen = 0;
			barsAgo  = -1;
			tolerance = plotWithin * TickSize;
			//if(Bars.BarsType.PeriodType==PeriodType.Volume) Log("Able to use simple volume!",LogLevel.Information);

            // ZFader Setup.....keep consistent with ZFader indicator...
			System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
   			if(coll.Length > 0) {
				strip = (System.Windows.Forms.ToolStrip)coll[0];
				System.Windows.Forms.ToolStripItem[] slider = strip.Items.Find(TBName,false);
				if(slider.Length > 0) {
					tsch = (RWT.ZControlHost)slider[0];
					tsch.addRef();
					tbar = (System.Windows.Forms.TrackBar)tsch.Control;
					tbar.Scroll += new EventHandler(trackBar1_Scroll);
				} else {
					tbar = new System.Windows.Forms.TrackBar();
					tbar.Maximum = 255;
					tbar.Minimum = 0;
					tbar.Value = 255;
					tbar.Scroll += new EventHandler(trackBar1_Scroll);
     				tsch = new RWT.ZControlHost(tbar,TBName);
					tsch.addRef();
     				strip.Items.Add(tsch);   
				}
   			}        
			
			
	   }
		
		private void trackBar1_Scroll(object sender, System.EventArgs e)
		{
			for(int i = 0; i < 8; ++i) {
				Plots[i].Pen.Color = Color.FromArgb(tbar.Value,Plots[i].Pen.Color);
			}
			ChartControl.ChartPanel.Invalidate(false);
		}
		
		protected override void OnTermination() {
		  	if((strip != null) && (tsch != null)) {
				if(tsch.removeRef() <= 0) {
	        		strip.Items.Remove(tsch);
				} 
		        strip = null;
  			    tsch = null;
			}				
		}
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "SRLine1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "SRLine2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Teal), PlotStyle.Line, "SRLine3"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DodgerBlue), PlotStyle.Line, "SRLine4"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "SRLine5"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkOrange), PlotStyle.Line, "SRLine6"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Magenta), PlotStyle.Line, "SRLine7"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Purple), PlotStyle.Line, "SRLine8"));
            Overlay				= true;
			for(int i = 0; i < 8; ++i) Plots[i].Pen.Width = 1;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			barsAgo = barsAgo + 1;
			if(CurrentBar < 2) return;
			
			newBar(Volume[0],Median[0],High[0],Low[0]);
			
			// look for a new swing...
			if(lookForHigh) {
   				if(High[0] > mxSeen) {
			  		mxSeen = High[0];
			  		barsAgo = 0;
				}
				
  			    if(mn[0] < mn[1]) {
				  lookForHigh = false;
				  addLine(barsAgo,true);
				  mnSeen = Low[0];
				  barsAgo = 0;
			    }
			} else {
				if(Low[0] < mnSeen) {
				   mnSeen = Low[0];
				   barsAgo = 0;
				}
				
				if(mx[0] > mx[1]) {
				   lookForHigh = true;
				   addLine(barsAgo,false);
				   mxSeen = High[0];
				   barsAgo = 0;
				}
			}
			
			foreach(rwt.SRLine l in slines) {
			  if(l.Enabled) {
				var loc = l.Location;
				if(loc >= mx[0]) { l.Enabled = false; }
				else if(Math.Abs(Close[0]-loc) < tolerance) {
				  Values[l.WhichPlot].Set(loc);
				}
			  }
			}
			
			foreach(rwt.SRLine l in rlines) {
			  if(l.Enabled) {
				var loc = l.Location;

				if(loc <= mn[0]) { l.Enabled = false; }
				else if(Math.Abs(Close[0]-loc) < tolerance) {
				  Values[l.WhichPlot].Set(loc);
				}
			  }
			}
        }

        #region Properties
        [Description("SwingStrength")]
        [GridCategory("Parameters")]
        public int SwingStrength
        {
            get { return swingStrength; }
            set { swingStrength = Math.Max(2, value); }
        }
		
		[Description("PlotWithin")]
        [GridCategory("Parameters")]
        public int PlotWithin
        {
            get { return plotWithin; }
            set { plotWithin = Math.Max(1, value); }
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
        private zSRLines3[] cachezSRLines3 = null;

        private static zSRLines3 checkzSRLines3 = new zSRLines3();

        /// <summary>
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        public zSRLines3 zSRLines3(int plotWithin, int swingStrength)
        {
            return zSRLines3(Input, plotWithin, swingStrength);
        }

        /// <summary>
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        public zSRLines3 zSRLines3(Data.IDataSeries input, int plotWithin, int swingStrength)
        {
            if (cachezSRLines3 != null)
                for (int idx = 0; idx < cachezSRLines3.Length; idx++)
                    if (cachezSRLines3[idx].PlotWithin == plotWithin && cachezSRLines3[idx].SwingStrength == swingStrength && cachezSRLines3[idx].EqualsInput(input))
                        return cachezSRLines3[idx];

            lock (checkzSRLines3)
            {
                checkzSRLines3.PlotWithin = plotWithin;
                plotWithin = checkzSRLines3.PlotWithin;
                checkzSRLines3.SwingStrength = swingStrength;
                swingStrength = checkzSRLines3.SwingStrength;

                if (cachezSRLines3 != null)
                    for (int idx = 0; idx < cachezSRLines3.Length; idx++)
                        if (cachezSRLines3[idx].PlotWithin == plotWithin && cachezSRLines3[idx].SwingStrength == swingStrength && cachezSRLines3[idx].EqualsInput(input))
                            return cachezSRLines3[idx];

                zSRLines3 indicator = new zSRLines3();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PlotWithin = plotWithin;
                indicator.SwingStrength = swingStrength;
                Indicators.Add(indicator);
                indicator.SetUp();

                zSRLines3[] tmp = new zSRLines3[cachezSRLines3 == null ? 1 : cachezSRLines3.Length + 1];
                if (cachezSRLines3 != null)
                    cachezSRLines3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezSRLines3 = tmp;
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
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSRLines3 zSRLines3(int plotWithin, int swingStrength)
        {
            return _indicator.zSRLines3(Input, plotWithin, swingStrength);
        }

        /// <summary>
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        public Indicator.zSRLines3 zSRLines3(Data.IDataSeries input, int plotWithin, int swingStrength)
        {
            return _indicator.zSRLines3(input, plotWithin, swingStrength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSRLines3 zSRLines3(int plotWithin, int swingStrength)
        {
            return _indicator.zSRLines3(Input, plotWithin, swingStrength);
        }

        /// <summary>
        /// SR lines based on swing highs and lows
        /// </summary>
        /// <returns></returns>
        public Indicator.zSRLines3 zSRLines3(Data.IDataSeries input, int plotWithin, int swingStrength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zSRLines3(input, plotWithin, swingStrength);
        }
    }
}
#endregion
