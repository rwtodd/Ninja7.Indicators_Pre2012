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

namespace RWT_HA {

	public enum PrimaryOHLC {
		BARS,     // just a pass through for the basic bars..
		INPUTS    // synthetic based on Input[0] and Input[1]
	}
	
	public enum SecondaryOHLC {
	  NONE,   // no smoothing
	  HEIKEN_ASHI, 
	  SMA,    // simple moving average
	  EMA,    // exponential moving average	
      WMA,     // weighted FIR 
	  HULL,   // HULL MA on WMAs
	  HULLEMA, // HULL MA on EMAs
	  MEDIANFILT,  // median filter
	  KAMA210,    // kaufman adaptive min 2 lookback 10
	  DEXPMA,    // Double EXP Smoothing (for linear trends)
	  TEMA,      // TEMA
	  DELAY      // n-bar delay
	}
	
	public interface OHLC {
		void update();
		double Open { get; }
		double Close { get; }
		double High { get; }
		double Low { get; }
	}
	
	public class OHLCBars : OHLC {
		private NinjaTrader.Data.IDataSeries opens, highs, lows, closes;
		private double lopen, lhigh, llow, lclose;
		
		public OHLCBars(NinjaTrader.Data.IDataSeries o,
			NinjaTrader.Data.IDataSeries h,
			NinjaTrader.Data.IDataSeries l,
			NinjaTrader.Data.IDataSeries c) {
			opens = o; highs = h; lows = l; closes = c;	
			update();
		}
		
	    public void update() {
		    lopen = opens[0];
			lhigh = highs[0];
			llow = lows[0];
			lclose = closes[0];
		}
		public double Open { get { return lopen; } }
		public double High { get { return lhigh; } }
		public double Low {  get { return llow; } }
		public double Close { get { return lclose; } }
		
	}

	public class InputBars : OHLC {
		private NinjaTrader.Data.IDataSeries input;
		private double opens, highs, lows, closes;
		public InputBars(NinjaTrader.Data.IDataSeries i)
		{
			input = i;	
			opens = input[0];
			closes = opens;
			highs = opens;
			lows = opens;
		}
		public void update() {
			opens = closes;
			closes = input[0];
			highs = Math.Max(opens,closes);
			lows = Math.Min(opens,closes);
		}
		public double Open { get { return opens; } }
		public double High { get { return highs; } }
		public double Low {  get { return lows; } }
		public double Close { get { return closes; } }
		
	}
	
	public class Heiken_Ashi : OHLC {
		private OHLC input;
		private double opens, highs, lows, closes;
		public Heiken_Ashi(OHLC parent) { 
			input = parent;
			opens = input.Open;
			highs = input.High;
			lows = input.Low;
			closes = (input.Open + input.High + input.Low + input.Close)*0.25;
		}
		public void update() {
		  	input.update();
			opens = (opens + closes)*0.5;
			closes = (input.Open + input.High + input.Low + input.Close)*0.25;
			highs = Math.Max(opens,input.High);
			lows = Math.Min(opens,input.Low);
		}
		public double Open { get { return opens; } }
		public double High { get { return highs; } }
		public double Low {  get { return lows; } }
		public double Close { get { return closes; } }

	}
	
	public class SmoothedBars : OHLC {
		private OHLC input;
		private RWT_MA.MovingAverage oma, hma, lma, cma;
		private double opens, highs, lows, closes;
		public SmoothedBars(OHLC parent, RWT_MA.MAType type, double len) {
			input = parent;
			oma = RWT_MA.MAFactory.create(type, len);
			hma = RWT_MA.MAFactory.create(type, len);
			lma = RWT_MA.MAFactory.create(type, len);
			cma = RWT_MA.MAFactory.create(type, len);
			oma.init(input.Open);
			hma.init(input.High);
			lma.init(input.Low);
			cma.init(input.Close);
			opens = input.Open;
			highs = input.High;
			lows = input.Low;
			closes = input.Close;
		}
		
