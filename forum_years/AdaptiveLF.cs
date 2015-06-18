// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The AdaptiveLF applies more weight to recent prices than the SMA.
	/// </summary>
	[Description ("Adaptive Laguerre Filter.")]
	public class AdaptiveLF : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		DataSeries _diff;
		double[] curL;
		double[] oldL;
		
		Z20091120SortedWindow sw;
		Z20100527FastMAX fmax;
		Z20100527FastMIN fmin;

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "AdaptiveLF"));

			Overlay				= true;
			PriceTypeSupported	= true;
		}
		
		protected override void OnStartUp() {
			_diff = new DataSeries (this);

			curL = new double[4];
			oldL = new double[4];
			
			sw = Z20091120SortedWindow(_diff,5);
			fmax = Z20100527FastMAX(_diff,period);
			fmin = Z20100527FastMIN(_diff,period);
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate ()
		{
			if (CurrentBar < 2)
				return;

			_diff.Set (Math.Abs (Input [0] - Value [1]));

			double alpha = 0;
			if (CurrentBar >= Period && CurrentBar >= 5)
			{
				double max = fmax[0];
				double min = fmin[0];
				if (max != min)
					alpha = (sw[0] - min) / (max - min);
			}

			Array.Copy(curL,oldL,4);
		    curL[0] = alpha * Input [0] + (1.0 - alpha) * oldL[0]; 
			curL[1] = -(1.0 - alpha) * curL[0] + oldL[0] + (1.0 - alpha) * oldL[1];
			curL[2] = -(1.0 - alpha) * curL[1] + oldL[1] + (1.0 - alpha) * oldL[2];
            curL[3] = -(1.0 - alpha) * curL[2] + oldL[2] + (1.0 - alpha) * oldL[3];
			Value.Set( (curL[0] + 2*curL[1] + 2*curL[2] + curL[3])/6.0  );
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description ("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private AdaptiveLF[] cacheAdaptiveLF = null;

        private static AdaptiveLF checkAdaptiveLF = new AdaptiveLF();

        /// <summary>
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        public AdaptiveLF AdaptiveLF(int period)
        {
            return AdaptiveLF(Input, period);
        }

        /// <summary>
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        public AdaptiveLF AdaptiveLF(Data.IDataSeries input, int period)
        {
            if (cacheAdaptiveLF != null)
                for (int idx = 0; idx < cacheAdaptiveLF.Length; idx++)
                    if (cacheAdaptiveLF[idx].Period == period && cacheAdaptiveLF[idx].EqualsInput(input))
                        return cacheAdaptiveLF[idx];

            lock (checkAdaptiveLF)
            {
                checkAdaptiveLF.Period = period;
                period = checkAdaptiveLF.Period;

                if (cacheAdaptiveLF != null)
                    for (int idx = 0; idx < cacheAdaptiveLF.Length; idx++)
                        if (cacheAdaptiveLF[idx].Period == period && cacheAdaptiveLF[idx].EqualsInput(input))
                            return cacheAdaptiveLF[idx];

                AdaptiveLF indicator = new AdaptiveLF();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                AdaptiveLF[] tmp = new AdaptiveLF[cacheAdaptiveLF == null ? 1 : cacheAdaptiveLF.Length + 1];
                if (cacheAdaptiveLF != null)
                    cacheAdaptiveLF.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAdaptiveLF = tmp;
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
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveLF AdaptiveLF(int period)
        {
            return _indicator.AdaptiveLF(Input, period);
        }

        /// <summary>
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveLF AdaptiveLF(Data.IDataSeries input, int period)
        {
            return _indicator.AdaptiveLF(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AdaptiveLF AdaptiveLF(int period)
        {
            return _indicator.AdaptiveLF(Input, period);
        }

        /// <summary>
        /// Adaptive Laguerre Filter.
        /// </summary>
        /// <returns></returns>
        public Indicator.AdaptiveLF AdaptiveLF(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AdaptiveLF(input, period);
        }
    }
}
#endregion
