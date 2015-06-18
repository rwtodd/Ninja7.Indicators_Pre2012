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
    /// hull-flavor ma
    /// </summary>
    [Description("hull-flavor ma")]
    public class zHullStyleMA : Indicator
    {
        #region Variables
            private int length = 20; // Default setting for Length
            private RWT_MA.MAType type = RWT_MA.MAType.EMA; // Default setting for Type
        // User defined variables (add any user defined variables below)
		    private RWT_MA.MovingAverage avg1, avg2, avgsqrt;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "HullMA"));
            Overlay				= true;
        }

		protected override void OnStartUp() {
			avg1 = RWT_MA.MAFactory.create(type,((double)length));	
			avg2 = RWT_MA.MAFactory.create(type,((double)length)/2.0);	
			avgsqrt = RWT_MA.MAFactory.create(type,Math.Sqrt((double)length));	
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar == 0) {
				avg1.init(Input[0]);
				avg2.init(Input[0]);
				avgsqrt.init(Input[0]);
				HullMA.Set(Input[0]);
				return;
			}

			var a1 = avg1.next(Input[0]);
			var a2 = avg2.next(Input[0]);
		
            HullMA.Set(avgsqrt.next(2*a2 - a1));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HullMA
        {
            get { return Values[0]; }
        }

        [Description("length or lag of the MA")]
        [GridCategory("Parameters")]
        public int Length
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
        private zHullStyleMA[] cachezHullStyleMA = null;

        private static zHullStyleMA checkzHullStyleMA = new zHullStyleMA();

        /// <summary>
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        public zHullStyleMA zHullStyleMA(int length, RWT_MA.MAType type)
        {
            return zHullStyleMA(Input, length, type);
        }

        /// <summary>
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        public zHullStyleMA zHullStyleMA(Data.IDataSeries input, int length, RWT_MA.MAType type)
        {
            if (cachezHullStyleMA != null)
                for (int idx = 0; idx < cachezHullStyleMA.Length; idx++)
                    if (cachezHullStyleMA[idx].Length == length && cachezHullStyleMA[idx].Type == type && cachezHullStyleMA[idx].EqualsInput(input))
                        return cachezHullStyleMA[idx];

            lock (checkzHullStyleMA)
            {
                checkzHullStyleMA.Length = length;
                length = checkzHullStyleMA.Length;
                checkzHullStyleMA.Type = type;
                type = checkzHullStyleMA.Type;

                if (cachezHullStyleMA != null)
                    for (int idx = 0; idx < cachezHullStyleMA.Length; idx++)
                        if (cachezHullStyleMA[idx].Length == length && cachezHullStyleMA[idx].Type == type && cachezHullStyleMA[idx].EqualsInput(input))
                            return cachezHullStyleMA[idx];

                zHullStyleMA indicator = new zHullStyleMA();
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

                zHullStyleMA[] tmp = new zHullStyleMA[cachezHullStyleMA == null ? 1 : cachezHullStyleMA.Length + 1];
                if (cachezHullStyleMA != null)
                    cachezHullStyleMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezHullStyleMA = tmp;
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
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHullStyleMA zHullStyleMA(int length, RWT_MA.MAType type)
        {
            return _indicator.zHullStyleMA(Input, length, type);
        }

        /// <summary>
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        public Indicator.zHullStyleMA zHullStyleMA(Data.IDataSeries input, int length, RWT_MA.MAType type)
        {
            return _indicator.zHullStyleMA(input, length, type);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHullStyleMA zHullStyleMA(int length, RWT_MA.MAType type)
        {
            return _indicator.zHullStyleMA(Input, length, type);
        }

        /// <summary>
        /// hull-flavor ma
        /// </summary>
        /// <returns></returns>
        public Indicator.zHullStyleMA zHullStyleMA(Data.IDataSeries input, int length, RWT_MA.MAType type)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zHullStyleMA(input, length, type);
        }
    }
}
#endregion
