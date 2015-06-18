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

namespace RWT_Bands {
  public enum BandType {
      STDDEV, // Standard Deviation bands
	  STDASYMM, // Asymmetric Std Deviation
	  RUNERR,   // Running error as we go...
	  KELTNER, // keltner bands...
  }

  public interface Band {
      void init(double price);
	  void update(double price, double avg);
	  double UpperBand { get; }
	  double LowerBand { get; }
  }

  public class KeltnerBand : Band {
	  private RWT_MA.MovingAverage highlow;	
	  private int length;
	  private double multiplier;
	  private RWT_HA.OHLC bars;
	  private double upBand, lwBand;

	  public KeltnerBand(RWT_HA.OHLC b, int len, double mult) {
		  bars = b;
		  length = len;
		  multiplier = mult;
	  } 
	  public void init(double price) {
		  highlow = new RWT_MA.EMA(length);
		  highlow.init(bars.High - bars.Low);
	  }
	  public void update(double price, double avg) {
		 var hl = highlow.next(bars.High - bars.Low); 
		 upBand = avg + multiplier*hl;
		 lwBand = avg - multiplier*hl;
	  }
	
	   public double UpperBand { get { return upBand; } }
	   public double LowerBand { get { return lwBand; } }

  }

  public class StdDevBand : Band {
	   private double[] prices;
	   private int length;
	   private double numDevs;
	   private int curIdx;
	   private double upBand, lwBand;
	   public StdDevBand(int len, double nDv) {
			length = len;
		    numDevs = nDv;
	   }	
	
	   public void init(double price) {
	       prices = new double[length];
		   for(int i = 0; i < length; ++i) {
				prices[i] = price;
	  		}
		   curIdx = 0;
	   }
	
	   public void update(double price, double avg) {
		   var devSum = 0.0;
		   prices[curIdx] = price;
		   if(++curIdx >= length) curIdx = 0;
		
		   for(int i = 0; i < length; ++i) {
			  var newVal = prices[i] - avg;
			  devSum += newVal*newVal;
		   } 
		
		   var stdDev = numDevs * Math.Sqrt(devSum / length);
		   upBand = avg + stdDev;
		   lwBand = avg - stdDev;
	   }
	
	   public double UpperBand { get { return upBand; } }
	   public double LowerBand { get { return lwBand; } }
  }

  public class RunnErrorBand : Band {
	   private double[] devs;
	   private int length;
	   private double numDevs;
	   private int curIdx;
	   private double upBand, lwBand;
	   private double devSum;
	   public RunnErrorBand(int len, double nDv) {
			length = len;
		    numDevs = nDv;
	   }	
	
	   public void init(double price) {
	       devs = new double[length];
		   curIdx = 0;
		   devSum = 0;		   
	   }
	
	   public void update(double price, double avg) {
		   var newVal = price - avg;
		   newVal = newVal*newVal;
		   devSum = devSum - devs[curIdx] + newVal;
		   devs[curIdx] = newVal;
		   if(++curIdx >= length) curIdx = 0;
				
		   var stdDev = numDevs * Math.Sqrt(devSum / length);
		   upBand = avg + stdDev;
		   lwBand = avg - stdDev;
	   }
	
	   public double UpperBand { get { return upBand; } }
	   public double LowerBand { get { return lwBand; } }
  }

  public class StdDevAsymmBand : Band {
	   private double[] prices;
	   private int length;
	   private double numDevs;
	   private int curIdx;
	   private double upBand, lwBand;
	   public StdDevAsymmBand(int len, double nDv) {
			length = len;
		    numDevs = nDv;
	   }	
	
	   public void init(double price) {
	       prices = new double[length];
		   for(int i = 0; i < length; ++i) {
				prices[i] = price;
	  		}
		   curIdx = 0;
	   }
	
	   public void update(double price, double avg) {
		   var devSumUp = 0.0;
		   int uplen = 0;
		   var devSumDn = 0.0;
		   int dnlen = 0;
		   prices[curIdx] = price;
		   if(++curIdx >= length) curIdx = 0;
		
		   for(int i = 0; i < length; ++i) {
			  var newVal = prices[i] - avg;
			  if(newVal > 0) {
			     devSumUp += newVal*newVal;  ++uplen;
			  } else {
				 devSumDn += newVal*newVal;  ++dnlen;
			  }
		   } 
		   if(uplen == 0) uplen = 1;
		   if(dnlen == 0) dnlen = 1;
		   upBand = avg + numDevs * Math.Sqrt(devSumUp / uplen);
		   lwBand = avg - numDevs * Math.Sqrt(devSumDn / dnlen);
	   }
	
