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
    public class ZRichIntMvmt : Indicator
    {
        #region Variables
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
		   private int lastSeen = -1;
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
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");
			//extdat = (NinjaTrader.Data.RichRange)(Bars.BarsType);
			lastSeen = -1;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        { 

			if(CurrentBar != lastSeen) {
				var olded = extdat.getExtraData(1,Bars,CurrentBar);
				lastSeen = CurrentBar;
			    if(olded != null) {
				  UpCount.Set(1,((double)olded.UpTicks)/Math.Max(olded.UpCount,1));
			      DnCount.Set(1,((double)(-olded.DnTicks))/Math.Max(olded.DnCount,1));
				  PlotColors[0][1] = Color.Green;
			      PlotColors[1][1] = Color.Red;
				}
			}
			
			var ed = extdat.getExtraData(0,Bars,CurrentBar);
			if(ed != null) {
			  UpCount.Set(((double)(ed.UpTicks))/Math.Max(ed.UpCount,1));
			  DnCount.Set(((double)(-ed.DnTicks))/Math.Max(ed.DnCount,1));
			  PlotColors[0][0] = Color.Cyan;
			  PlotColors[1][0] = Color.Magenta;
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
        private ZRichIntMvmt[] cacheZRichIntMvmt = null;

        private static ZRichIntMvmt checkZRichIntMvmt = new ZRichIntMvmt();

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichIntMvmt ZRichIntMvmt()
        {
            return ZRichIntMvmt(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichIntMvmt ZRichIntMvmt(Data.IDataSeries input)
        {
            if (cacheZRichIntMvmt != null)
                for (int idx = 0; idx < cacheZRichIntMvmt.Length; idx++)
                    if (cacheZRichIntMvmt[idx].EqualsInput(input))
                        return cacheZRichIntMvmt[idx];

            lock (checkZRichIntMvmt)
            {
                if (cacheZRichIntMvmt != null)
                    for (int idx = 0; idx < cacheZRichIntMvmt.Length; idx++)
                        if (cacheZRichIntMvmt[idx].EqualsInput(input))
                            return cacheZRichIntMvmt[idx];

                ZRichIntMvmt indicator = new ZRichIntMvmt();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichIntMvmt[] tmp = new ZRichIntMvmt[cacheZRichIntMvmt == null ? 1 : cacheZRichIntMvmt.Length + 1];
                if (cacheZRichIntMvmt != null)
                    cacheZRichIntMvmt.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichIntMvmt = tmp;
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
        public Indicator.ZRichIntMvmt ZRichIntMvmt()
        {
            return _indicator.ZRichIntMvmt(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichIntMvmt ZRichIntMvmt(Data.IDataSeries input)
        {
            return _indicator.ZRichIntMvmt(input);
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
        public Indicator.ZRichIntMvmt ZRichIntMvmt()
        {
            return _indicator.ZRichIntMvmt(Input);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichIntMvmt ZRichIntMvmt(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichIntMvmt(input);
        }
    }
}
#endregion
