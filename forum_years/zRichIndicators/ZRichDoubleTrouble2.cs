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
    public class ZRichDoubleTrouble2 : Indicator
    {
        #region Variables
        // User defined variables (add any user defined variables below)
		   private rwt.IExtendedData extdat = null;
		   private int avgLen = 11;
		   private long lastUpCount, lastDnCount;
		   private DataSeries buytotals, selltotals,pones;
		   private Color dnlight, dndark, uplight, updark;
		   private Color uplight2 = Color.Aquamarine;
		   private Color dnlight2 = Color.Violet;
		   private Color updark2 = Color.Teal;
		   private Color dndark2 = Color.Purple;
		
		   private Bollinger b1,b2;
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
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "bband1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "bband2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "bband3"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "bband4"));
			
			Plots[0].Pen.Width = 4;
			Plots[2].Pen.Width = 4;
			Plots[1].Pen.Width = 2;
			Plots[3].Pen.Width = 2;
						
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
			buytotals = new DataSeries(this);
			selltotals = new DataSeries(this);
			pones = new DataSeries(this);
			b1 = Bollinger(pones,1.0,avgLen);
			b2 = Bollinger(pones,2.0,avgLen);
			uplight = Color.FromArgb(50,uplight2);
			dnlight = Color.FromArgb(50,dnlight2);
			updark = Color.FromArgb(50,updark2);
			dndark = Color.FromArgb(50,dndark2);			
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
			double p1 = (bvwap - svwap)/TickSize;
			if(p1*p1 > 100) p1 = 0.0;
			pones.Set(Math.Abs(p1));
			double denom = ed2.UpTicks + ed2.DnTicks;
			double pct = 0.5;
			if(denom > 0) pct = ed2.UpTicks/denom;
		
			if(pct > 0.65) {
				Values[0].Set(p1);
				Values[1].Reset();				
				Values[2].Reset();
				Values[3].Set(-p1);
			} else if(pct < 0.35) {
				Values[0].Reset();
				Values[1].Set(p1);
				Values[2].Set(-p1);
				Values[3].Reset();				
		    } else {
				Values[0].Reset();
				Values[1].Set(p1);
				Values[2].Reset();
				Values[3].Set(-p1);
			}

			var biasnum = 4;
			if(pones[0] >= pones[1]) BackColor = (biasgreen?uplight:dnlight);
			else BackColor = (biasgreen?updark:dndark);
			Values[4].Set(b1.Upper[0]);
			Values[5].Set(b2.Upper[0]);
			Values[6].Set(-b1.Upper[0]);
			Values[7].Set(-b2.Upper[0]);
		}

        #region Properties

        [Description("How much data to avg?")]
        [GridCategory("Parameters")]
        public int AvgLen
        {
            get { return avgLen; }
            set { avgLen = Math.Max(1,value); }
        }	

        [Description("up bias color")]
        [GridCategory("Parameters")]
        public Color ColorUpBiasDark	
        {
            get { return updark2; }
            set { updark2 = value; }
        }
        [Browsable(false)]
        public string updark2Serialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(updark2); }
           set { updark2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }		
        [Description("up bias color")]
        [GridCategory("Parameters")]
        public Color ColorUpBiasLight	
        {
            get { return uplight2; }
            set { uplight2 = value; }
        }
        [Browsable(false)]
        public string uplight2Serialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(uplight2); }
           set { uplight2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Description("dn bias color")]
        [GridCategory("Parameters")]
        public Color ColorDnBiasLight	
        {
            get { return dnlight2; }
            set { dnlight2 = value; }
        }
        [Browsable(false)]
        public string dnlight2Serialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dnlight2); }
           set { dnlight2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Description("dn bias color")]
        [GridCategory("Parameters")]
        public Color ColorDnBiasDark	
        {
            get { return dndark2; }
            set { dndark2 = value; }
        }
        [Browsable(false)]
        public string dndark2Serialize
        {
           get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dndark2); }
           set { dndark2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private ZRichDoubleTrouble2[] cacheZRichDoubleTrouble2 = null;

        private static ZRichDoubleTrouble2 checkZRichDoubleTrouble2 = new ZRichDoubleTrouble2();

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichDoubleTrouble2 ZRichDoubleTrouble2(int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            return ZRichDoubleTrouble2(Input, avgLen, colorDnBiasDark, colorDnBiasLight, colorUpBiasDark, colorUpBiasLight);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public ZRichDoubleTrouble2 ZRichDoubleTrouble2(Data.IDataSeries input, int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            if (cacheZRichDoubleTrouble2 != null)
                for (int idx = 0; idx < cacheZRichDoubleTrouble2.Length; idx++)
                    if (cacheZRichDoubleTrouble2[idx].AvgLen == avgLen && cacheZRichDoubleTrouble2[idx].ColorDnBiasDark == colorDnBiasDark && cacheZRichDoubleTrouble2[idx].ColorDnBiasLight == colorDnBiasLight && cacheZRichDoubleTrouble2[idx].ColorUpBiasDark == colorUpBiasDark && cacheZRichDoubleTrouble2[idx].ColorUpBiasLight == colorUpBiasLight && cacheZRichDoubleTrouble2[idx].EqualsInput(input))
                        return cacheZRichDoubleTrouble2[idx];

            lock (checkZRichDoubleTrouble2)
            {
                checkZRichDoubleTrouble2.AvgLen = avgLen;
                avgLen = checkZRichDoubleTrouble2.AvgLen;
                checkZRichDoubleTrouble2.ColorDnBiasDark = colorDnBiasDark;
                colorDnBiasDark = checkZRichDoubleTrouble2.ColorDnBiasDark;
                checkZRichDoubleTrouble2.ColorDnBiasLight = colorDnBiasLight;
                colorDnBiasLight = checkZRichDoubleTrouble2.ColorDnBiasLight;
                checkZRichDoubleTrouble2.ColorUpBiasDark = colorUpBiasDark;
                colorUpBiasDark = checkZRichDoubleTrouble2.ColorUpBiasDark;
                checkZRichDoubleTrouble2.ColorUpBiasLight = colorUpBiasLight;
                colorUpBiasLight = checkZRichDoubleTrouble2.ColorUpBiasLight;

                if (cacheZRichDoubleTrouble2 != null)
                    for (int idx = 0; idx < cacheZRichDoubleTrouble2.Length; idx++)
                        if (cacheZRichDoubleTrouble2[idx].AvgLen == avgLen && cacheZRichDoubleTrouble2[idx].ColorDnBiasDark == colorDnBiasDark && cacheZRichDoubleTrouble2[idx].ColorDnBiasLight == colorDnBiasLight && cacheZRichDoubleTrouble2[idx].ColorUpBiasDark == colorUpBiasDark && cacheZRichDoubleTrouble2[idx].ColorUpBiasLight == colorUpBiasLight && cacheZRichDoubleTrouble2[idx].EqualsInput(input))
                            return cacheZRichDoubleTrouble2[idx];

                ZRichDoubleTrouble2 indicator = new ZRichDoubleTrouble2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AvgLen = avgLen;
                indicator.ColorDnBiasDark = colorDnBiasDark;
                indicator.ColorDnBiasLight = colorDnBiasLight;
                indicator.ColorUpBiasDark = colorUpBiasDark;
                indicator.ColorUpBiasLight = colorUpBiasLight;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichDoubleTrouble2[] tmp = new ZRichDoubleTrouble2[cacheZRichDoubleTrouble2 == null ? 1 : cacheZRichDoubleTrouble2.Length + 1];
                if (cacheZRichDoubleTrouble2 != null)
                    cacheZRichDoubleTrouble2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichDoubleTrouble2 = tmp;
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
        public Indicator.ZRichDoubleTrouble2 ZRichDoubleTrouble2(int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            return _indicator.ZRichDoubleTrouble2(Input, avgLen, colorDnBiasDark, colorDnBiasLight, colorUpBiasDark, colorUpBiasLight);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDoubleTrouble2 ZRichDoubleTrouble2(Data.IDataSeries input, int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            return _indicator.ZRichDoubleTrouble2(input, avgLen, colorDnBiasDark, colorDnBiasLight, colorUpBiasDark, colorUpBiasLight);
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
        public Indicator.ZRichDoubleTrouble2 ZRichDoubleTrouble2(int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            return _indicator.ZRichDoubleTrouble2(Input, avgLen, colorDnBiasDark, colorDnBiasLight, colorUpBiasDark, colorUpBiasLight);
        }

        /// <summary>
        /// Counts the up and dn trades
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichDoubleTrouble2 ZRichDoubleTrouble2(Data.IDataSeries input, int avgLen, Color colorDnBiasDark, Color colorDnBiasLight, Color colorUpBiasDark, Color colorUpBiasLight)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichDoubleTrouble2(input, avgLen, colorDnBiasDark, colorDnBiasLight, colorUpBiasDark, colorUpBiasLight);
        }
    }
}
#endregion
