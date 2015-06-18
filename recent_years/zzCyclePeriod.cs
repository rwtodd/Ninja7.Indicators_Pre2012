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
    /// Ehlers 111
    /// </summary>
    [Description("Ehlers 111")]
    public class zzCyclePeriod : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha = 0.0700; // Default setting for Alpha
            private double smoothAlpha = 0.4; // Default setting for SmoothAlpha
			private zDexpSmooth smooth;
			private DataSeries cycle, deltaphase;
		    private zSortedWindow zsw;
			private double lastInstPeriod, lastq1, lasti1;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.BlueViolet), PlotStyle.Line, "CycPeriod"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			smooth = zDexpSmooth(smoothAlpha,1,false);
			cycle = new DataSeries(this);
			deltaphase = new DataSeries(this);
			zsw = zSortedWindow(deltaphase,5,true);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 4) {
				cycle.Set(Median[0]);
				lastInstPeriod = 6;
				CycPeriod.Set(0);
				deltaphase.Set(0.1);
				return;
			}
			cycle.Set(  (1-0.5*alpha)*(1 - 0.5*alpha)*(smooth[0] - 2*smooth[1] + smooth[2]) +
			            2*(1-alpha)*cycle[1] -
			            (1 - alpha)*(1-alpha)*cycle[2] );
			
			if(CurrentBar < 7) { 
				CycPeriod.Set(0);
				deltaphase.Set(0.1);
				lastq1 = cycle[0];
				lasti1 = cycle[4];
				return; 
			}
			
			var q1 = (0.0962*cycle[0] + 0.5769*cycle[2] - 0.5769*cycle[4] - 0.0962*cycle[6])*
			         (0.5+0.08*lastInstPeriod);
			var i1 = cycle[3];
			
			if( (q1 != 0) &&
				(lastq1 != 0) ) {
				deltaphase.Set(Math.Atan(i1/q1) - Math.Atan(lasti1/lastq1));
			}
			if(deltaphase[0] < 0.0) deltaphase.Set(deltaphase[1]);
			else if(deltaphase[0] < 0.1) deltaphase.Set(0.1);
			else if(deltaphase[0] > 1.1) deltaphase.Set(1.1);
			zsw.Update();
			
			var dc = 15.0;
			if(zsw[0] != 0.0) {
				dc = 2*Math.PI / zsw[0] + 0.5;	
			}
			var instperiod = .3333*dc + 0.6667*lastInstPeriod;
			CycPeriod.Set(0.15*instperiod + 0.85*CycPeriod[1]);
			
			lastInstPeriod = instperiod;
			lastq1 = q1;
			lasti1 = i1;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CycPeriod
        {
            get { return Values[0]; }
        }

        [Description("Alpha")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { alpha = Math.Max(0.000, value); }
        }

        [Description("SmoothAlpha")]
        [GridCategory("Parameters")]
        public double SmoothAlpha
        {
            get { return smoothAlpha; }
            set { smoothAlpha = Math.Max(0.00, value); }
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
        private zzCyclePeriod[] cachezzCyclePeriod = null;

        private static zzCyclePeriod checkzzCyclePeriod = new zzCyclePeriod();

        /// <summary>
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        public zzCyclePeriod zzCyclePeriod(double alpha, double smoothAlpha)
        {
            return zzCyclePeriod(Input, alpha, smoothAlpha);
        }

        /// <summary>
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        public zzCyclePeriod zzCyclePeriod(Data.IDataSeries input, double alpha, double smoothAlpha)
        {
            if (cachezzCyclePeriod != null)
                for (int idx = 0; idx < cachezzCyclePeriod.Length; idx++)
                    if (Math.Abs(cachezzCyclePeriod[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cachezzCyclePeriod[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzCyclePeriod[idx].EqualsInput(input))
                        return cachezzCyclePeriod[idx];

            lock (checkzzCyclePeriod)
            {
                checkzzCyclePeriod.Alpha = alpha;
                alpha = checkzzCyclePeriod.Alpha;
                checkzzCyclePeriod.SmoothAlpha = smoothAlpha;
                smoothAlpha = checkzzCyclePeriod.SmoothAlpha;

                if (cachezzCyclePeriod != null)
                    for (int idx = 0; idx < cachezzCyclePeriod.Length; idx++)
                        if (Math.Abs(cachezzCyclePeriod[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cachezzCyclePeriod[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzCyclePeriod[idx].EqualsInput(input))
                            return cachezzCyclePeriod[idx];

                zzCyclePeriod indicator = new zzCyclePeriod();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.SmoothAlpha = smoothAlpha;
                Indicators.Add(indicator);
                indicator.SetUp();

                zzCyclePeriod[] tmp = new zzCyclePeriod[cachezzCyclePeriod == null ? 1 : cachezzCyclePeriod.Length + 1];
                if (cachezzCyclePeriod != null)
                    cachezzCyclePeriod.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezzCyclePeriod = tmp;
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
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zzCyclePeriod zzCyclePeriod(double alpha, double smoothAlpha)
        {
            return _indicator.zzCyclePeriod(Input, alpha, smoothAlpha);
        }

        /// <summary>
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        public Indicator.zzCyclePeriod zzCyclePeriod(Data.IDataSeries input, double alpha, double smoothAlpha)
        {
            return _indicator.zzCyclePeriod(input, alpha, smoothAlpha);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zzCyclePeriod zzCyclePeriod(double alpha, double smoothAlpha)
        {
            return _indicator.zzCyclePeriod(Input, alpha, smoothAlpha);
        }

        /// <summary>
        /// Ehlers 111
        /// </summary>
        /// <returns></returns>
        public Indicator.zzCyclePeriod zzCyclePeriod(Data.IDataSeries input, double alpha, double smoothAlpha)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zzCyclePeriod(input, alpha, smoothAlpha);
        }
    }
}
#endregion
