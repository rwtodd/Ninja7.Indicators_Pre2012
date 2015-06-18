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
    /// labels bars for note-taking
    /// </summary>
    [Description("labels bars for note-taking")]
    public class ZBarLabels : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int separation = 3; // Default setting for FontSize
		    private int tickFactor = 8; // how many ticks between numbering levels
		    private int minDistance = 3; // how many ticks from price should the label be, Minimum?
		    private bool drawOnPrice = false;
		    private bool rwtvisible = true;
		    private ATR atr = null;
		    private double plvl; // the price we drew at last
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
           // Overlay				= true;
			this.DrawOnPricePanel = drawOnPrice;
        }

		protected override void OnStartUp() {
		    if(tickFactor < 0) { atr = ATR(80); }
			plvl = 0;
		}

		private void drawNumberOnPrice(string lbl, int which, int offs) {
				int tf = tickFactor;
				int md = minDistance;
				if(tickFactor < 0) {
				  // choose our own factor and min distance
				  tf = (int)(atr[0]*1.5/TickSize);
				  md = Math.Max(1,tf/2);
				}				
				plvl = (which==1?Low[0]:Math.Min(Low[0],Low[1]))-md*TickSize;
				plvl -= ((plvl/TickSize)%tf)*TickSize;
  			    DrawLine(lbl+"RWTLN",true,offs,Low[offs]-TickSize,offs,plvl+TickSize,Color.LightGray,DashStyle.Dash,2);	
			    DrawText(lbl+"RWTBL",true,lbl,offs,plvl,0,Color.Black,ChartControl.Font,StringAlignment.Center,Color.Transparent,Color.White,255);				
	    }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var which = this.Bars.BarsSinceSession + 1;
			var decider = ((which - 1) % separation);
			if (decider == 0) {
			  var numstr = which.ToString();
			  if(!drawOnPrice) {
  			    DrawLine(numstr+"RWTLN",true,0,0.6,0,1.25,Color.Gray,DashStyle.Dash,2);	
			    DrawText(numstr+"RWTBL",true,numstr,0,0.5,0,Color.Black,ChartControl.Font,StringAlignment.Center,Color.Transparent,Color.White,255);
			  } else {
				if(!rwtvisible || CurrentBar < 3) return;
				drawNumberOnPrice(numstr, which, 0);
			  }
			} else if( (decider == 1) && rwtvisible && drawOnPrice && (CurrentBar > 3) &&
				       (Low[0] <= (plvl+2*TickSize)) ) {
						drawNumberOnPrice((which-1).ToString(),which-1,1);
					}
        }

        #region Properties

        [Description("How many bars between markers?")]
        [GridCategory("Parameters")]
        public int Separation
        {
            get { return separation; }
            set { separation = value; }
        }
        [Description("How many ticks minimum under price to mark?")]
        [GridCategory("Parameters")]
        public int MinDistance
        {
            get { return minDistance; }
            set {  minDistance = value; }
        }
        [Description("How many ticks between numbering levels on price panel?")]
        [GridCategory("Parameters")]
        public int TickFactor
        {
            get { return tickFactor; }
            set { tickFactor = value; }
        }

        [Description("Draw on Price?")]
        [GridCategory("Parameters")]
        public bool DrawOnPrice
        {
            get { return drawOnPrice; }
            set { drawOnPrice = value; }
        }
        [Description("Actually Draw?")]
        [GridCategory("Parameters")]
        public bool DrawVisible
        {
            get { return rwtvisible; }
            set { rwtvisible = value; }
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
        private ZBarLabels[] cacheZBarLabels = null;

        private static ZBarLabels checkZBarLabels = new ZBarLabels();

        /// <summary>
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        public ZBarLabels ZBarLabels(bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            return ZBarLabels(Input, drawOnPrice, drawVisible, minDistance, separation, tickFactor);
        }

        /// <summary>
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        public ZBarLabels ZBarLabels(Data.IDataSeries input, bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            if (cacheZBarLabels != null)
                for (int idx = 0; idx < cacheZBarLabels.Length; idx++)
                    if (cacheZBarLabels[idx].DrawOnPrice == drawOnPrice && cacheZBarLabels[idx].DrawVisible == drawVisible && cacheZBarLabels[idx].MinDistance == minDistance && cacheZBarLabels[idx].Separation == separation && cacheZBarLabels[idx].TickFactor == tickFactor && cacheZBarLabels[idx].EqualsInput(input))
                        return cacheZBarLabels[idx];

            lock (checkZBarLabels)
            {
                checkZBarLabels.DrawOnPrice = drawOnPrice;
                drawOnPrice = checkZBarLabels.DrawOnPrice;
                checkZBarLabels.DrawVisible = drawVisible;
                drawVisible = checkZBarLabels.DrawVisible;
                checkZBarLabels.MinDistance = minDistance;
                minDistance = checkZBarLabels.MinDistance;
                checkZBarLabels.Separation = separation;
                separation = checkZBarLabels.Separation;
                checkZBarLabels.TickFactor = tickFactor;
                tickFactor = checkZBarLabels.TickFactor;

                if (cacheZBarLabels != null)
                    for (int idx = 0; idx < cacheZBarLabels.Length; idx++)
                        if (cacheZBarLabels[idx].DrawOnPrice == drawOnPrice && cacheZBarLabels[idx].DrawVisible == drawVisible && cacheZBarLabels[idx].MinDistance == minDistance && cacheZBarLabels[idx].Separation == separation && cacheZBarLabels[idx].TickFactor == tickFactor && cacheZBarLabels[idx].EqualsInput(input))
                            return cacheZBarLabels[idx];

                ZBarLabels indicator = new ZBarLabels();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DrawOnPrice = drawOnPrice;
                indicator.DrawVisible = drawVisible;
                indicator.MinDistance = minDistance;
                indicator.Separation = separation;
                indicator.TickFactor = tickFactor;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZBarLabels[] tmp = new ZBarLabels[cacheZBarLabels == null ? 1 : cacheZBarLabels.Length + 1];
                if (cacheZBarLabels != null)
                    cacheZBarLabels.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZBarLabels = tmp;
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
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZBarLabels ZBarLabels(bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            return _indicator.ZBarLabels(Input, drawOnPrice, drawVisible, minDistance, separation, tickFactor);
        }

        /// <summary>
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        public Indicator.ZBarLabels ZBarLabels(Data.IDataSeries input, bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            return _indicator.ZBarLabels(input, drawOnPrice, drawVisible, minDistance, separation, tickFactor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZBarLabels ZBarLabels(bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            return _indicator.ZBarLabels(Input, drawOnPrice, drawVisible, minDistance, separation, tickFactor);
        }

        /// <summary>
        /// labels bars for note-taking
        /// </summary>
        /// <returns></returns>
        public Indicator.ZBarLabels ZBarLabels(Data.IDataSeries input, bool drawOnPrice, bool drawVisible, int minDistance, int separation, int tickFactor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZBarLabels(input, drawOnPrice, drawVisible, minDistance, separation, tickFactor);
        }
    }
}
#endregion
