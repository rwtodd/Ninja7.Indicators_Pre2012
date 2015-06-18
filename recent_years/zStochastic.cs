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
    /// stochastic computation
    /// </summary>
    [Description("stochastic computation")]
    public class zStochastic : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int length = 10; // Default setting for Length
            private bool useJustInput = false; // Default setting for UseJustInput
        // User defined variables (add any user defined variables below)
			private zFastMAX zmax;
			private zFastMIN zmin;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Stoch"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			zmax = zFastMAX( (useJustInput?High:Input), length );
			zmin = zFastMIN( (useJustInput?Low:Input), length );
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var diff = zmax[0] - zmin[0];
			if(diff > 0.0) {
			  Stoch.Set((Input[0] - zmin[0])/diff);	
			} else {
			  Stoch.Set(0.5);	
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Stoch
        {
            get { return Values[0]; }
        }

        [Description("bars back to check")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }

        [Description("Use just the input, or do OHLC?")]
        [GridCategory("Parameters")]
        public bool UseJustInput
        {
            get { return useJustInput; }
            set { useJustInput = value; }
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
        private zStochastic[] cachezStochastic = null;

        private static zStochastic checkzStochastic = new zStochastic();

        /// <summary>
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        public zStochastic zStochastic(int length, bool useJustInput)
        {
            return zStochastic(Input, length, useJustInput);
        }

        /// <summary>
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        public zStochastic zStochastic(Data.IDataSeries input, int length, bool useJustInput)
        {
            if (cachezStochastic != null)
                for (int idx = 0; idx < cachezStochastic.Length; idx++)
                    if (cachezStochastic[idx].Length == length && cachezStochastic[idx].UseJustInput == useJustInput && cachezStochastic[idx].EqualsInput(input))
                        return cachezStochastic[idx];

            lock (checkzStochastic)
            {
                checkzStochastic.Length = length;
                length = checkzStochastic.Length;
                checkzStochastic.UseJustInput = useJustInput;
                useJustInput = checkzStochastic.UseJustInput;

                if (cachezStochastic != null)
                    for (int idx = 0; idx < cachezStochastic.Length; idx++)
                        if (cachezStochastic[idx].Length == length && cachezStochastic[idx].UseJustInput == useJustInput && cachezStochastic[idx].EqualsInput(input))
                            return cachezStochastic[idx];

                zStochastic indicator = new zStochastic();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.UseJustInput = useJustInput;
                Indicators.Add(indicator);
                indicator.SetUp();

                zStochastic[] tmp = new zStochastic[cachezStochastic == null ? 1 : cachezStochastic.Length + 1];
                if (cachezStochastic != null)
                    cachezStochastic.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezStochastic = tmp;
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
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zStochastic zStochastic(int length, bool useJustInput)
        {
            return _indicator.zStochastic(Input, length, useJustInput);
        }

        /// <summary>
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        public Indicator.zStochastic zStochastic(Data.IDataSeries input, int length, bool useJustInput)
        {
            return _indicator.zStochastic(input, length, useJustInput);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zStochastic zStochastic(int length, bool useJustInput)
        {
            return _indicator.zStochastic(Input, length, useJustInput);
        }

        /// <summary>
        /// stochastic computation
        /// </summary>
        /// <returns></returns>
        public Indicator.zStochastic zStochastic(Data.IDataSeries input, int length, bool useJustInput)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zStochastic(input, length, useJustInput);
        }
    }
}
#endregion
