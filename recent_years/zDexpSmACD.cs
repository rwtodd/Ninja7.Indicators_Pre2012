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
    /// MACD of DexpSmooth lines
    /// </summary>
    [Description("MACD of DexpSmooth lines")]
    public class zDexpSmACD : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha1 = 0.182; // Default setting for Alpha1
            private double alpha2 = 0.095; // Default setting for Alpha2
            private int tau = 1; // Default setting for Tau
			zDexpSmooth fast;
			zDexpSmooth slow;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "MACD"));
            Add(new Plot(Color.FromKnownColor(KnownColor.PaleVioletRed), PlotStyle.Line, "Smooth"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Bar, "Hist"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			fast = zDexpSmooth(alpha1,tau);
			slow = zDexpSmooth(alpha2,tau);
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {
				MACD.Set(0);
				Smooth.Set(0); 
				Hist.Set(0);
				return;	
			}
			
            MACD.Set(fast[0]-slow[0]);
            Smooth.Set(Smooth[1] + 0.25*(MACD[0]-Smooth[1]));
            Hist.Set(MACD[0]-Smooth[0]);
			if( (Hist[0] > Hist[1]) || 
		        ((Hist[0] == Hist[1]) && (Hist[1] > Hist[2])) ) {
				PlotColors[2][0] = Color.MidnightBlue;		
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MACD
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Smooth
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Hist
        {
            get { return Values[2]; }
        }

        [Description("fast alpha")]
        [GridCategory("Parameters")]
        public double Alpha1
        {
            get { return alpha1; }
            set { alpha1 = Math.Max(0.000, value); }
        }

        [Description("slow alpha")]
        [GridCategory("Parameters")]
        public double Alpha2
        {
            get { return alpha2; }
            set { alpha2 = Math.Max(0.000, value); }
        }

        [Description("lookahead")]
        [GridCategory("Parameters")]
        public int Tau
        {
            get { return tau; }
            set { tau = Math.Max(0, value); }
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
        private zDexpSmACD[] cachezDexpSmACD = null;

        private static zDexpSmACD checkzDexpSmACD = new zDexpSmACD();

        /// <summary>
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        public zDexpSmACD zDexpSmACD(double alpha1, double alpha2, int tau)
        {
            return zDexpSmACD(Input, alpha1, alpha2, tau);
        }

        /// <summary>
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        public zDexpSmACD zDexpSmACD(Data.IDataSeries input, double alpha1, double alpha2, int tau)
        {
            if (cachezDexpSmACD != null)
                for (int idx = 0; idx < cachezDexpSmACD.Length; idx++)
                    if (Math.Abs(cachezDexpSmACD[idx].Alpha1 - alpha1) <= double.Epsilon && Math.Abs(cachezDexpSmACD[idx].Alpha2 - alpha2) <= double.Epsilon && cachezDexpSmACD[idx].Tau == tau && cachezDexpSmACD[idx].EqualsInput(input))
                        return cachezDexpSmACD[idx];

            lock (checkzDexpSmACD)
            {
                checkzDexpSmACD.Alpha1 = alpha1;
                alpha1 = checkzDexpSmACD.Alpha1;
                checkzDexpSmACD.Alpha2 = alpha2;
                alpha2 = checkzDexpSmACD.Alpha2;
                checkzDexpSmACD.Tau = tau;
                tau = checkzDexpSmACD.Tau;

                if (cachezDexpSmACD != null)
                    for (int idx = 0; idx < cachezDexpSmACD.Length; idx++)
                        if (Math.Abs(cachezDexpSmACD[idx].Alpha1 - alpha1) <= double.Epsilon && Math.Abs(cachezDexpSmACD[idx].Alpha2 - alpha2) <= double.Epsilon && cachezDexpSmACD[idx].Tau == tau && cachezDexpSmACD[idx].EqualsInput(input))
                            return cachezDexpSmACD[idx];

                zDexpSmACD indicator = new zDexpSmACD();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha1 = alpha1;
                indicator.Alpha2 = alpha2;
                indicator.Tau = tau;
                Indicators.Add(indicator);
                indicator.SetUp();

                zDexpSmACD[] tmp = new zDexpSmACD[cachezDexpSmACD == null ? 1 : cachezDexpSmACD.Length + 1];
                if (cachezDexpSmACD != null)
                    cachezDexpSmACD.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezDexpSmACD = tmp;
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
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpSmACD zDexpSmACD(double alpha1, double alpha2, int tau)
        {
            return _indicator.zDexpSmACD(Input, alpha1, alpha2, tau);
        }

        /// <summary>
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpSmACD zDexpSmACD(Data.IDataSeries input, double alpha1, double alpha2, int tau)
        {
            return _indicator.zDexpSmACD(input, alpha1, alpha2, tau);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpSmACD zDexpSmACD(double alpha1, double alpha2, int tau)
        {
            return _indicator.zDexpSmACD(Input, alpha1, alpha2, tau);
        }

        /// <summary>
        /// MACD of DexpSmooth lines
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpSmACD zDexpSmACD(Data.IDataSeries input, double alpha1, double alpha2, int tau)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zDexpSmACD(input, alpha1, alpha2, tau);
        }
    }
}
#endregion
