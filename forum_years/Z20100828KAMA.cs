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
    /// KAMA with real ema vals and an ATR band
    /// </summary>
    [Description("KAMA with real ema vals and an ATR band")]
    public class Z20100828KAMA : Indicator
    {
		#region Variables
		private int				period	= 11;
		private double				fast	= 2.5;
		private double				slow	= 30;
		private double fastCF,slowCF;
	    private int opacity = 2;

		private double bandwidth = 0;
		private double bandFactor = 0;
		private DataSeries upper,lower;
		private string regiontag;
		
		double noise = 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before
		/// any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "KAMA"));

			Overlay				= true;
		}
		
		protected override void OnStartUp() {
			fastCF = 2.0 /(fast + 1.0);
			slowCF = 2.0 /(slow + 1.0);
			
			if(bandFactor > 0) {
			  bandwidth = 0;
			  upper = new DataSeries(this,MaximumBarsLookBack.Infinite);
			  lower = new DataSeries(this,MaximumBarsLookBack.Infinite);
			  regiontag = "20100828kamc"+period.ToString()+bandFactor.ToString(); 	
			}
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar > 0) 
			{
				noise += Math.Abs(Input[0] - Input[1]);
			}
			if(CurrentBar > period) 
			{
				noise -= Math.Abs(Input[period] - Input[period+1]);
			}
			
			if (CurrentBar < period) 
			{
				Value.Set(Input[0]);
				return;
			}

			double signal = Math.Abs(Input[0] - Input[period]);
	
			// Prevent div by zero
			if (noise == 0) 
			{
				Value.Set(Value[1]);
				return;
			}

			double smooth = Math.Pow((signal / noise) * (fastCF - slowCF) + slowCF, 2);

			Value.Set(Value[1] + smooth * (Input[0] - Value[1]));
			
			if(bandFactor > 0) {
			  	bandwidth = bandwidth + smooth * ((High[0]-Low[0])-bandwidth);
				upper.Set(Value[0] + (bandwidth*bandFactor));
				lower.Set(Value[0] - (bandwidth*bandFactor));
			    DrawRegion(regiontag,CurrentBar-period,0,upper,lower,Plots[0].Pen.Color,Plots[0].Pen.Color,opacity);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Fast Length.")]
		[GridCategory("Parameters")]
		public double Fast
		{
			get { return fast; }
			set { fast = Math.Min(125, Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars used for calculations.")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(5, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Slow Length.")]
		[GridCategory("Parameters")]
		public double Slow
		{
			get { return slow; }
			set { slow = Math.Min(125, Math.Max(1, value)); }
		}
		
		
        [Description("How opaque is the band region?")]
        [Category("Parameters")]
        public int Opacity
        {
            get { return opacity; }
            set { opacity = Math.Max(1, value); }
        }

        [Description("Width of the band")]
        [Category("Parameters")]
        public double BandFactor
        {
            get { return bandFactor; }
            set { bandFactor = Math.Max(0.0, value); }
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
        private Z20100828KAMA[] cacheZ20100828KAMA = null;

        private static Z20100828KAMA checkZ20100828KAMA = new Z20100828KAMA();

        /// <summary>
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        public Z20100828KAMA Z20100828KAMA(double bandFactor, double fast, int opacity, int period, double slow)
        {
            return Z20100828KAMA(Input, bandFactor, fast, opacity, period, slow);
        }

        /// <summary>
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        public Z20100828KAMA Z20100828KAMA(Data.IDataSeries input, double bandFactor, double fast, int opacity, int period, double slow)
        {
            if (cacheZ20100828KAMA != null)
                for (int idx = 0; idx < cacheZ20100828KAMA.Length; idx++)
                    if (Math.Abs(cacheZ20100828KAMA[idx].BandFactor - bandFactor) <= double.Epsilon && Math.Abs(cacheZ20100828KAMA[idx].Fast - fast) <= double.Epsilon && cacheZ20100828KAMA[idx].Opacity == opacity && cacheZ20100828KAMA[idx].Period == period && Math.Abs(cacheZ20100828KAMA[idx].Slow - slow) <= double.Epsilon && cacheZ20100828KAMA[idx].EqualsInput(input))
                        return cacheZ20100828KAMA[idx];

            lock (checkZ20100828KAMA)
            {
                checkZ20100828KAMA.BandFactor = bandFactor;
                bandFactor = checkZ20100828KAMA.BandFactor;
                checkZ20100828KAMA.Fast = fast;
                fast = checkZ20100828KAMA.Fast;
                checkZ20100828KAMA.Opacity = opacity;
                opacity = checkZ20100828KAMA.Opacity;
                checkZ20100828KAMA.Period = period;
                period = checkZ20100828KAMA.Period;
                checkZ20100828KAMA.Slow = slow;
                slow = checkZ20100828KAMA.Slow;

                if (cacheZ20100828KAMA != null)
                    for (int idx = 0; idx < cacheZ20100828KAMA.Length; idx++)
                        if (Math.Abs(cacheZ20100828KAMA[idx].BandFactor - bandFactor) <= double.Epsilon && Math.Abs(cacheZ20100828KAMA[idx].Fast - fast) <= double.Epsilon && cacheZ20100828KAMA[idx].Opacity == opacity && cacheZ20100828KAMA[idx].Period == period && Math.Abs(cacheZ20100828KAMA[idx].Slow - slow) <= double.Epsilon && cacheZ20100828KAMA[idx].EqualsInput(input))
                            return cacheZ20100828KAMA[idx];

                Z20100828KAMA indicator = new Z20100828KAMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandFactor = bandFactor;
                indicator.Fast = fast;
                indicator.Opacity = opacity;
                indicator.Period = period;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100828KAMA[] tmp = new Z20100828KAMA[cacheZ20100828KAMA == null ? 1 : cacheZ20100828KAMA.Length + 1];
                if (cacheZ20100828KAMA != null)
                    cacheZ20100828KAMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100828KAMA = tmp;
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
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100828KAMA Z20100828KAMA(double bandFactor, double fast, int opacity, int period, double slow)
        {
            return _indicator.Z20100828KAMA(Input, bandFactor, fast, opacity, period, slow);
        }

        /// <summary>
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100828KAMA Z20100828KAMA(Data.IDataSeries input, double bandFactor, double fast, int opacity, int period, double slow)
        {
            return _indicator.Z20100828KAMA(input, bandFactor, fast, opacity, period, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100828KAMA Z20100828KAMA(double bandFactor, double fast, int opacity, int period, double slow)
        {
            return _indicator.Z20100828KAMA(Input, bandFactor, fast, opacity, period, slow);
        }

        /// <summary>
        /// KAMA with real ema vals and an ATR band
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100828KAMA Z20100828KAMA(Data.IDataSeries input, double bandFactor, double fast, int opacity, int period, double slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100828KAMA(input, bandFactor, fast, opacity, period, slow);
        }
    }
}
#endregion
