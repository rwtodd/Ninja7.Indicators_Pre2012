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
    /// TVI
    /// </summary>
    [Description("TVI")]
    public class ZRichCDeltaLine : Indicator
    {
        #region Variables
        // Wizard generated variables
		    private rwt.IExtendedData extdat = null;
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "Cumulative Delta"));
            Overlay				= false;
			CalculateOnBarClose=false;
        }

		
		protected override void OnStartUp() {
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");

		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var ed = extdat.getExtraData(0,Bars,CurrentBar);
			if(ed == null) return;
			
				
            Value.Set(ed.dClose);
        }

        #region Properties
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private ZRichCDeltaLine[] cacheZRichCDeltaLine = null;

        private static ZRichCDeltaLine checkZRichCDeltaLine = new ZRichCDeltaLine();

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public ZRichCDeltaLine ZRichCDeltaLine()
        {
            return ZRichCDeltaLine(Input);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public ZRichCDeltaLine ZRichCDeltaLine(Data.IDataSeries input)
        {
            if (cacheZRichCDeltaLine != null)
                for (int idx = 0; idx < cacheZRichCDeltaLine.Length; idx++)
                    if (cacheZRichCDeltaLine[idx].EqualsInput(input))
                        return cacheZRichCDeltaLine[idx];

            lock (checkZRichCDeltaLine)
            {
                if (cacheZRichCDeltaLine != null)
                    for (int idx = 0; idx < cacheZRichCDeltaLine.Length; idx++)
                        if (cacheZRichCDeltaLine[idx].EqualsInput(input))
                            return cacheZRichCDeltaLine[idx];

                ZRichCDeltaLine indicator = new ZRichCDeltaLine();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichCDeltaLine[] tmp = new ZRichCDeltaLine[cacheZRichCDeltaLine == null ? 1 : cacheZRichCDeltaLine.Length + 1];
                if (cacheZRichCDeltaLine != null)
                    cacheZRichCDeltaLine.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichCDeltaLine = tmp;
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
        /// TVI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichCDeltaLine ZRichCDeltaLine()
        {
            return _indicator.ZRichCDeltaLine(Input);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichCDeltaLine ZRichCDeltaLine(Data.IDataSeries input)
        {
            return _indicator.ZRichCDeltaLine(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichCDeltaLine ZRichCDeltaLine()
        {
            return _indicator.ZRichCDeltaLine(Input);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichCDeltaLine ZRichCDeltaLine(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichCDeltaLine(input);
        }
    }
}
#endregion
