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


public enum EventGroupingMethod {
  Minutes,Bars,Volume	
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// richard todd www.movethemarkets.com for noticing periodic events
    /// </summary>
    [Description("richard todd www.movethemarkets.com for noticing periodic events")]
    public class Z20091203PeriodicEvent : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int grouping = 30; // Default setting for Grouping
            private EventGroupingMethod groupMethod = EventGroupingMethod.Minutes;
		
		    private DateTime tgtTime;
		    private DateTime sessionTime;
		    private DateTime nullDate; // need this token since DateTime's can't be null.  sheesh!
		
            private int startTime = 830; // Default setting for StartTime
        // User defined variables (add any user defined variables below)
		    private IntSeries barCount;
		    private BoolSeries newBar;
		    private BoolSeries newSession;
		    private int lastBarSeen = -1;
		
		    // for counting volume...
		    private double volumeCount;
				    
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            //CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
			
			barCount = new IntSeries(this);
			newBar = new BoolSeries(this);
			newSession = new BoolSeries(this);
			
			nullDate = new DateTime(0,DateTimeKind.Unspecified);
			tgtTime = nullDate;
			sessionTime = nullDate;
			
			volumeCount = 0;
        }
		
	   private bool atSessionStart() {
		  if(sessionTime.CompareTo(nullDate)==0) updateSessionTime(); 
		
		  bool  ans = ((Time[0] > sessionTime) && 
			         (Time[1] <= sessionTime));
		  if(ans && (groupMethod==EventGroupingMethod.Minutes)) {
		     // at new session need to synchronize the tgtTime to it...
			 tgtTime = sessionTime;
			 advanceTargetTime();
		  }
		  updateSessionTime();
		  return ans;
	   }
	
	   private bool atBarResetTime() {
		  bool ans = false;
		  switch(groupMethod) {
			case EventGroupingMethod.Bars:
				ans = (barCount[0] >= grouping);
			    break;
			case EventGroupingMethod.Minutes:
  		      if(tgtTime.CompareTo(nullDate) == 0) { 
                 tgtTime = new DateTime(Time[0].Year,Time[0].Month,Time[0].Day,startTime/100,startTime%100,0,0,Time[0].Kind);		    
			     if(Time[0] < tgtTime) tgtTime = tgtTime.AddDays(-1);
			     advanceTargetTime();
		      }
		      ans =  (Time[0] > tgtTime);
		      advanceTargetTime();
			  break;
			case EventGroupingMethod.Volume:
			  ans = (volumeCount >= grouping);
			  break;
			default:
			  break;
		  }
		
		  return ans;
	   }	
	
       private void advanceTargetTime() {
			while(Time[0] > tgtTime) {
				tgtTime = tgtTime.AddMinutes(grouping);
//			    Print(Time[0] + " versus " + tgtTime + " in advanceTargetTime");
			}
	    }
		
	   private void updateSessionTime() {
		 if(sessionTime.CompareTo(nullDate) == 0) sessionTime = new DateTime(Time[0].Year,Time[0].Month,Time[0].Day,startTime/100,startTime%100,0,0,Time[0].Kind);
		
		 while(Time[0] > sessionTime)  {
//			Print(Time[0] + " versus " + sessionTime + " in updateSessionTime");
		   	sessionTime = sessionTime.AddDays(1);	
		 }
	   }
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBar < 2) { 
				barCount.Set(-1); 
				return; 
			}

			
			// do book-keeping on new bars...
			if(CurrentBar != lastBarSeen) {
			  lastBarSeen = CurrentBar;
			  barCount.Set(barCount[1]+1);			
			  volumeCount += Volume[1];
				
			  //Print("Bar: "+CurrentBar+" time: "+Time[0]+" tgt: "+tgtTime+ " sess: "+sessionTime);
			
			  // determine if it's time for a new bar...
			  bool barReset = atBarResetTime(); 
			  bool sessionStart = atSessionStart();
			
  			  if(barReset || sessionStart) {
			    // we need to start fresh...
			    barCount.Set(0);
				volumeCount = 0;
				newBar.Set(true);
				if(sessionStart) newSession.Set(true);
				
			  } else {
				// update stuff...
				newBar.Set(false);
				newSession.Set(false);
			  }
			}
			
			//Print("CurrentBar: " + CurrentBar + " Bar Count: " + barCount[0] + " Vol count: " +volumeCount+" " +newBar[0]);
			
        }

		
        #region Properties

        [Description("How many bars/minutes to group together?")]
        [Category("Parameters")]
        public int GroupingSize
        {
            get { return grouping; }
            set { grouping = Math.Max(0, value); }
        }
		
		[Description("What are we counting for periodic events?")]
        [Category("Parameters")]
        public EventGroupingMethod GroupingBy
        {
            get { return groupMethod; }
            set { groupMethod = value; }
        }

 
        [Description("What time does the session start?")]
        [Category("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = Math.Max(0, value); }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public IntSeries BarIndex
        {
            get { return barCount; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public BoolSeries IsNewBar
        {
            get { return newBar; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public BoolSeries IsNewSession
        {
            get { return newSession; }
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
        private Z20091203PeriodicEvent[] cacheZ20091203PeriodicEvent = null;

        private static Z20091203PeriodicEvent checkZ20091203PeriodicEvent = new Z20091203PeriodicEvent();

        /// <summary>
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        public Z20091203PeriodicEvent Z20091203PeriodicEvent(EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            return Z20091203PeriodicEvent(Input, groupingBy, groupingSize, startTime);
        }

        /// <summary>
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        public Z20091203PeriodicEvent Z20091203PeriodicEvent(Data.IDataSeries input, EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            if (cacheZ20091203PeriodicEvent != null)
                for (int idx = 0; idx < cacheZ20091203PeriodicEvent.Length; idx++)
                    if (cacheZ20091203PeriodicEvent[idx].GroupingBy == groupingBy && cacheZ20091203PeriodicEvent[idx].GroupingSize == groupingSize && cacheZ20091203PeriodicEvent[idx].StartTime == startTime && cacheZ20091203PeriodicEvent[idx].EqualsInput(input))
                        return cacheZ20091203PeriodicEvent[idx];

            lock (checkZ20091203PeriodicEvent)
            {
                checkZ20091203PeriodicEvent.GroupingBy = groupingBy;
                groupingBy = checkZ20091203PeriodicEvent.GroupingBy;
                checkZ20091203PeriodicEvent.GroupingSize = groupingSize;
                groupingSize = checkZ20091203PeriodicEvent.GroupingSize;
                checkZ20091203PeriodicEvent.StartTime = startTime;
                startTime = checkZ20091203PeriodicEvent.StartTime;

                if (cacheZ20091203PeriodicEvent != null)
                    for (int idx = 0; idx < cacheZ20091203PeriodicEvent.Length; idx++)
                        if (cacheZ20091203PeriodicEvent[idx].GroupingBy == groupingBy && cacheZ20091203PeriodicEvent[idx].GroupingSize == groupingSize && cacheZ20091203PeriodicEvent[idx].StartTime == startTime && cacheZ20091203PeriodicEvent[idx].EqualsInput(input))
                            return cacheZ20091203PeriodicEvent[idx];

                Z20091203PeriodicEvent indicator = new Z20091203PeriodicEvent();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.GroupingBy = groupingBy;
                indicator.GroupingSize = groupingSize;
                indicator.StartTime = startTime;
                Indicators.Add(indicator);
                indicator.SetUp();

                Z20091203PeriodicEvent[] tmp = new Z20091203PeriodicEvent[cacheZ20091203PeriodicEvent == null ? 1 : cacheZ20091203PeriodicEvent.Length + 1];
                if (cacheZ20091203PeriodicEvent != null)
                    cacheZ20091203PeriodicEvent.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZ20091203PeriodicEvent = tmp;
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
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091203PeriodicEvent Z20091203PeriodicEvent(EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            return _indicator.Z20091203PeriodicEvent(Input, groupingBy, groupingSize, startTime);
        }

        /// <summary>
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091203PeriodicEvent Z20091203PeriodicEvent(Data.IDataSeries input, EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            return _indicator.Z20091203PeriodicEvent(input, groupingBy, groupingSize, startTime);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Z20091203PeriodicEvent Z20091203PeriodicEvent(EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            return _indicator.Z20091203PeriodicEvent(Input, groupingBy, groupingSize, startTime);
        }

        /// <summary>
        /// richard todd www.movethemarkets.com for noticing periodic events
        /// </summary>
        /// <returns></returns>
        public Indicator.Z20091203PeriodicEvent Z20091203PeriodicEvent(Data.IDataSeries input, EventGroupingMethod groupingBy, int groupingSize, int startTime)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Z20091203PeriodicEvent(input, groupingBy, groupingSize, startTime);
        }
    }
}
#endregion
