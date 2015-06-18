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
    /// Double Exponential Smoothing p146 in Bowerman book.
    /// </summary>
    [Description("Double Exponential Smoothing p146 in Bowerman book.")]
    public class zDexpSmooth : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alpha = 0.200; // Default setting for Alpha
            private int tau = 1; // Default setting for Tau
			private double ema1;
			private double ema2;
			private double atauConst;
			private bool trackStdDev = false;
			private double[] deviations;
			private int devIndx;
		    
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "DXPMA"));
            Overlay				= true;
			atauConst = (alpha*tau)/(1.0-alpha);
        }

		protected override void OnStartUp() {
		    if(trackStdDev) {
				int sz = (int)(1/alpha - 1);
				deviations = new double[sz*2];
				devIndx = 0;
			}
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) {
				ema1 = Input[0];
				ema2 = ema1;
				return;
			}
			
			ema1 = ema1 + alpha*(Input[0]-ema1);
			ema2 = ema2 + alpha*(ema1-ema2);
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            DXPMA.Set(  (2.0+atauConst)*ema1 - (1.0+atauConst)*ema2 );

			if(trackStdDev) {
				var newval = Median[0] - DXPMA[0];
				deviations[devIndx++] = newval*newval;
				if(devIndx == deviations.Length) devIndx = 0;
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DXPMA
        {
            get { return Values[0]; }
        }
		
		[Browsable(false)]
		[XmlIgnore()]
		public double StdDev {
		   get {
			  // compute on demand
			  double sum = 0.0;
			  foreach(double dev in deviations) {
				sum += dev;	
			  }
			  return Math.Sqrt(sum / Math.Min(deviations.Length,CurrentBar));
		   }
		}
        [Description("Alpha smoothing constant")]
        [GridCategory("Parameters")]
        public double Alpha
        {
            get { return alpha; }
            set { if(value > 1) value = 2.0/(value+1.0);
				alpha = Math.Max(0.00, value); 
			}
        }

        [Description("Track StdDev?")]
        [GridCategory("Parameters")]
        public bool TrackStandardDeviations
        {
            get { return trackStdDev; }
            set { trackStdDev = value; 
			}
        }

        [Description("Number of periods to forecast ahead")]
        [GridCategory("Parameters")]
        public int Tau
        {
            get { return tau; }
            set { tau = Math.Max(0, value); }
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
        private zDexpSmooth[] cachezDexpSmooth = null;

        private static zDexpSmooth checkzDexpSmooth = new zDexpSmooth();

        /// <summary>
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        public zDexpSmooth zDexpSmooth(double alpha, int tau, bool trackStandardDeviations)
        {
            return zDexpSmooth(Input, alpha, tau, trackStandardDeviations);
        }

        /// <summary>
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        public zDexpSmooth zDexpSmooth(Data.IDataSeries input, double alpha, int tau, bool trackStandardDeviations)
        {
            if (cachezDexpSmooth != null)
                for (int idx = 0; idx < cachezDexpSmooth.Length; idx++)
                    if (Math.Abs(cachezDexpSmooth[idx].Alpha - alpha) <= double.Epsilon && cachezDexpSmooth[idx].Tau == tau && cachezDexpSmooth[idx].TrackStandardDeviations == trackStandardDeviations && cachezDexpSmooth[idx].EqualsInput(input))
                        return cachezDexpSmooth[idx];

            lock (checkzDexpSmooth)
            {
                checkzDexpSmooth.Alpha = alpha;
                alpha = checkzDexpSmooth.Alpha;
                checkzDexpSmooth.Tau = tau;
                tau = checkzDexpSmooth.Tau;
                checkzDexpSmooth.TrackStandardDeviations = trackStandardDeviations;
                trackStandardDeviations = checkzDexpSmooth.TrackStandardDeviations;

                if (cachezDexpSmooth != null)
                    for (int idx = 0; idx < cachezDexpSmooth.Length; idx++)
                        if (Math.Abs(cachezDexpSmooth[idx].Alpha - alpha) <= double.Epsilon && cachezDexpSmooth[idx].Tau == tau && cachezDexpSmooth[idx].TrackStandardDeviations == trackStandardDeviations && cachezDexpSmooth[idx].EqualsInput(input))
                            return cachezDexpSmooth[idx];

                zDexpSmooth indicator = new zDexpSmooth();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Alpha = alpha;
                indicator.Tau = tau;
                indicator.TrackStandardDeviations = trackStandardDeviations;
                Indicators.Add(indicator);
                indicator.SetUp();

                zDexpSmooth[] tmp = new zDexpSmooth[cachezDexpSmooth == null ? 1 : cachezDexpSmooth.Length + 1];
                if (cachezDexpSmooth != null)
                    cachezDexpSmooth.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezDexpSmooth = tmp;
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
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpSmooth zDexpSmooth(double alpha, int tau, bool trackStandardDeviations)
        {
            return _indicator.zDexpSmooth(Input, alpha, tau, trackStandardDeviations);
        }

        /// <summary>
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpSmooth zDexpSmooth(Data.IDataSeries input, double alpha, int tau, bool trackStandardDeviations)
        {
            return _indicator.zDexpSmooth(input, alpha, tau, trackStandardDeviations);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpSmooth zDexpSmooth(double alpha, int tau, bool trackStandardDeviations)
        {
            return _indicator.zDexpSmooth(Input, alpha, tau, trackStandardDeviations);
        }

        /// <summary>
        /// Double Exponential Smoothing p146 in Bowerman book.
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpSmooth zDexpSmooth(Data.IDataSeries input, double alpha, int tau, bool trackStandardDeviations)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zDexpSmooth(input, alpha, tau, trackStandardDeviations);
        }
    }
}
#endregion
