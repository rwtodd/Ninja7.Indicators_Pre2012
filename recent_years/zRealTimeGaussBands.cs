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
    public class zRealTimeGaussBands : Indicator
    {
        #region Variables
		private int barsToDisplay = 4;
        // for the Midline ........................
		private double len = 10.0; 
		private static int[][] binco = new int[10][]{
		  new int[] { 1 },
		  new int[] { 1, 1},
		  new int[] { 1, 2, 1},
		  new int[] { 1, 3, 3, 1},
		  new int[] { 1, 4, 6, 4, 1},
		  new int[] { 1, 5, 10, 10, 5, 1},
		  new int[] { 1, 6, 15, 20, 15, 6, 1},
		  new int[] { 1, 7, 21, 35, 35, 21, 7, 1},
		  new int[] { 1, 8, 28, 56, 70, 56, 28, 8, 1},
		  new int[] { 1, 9, 36, 84, 126, 126, 84, 36, 9, 1 }		
		};
		private double[] alphas;
		private double[] avg;
		private double avgTotal;
		private int curGaussIdx;
		private int poles = 5;		
		
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

		private double calcAvgTotal() {  // the avg for this bar, missing
			                             // the term for this bar
			int myIdx = curGaussIdx + 1;
			if(myIdx > poles) myIdx = 0;
			
			double ans = 0.0;
			for(int i = 1; i <= poles; ++i) {
			  	if(--myIdx < 0) myIdx = poles;
				ans += (((i&1)==1)?1.0:-1.0)*alphas[i]*avg[myIdx];
			}
			return ans;
		}
		
		private double newGaussVal(double price, double prevTotal) {
		    return (price*alphas[0] + prevTotal);	
		}
		
		private void initGauss(double price) {
		   if(poles < 1) poles = 1;
		   if(poles > 9) poles = 9;
		
			double beta = ( 1.0 - Math.Cos(2.0*Math.PI/len) ) / ( Math.Pow(1.4142,2.0/poles) - 1.0 );
			double alpha = ( -beta + Math.Sqrt(beta*beta + 2.0*beta) );

			alphas = new double[poles+1];
			alphas[0] = Math.Pow(alpha,poles);
			for(int i = 1; i < poles+1; ++i) {
			  alphas[i] = Math.Pow(1.0-alpha,i)*binco[poles][i];	
			}
			avg = new double[poles+1];

			for(int i =0; i <= poles; ++i) avg[i] = price;
			curIdx = 0;
			avgTotal = calcAvgTotal();			
		}
		
		protected override void OnStartUp() {
			// midline
			initGauss(Median[0]);
			prevMedian = Median[0];
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
			
			cubema1 = avg[0];
			cubema2 = avg[0];
			cubema3 = avg[0];
			clbema1 = avg[0];
			clbema2 = avg[0];
			clbema3 = avg[0];
			cnewVal = 0.0;
			cdevSum = 0.0;
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
			  // NEW BAR PROCESSING
			  prevBar = CurrentBar;
  			  if( barsToDisplay > 0  && prevBar > barsToDisplay) {
  			    UpperBand.Reset(barsToDisplay);
			    LowerBand.Reset(barsToDisplay);
			    MidLine.Reset(barsToDisplay);
			  }
			
			  // midline
			  var gv = newGaussVal(prevMedian,avgTotal);
			  if(++curGaussIdx > poles) curGaussIdx = 0;
			  avg[curGaussIdx] = gv;
			  avgTotal = calcAvgTotal();
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
			  var ngv = newGaussVal(prevMedian,avgTotal);
				
			  // runerr...
			  cnewVal = prevMedian - ngv;
			  cnewVal = cnewVal*cnewVal;
			  cdevSum = devSum - devs[curIdx] + cnewVal;			
			  var stdDev = numDevs * Math.Sqrt(Math.Max(cdevSum/devLen,1e-10));
				
			  // smooth bands...
			  cubema1 = ubema1 + balpha1*(ngv+stdDev - ubema1);
			  cubema2 = ubema2 + balpha2*(ngv+stdDev - ubema2);
			  cubema3 = ubema3 + balpha3*(cubema2+cubema2-cubema1-ubema3);
				
			  clbema1 = lbema1 + balpha1*(ngv-stdDev - lbema1);
			  clbema2 = lbema2 + balpha2*(ngv-stdDev - lbema2);
			  clbema3 = lbema3 + balpha3*(clbema2+clbema2-clbema1-lbema3);
				
              UpperBand.Set(cubema3);
              LowerBand.Set(clbema3);
	   		  MidLine.Set(ngv);
				
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
            set { len = value; devLen = (int)value; }
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
        public int Poles
        {
            get { return poles; }
            set { poles = value; }
        }		
        [Description("")]
        [GridCategory("Parameters")]
        public int BandLength
        {
            get { return devLen; }
            set { devLen = value; len = (double)value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int DisplayedBars
        {
            get { return barsToDisplay; }
            set { barsToDisplay = value; }
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
        private zRealTimeGaussBands[] cachezRealTimeGaussBands = null;

        private static zRealTimeGaussBands checkzRealTimeGaussBands = new zRealTimeGaussBands();

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public zRealTimeGaussBands zRealTimeGaussBands(int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            return zRealTimeGaussBands(Input, bandLength, bandSmoothing, displayedBars, length, numDevs, poles);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public zRealTimeGaussBands zRealTimeGaussBands(Data.IDataSeries input, int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            if (cachezRealTimeGaussBands != null)
                for (int idx = 0; idx < cachezRealTimeGaussBands.Length; idx++)
                    if (cachezRealTimeGaussBands[idx].BandLength == bandLength && Math.Abs(cachezRealTimeGaussBands[idx].BandSmoothing - bandSmoothing) <= double.Epsilon && cachezRealTimeGaussBands[idx].DisplayedBars == displayedBars && Math.Abs(cachezRealTimeGaussBands[idx].Length - length) <= double.Epsilon && Math.Abs(cachezRealTimeGaussBands[idx].NumDevs - numDevs) <= double.Epsilon && cachezRealTimeGaussBands[idx].Poles == poles && cachezRealTimeGaussBands[idx].EqualsInput(input))
                        return cachezRealTimeGaussBands[idx];

            lock (checkzRealTimeGaussBands)
            {
                checkzRealTimeGaussBands.BandLength = bandLength;
                bandLength = checkzRealTimeGaussBands.BandLength;
                checkzRealTimeGaussBands.BandSmoothing = bandSmoothing;
                bandSmoothing = checkzRealTimeGaussBands.BandSmoothing;
                checkzRealTimeGaussBands.DisplayedBars = displayedBars;
                displayedBars = checkzRealTimeGaussBands.DisplayedBars;
                checkzRealTimeGaussBands.Length = length;
                length = checkzRealTimeGaussBands.Length;
                checkzRealTimeGaussBands.NumDevs = numDevs;
                numDevs = checkzRealTimeGaussBands.NumDevs;
                checkzRealTimeGaussBands.Poles = poles;
                poles = checkzRealTimeGaussBands.Poles;

                if (cachezRealTimeGaussBands != null)
                    for (int idx = 0; idx < cachezRealTimeGaussBands.Length; idx++)
                        if (cachezRealTimeGaussBands[idx].BandLength == bandLength && Math.Abs(cachezRealTimeGaussBands[idx].BandSmoothing - bandSmoothing) <= double.Epsilon && cachezRealTimeGaussBands[idx].DisplayedBars == displayedBars && Math.Abs(cachezRealTimeGaussBands[idx].Length - length) <= double.Epsilon && Math.Abs(cachezRealTimeGaussBands[idx].NumDevs - numDevs) <= double.Epsilon && cachezRealTimeGaussBands[idx].Poles == poles && cachezRealTimeGaussBands[idx].EqualsInput(input))
                            return cachezRealTimeGaussBands[idx];

                zRealTimeGaussBands indicator = new zRealTimeGaussBands();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandLength = bandLength;
                indicator.BandSmoothing = bandSmoothing;
                indicator.DisplayedBars = displayedBars;
                indicator.Length = length;
                indicator.NumDevs = numDevs;
                indicator.Poles = poles;
                Indicators.Add(indicator);
                indicator.SetUp();

                zRealTimeGaussBands[] tmp = new zRealTimeGaussBands[cachezRealTimeGaussBands == null ? 1 : cachezRealTimeGaussBands.Length + 1];
                if (cachezRealTimeGaussBands != null)
                    cachezRealTimeGaussBands.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezRealTimeGaussBands = tmp;
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
        public Indicator.zRealTimeGaussBands zRealTimeGaussBands(int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            return _indicator.zRealTimeGaussBands(Input, bandLength, bandSmoothing, displayedBars, length, numDevs, poles);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zRealTimeGaussBands zRealTimeGaussBands(Data.IDataSeries input, int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            return _indicator.zRealTimeGaussBands(input, bandLength, bandSmoothing, displayedBars, length, numDevs, poles);
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
        public Indicator.zRealTimeGaussBands zRealTimeGaussBands(int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            return _indicator.zRealTimeGaussBands(Input, bandLength, bandSmoothing, displayedBars, length, numDevs, poles);
        }

        /// <summary>
        /// Special every-tick bands
        /// </summary>
        /// <returns></returns>
        public Indicator.zRealTimeGaussBands zRealTimeGaussBands(Data.IDataSeries input, int bandLength, double bandSmoothing, int displayedBars, double length, double numDevs, int poles)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zRealTimeGaussBands(input, bandLength, bandSmoothing, displayedBars, length, numDevs, poles);
        }
    }
}
#endregion
