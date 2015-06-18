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
    /// EMA version of keltner channels
    /// </summary>
    [Description("EMA version of keltner channels")]
    public class Z20090723KeltnerChannel : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int length = 55; // Default setting for Length
            private double bandFactor = 1.500; // Default setting for BandFactor
		    private int opacity = 2;
		    private DataSeries diff,upper,lower;
		    private string regiontag;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "MA"));
            // CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= true;
			
			diff				= new DataSeries(this);
			upper = new DataSeries(this,MaximumBarsLookBack.Infinite);
			lower = new DataSeries(this,MaximumBarsLookBack.Infinite);
			regiontag = "2009723kc"+length.ToString()+bandFactor.ToString();
		
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			diff.Set(High[0] - Low[0]);

			double middle	= EMA(Input,length)[0];
			double offset	= EMA(diff, length)[0] * bandFactor;

			upper.Set(middle + offset);
			lower.Set(middle - offset);

			DrawRegion(regiontag,CurrentBar,0,upper,lower,Plots[0].Pen.Color,Plots[0].Pen.Color,opacity);
            MA.Set(middle);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries MA
        {
            get { return Values[0]; }
        }

        [Description("Length of the MA")]
        [Category("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }
        [Description("How opaque is the band region?")]
        [Category("Parameters")]
        public int Opacity
        {
            get { return opacity; }
            set { opacity = Math.Max(1, value); }
        }

        [Description("Width of the band")]
        [Category("Parameters")]
        public double BandFactor
        {
            get { return bandFactor; }
            set { bandFactor = Math.Max(0.0, value); }
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
        private Z20090723KeltnerChannel[] cacheZ20090723KeltnerChannel = null;

        private static Z20090723KeltnerChannel checkZ20090723KeltnerChannel = new Z20090723KeltnerChannel();

        /// <summary>
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        public Z20090723KeltnerChannel Z20090723KeltnerChannel(double bandFactor, int length, int opacity)
        {
            return Z20090723KeltnerChannel(Input, bandFactor, length, opacity);
        }

        /// <summary>
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        public Z20090723KeltnerChannel Z20090723KeltnerChannel(Data.IDataSeries input, double bandFactor, int length, int opacity)
        {
            if (cacheZ20090723KeltnerChannel != null)
                for (int idx = 0; idx < cacheZ20090723KeltnerChannel.Length; idx++)
                    if (Math.Abs(cacheZ20090723KeltnerChannel[idx].BandFactor - bandFactor) <= double.Epsilon && cacheZ20090723KeltnerChannel[idx].Length == length && cacheZ20090723KeltnerChannel[idx].Opacity == opacity && cacheZ20090723KeltnerChannel[idx].EqualsInput(input))
                        return cacheZ20090723KeltnerChannel[idx];

            lock (checkZ20090723KeltnerChannel)
            {
                checkZ20090723KeltnerChannel.BandFactor = bandFactor;
                bandFactor = checkZ20090723KeltnerChannel.BandFactor;
                checkZ20090723KeltnerChannel.Length = length;
                length = checkZ20090723KeltnerChannel.Length;
                checkZ20090723KeltnerChannel.Opacity = opacity;
                opacity = checkZ20090723KeltnerChannel.Opacity;

                if (cacheZ20090723KeltnerChannel != null)
                    for (int idx = 0; idx < cacheZ20090723KeltnerChannel.Length; idx++)
                        if (Math.Abs(cacheZ20090723KeltnerChannel[idx].BandFactor - bandFactor) <= double.Epsilon && cacheZ20090723KeltnerChannel[idx].Length == length && cacheZ20090723KeltnerChannel[idx].Opacity == opacity && cacheZ20090723KeltnerChannel[idx].EqualsInput(input))
                            return cacheZ20090723KeltnerChannel[idx];

                Z20090723KeltnerChannel indicator = new Z20090723KeltnerChannel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandFactor = bandFactor;
                indicator.Length = length;
                indicator.Opacity = opacity;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20090723KeltnerChannel[] tmp = new Z20090723KeltnerChannel[cacheZ20090723KeltnerChannel == null ? 1 : cacheZ20090723KeltnerChannel.Length + 1];
                if (cacheZ20090723KeltnerChannel != null)
                    cacheZ20090723KeltnerChannel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20090723KeltnerChannel = tmp;
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
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20090723KeltnerChannel Z20090723KeltnerChannel(double bandFactor, int length, int opacity)
        {
            return _indicator.Z20090723KeltnerChannel(Input, bandFactor, length, opacity);
        }

        /// <summary>
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20090723KeltnerChannel Z20090723KeltnerChannel(Data.IDataSeries input, double bandFactor, int length, int opacity)
        {
            return _indicator.Z20090723KeltnerChannel(input, bandFactor, length, opacity);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20090723KeltnerChannel Z20090723KeltnerChannel(double bandFactor, int length, int opacity)
        {
            return _indicator.Z20090723KeltnerChannel(Input, bandFactor, length, opacity);
        }

        /// <summary>
        /// EMA version of keltner channels
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20090723KeltnerChannel Z20090723KeltnerChannel(Data.IDataSeries input, double bandFactor, int length, int opacity)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20090723KeltnerChannel(input, bandFactor, length, opacity);
        }
    }
}
#endregion
