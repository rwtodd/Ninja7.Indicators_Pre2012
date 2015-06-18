//
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
    /// </summary>
    [Description("zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.")]
    public class zHA2 : Indicator
    {
        #region Variables

        private Color           barColorDown         = Color.Red;
        private Color           barColorUp           = Color.Lime;
        private SolidBrush      brushDown            = null;
        private SolidBrush      brushUp              = null;
        private Color           shadowColor          = Color.Black;
        private Pen             shadowPen            = null;
        private int             shadowWidth          = 1;

		private bool toggleSwitch = true;
		private bool canseebars = true;
		private System.Windows.Forms.ToolStripButton hideBtn = null;
		private System.Windows.Forms.ToolStrip strip = null;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Gray, PlotStyle.Line, "HAOpen"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "HAClose"));
            PaintPriceMarkers   = false;
            CalculateOnBarClose = true;
            PlotsConfigurable   = false;
            Overlay             = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar == 0)
            {
                HAOpen.Set(Open[0]);
                HAClose.Set((High[0]+Low[0]+Close[0]+Open[0])*0.25);
                return;
            }

            HAClose.Set((High[0] + Low[0] + Close[0]+Open[0]) * 0.25); // Calculate the close
            HAOpen.Set((HAOpen[1] + HAClose[1]) * 0.5); // Calculate the open
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HAOpen
        {
            get { return Values[0]; }
        }

