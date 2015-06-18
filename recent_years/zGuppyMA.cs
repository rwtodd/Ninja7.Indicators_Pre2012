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
    public class zGuppyMA : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below
//			private bool startsOn = true;
			private int numPlots = 12;
			private double fastBegin = 5;
			private double fastEnd = 15;
			private double slowBegin = 30;
			private double slowEnd = 60;
			private RWT_MA.MovingAverage[] mas;
			private RWT_MA.MAType type = RWT_MA.MAType.EMA;
			private Color colorFast = Color.Orange, colorSlow = Color.Purple;
//	    // fader stuff
//		private string TBName = "rwtGuppyButton";
//		private System.Windows.Forms.ToolStrip strip = null;
//		private RWT.ZButtonHost btn = null;
//		private bool cansee = true;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast01"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast02"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast03"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast04"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast05"));			
            Add(new Plot(Color.Black, PlotStyle.Line, "Fast06"));

			Add(new Plot(Color.Black, PlotStyle.Line, "Slow01"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Slow02"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Slow03"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Slow04"));
            Add(new Plot(Color.Black, PlotStyle.Line, "Slow05"));			
            Add(new Plot(Color.Black, PlotStyle.Line, "Slow06"));
			
			Overlay				= true;
        }
		
		protected override void OnStartUp() {
			base.OnStartUp();
//			cansee = true;
			mas = new RWT_MA.MovingAverage[numPlots];
			for(int i = 0; i < numPlots/2; ++i) {
				var perF = fastBegin + i*(fastEnd - fastBegin)/5;
				var perS = slowBegin + i*(slowEnd - slowBegin)/5;
				mas[i] = RWT_MA.MAFactory.create(type,perF);
				mas[i].init(Input[0]);
				mas[i+6] = RWT_MA.MAFactory.create(type,perS);
				mas[i+6].init(Input[0]);
				Plots[i].Pen.Width = (i==5)?2:1;
				Plots[i].Pen.Color = colorFast;
				Plots[i+6].Pen.Width = (i==5)?2:1;
				Plots[i+6].Pen.Color = colorSlow;
			}
			//Print("-------------------");
			
//			System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
//   			if(coll.Length > 0) {
//				strip = (System.Windows.Forms.ToolStrip)coll[0];
//				System.Windows.Forms.ToolStripItem[] slider = strip.Items.Find(TBName,false);
//				if(slider.Length > 0) {
//					btn = (RWT.ZButtonHost)slider[0];
//					btn.addRef();
//					btn.Click += new EventHandler(btn1_click);
//					cansee = btn.Text.StartsWith("-");
//				} else {
//     				btn = new RWT.ZButtonHost(TBName,(startsOn?"-G-":"+G+"));
//					cansee = startsOn;
//					btn.addRef();
//					btn.Click += new EventHandler(btn1_click);
//     				strip.Items.Add(btn);
//				}
//   			}
//			
//			updatePlots();
		}
//		private void updatePlots() {
//			int alphaplot = (cansee)?255:0;
//			for(int i = 0; i < numPlots; ++i) {
//	        	Plots[i].Pen.Color = Color.FromArgb(alphaplot,Plots[i].Pen.Color);
//			}
//			ChartControl.ChartPanel.Invalidate(false);
//		}
//		
//		protected override void OnTermination() {
//		  	if((strip != null) && (btn != null)) {
//				if(btn.removeRef() <= 0) {
//	        		strip.Items.Remove(btn);
//					btn.Dispose();
//				} 
//		        strip = null;
//  			    btn = null;
//			}
//			base.OnTermination();				
//		}
//
//		 private void btn1_click(object sender, System.EventArgs e)
//         {
//			cansee = !cansee;
//			btn.Text = (cansee)?"-G-":"+G+";
//			updatePlots();
//         }
//
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			for(int i = 0; i < numPlots; ++i) {
			   Values[i].Set(mas[i].next(Input[0])); 
			}
//			this.DrawRegion("FastsRWT",CurrentBar,0,Values[0],Values[5],Color.Black,Color.Orange,200);
//			this.DrawRegion("SlowRWT",CurrentBar,0,Values[6],Values[11],Color.Black,Color.Purple,200);
        }

        #region Properties
		
