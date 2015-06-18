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
    /// Richard Todd www.movethemarkets.com
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com")]
    public class Z20100124ChangePtVWAP : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int grouping = 30; // Default setting for Grouping
            private EventGroupingMethod groupMethod = EventGroupingMethod.Minutes;
            private int startTime = 830; // Default setting for StartTime
		    private Z20091203PeriodicEvent pe;
            private double numSDs1 = 1; // Default setting for NumSDs1
            private double numSDs2 = 2.000; // Default setting for NumSDs2
            private double lastVolume;
		    private double minWidth = 8; // width in ticks.
		    private int NUM_CALCS = 10;
            private double threshold = 4.000; // Default setting for Threshold
        // User defined variables (add any user defined variables below)
		
			// for tracking and scorekeeping
		    // scorematrix[ contender , to-replace ]
		    double[,] scoreMatrix;
		
		    private class LikelyCalculation {
		       private double algMean, algS,algSumweight;
			   private double stdDev;
				
			   private static double[] dvals = {1.0, 0.920344, 0.841481, 
					0.764177, 0.689157, 0.617075, 0.548506, 0.483927, 0.423711, 0.36812, 
					0.317311, 0.271332, 0.230139, 0.193601, 0.161513, 0.133614, 0.109599, 
					0.0891309, 0.0718606, 0.0574331, 0.0455003, 0.0357288, 0.0278069, 
					0.0214482, 0.0163951, 0.0124193, 0.00932238, 0.00693395, 0.00511026, 
					0.00373163, 0.0026998, 0.00193521, 0.00137428, 0.000966848, 0.000673859, 
					0.000465258, 0.000318217, 0.000215599, 0.000144696, 0.0000961927, 0.0000633425 };
			
			   private double normalPct(double devs) {
			     double amount = Math.Abs(devs) / 0.1;
				 if(amount >= 40) return dvals[dvals.Length - 1];
				 int indx = Convert.ToInt32(Math.Floor(amount));
				 double pct = amount - (double)indx;
				 return dvals[indx]*(1.0-pct) + dvals[indx+1]*pct;
			   }
			
			   public void reset() {
			     algMean = 0;
			     algS = 0;
			     algSumweight = 0;	
				 stdDev = 0.0001;
			   }
			
			   public LikelyCalculation() {
				  reset();
			   }
			 
			   // DEBUG
			   public double devs(double price) {
				  return (price-algMean)/stdDev;
			   } public double stddev { get { return stdDev; } }
			
			   public double calcLikelihood(double price) {
				  return normalPct((price - algMean)/Math.Max(stdDev,0.00001));
			   }
			
			   public double Mean { get { return algMean; } }
			   public double upperSD(double amount) { 
				 return algMean + stdDev*amount;
			   }
			   public double lowerSD(double amount) { 
				 return algMean - stdDev*amount;
			   }
			
			   public void update(double weight, double price) {
			      double temp = weight + algSumweight;
			      double q = price - algMean;
			      double r = q * weight / temp;
			      algS = algS + algSumweight * q * r;
			      algMean = algMean + r;
			      algSumweight = temp;
			
			      stdDev = Math.Sqrt( algS / algSumweight );
			   }
				
			}

			private LikelyCalculation[] calcs;
			private double[] likelihoods;
		    private int oldest;
            private int best;
			
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "Mean"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "UpperSD"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "LowerSD"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "UpperSD2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "LowerSD2"));

            CalculateOnBarClose	= true;
            Overlay				= true;
			pe = null;
			oldest = 0;
			best = 0;
        }
		
		private void initScoreMatrix() {
		  likelihoods = new double[NUM_CALCS];
		  calcs = new LikelyCalculation[NUM_CALCS];
		  for(int i = 0; i < NUM_CALCS; ++i) {
		     calcs[i] = new LikelyCalculation();	
		  }
		  oldest = 0;
		  best = 0;
		  scoreMatrix = new double[NUM_CALCS,NUM_CALCS];
		  for(int i = 0; i < NUM_CALCS;++i) 
			for(int j = 0; j<NUM_CALCS;++j)
				scoreMatrix[i,j] = 0.0;
		}

		private void updateScoreMatrix(double price) {
		  //Print("oldest = "+oldest+" and best= "+best); // DEBUG
		  for(int i = 0; i<NUM_CALCS; ++i) {
			//Print(i+": devs= "+calcs[i].devs(price)+" at price= "+price+" with std dev= "+calcs[i].stddev+" and mean "+calcs[i].Mean);
			if( (calcs[i].upperSD(numSDs2) - calcs[i].lowerSD(numSDs2)) >= (minWidth*TickSize))
			   likelihoods[i] = calcs[i].calcLikelihood(price);
			else
			   likelihoods[i] = 0.0000633425;
			// Print(likelihoods[i]); DEBUG
		  }
		  for(int i = 0; i<NUM_CALCS; ++i) {
		    for(int j = 0; j<NUM_CALCS; ++j) {
				// i == replacOR , j == replacEE
				scoreMatrix[i,j] = Math.Max(0.0, scoreMatrix[i,j] + Math.Log( likelihoods[i] / likelihoods[j] ));
				// Print("SM["+i+":"+j+"]="+scoreMatrix[i,j]); // DEBUG
			}
		  }
		}
		
		private void chooseBest() {
		  double max = -1;
		  int maxIndex = -1;
		  if(oldest <= best) {
			for(int i = best; i < NUM_CALCS;++i) {
             //Print("SM["+i+":"+best+"]="+scoreMatrix[i,best]); // DEBUG
			 if(scoreMatrix[i,best] > max) {
			   max = scoreMatrix[i,best];
			   maxIndex = i;
			 }				
			}
			for(int i = 0; i < oldest; ++i) {
             //Print("SM["+i+":"+best+"]="+scoreMatrix[i,best]); // DEBUG
			 if(scoreMatrix[i,best] > max) {
			   max = scoreMatrix[i,best];
			   maxIndex = i;
			 }							  	
			}
		  } else {
			for(int i = best; i < oldest;++i) {
             //Print("SM["+i+":"+best+"]="+scoreMatrix[i,best]); // DEBUG
			 if(scoreMatrix[i,best] > max) {
			   max = scoreMatrix[i,best];
			   maxIndex = i;
			 }				
			}			
		  }
		
		  if(max > threshold) {
			Print("New BEST!... "+CurrentBar+" index: "+maxIndex+" score: "+max); // DEBUG
			best = maxIndex;
			//for(int i = 0; i < NUM_CALCS; ++i) scoreMatrix[i,best] = 1.0;

			// now get rid of all oldest ones...
//		    if(oldest < best) {
//			  for(int i = oldest; i < best; ++i) /*if(i != oldbest)*/ calcs[i].reset();	
//			} else if(oldest > best) {
//			  for(int i = oldest; i < NUM_CALCS; ++i) /* if(i != oldbest) */ calcs[i].reset();
//			  for(int i = 0; i < best; ++i) /* if(i != oldbest) */ calcs[i].reset();
//			}

		  }
		}
		
		private void addNewCalc() {
		  //Print("New Calc added..."); // DEBUG
		  if(oldest == best) { ++oldest; if(oldest >= NUM_CALCS) oldest = 0; }
		  calcs[oldest].reset();
		  for(int i = 0; i < NUM_CALCS; ++i) {
		    scoreMatrix[oldest,i] = 1.0;
			scoreMatrix[i,oldest] = 1.0;
		  }
		  ++oldest; // next time, we replace _this_ one!
		  if(oldest >= NUM_CALCS) oldest = 0;
		}
		
		private void updateVWAPS() {			
			double x = Median[0];
			double weight = Volume[0];
						
			for(int i = 0; i < NUM_CALCS; ++i) {
			  calcs[i].update(weight,x);	
			}
		}
				
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(pe == null) {
						pe = Z20091203PeriodicEvent(Input,groupMethod,grouping,startTime);
			            initScoreMatrix();			
            }
			//Print("New update"); // DEBUG									
			updateVWAPS();
						  
			updateScoreMatrix(Close[0]);
			chooseBest();	 
				
			if(pe.IsNewBar[0]) {
			    addNewCalc(); 
  			}
			
			Mean.Set(calcs[best].Mean);
            UpperSD.Set(calcs[best].upperSD(numSDs1));
            LowerSD.Set(calcs[best].lowerSD(numSDs1));
            UpperSD2.Set(calcs[best].upperSD(numSDs2));
            LowerSD2.Set(calcs[best].lowerSD(numSDs2));
        }

        #region Properties
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Mean
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperSD
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerSD
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperSD2
        {
            get { return Values[3]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerSD2
        {
            get { return Values[4]; }
        }

        [Description("How many bars/minutes to group together?")]
        [Category("Parameters")]
        public int GroupingSize
        {
            get { return grouping; }
            set { grouping = Math.Max(0, value); }
        }
		
        [Description("How many concurrent VWAPs?")]
        [Category("Parameters")]
        public int ConcurrentVWAPS
        {
            get { return NUM_CALCS; }
            set { NUM_CALCS = Math.Max(2, value); }
        }
		
		[Description("What are we counting for periodic events?")]
        [Category("Parameters")]
        public EventGroupingMethod GroupingBy
        {
            get { return groupMethod; }
            set { groupMethod = value; }
        }

        [Description("How many standard deviations?")]
        [Category("Parameters")]
        public double NumSDs1
        {
            get { return numSDs1; }
            set { numSDs1 = Math.Max(0.000, value); }
        }

        [Description("How many standard deviations?")]
        [Category("Parameters")]
        public double NumSDs2
        {
            get { return numSDs2; }
            set { numSDs2 = Math.Max(0.000, value); }
        }
        [Description("What time does the session start?")]
        [Category("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = Math.Max(0, value); }
        }

        [Description("Threshold for changepoint detection")]
        [Category("Parameters")]
        public double Threshold
        {
            get { return threshold; }
            set { threshold = Math.Max(0, value); }
        }
        [Description("Minimum width in ticks for a vwap to be considered...")]
        [Category("Parameters")]
        public double MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
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
        private Z20100124ChangePtVWAP[] cacheZ20100124ChangePtVWAP = null;

        private static Z20100124ChangePtVWAP checkZ20100124ChangePtVWAP = new Z20100124ChangePtVWAP();

        /// <summary>
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Z20100124ChangePtVWAP Z20100124ChangePtVWAP(int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            return Z20100124ChangePtVWAP(Input, concurrentVWAPS, groupingBy, groupingSize, minWidth, numSDs1, numSDs2, startTime, threshold);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Z20100124ChangePtVWAP Z20100124ChangePtVWAP(Data.IDataSeries input, int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            if (cacheZ20100124ChangePtVWAP != null)
                for (int idx = 0; idx < cacheZ20100124ChangePtVWAP.Length; idx++)
                    if (cacheZ20100124ChangePtVWAP[idx].ConcurrentVWAPS == concurrentVWAPS && cacheZ20100124ChangePtVWAP[idx].GroupingBy == groupingBy && cacheZ20100124ChangePtVWAP[idx].GroupingSize == groupingSize && Math.Abs(cacheZ20100124ChangePtVWAP[idx].MinWidth - minWidth) <= double.Epsilon && Math.Abs(cacheZ20100124ChangePtVWAP[idx].NumSDs1 - numSDs1) <= double.Epsilon && Math.Abs(cacheZ20100124ChangePtVWAP[idx].NumSDs2 - numSDs2) <= double.Epsilon && cacheZ20100124ChangePtVWAP[idx].StartTime == startTime && Math.Abs(cacheZ20100124ChangePtVWAP[idx].Threshold - threshold) <= double.Epsilon && cacheZ20100124ChangePtVWAP[idx].EqualsInput(input))
                        return cacheZ20100124ChangePtVWAP[idx];

            lock (checkZ20100124ChangePtVWAP)
            {
                checkZ20100124ChangePtVWAP.ConcurrentVWAPS = concurrentVWAPS;
                concurrentVWAPS = checkZ20100124ChangePtVWAP.ConcurrentVWAPS;
                checkZ20100124ChangePtVWAP.GroupingBy = groupingBy;
                groupingBy = checkZ20100124ChangePtVWAP.GroupingBy;
                checkZ20100124ChangePtVWAP.GroupingSize = groupingSize;
                groupingSize = checkZ20100124ChangePtVWAP.GroupingSize;
                checkZ20100124ChangePtVWAP.MinWidth = minWidth;
                minWidth = checkZ20100124ChangePtVWAP.MinWidth;
                checkZ20100124ChangePtVWAP.NumSDs1 = numSDs1;
                numSDs1 = checkZ20100124ChangePtVWAP.NumSDs1;
                checkZ20100124ChangePtVWAP.NumSDs2 = numSDs2;
                numSDs2 = checkZ20100124ChangePtVWAP.NumSDs2;
                checkZ20100124ChangePtVWAP.StartTime = startTime;
                startTime = checkZ20100124ChangePtVWAP.StartTime;
                checkZ20100124ChangePtVWAP.Threshold = threshold;
                threshold = checkZ20100124ChangePtVWAP.Threshold;

                if (cacheZ20100124ChangePtVWAP != null)
                    for (int idx = 0; idx < cacheZ20100124ChangePtVWAP.Length; idx++)
                        if (cacheZ20100124ChangePtVWAP[idx].ConcurrentVWAPS == concurrentVWAPS && cacheZ20100124ChangePtVWAP[idx].GroupingBy == groupingBy && cacheZ20100124ChangePtVWAP[idx].GroupingSize == groupingSize && Math.Abs(cacheZ20100124ChangePtVWAP[idx].MinWidth - minWidth) <= double.Epsilon && Math.Abs(cacheZ20100124ChangePtVWAP[idx].NumSDs1 - numSDs1) <= double.Epsilon && Math.Abs(cacheZ20100124ChangePtVWAP[idx].NumSDs2 - numSDs2) <= double.Epsilon && cacheZ20100124ChangePtVWAP[idx].StartTime == startTime && Math.Abs(cacheZ20100124ChangePtVWAP[idx].Threshold - threshold) <= double.Epsilon && cacheZ20100124ChangePtVWAP[idx].EqualsInput(input))
                            return cacheZ20100124ChangePtVWAP[idx];

                Z20100124ChangePtVWAP indicator = new Z20100124ChangePtVWAP();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ConcurrentVWAPS = concurrentVWAPS;
                indicator.GroupingBy = groupingBy;
                indicator.GroupingSize = groupingSize;
                indicator.MinWidth = minWidth;
                indicator.NumSDs1 = numSDs1;
                indicator.NumSDs2 = numSDs2;
                indicator.StartTime = startTime;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100124ChangePtVWAP[] tmp = new Z20100124ChangePtVWAP[cacheZ20100124ChangePtVWAP == null ? 1 : cacheZ20100124ChangePtVWAP.Length + 1];
                if (cacheZ20100124ChangePtVWAP != null)
                    cacheZ20100124ChangePtVWAP.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100124ChangePtVWAP = tmp;
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
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100124ChangePtVWAP Z20100124ChangePtVWAP(int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            return _indicator.Z20100124ChangePtVWAP(Input, concurrentVWAPS, groupingBy, groupingSize, minWidth, numSDs1, numSDs2, startTime, threshold);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100124ChangePtVWAP Z20100124ChangePtVWAP(Data.IDataSeries input, int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            return _indicator.Z20100124ChangePtVWAP(input, concurrentVWAPS, groupingBy, groupingSize, minWidth, numSDs1, numSDs2, startTime, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100124ChangePtVWAP Z20100124ChangePtVWAP(int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            return _indicator.Z20100124ChangePtVWAP(Input, concurrentVWAPS, groupingBy, groupingSize, minWidth, numSDs1, numSDs2, startTime, threshold);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100124ChangePtVWAP Z20100124ChangePtVWAP(Data.IDataSeries input, int concurrentVWAPS, EventGroupingMethod groupingBy, int groupingSize, double minWidth, double numSDs1, double numSDs2, int startTime, double threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100124ChangePtVWAP(input, concurrentVWAPS, groupingBy, groupingSize, minWidth, numSDs1, numSDs2, startTime, threshold);
        }
    }
}
#endregion
