// 
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
//
#region Using declarations
using System;
using System.Collections;
using System.Text;
#endregion

// This namespace holds all bars types. Do not change it.
namespace NinjaTrader.Data
{


	/// <summary>
	/// </summary>
	public class VolDensType : BarsType
	{
		private static bool		registered		= Data.BarsType.Register(new VolDensType());

		/// <summary>
		/// </summary>
		/// <param name="bars"></param>
		/// <param name="open"></param>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close"></param>
		/// <param name="time"></param>
		/// <param name="volume"></param>
		/// <param name="isRealtime"></param>
		public override void Add(Data.Bars bars, double open, double high, double low, double close, DateTime time, int volume, bool isRealtime)
		{

			if (bars.Count == 0) {
				AddBar(bars, open, high, low, close, time, volume);
                  }
			else
			{
                        UpdateBar(bars, open, high, low, close, time, volume);
				Data.Bar	bar			= (Bar) bars.Get(bars.Count - 1); 

                        double tall  = 1 + ((bar.High - bar.Low) / bars.Instrument.MasterInstrument.TickSize);
                        double denom = tall;
                        if(tall > 10) denom -= ((tall-10)/2.0);
                        if(tall > 20) denom -= ((tall-20)/3.0);
                        
                        if( (bar.Volume/denom) >= ((double)(bars.Period.Value)) ) 
                        {
                            AddBar(bars,close,close,close,close,time,0);
                        }
                  }
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(NinjaTrader.Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new VolDensType();
		}

		/// <summary>
		/// </summary>
		public override int DaysBack
		{
			get { return Gui.Chart.ChartData.DaysBackTick; }
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 10; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return "VolDens"; } // PeriodType.ToString(); }
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public override bool IsTimeBased
		{
			get { return false; }
		}

		/// <summary>
		/// </summary>
		public override int MaxLookBackDays
		{ 
			get { return 10;} 
		}

		/// <summary>
		/// </summary>
		public override int MaxValue
		{
			get { return -1; }
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Data.Bars bars, DateTime now)
		{
			throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
		}

		/// <summary>
		/// </summary>
		public VolDensType() : base(PeriodType.Custom7)
		{
		}

		/// <summary>
		/// </summary>
		public override int SortOrder
		{
			get { return 14001; }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return "VolDens " + period.Value.ToString() + " Per Tick";
		}
	}
}
