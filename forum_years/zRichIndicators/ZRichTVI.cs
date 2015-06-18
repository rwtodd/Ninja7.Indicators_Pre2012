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
    public class ZRichTVI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int len1 = 21; // Default setting for Len1
            private int len2 = 5; // Default setting for Len2
		    private rwt.IExtendedData extdat = null;
		
		    private double alpha1, alpha2;
		    private double sm1, sm2;
		    private double osm1, osm2;
		    private int lastSeen;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "TVI"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Zero"));
            Overlay				= false;
			CalculateOnBarClose=false;
        }

		
		protected override void OnStartUp() {
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");

			sm1 = 0; sm2 = 0; osm1 = 0; osm2 = 0;	
			alpha1 = 2.0/(1.0+len1);
			alpha2 = 2.0/(1.0+len2);
			lastSeen = -1;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var ed = extdat.getExtraData(0,Bars,CurrentBar);
			if(ed == null) return;
			double denom = ed.UpTicks + ed.DnTicks;
			double unsm = 0;
			if(denom > 0) unsm = (ed.UpTicks - ed.DnTicks)/denom;
			
			if(lastSeen != CurrentBar) {
			   lastSeen = CurrentBar;
			   osm1 = sm1;
			   osm2 = sm2;
			}
			
			sm1 = osm1 + alpha1*(unsm - osm1);
			sm2 = osm2 + alpha2*(sm1 - osm2);
            TVI.Set(sm2);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TVI
        {
            get { return Values[0]; }
        }

        [Description("Smoothing1")]
        [GridCategory("Parameters")]
        public int Len1
        {
            get { return len1; }
            set { len1 = Math.Max(1, value); }
        }

        [Description("Smoothing2")]
        [GridCategory("Parameters")]
        public int Len2
        {
            get { return len2; }
            set { len2 = Math.Max(1, value); }
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
        private ZRichTVI[] cacheZRichTVI = null;

        private static ZRichTVI checkZRichTVI = new ZRichTVI();

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public ZRichTVI ZRichTVI(int len1, int len2)
        {
            return ZRichTVI(Input, len1, len2);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public ZRichTVI ZRichTVI(Data.IDataSeries input, int len1, int len2)
        {
            if (cacheZRichTVI != null)
                for (int idx = 0; idx < cacheZRichTVI.Length; idx++)
                    if (cacheZRichTVI[idx].Len1 == len1 && cacheZRichTVI[idx].Len2 == len2 && cacheZRichTVI[idx].EqualsInput(input))
                        return cacheZRichTVI[idx];

            lock (checkZRichTVI)
            {
                checkZRichTVI.Len1 = len1;
                len1 = checkZRichTVI.Len1;
                checkZRichTVI.Len2 = len2;
                len2 = checkZRichTVI.Len2;

                if (cacheZRichTVI != null)
                    for (int idx = 0; idx < cacheZRichTVI.Length; idx++)
                        if (cacheZRichTVI[idx].Len1 == len1 && cacheZRichTVI[idx].Len2 == len2 && cacheZRichTVI[idx].EqualsInput(input))
                            return cacheZRichTVI[idx];

                ZRichTVI indicator = new ZRichTVI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Len1 = len1;
                indicator.Len2 = len2;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichTVI[] tmp = new ZRichTVI[cacheZRichTVI == null ? 1 : cacheZRichTVI.Length + 1];
                if (cacheZRichTVI != null)
                    cacheZRichTVI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichTVI = tmp;
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
        public Indicator.ZRichTVI ZRichTVI(int len1, int len2)
        {
            return _indicator.ZRichTVI(Input, len1, len2);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTVI ZRichTVI(Data.IDataSeries input, int len1, int len2)
        {
            return _indicator.ZRichTVI(input, len1, len2);
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
        public Indicator.ZRichTVI ZRichTVI(int len1, int len2)
        {
            return _indicator.ZRichTVI(Input, len1, len2);
        }

        /// <summary>
        /// TVI
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTVI ZRichTVI(Data.IDataSeries input, int len1, int len2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichTVI(input, len1, len2);
        }
    }
}
#endregion
