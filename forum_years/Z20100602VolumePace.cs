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
    /// Pace of volume
    /// </summary>
    [Description("Pace of volume")]
    public class Z20100602VolumePace : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int len1 = 8; // Default setting for Len1
            private int len2 = 13; // Default setting for Len2
            private int barsBack = 2; // Default setting for BarsBack
        // User defined variables (add any user defined variables below)
		    private double ema1, ema2;
		    private double alpha1, alpha2;
		    private double volcount;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Bar, "Pace"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            Overlay				= false;
        }
		
		protected override void OnStartUp() {	
			alpha1 = 2.0/(1.0+len1);
			alpha2 = 2.0/(1.0+len2);
			ema1 = 0;
			ema2 = 0;
			volcount = 0;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
		    // calculate the pace...
			if(CurrentBar < barsBack) return;
			
			if(CurrentBar == barsBack) {
			  volcount = 0;
			  for(int i= 0; i< barsBack; ++i) {
			    volcount += Volume[i];	
			  } 
			  return;
			}
			
			volcount -= Volume[barsBack];
			volcount += Volume[0];
			double ts = (Time[0].Subtract(Time[barsBack]).TotalMilliseconds)/1000.0;
            double pace = volcount*volcount/Math.Max(0.1,ts);
			ema1 = ema1 + alpha1*(pace - ema1);
			if(ema1 < 0.1) ema1 = 0.1;
			ema2 = ema2 + alpha2*(ema1 - ema2);
			if(ema2 < 0.1) ema2 = 0.1;		
			
            Value.Set(Math.Log(ema1/ema2));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Pace
        {
            get { return Values[0]; }
        }

        [Description("avg len")]
        [GridCategory("Parameters")]
        public int Len1
        {
            get { return len1; }
            set { len1 = Math.Max(1, value); }
        }

        [Description("avg len")]
        [GridCategory("Parameters")]
        public int Len2
        {
            get { return len2; }
            set { len2 = Math.Max(1, value); }
        }

        [Description("how many bars of overlap (1-4 is typical)")]
        [GridCategory("Parameters")]
        public int BarsBack
        {
            get { return barsBack; }
            set { barsBack = Math.Max(1, value); }
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
        private Z20100602VolumePace[] cacheZ20100602VolumePace = null;

        private static Z20100602VolumePace checkZ20100602VolumePace = new Z20100602VolumePace();

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Z20100602VolumePace Z20100602VolumePace(int barsBack, int len1, int len2)
        {
            return Z20100602VolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Z20100602VolumePace Z20100602VolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            if (cacheZ20100602VolumePace != null)
                for (int idx = 0; idx < cacheZ20100602VolumePace.Length; idx++)
                    if (cacheZ20100602VolumePace[idx].BarsBack == barsBack && cacheZ20100602VolumePace[idx].Len1 == len1 && cacheZ20100602VolumePace[idx].Len2 == len2 && cacheZ20100602VolumePace[idx].EqualsInput(input))
                        return cacheZ20100602VolumePace[idx];

            lock (checkZ20100602VolumePace)
            {
                checkZ20100602VolumePace.BarsBack = barsBack;
                barsBack = checkZ20100602VolumePace.BarsBack;
                checkZ20100602VolumePace.Len1 = len1;
                len1 = checkZ20100602VolumePace.Len1;
                checkZ20100602VolumePace.Len2 = len2;
                len2 = checkZ20100602VolumePace.Len2;

                if (cacheZ20100602VolumePace != null)
                    for (int idx = 0; idx < cacheZ20100602VolumePace.Length; idx++)
                        if (cacheZ20100602VolumePace[idx].BarsBack == barsBack && cacheZ20100602VolumePace[idx].Len1 == len1 && cacheZ20100602VolumePace[idx].Len2 == len2 && cacheZ20100602VolumePace[idx].EqualsInput(input))
                            return cacheZ20100602VolumePace[idx];

                Z20100602VolumePace indicator = new Z20100602VolumePace();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarsBack = barsBack;
                indicator.Len1 = len1;
                indicator.Len2 = len2;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20100602VolumePace[] tmp = new Z20100602VolumePace[cacheZ20100602VolumePace == null ? 1 : cacheZ20100602VolumePace.Length + 1];
                if (cacheZ20100602VolumePace != null)
                    cacheZ20100602VolumePace.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20100602VolumePace = tmp;
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
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100602VolumePace Z20100602VolumePace(int barsBack, int len1, int len2)
        {
            return _indicator.Z20100602VolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100602VolumePace Z20100602VolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            return _indicator.Z20100602VolumePace(input, barsBack, len1, len2);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20100602VolumePace Z20100602VolumePace(int barsBack, int len1, int len2)
        {
            return _indicator.Z20100602VolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20100602VolumePace Z20100602VolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20100602VolumePace(input, barsBack, len1, len2);
        }
    }
}
#endregion
