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
using System.Linq;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Volume bars, pct complete.
    /// </summary>
    [Description("Volume bars, pct complete.")]
    public class zVBarPctComplete : Indicator
    {
        #region Variables
		private double[] amounts;
		private int lastSeen;
		private Color[] colors = { Color.Green, Color.Red, Color.Yellow, Color.White, Color.Black };
		private int curIdx;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			CalculateOnBarClose = false;
            Overlay				= true;
        }
		
		protected override void OnStartUp() {
			
			amounts = ( new double[] { 0.2, 0.4, 0.6, 0.8, 5 } )
			              .Select( pct => pct * Bars.BarsType.Period.Value )
			              .ToArray();
			curIdx = 0;
			lastSeen = 0;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar != lastSeen) {  
				lastSeen = CurrentBar; 
				curIdx = 0;
				BarsArray[ 0 ].BarsData.PriceMarkerColor = colors[ 0 ];
			}
			
			while( Volume[ 0 ] >= amounts[ curIdx ] ) {
				BarsArray[ 0 ].BarsData.PriceMarkerColor = colors[ ++curIdx ];
			}
        }

        #region Properties

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private zVBarPctComplete[] cachezVBarPctComplete = null;

        private static zVBarPctComplete checkzVBarPctComplete = new zVBarPctComplete();

        /// <summary>
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        public zVBarPctComplete zVBarPctComplete()
        {
            return zVBarPctComplete(Input);
        }

        /// <summary>
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        public zVBarPctComplete zVBarPctComplete(Data.IDataSeries input)
        {
            if (cachezVBarPctComplete != null)
                for (int idx = 0; idx < cachezVBarPctComplete.Length; idx++)
                    if (cachezVBarPctComplete[idx].EqualsInput(input))
                        return cachezVBarPctComplete[idx];

            lock (checkzVBarPctComplete)
            {
                if (cachezVBarPctComplete != null)
                    for (int idx = 0; idx < cachezVBarPctComplete.Length; idx++)
                        if (cachezVBarPctComplete[idx].EqualsInput(input))
                            return cachezVBarPctComplete[idx];

                zVBarPctComplete indicator = new zVBarPctComplete();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                zVBarPctComplete[] tmp = new zVBarPctComplete[cachezVBarPctComplete == null ? 1 : cachezVBarPctComplete.Length + 1];
                if (cachezVBarPctComplete != null)
                    cachezVBarPctComplete.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezVBarPctComplete = tmp;
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
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zVBarPctComplete zVBarPctComplete()
        {
            return _indicator.zVBarPctComplete(Input);
        }

        /// <summary>
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        public Indicator.zVBarPctComplete zVBarPctComplete(Data.IDataSeries input)
        {
            return _indicator.zVBarPctComplete(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zVBarPctComplete zVBarPctComplete()
        {
            return _indicator.zVBarPctComplete(Input);
        }

        /// <summary>
        /// Volume bars, pct complete.
        /// </summary>
        /// <returns></returns>
        public Indicator.zVBarPctComplete zVBarPctComplete(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zVBarPctComplete(input);
        }
    }
}
#endregion
