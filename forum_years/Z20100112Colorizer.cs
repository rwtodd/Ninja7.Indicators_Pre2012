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

public enum RWTColorMethod {
	// ***** the 2color ones here....
  OneColor,      /* always neutral */
  RisingFalling, /* Green if >= last bar, Red if <= last bar */
  EMATrigger,    /* Green if >= EMA(param) of input, Red if <= EMA(param) of input */
  Donchian,      /* Green if making new highs or coming off them, 
	              * Red if making new lows or coming off them */
  AboveBelow,    /* Green above param level, Red below it. */
	
	// ***** the 3color ones here....
  DblEMATrigger, /* EMA(param) and EMA(2xparam), Green above both, red below both, else yellow */
  Donchian3,     /* Green if making new highs, Red if making new lows, else yellow */
  AboveBelow3,   /* Green rising above param, red falling below param, else yellow */
  RisingFalling3, /* Green rising, Red falling, Yellow if within param distance */
  StochStyle8020,    /* Green above 80, neutral middle, Red belew 20 */
  StochStyleParam,   /* Green above param, red below -param, otherwise neutral */
  RisingFallingMomentum /* Green rising, lightgreen rising fast, red falling, lightred falling fast */
}

// in the two-color ones, the neutral color is 
// always the one the user sets on the plot itself...
public enum RWTColorScheme {
  CyanMagenta,
  CyanYellowMagenta,
  GreenBlueRed,
  GreenRed,
  GreenYellowRed,
  TurquoisePurple
}


// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// 
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com generic colorizer")]
    public class Z20100112Colorizer : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double colorParam = 1; // Default setting for ColorParam
		    private RWTColorMethod colorMethod = RWTColorMethod.RisingFalling;
		
        // User defined variables (add any user defined variables below)
		    private int rising; // 1 up 0 neutral -1 down
		    private int oldRising;
		    private int lastSeen;
		
		    private Color curColor = Color.Black;
		
			// the colors themselves...
		    private RWTColorScheme colorScheme = RWTColorScheme.GreenRed;
		    private Color upColor, dnColor, ntColor;
		
		    private bool initp = false; // have we set up the colors yet?
		
		    // support series that some of them  need...
		    private IDataSeries help1,help2;
		    private Color extraColor1, extraColor2;
        #endregion

		private void setupColors(Color userColor) {
			switch(colorScheme) {
				case RWTColorScheme.CyanMagenta:
					upColor = Color.Cyan;
					ntColor = userColor;
					dnColor = Color.Magenta;
					break;
				case RWTColorScheme.CyanYellowMagenta:
					upColor = Color.Cyan;
					ntColor = Color.Yellow;
					dnColor = Color.Magenta;
					break;
				case RWTColorScheme.GreenBlueRed:
					upColor = Color.Green;
					ntColor = Color.Blue;
					dnColor = Color.Red;
					break;
				case RWTColorScheme.GreenRed:
					upColor = Color.Green;
					ntColor = userColor;
					dnColor = Color.Red;
					break;
				case RWTColorScheme.GreenYellowRed:
					upColor = Color.Green;
					ntColor = Color.Yellow;
					dnColor = Color.Red;
					break;
				case RWTColorScheme.TurquoisePurple:
					upColor = Color.PaleTurquoise;
					ntColor = userColor;
					dnColor = Color.Purple;
					break;
				default: // if all else fails....
					upColor = userColor;
					dnColor = userColor;
					ntColor = userColor;
					break;
			}
			
			// RisingFallingMOMO needs the brighter colors...
			if(colorMethod == RWTColorMethod.RisingFallingMomentum) {
			  extraColor1 = System.Windows.Forms.ControlPaint.LightLight(upColor);
			  extraColor2 = System.Windows.Forms.ControlPaint.LightLight(dnColor);
			}
			initp = true;
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
            PriceTypeSupported	= false;
		    rising = 1;
			oldRising = 1;
			lastSeen = -1;
			
			help1 = null;
			help2 = null;
			
			initp = false;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
	
			if(!initp || (CurrentBar < 3)) return;

			// new bar accounting
			if(CurrentBar != lastSeen) {
			  lastSeen = CurrentBar;
			  oldRising = rising;
			}
			
			switch(colorMethod) {
				case RWTColorMethod.RisingFalling:
					if(Input[0] > Input[1]) rising = 1;
					else if(Input[0] < Input[1]) rising = -1;
					else rising = oldRising;
					risingMethod();
					break;
				case RWTColorMethod.EMATrigger:
					if(help1 == null) help1 = EMA(Input,(int)colorParam).Value;	
					if(Input[0] > help1[0]) rising = 1;
					else if(Input[0] < help1[0]) rising = -1;
					else rising = oldRising;
					risingMethod();
					break;
				case RWTColorMethod.Donchian:
					if(help1 == null) {
					  help1 = MAX(Input,(int)colorParam).Value;
					  help2 = MIN(Input,(int)colorParam).Value;
					}
					if(Input[0] >= help1[0]) rising = 1;
					else if(Input[0] <= help2[0]) rising = -1;
					else rising = oldRising;									
					risingMethod();
					break;
				case RWTColorMethod.OneColor:
					rising = 0;
					risingMethod();
					break;
				case RWTColorMethod.AboveBelow:
					if(Input[0] > colorParam) rising = 1;
					else if(Input[0] < colorParam) rising = -1;
					else rising = oldRising;				
					risingMethod();
					break;
				case RWTColorMethod.AboveBelow3:
					if((Input[0] > colorParam) && (Input[0] > Input[1])) rising = 1;
					else if((Input[0] < colorParam) && (Input[0] < Input[1])) rising = -1;
					else rising = 0;
					risingMethod();
					break;
				case RWTColorMethod.Donchian3:
					if(help1 == null) {
					  help1 = MAX(Input,(int)colorParam).Value;
					  help2 = MIN(Input,(int)colorParam).Value;
					}
					if(Input[0] >= help1[0]) rising = 1;
					else if(Input[0] <= help2[0]) rising = -1;
					else rising = 0;									
					risingMethod();
					break;
				case RWTColorMethod.DblEMATrigger:
					if(help1 == null) {
						help1 = EMA(Input,(int)colorParam).Value;	
						help2 = EMA(Input,(int)(colorParam*2)).Value;
					}
					if( (Input[0] > help1[0]) && (Input[0] > help2[0]) ) rising = 1;
					else if( (Input[0] < help1[0]) && (Input[0] < help2[0]) ) rising = -1;
					else rising = 0;
					risingMethod();
					break;
				case RWTColorMethod.RisingFalling3:
					if( Math.Abs(Input[0] - Input[1]) < colorParam ) {
					  rising = 0;	
					} else if(Input[0] > Input[1]) {
					  rising = 1;	
					} else {
					  rising = -1;	
					}
					risingMethod();
					break;
				case RWTColorMethod.StochStyle8020:
					if(Input[0] > 80) rising = 1;
					else if(Input[0] < 20) rising = -1;
					else rising = 0;
					risingMethod();
					break;
				case RWTColorMethod.StochStyleParam:
					if(Input[0] > colorParam) rising = 1;
					else if(Input[0] < -colorParam) rising = -1;
					else rising = 0;
					risingMethod();
					break;
				case RWTColorMethod.RisingFallingMomentum:
					double diff1 = Input[0] - Input[1];
					double diff2 = Input[1] - Input[2];

					if(diff1 > 0) rising = 1;
					else if(diff1 < 0) rising = -1;
					else rising = oldRising;

					if(Math.Abs(diff1) > Math.Abs(diff2)) {
						curColor = ((rising>0)?extraColor1:extraColor2);
					} else {
					 	curColor = ((rising>0)?upColor:dnColor);
					}
					break;
			}
		    
        }

		// most color methods choose their color, in the end, like this:
		private void risingMethod() {
		  if(rising == 0) curColor = ntColor;
		  else if(rising == 1) curColor = upColor;
		  else curColor = dnColor;
		}
		
		public void drawColor(Indicator indicator, int plotNumber) {
		   if(!initp) setupColors(indicator.Plots[plotNumber].Pen.Color);		

		   Update();
		
		   indicator.PlotColors[plotNumber][0] = curColor;		
		}
		
        #region Properties
        [Description("Method of the Colorizer")]
        [GridCategory("Parameters")]
        public RWTColorMethod ColorMethod
        {
            get { return colorMethod; }
            set { colorMethod = value; }
        }

        [Description("Input to the Colorizer")]
        [GridCategory("Parameters")]
        public double ColorParam
        {
            get { return colorParam; }
            set { colorParam = value; }
        }
        [Description("Colorizer Color Scheme")]
        [GridCategory("Parameters")]
        public RWTColorScheme ColorScheme
        {
            get { return colorScheme; }
            set { colorScheme = value; }
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
        private Z20100112Colorizer[] cacheZ20100112Colorizer = null;

        private static Z20100112Colorizer checkZ20100112Colorizer = new Z20100112Colorizer();

        /// <summary>
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        public Z20100112Colorizer Z20100112Colorizer(RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            return Z20100112Colorizer(Input, colorMethod, colorParam, colorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        public Z20100112Colorizer Z20100112Colorizer(Data.IDataSeries input, RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            if (cacheZ20100112Colorizer != null)
                for (int idx = 0; idx < cacheZ20100112Colorizer.Length; idx++)
                    if (cacheZ20100112Colorizer[idx].ColorMethod == colorMethod && Math.Abs(cacheZ20100112Colorizer[idx].ColorParam - colorParam) <= double.Epsilon && cacheZ20100112Colorizer[idx].ColorScheme == colorScheme && cacheZ20100112Colorizer[idx].EqualsInput(input))
                        return cacheZ20100112Colorizer[idx];

            lock (checkZ20100112Colorizer)
            {
                checkZ20100112Colorizer.ColorMethod = colorMethod;
                colorMethod = checkZ20100112Colorizer.ColorMethod;
                checkZ20100112Colorizer.ColorParam = colorParam;
                colorParam = checkZ20100112Colorizer.ColorParam;
                checkZ20100112Colorizer.ColorScheme = colorScheme;
                colorScheme = checkZ20100112Colorizer.ColorScheme;

                if (cacheZ20100112Colorizer != null)
                    for (int idx = 0; idx < cacheZ20100112Colorizer.Length; idx++)
                        if (cacheZ20100112Colorizer[idx].ColorMethod == colorMethod && Math.Abs(cacheZ20100112Colorizer[idx].ColorParam - colorParam) <= double.Epsilon && cacheZ20100112Colorizer[idx].ColorScheme == colorScheme && cacheZ20100112Colorizer[idx].EqualsInput(input))
                            return cacheZ20100112Colorizer[idx];

                Z20100112Colorizer indicator = new Z20100112Colorizer();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorMethod = colorMethod;
                indicator.ColorParam = colorParam;
                indicator.ColorScheme = colorScheme;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100112Colorizer[] tmp = new Z20100112Colorizer[cacheZ20100112Colorizer == null ? 1 : cacheZ20100112Colorizer.Length + 1];
                if (cacheZ20100112Colorizer != null)
                    cacheZ20100112Colorizer.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100112Colorizer = tmp;
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
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100112Colorizer Z20100112Colorizer(RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            return _indicator.Z20100112Colorizer(Input, colorMethod, colorParam, colorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100112Colorizer Z20100112Colorizer(Data.IDataSeries input, RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            return _indicator.Z20100112Colorizer(input, colorMethod, colorParam, colorScheme);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100112Colorizer Z20100112Colorizer(RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            return _indicator.Z20100112Colorizer(Input, colorMethod, colorParam, colorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com generic colorizer
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100112Colorizer Z20100112Colorizer(Data.IDataSeries input, RWTColorMethod colorMethod, double colorParam, RWTColorScheme colorScheme)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100112Colorizer(input, colorMethod, colorParam, colorScheme);
        }
    }
}
#endregion