	   public double UpperBand { get { return upBand; } }
	   public double LowerBand { get { return lwBand; } }
  }

   public class BandFactory {
		public static Band create(BandType type, RWT_HA.OHLC bars, int len, double arg) {
			Band ans = null;
			switch(type) {
				case BandType.STDDEV:
					ans = new StdDevBand(len,arg);
					break;
				case BandType.STDASYMM:
					ans = new StdDevAsymmBand(len,arg);
					break;
				case BandType.RUNERR:
					ans = new RunnErrorBand(len,arg);
					break;
				case BandType.KELTNER:
					ans = new KeltnerBand(bars,len,arg);
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
    /// a band around an MA
    /// </summary>
    [Description("a band around an MA")]
    public class zMovingAverageBand : Indicator
    {
        #region Variables
        // Wizard generated variables
			private RWT_HA.PrimaryOHLC inputType = RWT_HA.PrimaryOHLC.BARS;
		
            private int smoothArg = 20; // Default setting for SmoothArg
			private RWT_MA.MAType smoothType = RWT_MA.MAType.SMA;
		    private RWT_Bands.BandType bandType = RWT_Bands.BandType.STDDEV;
		
			private RWT_MA.MAType bandsmoothType = RWT_MA.MAType.HULLEMA;
			private int bandsmoothArg = 4;
		
            private double bandArg = 2.000; // Default setting for BandArg
		    private int lengthOfBand = 20; // length of the band calculation..
        // User defined variables (add any user defined variables below)
			private RWT_Bands.Band band;
			private RWT_MA.MovingAverage smoother;
			private RWT_MA.MovingAverage bandSmoother1, bandSmoother2;
			private RWT_HA.OHLC bars;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkOrange), PlotStyle.Line, "BandHigh"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkOrange), PlotStyle.Line, "BandLow"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkOrange), PlotStyle.Line, "MidLine"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			bars = RWT_HA.OHLCFactory.createPrimary(inputType,Open,High,Low,Close,Input);
			
			smoother = RWT_MA.MAFactory.create(smoothType,smoothArg);
			smoother.init(Input[0]);
			band = RWT_Bands.BandFactory.create(bandType,bars,lengthOfBand,bandArg);	
			band.init(Input[0]);
			
