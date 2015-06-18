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
    /// A paintbar based on the TAS&C Clear concept from September 2010 issue
    /// </summary>
    [Description("A paintbar based on the TAS&C Clear concept from September 2010 issue")]
    public class Z20100909ClearTrend : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool trendBarReqd = false; // Default setting for TrendBarReqd
		    private Color colorUP = Color.LimeGreen;
		    private Color colorDN = Color.Red;

        // User defined variables (add any user defined variables below)
		    private double lowestHigh, highestLow;
		    private bool goingUp;
		    private double plowestHigh, phighestLow;
		    private bool pgoingUp;
		    private int lastSeen;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }

		protected override void OnStartUp() {
		   goingUp = true;
		   highestLow = 0;
		   lowestHigh = 0;
		   pgoingUp = true;
		   phighestLow = 0;
		   plowestHigh = 0;
		   lastSeen = -1;
		}
		
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(lastSeen != CurrentBar) {
			    pgoingUp = goingUp;
				phighestLow = highestLow;
				plowestHigh = lowestHigh;
				lastSeen = CurrentBar;
			}
			
			if(pgoingUp) {
			  if(Low[0] > phighestLow) {
			    highestLow = Low[0];	
			  }
			
			  if( (High[0] < phighestLow) && 
			      (!trendBarReqd || (Close[0] < Median[0])) 
				) {
			     goingUp = false;
				 lowestHigh = High[0];
			  }
			} else {
			  if(High[0] < plowestHigh) {
			    lowestHigh = High[0];	
			  }	
			
			  if( (Low[0] > plowestHigh) &&
			      (!trendBarReqd || (Close[0] > Median[0])) 
				) {
			     goingUp = true;
				 highestLow = Low[0];
			  }
			}
			
		    BarColor = (goingUp?colorUP:colorDN);
        }

        #region Properties

        [Description("Need a trend bar to change directions?")]
        [GridCategory("Parameters")]
        public bool TrendBarReqd
        {
            get { return trendBarReqd; }
            set { trendBarReqd = value; }
        }
		
        [Description("Color")]
        [GridCategory("Visual")]
        public Color ColorUP
        {
            get { return colorUP; }
            set { colorUP = value; }
        }
		
        [Browsable(false)]
        public string colorUPSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorUP); }
           set { colorUP = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Color")]
        [GridCategory("Visual")]
        public Color ColorDN
        {
            get { return colorDN; }
            set { colorDN = value; }
        }
		
        [Browsable(false)]
        public string colorDNSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorDN); }
           set { colorDN = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private Z20100909ClearTrend[] cacheZ20100909ClearTrend = null;

        private static Z20100909ClearTrend checkZ20100909ClearTrend = new Z20100909ClearTrend();

        /// <summary>
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        public Z20100909ClearTrend Z20100909ClearTrend(Color colorDN, Color colorUP, bool trendBarReqd)
        {
            return Z20100909ClearTrend(Input, colorDN, colorUP, trendBarReqd);
        }

        /// <summary>
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        public Z20100909ClearTrend Z20100909ClearTrend(Data.IDataSeries input, Color colorDN, Color colorUP, bool trendBarReqd)
        {
            if (cacheZ20100909ClearTrend != null)
                for (int idx = 0; idx < cacheZ20100909ClearTrend.Length; idx++)
                    if (cacheZ20100909ClearTrend[idx].ColorDN == colorDN && cacheZ20100909ClearTrend[idx].ColorUP == colorUP && cacheZ20100909ClearTrend[idx].TrendBarReqd == trendBarReqd && cacheZ20100909ClearTrend[idx].EqualsInput(input))
                        return cacheZ20100909ClearTrend[idx];

            lock (checkZ20100909ClearTrend)
            {
                checkZ20100909ClearTrend.ColorDN = colorDN;
                colorDN = checkZ20100909ClearTrend.ColorDN;
                checkZ20100909ClearTrend.ColorUP = colorUP;
                colorUP = checkZ20100909ClearTrend.ColorUP;
                checkZ20100909ClearTrend.TrendBarReqd = trendBarReqd;
                trendBarReqd = checkZ20100909ClearTrend.TrendBarReqd;

                if (cacheZ20100909ClearTrend != null)
                    for (int idx = 0; idx < cacheZ20100909ClearTrend.Length; idx++)
                        if (cacheZ20100909ClearTrend[idx].ColorDN == colorDN && cacheZ20100909ClearTrend[idx].ColorUP == colorUP && cacheZ20100909ClearTrend[idx].TrendBarReqd == trendBarReqd && cacheZ20100909ClearTrend[idx].EqualsInput(input))
                            return cacheZ20100909ClearTrend[idx];

                Z20100909ClearTrend indicator = new Z20100909ClearTrend();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorDN = colorDN;
                indicator.ColorUP = colorUP;
                indicator.TrendBarReqd = trendBarReqd;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100909ClearTrend[] tmp = new Z20100909ClearTrend[cacheZ20100909ClearTrend == null ? 1 : cacheZ20100909ClearTrend.Length + 1];
                if (cacheZ20100909ClearTrend != null)
                    cacheZ20100909ClearTrend.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100909ClearTrend = tmp;
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
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100909ClearTrend Z20100909ClearTrend(Color colorDN, Color colorUP, bool trendBarReqd)
        {
            return _indicator.Z20100909ClearTrend(Input, colorDN, colorUP, trendBarReqd);
        }

        /// <summary>
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100909ClearTrend Z20100909ClearTrend(Data.IDataSeries input, Color colorDN, Color colorUP, bool trendBarReqd)
        {
            return _indicator.Z20100909ClearTrend(input, colorDN, colorUP, trendBarReqd);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100909ClearTrend Z20100909ClearTrend(Color colorDN, Color colorUP, bool trendBarReqd)
        {
            return _indicator.Z20100909ClearTrend(Input, colorDN, colorUP, trendBarReqd);
        }

        /// <summary>
        /// A paintbar based on the TAS&C Clear concept from September 2010 issue
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100909ClearTrend Z20100909ClearTrend(Data.IDataSeries input, Color colorDN, Color colorUP, bool trendBarReqd)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100909ClearTrend(input, colorDN, colorUP, trendBarReqd);
        }
    }
}
#endregion