		public void update() {
		   input.update();
		   opens = oma.next(input.Open);
		   highs = hma.next(input.High);
		   lows = lma.next(input.Low);
		   closes = cma.next(input.Close);
		}
		public double Open { get { return opens; } }
		public double High { get { return highs; } }
		public double Low {  get { return lows; } }
		public double Close { get { return closes; } }
	}
	
	public class OHLCFactory {
	
		public static OHLC createPrimary(PrimaryOHLC type, 
			NinjaTrader.Data.IDataSeries o,
			NinjaTrader.Data.IDataSeries h,
			NinjaTrader.Data.IDataSeries l,
			NinjaTrader.Data.IDataSeries c,
			NinjaTrader.Data.IDataSeries inp) {
		
			OHLC ans = null;
			switch(type) {
				case PrimaryOHLC.BARS:
					ans = new OHLCBars(o,h,l,c);
					break;
				case PrimaryOHLC.INPUTS:
					ans = new InputBars(inp);
					break;
			}
			return ans;
		}
			
		public static OHLC createSecondary(OHLC parent,
				SecondaryOHLC type, double arg) {
		
			OHLC ans = null;
			switch(type) {
				case SecondaryOHLC.NONE:
					ans = parent;
					break;
				case SecondaryOHLC.HEIKEN_ASHI:
					ans = new Heiken_Ashi(parent);
					break;
				case SecondaryOHLC.DEXPMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.DEXPMA, arg);
					break;
				case SecondaryOHLC.EMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.EMA, arg);
					break;
				case SecondaryOHLC.HULL:
					ans = new SmoothedBars(parent, RWT_MA.MAType.HULL, arg);
					break;
				case SecondaryOHLC.HULLEMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.HULLEMA, arg);
					break;
				case SecondaryOHLC.KAMA210:
					ans = new SmoothedBars(parent, RWT_MA.MAType.KAMA210, arg);
					break;
				case SecondaryOHLC.SMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.SMA, arg);
					break;
				case SecondaryOHLC.WMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.WMA, arg);
					break;
				case SecondaryOHLC.MEDIANFILT:
					ans = new SmoothedBars(parent, RWT_MA.MAType.MEDIANFILT, arg);
					break;
				case SecondaryOHLC.TEMA:
					ans = new SmoothedBars(parent, RWT_MA.MAType.TEMA, arg);
					break;
				case SecondaryOHLC.DELAY:
					ans = new SmoothedBars(parent, RWT_MA.MAType.DELAY, arg);
					break;
			}
			return ans;
		}
	}
	
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Heiken Ashi Smoothed view
    /// </summary>
    [Description("Heiken Ashi Smoothed view")]
    public class zHASmoothed : Indicator
    {
        #region Variables
        // Wizard generated variables
		    private RWT_HA.OHLC habars;
		
			private RWT_HA.PrimaryOHLC primType;
			private RWT_HA.SecondaryOHLC smoothType;
			private double smoothArg = 40;
		
			private Color upcolor = Color.Green;
		    private Color dncolor = Color.Red;
		    private Color ltup = Color.DodgerBlue;
		    private Color ltdn = Color.Magenta;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "HAHigh"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "HALow"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Cross, "HAMid"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			var prim = RWT_HA.OHLCFactory.createPrimary(primType,Open,High,Low,Close,Input);
			var sm   = RWT_HA.OHLCFactory.createSecondary(prim,smoothType,smoothArg);
			habars = RWT_HA.OHLCFactory.createSecondary(sm,RWT_HA.SecondaryOHLC.HEIKEN_ASHI,0.0);
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			habars.update();
			HAMid.Set((habars.High + habars.Low) * 0.5);
			HAHigh.Set(habars.High);
			HALow.Set(habars.Low);
			Color col;
			if(habars.Open < habars.Close) {
			  col = ( (habars.Low < habars.Open)?ltup:upcolor );
			} else {
			  col = ( (habars.High > habars.Open)?ltdn:dncolor );  					
			}
			PlotColors[0][0] = col;
			PlotColors[1][0] = col;
			PlotColors[2][0] = col;
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HAHigh
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HALow
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HAMid
        {
            get { return Values[2]; }
        }

        [Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public double SmoothArg
        {
            get { return smoothArg; }
            set { smoothArg = value; }
        }

		[Description("Smoothing constant")]
        [GridCategory("Parameters")]
        public RWT_HA.SecondaryOHLC SmoothType
        {
            get { return smoothType; }
            set { smoothType = value; }
        }

		[Description("Input Type")]
        [GridCategory("Parameters")]
        public RWT_HA.PrimaryOHLC PrimaryType
        {
            get { return primType; }
            set { primType = value; }
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
        private zHASmoothed[] cachezHASmoothed = null;

        private static zHASmoothed checkzHASmoothed = new zHASmoothed();

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public zHASmoothed zHASmoothed(RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            return zHASmoothed(Input, primaryType, smoothArg, smoothType);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public zHASmoothed zHASmoothed(Data.IDataSeries input, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            if (cachezHASmoothed != null)
                for (int idx = 0; idx < cachezHASmoothed.Length; idx++)
                    if (cachezHASmoothed[idx].PrimaryType == primaryType && Math.Abs(cachezHASmoothed[idx].SmoothArg - smoothArg) <= double.Epsilon && cachezHASmoothed[idx].SmoothType == smoothType && cachezHASmoothed[idx].EqualsInput(input))
                        return cachezHASmoothed[idx];

            lock (checkzHASmoothed)
            {
                checkzHASmoothed.PrimaryType = primaryType;
                primaryType = checkzHASmoothed.PrimaryType;
                checkzHASmoothed.SmoothArg = smoothArg;
                smoothArg = checkzHASmoothed.SmoothArg;
                checkzHASmoothed.SmoothType = smoothType;
                smoothType = checkzHASmoothed.SmoothType;

                if (cachezHASmoothed != null)
                    for (int idx = 0; idx < cachezHASmoothed.Length; idx++)
                        if (cachezHASmoothed[idx].PrimaryType == primaryType && Math.Abs(cachezHASmoothed[idx].SmoothArg - smoothArg) <= double.Epsilon && cachezHASmoothed[idx].SmoothType == smoothType && cachezHASmoothed[idx].EqualsInput(input))
                            return cachezHASmoothed[idx];

                zHASmoothed indicator = new zHASmoothed();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PrimaryType = primaryType;
                indicator.SmoothArg = smoothArg;
                indicator.SmoothType = smoothType;
                Indicators.Add(indicator);
                indicator.SetUp();

                zHASmoothed[] tmp = new zHASmoothed[cachezHASmoothed == null ? 1 : cachezHASmoothed.Length + 1];
                if (cachezHASmoothed != null)
                    cachezHASmoothed.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezHASmoothed = tmp;
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
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHASmoothed zHASmoothed(RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            return _indicator.zHASmoothed(Input, primaryType, smoothArg, smoothType);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public Indicator.zHASmoothed zHASmoothed(Data.IDataSeries input, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            return _indicator.zHASmoothed(input, primaryType, smoothArg, smoothType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHASmoothed zHASmoothed(RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            return _indicator.zHASmoothed(Input, primaryType, smoothArg, smoothType);
        }

        /// <summary>
        /// Heiken Ashi Smoothed view
        /// </summary>
        /// <returns></returns>
        public Indicator.zHASmoothed zHASmoothed(Data.IDataSeries input, RWT_HA.PrimaryOHLC primaryType, double smoothArg, RWT_HA.SecondaryOHLC smoothType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zHASmoothed(input, primaryType, smoothArg, smoothType);
        }
    }
}
#endregion
