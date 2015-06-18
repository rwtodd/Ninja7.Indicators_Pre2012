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
    public class ZRichDoubleTrouble : Indicator
    {
        #region Variables
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
		   private int avgLen = 11;
		   private long lastUpCount, lastDnCount;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Lime), PlotStyle.Bar, "Above1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "Above2"));
			
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "Below1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), PlotStyle.Bar, "Below2"));

			Add(new Plot(Color.FromKnownColor(KnownColor.Lime), PlotStyle.Dot, "BiasUp1"));
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Dot, "BiasUp2"));
			
			Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Dot, "BiasDn1"));
			Add(new Plot(Color.FromKnownColor(KnownColor.DarkRed), PlotStyle.Dot, "BiasDn2"));
			
			Plots[0].Pen.Width = 3;
			Plots[2].Pen.Width = 3;
			Plots[1].Pen.Width = 1;
			Plots[3].Pen.Width = 1;
			Plots[4].Pen.Width = 3;
			Plots[5].Pen.Width = 3;
			Plots[6].Pen.Width = 3;
			Plots[7].Pen.Width = 3;
						
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
			BarsRequired = 1;
			
        }

		protected override void OnStartUp() {
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType, fool!");
			lastUpCount = 0;
			lastDnCount = 0;
		}
				
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        { 			
			double bvwap = 0;  double bvol = 0;
			double svwap = 0;  double svol = 0;
			double tvwap = 0;  double tvol = 0;
			for(int i = 0; i < avgLen; ++i) {
				var ed = extdat.getExtraData(i,Bars,CurrentBar);
				if(ed == null) break;
				bvwap += ed.UpTicks*Median[i];
				svwap += ed.DnTicks*Median[i];
				tvwap += Volume[i]*Median[i];
				bvol += ed.UpTicks;
				svol += ed.DnTicks;
				tvol += Volume[i];
			}
			if(bvol > 0) bvwap /= bvol;
			if(svol > 0) svwap /= svol;
			if(tvol > 0) tvwap /= tvol;
			 
			var ed2 = extdat.getExtraData(0,Bars,CurrentBar);
		    if(ed2 == null) return;
			double diff = Math.Abs(tvwap - bvwap) - Math.Abs(tvwap - svwap);
			bool biasgreen = true;
			if(diff > 0) { biasgreen = false; }
			else if(diff == 0) {
				biasgreen = ed2.UpTicks >= ed2.DnTicks;
			}
			double p1 = 0.5*(bvwap - svwap);
			if(ed2.UpTicks > ed2.DnTicks) {
			  	Values[0].Set(p1);
				Values[3].Set(-p1);
				if(biasgreen)  {Values[4].Set(0); } else { Values[7].Set(0); }
			} else if(ed2.UpTicks == ed2.DnTicks) {
				Values[1].Set(p1);
				Values[3].Set(-p1);
				if(biasgreen)  {Values[4].Set(0); } else { Values[7].Set(0); }
		    } else {
				Values[1].Set(p1);
				Values[2].Set(-p1);				
				if(biasgreen)  {Values[5].Set(0); } else { Values[6].Set(0); }
			}			
		}

        #region Properties

        [Description("How much data to avg?")]
        [GridCategory("Parameters")]
        public int AvgLen
        {
            get { return avgLen; }
            set { avgLen = Math.Max(1,value); }
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
        private ZRichDoubleTrouble[] cacheZRichDoubleTrouble = null;

        private static ZRichDoubleTrouble checkZRichDoubleTrouble = new ZRichDoubleTrouble();

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichDoubleTrouble ZRichDoubleTrouble(int avgLen)
        {
            return ZRichDoubleTrouble(Input, avgLen);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichDoubleTrouble ZRichDoubleTrouble(Data.IDataSeries input, int avgLen)
        {
            if (cacheZRichDoubleTrouble != null)
                for (int idx = 0; idx < cacheZRichDoubleTrouble.Length; idx++)
                    if (cacheZRichDoubleTrouble[idx].AvgLen == avgLen && cacheZRichDoubleTrouble[idx].EqualsInput(input))
                        return cacheZRichDoubleTrouble[idx];

            lock (checkZRichDoubleTrouble)
            {
                checkZRichDoubleTrouble.AvgLen = avgLen;
                avgLen = checkZRichDoubleTrouble.AvgLen;

                if (cacheZRichDoubleTrouble != null)
                    for (int idx = 0; idx < cacheZRichDoubleTrouble.Length; idx++)
                        if (cacheZRichDoubleTrouble[idx].AvgLen == avgLen && cacheZRichDoubleTrouble[idx].EqualsInput(input))
                            return cacheZRichDoubleTrouble[idx];

                ZRichDoubleTrouble indicator = new ZRichDoubleTrouble();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AvgLen = avgLen;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichDoubleTrouble[] tmp = new ZRichDoubleTrouble[cacheZRichDoubleTrouble == null ? 1 : cacheZRichDoubleTrouble.Length + 1];
                if (cacheZRichDoubleTrouble != null)
                    cacheZRichDoubleTrouble.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichDoubleTrouble = tmp;
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
        public Indicator.ZRichDoubleTrouble ZRichDoubleTrouble(int avgLen)
        {
            return _indicator.ZRichDoubleTrouble(Input, avgLen);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDoubleTrouble ZRichDoubleTrouble(Data.IDataSeries input, int avgLen)
        {
            return _indicator.ZRichDoubleTrouble(input, avgLen);
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
        public Indicator.ZRichDoubleTrouble ZRichDoubleTrouble(int avgLen)
        {
            return _indicator.ZRichDoubleTrouble(Input, avgLen);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDoubleTrouble ZRichDoubleTrouble(Data.IDataSeries input, int avgLen)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichDoubleTrouble(input, avgLen);
        }
    }
}
#endregion
