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

namespace rwt {
  public class ExtendedData {
     public long UpTicks, DnTicks;
     public long dOpen, dClose, dHigh, dLow;
     public DateTime dt;
     public long UpCount, DnCount; 

     // for footprints...
     private long[] footprintUP;
     private long[] footprintDN;
     private double topPrice;

     public ExtendedData(DateTime tm, ExtendedData ed) {
         dt = tm;
         if(ed != null) {
           dOpen = ed.dClose;
           dHigh = ed.dClose;
           dLow = ed.dClose;
           dClose = ed.dClose;
           UpTicks = 0; DnTicks = 0;
           UpCount = 0; DnCount = 0;
         }

         footprintUP = new long[8];
         footprintDN = new long[8];
         Array.Clear(footprintUP,0,8);
         Array.Clear(footprintDN,0,8);
         topPrice = -1;
     }
 
    public long[] checkIntegrity(out double tp) {
        tp= topPrice;
        return footprintUP;
    }

    public long findLargestLevel(bool combined) {
       long ans = 1;
       if(combined) {
          for(int idx = 0; idx < footprintUP.Length; ++idx) {
              ans = Math.Max(ans,footprintUP[idx]+footprintDN[idx]);
          }
       } else {
          foreach(var t in footprintUP) { if(ans < t) ans = t; }
          foreach(var t in footprintDN) { if(ans < t) ans = t; }
       }
       return ans;
    }

     // for footprints
		private int memLookupIndex(double price, double tickSize) {
       if(topPrice < 0) {
           topPrice = price + 4*tickSize;
       }

		   int rawIndex = (int)(Math.Round((topPrice - price)/tickSize));

		   if((rawIndex >= 0) && (rawIndex < footprintUP.Length))
			  return rawIndex;

       int oldlen = footprintUP.Length;
		
			// need to shift things around...
		   if(rawIndex > 0) {
         // add to the end...
         int newlen = Math.Max(rawIndex + 1,(int)(footprintUP.Length*1.5)+1);
         Array.Resize<long>(ref footprintUP, newlen);
         Array.Resize<long>(ref footprintDN, newlen);
                   
         Array.Clear(footprintUP,oldlen,newlen-oldlen);
         Array.Clear(footprintDN,oldlen,newlen-oldlen);

         return rawIndex;
       } else {
         // add to the front...
         int newlen = Math.Max(footprintUP.Length - rawIndex, (int)(footprintUP.Length*1.5)+1);
         topPrice += (newlen - footprintUP.Length)*tickSize;
         long[] bigger = new long[newlen];
         Array.Clear(bigger,0,newlen-footprintUP.Length);
         Array.Copy(footprintUP,0,bigger,newlen - footprintUP.Length,footprintUP.Length);
         footprintUP = bigger;
         bigger = new long[newlen];
         Array.Clear(bigger,0,newlen-footprintDN.Length);
         Array.Copy(footprintDN,0,bigger,newlen - footprintDN.Length,footprintDN.Length);
         footprintDN = bigger;
       }
		   return (int)(Math.Round((topPrice - price)/tickSize));
    }

     public void getUpDnTicksAtPrice(double tickSize, double price, out long up, out long dn) {
		    int rawIndex = (int)(Math.Round((topPrice - price)/tickSize));
		    if((rawIndex >= 0) && (rawIndex < footprintUP.Length)) {
           up = footprintUP[rawIndex];
           dn = footprintDN[rawIndex];
        } else {
           up = 0;
           dn = 0;
        }
     }
  
     public void addVolume(double close, long vol, double tickSize) {
         // + vol for upticks   - vol for downticks
         var nd = dClose + vol;
         if(nd > dHigh) dHigh = nd;
         else if(nd < dLow) dLow = nd;
         dClose = nd;

         var idx = memLookupIndex(close,tickSize);
         // DEBUG if(idx < 0 || idx >= footprintUP.Length) throw new Exception("Bad idx "+idx+" topprice "+topPrice+" len "+footprintUP.Length);

         if(vol > 0) {
            UpTicks += vol;
            footprintUP[idx] += vol;
         }
         else {
            DnTicks -= vol;
            footprintDN[idx] -= vol;
         }

     }

     public void addLevel(int dir) {
         if(dir > 0) {
            UpCount += 1;
         }
         else {
            DnCount += 1;
         }
     }
  }

  public interface IExtendedData {
          ExtendedData getExtraData(int barsBack, NinjaTrader.Data.Bars bars, int cb);
  }
}
