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
    [Description("Laguerre-ized ergodic candle oscillator for zrich cumulative delta.")]
    public class ZRichDeltaECO : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double gamma = 0.300; // Default setting for Gamma
        // User defined variables (add any user defined variables below)
		    private WMA weightma;
		    private EMA expma;
		    private int smoothing = 8;
		    private int trigger = 5;
		    private double[] ol,pol,hl,phl,ll,pll,cl,pcl;
		    private DataSeries unsmoothed;
		    private int lastSeenBar = -1;
		   private rwt.IExtendedData extdat = null;
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
			CalculateOnBarClose=false;
        }

		protected override void OnStartUp() {
			weightma = null;
			expma = null;
			ol = new double[4];
			pol = new double[4];
			hl = new double[4];
			phl = new double[4];
			ll = new double[4];
			pll = new double[4];
			cl = new double[4];
			pcl = new double[4];
			unsmoothed = new DataSeries(this);
			lastSeenBar = -1;		
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
			
			if(CurrentBar != lastSeenBar) {
		      for(int i = 0; i < 4; ++i) {
			    pol[i] = ol[i]; // remember previous bar value...
			    phl[i] = hl[i]; 
			    pll[i] = ll[i]; 
			    pcl[i] = cl[i]; 
			  }
			  lastSeenBar = CurrentBar;
			}

			
			// update all the Laguerre numbers....
			ol[0] = (1-gamma)*ed.dOpen + gamma*pol[0];
			hl[0] = (1-gamma)*ed.dHigh + gamma*phl[0];
			ll[0] = (1-gamma)*ed.dLow + gamma*pll[0];
			cl[0] = (1-gamma)*ed.dClose + gamma*pcl[0];
			for(int i = 1; i < 4; ++i) {
			  ol[i] = -gamma*ol[i-1] + pol[i-1] + gamma*pol[i];
			  hl[i] = -gamma*hl[i-1] + phl[i-1] + gamma*phl[i];
			  ll[i] = -gamma*ll[i-1] + pll[i-1] + gamma*pll[i];
			  cl[i] = -gamma*cl[i-1] + pcl[i-1] + gamma*pcl[i];
			}
						
			double value1 = 0.0;
			double value2 = 0.0;
			for(int i = 0; i < 4; ++i) {
				value1 += (cl[i] - ol[i]);
				value2 += (hl[i] - ll[i]);
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
        private ZRichDeltaECO[] cacheZRichDeltaECO = null;

        private static ZRichDeltaECO checkZRichDeltaECO = new ZRichDeltaECO();

        /// <summary>
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        public ZRichDeltaECO ZRichDeltaECO(double gamma, int smoothing, int trigger)
        {
            return ZRichDeltaECO(Input, gamma, smoothing, trigger);
        }

        /// <summary>
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        public ZRichDeltaECO ZRichDeltaECO(Data.IDataSeries input, double gamma, int smoothing, int trigger)
        {
            if (cacheZRichDeltaECO != null)
                for (int idx = 0; idx < cacheZRichDeltaECO.Length; idx++)
                    if (Math.Abs(cacheZRichDeltaECO[idx].Gamma - gamma) <= double.Epsilon && cacheZRichDeltaECO[idx].Smoothing == smoothing && cacheZRichDeltaECO[idx].Trigger == trigger && cacheZRichDeltaECO[idx].EqualsInput(input))
                        return cacheZRichDeltaECO[idx];

            lock (checkZRichDeltaECO)
            {
                checkZRichDeltaECO.Gamma = gamma;
                gamma = checkZRichDeltaECO.Gamma;
                checkZRichDeltaECO.Smoothing = smoothing;
                smoothing = checkZRichDeltaECO.Smoothing;
                checkZRichDeltaECO.Trigger = trigger;
                trigger = checkZRichDeltaECO.Trigger;

                if (cacheZRichDeltaECO != null)
                    for (int idx = 0; idx < cacheZRichDeltaECO.Length; idx++)
                        if (Math.Abs(cacheZRichDeltaECO[idx].Gamma - gamma) <= double.Epsilon && cacheZRichDeltaECO[idx].Smoothing == smoothing && cacheZRichDeltaECO[idx].Trigger == trigger && cacheZRichDeltaECO[idx].EqualsInput(input))
                            return cacheZRichDeltaECO[idx];

                ZRichDeltaECO indicator = new ZRichDeltaECO();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Gamma = gamma;
                indicator.Smoothing = smoothing;
                indicator.Trigger = trigger;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichDeltaECO[] tmp = new ZRichDeltaECO[cacheZRichDeltaECO == null ? 1 : cacheZRichDeltaECO.Length + 1];
                if (cacheZRichDeltaECO != null)
                    cacheZRichDeltaECO.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichDeltaECO = tmp;
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
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichDeltaECO ZRichDeltaECO(double gamma, int smoothing, int trigger)
        {
            return _indicator.ZRichDeltaECO(Input, gamma, smoothing, trigger);
        }

        /// <summary>
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDeltaECO ZRichDeltaECO(Data.IDataSeries input, double gamma, int smoothing, int trigger)
        {
            return _indicator.ZRichDeltaECO(input, gamma, smoothing, trigger);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichDeltaECO ZRichDeltaECO(double gamma, int smoothing, int trigger)
        {
            return _indicator.ZRichDeltaECO(Input, gamma, smoothing, trigger);
        }

        /// <summary>
        /// Laguerre-ized ergodic candle oscillator for zrich cumulative delta.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDeltaECO ZRichDeltaECO(Data.IDataSeries input, double gamma, int smoothing, int trigger)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichDeltaECO(input, gamma, smoothing, trigger);
        }
    }
}
#endregion
