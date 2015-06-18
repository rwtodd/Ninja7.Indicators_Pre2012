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
    /// Trade Count, only with a memory
    /// </summary>
    [Description("Trade Count, only with a memory")]
    public class ZRichTCwMem : Indicator
    {
        #region Variables
        // Wizard generated variables
			private bool compactForm = true; // Compact form
            private int memorySize = 8; // Default setting for MemorySize
            private bool fuzzyPrices = false; // Default setting for FuzzyPrices
		    private rwt.IExtendedData extdat = null;
		
			private long prevUp, prevDn;
			private double[] memoryUp;
		    private double[] memoryDn;
		    private double[] oldMemoryUp;
		    private double[] oldMemoryDn;
		    private double topPrice = -1;
		   
		    private int lastBarSeen = -1;

        // User defined variables (add any user defined variables below)
        #endregion

		private void memGotNewBar() {
		  // save off memory...
	      Array.Copy(memoryUp,oldMemoryUp,memorySize);
		  Array.Copy(memoryDn,oldMemoryDn,memorySize);
		}
		
		private int memLookupIndex(double price) {
		   int rawIndex = (int)((topPrice - price)/TickSize);

		   if((rawIndex >= 0) && (rawIndex < memorySize))
			  return rawIndex;
		
			// need to shift things around...
		   if(rawIndex > 0) {
			   int shiftAmt = rawIndex - (memorySize - 1);
			   if(shiftAmt < memorySize) {
				 Array.Copy(memoryUp,shiftAmt,memoryUp,0,memorySize-shiftAmt);
				 Array.Copy(memoryDn,shiftAmt,memoryDn,0,memorySize-shiftAmt);
				 Array.Copy(oldMemoryUp,shiftAmt,oldMemoryUp,0,memorySize-shiftAmt);
				 Array.Copy(oldMemoryDn,shiftAmt,oldMemoryDn,0,memorySize-shiftAmt);
				 Array.Clear(memoryUp,memorySize-shiftAmt,shiftAmt);
				 Array.Clear(memoryDn,memorySize-shiftAmt,shiftAmt);
				 Array.Clear(oldMemoryUp,memorySize-shiftAmt,shiftAmt);
				 Array.Clear(oldMemoryDn,memorySize-shiftAmt,shiftAmt);
			   } else {
				 // no sense in shifting the memory.. just clear it...
				 Array.Clear(memoryUp,0,memorySize);
				 Array.Clear(memoryDn,0,memorySize);
				 Array.Clear(oldMemoryUp,0,memorySize);
				 Array.Clear(oldMemoryDn,0,memorySize);
			   }
		
			   topPrice = price + ((memorySize-1)*TickSize);
			   return (memorySize-1);
			
		    } else {
			   int shiftAmt = -rawIndex;
			   if(shiftAmt < memorySize) {
				 Array.Copy(memoryUp,0,memoryUp,shiftAmt,memorySize-shiftAmt);
				 Array.Copy(memoryDn,0,memoryDn,shiftAmt,memorySize-shiftAmt);
				 Array.Copy(oldMemoryUp,0,oldMemoryUp,shiftAmt,memorySize-shiftAmt);
				 Array.Copy(oldMemoryDn,0,oldMemoryDn,shiftAmt,memorySize-shiftAmt);
				 Array.Clear(memoryUp,0,shiftAmt);
				 Array.Clear(memoryDn,0,shiftAmt);
				 Array.Clear(oldMemoryUp,0,shiftAmt);
				 Array.Clear(oldMemoryDn,0,shiftAmt);							
			   } else {
				 // no sense in shifting the memory.. just clear it...
				 Array.Clear(memoryUp,0,memorySize);
				 Array.Clear(memoryDn,0,memorySize);
				 Array.Clear(oldMemoryUp,0,memorySize);
				 Array.Clear(oldMemoryDn,0,memorySize);				
			   }
			
			   topPrice = price;
			   return 0;
			}
		   
			// shouldn't get here...
		    return -1;
		}
		
		private void memInsertDeltaUp(double price,double delta) {
		  double realprice = Bars.Instrument.MasterInstrument.Round2TickSize(price);
		  int indx = memLookupIndex(realprice);
		  memoryUp[indx] += delta;
		}
		
		private void memInsertDeltaDn(double price,double delta) {
		  double realprice = Bars.Instrument.MasterInstrument.Round2TickSize(price);
		  int indx = memLookupIndex(realprice);
		  memoryDn[indx] += delta;
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSpringGreen), PlotStyle.Bar, "BuyersThisBar"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Violet), PlotStyle.Bar, "SellersThisBar"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Bar, "Buyers"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Bar, "Sellers"));
            Add(new Line(Color.FromKnownColor(KnownColor.Black), 0, "Zero"));
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
			lastBarSeen = -1;
			topPrice = -1;
			Plots[0].Pen.Width =3;
			Plots[1].Pen.Width =3;
			Plots[2].Pen.Width =3;
			Plots[3].Pen.Width =3;

        }

		protected override void OnStartUp() {
			memoryUp = new double[memorySize];
			memoryDn = new double[memorySize];
			oldMemoryUp = new double[memorySize];
			oldMemoryDn = new double[memorySize];		
			prevUp = 0;
			prevDn = 0;
			extdat = Bars.BarsType as rwt.IExtendedData;
			if(extdat == null) throw new Exception("Only use this indicator on an Extended Data BarType!");			
		}
		
		private void memPressureForBar(out double totalUp, out double currentUp, 
			                             out double totalDn, out double currentDn) {
			totalUp = 0; totalDn = 0;
			currentUp = 0; currentDn = 0;
			
			int indx = memLookupIndex(High[0]);
			int last = indx + (int)((High[0]-Low[0])/TickSize);								
			if(last >= memorySize) last = (memorySize - 1);
						
			for( ; indx <= last; ++indx ) {
				totalUp += memoryUp[indx];
				currentUp += (memoryUp[indx] - oldMemoryUp[indx]);
				totalDn += memoryDn[indx];
				currentDn += (memoryDn[indx] - oldMemoryDn[indx]);
				
				if(fuzzyPrices) {
				  if(indx > 0) {
					totalUp += memoryUp[indx-1]*0.5;
					currentUp += (memoryUp[indx-1] - oldMemoryUp[indx-1])*0.5;
					totalDn += memoryDn[indx-1]*0.5;
					currentDn += (memoryDn[indx-1] - oldMemoryDn[indx-1])*0.5;
				  }
				  if(indx > 1) {
					totalUp += memoryUp[indx-2]*0.25;
					currentUp += (memoryUp[indx-2] - oldMemoryUp[indx-2])*0.25;
					totalDn += memoryDn[indx-2]*0.25;
					currentDn += (memoryDn[indx-2] - oldMemoryDn[indx-2])*0.25;
				  }
				  if(indx < (memorySize - 1)) {
					  totalUp += memoryUp[indx+1]*0.5;
					  currentUp += (memoryUp[indx+1] - oldMemoryUp[indx+1])*0.5;
					  totalDn += memoryDn[indx+1]*0.5;
					  currentDn += (memoryDn[indx+1] - oldMemoryDn[indx+1])*0.5;					
				  }
				  if(indx < (memorySize - 2)) {
					  totalUp += memoryUp[indx+2]*0.25;
					  currentUp += (memoryUp[indx+2] - oldMemoryUp[indx+2])*0.25;
					  totalDn += memoryDn[indx+2]*0.25;
					  currentDn += (memoryDn[indx+2] - oldMemoryDn[indx+2])*0.25;					
				  }
				}
			}
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			var ed = extdat.getExtraData(0,Bars,CurrentBar);

			if(CurrentBar != lastBarSeen) {
				memGotNewBar();
				lastBarSeen = CurrentBar;
				
				if(ed!=null) {
				  prevUp = ed.UpTicks;
				  prevDn = ed.DnTicks;
				  for(double p = Low[0]; p <= High[0]; p += TickSize) {
				     long upt; long dnt;
				     ed.getUpDnTicksAtPrice(TickSize,p,out upt, out dnt);
					 if(upt>0) memInsertDeltaUp(p,upt);
					 if(dnt>0) memInsertDeltaDn(p,dnt);
				  }
				}
			}  else {
				if(ed!=null) {
				  if(ed.UpTicks > prevUp) {
				     memInsertDeltaUp(Close[0],ed.UpTicks - prevUp);
					 prevUp = ed.UpTicks;
				  }
				  if(ed.DnTicks > prevDn) {
				     memInsertDeltaDn(Close[0],ed.DnTicks - prevDn);
					 prevDn = ed.DnTicks;
				  }
				  
				}
			}
			

			double totalUp, totalDn,currentUp, currentDn;
			memPressureForBar(out totalUp, out currentUp, out totalDn, out currentDn);
			
			if(compactForm) {
			  if(totalUp > totalDn) { totalUp -= totalDn; totalDn = 0; }
			  else { totalDn -= totalUp; totalUp = 0; }
			  
			}
			
            Buyers.Set(totalUp);
            Sellers.Set(-totalDn);
            BuyersThisBar.Set(currentUp);
            SellersThisBar.Set(-currentDn);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Buyers
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Sellers
        {
            get { return Values[3]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BuyersThisBar
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SellersThisBar
        {
            get { return Values[1]; }
        }

        [Description("Memory Size in Ticks")]
        [Category("Parameters")]
        public int MemorySize
        {
            get { return memorySize; }
            set { memorySize = Math.Max(2, value); }
        }

        [Description("Use fuzzy price mechanism?")]
        [Category("Parameters")]
        public bool FuzzyPrices
        {
            get { return fuzzyPrices; }
            set { fuzzyPrices = value; }
        }
		
 
        [Description("Use compact output representation?")]
        [Category("Parameters")]
        public bool  CompactForm
        {
            get { return compactForm; }
            set { compactForm = value; }
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
        private ZRichTCwMem[] cacheZRichTCwMem = null;

        private static ZRichTCwMem checkZRichTCwMem = new ZRichTCwMem();

        /// <summary>
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        public ZRichTCwMem ZRichTCwMem(bool compactForm, bool fuzzyPrices, int memorySize)
        {
            return ZRichTCwMem(Input, compactForm, fuzzyPrices, memorySize);
        }

        /// <summary>
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        public ZRichTCwMem ZRichTCwMem(Data.IDataSeries input, bool compactForm, bool fuzzyPrices, int memorySize)
        {
            if (cacheZRichTCwMem != null)
                for (int idx = 0; idx < cacheZRichTCwMem.Length; idx++)
                    if (cacheZRichTCwMem[idx].CompactForm == compactForm && cacheZRichTCwMem[idx].FuzzyPrices == fuzzyPrices && cacheZRichTCwMem[idx].MemorySize == memorySize && cacheZRichTCwMem[idx].EqualsInput(input))
                        return cacheZRichTCwMem[idx];

            lock (checkZRichTCwMem)
            {
                checkZRichTCwMem.CompactForm = compactForm;
                compactForm = checkZRichTCwMem.CompactForm;
                checkZRichTCwMem.FuzzyPrices = fuzzyPrices;
                fuzzyPrices = checkZRichTCwMem.FuzzyPrices;
                checkZRichTCwMem.MemorySize = memorySize;
                memorySize = checkZRichTCwMem.MemorySize;

                if (cacheZRichTCwMem != null)
                    for (int idx = 0; idx < cacheZRichTCwMem.Length; idx++)
                        if (cacheZRichTCwMem[idx].CompactForm == compactForm && cacheZRichTCwMem[idx].FuzzyPrices == fuzzyPrices && cacheZRichTCwMem[idx].MemorySize == memorySize && cacheZRichTCwMem[idx].EqualsInput(input))
                            return cacheZRichTCwMem[idx];

                ZRichTCwMem indicator = new ZRichTCwMem();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CompactForm = compactForm;
                indicator.FuzzyPrices = fuzzyPrices;
                indicator.MemorySize = memorySize;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZRichTCwMem[] tmp = new ZRichTCwMem[cacheZRichTCwMem == null ? 1 : cacheZRichTCwMem.Length + 1];
                if (cacheZRichTCwMem != null)
                    cacheZRichTCwMem.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZRichTCwMem = tmp;
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
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichTCwMem ZRichTCwMem(bool compactForm, bool fuzzyPrices, int memorySize)
        {
            return _indicator.ZRichTCwMem(Input, compactForm, fuzzyPrices, memorySize);
        }

        /// <summary>
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTCwMem ZRichTCwMem(Data.IDataSeries input, bool compactForm, bool fuzzyPrices, int memorySize)
        {
            return _indicator.ZRichTCwMem(input, compactForm, fuzzyPrices, memorySize);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZRichTCwMem ZRichTCwMem(bool compactForm, bool fuzzyPrices, int memorySize)
        {
            return _indicator.ZRichTCwMem(Input, compactForm, fuzzyPrices, memorySize);
        }

        /// <summary>
        /// Trade Count, only with a memory
        /// </summary>
        /// <returns></returns>
        public Indicator.ZRichTCwMem ZRichTCwMem(Data.IDataSeries input, bool compactForm, bool fuzzyPrices, int memorySize)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZRichTCwMem(input, compactForm, fuzzyPrices, memorySize);
        }
    }
}
#endregion
