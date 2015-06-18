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
    /// DexpSmooth with StdDev Bands
    /// </summary>
    [Description("DexpSmooth with StdDev Bands")]
    public class zDexpBands : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha = 0.095; // Default setting for Alpha
            private double stdDevs = 2.000; // Default setting for StdDevs
			private zDexpSmooth dexp;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.RoyalBlue), PlotStyle.Line, "Dexp"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "LowerBand"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "UpperBand"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			dexp = zDexpSmooth(alpha,1,true);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Dexp.Set(dexp[0]);
			var devs = dexp.StdDev * stdDevs;			
            LowerBand.Set(Dexp[0]-devs);
            UpperBand.Set(Dexp[0]+devs);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Dexp
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerBand
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperBand
        {
            get { return Values[2]; }
        }

        [Description("Alpha Value")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set {  if(value > 1) value = 2.0/(value+1.0);
				alpha = Math.Max(0.00, value); }
        }

        [Description("Num Devs")]
        [GridCategory("Parameters")]
        public double StdDevs
        {
            get { return stdDevs; }
            set { stdDevs = Math.Max(0.000, value); }
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
        private zDexpBands[] cachezDexpBands = null;

        private static zDexpBands checkzDexpBands = new zDexpBands();

        /// <summary>
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        public zDexpBands zDexpBands(double alpha, double stdDevs)
        {
            return zDexpBands(Input, alpha, stdDevs);
        }

        /// <summary>
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        public zDexpBands zDexpBands(Data.IDataSeries input, double alpha, double stdDevs)
        {
            if (cachezDexpBands != null)
                for (int idx = 0; idx < cachezDexpBands.Length; idx++)
                    if (Math.Abs(cachezDexpBands[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cachezDexpBands[idx].StdDevs - stdDevs) <= double.Epsilon && cachezDexpBands[idx].EqualsInput(input))
                        return cachezDexpBands[idx];

            lock (checkzDexpBands)
            {
                checkzDexpBands.Alpha = alpha;
                alpha = checkzDexpBands.Alpha;
                checkzDexpBands.StdDevs = stdDevs;
                stdDevs = checkzDexpBands.StdDevs;

                if (cachezDexpBands != null)
                    for (int idx = 0; idx < cachezDexpBands.Length; idx++)
                        if (Math.Abs(cachezDexpBands[idx].Alpha - alpha) <= double.Epsilon && Math.Abs(cachezDexpBands[idx].StdDevs - stdDevs) <= double.Epsilon && cachezDexpBands[idx].EqualsInput(input))
                            return cachezDexpBands[idx];

                zDexpBands indicator = new zDexpBands();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.StdDevs = stdDevs;
                Indicators.Add(indicator);
                indicator.SetUp();

                zDexpBands[] tmp = new zDexpBands[cachezDexpBands == null ? 1 : cachezDexpBands.Length + 1];
                if (cachezDexpBands != null)
                    cachezDexpBands.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezDexpBands = tmp;
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
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpBands zDexpBands(double alpha, double stdDevs)
        {
            return _indicator.zDexpBands(Input, alpha, stdDevs);
        }

        /// <summary>
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpBands zDexpBands(Data.IDataSeries input, double alpha, double stdDevs)
        {
            return _indicator.zDexpBands(input, alpha, stdDevs);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpBands zDexpBands(double alpha, double stdDevs)
        {
            return _indicator.zDexpBands(Input, alpha, stdDevs);
        }

        /// <summary>
        /// DexpSmooth with StdDev Bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpBands zDexpBands(Data.IDataSeries input, double alpha, double stdDevs)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zDexpBands(input, alpha, stdDevs);
        }
    }
}
#endregion
