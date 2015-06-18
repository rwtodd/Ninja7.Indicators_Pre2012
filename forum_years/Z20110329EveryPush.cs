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
    /// everypush
    /// </summary>
    [Description("everypush")]
    public class Z20110329EveryPush : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        // Wizard generated variables
            private int lookBack = 2; // Default setting for LookBack
            private Z20100527FastMAX maxlb;
		    private Z20100527FastMIN minlb;
            private Z20100527FastMAX maxlen;
		    private Z20100527FastMIN minlen;
		    private ATR myatr;
		    private int length = 11;
			private Color upClr = Color.Green;
		    private Color dnClr = Color.Red;
		    private double lastMove;
		    private bool plotBars = true;
		    private bool plotHistogram = false;
		    private int mctrend;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Bar, "EP"));
            Overlay				= false;
			mctrend = 0;
        }

		protected override void OnStartUp() {
			maxlb = Z20100527FastMAX(High,lookBack);
			minlb = Z20100527FastMIN(Low,lookBack);
			maxlen = Z20100527FastMAX(High,length);
			minlen = Z20100527FastMIN(Low,length);
			myatr = ATR(length);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < length) { Value.Set(0);  return; }
			
			double upmoves = 0;
			double dnmoves = 0;
			for(int indx = (length - 1); indx >= 0; --indx) {
			  double tmp = High[indx] - maxlb[indx+1];
			  double tmp2 = minlb[indx+1] - Low[indx];
			  if(tmp > 0) {
			      upmoves += tmp;
				  lastMove = 1;
			  }
			  if(tmp2 > 0) {
			     dnmoves += tmp2;
				 lastMove = -1;
			  }
			  // if we had "both" moves, pick based on the close..
			  if((tmp > 0) && (tmp2 > 0)) {
			   	if(Close[indx] > Open[indx]) lastMove = 1;
			  }
			
			}
			
			double curPush = lastMove * ((maxlen[0]-minlen[0])/(myatr[0]+upmoves+dnmoves));
			double alpha = 2.0 / (length + 1.0);
  			Value.Set( Value[1] + alpha*(curPush - Value[1]));
			
			if((Value[0] > 0) && (Value[1] < Value[0])) 
				PlotColors[0][0] = upClr;
			if((Value[0] < 0) && (Value[1] > Value[0])) 
				PlotColors[0][0] = dnClr;			
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries EP
        {
            get { return Values[0]; }
        }

		
        [Description("How far to look back")]
        [GridCategory("Parameters")]
        public int LookBack
        {
            get { return lookBack; }
            set { lookBack = Math.Max(2, value); }
        }
        [Description("Length to Consider")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(6, value); }
        }
        [Description("up bias color")]
        [GridCategory("Parameters")]
        public Color ClrUP	
        {
            get { return upClr; }
            set { upClr = value; }
        }
        [Browsable(false)]
        public string upClrSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upClr); }
           set { upClr = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }		
        [Description("dn bias color")]
        [GridCategory("Parameters")]
        public Color ClrDN	
        {
            get { return dnClr; }
            set { dnClr = value; }
        }
        [Browsable(false)]
        public string dnClrSerialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dnClr); }
           set { dnClr = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private Z20110329EveryPush[] cacheZ20110329EveryPush = null;

        private static Z20110329EveryPush checkZ20110329EveryPush = new Z20110329EveryPush();

        /// <summary>
        /// everypush
        /// </summary>
        /// <returns></returns>
        public Z20110329EveryPush Z20110329EveryPush(Color clrDN, Color clrUP, int length, int lookBack)
        {
            return Z20110329EveryPush(Input, clrDN, clrUP, length, lookBack);
        }

        /// <summary>
        /// everypush
        /// </summary>
        /// <returns></returns>
        public Z20110329EveryPush Z20110329EveryPush(Data.IDataSeries input, Color clrDN, Color clrUP, int length, int lookBack)
        {
            if (cacheZ20110329EveryPush != null)
                for (int idx = 0; idx < cacheZ20110329EveryPush.Length; idx++)
                    if (cacheZ20110329EveryPush[idx].ClrDN == clrDN && cacheZ20110329EveryPush[idx].ClrUP == clrUP && cacheZ20110329EveryPush[idx].Length == length && cacheZ20110329EveryPush[idx].LookBack == lookBack && cacheZ20110329EveryPush[idx].EqualsInput(input))
                        return cacheZ20110329EveryPush[idx];

            lock (checkZ20110329EveryPush)
            {
                checkZ20110329EveryPush.ClrDN = clrDN;
                clrDN = checkZ20110329EveryPush.ClrDN;
                checkZ20110329EveryPush.ClrUP = clrUP;
                clrUP = checkZ20110329EveryPush.ClrUP;
                checkZ20110329EveryPush.Length = length;
                length = checkZ20110329EveryPush.Length;
                checkZ20110329EveryPush.LookBack = lookBack;
                lookBack = checkZ20110329EveryPush.LookBack;

                if (cacheZ20110329EveryPush != null)
                    for (int idx = 0; idx < cacheZ20110329EveryPush.Length; idx++)
                        if (cacheZ20110329EveryPush[idx].ClrDN == clrDN && cacheZ20110329EveryPush[idx].ClrUP == clrUP && cacheZ20110329EveryPush[idx].Length == length && cacheZ20110329EveryPush[idx].LookBack == lookBack && cacheZ20110329EveryPush[idx].EqualsInput(input))
                            return cacheZ20110329EveryPush[idx];

                Z20110329EveryPush indicator = new Z20110329EveryPush();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ClrDN = clrDN;
                indicator.ClrUP = clrUP;
                indicator.Length = length;
                indicator.LookBack = lookBack;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20110329EveryPush[] tmp = new Z20110329EveryPush[cacheZ20110329EveryPush == null ? 1 : cacheZ20110329EveryPush.Length + 1];
                if (cacheZ20110329EveryPush != null)
                    cacheZ20110329EveryPush.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20110329EveryPush = tmp;
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
        /// everypush
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20110329EveryPush Z20110329EveryPush(Color clrDN, Color clrUP, int length, int lookBack)
        {
            return _indicator.Z20110329EveryPush(Input, clrDN, clrUP, length, lookBack);
        }

        /// <summary>
        /// everypush
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20110329EveryPush Z20110329EveryPush(Data.IDataSeries input, Color clrDN, Color clrUP, int length, int lookBack)
        {
            return _indicator.Z20110329EveryPush(input, clrDN, clrUP, length, lookBack);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// everypush
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20110329EveryPush Z20110329EveryPush(Color clrDN, Color clrUP, int length, int lookBack)
        {
            return _indicator.Z20110329EveryPush(Input, clrDN, clrUP, length, lookBack);
        }

        /// <summary>
        /// everypush
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20110329EveryPush Z20110329EveryPush(Data.IDataSeries input, Color clrDN, Color clrUP, int length, int lookBack)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20110329EveryPush(input, clrDN, clrUP, length, lookBack);
        }
    }
}
#endregion
