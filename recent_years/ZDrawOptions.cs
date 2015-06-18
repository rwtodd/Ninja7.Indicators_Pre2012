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
    /// Richard Todd. Let's you draw with styles more easily.
    /// </summary>
    [Description("Richard Todd. Lets you draw with styles more easily.")]
    public class ZDrawOptions : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion
		private System.Windows.Forms.Panel drawPanel = null;
		private Color selectedColor = Color.Blue;
		private int selectedSize = 1;
		private System.Drawing.FontStyle selectedFontStyle = FontStyle.Regular;
		private System.Drawing.Drawing2D.DashStyle selectedStyle = System.Drawing.Drawing2D.DashStyle.Solid; 
		private bool toggleSwitch = false;
		private System.Windows.Forms.ToolStripButton hideBtn = null;
		private System.Windows.Forms.ToolStrip strip = null;
		
#region UIStuff
		private void setupFontStyles(System.Windows.Forms.Panel pan, System.Drawing.FontStyle[] styles) {
			var gb = new System.Windows.Forms.FlowLayoutPanel();
			gb.SuspendLayout();
			gb.Margin = new System.Windows.Forms.Padding(1,1,1,1);
			gb.AutoSize = true;
			var fmt = new StringFormat();
			fmt.Alignment = StringAlignment.Center;
			fmt.LineAlignment = StringAlignment.Center;
			
			for(int idx = 0; idx < styles.Length; ++idx) {
				var rb = new System.Windows.Forms.RadioButton();
				rb.Appearance = System.Windows.Forms.Appearance.Button;
				var im = new Bitmap(50,20);
				var graph = Graphics.FromImage(im);
				graph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				var impen = new SolidBrush(ChartControl.AxisColor);
				var thefont = new Font("Georgia",10.0f,styles[idx],GraphicsUnit.Point);
				graph.DrawString("text",thefont,impen,new RectangleF(new Point(0,0),new SizeF(50,20)),fmt);
				rb.Image = im;
				var thisst = styles[idx];
				rb.Click += ((s,e) => { cachedObj = null; selectedFontStyle = thisst; }); 
				rb.Size = im.Size;
				gb.Controls.Add(rb);
				if(idx == 0) { rb.Checked = true; selectedFontStyle = thisst; }
			}
					
			gb.ResumeLayout(false);
			pan.Controls.Add(gb);
		}		
		
		private void setupStyles(System.Windows.Forms.Panel pan, System.Drawing.Drawing2D.DashStyle[] styles) {
			var gb = new System.Windows.Forms.FlowLayoutPanel();
			gb.SuspendLayout();
			gb.Margin = new System.Windows.Forms.Padding(1,1,1,1);
			gb.AutoSize = true;
			
			// SOLID, DASHED, DOTTED
			for(int idx = 0; idx < styles.Length; ++idx) {
				var rb = new System.Windows.Forms.RadioButton();
				rb.Appearance = System.Windows.Forms.Appearance.Button;
				var im = new Bitmap(30,20);
				var graph = Graphics.FromImage(im);
				var impen = new Pen(ChartControl.AxisColor,1.0f);
				impen.DashStyle = styles[idx];
				graph.DrawLine(impen,0,10,30,10);
				rb.Image = im;
				var thisst = styles[idx];
				rb.Click += ((s,e) => { cachedObj = null; selectedStyle = thisst; }); 
				rb.Size = im.Size;
				gb.Controls.Add(rb);
				if(idx == 0) { rb.Checked = true; selectedStyle = thisst; }
			}
			
			
			gb.ResumeLayout(false);
			pan.Controls.Add(gb);
		}

		
		private void setupSizes(System.Windows.Forms.Panel pan, int[] sizes) {
			var gb = new System.Windows.Forms.FlowLayoutPanel();
			gb.SuspendLayout();
			gb.Margin = new System.Windows.Forms.Padding(1,1,1,1);
			gb.AutoSize = true;
			System.Windows.Forms.RadioButton rb = null;
			var dpen = new Pen(ChartControl.AxisColor);
			for(int idx = 0; idx < sizes.Length; ++idx) {
				rb = new System.Windows.Forms.RadioButton();
				rb.Appearance = System.Windows.Forms.Appearance.Button;
				var im = new Bitmap(20,20);
				var graph = Graphics.FromImage(im);
				float wid = 10.0f*sizes[idx]/sizes[sizes.Length-1];
				float pad = (20.0f - wid)/2.0f;
				graph.DrawRectangle(dpen,pad,pad,wid,wid);
				rb.Image = im;
				var thissz = sizes[idx]; // necessary for the Lambda below to work..
				rb.Click += ((s,e) => { cachedObj = null; selectedSize = thissz; }); 
				rb.Size = im.Size;
				gb.Controls.Add(rb);
				if(idx == 0) { rb.Checked = true; selectedSize = thissz; }
			}
			gb.ResumeLayout(false);
			pan.Controls.Add(gb);
		}
		
		private void setupColorList(System.Windows.Forms.Panel pan, Color[] colors) {
			var gb = new System.Windows.Forms.FlowLayoutPanel();
			gb.SuspendLayout();
			
			gb.Margin = new System.Windows.Forms.Padding(1,1,1,1);
			gb.AutoSize = true;
			
			//gb.Dock = System.Windows.Forms.DockStyle.Left;
			var brush = new System.Drawing.SolidBrush(Color.Blue);
			System.Windows.Forms.RadioButton rb = null;
			foreach(Color c in colors) {
				rb = new System.Windows.Forms.RadioButton();
				rb.Appearance = System.Windows.Forms.Appearance.Button;
				var im = new Bitmap(20,20);
				var graph = Graphics.FromImage(im);
				brush.Color = c;
				graph.FillRectangle(brush,0,0,20,20);
				rb.Image = im;
				var thiscol = c; // necessary for the Lambda below to work..
				rb.Click += ((s,e) => { cachedObj = null; selectedColor = thiscol; }); 
				rb.Size = im.Size;
				gb.Controls.Add(rb);
		    }
			rb.Checked = true;
			selectedColor = colors[colors.Length-1];
			gb.ResumeLayout(false);
			pan.Controls.Add(gb);
		}
