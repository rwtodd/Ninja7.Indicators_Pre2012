#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion
//MT4 CODE CREDITS SECTION
//+------------------------------------------------------------------+
//|                                                  FantailVMA3.mq4 |
//|                                  Copyright © 2007, Forex-TSD.com |
//|                         Written by IgorAD,igorad2003@yahoo.co.uk |   
//|            http://finance.groups.yahoo.com/group/TrendLaboratory |                                      
//+------------------------------------------------------------------+
//Revision history; 
//FantailVMA1: Modified version of VarMA_v1.1.mq4 and Fantail.tpl
//FantailVMA1.mq4 is intended to used by adding the template FantailVMA1.tpl to the chart. 10 Sept 2007.
//Two lines have been commented out and a third one added to use one less array, VarMA[].
//FantailVMA2 & Fantail2.tpl: Turbo version attempt, fantail not adjusted yet for shorter horizontal, 17 Sept 2007.
//FantailVMA3 & Fantail3.tpl: Live end of previous version had the fantail lines defaulting to MA_Length=1.
//#property copyright "Copyright © 2007, Forex-TSD.com "
//#property link      "http://www.forex-tsd.com/"
//MT4 CODE CREDITS SECTION END

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// FantailVMA3
    /// </summary>
    [Description("FantailVMA3")]
    [Gui.Design.DisplayName("FantailVMA3")]
    public class FantailVMA3 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int mAPeriod = 1; // Default setting for MAPeriod
            private int aDXPeriod = 2; // Default setting for ADXPeriod
            private double weighting = 2; // Default setting for Weighting
        // User defined variables (add any user defined variables below)
			private DataSeries MA;
			private EMA ema;
			private double VarMA;
			private double VarMAPrev;
			private double vma;
			private double vmaPrev;
			private double sPDI;
			private double sPDIPrev;
			private double sMDI;
			private double sMDIPrev;
			private double STR;
			private double STRPrev;
			private DataSeries adx;
			private double alfa;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "FVMA"));
            CalculateOnBarClose	= true;
            Overlay				= true;
        }

		protected override void OnStartUp() {
			MA = new DataSeries( this );
			VarMA = 0;
			VarMAPrev = 0;
			vma = 0;
			vmaPrev = 0;
			sPDI = 0;
			sPDIPrev = 0;
			sMDI = 0;
			sMDIPrev = 0;
			STR = 0;
			STRPrev = 0;
			adx = new DataSeries( this );
			alfa = 1.0/ADXPeriod;
			ema = EMA( MA, MAPeriod );			
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			try
			{
				if( CurrentBar < 10 )
					return;
				int i = 0;
				double Hi  = High[i];
				double Hi1 = High[i+1];
				double Lo  = Low[i];
				double Lo1 = Low[i+1];
				double Close1= Close[i+1];
				double Bulls = 0.5*(Math.Abs(Hi-Hi1)+(Hi-Hi1));
				double Bears = 0.5*(Math.Abs(Lo1-Lo)+(Lo1-Lo));
				
				if (Bulls > Bears)
					Bears = 0;
				else if (Bulls < Bears)
					Bulls = 0;
				else if (Bulls == Bears)
				{
					Bulls = 0;Bears = 0;
				}
				
				sPDI = (Weighting*sPDIPrev + Bulls)/(Weighting+1);//ma weighting 
				sMDI = (Weighting*sMDIPrev + Bears)/(Weighting+1);//ma weighting 
				
				double   TR = Math.Max(Hi-Lo,Hi-Close1); 
				STR  = (Weighting*STRPrev + TR)/(Weighting+1);//ma weighting  
				double PDI = 0;
				double MDI = 0;
				if(STR>0 )
				{
					PDI = sPDI/STR;
					MDI = sMDI/STR;
				}
				double DX = 0;         
				if((PDI + MDI) > 0) 
					DX = Math.Abs(PDI - MDI)/(PDI + MDI); 
				else
					DX = 0;
				
				adx.Set( (Weighting*adx[i+1] + DX)/(Weighting+1));//ma weighting    
				double vADX = adx[i]; 
					
					
				
				double ADXmin = 1000000;
				for (int j=0; j<=ADXPeriod-1;j++)
					ADXmin = Math.Min(ADXmin,adx[i+j]);
					
				double ADXmax = -1;
				for (int j=0; j<=ADXPeriod-1;j++)
					ADXmax = Math.Max(ADXmax,adx[i+j]);
				
				
				double Diff = ADXmax - ADXmin;
				double Const = 0;
				if(Diff > 0)
					Const = (vADX- ADXmin)/Diff;
				else
					Const = 0;
					
				
				VarMA=((2-Const)*VarMAPrev+Const*Input[i])/2;//Same equation but one less array, mod 10 Sept 2007.
				VarMAPrev = VarMA;
				vmaPrev = vma;
				sPDIPrev = sPDI;
				sMDIPrev = sMDI;
				STRPrev = STR;
				MA.Set( VarMA );
				FVMA.Set( MA[ 0 ] );
			}
			catch( Exception ex )
			{
				Print( ex.ToString() );
			}
	}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries FVMA
        {
            get { return Values[0]; }
        }

        [Description("")]
        [Category("Parameters")]
        public int MAPeriod
        {
            get { return mAPeriod; }
            set { mAPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int ADXPeriod
        {
            get { return aDXPeriod; }
            set { aDXPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public double Weighting
        {
            get { return weighting; }
            set { weighting = Math.Max(1, value); }
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
        private FantailVMA3[] cacheFantailVMA3 = null;

        private static FantailVMA3 checkFantailVMA3 = new FantailVMA3();

        /// <summary>
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        public FantailVMA3 FantailVMA3(int aDXPeriod, int mAPeriod, double weighting)
        {
            return FantailVMA3(Input, aDXPeriod, mAPeriod, weighting);
        }

        /// <summary>
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        public FantailVMA3 FantailVMA3(Data.IDataSeries input, int aDXPeriod, int mAPeriod, double weighting)
        {
            if (cacheFantailVMA3 != null)
                for (int idx = 0; idx < cacheFantailVMA3.Length; idx++)
                    if (cacheFantailVMA3[idx].ADXPeriod == aDXPeriod && cacheFantailVMA3[idx].MAPeriod == mAPeriod && Math.Abs(cacheFantailVMA3[idx].Weighting - weighting) <= double.Epsilon && cacheFantailVMA3[idx].EqualsInput(input))
                        return cacheFantailVMA3[idx];

            lock (checkFantailVMA3)
            {
                checkFantailVMA3.ADXPeriod = aDXPeriod;
                aDXPeriod = checkFantailVMA3.ADXPeriod;
                checkFantailVMA3.MAPeriod = mAPeriod;
                mAPeriod = checkFantailVMA3.MAPeriod;
                checkFantailVMA3.Weighting = weighting;
                weighting = checkFantailVMA3.Weighting;

                if (cacheFantailVMA3 != null)
                    for (int idx = 0; idx < cacheFantailVMA3.Length; idx++)
                        if (cacheFantailVMA3[idx].ADXPeriod == aDXPeriod && cacheFantailVMA3[idx].MAPeriod == mAPeriod && Math.Abs(cacheFantailVMA3[idx].Weighting - weighting) <= double.Epsilon && cacheFantailVMA3[idx].EqualsInput(input))
                            return cacheFantailVMA3[idx];

                FantailVMA3 indicator = new FantailVMA3();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ADXPeriod = aDXPeriod;
                indicator.MAPeriod = mAPeriod;
                indicator.Weighting = weighting;
                Indicators.Add(indicator);
                indicator.SetUp();

                FantailVMA3[] tmp = new FantailVMA3[cacheFantailVMA3 == null ? 1 : cacheFantailVMA3.Length + 1];
                if (cacheFantailVMA3 != null)
                    cacheFantailVMA3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFantailVMA3 = tmp;
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
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FantailVMA3 FantailVMA3(int aDXPeriod, int mAPeriod, double weighting)
        {
            return _indicator.FantailVMA3(Input, aDXPeriod, mAPeriod, weighting);
        }

        /// <summary>
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        public Indicator.FantailVMA3 FantailVMA3(Data.IDataSeries input, int aDXPeriod, int mAPeriod, double weighting)
        {
            return _indicator.FantailVMA3(input, aDXPeriod, mAPeriod, weighting);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FantailVMA3 FantailVMA3(int aDXPeriod, int mAPeriod, double weighting)
        {
            return _indicator.FantailVMA3(Input, aDXPeriod, mAPeriod, weighting);
        }

        /// <summary>
        /// FantailVMA3
        /// </summary>
        /// <returns></returns>
        public Indicator.FantailVMA3 FantailVMA3(Data.IDataSeries input, int aDXPeriod, int mAPeriod, double weighting)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FantailVMA3(input, aDXPeriod, mAPeriod, weighting);
        }
    }
}
#endregion
