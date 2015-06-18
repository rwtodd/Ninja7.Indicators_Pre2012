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
    public class zzCyberSineWave : Indicator
    {
        #region Variables
        // Wizard generated variables
			private double smalpha = 0.33; // smoothing alpha
			private zDexpSmooth smooth;
			private zzCyclePeriod cycper;
			private DataSeries cycle;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "SineWave"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Trigger"));
            Overlay				= false;
        }
		
		protected override void OnStartUp() {
			smooth = zDexpSmooth(smalpha,1,false);
			cycle = new DataSeries(this);
			cycper = zzCyclePeriod(0.07,0.3333);
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 4) {
				cycle.Set(0);	
				return;
			}
			var alpha = 2.0/(cycper[0] + 1.0);
			int dcperiod = (int)(cycper[0]);
			var realpart = 0.0;
			var imagpart = 0.0;
            cycle.Set(
			  (1 - 0.5*alpha)*(1 - 0.5*alpha)*(smooth[0] - 2*smooth[1] + smooth[2]) +
			  2*(1-alpha)*cycle[1] -
			  (1 - alpha)*(1 - alpha)*cycle[2]
			);
			var maxcount = Math.Min(CurrentBar, dcperiod);
			for(int count = 0; count < maxcount; ++count) {
			  var rads = 2.0*Math.PI*count/dcperiod; // rwt cycper[0]
			  realpart += Math.Sin(rads)*cycle[count];
			  imagpart += Math.Cos(rads)*cycle[count];
			}
			var dcphase = 0.0;
			if(Math.Abs(imagpart) > 0.001) dcphase = Math.Atan(realpart/imagpart);
			else dcphase = 0.5*Math.PI*Math.Sign(realpart);
			
			dcphase += 90;
			if(imagpart < 0) dcphase += Math.PI;
			if(dcphase >  (1.75*Math.PI)) dcphase -= (2.0*Math.PI);
			SineWave.Set(Math.Sin(dcphase));
            Trigger.Set(Math.Sin(dcphase + Math.PI/4.0));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SineWave
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
        private zzCyberSineWave[] cachezzCyberSineWave = null;

        private static zzCyberSineWave checkzzCyberSineWave = new zzCyberSineWave();

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public zzCyberSineWave zzCyberSineWave(double smoothAlpha)
        {
            return zzCyberSineWave(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public zzCyberSineWave zzCyberSineWave(Data.IDataSeries input, double smoothAlpha)
        {
            if (cachezzCyberSineWave != null)
                for (int idx = 0; idx < cachezzCyberSineWave.Length; idx++)
                    if (Math.Abs(cachezzCyberSineWave[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzCyberSineWave[idx].EqualsInput(input))
                        return cachezzCyberSineWave[idx];

            lock (checkzzCyberSineWave)
            {
                checkzzCyberSineWave.SmoothAlpha = smoothAlpha;
                smoothAlpha = checkzzCyberSineWave.SmoothAlpha;

                if (cachezzCyberSineWave != null)
                    for (int idx = 0; idx < cachezzCyberSineWave.Length; idx++)
                        if (Math.Abs(cachezzCyberSineWave[idx].SmoothAlpha - smoothAlpha) <= double.Epsilon && cachezzCyberSineWave[idx].EqualsInput(input))
                            return cachezzCyberSineWave[idx];

                zzCyberSineWave indicator = new zzCyberSineWave();
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

                zzCyberSineWave[] tmp = new zzCyberSineWave[cachezzCyberSineWave == null ? 1 : cachezzCyberSineWave.Length + 1];
                if (cachezzCyberSineWave != null)
                    cachezzCyberSineWave.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezzCyberSineWave = tmp;
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
        public Indicator.zzCyberSineWave zzCyberSineWave(double smoothAlpha)
        {
            return _indicator.zzCyberSineWave(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.zzCyberSineWave zzCyberSineWave(Data.IDataSeries input, double smoothAlpha)
        {
            return _indicator.zzCyberSineWave(input, smoothAlpha);
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
        public Indicator.zzCyberSineWave zzCyberSineWave(double smoothAlpha)
        {
            return _indicator.zzCyberSineWave(Input, smoothAlpha);
        }

        /// <summary>
        /// CyberCycle Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.zzCyberSineWave zzCyberSineWave(Data.IDataSeries input, double smoothAlpha)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zzCyberSineWave(input, smoothAlpha);
        }
    }
}
#endregion
