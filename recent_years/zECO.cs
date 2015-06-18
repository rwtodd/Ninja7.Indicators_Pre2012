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
    /// ergodic candle oscillator
    /// </summary>
    [Description("ergodic candle oscillator")]
    public class zECO : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double ema1 = 0.10; // Default setting for Ema1
            private double ema2 = 0.100; // Default setting for Ema2
			private double num1, num2, den1, den2;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "ECO"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {
			  num1 = 0; num2 = 0; den1 = Math.Max(High[0]-Low[0],0.00001); den2 = den1;	
			}
			
			num1 = num1 + ema1*(Close[0]-Open[0]-num1);
			num2 = num2 + ema2*(num1 - num2);
			
			den1 = den1 + ema1*(High[0]-Low[0]-den1);
			den2 = den2 + ema2*(den1 - den2);
			
			// Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            ECO.Set(num2/den2);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ECO
        {
            get { return Values[0]; }
        }

        [Description("ema 1")]
        [GridCategory("Parameters")]
        public double Ema1
        {
            get { return ema1; }
            set { if(value > 1) value = 2.0/(value+1.0); ema1 = Math.Max(0.000, value); }
        }

        [Description("ema 2")]
        [GridCategory("Parameters")]
        public double Ema2
        {
            get { return ema2; }
            set { if(value > 1) value = 2.0/(value+1.0); ema2 = Math.Max(0.000, value); }
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
        private zECO[] cachezECO = null;

        private static zECO checkzECO = new zECO();

        /// <summary>
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        public zECO zECO(double ema1, double ema2)
        {
            return zECO(Input, ema1, ema2);
        }

        /// <summary>
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        public zECO zECO(Data.IDataSeries input, double ema1, double ema2)
        {
            if (cachezECO != null)
                for (int idx = 0; idx < cachezECO.Length; idx++)
                    if (Math.Abs(cachezECO[idx].Ema1 - ema1) <= double.Epsilon && Math.Abs(cachezECO[idx].Ema2 - ema2) <= double.Epsilon && cachezECO[idx].EqualsInput(input))
                        return cachezECO[idx];

            lock (checkzECO)
            {
                checkzECO.Ema1 = ema1;
                ema1 = checkzECO.Ema1;
                checkzECO.Ema2 = ema2;
                ema2 = checkzECO.Ema2;

                if (cachezECO != null)
                    for (int idx = 0; idx < cachezECO.Length; idx++)
                        if (Math.Abs(cachezECO[idx].Ema1 - ema1) <= double.Epsilon && Math.Abs(cachezECO[idx].Ema2 - ema2) <= double.Epsilon && cachezECO[idx].EqualsInput(input))
                            return cachezECO[idx];

                zECO indicator = new zECO();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Ema1 = ema1;
                indicator.Ema2 = ema2;
                Indicators.Add(indicator);
                indicator.SetUp();

                zECO[] tmp = new zECO[cachezECO == null ? 1 : cachezECO.Length + 1];
                if (cachezECO != null)
                    cachezECO.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezECO = tmp;
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
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zECO zECO(double ema1, double ema2)
        {
            return _indicator.zECO(Input, ema1, ema2);
        }

        /// <summary>
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        public Indicator.zECO zECO(Data.IDataSeries input, double ema1, double ema2)
        {
            return _indicator.zECO(input, ema1, ema2);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zECO zECO(double ema1, double ema2)
        {
            return _indicator.zECO(Input, ema1, ema2);
        }

        /// <summary>
        /// ergodic candle oscillator
        /// </summary>
        /// <returns></returns>
        public Indicator.zECO zECO(Data.IDataSeries input, double ema1, double ema2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zECO(input, ema1, ema2);
        }
    }
}
#endregion
