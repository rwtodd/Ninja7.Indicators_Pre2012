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
    /// Laguerre-ized ergodic candle oscillator
    /// </summary>
    [Description("EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.")]
    public class Z20101212LaguerreECO2 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double gamma = 0.300; // Default setting for Gamma
        // User defined variables (add any user defined variables below)
		    private WMA weightma;
		    private EMA expma;
		    private int len = 4;
		    private int smoothing = 8;
		    private int trigger = 5;
			private Z20101212LaguerreFilter o = null;
			private Z20101212LaguerreFilter h = null;
			private Z20101212LaguerreFilter l = null;
			private Z20101212LaguerreFilter c = null;
		    private DataSeries unsmoothed;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "ECO"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gold), PlotStyle.Line, "Trigger"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Cyan), PlotStyle.Bar, "Histogram"));
            Add(new Line(Color.FromKnownColor(KnownColor.Blue), 0, "Zero"));
            Overlay				= false;
            PriceTypeSupported	= false;
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 2;
			weightma = null;
			expma = null;
			unsmoothed = new DataSeries(this);
        }

		protected override void OnStartUp() {
			o = Z20101212LaguerreFilter(Open,gamma,len);
			h = Z20101212LaguerreFilter(High,gamma,len);
			l = Z20101212LaguerreFilter(Low,gamma,len);
			c = Z20101212LaguerreFilter(Close,gamma,len);
		
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
						
			double value1 = 0.0;
			double value2 = 0.0;
			
			o.Update();
			h.Update();
			l.Update();
			c.Update();
			
			for(int i = 0; i < len; ++i) {
				value1 += (c.filtered(i) - o.filtered(i));
				value2 += (h.filtered(i) - l.filtered(i));
			}
			
			if(value2 != 0.0) {
			  unsmoothed.Set(100.0*value1/value2);	
			} else {
			  if((CurrentBar > 1) && unsmoothed.ContainsValue(1)) {
				unsmoothed.Set(unsmoothed[1]);
			  } else {
			    unsmoothed.Set(0);	
			  }
			}
			
			if(weightma == null) {
				weightma = WMA(unsmoothed,smoothing);
				expma = EMA(weightma.Value,trigger);
			}
			
			ECO.Set(weightma[0]);
			TriggerLine.Set(expma[0]);
			Histogram.Set(weightma[0] - expma[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ECO
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries TriggerLine
        {
            get { return Values[1]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Histogram
        {
            get { return Values[2]; }
        }

        [Description("gamma")]
        [Category("Parameters")]
        public double Gamma
        {
            get { return gamma; }
            set { gamma = Math.Max(0.000, value); }
        }
        [Description("length")]
        [Category("Parameters")]
        public int Length
        {
            get { return len; }
            set { len = value; }
        }
        [Description("smoothing")]
        [Category("Parameters")]
        public int Smoothing
        {
            get { return smoothing; }
            set { smoothing = Math.Max(1, value); }
        }
        [Description("trigger")]
        [Category("Parameters")]
        public int Trigger
        {
            get { return trigger; }
            set { trigger = Math.Max(1, value); }
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
        private Z20101212LaguerreECO2[] cacheZ20101212LaguerreECO2 = null;

        private static Z20101212LaguerreECO2 checkZ20101212LaguerreECO2 = new Z20101212LaguerreECO2();

        /// <summary>
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreECO2 Z20101212LaguerreECO2(double gamma, int length, int smoothing, int trigger)
        {
            return Z20101212LaguerreECO2(Input, gamma, length, smoothing, trigger);
        }

        /// <summary>
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        public Z20101212LaguerreECO2 Z20101212LaguerreECO2(Data.IDataSeries input, double gamma, int length, int smoothing, int trigger)
        {
            if (cacheZ20101212LaguerreECO2 != null)
                for (int idx = 0; idx < cacheZ20101212LaguerreECO2.Length; idx++)
                    if (Math.Abs(cacheZ20101212LaguerreECO2[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreECO2[idx].Length == length && cacheZ20101212LaguerreECO2[idx].Smoothing == smoothing && cacheZ20101212LaguerreECO2[idx].Trigger == trigger && cacheZ20101212LaguerreECO2[idx].EqualsInput(input))
                        return cacheZ20101212LaguerreECO2[idx];

            lock (checkZ20101212LaguerreECO2)
            {
                checkZ20101212LaguerreECO2.Gamma = gamma;
                gamma = checkZ20101212LaguerreECO2.Gamma;
                checkZ20101212LaguerreECO2.Length = length;
                length = checkZ20101212LaguerreECO2.Length;
                checkZ20101212LaguerreECO2.Smoothing = smoothing;
                smoothing = checkZ20101212LaguerreECO2.Smoothing;
                checkZ20101212LaguerreECO2.Trigger = trigger;
                trigger = checkZ20101212LaguerreECO2.Trigger;

                if (cacheZ20101212LaguerreECO2 != null)
                    for (int idx = 0; idx < cacheZ20101212LaguerreECO2.Length; idx++)
                        if (Math.Abs(cacheZ20101212LaguerreECO2[idx].Gamma - gamma) <= double.Epsilon && cacheZ20101212LaguerreECO2[idx].Length == length && cacheZ20101212LaguerreECO2[idx].Smoothing == smoothing && cacheZ20101212LaguerreECO2[idx].Trigger == trigger && cacheZ20101212LaguerreECO2[idx].EqualsInput(input))
                            return cacheZ20101212LaguerreECO2[idx];

                Z20101212LaguerreECO2 indicator = new Z20101212LaguerreECO2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Gamma = gamma;
                indicator.Length = length;
                indicator.Smoothing = smoothing;
                indicator.Trigger = trigger;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20101212LaguerreECO2[] tmp = new Z20101212LaguerreECO2[cacheZ20101212LaguerreECO2 == null ? 1 : cacheZ20101212LaguerreECO2.Length + 1];
                if (cacheZ20101212LaguerreECO2 != null)
                    cacheZ20101212LaguerreECO2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20101212LaguerreECO2 = tmp;
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
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreECO2 Z20101212LaguerreECO2(double gamma, int length, int smoothing, int trigger)
        {
            return _indicator.Z20101212LaguerreECO2(Input, gamma, length, smoothing, trigger);
        }

        /// <summary>
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreECO2 Z20101212LaguerreECO2(Data.IDataSeries input, double gamma, int length, int smoothing, int trigger)
        {
            return _indicator.Z20101212LaguerreECO2(input, gamma, length, smoothing, trigger);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20101212LaguerreECO2 Z20101212LaguerreECO2(double gamma, int length, int smoothing, int trigger)
        {
            return _indicator.Z20101212LaguerreECO2(Input, gamma, length, smoothing, trigger);
        }

        /// <summary>
        /// EOTProlive.com FREE indicator.  Laguerre-ized ergodic candle oscillator.
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20101212LaguerreECO2 Z20101212LaguerreECO2(Data.IDataSeries input, double gamma, int length, int smoothing, int trigger)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20101212LaguerreECO2(input, gamma, length, smoothing, trigger);
        }
    }
}
#endregion
