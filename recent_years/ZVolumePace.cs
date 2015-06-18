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
    public class ZVolumePace : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int len1 = 5; // Default setting for Len1
            private int len2 = 5; // Default setting for Len2
            private int barsBack = 2; // Default setting for BarsBack
        // User defined variables (add any user defined variables below)
		    private double ema1, ema2;
		    private double alpha1, alpha2;
		    private double volcount;
		    private Color buycolor = Color.SteelBlue;
		    private Color sellcolor = Color.Firebrick;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Yellow), PlotStyle.Bar, "Pace"));
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
			if(CurrentBar < barsBack) {
			    Value.Set(0);
				return;
			}
			if(CurrentBar == barsBack) {
			  volcount = 0;
			  for(int i= 0; i< barsBack; ++i) {
			    volcount += Volume[i];	
			  } 
			  Value.Set(0);
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
			if(Value[0] > Value[1]) {
				PlotColors[0][0] = buycolor;
			} else if(Value[0] < Value[1]) {
				PlotColors[0][0] = sellcolor;	
			}
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
		

        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Buy Color")]
        public Color BuyColor
        {
            get { return buycolor; }
            set { buycolor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BuyColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(buycolor); }
            set { buycolor = Gui.Design.SerializableColor.FromString(value); }
        }
        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Sell Color")]
        public Color SellColor
        {
            get { return sellcolor; }
            set { sellcolor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string SellColorSerialize
 	   {
            get { return Gui.Design.SerializableColor.ToString(sellcolor); }
            set { sellcolor = Gui.Design.SerializableColor.FromString(value); }
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
        private ZVolumePace[] cacheZVolumePace = null;

        private static ZVolumePace checkZVolumePace = new ZVolumePace();

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public ZVolumePace ZVolumePace(int barsBack, int len1, int len2)
        {
            return ZVolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public ZVolumePace ZVolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            if (cacheZVolumePace != null)
                for (int idx = 0; idx < cacheZVolumePace.Length; idx++)
                    if (cacheZVolumePace[idx].BarsBack == barsBack && cacheZVolumePace[idx].Len1 == len1 && cacheZVolumePace[idx].Len2 == len2 && cacheZVolumePace[idx].EqualsInput(input))
                        return cacheZVolumePace[idx];

            lock (checkZVolumePace)
            {
                checkZVolumePace.BarsBack = barsBack;
                barsBack = checkZVolumePace.BarsBack;
                checkZVolumePace.Len1 = len1;
                len1 = checkZVolumePace.Len1;
                checkZVolumePace.Len2 = len2;
                len2 = checkZVolumePace.Len2;

                if (cacheZVolumePace != null)
                    for (int idx = 0; idx < cacheZVolumePace.Length; idx++)
                        if (cacheZVolumePace[idx].BarsBack == barsBack && cacheZVolumePace[idx].Len1 == len1 && cacheZVolumePace[idx].Len2 == len2 && cacheZVolumePace[idx].EqualsInput(input))
                            return cacheZVolumePace[idx];

                ZVolumePace indicator = new ZVolumePace();
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

                ZVolumePace[] tmp = new ZVolumePace[cacheZVolumePace == null ? 1 : cacheZVolumePace.Length + 1];
                if (cacheZVolumePace != null)
                    cacheZVolumePace.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZVolumePace = tmp;
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
        public Indicator.ZVolumePace ZVolumePace(int barsBack, int len1, int len2)
        {
            return _indicator.ZVolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.ZVolumePace ZVolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            return _indicator.ZVolumePace(input, barsBack, len1, len2);
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
        public Indicator.ZVolumePace ZVolumePace(int barsBack, int len1, int len2)
        {
            return _indicator.ZVolumePace(Input, barsBack, len1, len2);
        }

        /// <summary>
        /// Pace of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.ZVolumePace ZVolumePace(Data.IDataSeries input, int barsBack, int len1, int len2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZVolumePace(input, barsBack, len1, len2);
        }
    }
}
#endregion
