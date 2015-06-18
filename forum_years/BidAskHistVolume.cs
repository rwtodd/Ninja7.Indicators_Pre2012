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
	/// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
	/// </summary>
	[Description("Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.")]
    [Gui.Design.DisplayName("Bid/Ask/Hist Volume")]
	public class BidAskHistVolume : Indicator
	{
		#region Variables
		private int                             activeBar       = -1;
        private double                          askPrice        = 0;
        private double                          bidPrice        = 0;
		private double                          buys            = 0;
		private bool                            firstPaint      = true;
		private double                          sells           = 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Bar,  "Sells"));
			Add(new Plot(new Pen(Color.Lime, 4), PlotStyle.Bar, "Buys"));
			Add(new Plot(new Pen(Color.DarkSalmon, 6), PlotStyle.Bar, "Volume Up"));
			Add(new Plot(new Pen(Color.SteelBlue, 6),  PlotStyle.Bar, "Volume Down"));
			Add(new Plot(new Pen(Color.Gray, 6),       PlotStyle.Bar, "Volume Doji"));

			CalculateOnBarClose = false;
			DisplayInDataBox = false;
			PaintPriceMarkers = false;
			PlotsConfigurable = true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{

			if(Open[0] > Close[0])
			   Values[2].Set(Volume[0]);
			else if(Open[0] < Close[0])
			   Values[3].Set(Volume[0]);
			else 
			  Values[4].Set(Volume[0]);

			if (CurrentBar < activeBar)
			{
				return;
			}
			else if (CurrentBar != activeBar)
			{
				buys = 0;
				sells = 0;
				activeBar = CurrentBar;
			}

			if (!Historical)
			{
				Values[1].Set(buys);
				Values[0].Set(sells);
			}
		}

        /// <summary>
        /// Called on each incoming real time market data event
        /// </summary>
        protected override void OnMarketData(MarketDataEventArgs e)
        {
            if (e.MarketDataType == MarketDataType.Ask)
                askPrice = e.Price;
            else if (e.MarketDataType == MarketDataType.Bid)
                bidPrice = e.Price;
            else if (e.MarketDataType == MarketDataType.Last)
            {
                if (askPrice > 0 && e.Price >= askPrice)
                    buys += e.Volume;
                else if (bidPrice > 0 && e.Price <= bidPrice)
                    sells += e.Volume;
            }    
        }
	}
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private BidAskHistVolume[] cacheBidAskHistVolume = null;

        private static BidAskHistVolume checkBidAskHistVolume = new BidAskHistVolume();

        /// <summary>
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public BidAskHistVolume BidAskHistVolume()
        {
            return BidAskHistVolume(Input);
        }

        /// <summary>
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public BidAskHistVolume BidAskHistVolume(Data.IDataSeries input)
        {
            if (cacheBidAskHistVolume != null)
                for (int idx = 0; idx < cacheBidAskHistVolume.Length; idx++)
                    if (cacheBidAskHistVolume[idx].EqualsInput(input))
                        return cacheBidAskHistVolume[idx];

            lock (checkBidAskHistVolume)
            {
                if (cacheBidAskHistVolume != null)
                    for (int idx = 0; idx < cacheBidAskHistVolume.Length; idx++)
                        if (cacheBidAskHistVolume[idx].EqualsInput(input))
                            return cacheBidAskHistVolume[idx];

                BidAskHistVolume indicator = new BidAskHistVolume();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                BidAskHistVolume[] tmp = new BidAskHistVolume[cacheBidAskHistVolume == null ? 1 : cacheBidAskHistVolume.Length + 1];
                if (cacheBidAskHistVolume != null)
                    cacheBidAskHistVolume.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBidAskHistVolume = tmp;
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
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BidAskHistVolume BidAskHistVolume()
        {
            return _indicator.BidAskHistVolume(Input);
        }

        /// <summary>
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.BidAskHistVolume BidAskHistVolume(Data.IDataSeries input)
        {
            return _indicator.BidAskHistVolume(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BidAskHistVolume BidAskHistVolume()
        {
            return _indicator.BidAskHistVolume(Input);
        }

        /// <summary>
        /// Shows historical volume on all bars, and adds Bid/Ask volume on new bars, splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.BidAskHistVolume BidAskHistVolume(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BidAskHistVolume(input);
        }
    }
}
#endregion
