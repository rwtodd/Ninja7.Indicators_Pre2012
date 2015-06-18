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
    /// Draws virtual bars
    /// </summary>
    [Description("Richard Todd www.movethemarkets.com Draws virtual bars")]
    public class Z20091123VirtualBars : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int grouping = 30; // Default setting for Grouping
		    private EventGroupingMethod groupBy = EventGroupingMethod.Minutes;
			private bool boxStyle = false;
		
			private Z20091203PeriodicEvent pe;
		
            private int startTime = 830; // Default setting for StartTime
        // User defined variables (add any user defined variables below)
		    private DataSeries mxSeen,mnSeen,opSeen,clSeen;
		    private int lastBarSeen = -1;

		    private bool gapless = false;

            private Color colorBullish = Color.Green;
            private Color colorBearish = Color.Red;

		    // to draw candle outlines...
		    private Pen bullPen,bearPen;
		    private SolidBrush bullBrush,bearBrush;
			private int penWidth = 3;
		    private int candleAlpha = 255;
		    
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
			mxSeen = new DataSeries(this,MaximumBarsLookBack.Infinite);
			mnSeen = new DataSeries(this,MaximumBarsLookBack.Infinite);
			opSeen = new DataSeries(this,MaximumBarsLookBack.Infinite);
			clSeen = new DataSeries(this,MaximumBarsLookBack.Infinite);
			
			bullBrush = new SolidBrush(Color.FromArgb(candleAlpha,colorBullish) );
			bullPen = new Pen( bullBrush, penWidth );
			bearBrush = new SolidBrush(Color.FromArgb(candleAlpha,colorBearish) );
			bearPen = new Pen( bearBrush, penWidth );
			pe = null;
        }
		
	
	
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(pe == null) pe = Z20091203PeriodicEvent(groupBy,grouping,startTime);
			if(CurrentBar < 2) { 
				mxSeen.Set(High[0]); 
				mnSeen.Set(Low[0]); 
				opSeen.Set(Open[0]);
				clSeen.Set(Close[0]);
				return; 
			}

			// do book-keeping on new bars...
			if(CurrentBar != lastBarSeen) {
			  lastBarSeen = CurrentBar;
				
			  //Print("Bar: "+CurrentBar+" time: "+Time[0]+" tgt: "+tgtTime+ " sess: "+sessionTime);
			
			  // determine if it's time for a new bar...
			  bool barReset = pe.IsNewBar[0]; 
			  bool sessionStart = pe.IsNewSession[0];
			
  			  if(barReset || sessionStart) {
			    // we need to start fresh...
				mxSeen.Set(High[0]);
				mnSeen.Set(Low[0]);
				opSeen.Set(Open[0]);
				
				// for gapless bars at session start, you need to
				// set the open to the previous close, just this one time
				if(sessionStart && gapless) {
				  opSeen.Set(Close[1]);
				  if(Close[1] < Low[0]) mnSeen.Set(Close[1]);
				  if(Close[1] > High[0]) mxSeen.Set(Close[1]);
				}
								
			  } else {
			    mxSeen.Set(mxSeen[1]);
				mnSeen.Set(mnSeen[1]);
				opSeen.Set(opSeen[1]);
			  }
			}
			
			// update the max and min that we've seen so far...
			mxSeen.Set(Math.Max(mxSeen[0],High[0]));
			mnSeen.Set(Math.Min(mnSeen[0],Low[0]));
			
			// update the close
			clSeen.Set(Close[0]);
        }


public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)

