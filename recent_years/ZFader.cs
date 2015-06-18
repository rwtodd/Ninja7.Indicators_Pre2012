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

/*
 * 2011 Richard Todd
 * ZFader -- lets you fade in and out price-action indicators...
*/
namespace RWT {
  class ZControlHost : System.Windows.Forms.ToolStripControlHost {
	private int refCount;
	public ZControlHost(System.Windows.Forms.Control ctl, string name) :base(ctl,name) {
		refCount = 0;	
	}
	public void addRef() { ++refCount; }
	public int removeRef() { return --refCount; }
  }
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Allows you to fade in or out an indictator.
    /// </summary>
    [Description("2011 Richard Todd Allows you to fade in or out an indictator.")]
    public class ZFader : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private System.Windows.Forms.ToolStrip strip = null;
		private RWT.ZControlHost tsch = null;
		private System.Windows.Forms.TrackBar tbar = null;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot"));
            Overlay				= true;
        }

		private string TBName = "FADER";
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
        	Plots[0].Pen.Color = Color.FromArgb(tbar.Value,Plots[0].Pen.Color);
			//ChartControl.Invalidate(ChartControl.Region);
			ChartControl.ChartPanel.Invalidate(false);
         }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot.Set(Input[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot
        {
            get { return Values[0]; }
        }
		[Description("Name of the Fader Group.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Fader Group")]
		public string FaderGroup
		{
			get { return TBName; }
			set { TBName = value; }
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
        private ZFader[] cacheZFader = null;

        private static ZFader checkZFader = new ZFader();

        /// <summary>
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        public ZFader ZFader(string faderGroup)
        {
            return ZFader(Input, faderGroup);
        }

        /// <summary>
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        public ZFader ZFader(Data.IDataSeries input, string faderGroup)
        {
            if (cacheZFader != null)
                for (int idx = 0; idx < cacheZFader.Length; idx++)
                    if (cacheZFader[idx].FaderGroup == faderGroup && cacheZFader[idx].EqualsInput(input))
                        return cacheZFader[idx];

            lock (checkZFader)
            {
                checkZFader.FaderGroup = faderGroup;
                faderGroup = checkZFader.FaderGroup;

                if (cacheZFader != null)
                    for (int idx = 0; idx < cacheZFader.Length; idx++)
                        if (cacheZFader[idx].FaderGroup == faderGroup && cacheZFader[idx].EqualsInput(input))
                            return cacheZFader[idx];

                ZFader indicator = new ZFader();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FaderGroup = faderGroup;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZFader[] tmp = new ZFader[cacheZFader == null ? 1 : cacheZFader.Length + 1];
                if (cacheZFader != null)
                    cacheZFader.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZFader = tmp;
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
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZFader ZFader(string faderGroup)
        {
            return _indicator.ZFader(Input, faderGroup);
        }

        /// <summary>
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZFader ZFader(Data.IDataSeries input, string faderGroup)
        {
            return _indicator.ZFader(input, faderGroup);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZFader ZFader(string faderGroup)
        {
            return _indicator.ZFader(Input, faderGroup);
        }

        /// <summary>
        /// 2011 Richard Todd Allows you to fade in or out an indictator.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZFader ZFader(Data.IDataSeries input, string faderGroup)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZFader(input, faderGroup);
        }
    }
}
#endregion
