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
    /// countback line
    /// </summary>
    [Description("countback line")]
    public class zCBL : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int levels = 3; // Default setting for Levels
		    private double sellLevel;
			private double buyLevel;
	    // fader stuff
  		private string TBName = "FADER2";
		private System.Windows.Forms.ToolStrip strip = null;
		private RWT.ZControlHost tsch = null;
		private System.Windows.Forms.TrackBar tbar = null;

        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Dot, "BuyLine"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Dot, "SellLine"));
            Overlay				= true;
			sellLevel = -1;
			buyLevel = Double.MaxValue;
        }
		
		protected override void OnStartUp() {
			base.OnStartUp();
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
		
		protected override void OnTermination() {
		  	if((strip != null) && (tsch != null)) {
				if(tsch.removeRef() <= 0) {
	        		strip.Items.Remove(tsch);
					tsch.Dispose();
				} 
		        strip = null;
  			    tsch = null;
			}
			base.OnTermination();							
		}
		
		 private void trackBar1_Scroll(object sender, System.EventArgs e)
         {
        	// Display the trackbar value in the text box.
			for(int i = 0; i < 2; ++i) {
	        	Plots[i].Pen.Color = Color.FromArgb(tbar.Value,Plots[i].Pen.Color);
			}
			ChartControl.ChartPanel.Invalidate(false);
         }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) return;
			
			if(High[0] >= High[1]) {
				var maxidx = CurrentBar;
			  	var count = 1;
				var idx = 1;
				var cursell = Low[0];
				while(count < levels) {
					if(idx >= maxidx) break;
					
				  	if(Low[idx] < cursell) {
					  ++count;
					  cursell = Low[idx];
					}
					else if(High[idx] > High[0]) {
					  cursell = -1; 
					  break;	
					}
					++idx;
				}
				if(cursell > sellLevel) sellLevel = cursell;
			}
			
			if(Low[0] <= Low[1]) {
				var maxidx = CurrentBar;
			  	var count = 1;
				var idx = 1;
				var curbuy = High[0];
				while(count < levels) {
					if(idx >= maxidx) break;
					
				  	if(High[idx] > curbuy) {
					  ++count;
					  curbuy = High[idx];
					}
					else if(Low[idx] < Low[0]) {
					  curbuy = Double.MaxValue; 
					  break;	
					}
					++idx;
				}
				if(curbuy < buyLevel) buyLevel = curbuy;				
			}
			
			
            //BuyLine.Set(Close[0]);
			if(sellLevel >= 0)
              SellLine.Set(sellLevel);
			if(buyLevel < Double.MaxValue) 
			  BuyLine.Set(buyLevel);	
		
			if(Close[0] < sellLevel) {
			  sellLevel = -1;	
			}
			if(Close[0] > buyLevel) {
			  buyLevel = Double.MaxValue;	
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BuyLine
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SellLine
        {
            get { return Values[1]; }
        }

        [Description("Levels to count back")]
        [GridCategory("Parameters")]
        public int Levels
        {
            get { return levels; }
            set { levels = Math.Max(1, value); }
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
        private zCBL[] cachezCBL = null;

        private static zCBL checkzCBL = new zCBL();

        /// <summary>
        /// countback line
        /// </summary>
        /// <returns></returns>
        public zCBL zCBL(int levels)
        {
            return zCBL(Input, levels);
        }

        /// <summary>
        /// countback line
        /// </summary>
        /// <returns></returns>
        public zCBL zCBL(Data.IDataSeries input, int levels)
        {
            if (cachezCBL != null)
                for (int idx = 0; idx < cachezCBL.Length; idx++)
                    if (cachezCBL[idx].Levels == levels && cachezCBL[idx].EqualsInput(input))
                        return cachezCBL[idx];

            lock (checkzCBL)
            {
                checkzCBL.Levels = levels;
                levels = checkzCBL.Levels;

                if (cachezCBL != null)
                    for (int idx = 0; idx < cachezCBL.Length; idx++)
                        if (cachezCBL[idx].Levels == levels && cachezCBL[idx].EqualsInput(input))
                            return cachezCBL[idx];

                zCBL indicator = new zCBL();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Levels = levels;
                Indicators.Add(indicator);
                indicator.SetUp();

                zCBL[] tmp = new zCBL[cachezCBL == null ? 1 : cachezCBL.Length + 1];
                if (cachezCBL != null)
                    cachezCBL.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezCBL = tmp;
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
        /// countback line
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zCBL zCBL(int levels)
        {
            return _indicator.zCBL(Input, levels);
        }

        /// <summary>
        /// countback line
        /// </summary>
        /// <returns></returns>
        public Indicator.zCBL zCBL(Data.IDataSeries input, int levels)
        {
            return _indicator.zCBL(input, levels);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// countback line
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zCBL zCBL(int levels)
        {
            return _indicator.zCBL(Input, levels);
        }

        /// <summary>
        /// countback line
        /// </summary>
        /// <returns></returns>
        public Indicator.zCBL zCBL(Data.IDataSeries input, int levels)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zCBL(input, levels);
        }
    }
}
#endregion
