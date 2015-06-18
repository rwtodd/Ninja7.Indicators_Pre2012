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
    /// MACD of independent MA types
    /// </summary>
    [Description("MACD of independent MA types")]
    public class zMaMacd : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double len1 = 38.000; // Default setting for Len1
            private double len2 = 40.700; // Default setting for Len2
		    private double trigger = 0;
		    private double alpha = 0.333333; 
			private RWT_MA.MovingAverage ma1;
			private RWT_MA.MovingAverage ma2;
			private RWT_MA.MAType type1 = RWT_MA.MAType.HULLEMA;
			private RWT_MA.MAType type2 = RWT_MA.MAType.GAUSSIAN;
		    private double scale = 3.0;
		
        // User defined variables (add any user defined variables below)
        #endregion

		protected override void OnStartUp() {
			ma1 = RWT_MA.MAFactory.create(type1,len1);
			ma2 = RWT_MA.MAFactory.create(type2,len2);
			ma1.init(Input[0]);
			ma2.init(Input[0]);
			trigger = 0;
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Crimson), PlotStyle.Line, "MACD"));
            Add(new Plot(Color.FromKnownColor(KnownColor.MidnightBlue), PlotStyle.Bar, "Hist"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "Zero"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var macdval = ma1.next(Input[0]) - ma2.next(Input[0]);
			trigger = trigger + alpha*(macdval - trigger);
            MACD.Set(macdval);
            Hist.Set(scale*(macdval-trigger));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MACD
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Hist
        {
            get { return Values[1]; }
        }

        [Description("length 1")]
        [GridCategory("Parameters")]
        public double Len1
        {
            get { return len1; }
            set { len1 = Math.Max(1, value); }
        }

        [Description("length 2")]
        [GridCategory("Parameters")]
        public double Len2
        {
            get { return len2; }
            set { len2 = Math.Max(1, value); }
        }
		
		[Description("Type1")]
		[GridCategory("Parameters")]
		public RWT_MA.MAType AType1 {
			get { return type1; }
			set { type1 = value; }
		}
		[Description("Type1")]
		[GridCategory("Parameters")]
		public RWT_MA.MAType AType2 {
			get { return type2; }
			set { type2 = value; }
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
        private zMaMacd[] cachezMaMacd = null;

        private static zMaMacd checkzMaMacd = new zMaMacd();

        /// <summary>
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        public zMaMacd zMaMacd(RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            return zMaMacd(Input, aType1, aType2, len1, len2);
        }

        /// <summary>
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        public zMaMacd zMaMacd(Data.IDataSeries input, RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            if (cachezMaMacd != null)
                for (int idx = 0; idx < cachezMaMacd.Length; idx++)
                    if (cachezMaMacd[idx].AType1 == aType1 && cachezMaMacd[idx].AType2 == aType2 && Math.Abs(cachezMaMacd[idx].Len1 - len1) <= double.Epsilon && Math.Abs(cachezMaMacd[idx].Len2 - len2) <= double.Epsilon && cachezMaMacd[idx].EqualsInput(input))
                        return cachezMaMacd[idx];

            lock (checkzMaMacd)
            {
                checkzMaMacd.AType1 = aType1;
                aType1 = checkzMaMacd.AType1;
                checkzMaMacd.AType2 = aType2;
                aType2 = checkzMaMacd.AType2;
                checkzMaMacd.Len1 = len1;
                len1 = checkzMaMacd.Len1;
                checkzMaMacd.Len2 = len2;
                len2 = checkzMaMacd.Len2;

                if (cachezMaMacd != null)
                    for (int idx = 0; idx < cachezMaMacd.Length; idx++)
                        if (cachezMaMacd[idx].AType1 == aType1 && cachezMaMacd[idx].AType2 == aType2 && Math.Abs(cachezMaMacd[idx].Len1 - len1) <= double.Epsilon && Math.Abs(cachezMaMacd[idx].Len2 - len2) <= double.Epsilon && cachezMaMacd[idx].EqualsInput(input))
                            return cachezMaMacd[idx];

                zMaMacd indicator = new zMaMacd();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AType1 = aType1;
                indicator.AType2 = aType2;
                indicator.Len1 = len1;
                indicator.Len2 = len2;
                Indicators.Add(indicator);
                indicator.SetUp();

                zMaMacd[] tmp = new zMaMacd[cachezMaMacd == null ? 1 : cachezMaMacd.Length + 1];
                if (cachezMaMacd != null)
                    cachezMaMacd.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezMaMacd = tmp;
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
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMaMacd zMaMacd(RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            return _indicator.zMaMacd(Input, aType1, aType2, len1, len2);
        }

        /// <summary>
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        public Indicator.zMaMacd zMaMacd(Data.IDataSeries input, RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            return _indicator.zMaMacd(input, aType1, aType2, len1, len2);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zMaMacd zMaMacd(RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            return _indicator.zMaMacd(Input, aType1, aType2, len1, len2);
        }

        /// <summary>
        /// MACD of independent MA types
        /// </summary>
        /// <returns></returns>
        public Indicator.zMaMacd zMaMacd(Data.IDataSeries input, RWT_MA.MAType aType1, RWT_MA.MAType aType2, double len1, double len2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zMaMacd(input, aType1, aType2, len1, len2);
        }
    }
}
#endregion
