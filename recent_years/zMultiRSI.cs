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
    /// multiple RSI
    /// </summary>
    [Description("multiple RSI")]
    public class zMultiRSI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int periodBegin = 5; // Default setting for PeriodBegin
            private int periodInterval = 2; // Default setting for PeriodInterval
            private Color colorOfBand = Color.Orange; // Default setting for ColorOfBand
            private Color colorBegin = Color.Green; // Default setting for ColorBegin
			private double[] alphas;
			private double[] upavgs;
		    private double[] dnavgs;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Line(System.Drawing.Color.Black, 20, "Lower"));
			Add(new Line(System.Drawing.Color.Black, 80, "Upper"));
			Add(new Line(System.Drawing.Color.Black, 50, "Middle"));

            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI01"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI02"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI03"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI04"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI05"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "RSI06"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			alphas = new double[6];
			upavgs = new double[6];
			dnavgs = new double[6];
			int per = periodBegin;
			for(int i = 0; i < 6; ++i, per += periodInterval) { 
				Plots[i].Pen.Width = ((i>0)?1:2);
				Plots[i].Pen.Color = ((i>0)?colorOfBand:colorBegin);
//				alphas[i] = 1.0/((double)per);
				alphas[i] = 2.0/(1.0 + (double)per);
			}			
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) return;
			
			var up = 0.0;
			var dn = 0.0;
			var change = Input[0] - Input[1];
			if(change < 0) {
			  dn = -change;	
			} else {
			  up = change;	
			}
			
			for(int i = 0; i < 6; ++i) {
			  upavgs[i] = upavgs[i] + alphas[i]*(up - upavgs[i]);
			  dnavgs[i] = dnavgs[i] + alphas[i]*(dn - dnavgs[i]);
			  if(dnavgs[i] == 0) Values[i].Set(100);
//			  else Values[i].Set(100 - 100/(1 + upavgs[i] / dnavgs[i]));
			  else Values[i].Set(100*upavgs[i]/(upavgs[i]+dnavgs[i]));
			}			
		}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RSI01
        {
            get { return Values[0]; }
        }

        [Description("beginning")]
        [GridCategory("Parameters")]
        public int PeriodBegin
        {
            get { return periodBegin; }
            set { periodBegin = Math.Max(1, value); }
        }

        [Description("Step")]
        [GridCategory("Parameters")]
        public int PeriodInterval
        {
            get { return periodInterval; }
            set { periodInterval = Math.Max(1, value); }
        }

        [Description("color")]
        [GridCategory("Parameters")]
        public Color ColorOfBand
        {
            get { return colorOfBand; }
            set { colorOfBand = value; }
        }

		// Serialize our Color object
		[Browsable(false)]
		public string ColorOfBandSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorOfBand); }
			set { colorOfBand = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
        [Description("color")]
        [GridCategory("Parameters")]
        public Color ColorBegin
        {
            get { return colorBegin; }
            set { colorBegin = value; }
        }
		
		// Serialize our Color object
		[Browsable(false)]
		public string ColorBeginSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorBegin); }
			set { colorBegin = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private zMultiRSI[] cachezMultiRSI = null;

        private static zMultiRSI checkzMultiRSI = new zMultiRSI();

        /// <summary>
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        public zMultiRSI zMultiRSI(Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            return zMultiRSI(Input, colorBegin, colorOfBand, periodBegin, periodInterval);
        }

        /// <summary>
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        public zMultiRSI zMultiRSI(Data.IDataSeries input, Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            if (cachezMultiRSI != null)
                for (int idx = 0; idx < cachezMultiRSI.Length; idx++)
                    if (cachezMultiRSI[idx].ColorBegin == colorBegin && cachezMultiRSI[idx].ColorOfBand == colorOfBand && cachezMultiRSI[idx].PeriodBegin == periodBegin && cachezMultiRSI[idx].PeriodInterval == periodInterval && cachezMultiRSI[idx].EqualsInput(input))
                        return cachezMultiRSI[idx];

            lock (checkzMultiRSI)
            {
                checkzMultiRSI.ColorBegin = colorBegin;
                colorBegin = checkzMultiRSI.ColorBegin;
                checkzMultiRSI.ColorOfBand = colorOfBand;
                colorOfBand = checkzMultiRSI.ColorOfBand;
                checkzMultiRSI.PeriodBegin = periodBegin;
                periodBegin = checkzMultiRSI.PeriodBegin;
                checkzMultiRSI.PeriodInterval = periodInterval;
                periodInterval = checkzMultiRSI.PeriodInterval;

                if (cachezMultiRSI != null)
                    for (int idx = 0; idx < cachezMultiRSI.Length; idx++)
                        if (cachezMultiRSI[idx].ColorBegin == colorBegin && cachezMultiRSI[idx].ColorOfBand == colorOfBand && cachezMultiRSI[idx].PeriodBegin == periodBegin && cachezMultiRSI[idx].PeriodInterval == periodInterval && cachezMultiRSI[idx].EqualsInput(input))
                            return cachezMultiRSI[idx];

                zMultiRSI indicator = new zMultiRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorBegin = colorBegin;
                indicator.ColorOfBand = colorOfBand;
                indicator.PeriodBegin = periodBegin;
                indicator.PeriodInterval = periodInterval;
                Indicators.Add(indicator);
                indicator.SetUp();

                zMultiRSI[] tmp = new zMultiRSI[cachezMultiRSI == null ? 1 : cachezMultiRSI.Length + 1];
                if (cachezMultiRSI != null)
                    cachezMultiRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezMultiRSI = tmp;
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
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMultiRSI zMultiRSI(Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            return _indicator.zMultiRSI(Input, colorBegin, colorOfBand, periodBegin, periodInterval);
        }

        /// <summary>
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        public Indicator.zMultiRSI zMultiRSI(Data.IDataSeries input, Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            return _indicator.zMultiRSI(input, colorBegin, colorOfBand, periodBegin, periodInterval);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMultiRSI zMultiRSI(Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            return _indicator.zMultiRSI(Input, colorBegin, colorOfBand, periodBegin, periodInterval);
        }

        /// <summary>
        /// multiple RSI
        /// </summary>
        /// <returns></returns>
        public Indicator.zMultiRSI zMultiRSI(Data.IDataSeries input, Color colorBegin, Color colorOfBand, int periodBegin, int periodInterval)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zMultiRSI(input, colorBegin, colorOfBand, periodBegin, periodInterval);
        }
    }
}
#endregion