{

int barPaintWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
int spacing = (ChartControl.BarSpace - barPaintWidth) / 2 - 1;
if(spacing < 0) spacing = 0;

int bars = ChartControl.BarsPainted;

int index;

float x1, y1, y2, y3, y4;

float lastX=0;

float lastY=0;


Exception caughtException;
while (bars >= 0)

{

index = ChartControl.LastBarPainted - ChartControl.BarsPainted + bars;
if (ChartControl.ShowBarsRequired || ((index - base.Displacement) >= base.BarsRequired))

{
  // only draw the candle ends...
  if( (index > 0) && 
	  ( (index == CurrentBar)  || (pe.BarIndex.Get(index+1) <= pe.BarIndex.Get(index)) ) ) {
		
try

{
int multibarWidth = (ChartControl.BarSpace * (pe.BarIndex.Get(index))) + barPaintWidth;

x1 = (((ChartControl.CanvasRight - ChartControl.BarMarginRight) - (multibarWidth / 2)) - ((ChartControl.BarsPainted - 1) * ChartControl.BarSpace)) + ((bars-1) * (base.ChartControl.BarSpace)) - penWidth/2;

y1 = (bounds.Y + bounds.Height) - ((int) ((((double)mxSeen.Get(index) - min) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height));

y2 = (bounds.Y + bounds.Height) - ((int) ((((double)clSeen.Get(index) - min) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height));

y3 = (bounds.Y + bounds.Height) - ((int) ((((double)opSeen.Get(index) - min) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height));

y4 = (bounds.Y + bounds.Height) - ((int) ((((double)mnSeen.Get(index) - min) / Gui.Chart.ChartControl.MaxMinusMin(max, min)) * bounds.Height));


if ( y2 > y3 )

{

if(!boxStyle) {
  graphics.DrawRectangle(bearPen,(int)x1-multibarWidth/2-spacing, (int)y3, multibarWidth+2*spacing-penWidth/2,y2-y3);
  graphics.DrawLine( bearPen, x1, y4, x1, y2);
  graphics.DrawLine( bearPen, x1, y1, x1, y3);
} else {
  graphics.DrawRectangle(bearPen,(int)x1-multibarWidth/2-spacing, (int)y1, multibarWidth+2*spacing-penWidth/2,y4-y1 );
  graphics.FillRectangle(bearBrush,(int)x1-multibarWidth/2-spacing,(int)y3, multibarWidth+2*spacing-penWidth/2,y2-y3 );  	
}
}

else if ( y2 < y3 )
{
	if(!boxStyle) {
graphics.DrawRectangle( bullPen,(int)x1-multibarWidth/2-spacing, (int)y2, multibarWidth+2*spacing-penWidth/2,y3-y2 );
graphics.DrawLine( bullPen, x1, y4, x1, y3);
graphics.DrawLine( bullPen, x1, y1, x1, y2);
	} else {
graphics.DrawRectangle( bullPen,(int)x1-multibarWidth/2-spacing, (int)y1, multibarWidth+2*spacing-penWidth/2,y4-y1 );
graphics.FillRectangle( bullBrush,(int)x1-multibarWidth/2-spacing, (int)y2, multibarWidth+2*spacing-penWidth/2,y3-y2 );		
	}
}

else if ( y2 == y3 ) {
if(!boxStyle) {
graphics.DrawRectangle( bullPen,(int)x1-multibarWidth/2-spacing, (int)y3, multibarWidth+2*spacing-penWidth/2,1);
graphics.DrawLine( bullPen, x1, y4, x1, y3);
graphics.DrawLine( bullPen, x1, y1, x1, y2);
} else {
  graphics.DrawRectangle( bullPen,(int)x1-multibarWidth/2-spacing, (int)y1, multibarWidth+2*spacing-penWidth/2,y4-y1 );
  graphics.DrawRectangle( bullPen,(int)x1-multibarWidth/2-spacing, (int)y3, multibarWidth+2*spacing-penWidth/2,1);	
}
}

}

catch (Exception exception) {

caughtException = exception;

}

} // only draw the candle ends...
} // if on canvas

bars--;

} // candle drawing loop...


}

		
        #region Properties

        [Description("How many bars/minutes to group together?")]
        [Category("Parameters")]
        public int GroupingSize
        {
            get { return grouping; }
            set { grouping = Math.Max(2, value); }
        }
		[Description("Is the grouping in Minutes? (otherwise assumes bars)")]
        [Category("Parameters")]
        public EventGroupingMethod GroupingBy
        {
            get { return groupBy; }
            set { groupBy = value; }
        }
		[Description("Set open of session to close of previous session?")]
        [Category("Parameters")]
        public bool GaplessSessions
        {
            get { return gapless; }
            set { gapless = value; }
        }
		
		[Description("Plot as boxes with filled bodies? (false = candle-and-wick shape)")]
        [Category("Parameters")]
        public bool BoxStyle
        {
            get { return boxStyle; }
            set { boxStyle = value; }
        }
		
        [Description("How wide should the lines be?")]
        [Category("Parameters")]
        public int LineWidth
        {
            get { return penWidth; }
            set { penWidth = Math.Max(1, value); }
        }
        [Description("How opaque should the lines be (0 to 255)?")]
        [Category("Parameters")]
        public int PenAlpha
        {
            get { return candleAlpha; }
            set { candleAlpha = Math.Max(1, value); }
        }

        [Description("What time does the session start?")]
        [Category("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = Math.Max(0, value); }
        }
		
[Description("Color Bullish")]
[Category("Parameters")]
public Color ColorBullish

{

get { return colorBullish; }

set { colorBullish = value; }

}

[Browsable(false)]

public string colorBullishSerialize

{

get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorBullish); }

set { colorBullish = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }

}

[Description("Color Bearish")]
[Category("Parameters")]
public Color ColorBearish

{

get { return colorBearish; }

set { colorBearish = value; }

}

[Browsable(false)]

public string colorBearishSerialize

{

get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorBearish); }

set { colorBearish = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }

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
        private Z20091123VirtualBars[] cacheZ20091123VirtualBars = null;

        private static Z20091123VirtualBars checkZ20091123VirtualBars = new Z20091123VirtualBars();

        /// <summary>
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        public Z20091123VirtualBars Z20091123VirtualBars(bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            return Z20091123VirtualBars(Input, boxStyle, colorBearish, colorBullish, gaplessSessions, groupingBy, groupingSize, lineWidth, penAlpha, startTime);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        public Z20091123VirtualBars Z20091123VirtualBars(Data.IDataSeries input, bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            if (cacheZ20091123VirtualBars != null)
                for (int idx = 0; idx < cacheZ20091123VirtualBars.Length; idx++)
                    if (cacheZ20091123VirtualBars[idx].BoxStyle == boxStyle && cacheZ20091123VirtualBars[idx].ColorBearish == colorBearish && cacheZ20091123VirtualBars[idx].ColorBullish == colorBullish && cacheZ20091123VirtualBars[idx].GaplessSessions == gaplessSessions && cacheZ20091123VirtualBars[idx].GroupingBy == groupingBy && cacheZ20091123VirtualBars[idx].GroupingSize == groupingSize && cacheZ20091123VirtualBars[idx].LineWidth == lineWidth && cacheZ20091123VirtualBars[idx].PenAlpha == penAlpha && cacheZ20091123VirtualBars[idx].StartTime == startTime && cacheZ20091123VirtualBars[idx].EqualsInput(input))
                        return cacheZ20091123VirtualBars[idx];

            lock (checkZ20091123VirtualBars)
            {
                checkZ20091123VirtualBars.BoxStyle = boxStyle;
                boxStyle = checkZ20091123VirtualBars.BoxStyle;
                checkZ20091123VirtualBars.ColorBearish = colorBearish;
                colorBearish = checkZ20091123VirtualBars.ColorBearish;
                checkZ20091123VirtualBars.ColorBullish = colorBullish;
                colorBullish = checkZ20091123VirtualBars.ColorBullish;
                checkZ20091123VirtualBars.GaplessSessions = gaplessSessions;
                gaplessSessions = checkZ20091123VirtualBars.GaplessSessions;
                checkZ20091123VirtualBars.GroupingBy = groupingBy;
                groupingBy = checkZ20091123VirtualBars.GroupingBy;
                checkZ20091123VirtualBars.GroupingSize = groupingSize;
                groupingSize = checkZ20091123VirtualBars.GroupingSize;
                checkZ20091123VirtualBars.LineWidth = lineWidth;
                lineWidth = checkZ20091123VirtualBars.LineWidth;
                checkZ20091123VirtualBars.PenAlpha = penAlpha;
                penAlpha = checkZ20091123VirtualBars.PenAlpha;
                checkZ20091123VirtualBars.StartTime = startTime;
                startTime = checkZ20091123VirtualBars.StartTime;

                if (cacheZ20091123VirtualBars != null)
                    for (int idx = 0; idx < cacheZ20091123VirtualBars.Length; idx++)
                        if (cacheZ20091123VirtualBars[idx].BoxStyle == boxStyle && cacheZ20091123VirtualBars[idx].ColorBearish == colorBearish && cacheZ20091123VirtualBars[idx].ColorBullish == colorBullish && cacheZ20091123VirtualBars[idx].GaplessSessions == gaplessSessions && cacheZ20091123VirtualBars[idx].GroupingBy == groupingBy && cacheZ20091123VirtualBars[idx].GroupingSize == groupingSize && cacheZ20091123VirtualBars[idx].LineWidth == lineWidth && cacheZ20091123VirtualBars[idx].PenAlpha == penAlpha && cacheZ20091123VirtualBars[idx].StartTime == startTime && cacheZ20091123VirtualBars[idx].EqualsInput(input))
                            return cacheZ20091123VirtualBars[idx];

                Z20091123VirtualBars indicator = new Z20091123VirtualBars();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BoxStyle = boxStyle;
                indicator.ColorBearish = colorBearish;
                indicator.ColorBullish = colorBullish;
                indicator.GaplessSessions = gaplessSessions;
                indicator.GroupingBy = groupingBy;
                indicator.GroupingSize = groupingSize;
                indicator.LineWidth = lineWidth;
                indicator.PenAlpha = penAlpha;
                indicator.StartTime = startTime;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20091123VirtualBars[] tmp = new Z20091123VirtualBars[cacheZ20091123VirtualBars == null ? 1 : cacheZ20091123VirtualBars.Length + 1];
                if (cacheZ20091123VirtualBars != null)
                    cacheZ20091123VirtualBars.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20091123VirtualBars = tmp;
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
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091123VirtualBars Z20091123VirtualBars(bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            return _indicator.Z20091123VirtualBars(Input, boxStyle, colorBearish, colorBullish, gaplessSessions, groupingBy, groupingSize, lineWidth, penAlpha, startTime);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091123VirtualBars Z20091123VirtualBars(Data.IDataSeries input, bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            return _indicator.Z20091123VirtualBars(input, boxStyle, colorBearish, colorBullish, gaplessSessions, groupingBy, groupingSize, lineWidth, penAlpha, startTime);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091123VirtualBars Z20091123VirtualBars(bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            return _indicator.Z20091123VirtualBars(Input, boxStyle, colorBearish, colorBullish, gaplessSessions, groupingBy, groupingSize, lineWidth, penAlpha, startTime);
        }

        /// <summary>
        /// Richard Todd www.movethemarkets.com Draws virtual bars
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091123VirtualBars Z20091123VirtualBars(Data.IDataSeries input, bool boxStyle, Color colorBearish, Color colorBullish, bool gaplessSessions, EventGroupingMethod groupingBy, int groupingSize, int lineWidth, int penAlpha, int startTime)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091123VirtualBars(input, boxStyle, colorBearish, colorBullish, gaplessSessions, groupingBy, groupingSize, lineWidth, penAlpha, startTime);
        }
    }
}
#endregion
