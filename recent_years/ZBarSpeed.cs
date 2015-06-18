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
    /// Bars per Minute
    /// </summary>
    [Description("Bars per Minute")]
    public class ZBarSpeed : Indicator
    {
        #region Variables
        // Wizard generated variablesj
		double alpha = 0.2;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Goldenrod), PlotStyle.Bar, "BPM"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "Zero"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 1) { BPM.Set(10.0); return; }
			var mins = 3600.0 / Math.Min(Math.Max(Time[0].Subtract(Time[1]).TotalSeconds,1.0),1200.0);
            BPM.Set(BPM[1] + alpha*(mins-BPM[1]));
			if(BPM[0] < BPM[1]) PlotColors[0][0] = Color.Gray;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BPM
        {
            get { return Values[0]; }
        }

        [Description("smoothing constant")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { if(value > 1.0) { value = 2.0/(1.0+value); }
			      alpha = Math.Max(0, value); }
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
        private ZBarSpeed[] cacheZBarSpeed = null;

        private static ZBarSpeed checkZBarSpeed = new ZBarSpeed();

        /// <summary>
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        public ZBarSpeed ZBarSpeed(double alpha)
        {
            return ZBarSpeed(Input, alpha);
        }

        /// <summary>
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        public ZBarSpeed ZBarSpeed(Data.IDataSeries input, double alpha)
        {
            if (cacheZBarSpeed != null)
                for (int idx = 0; idx < cacheZBarSpeed.Length; idx++)
                    if (Math.Abs(cacheZBarSpeed[idx].Alpha - alpha) <= double.Epsilon && cacheZBarSpeed[idx].EqualsInput(input))
                        return cacheZBarSpeed[idx];

            lock (checkZBarSpeed)
            {
                checkZBarSpeed.Alpha = alpha;
                alpha = checkZBarSpeed.Alpha;

                if (cacheZBarSpeed != null)
                    for (int idx = 0; idx < cacheZBarSpeed.Length; idx++)
                        if (Math.Abs(cacheZBarSpeed[idx].Alpha - alpha) <= double.Epsilon && cacheZBarSpeed[idx].EqualsInput(input))
                            return cacheZBarSpeed[idx];

                ZBarSpeed indicator = new ZBarSpeed();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZBarSpeed[] tmp = new ZBarSpeed[cacheZBarSpeed == null ? 1 : cacheZBarSpeed.Length + 1];
                if (cacheZBarSpeed != null)
                    cacheZBarSpeed.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZBarSpeed = tmp;
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
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZBarSpeed ZBarSpeed(double alpha)
        {
            return _indicator.ZBarSpeed(Input, alpha);
        }

        /// <summary>
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        public Indicator.ZBarSpeed ZBarSpeed(Data.IDataSeries input, double alpha)
        {
            return _indicator.ZBarSpeed(input, alpha);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZBarSpeed ZBarSpeed(double alpha)
        {
            return _indicator.ZBarSpeed(Input, alpha);
        }

        /// <summary>
        /// Bars per Minute
        /// </summary>
        /// <returns></returns>
        public Indicator.ZBarSpeed ZBarSpeed(Data.IDataSeries input, double alpha)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZBarSpeed(input, alpha);
        }
    }
}
#endregion
