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
    /// better adx
    /// </summary>
    [Description("better adx")]
    public class ZADX : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int dMLength = 40;
			private RWT_MA.MAType dMType = RWT_MA.MAType.HULLEMA;
			private int aDXLength = 10;
			private RWT_MA.MAType aDXType = RWT_MA.MAType.HULLEMA;
		    private RWT_MA.MovingAverage truerange, dplus, dminus, adx;

			private RWT_HA.PrimaryOHLC barsType = RWT_HA.PrimaryOHLC.BARS;
		    private RWT_HA.SecondaryOHLC barSmoothType = RWT_HA.SecondaryOHLC.NONE;
		 	private double barSmoothArg = 1;
			private RWT_HA.OHLC bars;
			private double oldADX;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "ADX"));
            Add(new Plot(Color.FromKnownColor(KnownColor.SteelBlue), PlotStyle.Line, "DMPlus"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "DMMinus"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 25, "TrendMark"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			var b1 = RWT_HA.OHLCFactory.createPrimary(barsType,Open,High,Low,Close,Input);
			bars = RWT_HA.OHLCFactory.createSecondary(b1,barSmoothType,barSmoothArg);
			
			truerange = RWT_MA.MAFactory.create(dMType,dMLength);
			dplus     = RWT_MA.MAFactory.create(dMType,dMLength);
			dminus    = RWT_MA.MAFactory.create(dMType,dMLength);
			adx = RWT_MA.MAFactory.create(aDXType,aDXLength); 
			truerange.init(bars.High - bars.Low);
			dplus.init(0);
			dminus.init(0);
			adx.init(.5);
			ADX.Set(0);
			oldADX = 0;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var oldHigh = bars.High;
			var oldClose = bars.Close;
			var oldLow = bars.Low;
			bars.update();
			
			// true range...
			var trval = truerange.next( Math.Max(bars.High-bars.Low, 
			                              Math.Max(bars.High - oldClose, oldClose - bars.Low)) );
			if(trval == 0.0) trval = 0.000001;
			
			// RWT try upmove/dnmove = Median[0] - Median[1];
			// instead of High - Close[1] , Close[1] - Low[0];
			var upmove = Math.Max(0,bars.High - oldHigh);
			var dnmove = Math.Max(0,oldLow - bars.Low);
			if(upmove > dnmove) {
				dnmove = 0;
			} else {
			    upmove = 0;	
			}
			
			var dplusval = dplus.next(upmove);
			var dminval = dminus.next(dnmove);
			
			var dmiplus = 100.0 * dplusval / trval;
			var dmiminus = 100.0 * dminval / trval;
			
			var dmisum = dmiplus + dmiminus;
			var dmidiff = Math.Abs(dmiplus - dmiminus);
			var adxval = 100.0 * adx.next( (dmisum > 0)?(dmidiff/dmisum):0.5 );
			
            ADX.Set(adxval);
			if(adxval < oldADX) PlotColors[0][0] = Color.Gray;
            DMPlus.Set(dmiplus);
            DMMinus.Set(dmiminus);
			oldADX = adxval;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ADX
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DMPlus
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DMMinus
        {
            get { return Values[2]; }
        }
		
		[Description("bars argument")]
        [GridCategory("Parameters")]
        public double BarSmoothArg
        {
            get { return barSmoothArg; }
            set { barSmoothArg = value; }
        }

        [Description("bars")]
        [GridCategory("Parameters")]
        public RWT_HA.SecondaryOHLC BarSmoothType
		{
			get { return barSmoothType; } set { barSmoothType = value; }	
		}
        [Description("bars")]
        [GridCategory("Parameters")]
        public RWT_HA.PrimaryOHLC BarsType
		{
			get { return barsType; } set { barsType = value; }	
		}
        [Description("smoothing")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType ADXType
		{
			get { return aDXType; } set { aDXType = value; }	
		}
        [Description("smoothing")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType DMType
		{
			get { return dMType; } set { dMType = value; }	
		}
		
		[Description("smoothing constant")]
        [GridCategory("Parameters")]
        public int ADXLength
        {
            get { return aDXLength; }
            set { aDXLength = value; }
        }

        [Description("smoothing constant")]
        [GridCategory("Parameters")]
        public int DMLength
        {
            get { return dMLength; }
            set { dMLength = value; }
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
        private ZADX[] cacheZADX = null;

        private static ZADX checkZADX = new ZADX();

        /// <summary>
        /// better adx
        /// </summary>
        /// <returns></returns>
        public ZADX ZADX(int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            return ZADX(Input, aDXLength, aDXType, barSmoothArg, barSmoothType, barsType, dMLength, dMType);
        }

        /// <summary>
        /// better adx
        /// </summary>
        /// <returns></returns>
        public ZADX ZADX(Data.IDataSeries input, int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            if (cacheZADX != null)
                for (int idx = 0; idx < cacheZADX.Length; idx++)
                    if (cacheZADX[idx].ADXLength == aDXLength && cacheZADX[idx].ADXType == aDXType && Math.Abs(cacheZADX[idx].BarSmoothArg - barSmoothArg) <= double.Epsilon && cacheZADX[idx].BarSmoothType == barSmoothType && cacheZADX[idx].BarsType == barsType && cacheZADX[idx].DMLength == dMLength && cacheZADX[idx].DMType == dMType && cacheZADX[idx].EqualsInput(input))
                        return cacheZADX[idx];

            lock (checkZADX)
            {
                checkZADX.ADXLength = aDXLength;
                aDXLength = checkZADX.ADXLength;
                checkZADX.ADXType = aDXType;
                aDXType = checkZADX.ADXType;
                checkZADX.BarSmoothArg = barSmoothArg;
                barSmoothArg = checkZADX.BarSmoothArg;
                checkZADX.BarSmoothType = barSmoothType;
                barSmoothType = checkZADX.BarSmoothType;
                checkZADX.BarsType = barsType;
                barsType = checkZADX.BarsType;
                checkZADX.DMLength = dMLength;
                dMLength = checkZADX.DMLength;
                checkZADX.DMType = dMType;
                dMType = checkZADX.DMType;

                if (cacheZADX != null)
                    for (int idx = 0; idx < cacheZADX.Length; idx++)
                        if (cacheZADX[idx].ADXLength == aDXLength && cacheZADX[idx].ADXType == aDXType && Math.Abs(cacheZADX[idx].BarSmoothArg - barSmoothArg) <= double.Epsilon && cacheZADX[idx].BarSmoothType == barSmoothType && cacheZADX[idx].BarsType == barsType && cacheZADX[idx].DMLength == dMLength && cacheZADX[idx].DMType == dMType && cacheZADX[idx].EqualsInput(input))
                            return cacheZADX[idx];

                ZADX indicator = new ZADX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXLength = aDXLength;
                indicator.ADXType = aDXType;
                indicator.BarSmoothArg = barSmoothArg;
                indicator.BarSmoothType = barSmoothType;
                indicator.BarsType = barsType;
                indicator.DMLength = dMLength;
                indicator.DMType = dMType;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZADX[] tmp = new ZADX[cacheZADX == null ? 1 : cacheZADX.Length + 1];
                if (cacheZADX != null)
                    cacheZADX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZADX = tmp;
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
        /// better adx
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZADX ZADX(int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            return _indicator.ZADX(Input, aDXLength, aDXType, barSmoothArg, barSmoothType, barsType, dMLength, dMType);
        }

        /// <summary>
        /// better adx
        /// </summary>
        /// <returns></returns>
        public Indicator.ZADX ZADX(Data.IDataSeries input, int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            return _indicator.ZADX(input, aDXLength, aDXType, barSmoothArg, barSmoothType, barsType, dMLength, dMType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// better adx
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZADX ZADX(int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            return _indicator.ZADX(Input, aDXLength, aDXType, barSmoothArg, barSmoothType, barsType, dMLength, dMType);
        }

        /// <summary>
        /// better adx
        /// </summary>
        /// <returns></returns>
        public Indicator.ZADX ZADX(Data.IDataSeries input, int aDXLength, RWT_MA.MAType aDXType, double barSmoothArg, RWT_HA.SecondaryOHLC barSmoothType, RWT_HA.PrimaryOHLC barsType, int dMLength, RWT_MA.MAType dMType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZADX(input, aDXLength, aDXType, barSmoothArg, barSmoothType, barsType, dMLength, dMType);
        }
    }
}
#endregion
