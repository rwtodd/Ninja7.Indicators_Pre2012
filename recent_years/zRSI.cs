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
    /// my rsi
    /// </summary>
    [Description("my rsi")]
    public class zRSI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double alphaUD = 2.0/28.0; // Default setting for AlphaUD
            private double alphaTLine = 0.5; // Default setting for AlphaTLine
			private double u = 0.0,d = 0.0000001;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumPurple), PlotStyle.Line, "RSI"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Goldenrod), PlotStyle.Line, "Trigger"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 70, "OverBought"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 30, "OverSold"));
            Add(new Line(Color.FromKnownColor(KnownColor.Gray), 50, "Midln"));
            Overlay				= false;
			Lines[2].Pen.DashStyle = DashStyle.Dash;
			Plots[0].Pen.Width = 2;
        }

		protected override void OnStartUp() {
			u = 0.0;
			d = 0.0000001;
	    }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar == 0) { RSIVal.Set(50.0); Trigger.Set(50); return; }
			
			var diff = Input[0] - Input[1];
		    u += alphaUD*(Math.Max(diff,0.0)-u);
			d -= alphaUD*(Math.Min(diff,0.0)+d);			
            RSIVal.Set(100.0 - 100.0/(1.0 + u/d) );			
            Trigger.Set(Trigger[1] + alphaTLine*(RSIVal[0]-Trigger[1]));
        }
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RSIVal
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Trigger
        {
            get { return Values[1]; }
        }

        [Description("smoothing of the U and D avgs")]
        [GridCategory("Parameters")]
        public double AlphaUD
        {
            get { return alphaUD; }
            set { if(value > 1) value = 2.0/(value+1.0);
                  alphaUD = Math.Max(0.000, value); }
        }

        [Description("smoothing to make a trigger line")]
        [GridCategory("Parameters")]
        public double AlphaTLine
        {
            get { return alphaTLine; }
            set { if(value > 1) value = 2.0/(value+1.0);
			      alphaTLine = Math.Max(0.0,value); }
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
        private zRSI[] cachezRSI = null;

        private static zRSI checkzRSI = new zRSI();

        /// <summary>
        /// my rsi
        /// </summary>
        /// <returns></returns>
        public zRSI zRSI(double alphaTLine, double alphaUD)
        {
            return zRSI(Input, alphaTLine, alphaUD);
        }

        /// <summary>
        /// my rsi
        /// </summary>
        /// <returns></returns>
        public zRSI zRSI(Data.IDataSeries input, double alphaTLine, double alphaUD)
        {
            if (cachezRSI != null)
                for (int idx = 0; idx < cachezRSI.Length; idx++)
                    if (Math.Abs(cachezRSI[idx].AlphaTLine - alphaTLine) <= double.Epsilon && Math.Abs(cachezRSI[idx].AlphaUD - alphaUD) <= double.Epsilon && cachezRSI[idx].EqualsInput(input))
                        return cachezRSI[idx];

            lock (checkzRSI)
            {
                checkzRSI.AlphaTLine = alphaTLine;
                alphaTLine = checkzRSI.AlphaTLine;
                checkzRSI.AlphaUD = alphaUD;
                alphaUD = checkzRSI.AlphaUD;

                if (cachezRSI != null)
                    for (int idx = 0; idx < cachezRSI.Length; idx++)
                        if (Math.Abs(cachezRSI[idx].AlphaTLine - alphaTLine) <= double.Epsilon && Math.Abs(cachezRSI[idx].AlphaUD - alphaUD) <= double.Epsilon && cachezRSI[idx].EqualsInput(input))
                            return cachezRSI[idx];

                zRSI indicator = new zRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlphaTLine = alphaTLine;
                indicator.AlphaUD = alphaUD;
                Indicators.Add(indicator);
                indicator.SetUp();

                zRSI[] tmp = new zRSI[cachezRSI == null ? 1 : cachezRSI.Length + 1];
                if (cachezRSI != null)
                    cachezRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezRSI = tmp;
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
        /// my rsi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRSI zRSI(double alphaTLine, double alphaUD)
        {
            return _indicator.zRSI(Input, alphaTLine, alphaUD);
        }

        /// <summary>
        /// my rsi
        /// </summary>
        /// <returns></returns>
        public Indicator.zRSI zRSI(Data.IDataSeries input, double alphaTLine, double alphaUD)
        {
            return _indicator.zRSI(input, alphaTLine, alphaUD);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// my rsi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zRSI zRSI(double alphaTLine, double alphaUD)
        {
            return _indicator.zRSI(Input, alphaTLine, alphaUD);
        }

        /// <summary>
        /// my rsi
        /// </summary>
        /// <returns></returns>
        public Indicator.zRSI zRSI(Data.IDataSeries input, double alphaTLine, double alphaUD)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zRSI(input, alphaTLine, alphaUD);
        }
    }
}
#endregion
