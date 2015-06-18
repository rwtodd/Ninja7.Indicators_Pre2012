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
    /// Special every-tick bands
    /// </summary>
    [Description("Special every-tick bands")]
    public class zRealTimeBands : Indicator
    {
        #region Variables
        // for the Midline ........................
		private double len = 10.0; 
		private double alpha1; // len
		private double alpha2; // lenn/2
		private double alpha3; // sqrt(len)
		private double ema1, cema1;
		private double ema2, cema2;
		private double ema3, cema3;
		
		// for smoothing the bands...
		private double bandSmooth = 4.0;
		private double balpha1; // len
		private double balpha2; // lenn/2
		private double balpha3; // sqrt(len)
		private double ubema1, cubema1;
		private double ubema2, cubema2;
		private double ubema3, cubema3;
		private double lbema1, clbema1;
		private double lbema2, clbema2;
		private double lbema3, clbema3;
		
		private double prevMedian;
		private int prevBar;
		// for the runerr calculation ..................
		private double cnewVal;
		private int devLen = 10;
		private double numDevs = 2.0;
		private double[] devs;
		private int curIdx;
		private double devSum, cdevSum;
		
        // User defined variables (add any user defined variables below)
        #endregion

		protected override void OnStartUp() {
			// midline
			alpha1 = 2.0/(len+1.0);
			alpha2 = 2.0/(0.5*len+1.0);
			alpha3 = 2.0/(Math.Sqrt(len)+1.0);
			prevMedian = -1;
			prevBar = -1;

			// smoothing bands...
			balpha1 = 2.0/(bandSmooth+1.0);
			balpha2 = 2.0/(0.5*bandSmooth+1.0);
			balpha3 = 2.0/(Math.Sqrt(bandSmooth)+1.0);
			
			// runerr
			devs = new double[devLen];
			for(int i = 0; i < devLen; ++i) devs[i] = 0.0;
			curIdx = 0;
			devSum = 0;
			cdevSum = 0;
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Tomato), PlotStyle.Line, "UpperBand"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Tomato), PlotStyle.Line, "LowerBand"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Tomato), PlotStyle.Line, "Midline"));
            Overlay				= true;
			CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar > prevBar) {
			  prevBar = CurrentBar;

			  if(CurrentBar == 0) {
			     cema1 = Median[0];
				 cema2 = cema1;
				 cema3 = cema1;
				 cubema1 = cema1;
				 cubema2 = cema1;
				 cubema3 = cema1;
				 clbema1 = cema1;
				 clbema2 = cema1;
				 clbema3 = cema1;
				 cnewVal = 0.0;
				 cdevSum = 0.0;
			  }
			
			  // midline
			  ema1 = cema1;
			  ema2 = cema2;
			  ema3 = cema3;
			  prevMedian = -1;
			
			  // runerr
			  devSum = cdevSum;
			  devs[curIdx] = cnewVal;
			  if(++curIdx >= devLen) curIdx = 0;
			
			  // smooth bands
			  ubema1 = cubema1;
			  ubema2 = cubema2;
			  ubema3 = cubema3;
			  lbema1 = clbema1;
			  lbema2 = clbema2;
			  lbema3 = clbema3;			  
			}
			
			if(prevMedian != Median[0]) {
			  prevMedian = Median[0];
			  // midline...
			  cema1 = ema1 + alpha1*(prevMedian - ema1);
			  cema2 = ema2 + alpha2*(prevMedian - ema2);
			  cema3 = ema3 + alpha3*( cema2 + cema2 - cema1 - ema3 );	
				
			  // runerr...
			  cnewVal = prevMedian - cema3;
			  cnewVal = cnewVal*cnewVal;
			  cdevSum = devSum - devs[curIdx] + cnewVal;			
			  var stdDev = numDevs * Math.Sqrt(Math.Max(cdevSum/devLen,1e-10));
				
			  // smooth bands...
			  cubema1 = ubema1 + balpha1*(cema3+stdDev - ubema1);
			  cubema2 = ubema2 + balpha2*(cema3+stdDev - ubema2);
			  cubema3 = ubema3 + balpha3*(cubema2+cubema2-cubema1-ubema3);
				
			  clbema1 = lbema1 + balpha1*(cema3-stdDev - lbema1);
			  clbema2 = lbema2 + balpha2*(cema3-stdDev - lbema2);
			  clbema3 = lbema3 + balpha3*(clbema2+clbema2-clbema1-lbema3);
				
              UpperBand.Set(cubema3);
              LowerBand.Set(clbema3);
	   		  MidLine.Set(cema3);
			}			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperBand
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
        public DataSeries MidLine
        {
            get { return Values[2]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Length
        {
            get { return len; }
            set { len = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public double BandSmoothing
        {
            get { return bandSmooth; }
            set { bandSmooth = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int BandLength
        {
            get { return devLen; }
            set { devLen = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public double NumDevs
        {
            get { return numDevs; }
            set { numDevs = value; }
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
        private zRealTimeBands[] cachezRealTimeBands = null;

        private static zRealTimeBands checkzRealTimeBands = new zRealTimeBands();

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public zRealTimeBands zRealTimeBands(int bandLength, double bandSmoothing, double length, double numDevs)
        {
            return zRealTimeBands(Input, bandLength, bandSmoothing, length, numDevs);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public zRealTimeBands zRealTimeBands(Data.IDataSeries input, int bandLength, double bandSmoothing, double length, double numDevs)
        {
            if (cachezRealTimeBands != null)
                for (int idx = 0; idx < cachezRealTimeBands.Length; idx++)
                    if (cachezRealTimeBands[idx].BandLength == bandLength && Math.Abs(cachezRealTimeBands[idx].BandSmoothing - bandSmoothing) <= double.Epsilon && Math.Abs(cachezRealTimeBands[idx].Length - length) <= double.Epsilon && Math.Abs(cachezRealTimeBands[idx].NumDevs - numDevs) <= double.Epsilon && cachezRealTimeBands[idx].EqualsInput(input))
                        return cachezRealTimeBands[idx];

            lock (checkzRealTimeBands)
            {
                checkzRealTimeBands.BandLength = bandLength;
                bandLength = checkzRealTimeBands.BandLength;
                checkzRealTimeBands.BandSmoothing = bandSmoothing;
                bandSmoothing = checkzRealTimeBands.BandSmoothing;
                checkzRealTimeBands.Length = length;
                length = checkzRealTimeBands.Length;
                checkzRealTimeBands.NumDevs = numDevs;
                numDevs = checkzRealTimeBands.NumDevs;

                if (cachezRealTimeBands != null)
                    for (int idx = 0; idx < cachezRealTimeBands.Length; idx++)
                        if (cachezRealTimeBands[idx].BandLength == bandLength && Math.Abs(cachezRealTimeBands[idx].BandSmoothing - bandSmoothing) <= double.Epsilon && Math.Abs(cachezRealTimeBands[idx].Length - length) <= double.Epsilon && Math.Abs(cachezRealTimeBands[idx].NumDevs - numDevs) <= double.Epsilon && cachezRealTimeBands[idx].EqualsInput(input))
                            return cachezRealTimeBands[idx];

                zRealTimeBands indicator = new zRealTimeBands();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandLength = bandLength;
                indicator.BandSmoothing = bandSmoothing;
                indicator.Length = length;
                indicator.NumDevs = numDevs;
                Indicators.Add(indicator);
                indicator.SetUp();

                zRealTimeBands[] tmp = new zRealTimeBands[cachezRealTimeBands == null ? 1 : cachezRealTimeBands.Length + 1];
                if (cachezRealTimeBands != null)
                    cachezRealTimeBands.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezRealTimeBands = tmp;
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
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRealTimeBands zRealTimeBands(int bandLength, double bandSmoothing, double length, double numDevs)
        {
            return _indicator.zRealTimeBands(Input, bandLength, bandSmoothing, length, numDevs);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zRealTimeBands zRealTimeBands(Data.IDataSeries input, int bandLength, double bandSmoothing, double length, double numDevs)
        {
            return _indicator.zRealTimeBands(input, bandLength, bandSmoothing, length, numDevs);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRealTimeBands zRealTimeBands(int bandLength, double bandSmoothing, double length, double numDevs)
        {
            return _indicator.zRealTimeBands(Input, bandLength, bandSmoothing, length, numDevs);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zRealTimeBands zRealTimeBands(Data.IDataSeries input, int bandLength, double bandSmoothing, double length, double numDevs)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zRealTimeBands(input, bandLength, bandSmoothing, length, numDevs);
        }
    }
}
#endregion
