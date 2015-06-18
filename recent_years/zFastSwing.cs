// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// The zFastSwing indicator plots lines that represents the swing high and low points.
    /// </summary>
    [Description("The zFastSwing indicator plots lines that represents the swing high and low points.")]
    [Gui.Design.DisplayName("zFastSwing (High/Low)")]
    public class zFastSwing : Indicator
    {
        #region Variables
		  private MAX mx;
		  private MIN mn;
		  private int strength = 5;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Dot, "zFastSwing high"));
            Add(new Plot(Color.Orange, PlotStyle.Dot, "zFastSwing low"));
			Plots[0].Pen.Width		= 2;
			Plots[0].Pen.DashStyle	= DashStyle.Dot;
			Plots[1].Pen.Width		= 2;
			Plots[1].Pen.DashStyle	= DashStyle.Dot;
            
			DisplayInDataBox	= false;
			PaintPriceMarkers	= false;
            Overlay				= true;
        }

		protected override void OnStartUp() {
			var period = 2*strength + 1;
			mx = MAX(High,period);
			mn = MIN(Low,period);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < strength) {
			  Values[0].Set(High[0]);
			  Values[1].Set(Low[0]);
			  return;
			}
			
			var potentialHigh=High[strength];
			var potentialLow=Low[strength];
			var ishigh = false;
			var islow = false;
			int i = 0; // counter
			if(mx[0] == potentialHigh) {
			  
//			  for(i = 0; i < strength; ++i)	
//				if(High[i] >= potentialHigh) {
//				  break;
//				}
//			  if(i == strength) {
				ishigh = true;
				for(i = 0; i <= strength; ++i)
					Values[0].Set(i,potentialHigh);
//			  }
			} 
			
			if(!ishigh) {
				if(Values[0].ContainsValue(1) && High[0] <= Values[0][1])
  					Values[0].Set(Values[0][1]);	
			}
			
			if(mn[0] == potentialLow) {
//			  for(i = 0; i < strength; ++i)	
//				if(Low[i] <= potentialLow) {
//				  break;
//				}
//			  if(i == strength) {
				islow = true;
				for(i = 0; i <= strength; ++i)
					Values[1].Set(i,potentialLow);
//			  }
			} 
			
			if(!islow) {
				if(Values[1].ContainsValue(1) && Low[0] >= Values[1][1])
				  Values[1].Set(Values[1][1]);	
			}
        }

        #region Functions

        #endregion

        #region Properties
        [Description("Number of bars required on each side of the swing point.")]
        [GridCategory("Parameters")]
        public int Strength
        {
            get { return strength; }
            set { strength = Math.Max(1, value); }
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
        private zFastSwing[] cachezFastSwing = null;

        private static zFastSwing checkzFastSwing = new zFastSwing();

        /// <summary>
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public zFastSwing zFastSwing(int strength)
        {
            return zFastSwing(Input, strength);
        }

        /// <summary>
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public zFastSwing zFastSwing(Data.IDataSeries input, int strength)
        {
            if (cachezFastSwing != null)
                for (int idx = 0; idx < cachezFastSwing.Length; idx++)
                    if (cachezFastSwing[idx].Strength == strength && cachezFastSwing[idx].EqualsInput(input))
                        return cachezFastSwing[idx];

            lock (checkzFastSwing)
            {
                checkzFastSwing.Strength = strength;
                strength = checkzFastSwing.Strength;

                if (cachezFastSwing != null)
                    for (int idx = 0; idx < cachezFastSwing.Length; idx++)
                        if (cachezFastSwing[idx].Strength == strength && cachezFastSwing[idx].EqualsInput(input))
                            return cachezFastSwing[idx];

                zFastSwing indicator = new zFastSwing();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Strength = strength;
                Indicators.Add(indicator);
                indicator.SetUp();

                zFastSwing[] tmp = new zFastSwing[cachezFastSwing == null ? 1 : cachezFastSwing.Length + 1];
                if (cachezFastSwing != null)
                    cachezFastSwing.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachezFastSwing = tmp;
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
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zFastSwing zFastSwing(int strength)
        {
            return _indicator.zFastSwing(Input, strength);
        }

        /// <summary>
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Indicator.zFastSwing zFastSwing(Data.IDataSeries input, int strength)
        {
            return _indicator.zFastSwing(input, strength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.zFastSwing zFastSwing(int strength)
        {
            return _indicator.zFastSwing(Input, strength);
        }

        /// <summary>
        /// The zFastSwing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Indicator.zFastSwing zFastSwing(Data.IDataSeries input, int strength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.zFastSwing(input, strength);
        }
    }
}
#endregion
