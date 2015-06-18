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
    /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
    /// </summary>
    [Description("Guppy Stuff")]
    public class zMultEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		    private int periodStart = 5;
			private int periodAdder = 2;
			private double[] alphas;  // typical: 5,2 30,5 110,10 320,20 
			private Color bandColor = Color.Orange; // blue green purple
			private Color firstColor = Color.Crimson; 
		
	    // fader stuff
		private string TBName = "FADER";
		private System.Windows.Forms.ToolStrip strip = null;
		private RWT.ZControlHost tsch = null;
		private System.Windows.Forms.TrackBar tbar = null;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha01"));
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha02"));
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha03"));
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha04"));
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha05"));			
            Add(new Plot(bandColor, PlotStyle.Line, "Alpha06"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			base.OnStartUp();

			alphas = new double[6];
            double per=periodStart;
			for(int i = 0; i < 6; ++i, per += periodAdder) { 
				Plots[i].Pen.Width = 1;
				Plots[i].Pen.Color = (i==0)?firstColor:bandColor;
				alphas[i] = 2.0/(per+1.0);
			}
			
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
			for(int i = 0; i < 6; ++i) {
	        	Plots[i].Pen.Color = Color.FromArgb(tbar.Value,Plots[i].Pen.Color);
			}
			ChartControl.ChartPanel.Invalidate(false);
         }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {
				for(int i = 0; i < 6; ++i) Values[i].Set(Input[0]);
				return;
			}
			for(int i = 0; i < 6; ++i) {
			   Values[i].Set(Values[i][1]+alphas[i]*(Input[0]-Values[i][1]));
			}
			
        }

        #region Properties
		[Description("First Period")]
		[GridCategory("Parameters")]
		public int PeriodBegin {
			get { return periodStart; }
			set { periodStart = Math.Max(value,1); }
		}
		[Description("Period Adder")]
		[GridCategory("Parameters")]
		public int PeriodInterval {
			get { return periodAdder; }
			set { periodAdder = Math.Max(value,1); }
		}
		[Description("Color")]
		[GridCategory("Parameters")]
		public Color BandColor {
			get { return bandColor; }
			set { bandColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string BandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bandColor); }
			set { bandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Color")]
		[GridCategory("Parameters")]
		public Color FirstColor {
			get { return firstColor; }
			set { firstColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string FirstColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(firstColor); }
			set { firstColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private zMultEMA[] cachezMultEMA = null;

        private static zMultEMA checkzMultEMA = new zMultEMA();

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zMultEMA zMultEMA(Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            return zMultEMA(Input, bandColor, firstColor, periodBegin, periodInterval);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zMultEMA zMultEMA(Data.IDataSeries input, Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            if (cachezMultEMA != null)
                for (int idx = 0; idx < cachezMultEMA.Length; idx++)
                    if (cachezMultEMA[idx].BandColor == bandColor && cachezMultEMA[idx].FirstColor == firstColor && cachezMultEMA[idx].PeriodBegin == periodBegin && cachezMultEMA[idx].PeriodInterval == periodInterval && cachezMultEMA[idx].EqualsInput(input))
                        return cachezMultEMA[idx];

            lock (checkzMultEMA)
            {
                checkzMultEMA.BandColor = bandColor;
                bandColor = checkzMultEMA.BandColor;
                checkzMultEMA.FirstColor = firstColor;
                firstColor = checkzMultEMA.FirstColor;
                checkzMultEMA.PeriodBegin = periodBegin;
                periodBegin = checkzMultEMA.PeriodBegin;
                checkzMultEMA.PeriodInterval = periodInterval;
                periodInterval = checkzMultEMA.PeriodInterval;

                if (cachezMultEMA != null)
                    for (int idx = 0; idx < cachezMultEMA.Length; idx++)
                        if (cachezMultEMA[idx].BandColor == bandColor && cachezMultEMA[idx].FirstColor == firstColor && cachezMultEMA[idx].PeriodBegin == periodBegin && cachezMultEMA[idx].PeriodInterval == periodInterval && cachezMultEMA[idx].EqualsInput(input))
                            return cachezMultEMA[idx];

                zMultEMA indicator = new zMultEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandColor = bandColor;
                indicator.FirstColor = firstColor;
                indicator.PeriodBegin = periodBegin;
                indicator.PeriodInterval = periodInterval;
                Indicators.Add(indicator);
                indicator.SetUp();

                zMultEMA[] tmp = new zMultEMA[cachezMultEMA == null ? 1 : cachezMultEMA.Length + 1];
                if (cachezMultEMA != null)
                    cachezMultEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezMultEMA = tmp;
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
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMultEMA zMultEMA(Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            return _indicator.zMultEMA(Input, bandColor, firstColor, periodBegin, periodInterval);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zMultEMA zMultEMA(Data.IDataSeries input, Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            return _indicator.zMultEMA(input, bandColor, firstColor, periodBegin, periodInterval);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMultEMA zMultEMA(Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            return _indicator.zMultEMA(Input, bandColor, firstColor, periodBegin, periodInterval);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zMultEMA zMultEMA(Data.IDataSeries input, Color bandColor, Color firstColor, int periodBegin, int periodInterval)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zMultEMA(input, bandColor, firstColor, periodBegin, periodInterval);
        }
    }
}
#endregion
