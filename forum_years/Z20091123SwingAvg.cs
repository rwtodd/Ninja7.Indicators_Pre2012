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
    /// Richard Todd average of swings
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com average of swings")]
    public class Z20091123SwingAvg : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int swingStrength = 5; // Default setting for SwingStrength
            private int avgLen = 10; // Default setting for AvgLen
        // User defined variables (add any user defined variables below)
		    private Swing sw;
		    private double[] swings;
		    private int swindex;
		    private int lastSwing;
		    private bool ok;
		
      private RWTColorMethod colorMethod=RWTColorMethod.RisingFalling;
	  private RWTColorScheme colorScheme=RWTColorScheme.GreenRed;
      private double colorParam = 5.0;
      private Z20100112Colorizer colorizer;
		
		private double numDeviations = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "SwingAvg"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "UpperBand"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "LowerBand"));
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			sw = null;
			swindex = 0;
			lastSwing = swingStrength * 100;
			ok = false;
        }

		
		protected override void OnStartUp() {
			swings = new double[avgLen];
			sw = Swing(swingStrength);
            colorizer =  Z20100112Colorizer(SwingAvg,colorMethod,colorParam,colorScheme);			
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			++lastSwing;
			
			// check for a new swing...
			bool changed = false;
			int age = sw.SwingLowBar(0,1,swingStrength*2);
			if( (age >= 0) && 
				(age < lastSwing)) {
			  swings[swindex++] = Low[age];
			  if(swindex >= avgLen) { ok = true; swindex = 0; }
 			  lastSwing = age;
			  changed = true;
			}
			age = sw.SwingHighBar(0,1,swingStrength*2);
			if( (age >= 0) && 
				(age < lastSwing)) {
			  swings[swindex++] = High[age];
			  if(swindex >= avgLen) { ok = true; swindex = 0; }
			  lastSwing = age;
			  changed = true;
			}
			
			if(ok) {
				if(changed) {
			       double sum = 0;
		           double avg = 0;
			       foreach(double d in swings) sum += d;
				   avg = sum/avgLen;
			       SwingAvg.Set(avg);
				   if(!SwingAvg.ContainsValue(1)) SwingAvg.Set(1,avg);
				   if(numDeviations > 0) {
					  sum =0;
					  foreach(double d in swings) sum += ((d - avg)*(d - avg));
					  sum = numDeviations * Math.Sqrt(sum / (swings.Length - 1));
					  UpperBand.Set(avg + sum);
					  LowerBand.Set(avg - sum);
				      if(!UpperBand.ContainsValue(1)) {
  					    UpperBand.Set(1,avg + sum);
					    LowerBand.Set(1,avg - sum);
					  }
				   }
			     } else {
			       SwingAvg.Set(SwingAvg[1]);
				   if(numDeviations > 0) {
					  UpperBand.Set(UpperBand[1]);
					  LowerBand.Set(LowerBand[1]);
				   }
		     	 }
			}
			
			if(ok) colorizer.drawColor(this,0);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SwingAvg
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpperBand
        {
            get { return Values[1]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerBand
        {
            get { return Values[2]; }
        }

        [Description("Size of the swings you care about")]
        [GridCategory("Parameters")]
        public int SwingStrength
        {
            get { return swingStrength; }
            set { swingStrength = Math.Max(1, value); }
        }

        [Description("Length of the Avg")]
        [GridCategory("Parameters")]
        public int AvgLen
        {
            get { return avgLen; }
            set { avgLen = Math.Max(1, value); }
        }
		
        [Description("Deviation Bands?")]
        [GridCategory("Parameters")]
        public double NumDeviations
        {
            get { return numDeviations; }
            set { numDeviations = value; }
        }
		
		
        [Description("Method of the Colorizer")]
        [GridCategory("Parameters")]
        public RWTColorMethod zColorMethod
        {
            get { return colorMethod; }
            set { colorMethod = value; }
        }

        [Description("Input to the Colorizer")]
        [GridCategory("Parameters")]
        public double zColorParam
        {
            get { return colorParam; }
            set { colorParam = value; }
        }
		
        [Description("Colorizer Color Scheme")]
        [GridCategory("Parameters")]
        public RWTColorScheme zColorScheme
        {
            get { return colorScheme; }
            set { colorScheme = value; }
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
        private Z20091123SwingAvg[] cacheZ20091123SwingAvg = null;

        private static Z20091123SwingAvg checkZ20091123SwingAvg = new Z20091123SwingAvg();

        /// <summary>
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        public Z20091123SwingAvg Z20091123SwingAvg(int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            return Z20091123SwingAvg(Input, avgLen, numDeviations, swingStrength, zColorMethod, zColorParam, zColorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        public Z20091123SwingAvg Z20091123SwingAvg(Data.IDataSeries input, int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            if (cacheZ20091123SwingAvg != null)
                for (int idx = 0; idx < cacheZ20091123SwingAvg.Length; idx++)
                    if (cacheZ20091123SwingAvg[idx].AvgLen == avgLen && Math.Abs(cacheZ20091123SwingAvg[idx].NumDeviations - numDeviations) <= double.Epsilon && cacheZ20091123SwingAvg[idx].SwingStrength == swingStrength && cacheZ20091123SwingAvg[idx].zColorMethod == zColorMethod && Math.Abs(cacheZ20091123SwingAvg[idx].zColorParam - zColorParam) <= double.Epsilon && cacheZ20091123SwingAvg[idx].zColorScheme == zColorScheme && cacheZ20091123SwingAvg[idx].EqualsInput(input))
                        return cacheZ20091123SwingAvg[idx];

            lock (checkZ20091123SwingAvg)
            {
                checkZ20091123SwingAvg.AvgLen = avgLen;
                avgLen = checkZ20091123SwingAvg.AvgLen;
                checkZ20091123SwingAvg.NumDeviations = numDeviations;
                numDeviations = checkZ20091123SwingAvg.NumDeviations;
                checkZ20091123SwingAvg.SwingStrength = swingStrength;
                swingStrength = checkZ20091123SwingAvg.SwingStrength;
                checkZ20091123SwingAvg.zColorMethod = zColorMethod;
                zColorMethod = checkZ20091123SwingAvg.zColorMethod;
                checkZ20091123SwingAvg.zColorParam = zColorParam;
                zColorParam = checkZ20091123SwingAvg.zColorParam;
                checkZ20091123SwingAvg.zColorScheme = zColorScheme;
                zColorScheme = checkZ20091123SwingAvg.zColorScheme;

                if (cacheZ20091123SwingAvg != null)
                    for (int idx = 0; idx < cacheZ20091123SwingAvg.Length; idx++)
                        if (cacheZ20091123SwingAvg[idx].AvgLen == avgLen && Math.Abs(cacheZ20091123SwingAvg[idx].NumDeviations - numDeviations) <= double.Epsilon && cacheZ20091123SwingAvg[idx].SwingStrength == swingStrength && cacheZ20091123SwingAvg[idx].zColorMethod == zColorMethod && Math.Abs(cacheZ20091123SwingAvg[idx].zColorParam - zColorParam) <= double.Epsilon && cacheZ20091123SwingAvg[idx].zColorScheme == zColorScheme && cacheZ20091123SwingAvg[idx].EqualsInput(input))
                            return cacheZ20091123SwingAvg[idx];

                Z20091123SwingAvg indicator = new Z20091123SwingAvg();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AvgLen = avgLen;
                indicator.NumDeviations = numDeviations;
                indicator.SwingStrength = swingStrength;
                indicator.zColorMethod = zColorMethod;
                indicator.zColorParam = zColorParam;
                indicator.zColorScheme = zColorScheme;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20091123SwingAvg[] tmp = new Z20091123SwingAvg[cacheZ20091123SwingAvg == null ? 1 : cacheZ20091123SwingAvg.Length + 1];
                if (cacheZ20091123SwingAvg != null)
                    cacheZ20091123SwingAvg.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20091123SwingAvg = tmp;
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
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091123SwingAvg Z20091123SwingAvg(int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            return _indicator.Z20091123SwingAvg(Input, avgLen, numDeviations, swingStrength, zColorMethod, zColorParam, zColorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091123SwingAvg Z20091123SwingAvg(Data.IDataSeries input, int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            return _indicator.Z20091123SwingAvg(input, avgLen, numDeviations, swingStrength, zColorMethod, zColorParam, zColorScheme);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091123SwingAvg Z20091123SwingAvg(int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            return _indicator.Z20091123SwingAvg(Input, avgLen, numDeviations, swingStrength, zColorMethod, zColorParam, zColorScheme);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com average of swings
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091123SwingAvg Z20091123SwingAvg(Data.IDataSeries input, int avgLen, double numDeviations, int swingStrength, RWTColorMethod zColorMethod, double zColorParam, RWTColorScheme zColorScheme)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091123SwingAvg(input, avgLen, numDeviations, swingStrength, zColorMethod, zColorParam, zColorScheme);
        }
    }
}
#endregion
