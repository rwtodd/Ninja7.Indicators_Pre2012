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
    /// Subtract the Dexp from the Momo and see what's shakin
    /// </summary>
    [Description("Subtract the Dexp from the Momo and see what's shakin")]
    public class zDexpMomoSub : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha = 0.182; // Default setting for Alpha
            private int momo = 1; // Default setting for Momo
		    private Momentum mom;
		    private zDexpSmooth dexp;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "Difference"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
		  mom = Momentum(Median,momo);
		  dexp = zDexpSmooth(mom,alpha,1);
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < momo) return;
            Difference.Set(mom[0]-dexp[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Difference
        {
            get { return Values[0]; }
        }

        [Description("alpha")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { alpha = Math.Max(0.000, value); }
        }

        [Description("momentum period")]
        [GridCategory("Parameters")]
        public int Momo
        {
            get { return momo; }
            set { momo = Math.Max(1, value); }
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
        private zDexpMomoSub[] cachezDexpMomoSub = null;

        private static zDexpMomoSub checkzDexpMomoSub = new zDexpMomoSub();

        /// <summary>
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        public zDexpMomoSub zDexpMomoSub(double alpha, int momo)
        {
            return zDexpMomoSub(Input, alpha, momo);
        }

        /// <summary>
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        public zDexpMomoSub zDexpMomoSub(Data.IDataSeries input, double alpha, int momo)
        {
            if (cachezDexpMomoSub != null)
                for (int idx = 0; idx < cachezDexpMomoSub.Length; idx++)
                    if (Math.Abs(cachezDexpMomoSub[idx].Alpha - alpha) <= double.Epsilon && cachezDexpMomoSub[idx].Momo == momo && cachezDexpMomoSub[idx].EqualsInput(input))
                        return cachezDexpMomoSub[idx];

            lock (checkzDexpMomoSub)
            {
                checkzDexpMomoSub.Alpha = alpha;
                alpha = checkzDexpMomoSub.Alpha;
                checkzDexpMomoSub.Momo = momo;
                momo = checkzDexpMomoSub.Momo;

                if (cachezDexpMomoSub != null)
                    for (int idx = 0; idx < cachezDexpMomoSub.Length; idx++)
                        if (Math.Abs(cachezDexpMomoSub[idx].Alpha - alpha) <= double.Epsilon && cachezDexpMomoSub[idx].Momo == momo && cachezDexpMomoSub[idx].EqualsInput(input))
                            return cachezDexpMomoSub[idx];

                zDexpMomoSub indicator = new zDexpMomoSub();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.Momo = momo;
                Indicators.Add(indicator);
                indicator.SetUp();

                zDexpMomoSub[] tmp = new zDexpMomoSub[cachezDexpMomoSub == null ? 1 : cachezDexpMomoSub.Length + 1];
                if (cachezDexpMomoSub != null)
                    cachezDexpMomoSub.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezDexpMomoSub = tmp;
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
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpMomoSub zDexpMomoSub(double alpha, int momo)
        {
            return _indicator.zDexpMomoSub(Input, alpha, momo);
        }

        /// <summary>
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpMomoSub zDexpMomoSub(Data.IDataSeries input, double alpha, int momo)
        {
            return _indicator.zDexpMomoSub(input, alpha, momo);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpMomoSub zDexpMomoSub(double alpha, int momo)
        {
            return _indicator.zDexpMomoSub(Input, alpha, momo);
        }

        /// <summary>
        /// Subtract the Dexp from the Momo and see what's shakin
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpMomoSub zDexpMomoSub(Data.IDataSeries input, double alpha, int momo)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zDexpMomoSub(input, alpha, momo);
        }
    }
}
#endregion
