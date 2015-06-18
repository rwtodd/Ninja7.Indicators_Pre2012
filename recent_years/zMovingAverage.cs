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


namespace RWT_MA {

	public enum MAType {
	  SMA,    // simple moving average
	  EMA,    // exponential moving average	
	  GAUSSIAN, // gaussian filter
      WMA,     // weighted FIR 
	  HULL,   // HULL MA on WMAs
	  HULLEMA, // HULL MA on EMAs
	  MEDIANFILT, // medians (can also give quartiles)
	  DELAY,      // delays by len (and also gives circular buffer)
	  KAMA210,    // kaufman adaptive min 2 lookback 10
	  DEXPMA,    // Double EXP Smoothing (for linear trends)
	  TEMA,       // Triple EMA
	  SUPER2POLE, // 2-Pole Super Smoother
	  SUPER3POLE, // 3-Pole Super Smoother
      LINREGSLOPE // Slope of the lin reg line of the input..
	}
	
	public interface MovingAverage {
		void init(double price);   // start us off...
		double next(double price); // compute the next value
	}

	public class GAUSSIAN : MovingAverage {
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
		private int curIdx;
		private int poles;
		
		public GAUSSIAN(int poles_, double len) {
			poles = poles_;
			if(poles < 1) poles = 1;
			double beta = ( 1.0 - Math.Cos(2.0*Math.PI/len) ) / ( Math.Pow(1.4142,2.0/poles) - 1.0 );
			double alpha = ( -beta + Math.Sqrt(beta*beta + 2.0*beta) );

			alphas = new double[poles+1];
			alphas[0] = Math.Pow(alpha,poles);
			for(int i = 1; i < poles+1; ++i) {
			  alphas[i] = Math.Pow(1.0-alpha,i)*binco[poles][i];	
			}
			avg = new double[poles+1];
		}
		
		public void init(double price) {
			for(int i =0; i < (poles+1); ++i) avg[i] = price;
			curIdx = 0;
		}
		
		public double next(double price) {
		    if(++curIdx > poles) curIdx = 0;
			int myIdx = curIdx;
			double curavg = price * alphas[0];
			for(int i = 1; i < (poles+1); ++i) {
				if(--myIdx < 0) myIdx = poles;
				curavg += (((i&1)==1)?1.0:-1.0)*alphas[i]*avg[myIdx];
			}
			avg[curIdx] = curavg;
			return curavg;
		}
	}
	
	public class LINREGSLOPE : MovingAverage {
		private int length;
		private double period;
		private double sumX;
		private double divisor;
		private double sumY;
		private double[] prices;
		private int curIdx;
		public LINREGSLOPE(double len) {
			length = (int)len;
			period = (double)length;
		}
		public void init(double price) {
			prices = new double[length];
			for(int i = 0; i < length; ++i) {
				prices[i] = price;	
			}
			curIdx = 0;
		    sumX	= period * (period + 1) * 0.5;
			divisor =  period*period*(period+1)*(2*period+1)/6.0  - sumX*sumX;
			sumY = price*length;
		}
		public double next(double price) {
			// update circular buffer and sumY...
			if(++curIdx >= length) curIdx = 0;
			sumY = sumY - prices[curIdx] + price;
			prices[curIdx] = price;
			
			// calculate slope...
			double X = 1;
			double sumXY = 0;
			// go from curIdx+1 to end...
			for(int i1 = curIdx+1; i1 < length; ++i1) {
				sumXY += X*prices[i1];
				X += 1.0;	
			}
			// and from beginning to curIdx...
			for(int i2 = 0; i2 <= curIdx; ++i2) {
				sumXY += X*prices[i2];
				X += 1.0;
			}
			// and compute the linreg slope...
			return (period*sumXY - sumX*sumY) / divisor;
		}
	}
	
	public class DEXPMA : MovingAverage {
		  private double ema1, ema2;
		  private double atauconst;
		  private double alpha;
		 
		  public DEXPMA(double length)  {
				alpha = 2.0/(1.0+length);
			    var tau = 1.0;
			    atauconst = (alpha*tau)/(1.0-alpha);
		  }
		  public void init(double val) {
		    ema1 = val; ema2 = val;	
		  }
		  public double next(double val) {
			ema1 = ema1 + alpha*(val-ema1);
			ema2 = ema2 + alpha*(ema1-ema2);
			return  (2.0+atauconst)*ema1 - (1.0+atauconst)*ema2;
		  }
	}
	
