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
    public class zRainbowEMA : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below
			private double[] alphas;  // typical: 5,15 30,55 
			private bool startsOn = true;
			private int numPlots = 18;
			private bool threeBands = true;
	    // fader stuff
		private string TBName = "MultiEMA";
		private System.Windows.Forms.ToolStrip strip = null;
		private RWT.ZButtonHost btn = null;
		private bool cansee = true;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha01"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha02"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha03"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha04"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha05"));			
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha06"));

			Add(new Plot(Color.Black, PlotStyle.Line, "Alpha11"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha12"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha13"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha14"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha15"));			
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha16"));

            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha21"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha22"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha23"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha24"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha25"));			
            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha26"));

//			Add(new Plot(Color.Black, PlotStyle.Line, "Alpha31"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha32"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha33"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha34"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha35"));			
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha36"));
//
//			Add(new Plot(Color.Black, PlotStyle.Line, "Alpha41"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha42"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha43"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha44"));
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha45"));			
//            Add(new Plot(Color.Black, PlotStyle.Line, "Alpha46"));
			
			Overlay				= true;
        }
		
		private Color[] rcolors = { Color.Orange, Color.Purple, Color.DarkGray };
//		private double[] bands = {    5,         30,            180  };
//		private double[] bande = {    15,        90,            540  }; 
		
		private double[] bands = {    5,         30,            180  };
		private double[] bande = {    15,        60,            360  }; 
		protected override void OnStartUp() {
			base.OnStartUp();
			numPlots = threeBands?18:12;			
			cansee = true;
			alphas = new double[numPlots];
            double per=5;
			for(int i = 0; i < numPlots; ++i) {
				int idx = i/6;
				per = bands[idx] + (i%6)*(bande[idx] - bands[idx])/5;
				alphas[i] = 2.0/(per+1.0);
				//Print(per); if(i%6==5) { Print(" - "); }
				Plots[i].Pen.Width = (i%6==5)?2:1;
				Plots[i].Pen.Color = rcolors[idx];
			}
			//Print("-------------------");
			
			System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
   			if(coll.Length > 0) {
				strip = (System.Windows.Forms.ToolStrip)coll[0];
				System.Windows.Forms.ToolStripItem[] slider = strip.Items.Find(TBName,false);
				if(slider.Length > 0) {
					btn = (RWT.ZButtonHost)slider[0];
					btn.addRef();
					btn.Click += new EventHandler(btn1_click);
					cansee = btn.Text.StartsWith("-");
				} else {
     				btn = new RWT.ZButtonHost(TBName,(startsOn?"-G-":"+G+"));
					cansee = startsOn;
					btn.addRef();
					btn.Click += new EventHandler(btn1_click);
     				strip.Items.Add(btn);
				}
   			}
			updatePlots();
		}
		private void updatePlots() {
			int alphaplot = (cansee)?255:0;
			for(int i = 0; i < numPlots; ++i) {
	        	Plots[i].Pen.Color = Color.FromArgb(alphaplot,Plots[i].Pen.Color);
			}
			ChartControl.ChartPanel.Invalidate(false);
		}
		
		protected override void OnTermination() {
		  	if((strip != null) && (btn != null)) {
				if(btn.removeRef() <= 0) {
	        		strip.Items.Remove(btn);
					btn.Dispose();
				} 
		        strip = null;
  			    btn = null;
			}
			base.OnTermination();				
		}

		 private void btn1_click(object sender, System.EventArgs e)
         {
			cansee = !cansee;
			btn.Text = (cansee)?"-G-":"+G+";
			updatePlots();
         }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {
				for(int i = 0; i < numPlots; ++i) Values[i].Set(Input[0]);
				return;
			}
			for(int i = 0; i < numPlots; ++i) {
			   Values[i].Set(Values[i][1]+alphas[i]*(Input[0]-Values[i][1]));
			}
			
        }

        #region Properties
		
		[Description("Starts On?")]
		[GridCategory("Parameters")]
		public bool StartsOn {
			get { return startsOn; }
			set { startsOn = value; }
		}
		[Description("Use Three Bands?")]
		[GridCategory("Parameters")]
		public bool UseThreeBands {
			get { return threeBands; }
			set { threeBands = value; }
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
        private zRainbowEMA[] cachezRainbowEMA = null;

        private static zRainbowEMA checkzRainbowEMA = new zRainbowEMA();

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zRainbowEMA zRainbowEMA(bool startsOn, bool useThreeBands)
        {
            return zRainbowEMA(Input, startsOn, useThreeBands);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zRainbowEMA zRainbowEMA(Data.IDataSeries input, bool startsOn, bool useThreeBands)
        {
            if (cachezRainbowEMA != null)
                for (int idx = 0; idx < cachezRainbowEMA.Length; idx++)
                    if (cachezRainbowEMA[idx].StartsOn == startsOn && cachezRainbowEMA[idx].UseThreeBands == useThreeBands && cachezRainbowEMA[idx].EqualsInput(input))
                        return cachezRainbowEMA[idx];

            lock (checkzRainbowEMA)
            {
                checkzRainbowEMA.StartsOn = startsOn;
                startsOn = checkzRainbowEMA.StartsOn;
                checkzRainbowEMA.UseThreeBands = useThreeBands;
                useThreeBands = checkzRainbowEMA.UseThreeBands;

                if (cachezRainbowEMA != null)
                    for (int idx = 0; idx < cachezRainbowEMA.Length; idx++)
                        if (cachezRainbowEMA[idx].StartsOn == startsOn && cachezRainbowEMA[idx].UseThreeBands == useThreeBands && cachezRainbowEMA[idx].EqualsInput(input))
                            return cachezRainbowEMA[idx];

                zRainbowEMA indicator = new zRainbowEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.StartsOn = startsOn;
                indicator.UseThreeBands = useThreeBands;
                Indicators.Add(indicator);
                indicator.SetUp();

                zRainbowEMA[] tmp = new zRainbowEMA[cachezRainbowEMA == null ? 1 : cachezRainbowEMA.Length + 1];
                if (cachezRainbowEMA != null)
                    cachezRainbowEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezRainbowEMA = tmp;
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
        public Indicator.zRainbowEMA zRainbowEMA(bool startsOn, bool useThreeBands)
        {
            return _indicator.zRainbowEMA(Input, startsOn, useThreeBands);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zRainbowEMA zRainbowEMA(Data.IDataSeries input, bool startsOn, bool useThreeBands)
        {
            return _indicator.zRainbowEMA(input, startsOn, useThreeBands);
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
        public Indicator.zRainbowEMA zRainbowEMA(bool startsOn, bool useThreeBands)
        {
            return _indicator.zRainbowEMA(Input, startsOn, useThreeBands);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zRainbowEMA zRainbowEMA(Data.IDataSeries input, bool startsOn, bool useThreeBands)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zRainbowEMA(input, startsOn, useThreeBands);
        }
    }
}
#endregion
