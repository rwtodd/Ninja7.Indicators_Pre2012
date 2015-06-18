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
    /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
    /// </summary>
    [Description("Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels")]
    public class zDexpMultiRes : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
			private zDexpSmooth[] smoothers;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaOne"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaTwo"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaThree"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaFour"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaFive"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaSix"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaSeven"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaEight"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Block, "AlphaNine"));
			for(int i = 0; i < 9; ++i) Plots[i].Pen.Width = 3;
            Overlay				= false;
        }

		protected override void OnStartUp() {
			smoothers = new zDexpSmooth[9];
			double[] alphas = { 5, 10, 20, 35, 50, 75, 100, 150, 200 };
			for(int i = 0; i < 9; ++i) {
				smoothers[i] = zDexpSmooth(2.0/(alphas[i]+1.0),1,true);
			}
		}
		
		private Color determineColor(double devs) {
			if(devs >= 0) {
				return Color.FromArgb(255, 0, (int)(Math.Min(devs/3.5,1.0)*255),0);	
			} else {
				return Color.FromArgb(255,(int)(Math.Min(-devs/3.5,1.0)*255),0,0);					
			}
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 6) return;
			
			var curHigh = High[0];
			var curLow  = Low[0];
			for(int i = 0; i < 9; ++i) {
				
				var diff = curHigh - smoothers[i][0];
				var diffLow = curLow - smoothers[i][0];
				if(Math.Abs(diffLow) > Math.Abs(diff)) diff = diffLow;
				var dev = Math.Max(smoothers[i].StdDev,0.000001);
				Values[i].Set(-i);
				PlotColors[i][0] = determineColor(diff/dev);
			}
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AlphaOne
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AlphaTwo
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AlphaThree
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AlphaFour
        {
            get { return Values[3]; }
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
        private zDexpMultiRes[] cachezDexpMultiRes = null;

        private static zDexpMultiRes checkzDexpMultiRes = new zDexpMultiRes();

        /// <summary>
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        public zDexpMultiRes zDexpMultiRes()
        {
            return zDexpMultiRes(Input);
        }

        /// <summary>
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        public zDexpMultiRes zDexpMultiRes(Data.IDataSeries input)
        {
            if (cachezDexpMultiRes != null)
                for (int idx = 0; idx < cachezDexpMultiRes.Length; idx++)
                    if (cachezDexpMultiRes[idx].EqualsInput(input))
                        return cachezDexpMultiRes[idx];

            lock (checkzDexpMultiRes)
            {
                if (cachezDexpMultiRes != null)
                    for (int idx = 0; idx < cachezDexpMultiRes.Length; idx++)
                        if (cachezDexpMultiRes[idx].EqualsInput(input))
                            return cachezDexpMultiRes[idx];

                zDexpMultiRes indicator = new zDexpMultiRes();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                zDexpMultiRes[] tmp = new zDexpMultiRes[cachezDexpMultiRes == null ? 1 : cachezDexpMultiRes.Length + 1];
                if (cachezDexpMultiRes != null)
                    cachezDexpMultiRes.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezDexpMultiRes = tmp;
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
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpMultiRes zDexpMultiRes()
        {
            return _indicator.zDexpMultiRes(Input);
        }

        /// <summary>
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpMultiRes zDexpMultiRes(Data.IDataSeries input)
        {
            return _indicator.zDexpMultiRes(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zDexpMultiRes zDexpMultiRes()
        {
            return _indicator.zDexpMultiRes(Input);
        }

        /// <summary>
        /// Dexp Multiresolution analysis of price relative to Std Deviation bands at multiple Alpha-levels
        /// </summary>
        /// <returns></returns>
        public Indicator.zDexpMultiRes zDexpMultiRes(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zDexpMultiRes(input);
        }
    }
}
#endregion