	public class DELAY : MovingAverage {
		private double[] prices; // circular buffer
		private int curIdx;
		private int length;
		public DELAY(double len) {
			length = (int)len;	
		}
		public void init(double price) {
			prices = new double[length];
			for(int i = 0; i < length; ++i) {
				prices[i] = price;	
			}
			curIdx = 0;
		}
		public double next(double price) {
			if(++curIdx >= length) curIdx = 0;
			var ans = prices[curIdx];
			prices[curIdx] = price;
			return ans;
		}
		
		public double this[int i] {
		  get {
		    // get the value i bars ago..
			// assumes NEXT already called on this bar
			// so curIdx points to this[0]...
			var idx = curIdx - i;
			while(idx < 0) idx += length;
			return prices[idx];
		  }
		}
	}
	
	public class SMA : MovingAverage {
		private DELAY delay;  // circular buffer
		private double length;
		private double sum;
		
		public SMA(double len) {
			length = Math.Floor(len);
			delay = new DELAY(length);
		}
		public void init(double price) {
			delay.init(price);
			sum = price*length;
		}
		
		public double next(double price) {
			sum = sum - delay.next(price) + price;
			return ( sum  / length );
		}
	}
	
	public class MEDIANFILT : MovingAverage {
            private int length;
		    private double[] prices;
		    private DELAY delay;
		public MEDIANFILT(double len) {
		   length = (int)len;
		   delay = new DELAY(len);
		}
		public void init(double price) {
			delay.init(price);
			prices = new double[length];
			for(int i = 0; i < length; ++i) {
				prices[i] = price;
			}
		}
		public double next(double toadd) {
			var todel = delay.next(toadd);
			
			// adjust the sorted prices...
			var endIndx = length - 1;
			for(int i = 0; i < length; ++i) {
			  if(prices[i] == todel) {
			    if( (i == endIndx) ||
					 (prices[i+1] >= toadd) ) {
					prices[i] = toadd;
					break;
				}
				prices[i] = prices[i+1];
				todel = prices[i];
			  } 
			  else if(prices[i] > toadd) {
			    var tmp = prices[i];
				prices[i] = toadd;
				toadd = tmp;
			  }
			}
			
			// compute the median
			var ans = prices[length/2];
			
			if((length & 1) == 0) { 
				ans = (0.5*(ans+prices[length/2-1]));
            }
			return ans;
		}
		
		double[] Window {  get { return prices; } }
	}
		
	public class EMA : MovingAverage {
		private double alpha;
		private double current;
		public EMA(double len) {
		  alpha = 2.0/(1.0+len);
		  current = 0.0;
		}
		public void init(double price) {
		  current = price;	
		}
		public double next(double price) {
		  current = current + alpha*(price-current);
		  return current;
		}
	}
	
	public class TEMA : MovingAverage {
		private double alpha;
		private double current0, current1, current2;
		public TEMA(double len) {
		  alpha = 2.0/(1.0+len);
		  current0 = 0.0;
		  current1 = 0.0;
		  current2 = 0.0;
		}
		public void init(double price) {
		  current0 = price;
		  current1 = price;
		  current2 = price;
		}
		public double next(double price) {
		  current2 = current2 + alpha*(price-current2);
		  current1 = current1 + alpha*(current2-current1);
		  current0 = current0 + alpha*(current1-current0);
		  return 3.0*current2 - 3.0*current1 + current0;
		}
	}
	
	public class WMA : MovingAverage {
		private double[] prices;  // circular buffer
		private int curIdx;
		private int length;
		
		public WMA(double len) {
			length = (int)len;
			prices = new double[length];
		}
		public void init(double price) {
			for(int i = 0; i < length; ++i) {
				prices[i] = price;	
			}
			curIdx = 0;
		}
		public double next(double price) {
			// 1. put the price in the circular buffer
			if(++curIdx >= length) curIdx = 0;
			prices[curIdx] = price;
			// 2. calculate the WMA..
			int mult = length;
			int i = curIdx;
			double sum = 0.0;
			
			while(mult > 0) {
			  sum += prices[(i--)]*(mult--);
			  if(i < 0) i = length - 1;
			}
			
			return ( sum  / ( length * (length+1) / 2 ) );
		}
	}
	
