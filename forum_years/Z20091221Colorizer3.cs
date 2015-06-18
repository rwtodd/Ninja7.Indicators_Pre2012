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

public enum Colorizer3Method {
	// ***** the 2color ones here....
  OneColor,      /* always green */
  RisingFalling, /* Green if >= last bar, Red if <= last bar */
  EMATrigger,    /* Green if >= EMA(param) of input, Red if <= EMA(param) of input */
  Donchian,      /* Green if making new highs or coming off them, 
	              * Red if making new lows or coming off them */
  AboveBelow,    /* Green above param level, Red below it. */
	
	// ***** the 3color ones here....
  DblEMATrigger, /* EMA(param) and EMA(2xparam), Green above both, red below both, else yellow */
  Donchian3,     /* Green if making new highs, Red if making new lows, else yellow */
  AboveBelow3,   /* Green rising above param, red falling below param, else yellow */
  RisingFalling3 /* Green rising, Red falling, Yellow if within param distance */
}


// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Richard Todd www.movethemarkets.com 3-color colorizer
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com 3-color colorizer")]
    public class Z20091221Colorizer3 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double colorParam = 1; // Default setting for ColorParam
		    private Colorizer3Method colorMethod = Colorizer3Method.RisingFalling;
		
        // User defined variables (add any user defined variables below)
		    private int rising; // 1 up 0 neutral -1 down
		    private int oldRising;
		    private int lastSeen;
		
		    // support series that some of them  need...
		    private IDataSeries help1,help2;
        #endregion

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
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) return;

			// new bar accounting
			if(CurrentBar != lastSeen) {
			  lastSeen = CurrentBar;
			  oldRising = rising;
			}
			
			switch(colorMethod) {
				case Colorizer3Method.RisingFalling:
					if(Input[0] > Input[1]) rising = 1;
					else if(Input[0] < Input[1]) rising = -1;
					else rising = oldRising;
					break;
				case Colorizer3Method.EMATrigger:
					if(help1 == null) help1 = EMA(Input,(int)colorParam).Value;	
					if(Input[0] > help1[0]) rising = 1;
					else if(Input[0] < help1[0]) rising = -1;
					else rising = oldRising;				
					break;
				case Colorizer3Method.Donchian:
					if(help1 == null) {
					  help1 = MAX(Input,(int)colorParam).Value;
					  help2 = MIN(Input,(int)colorParam).Value;
					}
					if(Input[0] >= help1[0]) rising = 1;
					else if(Input[0] <= help2[0]) rising = -1;
					else rising = oldRising;									
					break;
				case Colorizer3Method.OneColor:
					rising = 1;
					break;
				case Colorizer3Method.AboveBelow:
					if(Input[0] > colorParam) rising = 1;
					else if(Input[0] < colorParam) rising = -1;
					else rising = oldRising;				
					break;
				case Colorizer3Method.AboveBelow3:
					if((Input[0] > colorParam) && (Input[0] > Input[1])) rising = 1;
					else if((Input[0] < colorParam) && (Input[0] < Input[1])) rising = -1;
					else rising = 0;
					break;
				case Colorizer3Method.Donchian3:
					if(help1 == null) {
					  help1 = MAX(Input,(int)colorParam).Value;
					  help2 = MIN(Input,(int)colorParam).Value;
					}
					if(Input[0] >= help1[0]) rising = 1;
					else if(Input[0] <= help2[0]) rising = -1;
					else rising = 0;									
					break;
				case Colorizer3Method.DblEMATrigger:
					if(help1 == null) {
						help1 = EMA(Input,(int)colorParam).Value;	
						help2 = EMA(Input,(int)(colorParam*2)).Value;
					}
					if( (Input[0] > help1[0]) && (Input[0] > help2[0]) ) rising = 1;
					else if( (Input[0] < help1[0]) && (Input[0] < help2[0]) ) rising = -1;
					else rising = 0;
					break;
				case Colorizer3Method.RisingFalling3:
					if( Math.Abs(Input[0] - Input[1]) < colorParam ) {
					  rising = 0;	
					} else if(Input[0] > Input[1]) {
					  rising = 1;	
					} else {
					  rising = -1;	
					}
					break;
			}
        }

		public void drawColor(Indicator indicator, int upPlot,int dnPlot, int ntPlot) {
		   Update();
		   switch(rising) {
			case 1:
			 if(indicator.Plots[upPlot].PlotStyle==PlotStyle.Line) 
				indicator.Values[upPlot].Set(1,Input[1]);
			 indicator.Values[upPlot].Set(Input[0]);
			 indicator.Values[dnPlot].Reset();
			 indicator.Values[ntPlot].Reset();
			 break;
			case -1:
			 if(indicator.Plots[dnPlot].PlotStyle==PlotStyle.Line) 
				indicator.Values[dnPlot].Set(1,Input[1]);
			 indicator.Values[dnPlot].Set(Input[0]);
			 indicator.Values[upPlot].Reset();
			 indicator.Values[ntPlot].Reset();
		     break;
			case 0:
			 if(indicator.Plots[ntPlot].PlotStyle==PlotStyle.Line) 
				indicator.Values[ntPlot].Set(1,Input[1]);
			 indicator.Values[ntPlot].Set(Input[0]);
			 indicator.Values[dnPlot].Reset();
			 indicator.Values[upPlot].Reset();
			 break;
		   }
		}
		
        #region Properties
        [Description("Method of the 3-Color Colorizer")]
        [Category("Parameters")]
        public Colorizer3Method ColorMethod
        {
            get { return colorMethod; }
            set { colorMethod = value; }
        }

        [Description("Input to the 3-Color Colorizer")]
        [Category("Parameters")]
        public double ColorParam
        {
            get { return colorParam; }
            set { colorParam = value; }
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
        private Z20091221Colorizer3[] cacheZ20091221Colorizer3 = null;

        private static Z20091221Colorizer3 checkZ20091221Colorizer3 = new Z20091221Colorizer3();

        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        public Z20091221Colorizer3 Z20091221Colorizer3(Colorizer3Method colorMethod, double colorParam)
        {
            return Z20091221Colorizer3(Input, colorMethod, colorParam);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        public Z20091221Colorizer3 Z20091221Colorizer3(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam)
        {
            checkZ20091221Colorizer3.ColorMethod = colorMethod;
            colorMethod = checkZ20091221Colorizer3.ColorMethod;
            checkZ20091221Colorizer3.ColorParam = colorParam;
            colorParam = checkZ20091221Colorizer3.ColorParam;

            if (cacheZ20091221Colorizer3 != null)
                for (int idx = 0; idx < cacheZ20091221Colorizer3.Length; idx++)
                    if (cacheZ20091221Colorizer3[idx].ColorMethod == colorMethod && Math.Abs(cacheZ20091221Colorizer3[idx].ColorParam - colorParam) <= double.Epsilon && cacheZ20091221Colorizer3[idx].EqualsInput(input))
                        return cacheZ20091221Colorizer3[idx];

            Z20091221Colorizer3 indicator = new Z20091221Colorizer3();
            indicator.BarsRequired = BarsRequired;
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.ColorMethod = colorMethod;
            indicator.ColorParam = colorParam;
            indicator.SetUp();

            Z20091221Colorizer3[] tmp = new Z20091221Colorizer3[cacheZ20091221Colorizer3 == null ? 1 : cacheZ20091221Colorizer3.Length + 1];
            if (cacheZ20091221Colorizer3 != null)
                cacheZ20091221Colorizer3.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheZ20091221Colorizer3 = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091221Colorizer3 Z20091221Colorizer3(Colorizer3Method colorMethod, double colorParam)
        {
            return _indicator.Z20091221Colorizer3(Input, colorMethod, colorParam);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091221Colorizer3 Z20091221Colorizer3(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam)
        {
            return _indicator.Z20091221Colorizer3(input, colorMethod, colorParam);
        }

    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091221Colorizer3 Z20091221Colorizer3(Colorizer3Method colorMethod, double colorParam)
        {
            return _indicator.Z20091221Colorizer3(Input, colorMethod, colorParam);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com 3-color colorizer
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091221Colorizer3 Z20091221Colorizer3(Data.IDataSeries input, Colorizer3Method colorMethod, double colorParam)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091221Colorizer3(input, colorMethod, colorParam);
        }

    }
}
#endregion