			bandSmoother1 = RWT_MA.MAFactory.create(bandsmoothType,bandsmoothArg);
			bandSmoother1.init(Input[0]);
			bandSmoother2 = RWT_MA.MAFactory.create(bandsmoothType,bandsmoothArg);
			bandSmoother2.init(Input[0]);

		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			bars.update();
			var maval = smoother.next(Input[0]);
			band.update(Input[0],maval);
			BandHigh.Set(bandSmoother1.next(band.UpperBand));
            BandLow.Set(bandSmoother2.next(band.LowerBand));
            MidLine.Set(maval);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BandHigh
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BandLow
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
        public int SmoothArg
        {
            get { return smoothArg; }
            set { smoothArg = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int BandSmoothArg
        {
            get { return bandsmoothArg; }
            set { bandsmoothArg = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType SmoothType
        {
            get { return smoothType; }
            set { smoothType = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType BandSmoothType
        {
            get { return bandsmoothType; }
            set { bandsmoothType = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public RWT_Bands.BandType BandType
        {
            get { return bandType; }
            set { bandType = value; }
        }

        [Description("Amount of Banding")]
        [GridCategory("Parameters")]
        public int LengthOfBand
        {
            get { return lengthOfBand; }
            set { lengthOfBand = value; }
        }
		
        [Description("Amount of Banding")]
        [GridCategory("Parameters")]
        public double BandArg
        {
            get { return bandArg; }
            set { bandArg = value; }
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
        private zMovingAverageBand[] cachezMovingAverageBand = null;

        private static zMovingAverageBand checkzMovingAverageBand = new zMovingAverageBand();

        /// <summary>
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        public zMovingAverageBand zMovingAverageBand(double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            return zMovingAverageBand(Input, bandArg, bandSmoothArg, bandSmoothType, bandType, lengthOfBand, smoothArg, smoothType);
        }

        /// <summary>
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        public zMovingAverageBand zMovingAverageBand(Data.IDataSeries input, double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            if (cachezMovingAverageBand != null)
                for (int idx = 0; idx < cachezMovingAverageBand.Length; idx++)
                    if (Math.Abs(cachezMovingAverageBand[idx].BandArg - bandArg) <= double.Epsilon && cachezMovingAverageBand[idx].BandSmoothArg == bandSmoothArg && cachezMovingAverageBand[idx].BandSmoothType == bandSmoothType && cachezMovingAverageBand[idx].BandType == bandType && cachezMovingAverageBand[idx].LengthOfBand == lengthOfBand && cachezMovingAverageBand[idx].SmoothArg == smoothArg && cachezMovingAverageBand[idx].SmoothType == smoothType && cachezMovingAverageBand[idx].EqualsInput(input))
                        return cachezMovingAverageBand[idx];

            lock (checkzMovingAverageBand)
            {
                checkzMovingAverageBand.BandArg = bandArg;
                bandArg = checkzMovingAverageBand.BandArg;
                checkzMovingAverageBand.BandSmoothArg = bandSmoothArg;
                bandSmoothArg = checkzMovingAverageBand.BandSmoothArg;
                checkzMovingAverageBand.BandSmoothType = bandSmoothType;
                bandSmoothType = checkzMovingAverageBand.BandSmoothType;
                checkzMovingAverageBand.BandType = bandType;
                bandType = checkzMovingAverageBand.BandType;
                checkzMovingAverageBand.LengthOfBand = lengthOfBand;
                lengthOfBand = checkzMovingAverageBand.LengthOfBand;
                checkzMovingAverageBand.SmoothArg = smoothArg;
                smoothArg = checkzMovingAverageBand.SmoothArg;
                checkzMovingAverageBand.SmoothType = smoothType;
                smoothType = checkzMovingAverageBand.SmoothType;

                if (cachezMovingAverageBand != null)
                    for (int idx = 0; idx < cachezMovingAverageBand.Length; idx++)
                        if (Math.Abs(cachezMovingAverageBand[idx].BandArg - bandArg) <= double.Epsilon && cachezMovingAverageBand[idx].BandSmoothArg == bandSmoothArg && cachezMovingAverageBand[idx].BandSmoothType == bandSmoothType && cachezMovingAverageBand[idx].BandType == bandType && cachezMovingAverageBand[idx].LengthOfBand == lengthOfBand && cachezMovingAverageBand[idx].SmoothArg == smoothArg && cachezMovingAverageBand[idx].SmoothType == smoothType && cachezMovingAverageBand[idx].EqualsInput(input))
                            return cachezMovingAverageBand[idx];

                zMovingAverageBand indicator = new zMovingAverageBand();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandArg = bandArg;
                indicator.BandSmoothArg = bandSmoothArg;
                indicator.BandSmoothType = bandSmoothType;
                indicator.BandType = bandType;
                indicator.LengthOfBand = lengthOfBand;
                indicator.SmoothArg = smoothArg;
                indicator.SmoothType = smoothType;
                Indicators.Add(indicator);
                indicator.SetUp();

                zMovingAverageBand[] tmp = new zMovingAverageBand[cachezMovingAverageBand == null ? 1 : cachezMovingAverageBand.Length + 1];
                if (cachezMovingAverageBand != null)
                    cachezMovingAverageBand.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezMovingAverageBand = tmp;
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
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMovingAverageBand zMovingAverageBand(double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            return _indicator.zMovingAverageBand(Input, bandArg, bandSmoothArg, bandSmoothType, bandType, lengthOfBand, smoothArg, smoothType);
        }

        /// <summary>
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        public Indicator.zMovingAverageBand zMovingAverageBand(Data.IDataSeries input, double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            return _indicator.zMovingAverageBand(input, bandArg, bandSmoothArg, bandSmoothType, bandType, lengthOfBand, smoothArg, smoothType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMovingAverageBand zMovingAverageBand(double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            return _indicator.zMovingAverageBand(Input, bandArg, bandSmoothArg, bandSmoothType, bandType, lengthOfBand, smoothArg, smoothType);
        }

        /// <summary>
        /// a band around an MA
        /// </summary>
        /// <returns></returns>
        public Indicator.zMovingAverageBand zMovingAverageBand(Data.IDataSeries input, double bandArg, int bandSmoothArg, RWT_MA.MAType bandSmoothType, RWT_Bands.BandType bandType, int lengthOfBand, int smoothArg, RWT_MA.MAType smoothType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zMovingAverageBand(input, bandArg, bandSmoothArg, bandSmoothType, bandType, lengthOfBand, smoothArg, smoothType);
        }
    }
}
#endregion