//        [Browsable(false)]
//        [XmlIgnore]
//        public DataSeries HAHigh
//        {
//            get { return Values[1]; }
//        }
//
//        [Browsable(false)]
//        [XmlIgnore]
//        public DataSeries HALow
//        {
//            get { return Values[2]; }
//        }

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HAClose
        {
            get { return Values[1]; /* [3] */ }
        }

        [XmlIgnore]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Down color")]
        public Color BarColorDown
        {
            get { return barColorDown; }
            set { barColorDown = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorDownSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorDown); }
            set { barColorDown = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Up color")]
        public Color BarColorUp
        {
            get { return barColorUp; }
            set { barColorUp = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorUpSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorUp); }
            set { barColorUp = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow color")]
        public Color ShadowColor
        {
            get { return shadowColor; }
            set { shadowColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string ShadowColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(shadowColor); }
            set { shadowColor = Gui.Design.SerializableColor.FromString(value); }
        }
		
	[Description("Include ability to hide ha bars?")]
	[GridCategory("Parameters")]
	public bool ToggleSwitch
	{
 	 get { return toggleSwitch; }
 	 set { toggleSwitch = value;
        }
	}      


        /// <summary>
        /// </summary>
        [Description("Width of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow width")]
        public int ShadowWidth
        {
            get { return shadowWidth; }
            set { shadowWidth = Math.Max(value, 1); }
        }

        #endregion

        #region Miscellaneous

        protected override void OnStartUp()
        {
			base.OnStartUp();
			
            if (ChartControl == null || Bars == null)
                return;

            brushUp                             = new SolidBrush(barColorUp);
            brushDown                           = new SolidBrush(barColorDown);
            shadowPen                           = new Pen(shadowColor, shadowWidth);
			
			if(toggleSwitch) {
				System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
				if(coll.Length > 0) {
					hideBtn = new System.Windows.Forms.ToolStripButton("-HA2-");
					hideBtn.Click += hideShowHA2;
					strip = (System.Windows.Forms.ToolStrip)coll[0];
					strip.Items.Add(hideBtn);   
					canseebars = true;
				}  
			}

        }

		private void hideShowHA2(object s, EventArgs e) {
			if(canseebars) {
				canseebars = false;
				hideBtn.Text = "+HA2+";
			} else {
				canseebars = true;
				hideBtn.Text = "-HA2-";
			}
			ChartControl.ChartPanel.Invalidate(false);

		}		
		
		
        protected override void OnTermination()
        {
            if (brushUp != null) brushUp.Dispose();
            if (brushDown != null) brushDown.Dispose();
            if (shadowPen != null) shadowPen.Dispose();
			
			if(strip != null) {
  				strip.Items.Remove(hideBtn);
				hideBtn.Dispose();
			}
			base.OnTermination();
        }


        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
            if (Bars == null || ChartControl == null || !canseebars)
                return;

            int barPaintWidth = Math.Max(3, 1 + 2 * ((int)Bars.BarsData.ChartStyle.BarWidth - 1) + 2 * shadowWidth);

            for (int idx = FirstBarIndexPainted; idx <= LastBarIndexPainted; idx++)
            {
                if (idx - Displacement < 0 || idx - Displacement >= BarsArray[0].Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
                    continue;
                double valC = HAClose.Get(idx);
                double valO = HAOpen.Get(idx);
                int x = ChartControl.GetXByBarIdx(BarsArray[0], idx);
                int y1 = ChartControl.GetYByValue(this, valO);
                int y4 = ChartControl.GetYByValue(this, valC);
				
                if (y4 == y1)
                    graphics.DrawLine(shadowPen, x - barPaintWidth / 2, y1, x + barPaintWidth / 2, y1);
                else
                {
                    if (y4 > y1)
                        graphics.FillRectangle(brushDown, x - barPaintWidth / 2, y1, barPaintWidth, y4 - y1);
                    else
                        graphics.FillRectangle(brushUp, x - barPaintWidth / 2, y4, barPaintWidth, y1 - y4);
                    graphics.DrawRectangle(shadowPen, (x - barPaintWidth / 2) + (shadowPen.Width / 2), Math.Min(y4, y1), barPaintWidth - shadowPen.Width, Math.Abs(y4 - y1));
                }
            }
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
        private zHA2[] cachezHA2 = null;

        private static zHA2 checkzHA2 = new zHA2();

        /// <summary>
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public zHA2 zHA2(bool toggleSwitch)
        {
            return zHA2(Input, toggleSwitch);
        }

        /// <summary>
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public zHA2 zHA2(Data.IDataSeries input, bool toggleSwitch)
        {
            if (cachezHA2 != null)
                for (int idx = 0; idx < cachezHA2.Length; idx++)
                    if (cachezHA2[idx].ToggleSwitch == toggleSwitch && cachezHA2[idx].EqualsInput(input))
                        return cachezHA2[idx];

            lock (checkzHA2)
            {
                checkzHA2.ToggleSwitch = toggleSwitch;
                toggleSwitch = checkzHA2.ToggleSwitch;

                if (cachezHA2 != null)
                    for (int idx = 0; idx < cachezHA2.Length; idx++)
                        if (cachezHA2[idx].ToggleSwitch == toggleSwitch && cachezHA2[idx].EqualsInput(input))
                            return cachezHA2[idx];

                zHA2 indicator = new zHA2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ToggleSwitch = toggleSwitch;
                Indicators.Add(indicator);
                indicator.SetUp();

                zHA2[] tmp = new zHA2[cachezHA2 == null ? 1 : cachezHA2.Length + 1];
                if (cachezHA2 != null)
                    cachezHA2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezHA2 = tmp;
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
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHA2 zHA2(bool toggleSwitch)
        {
            return _indicator.zHA2(Input, toggleSwitch);
        }

        /// <summary>
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public Indicator.zHA2 zHA2(Data.IDataSeries input, bool toggleSwitch)
        {
            return _indicator.zHA2(input, toggleSwitch);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zHA2 zHA2(bool toggleSwitch)
        {
            return _indicator.zHA2(Input, toggleSwitch);
        }

        /// <summary>
        /// zHA2 technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public Indicator.zHA2 zHA2(Data.IDataSeries input, bool toggleSwitch)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zHA2(input, toggleSwitch);
        }
    }
}
#endregion
