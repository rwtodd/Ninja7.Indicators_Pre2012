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
using System.Windows.Forms;
using System.Collections.Generic;
#endregion
using Microsoft.Office.Interop;

namespace RWT {
  public class Annotation {
    private IText marker;
	private int num;
	private string mtxt;
	
	public Annotation(int number, IText mkr) {
		marker = mkr;
	  	Number = number;
		mtxt = "";
	}
	
	public int Number {
	  get { return num; }
	  set { 
			num = value; 
		    marker.Text = num.ToString();
	  }
	}
	
	public void removeMarker(NinjaTrader.Indicator.Indicator i) {
	  i.RemoveDrawObject(marker);
      marker = null;
	  num = -1;
	  mtxt = null;
	}
	
	public void lookLike(Annotation other) {
	   marker.TextColor = other.marker.TextColor;
	   marker.Font = other.marker.Font;
	}
	
	public string Text {
	  get { return mtxt; }
	  set { mtxt = value; }
	}
	
	public int sortOrder() { return -marker.BarsAgo; }
	
  }
}
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Richard Todd. Annotate a chart and export to MS Word
    /// </summary>
    [Description("Richard Todd. Annotate a chart and export to MS Word")]
    public class ZAnnotate2 : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion

		private List<RWT.Annotation> anns;
		private int tagno;
		
		// for the toolstrip stuff...
		private System.Windows.Forms.ToolStrip strip = null;
		private System.Windows.Forms.ToolStripButton hideBtn = null;		
		private System.Windows.Forms.Button newBtn = null;		
		private System.Windows.Forms.Button reorgBtn = null;
		private System.Windows.Forms.Button publishBtn = null;
		private System.Windows.Forms.Button redrawBtn = null;
		private System.Windows.Forms.Panel annotPanel = null;
		private System.Windows.Forms.Splitter theSplitter = null;
		private System.Windows.Forms.DockStyle dockWhere = System.Windows.Forms.DockStyle.Left;
		private System.Windows.Forms.DataGridView dgv = null;
		private System.Windows.Forms.BindingSource bs = null;
		private System.Windows.Forms.TextBox didWell = null;
		private System.Windows.Forms.TextBox needsImprovement = null;
		
		private void reOrderAnnotations() {
		  	anns.Sort( (x,y) => x.sortOrder().CompareTo(y.sortOrder()) );
			// renumber the list...
			for(int idx = 0; idx < anns.Count; ++idx) {
				anns[idx].Number = (idx+1);
			}
			bs.ResetBindings(false);
			ChartControl.ChartPanel.Invalidate(false);
		}

		private void createAnnotation() {
			var whichbar = CurrentBar - ChartControl.LastBarPainted;
			var myt = DrawText("anntxt"+tagno.ToString(),"x",
				               whichbar,Low[whichbar]-TickSize,
				               ChartControl.ChartStyle.Pen.Color);
			++tagno; 
			myt.AutoScale = true;
			myt.Locked = false;
			bs.Add(new RWT.Annotation(anns.Count+1,myt));
			ChartControl.ChartPanel.Invalidate(false);
		}
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }

		protected override void OnStartUp()
		{
			base.OnStartUp();
			anns = new List<RWT.Annotation>();
			tagno = 0;
			
			theSplitter = new System.Windows.Forms.Splitter();
			theSplitter.Dock = dockWhere;
			theSplitter.Width = 2;
			theSplitter.BackColor = Color.Gray;
			ChartControl.Controls.Add(theSplitter);
			annotPanel = new Panel();
			annotPanel.Dock = dockWhere;
			
			dgv = new DataGridView();
			bs = new BindingSource();
			bs.AllowNew = false;
			bs.DataSource = anns;
			dgv.DataSource = bs;
			//dgv.Columns[0].ReadOnly = true;
			dgv.Dock = System.Windows.Forms.DockStyle.Fill;
			annotPanel.Controls.Add(dgv);
			
			didWell = new System.Windows.Forms.TextBox();
			didWell.Text = "Talk about what you did well today.";
			didWell.Dock =  System.Windows.Forms.DockStyle.Bottom;
			didWell.AcceptsReturn = true;
			didWell.WordWrap = true;
			didWell.Multiline = true;
			didWell.MaxLength = 4096;
			didWell.Height = 80;
			didWell.ScrollBars = ScrollBars.Vertical;
			annotPanel.Controls.Add(didWell);
			
			needsImprovement = new System.Windows.Forms.TextBox();
			needsImprovement.Text = "Talk about what need to improve today.";
			needsImprovement.Dock =  System.Windows.Forms.DockStyle.Bottom;
			needsImprovement.AcceptsReturn = true;
			needsImprovement.WordWrap = true;
			needsImprovement.Multiline = true;
			needsImprovement.MaxLength = 4096;
			needsImprovement.Height = 80;
			needsImprovement.ScrollBars = ScrollBars.Vertical;
			annotPanel.Controls.Add(needsImprovement);
			
			newBtn = new System.Windows.Forms.Button();
			newBtn.Text = "New Annotation";
			newBtn.Click += newBtn_Click;
			newBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
			annotPanel.Controls.Add(newBtn);
			
			reorgBtn = new System.Windows.Forms.Button();
			reorgBtn.Text = "Renumber Annotations";
			reorgBtn.Click += reorgBtn_Click;
			reorgBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
			annotPanel.Controls.Add(reorgBtn);
			
			redrawBtn = new System.Windows.Forms.Button();
			redrawBtn.Text = "Redraw Annotations";
			redrawBtn.Click += redrawBtn_Click;
			redrawBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
			annotPanel.Controls.Add(redrawBtn);
			
			publishBtn = new System.Windows.Forms.Button();
			publishBtn.Text = "Publish";
			publishBtn.Click += publishBtn_Click;
			publishBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
			annotPanel.Controls.Add(publishBtn);
			
			ChartControl.Controls.Add(annotPanel);
			
			System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
			if(coll.Length > 0) {
				hideBtn = new System.Windows.Forms.ToolStripButton("-Notes-");
				hideBtn.Click += hideShowPanel;
				strip = (System.Windows.Forms.ToolStrip)coll[0];
				strip.Items.Add(hideBtn);   
			}  
			
			dgv.Columns[0].ReadOnly = true;
			dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dgv.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dgv.UserDeletingRow += dgv_UserDeletingRow;
		}

		private void hideShowPanel(object s, EventArgs e) {
			if(annotPanel == null) return;

			if(annotPanel.Visible) {
				annotPanel.Hide();
				theSplitter.Hide();
				hideBtn.Text = "+Notes+";
			} else {
				theSplitter.Show();
				annotPanel.Show();
				hideBtn.Text = "-Notes-";
			}
		}		
		private void newBtn_Click(object s, EventArgs e) {
			createAnnotation();
		}
		private void redrawBtn_Click(object s, EventArgs e) {
			// make them all look like the last one...
			if(anns.Count <= 1) return;
			
			var annot = anns[anns.Count-1];
			for(int i = 0; i < (anns.Count-1); ++i) {
			  anns[i].lookLike(annot);	
			}
			ChartControl.ChartPanel.Invalidate(false);

		}
		
		private void reorgBtn_Click(object s, EventArgs e) {
			reOrderAnnotations();
		}
		
		private void dgv_UserDeletingRow(object sender,
			DataGridViewRowCancelEventArgs e)
		{
			foreach(DataGridViewRow r in dgv.SelectedRows) {
				((RWT.Annotation)r.DataBoundItem).removeMarker(this);			  	
			}
			ChartControl.ChartPanel.Invalidate(false);
			
		}		
		

		private void publishBtn_Click(object s, EventArgs e) {
			var fselect = new System.Windows.Forms.SaveFileDialog();
			fselect.DefaultExt = ".docx";
			fselect.AddExtension = true;
			fselect.Filter = "Word2010 (*.docx)|*.docx";
			fselect.Title = "Save As...";
			fselect.FileOk += publishBtn_OK;
			fselect.ShowHelp = false;
			fselect.OverwritePrompt = false;
			fselect.ShowDialog();
		}
		
		private void publishBtn_OK(object sender, 
		System.ComponentModel.CancelEventArgs e)
	    {
			var fselect = sender as System.Windows.Forms.SaveFileDialog;
			if(fselect == null)  {
				MessageBox.Show("PROBLEM!");
		        return;
			}
			
			var bm = new Bitmap(ChartControl.ChartPanel.Width,ChartControl.ChartPanel.Height);
			ChartControl.ChartPanel.DrawToBitmap(bm,new Rectangle(0,0,bm.Width,bm.Height));
			var chartName = System.IO.Path.GetTempPath() + "\\ninjachart.png";
			var strm = new System.IO.FileStream(chartName,System.IO.FileMode.Create);
			bm.Save(strm,System.Drawing.Imaging.ImageFormat.Png);
			strm.Close();
			//MessageBox.Show(System.IO.Path.GetTempPath());
			var oword = new Microsoft.Office.Interop.Word.Application();
			oword.Visible = true;

			//var odoc = new Microsoft.Office.Interop.Word.Document();
			object oMissing = System.Reflection.Missing.Value;
			object strFilename = fselect.FileName; 
			var odoc = oword.Documents.Open(ref strFilename,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			
			// go to the end..
			odoc.Characters.Last.Select();
			oword.Selection.Collapse(ref oMissing);
			object brktype = 7;
			oword.Selection.InsertBreak(ref brktype);
			object strStyle = "Heading 1";
			oword.Selection.set_Style(ref strStyle);
			oword.Selection.TypeText(DateTime.Now.ToLongDateString());
			oword.Selection.TypeParagraph();
			strStyle = "Heading 2";
			oword.Selection.set_Style(ref strStyle);
			oword.Selection.TypeText(Instrument.ToString());
			oword.Selection.TypeParagraph();
			
			object oFalse = false;
			object oTrue = true;
			oword.Selection.Collapse(ref oMissing);
			object cur = oword.Selection.Range;
			odoc.InlineShapes.AddPicture(chartName,ref oFalse,ref oTrue,ref cur);
			odoc.Characters.Last.Select();
			oword.Selection.Collapse(ref oMissing);
			oword.Selection.TypeParagraph();
//			object numOne = 1;
//			oword.ListGalleries[Microsoft.Office.Interop.Word.WdListGalleryType.wdNumberGallery].ListTemplates[ref numOne].Name = "";
			object listbehav = Microsoft.Office.Interop.Word.WdDefaultListBehavior.wdWord10ListBehavior;
			oword.Selection.Range.ListFormat.ApplyNumberDefault(ref listbehav);
			foreach(RWT.Annotation a in anns) {
			  oword.Selection.TypeText(a.Text);
			  oword.Selection.TypeParagraph();
			}
			object numtyp = Microsoft.Office.Interop.Word.WdNumberType.wdNumberParagraph;
			oword.Selection.Range.ListFormat.RemoveNumbers(ref numtyp);
			if(didWell.TextLength > 0) {
			  oword.Selection.TypeText("What I did well: ");
			  oword.Selection.TypeText(didWell.Text);
			  oword.Selection.TypeParagraph();
			}
			if(needsImprovement.TextLength > 0) {
			  oword.Selection.TypeText("What I need to improve: ");
			  oword.Selection.TypeText(needsImprovement.Text);
			  oword.Selection.TypeParagraph();
			}
            // http://www.c-sharpcorner.com/UploadFile/amrish_deep/WordAutomation05102007223934PM/WordAutomation.aspx
		}
		
		protected override void OnTermination() 
		{
			if(annotPanel != null) { ChartControl.Controls.Remove(annotPanel); annotPanel.Dispose(); }
			if(theSplitter != null) { ChartControl.Controls.Remove(theSplitter); theSplitter.Dispose(); }
         
			if(strip != null) {
  				strip.Items.Remove(hideBtn);
				hideBtn.Dispose();
			}

			dgv = null;
			theSplitter = null;
			strip = null;
			hideBtn = null;
			newBtn = null;
			reorgBtn = null;
			didWell = null;
			needsImprovement = null;
			base.OnTermination();
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			
        }

        #region Properties
	[Description("Where to dock the panel?")]
	[GridCategory("Parameters")]
	public System.Windows.Forms.DockStyle DockWhere
	{
 	 get { return dockWhere; }
 	 set { if((value != System.Windows.Forms.DockStyle.Fill) &&
             (value != System.Windows.Forms.DockStyle.None)) 
                    dockWhere = value;
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
        private ZAnnotate2[] cacheZAnnotate2 = null;

        private static ZAnnotate2 checkZAnnotate2 = new ZAnnotate2();

        /// <summary>
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        public ZAnnotate2 ZAnnotate2(System.Windows.Forms.DockStyle dockWhere)
        {
            return ZAnnotate2(Input, dockWhere);
        }

        /// <summary>
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        public ZAnnotate2 ZAnnotate2(Data.IDataSeries input, System.Windows.Forms.DockStyle dockWhere)
        {
            if (cacheZAnnotate2 != null)
                for (int idx = 0; idx < cacheZAnnotate2.Length; idx++)
                    if (cacheZAnnotate2[idx].DockWhere == dockWhere && cacheZAnnotate2[idx].EqualsInput(input))
                        return cacheZAnnotate2[idx];

            lock (checkZAnnotate2)
            {
                checkZAnnotate2.DockWhere = dockWhere;
                dockWhere = checkZAnnotate2.DockWhere;

                if (cacheZAnnotate2 != null)
                    for (int idx = 0; idx < cacheZAnnotate2.Length; idx++)
                        if (cacheZAnnotate2[idx].DockWhere == dockWhere && cacheZAnnotate2[idx].EqualsInput(input))
                            return cacheZAnnotate2[idx];

                ZAnnotate2 indicator = new ZAnnotate2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DockWhere = dockWhere;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZAnnotate2[] tmp = new ZAnnotate2[cacheZAnnotate2 == null ? 1 : cacheZAnnotate2.Length + 1];
                if (cacheZAnnotate2 != null)
                    cacheZAnnotate2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZAnnotate2 = tmp;
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
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZAnnotate2 ZAnnotate2(System.Windows.Forms.DockStyle dockWhere)
        {
            return _indicator.ZAnnotate2(Input, dockWhere);
        }

        /// <summary>
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        public Indicator.ZAnnotate2 ZAnnotate2(Data.IDataSeries input, System.Windows.Forms.DockStyle dockWhere)
        {
            return _indicator.ZAnnotate2(input, dockWhere);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZAnnotate2 ZAnnotate2(System.Windows.Forms.DockStyle dockWhere)
        {
            return _indicator.ZAnnotate2(Input, dockWhere);
        }

        /// <summary>
        /// Richard Todd. Annotate a chart and export to MS Word
        /// </summary>
        /// <returns></returns>
        public Indicator.ZAnnotate2 ZAnnotate2(Data.IDataSeries input, System.Windows.Forms.DockStyle dockWhere)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZAnnotate2(input, dockWhere);
        }
    }
}
#endregion