#endregion		

		protected override void OnStartUp() {
			var colors = new Color[] { 
				Color.Black, 
				Color.White, 
				Color.Gray,
				Color.Firebrick,
				Color.Red,
				Color.OrangeRed,
				Color.Orange,
				Color.Goldenrod,
				Color.Yellow,
				Color.YellowGreen,
				Color.Green,
				Color.DarkCyan,
				Color.DarkBlue,
				Color.Blue,
				Color.Indigo,
				Color.Violet
			};
			
			// these must be in order from smallest to largest...
			var sizes = new int[] { 1, 2, 3, 4 };
			
			var styles = new System.Drawing.Drawing2D.DashStyle[] {
				System.Drawing.Drawing2D.DashStyle.Solid,
				System.Drawing.Drawing2D.DashStyle.Dash,
				System.Drawing.Drawing2D.DashStyle.DashDot,
				System.Drawing.Drawing2D.DashStyle.DashDotDot,
				System.Drawing.Drawing2D.DashStyle.Dot
			};
			
			var fstyles = new System.Drawing.FontStyle[] { 
				System.Drawing.FontStyle.Regular,
				System.Drawing.FontStyle.Bold,
				System.Drawing.FontStyle.Italic 
			};
			
			drawPanel = new System.Windows.Forms.FlowLayoutPanel();
			drawPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			drawPanel.AutoSize = true; 
			drawPanel.SuspendLayout();

			cachedObj = null;

			setupColorList(drawPanel, colors);
			setupSizes(drawPanel, sizes);
			setupStyles(drawPanel,styles);
			setupFontStyles(drawPanel,fstyles);
			
			drawPanel.ResumeLayout(false);
			ChartControl.Controls.Add(drawPanel);
			
			if(toggleSwitch) {
				System.Windows.Forms.Control[] coll = ChartControl.Controls.Find("tsrTool",false);
				if(coll.Length > 0) {
					hideBtn = new System.Windows.Forms.ToolStripButton("+DrawOpts+");
					hideBtn.Click += hideShowPanel;
					strip = (System.Windows.Forms.ToolStrip)coll[0];
					strip.Items.Add(hideBtn);   
					drawPanel.Hide();
				}  
			}
			
		}
		private void hideShowPanel(object s, EventArgs e) {
			if(drawPanel == null) return;

			if(drawPanel.Visible) {
				drawPanel.Hide();
				hideBtn.Text = "+DrawOpts+";
			    ChartControl.ChartObjects.InsertComplete -= objects_added;			
			} else {
				drawPanel.Show();
				hideBtn.Text = "-DrawOpts-";
			 	cachedObj = null;
			    ChartControl.ChartObjects.InsertComplete += objects_added;
			}
		}		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }
		
		
		private object cachedObj = null;
		private void objects_added(object sender, CollectionEventArgs e) {
			if(e.Value != cachedObj) {
			   cachedObj = e.Value;

//			   Print(e.Value.GetType().ToString());
//			   foreach(var t in e.Value.GetType().FindInterfaces( (m,o)=>true, null)) {
//			    Print("... "+ t.ToString());
//			   }
				
			   var cl = e.Value as NinjaTrader.Gui.Chart.ILine;
			   if(cl != null) {
				   cl.Pen.Color = selectedColor;
				   cl.Pen.Width = selectedSize;
				   cl.Pen.DashStyle = selectedStyle;
				   return;
			   } 
			   var cs = e.Value as NinjaTrader.Gui.Chart.IShape;
			   if(cs != null) {
				   cs.Pen.Color = selectedColor;
				   cs.Pen.Width = selectedSize;
				   cs.Pen.DashStyle = selectedStyle;
				   cs.AreaColor = selectedColor;
				   return;				  
			   }
							
				var ctxt = e.Value as NinjaTrader.Gui.Chart.IText;
			    if(ctxt != null) {
				  ctxt.Font = new Font(ctxt.Font.FontFamily,6.0f+selectedSize*4.0f,selectedFontStyle);
				  ctxt.TextColor = selectedColor;
				  return;
				}
				
				var cmark = e.Value as NinjaTrader.Gui.Chart.IMarker;
			    if(cmark != null) {
				  cmark.Color = selectedColor;
				  return;	
				}

				var tc = e.Value as NinjaTrader.Gui.Chart.ChartTrendChannel;
			    if(tc != null) {
				  foreach(FibonacciLevel lvl in tc.Levels) {
					lvl.Pen.Color = selectedColor;
					lvl.Pen.Width = selectedSize;
					lvl.Pen.DashStyle = selectedStyle;
				  }
				  return;
				}
				var fr = e.Value as NinjaTrader.Gui.Chart.FibonacciRetracements;
			    if(fr != null) {
				  foreach(FibonacciLevel lvl in fr.Levels) {
					lvl.Pen.Color = selectedColor;
					lvl.Pen.Width = selectedSize;
					lvl.Pen.DashStyle = selectedStyle;
				  }
				  return;
				}				

			}
		    //else {
			//  Print("...SAME");	
			//}
		}
		
		protected override void OnTermination() {
			if(String.Equals(hideBtn.Text,"-DrawOpts-")) {
			  ChartControl.ChartObjects.InsertComplete -= objects_added;	
			}
			ChartControl.Controls.Remove(drawPanel);
			drawPanel.Dispose();
			if(strip != null) {
  				strip.Items.Remove(hideBtn);
				hideBtn.Dispose();
			}
			base.OnTermination();
			
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
        }

        #region Properties
	[Description("Include ability to hide drawOptions?")]
	[GridCategory("Parameters")]
	public bool ToggleSwitch
	{
 	 get { return toggleSwitch; }
 	 set { toggleSwitch = value;
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
        private ZDrawOptions[] cacheZDrawOptions = null;

        private static ZDrawOptions checkZDrawOptions = new ZDrawOptions();

        /// <summary>
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        public ZDrawOptions ZDrawOptions(bool toggleSwitch)
        {
            return ZDrawOptions(Input, toggleSwitch);
        }

        /// <summary>
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        public ZDrawOptions ZDrawOptions(Data.IDataSeries input, bool toggleSwitch)
        {
            if (cacheZDrawOptions != null)
                for (int idx = 0; idx < cacheZDrawOptions.Length; idx++)
                    if (cacheZDrawOptions[idx].ToggleSwitch == toggleSwitch && cacheZDrawOptions[idx].EqualsInput(input))
                        return cacheZDrawOptions[idx];

            lock (checkZDrawOptions)
            {
                checkZDrawOptions.ToggleSwitch = toggleSwitch;
                toggleSwitch = checkZDrawOptions.ToggleSwitch;

                if (cacheZDrawOptions != null)
                    for (int idx = 0; idx < cacheZDrawOptions.Length; idx++)
                        if (cacheZDrawOptions[idx].ToggleSwitch == toggleSwitch && cacheZDrawOptions[idx].EqualsInput(input))
                            return cacheZDrawOptions[idx];

                ZDrawOptions indicator = new ZDrawOptions();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ToggleSwitch = toggleSwitch;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZDrawOptions[] tmp = new ZDrawOptions[cacheZDrawOptions == null ? 1 : cacheZDrawOptions.Length + 1];
                if (cacheZDrawOptions != null)
                    cacheZDrawOptions.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZDrawOptions = tmp;
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
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZDrawOptions ZDrawOptions(bool toggleSwitch)
        {
            return _indicator.ZDrawOptions(Input, toggleSwitch);
        }

        /// <summary>
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZDrawOptions ZDrawOptions(Data.IDataSeries input, bool toggleSwitch)
        {
            return _indicator.ZDrawOptions(input, toggleSwitch);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZDrawOptions ZDrawOptions(bool toggleSwitch)
        {
            return _indicator.ZDrawOptions(Input, toggleSwitch);
        }

        /// <summary>
        /// Richard Todd. Lets you draw with styles more easily.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZDrawOptions ZDrawOptions(Data.IDataSeries input, bool toggleSwitch)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZDrawOptions(input, toggleSwitch);
        }
    }
}
#endregion