//		[Description("Starts On?")]
//		[GridCategory("Parameters")]
//		public bool StartsOn {
//			get { return startsOn; }
//			set { startsOn = value; }
//		}
		
		[Description("Fast Begin")]
		[GridCategory("Parameters")]
		public double FastBegin {
			get { return fastBegin; }
			set { fastBegin = value; }
		}
		[Description("Fast End")]
		[GridCategory("Parameters")]
		public double FastEnd {
			get { return fastEnd; }
			set { fastEnd = value; }
		}
		[Description("Slow Begin")]
		[GridCategory("Parameters")]
		public double SlowBegin {
			get { return slowBegin; }
			set { slowBegin = value; }
		}
		[Description("Slow End")]
		[GridCategory("Parameters")]
		public double SlowEnd {
			get { return slowEnd; }
			set { slowEnd = value; }
		}
		
		[Description("Type")]
		[GridCategory("Parameters")]
		public RWT_MA.MAType AType {
			get { return type; }
			set { type = value; }
		}
		
        [XmlIgnore]
        [Description("Color of fast band.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Fast Color")]
        public Color FastColor
        {
            get { return colorFast; }
            set { colorFast = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string FastColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(colorFast); }
            set { colorFast = Gui.Design.SerializableColor.FromString(value); }
        }
		
        [XmlIgnore]
        [Description("Color of Slow band.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Slow Color")]
        public Color SlowColor
        {
            get { return colorSlow; }
            set { colorSlow = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string SlowColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(colorSlow); }
            set { colorSlow = Gui.Design.SerializableColor.FromString(value); }
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
        private zGuppyMA[] cachezGuppyMA = null;

        private static zGuppyMA checkzGuppyMA = new zGuppyMA();

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zGuppyMA zGuppyMA(RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            return zGuppyMA(Input, aType, fastBegin, fastEnd, slowBegin, slowEnd);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public zGuppyMA zGuppyMA(Data.IDataSeries input, RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            if (cachezGuppyMA != null)
                for (int idx = 0; idx < cachezGuppyMA.Length; idx++)
                    if (cachezGuppyMA[idx].AType == aType && Math.Abs(cachezGuppyMA[idx].FastBegin - fastBegin) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].FastEnd - fastEnd) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].SlowBegin - slowBegin) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].SlowEnd - slowEnd) <= double.Epsilon && cachezGuppyMA[idx].EqualsInput(input))
                        return cachezGuppyMA[idx];

            lock (checkzGuppyMA)
            {
                checkzGuppyMA.AType = aType;
                aType = checkzGuppyMA.AType;
                checkzGuppyMA.FastBegin = fastBegin;
                fastBegin = checkzGuppyMA.FastBegin;
                checkzGuppyMA.FastEnd = fastEnd;
                fastEnd = checkzGuppyMA.FastEnd;
                checkzGuppyMA.SlowBegin = slowBegin;
                slowBegin = checkzGuppyMA.SlowBegin;
                checkzGuppyMA.SlowEnd = slowEnd;
                slowEnd = checkzGuppyMA.SlowEnd;

                if (cachezGuppyMA != null)
                    for (int idx = 0; idx < cachezGuppyMA.Length; idx++)
                        if (cachezGuppyMA[idx].AType == aType && Math.Abs(cachezGuppyMA[idx].FastBegin - fastBegin) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].FastEnd - fastEnd) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].SlowBegin - slowBegin) <= double.Epsilon && Math.Abs(cachezGuppyMA[idx].SlowEnd - slowEnd) <= double.Epsilon && cachezGuppyMA[idx].EqualsInput(input))
                            return cachezGuppyMA[idx];

                zGuppyMA indicator = new zGuppyMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AType = aType;
                indicator.FastBegin = fastBegin;
                indicator.FastEnd = fastEnd;
                indicator.SlowBegin = slowBegin;
                indicator.SlowEnd = slowEnd;
                Indicators.Add(indicator);
                indicator.SetUp();

                zGuppyMA[] tmp = new zGuppyMA[cachezGuppyMA == null ? 1 : cachezGuppyMA.Length + 1];
                if (cachezGuppyMA != null)
                    cachezGuppyMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezGuppyMA = tmp;
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
        public Indicator.zGuppyMA zGuppyMA(RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            return _indicator.zGuppyMA(Input, aType, fastBegin, fastEnd, slowBegin, slowEnd);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zGuppyMA zGuppyMA(Data.IDataSeries input, RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            return _indicator.zGuppyMA(input, aType, fastBegin, fastEnd, slowBegin, slowEnd);
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
        public Indicator.zGuppyMA zGuppyMA(RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            return _indicator.zGuppyMA(Input, aType, fastBegin, fastEnd, slowBegin, slowEnd);
        }

        /// <summary>
        /// Guppy Stuff
        /// </summary>
        /// <returns></returns>
        public Indicator.zGuppyMA zGuppyMA(Data.IDataSeries input, RWT_MA.MAType aType, double fastBegin, double fastEnd, double slowBegin, double slowEnd)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zGuppyMA(input, aType, fastBegin, fastEnd, slowBegin, slowEnd);
        }
    }
}
#endregion