	public class Hull : MovingAverage {
		WMA wma1, wma2, wmasqrt;
		public Hull(double len) {
			wma1 = new WMA(len);
			wma2 = new WMA(len/2.0);
			wmasqrt = new WMA(Math.Sqrt(len));
		}
		public void init(double price) {
			wma1.init(price);
			wma2.init(price);
			wmasqrt.init(price);
		}
		public double next(double price) {
		    var a1 = wma1.next(price);
			var a2 = 2*wma2.next(price);
			return wmasqrt.next(a2 - a1);
		}
	}
	public class HullEMA : MovingAverage {
		EMA ema1, ema2, emasqrt;
		public HullEMA(double len) {
			ema1 = new EMA(len);
			ema2 = new EMA(len/2.0);
			emasqrt = new EMA(Math.Sqrt(len));
		}
		public void init(double price) {
			ema1.init(price);
			ema2.init(price);
			emasqrt.init(price);
		}
		public double next(double price) {
		    var a1 = ema1.next(price);
			var a2 = 2*ema2.next(price);
			return emasqrt.next(a2 - a1);
		}
	}

	public class Kama210 : MovingAverage {
		private double[] diffSeries;
		private double[] priceSeries;
		private int pIndex;
		private int dsIndex, lookback;
		private double dsSum, fastAlpha, slowAlpha;
		private double maVal;
		public Kama210(double len) {
			slowAlpha = 2.0/(1.0+len);
			fastAlpha = 2.0/3.0;
			lookback = 10;
			diffSeries = new double[lookback];
			priceSeries = new double[lookback];
		}
		public void init(double price) {
			maVal = price;
			dsIndex = 0;
			pIndex = 0;
			dsSum = 0;
			for(int i = 0; i < lookback; ++i) {
			  priceSeries[i] = price;	
			}
		}
		public double next(double price) {
			var diff = Math.Abs(price - priceSeries[pIndex]);
			
			if(++dsIndex >= lookback) dsIndex = 0;
			dsSum = dsSum - diffSeries[dsIndex] + diff;
			diffSeries[dsIndex] = diff; 
						
			if(++pIndex >= lookback) pIndex = 0;
			var signal = Math.Abs(price - priceSeries[pIndex]);
			priceSeries[pIndex] = price;

			if(dsSum == 0) {
			  return maVal;	
			} 
			var alpha = (signal / dsSum) * (fastAlpha - slowAlpha) + slowAlpha;
			alpha = alpha*alpha;
			maVal = maVal + alpha*(price - maVal);
			return maVal;
		}
		
	}
	
	public class Super2 : MovingAverage {
		private double coef1, coef2, coef3;
		private double filt1, filt2;
		
		public Super2(double len) {
		  var a1 = Math.Exp(-Math.Sqrt(2.0)*Math.PI / len );
		  var b1 = 2.0*a1*Math.Cos(Math.Sqrt(2.0)*Math.PI / len);
		  coef2 = b1;
		  coef3 = -a1*a1;
		  coef1 = 1 - coef2 - coef3;
		}
		public void init(double price) {
		  filt1 = price; filt2 = price;	
		}
		public double next(double price) {
		  var ans = coef1*price + coef2*filt1 + coef3*filt2;
		  filt2 = filt1; filt1 = ans; 
		  return ans;
		}
	}
	
	public class Super3 : MovingAverage {
		private double coef1, coef2, coef3, coef4;
		private double filt1, filt2, filt3;
		
		public Super3(double len) {
		  var a1 = Math.Exp(-Math.PI / len );
		  var b1 = 2.0*a1*Math.Cos(1.738*Math.PI / len);
		  var c1 = a1*a1;
		  coef2 = b1+c1;
		  coef3 = -(c1+b1*c1);
		  coef4 = c1*c1;
		  coef1 = 1 - coef2 - coef3 - coef4;
		}
		public void init(double price) {
		  filt1 = price; filt2 = price;	filt3 = price;
		}
		public double next(double price) {
		  var ans = coef1*price + coef2*filt1 + coef3*filt2 + coef4*filt3;
		  filt3 = filt2; filt2 = filt1; filt1 = ans; 
		  return ans;
		}
	}

