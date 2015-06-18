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
    /// CyberCycle Indicator
    /// </summary>
    [Description("CyberCycle Indicator")]
    public class zzAdaptCyberCycle : Indicator
    {
        #region Variables
        // Wizard generated variables
			private double smalpha = 0.33; // smoothing alpha
			private zDexpSmooth smooth;
			private zzCyclePeriod cycper;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "Cycle"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Trigger"));
            Overlay				= false;
        }
		
		protected override void OnStartUp() {
			smooth = zDexpSmooth(smalpha,1,false);
			cycper = zzCyclePeriod(0.07,0.3333);
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 4) {
				Cycle.Set(0);	
				return;
			}
			var alpha = 2.0/(cycper[0] + 1.0);
            Cycle.Set(
			  (1 - 0.5*alpha)*(1 - 0.5*alpha)*(smooth[0] - 2*smooth[1] + smooth[2]) +
			  2*(1-alpha)*Cycle[1] -
			  (1 - alpha)*(1 - alpha)*Cycle[2]
			);
            Trigger.Set(Cycle[1]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Cycle
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Trigger
        {
            get { return Values[1]; }
        }

        [Description("smoothing alpha value")]
        [GridCategory("Parameters")]
        public double SmoothAlpha
        {
            get { return smalpha; }
			set { smalpha = Math.Max(0.000, value); }
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
        private zzAdaptCyberCycle[] cachezzAdaptCyberCycle = null;

        private static zzAdaptCyberCycle checkzzAdaptCyberCycle = new zzAdaptCyberCycle();

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public zzAdaptCyberCycle zzAdaptCyberCycle(double smoothAlpha)
        {
            return zzAdaptCyberCycle(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public zzAdaptCyberCycle zzAdaptCyberCycle(Data.IDataSeries input, double smoothAlpha)
        {
            if (cachezzAdaptCyberCycle != null)
                for (int idx = 0; idx < cachezzAdaptCyberCycle.Length; idx++)
                    if (Math.Abs(cachezzAdaptCyberCycle[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzAdaptCyberCycle[idx].EqualsInput(input))
                        return cachezzAdaptCyberCycle[idx];

            lock (checkzzAdaptCyberCycle)
            {
                checkzzAdaptCyberCycle.SmoothAlpha = smoothAlpha;
                smoothAlpha = checkzzAdaptCyberCycle.SmoothAlpha;

                if (cachezzAdaptCyberCycle != null)
                    for (int idx = 0; idx < cachezzAdaptCyberCycle.Length; idx++)
                        if (Math.Abs(cachezzAdaptCyberCycle[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzAdaptCyberCycle[idx].EqualsInput(input))
                            return cachezzAdaptCyberCycle[idx];

                zzAdaptCyberCycle indicator = new zzAdaptCyberCycle();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.SmoothAlpha = smoothAlpha;
                Indicators.Add(indicator);
                indicator.SetUp();

                zzAdaptCyberCycle[] tmp = new zzAdaptCyberCycle[cachezzAdaptCyberCycle == null ? 1 : cachezzAdaptCyberCycle.Length + 1];
                if (cachezzAdaptCyberCycle != null)
                    cachezzAdaptCyberCycle.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezzAdaptCyberCycle = tmp;
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
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zzAdaptCyberCycle zzAdaptCyberCycle(double smoothAlpha)
        {
            return _indicator.zzAdaptCyberCycle(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.zzAdaptCyberCycle zzAdaptCyberCycle(Data.IDataSeries input, double smoothAlpha)
        {
            return _indicator.zzAdaptCyberCycle(input, smoothAlpha);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zzAdaptCyberCycle zzAdaptCyberCycle(double smoothAlpha)
        {
            return _indicator.zzAdaptCyberCycle(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.zzAdaptCyberCycle zzAdaptCyberCycle(Data.IDataSeries input, double smoothAlpha)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zzAdaptCyberCycle(input, smoothAlpha);
        }
    }
}
#endregion
