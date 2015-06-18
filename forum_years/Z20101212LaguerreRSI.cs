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
    /// laguerre rsi
    /// </summary>
    [Description("laguerre rsi")]
    public class Z20101212LaguerreRSI : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double gamma = 0.6; // Default setting for Gamma
		    private int len = 4;
			private Z20101212LaguerreFilter lf;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "LRSI"));
            Overlay				= false;
        }

	    protected override void OnStartUp() {
			lf = Z20101212LaguerreFilter(Input,gamma,len);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double cup = 0;
			double cdn = 0;
			
			lf.Update();
			
			for(int i = 0; i < (len-1); ++i) {
			  var diff = lf.filtered(i) - lf.filtered(i+1);
			  if(diff >= 0) cup += diff;
			  else cdn += -diff;
			}
			
			if((cup + cdn) > 0)
				Value.Set( cup/(cup+cdn) );
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LRSI
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return len; }
            set { len = value; }
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
        private Z20101212LaguerreRSI[] cacheZ20101212LaguerreRSI = null;

        private static Z20101212LaguerreRSI checkZ20101212LaguerreRSI = new Z20101212LaguerreRSI();

        /// <summary>
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreRSI Z20101212LaguerreRSI(double gamma, int length)
        {
            return Z20101212LaguerreRSI(Input, gamma, length);
        }

        /// <summary>
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreRSI Z20101212LaguerreRSI(Data.IDataSeries input, double gamma, int length)
        {
            if (cacheZ20101212LaguerreRSI != null)
                for (int idx = 0; idx < cacheZ20101212LaguerreRSI.Length; idx++)
                    if (Math.Abs(cacheZ20101212LaguerreRSI[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreRSI[idx].Length == length && cacheZ20101212LaguerreRSI[idx].EqualsInput(input))
                        return cacheZ20101212LaguerreRSI[idx];

            lock (checkZ20101212LaguerreRSI)
            {
                checkZ20101212LaguerreRSI.Gamma = gamma;
                gamma = checkZ20101212LaguerreRSI.Gamma;
                checkZ20101212LaguerreRSI.Length = length;
                length = checkZ20101212LaguerreRSI.Length;

                if (cacheZ20101212LaguerreRSI != null)
                    for (int idx = 0; idx < cacheZ20101212LaguerreRSI.Length; idx++)
                        if (Math.Abs(cacheZ20101212LaguerreRSI[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreRSI[idx].Length == length && cacheZ20101212LaguerreRSI[idx].EqualsInput(input))
                            return cacheZ20101212LaguerreRSI[idx];

                Z20101212LaguerreRSI indicator = new Z20101212LaguerreRSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Gamma = gamma;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20101212LaguerreRSI[] tmp = new Z20101212LaguerreRSI[cacheZ20101212LaguerreRSI == null ? 1 : cacheZ20101212LaguerreRSI.Length + 1];
                if (cacheZ20101212LaguerreRSI != null)
                    cacheZ20101212LaguerreRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20101212LaguerreRSI = tmp;
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
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreRSI Z20101212LaguerreRSI(double gamma, int length)
        {
            return _indicator.Z20101212LaguerreRSI(Input, gamma, length);
        }

        /// <summary>
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreRSI Z20101212LaguerreRSI(Data.IDataSeries input, double gamma, int length)
        {
            return _indicator.Z20101212LaguerreRSI(input, gamma, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreRSI Z20101212LaguerreRSI(double gamma, int length)
        {
            return _indicator.Z20101212LaguerreRSI(Input, gamma, length);
        }

        /// <summary>
        /// laguerre rsi
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreRSI Z20101212LaguerreRSI(Data.IDataSeries input, double gamma, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20101212LaguerreRSI(input, gamma, length);
        }
    }
}
#endregion
