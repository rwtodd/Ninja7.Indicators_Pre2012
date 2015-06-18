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
    /// Heiken Ashi Smoothed view
    /// </summary>
    [Description("Heiken Ashi Smoothed view")]
    public class zHASmoothed2 : Indicator
    {
        #region Variables
        // Wizard generated variables
		    private int maxSmoothing, lowSmoothing, lookback;
			private Color upcolor = Color.Green;
		    private Color dncolor = Color.Red;
		    private Color ltup = Color.DodgerBlue;
		    private Color ltdn = Color.Magenta;
		zKAMA highs, lows, opens, closes;
		private bool isRising;
		double haopen, haclose;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "HAHigh"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "HALow"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Cross, "HAMid"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
		  opens = zKAMA(Open,lookback,lowSmoothing,maxSmoothing); 
		  highs = zKAMA(High,lookback,lowSmoothing,maxSmoothing);
		  lows = zKAMA(Low,lookback,lowSmoothing,maxSmoothing);
		  closes = zKAMA(Close,lookback,lowSmoothing,maxSmoothing);
		  isRising = true;
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar == 0) {
				opens.Update();
				highs.Update();
				lows.Update();
				closes.Update();
				haopen = Open[0];
				haclose = (High[0]+Low[0]+Close[0]+Open[0])*0.25;
				HAHigh.Set(High[0]);
				HALow.Set(Low[0]);
				HAMid.Set((HAHigh[0]+HALow[0])*0.5);
				return;
			}

			haopen = (haopen + haclose) * 0.5;
			haclose = (highs[0]+lows[0]+opens[0]+closes[0])*0.25;
			if(haopen < haclose) {
			  HAHigh.Set(Math.Max(haclose,highs[0]));
			  HALow.Set(Math.Min(haopen,lows[0]));
			} else {
			  HAHigh.Set(Math.Max(haopen,highs[0]));
			  HALow.Set(Math.Min(haclose,lows[0]));				
			}
			HAMid.Set((HAHigh[0]+HALow[0])*0.5);
			Color col;
			
			if(haopen != haclose) {
				isRising = (haopen < haclose);
			}
			if(isRising) {
			  col = ( (HALow[0] < haopen)?ltup:upcolor );
			} else {
			  col = ( (HAHigh[0] > haopen)?ltdn:dncolor );  					
			}
			
			PlotColors[0][0] = col;
			PlotColors[1][0] = col;
			PlotColors[2][0] = col;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HAHigh
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HALow
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HAMid
        {
            get { return Values[2]; }
        }

        [Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public int LowSmoothing
        {
            get { return lowSmoothing; }
            set { lowSmoothing = value; }
        }
        [Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public int MaxSmoothing
        {
            get { return maxSmoothing; }
            set { maxSmoothing = value; }
        }

        [Description("Lookback")]
        [GridCategory("Parameters")]
        public int LookBack
        {
            get { return lookback; }
            set { lookback = Math.Max(1, value); }
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
        private zHASmoothed2[] cachezHASmoothed2 = null;

        private static zHASmoothed2 checkzHASmoothed2 = new zHASmoothed2();

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public zHASmoothed2 zHASmoothed2(int lookBack, int lowSmoothing, int maxSmoothing)
        {
            return zHASmoothed2(Input, lookBack, lowSmoothing, maxSmoothing);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public zHASmoothed2 zHASmoothed2(Data.IDataSeries input, int lookBack, int lowSmoothing, int maxSmoothing)
        {
            if (cachezHASmoothed2 != null)
                for (int idx = 0; idx < cachezHASmoothed2.Length; idx++)
                    if (cachezHASmoothed2[idx].LookBack == lookBack && cachezHASmoothed2[idx].LowSmoothing == lowSmoothing && cachezHASmoothed2[idx].MaxSmoothing == maxSmoothing && cachezHASmoothed2[idx].EqualsInput(input))
                        return cachezHASmoothed2[idx];

            lock (checkzHASmoothed2)
            {
                checkzHASmoothed2.LookBack = lookBack;
                lookBack = checkzHASmoothed2.LookBack;
                checkzHASmoothed2.LowSmoothing = lowSmoothing;
                lowSmoothing = checkzHASmoothed2.LowSmoothing;
                checkzHASmoothed2.MaxSmoothing = maxSmoothing;
                maxSmoothing = checkzHASmoothed2.MaxSmoothing;

                if (cachezHASmoothed2 != null)
                    for (int idx = 0; idx < cachezHASmoothed2.Length; idx++)
                        if (cachezHASmoothed2[idx].LookBack == lookBack && cachezHASmoothed2[idx].LowSmoothing == lowSmoothing && cachezHASmoothed2[idx].MaxSmoothing == maxSmoothing && cachezHASmoothed2[idx].EqualsInput(input))
                            return cachezHASmoothed2[idx];

                zHASmoothed2 indicator = new zHASmoothed2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LookBack = lookBack;
                indicator.LowSmoothing = lowSmoothing;
                indicator.MaxSmoothing = maxSmoothing;
                Indicators.Add(indicator);
                indicator.SetUp();

                zHASmoothed2[] tmp = new zHASmoothed2[cachezHASmoothed2 == null ? 1 : cachezHASmoothed2.Length + 1];
                if (cachezHASmoothed2 != null)
                    cachezHASmoothed2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezHASmoothed2 = tmp;
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
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHASmoothed2 zHASmoothed2(int lookBack, int lowSmoothing, int maxSmoothing)
        {
            return _indicator.zHASmoothed2(Input, lookBack, lowSmoothing, maxSmoothing);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public Indicator.zHASmoothed2 zHASmoothed2(Data.IDataSeries input, int lookBack, int lowSmoothing, int maxSmoothing)
        {
            return _indicator.zHASmoothed2(input, lookBack, lowSmoothing, maxSmoothing);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHASmoothed2 zHASmoothed2(int lookBack, int lowSmoothing, int maxSmoothing)
        {
            return _indicator.zHASmoothed2(Input, lookBack, lowSmoothing, maxSmoothing);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public Indicator.zHASmoothed2 zHASmoothed2(Data.IDataSeries input, int lookBack, int lowSmoothing, int maxSmoothing)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zHASmoothed2(input, lookBack, lowSmoothing, maxSmoothing);
        }
    }
}
#endregion
