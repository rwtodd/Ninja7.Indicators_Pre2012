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
    /// an average of swings
    /// </summary>
    [Description("an average of swings")]
    public class zSwingAverage : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int swingStrength = 5; // Default setting for SwingStrength
			private int length = 10;
			private RWT_MA.MAType type = RWT_MA.MAType.HULLEMA;
			private RWT_HA.PrimaryOHLC intype = RWT_HA.PrimaryOHLC.BARS;
			private RWT_MA.MovingAverage smoother;
		
			private IDataSeries highSeries, lowSeries;
			private zFastMIN mins;
			private zFastMAX maxs;
        // User defined variables (add any user defined variables below)
        	private double extseen;
			private bool lookForHigh;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "SwingAvg"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			if(intype == RWT_HA.PrimaryOHLC.BARS) {
				highSeries = High;
				lowSeries = Low;
			} else {
				highSeries = Input;
				lowSeries = Input;
			}
			
			maxs = zFastMAX(highSeries,swingStrength);
			mins = zFastMIN(lowSeries,swingStrength);
			smoother = RWT_MA.MAFactory.create(type,(double)length);
			smoother.init(lowSeries[0]);
			extseen = highSeries[0];
			lookForHigh = true;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(lookForHigh) {
				extseen = Math.Max(extseen,highSeries[0]);
			} else {
				extseen = Math.Min(extseen,lowSeries[0]);	
			}
			
			if(CurrentBar == 0) { Value.Set(lowSeries[0]); return; }
			
			if(lookForHigh) {
				if(mins[0] < mins[1]) {
					lookForHigh = false;
					Value.Set(smoother.next(extseen));
					extseen = lowSeries[0];
					return;
				}
			} else {
				if(maxs[0] > maxs[1]) {
					lookForHigh = true;	
					Value.Set(smoother.next(extseen));
					extseen = highSeries[0];
					return;
				}
			}
			
			Value.Set(Value[1]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SwingAvg
        {
            get { return Values[0]; }
        }

        [Description("Swing Strength")]
        [GridCategory("Parameters")]
        public int SwingStrength
        {
            get { return swingStrength; }
            set { swingStrength = Math.Max(2, value); }
        }
		
        [Description("length or lag of the MA")]
        [GridCategory("Parameters")]
        public int MALength
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }

        [Description("type of MA to use")]
        [GridCategory("Parameters")]
        public RWT_MA.MAType MAType
        {
            get { return type; }
            set { type =  value; }
        }
        [Description("type of Input to use")]
        [GridCategory("Parameters")]
        public RWT_HA.PrimaryOHLC InputType
        {
            get { return intype; }
            set { intype =  value; }
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
        private zSwingAverage[] cachezSwingAverage = null;

        private static zSwingAverage checkzSwingAverage = new zSwingAverage();

        /// <summary>
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        public zSwingAverage zSwingAverage(RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            return zSwingAverage(Input, inputType, mALength, mAType, swingStrength);
        }

        /// <summary>
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        public zSwingAverage zSwingAverage(Data.IDataSeries input, RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            if (cachezSwingAverage != null)
                for (int idx = 0; idx < cachezSwingAverage.Length; idx++)
                    if (cachezSwingAverage[idx].InputType == inputType && cachezSwingAverage[idx].MALength == mALength && cachezSwingAverage[idx].MAType == mAType && cachezSwingAverage[idx].SwingStrength == swingStrength && cachezSwingAverage[idx].EqualsInput(input))
                        return cachezSwingAverage[idx];

            lock (checkzSwingAverage)
            {
                checkzSwingAverage.InputType = inputType;
                inputType = checkzSwingAverage.InputType;
                checkzSwingAverage.MALength = mALength;
                mALength = checkzSwingAverage.MALength;
                checkzSwingAverage.MAType = mAType;
                mAType = checkzSwingAverage.MAType;
                checkzSwingAverage.SwingStrength = swingStrength;
                swingStrength = checkzSwingAverage.SwingStrength;

                if (cachezSwingAverage != null)
                    for (int idx = 0; idx < cachezSwingAverage.Length; idx++)
                        if (cachezSwingAverage[idx].InputType == inputType && cachezSwingAverage[idx].MALength == mALength && cachezSwingAverage[idx].MAType == mAType && cachezSwingAverage[idx].SwingStrength == swingStrength && cachezSwingAverage[idx].EqualsInput(input))
                            return cachezSwingAverage[idx];

                zSwingAverage indicator = new zSwingAverage();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.InputType = inputType;
                indicator.MALength = mALength;
                indicator.MAType = mAType;
                indicator.SwingStrength = swingStrength;
                Indicators.Add(indicator);
                indicator.SetUp();

                zSwingAverage[] tmp = new zSwingAverage[cachezSwingAverage == null ? 1 : cachezSwingAverage.Length + 1];
                if (cachezSwingAverage != null)
                    cachezSwingAverage.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezSwingAverage = tmp;
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
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSwingAverage zSwingAverage(RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            return _indicator.zSwingAverage(Input, inputType, mALength, mAType, swingStrength);
        }

        /// <summary>
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        public Indicator.zSwingAverage zSwingAverage(Data.IDataSeries input, RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            return _indicator.zSwingAverage(input, inputType, mALength, mAType, swingStrength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zSwingAverage zSwingAverage(RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            return _indicator.zSwingAverage(Input, inputType, mALength, mAType, swingStrength);
        }

        /// <summary>
        /// an average of swings
        /// </summary>
        /// <returns></returns>
        public Indicator.zSwingAverage zSwingAverage(Data.IDataSeries input, RWT_HA.PrimaryOHLC inputType, int mALength, RWT_MA.MAType mAType, int swingStrength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zSwingAverage(input, inputType, mALength, mAType, swingStrength);
        }
    }
}
#endregion
