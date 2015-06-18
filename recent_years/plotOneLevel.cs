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
    /// plot an indicator on one level
    /// </summary>
    [Description("plot an indicator on one level")]
    public class plotOneLevel : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double level = 0.000; // Default setting for Level
			private bool goingUp = true;
			private double prevVal = 0.0;
			private Color upColor = Color.LimeGreen;
		 	private Color dnColor = Color.Red;
			private string alertSound = "NONE";
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Dot, "Plot"));
            Overlay				= false;
        }

		protected override void OnStartUp() {
			prevVal = Input[0];
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(Input[0] > prevVal) {
  			    if(!goingUp) if(alertSound.CompareTo("NONE")!=0) PlaySound(alertSound);				
				goingUp = true;	
			} else if(Input[0] < prevVal) {
				if(goingUp) if(alertSound.CompareTo("NONE")!=0) PlaySound(alertSound);
				goingUp = false;
			} 
			
            Plot.Set(level);
			PlotColors[0][0] = (goingUp?upColor:dnColor);
			prevVal = Input[0];
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot
        {
            get { return Values[0]; }
        }

        [Description("which level to plot")]
        [GridCategory("Parameters")]
        public double Level
        {
            get { return level; }
            set { level = value; }
        }
        [Description("alert sound?")]
        [GridCategory("Parameters")]
        public string AlertSound
        {
            get { return alertSound; }
            set { alertSound = value; }
        }
		
      [XmlIgnore]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Down color")]
        public Color DnColor
        {
            get { return dnColor; }
            set { dnColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string DnColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(dnColor); }
            set { dnColor = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Up color")]
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string UpColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(upColor); }
            set { upColor = Gui.Design.SerializableColor.FromString(value); }
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
        private plotOneLevel[] cacheplotOneLevel = null;

        private static plotOneLevel checkplotOneLevel = new plotOneLevel();

        /// <summary>
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        public plotOneLevel plotOneLevel(string alertSound, double level)
        {
            return plotOneLevel(Input, alertSound, level);
        }

        /// <summary>
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        public plotOneLevel plotOneLevel(Data.IDataSeries input, string alertSound, double level)
        {
            if (cacheplotOneLevel != null)
                for (int idx = 0; idx < cacheplotOneLevel.Length; idx++)
                    if (cacheplotOneLevel[idx].AlertSound == alertSound && Math.Abs(cacheplotOneLevel[idx].Level - level) <= double.Epsilon && cacheplotOneLevel[idx].EqualsInput(input))
                        return cacheplotOneLevel[idx];

            lock (checkplotOneLevel)
            {
                checkplotOneLevel.AlertSound = alertSound;
                alertSound = checkplotOneLevel.AlertSound;
                checkplotOneLevel.Level = level;
                level = checkplotOneLevel.Level;

                if (cacheplotOneLevel != null)
                    for (int idx = 0; idx < cacheplotOneLevel.Length; idx++)
                        if (cacheplotOneLevel[idx].AlertSound == alertSound && Math.Abs(cacheplotOneLevel[idx].Level - level) <= double.Epsilon && cacheplotOneLevel[idx].EqualsInput(input))
                            return cacheplotOneLevel[idx];

                plotOneLevel indicator = new plotOneLevel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertSound = alertSound;
                indicator.Level = level;
                Indicators.Add(indicator);
                indicator.SetUp();

                plotOneLevel[] tmp = new plotOneLevel[cacheplotOneLevel == null ? 1 : cacheplotOneLevel.Length + 1];
                if (cacheplotOneLevel != null)
                    cacheplotOneLevel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheplotOneLevel = tmp;
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
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.plotOneLevel plotOneLevel(string alertSound, double level)
        {
            return _indicator.plotOneLevel(Input, alertSound, level);
        }

        /// <summary>
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        public Indicator.plotOneLevel plotOneLevel(Data.IDataSeries input, string alertSound, double level)
        {
            return _indicator.plotOneLevel(input, alertSound, level);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.plotOneLevel plotOneLevel(string alertSound, double level)
        {
            return _indicator.plotOneLevel(Input, alertSound, level);
        }

        /// <summary>
        /// plot an indicator on one level
        /// </summary>
        /// <returns></returns>
        public Indicator.plotOneLevel plotOneLevel(Data.IDataSeries input, string alertSound, double level)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.plotOneLevel(input, alertSound, level);
        }
    }
}
#endregion
