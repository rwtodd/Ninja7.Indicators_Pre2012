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
    /// Counts the up and dn trades
    /// </summary>
    [Description("Counts the up and dn trades")]
    public class ZRichTradeCount : Indicator
    {
        #region Variables
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "UpCount"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "DnCount"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
			Plots[0].Pen.Width =3;
			Plots[1].Pen.Width =3;
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
			BarsRequired = 1;
        }

		protected override void OnStartUp() {
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType, fool!");
		}
				
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        { 
			var ed = extdat.getExtraData(0,Bars,CurrentBar);
			if(ed != null) {
  			  //Print("DT at "+idx+" is " + ed.dt);
			  UpCount.Set(ed.UpTicks);
			  DnCount.Set(-ed.DnTicks);
			}
		}

        #region Properties

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries UpCount
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DnCount
        {
            get { return Values[1]; }
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
        private ZRichTradeCount[] cacheZRichTradeCount = null;

        private static ZRichTradeCount checkZRichTradeCount = new ZRichTradeCount();

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichTradeCount ZRichTradeCount()
        {
            return ZRichTradeCount(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichTradeCount ZRichTradeCount(Data.IDataSeries input)
        {
            if (cacheZRichTradeCount != null)
                for (int idx = 0; idx < cacheZRichTradeCount.Length; idx++)
                    if (cacheZRichTradeCount[idx].EqualsInput(input))
                        return cacheZRichTradeCount[idx];

            lock (checkZRichTradeCount)
            {
                if (cacheZRichTradeCount != null)
                    for (int idx = 0; idx < cacheZRichTradeCount.Length; idx++)
                        if (cacheZRichTradeCount[idx].EqualsInput(input))
                            return cacheZRichTradeCount[idx];

                ZRichTradeCount indicator = new ZRichTradeCount();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichTradeCount[] tmp = new ZRichTradeCount[cacheZRichTradeCount == null ? 1 : cacheZRichTradeCount.Length + 1];
                if (cacheZRichTradeCount != null)
                    cacheZRichTradeCount.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichTradeCount = tmp;
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
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichTradeCount ZRichTradeCount()
        {
            return _indicator.ZRichTradeCount(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTradeCount ZRichTradeCount(Data.IDataSeries input)
        {
            return _indicator.ZRichTradeCount(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichTradeCount ZRichTradeCount()
        {
            return _indicator.ZRichTradeCount(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTradeCount ZRichTradeCount(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichTradeCount(input);
        }
    }
}
#endregion
