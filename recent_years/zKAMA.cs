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
    /// kaufman adaptive
    /// </summary>
    [Description("kaufman adaptive")]
    public class zKAMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int lowSmooth = 2; // Default setting for LowSmooth
            private int maxSmooth = 30; // Default setting for MaxSmooth
            private int lookback = 10; // Default setting for Lookback
        // User defined variables (add any user defined variables below)
			private double[] diffSeries;
			private int dsIndex; 
		    private double dsSum;
		   private double fastAlpha, slowAlpha;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Purple), PlotStyle.Line, "KAMA"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
		    diffSeries = new double[lookback];
			dsIndex = 0;
			dsSum = 0;
			fastAlpha = 2.0/(1.0+lowSmooth);
			slowAlpha = 2.0/(1.0+maxSmooth);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var cb = CurrentBar;
			if(cb > 0) {
			    if(++dsIndex >= lookback) dsIndex = 0;
				dsSum -= diffSeries[dsIndex];
				diffSeries[dsIndex] = Math.Abs(Input[0]-Input[1]);
				dsSum += diffSeries[dsIndex];
			} else { 
			    Value.Set(Input[0]);
				return;
			}
		
			if(dsSum == 0) {
			  Value.Set(Value[1]);
			  return;
			}
			
			var signal = Math.Abs(Input[0] - Input[ (lookback < cb)?lookback:cb ]);
		    var alpha = (signal / dsSum) * (fastAlpha-slowAlpha) + slowAlpha;
			alpha = alpha*alpha;
            Value.Set(Value[1] + alpha*(Input[0]-Value[1]));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries KAMA
        {
            get { return Values[0]; }
        }

        [Description("minimum smoothing")]
        [GridCategory("Parameters")]
        public int LowSmooth
        {
            get { return lowSmooth; }
            set { lowSmooth = Math.Max(1, value); }
        }

        [Description("maximum smoothing")]
        [GridCategory("Parameters")]
        public int MaxSmooth
        {
            get { return maxSmooth; }
            set { maxSmooth = Math.Max(2, value); }
        }

        [Description("lookback period")]
        [GridCategory("Parameters")]
        public int Lookback
        {
            get { return lookback; }
            set { lookback = Math.Max(2, value); }
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
        private zKAMA[] cachezKAMA = null;

        private static zKAMA checkzKAMA = new zKAMA();

        /// <summary>
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        public zKAMA zKAMA(int lookback, int lowSmooth, int maxSmooth)
        {
            return zKAMA(Input, lookback, lowSmooth, maxSmooth);
        }

        /// <summary>
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        public zKAMA zKAMA(Data.IDataSeries input, int lookback, int lowSmooth, int maxSmooth)
        {
            if (cachezKAMA != null)
                for (int idx = 0; idx < cachezKAMA.Length; idx++)
                    if (cachezKAMA[idx].Lookback == lookback && cachezKAMA[idx].LowSmooth == lowSmooth && cachezKAMA[idx].MaxSmooth == maxSmooth && cachezKAMA[idx].EqualsInput(input))
                        return cachezKAMA[idx];

            lock (checkzKAMA)
            {
                checkzKAMA.Lookback = lookback;
                lookback = checkzKAMA.Lookback;
                checkzKAMA.LowSmooth = lowSmooth;
                lowSmooth = checkzKAMA.LowSmooth;
                checkzKAMA.MaxSmooth = maxSmooth;
                maxSmooth = checkzKAMA.MaxSmooth;

                if (cachezKAMA != null)
                    for (int idx = 0; idx < cachezKAMA.Length; idx++)
                        if (cachezKAMA[idx].Lookback == lookback && cachezKAMA[idx].LowSmooth == lowSmooth && cachezKAMA[idx].MaxSmooth == maxSmooth && cachezKAMA[idx].EqualsInput(input))
                            return cachezKAMA[idx];

                zKAMA indicator = new zKAMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Lookback = lookback;
                indicator.LowSmooth = lowSmooth;
                indicator.MaxSmooth = maxSmooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                zKAMA[] tmp = new zKAMA[cachezKAMA == null ? 1 : cachezKAMA.Length + 1];
                if (cachezKAMA != null)
                    cachezKAMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezKAMA = tmp;
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
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zKAMA zKAMA(int lookback, int lowSmooth, int maxSmooth)
        {
            return _indicator.zKAMA(Input, lookback, lowSmooth, maxSmooth);
        }

        /// <summary>
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        public Indicator.zKAMA zKAMA(Data.IDataSeries input, int lookback, int lowSmooth, int maxSmooth)
        {
            return _indicator.zKAMA(input, lookback, lowSmooth, maxSmooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zKAMA zKAMA(int lookback, int lowSmooth, int maxSmooth)
        {
            return _indicator.zKAMA(Input, lookback, lowSmooth, maxSmooth);
        }

        /// <summary>
        /// kaufman adaptive
        /// </summary>
        /// <returns></returns>
        public Indicator.zKAMA zKAMA(Data.IDataSeries input, int lookback, int lowSmooth, int maxSmooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zKAMA(input, lookback, lowSmooth, maxSmooth);
        }
    }
}
#endregion
