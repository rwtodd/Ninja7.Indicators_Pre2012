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
    /// RankSumTest
    /// </summary>
    [Description("RankSumTest")]
    public class zRankSum : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int n = 14; // Default setting for N
		    private int len = 28;
			private double mu, sigma;
		    private double[] zSorted;
			private int[] zIndexes;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Z"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "Zero"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
		  mu = 	n*(2*n+1)*0.5;
		  sigma = Math.Sqrt(n*n*(2*n+1)/12.0);
		  len = 2*n;
		  zSorted = new double[len];
		  zIndexes = new int[len];
		}
		
		private void prettyPrintArrs() {
		  Print("\nAt start of "+CurrentBar);
		  for(int i = 0; i < len; ++i) {
		    Print("  "+i+": Bar "+zIndexes[i]+" is "+zSorted[i]); 	
		  } 
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
  			if(CurrentBar == 0) {
			   for(int i = 0; i < len; ++i) {
				zSorted[i] = Input[0];
				zIndexes[i] = i-len; // zIndexes always has CurrentBar
			   }
			}

			//prettyPrintArrs();
			
			double toAdd = Input[0];
			int idxAdd = CurrentBar;
			int idxDel = CurrentBar - len;
			double toDel = Input[CurrentBar-Math.Max(idxDel,0)];
			//Print("Adding Bar "+idxAdd+" ("+toAdd+")");
			//Print("Deleting Bar "+idxDel+" ("+toDel+")");
			
			// locate the deletion...
			int loc = Array.BinarySearch<double>(zSorted,toDel);
			if(loc < 0) { 
				Print("loc = "+loc+" when looking for "+toDel);
				foreach(double x in zSorted) Print(x);
			}
			//Print("BinSearch found loc "+loc);
			// search left to right until we find our idxDel...
			while(zIndexes[loc] > idxDel) { loc--; /* RWT  if(loc < 0) Print("ISSUE!");*/ }
			while(zIndexes[loc] < idxDel) { loc++; /* RWT  if(loc >= len) Print("--ISSUE!"); */ } 
			
			/* RWT DEBUG */ if((zSorted[loc] != toDel) || (zIndexes[loc] != idxDel)) Print("PROBLEM!!!");
			// Print("At location "+loc+"...."); /* RWT DEBUG */
			
			// figure out the direction...
			if(toDel <= toAdd) {
			  // going forward...	
			  while(loc++ < len) {
				 var tst = (loc < len)?zSorted[loc]:Double.MaxValue;
				 var tstIdx = (loc < len)?zIndexes[loc]:(len+1);
				 if((tst < toAdd) || 
					 ((tst == toAdd) && (tstIdx < idxAdd)) 
					) {
				   zSorted[loc-1] = tst;
				   zIndexes[loc-1] = tstIdx;
				 } else {
				   zSorted[loc-1] = toAdd;
				   zIndexes[loc-1] = idxAdd;
				   break; // DONE!
				 }
			  }
			} else {
			  // going backward...
			  while(loc-- >= 0) {
			     var tst = (loc >= 0)?zSorted[loc]:Double.MinValue;
				 var tstIdx = (loc >= 0)?zIndexes[loc]:-len-1;
				 if( (tst > toAdd) ||
					((tst == toAdd) && (tstIdx > idxAdd)) 
				){
				   zSorted[loc+1] = tst;
				   zIndexes[loc+1] = tstIdx;
				 } else {
				   zSorted[loc+1] = toAdd;
				   zIndexes[loc+1] = idxAdd;
				   break; // DONE!
				 }
			  }
			}			
			
			if(CurrentBar < len) {
			  Z.Set(0.0); return;	
			}
			
			// now score the ranked list...
			int totScore = 0;
			for(int i = 0; i < len; ++i)  {
			  if(zIndexes[i] > (CurrentBar - n)) totScore += i;
			}
			Z.Set( (totScore - mu)/sigma );
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Z
        {
            get { return Values[0]; }
        }

        [Description("Number of bars per side")]
        [GridCategory("Parameters")]
        public int N
        {
            get { return n; }
            set { n = Math.Max(2, value); }
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
        private zRankSum[] cachezRankSum = null;

        private static zRankSum checkzRankSum = new zRankSum();

        /// <summary>
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        public zRankSum zRankSum(int n)
        {
            return zRankSum(Input, n);
        }

        /// <summary>
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        public zRankSum zRankSum(Data.IDataSeries input, int n)
        {
            if (cachezRankSum != null)
                for (int idx = 0; idx < cachezRankSum.Length; idx++)
                    if (cachezRankSum[idx].N == n && cachezRankSum[idx].EqualsInput(input))
                        return cachezRankSum[idx];

            lock (checkzRankSum)
            {
                checkzRankSum.N = n;
                n = checkzRankSum.N;

                if (cachezRankSum != null)
                    for (int idx = 0; idx < cachezRankSum.Length; idx++)
                        if (cachezRankSum[idx].N == n && cachezRankSum[idx].EqualsInput(input))
                            return cachezRankSum[idx];

                zRankSum indicator = new zRankSum();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.N = n;
                Indicators.Add(indicator);
                indicator.SetUp();

                zRankSum[] tmp = new zRankSum[cachezRankSum == null ? 1 : cachezRankSum.Length + 1];
                if (cachezRankSum != null)
                    cachezRankSum.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezRankSum = tmp;
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
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRankSum zRankSum(int n)
        {
            return _indicator.zRankSum(Input, n);
        }

        /// <summary>
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        public Indicator.zRankSum zRankSum(Data.IDataSeries input, int n)
        {
            return _indicator.zRankSum(input, n);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRankSum zRankSum(int n)
        {
            return _indicator.zRankSum(Input, n);
        }

        /// <summary>
        /// RankSumTest
        /// </summary>
        /// <returns></returns>
        public Indicator.zRankSum zRankSum(Data.IDataSeries input, int n)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zRankSum(input, n);
        }
    }
}
#endregion