	public class MAFactory {
	   public static MovingAverage create(MAType type, double len) {
		  MovingAverage ans = null;
		  switch(type) {
			case MAType.SMA:
				ans = new RWT_MA.SMA(len);
				break;
			case MAType.HULL:
				ans = new RWT_MA.Hull(len);
				break;
			case MAType.GAUSSIAN:
				ans = new RWT_MA.GAUSSIAN((int)Math.Floor((len-(int)len)*10),Math.Floor(len));
				break;
			case MAType.HULLEMA:
				ans = new RWT_MA.HullEMA(len);
				break;
			case MAType.KAMA210:
				ans = new RWT_MA.Kama210(len);
				break;
			case MAType.EMA:
				ans = new RWT_MA.EMA(len);
				break;
			case MAType.WMA:
				ans = new RWT_MA.WMA(len);
				break;
			case MAType.DEXPMA:
				ans = new RWT_MA.DEXPMA(len);
				break;
			case MAType.MEDIANFILT:
				ans = new RWT_MA.MEDIANFILT(len);
				break;
			case MAType.DELAY:
				ans = new RWT_MA.DELAY(len);
				break;
			case MAType.TEMA:
				ans = new RWT_MA.TEMA(len);
				break;
			case MAType.SUPER2POLE:
				ans = new RWT_MA.Super2(len);
				break;
			case MAType.SUPER3POLE:
				ans = new RWT_MA.Super3(len);
				break;
			case MAType.LINREGSLOPE:
				ans = new RWT_MA.LINREGSLOPE(len);
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
    /// A moving average
    /// </summary>
    [Description("A moving average")]
    public class zMovingAverage : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double length = 20; // Default setting for Length
            private RWT_MA.MAType type = RWT_MA.MAType.EMA; // Default setting for Type
        // User defined variables (add any user defined variables below)
		    private RWT_MA.MovingAverage movavg;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.SlateBlue), PlotStyle.Line, "MovingAvg"));
            //Overlay				= true;
        }

		protected override void OnStartUp() {
			movavg = RWT_MA.MAFactory.create(type,length);	
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar == 0) {
				movavg.init(Input[0]);	
				Value.Set(Input[0]);
				return;
			}
			
			Value.Set(movavg.next(Input[0]));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MovingAvg
        {
            get { return Values[0]; }
        }

        [Description("length or lag of the MA")]
        [GridCategory("Parameters")]
        public double Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }

        [Description("type of MA to use")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType Type
        {
            get { return type; }
            set { type =  value; }
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
        private zMovingAverage[] cachezMovingAverage = null;

        private static zMovingAverage checkzMovingAverage = new zMovingAverage();

        /// <summary>
        /// A moving average
        /// </summary>
        /// <returns></returns>
        public zMovingAverage zMovingAverage(double length, RWT_MA.MAType type)
        {
            return zMovingAverage(Input, length, type);
        }

        /// <summary>
        /// A moving average
        /// </summary>
        /// <returns></returns>
        public zMovingAverage zMovingAverage(Data.IDataSeries input, double length, RWT_MA.MAType type)
        {
            if (cachezMovingAverage != null)
                for (int idx = 0; idx < cachezMovingAverage.Length; idx++)
                    if (Math.Abs(cachezMovingAverage[idx].Length - length) <= double.Epsilon && cachezMovingAverage[idx].Type == type && cachezMovingAverage[idx].EqualsInput(input))
                        return cachezMovingAverage[idx];

            lock (checkzMovingAverage)
            {
                checkzMovingAverage.Length = length;
                length = checkzMovingAverage.Length;
                checkzMovingAverage.Type = type;
                type = checkzMovingAverage.Type;

                if (cachezMovingAverage != null)
                    for (int idx = 0; idx < cachezMovingAverage.Length; idx++)
                        if (Math.Abs(cachezMovingAverage[idx].Length - length) <= double.Epsilon && cachezMovingAverage[idx].Type == type && cachezMovingAverage[idx].EqualsInput(input))
                            return cachezMovingAverage[idx];

                zMovingAverage indicator = new zMovingAverage();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.Type = type;
                Indicators.Add(indicator);
                indicator.SetUp();

                zMovingAverage[] tmp = new zMovingAverage[cachezMovingAverage == null ? 1 : cachezMovingAverage.Length + 1];
                if (cachezMovingAverage != null)
                    cachezMovingAverage.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezMovingAverage = tmp;
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
        /// A moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMovingAverage zMovingAverage(double length, RWT_MA.MAType type)
        {
            return _indicator.zMovingAverage(Input, length, type);
        }

        /// <summary>
        /// A moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.zMovingAverage zMovingAverage(Data.IDataSeries input, double length, RWT_MA.MAType type)
        {
            return _indicator.zMovingAverage(input, length, type);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMovingAverage zMovingAverage(double length, RWT_MA.MAType type)
        {
            return _indicator.zMovingAverage(Input, length, type);
        }

        /// <summary>
        /// A moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.zMovingAverage zMovingAverage(Data.IDataSeries input, double length, RWT_MA.MAType type)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zMovingAverage(input, length, type);
        }
    }
}
#endregion
