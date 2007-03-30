using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.Reflection;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
	/// <summary>
	/// Control designer of the graph-control
	/// </summary>
    internal class DiagramControlDesigner : ControlDesigner
	{
		

		#region Properties

        /// <summary>
        /// the AnotherOne field
        /// </summary>
        private int mAnotherOne;
        /// <summary>
        /// Gets or sets the AnotherOne
        /// </summary>
        [Browsable(true)]
        public int AnotherOne
        {
            get { return mAnotherOne; }
            set { mAnotherOne = value; }
        }
	
		/// <summary>
		/// Gets the verbs of the control
		/// </summary>
		public override System.ComponentModel.Design.DesignerVerbCollection Verbs
		{
			get
			{		
				DesignerVerbCollection col=new DesignerVerbCollection();
				col.Add(new DesignerVerb("About",new EventHandler(About)));
				col.Add(new DesignerVerb("Help",new EventHandler(NetronSite)));
				return col;
			}
		}
		#endregion

		#region Constructor
		public DiagramControlDesigner()
		{
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.Diagramming.Core.UI.AboutSplash.jpg");
					
            //bmp= Bitmap.FromStream(stream) as Bitmap;
            //stream.Close();
            //stream=null;

		}
		#endregion

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                // Create action list collection
                DesignerActionListCollection actionLists = new DesignerActionListCollection();

                // Add custom action list
                actionLists.Add(new DiagramControlDesignerActionList(this.Component));

                // Return to the designer action service
                return actionLists;
            }
        }

		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize (component);
			(component as Control).AllowDrop = false;
			(component as DiagramControl).BackColor = Color.White;
			//(component as GraphControl).EnableContextMenu = true;
		}
		

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			base.OnPaintAdornments (pe);
            Graphics g = pe.Graphics;

            #region Version Info
            g.TranslateTransform(200, 200);
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
			g.DrawString("Netron Light Version " + ass.GetName().Version.ToString(),ArtPallet.DefaultFont,Brushes.DimGray,new PointF(50,130));
            #endregion

            #region Some help for the uninitiated

            g.DrawString("Control and shapes properties at run-time:", ArtPallet.DefaultBoldFont, Brushes.DimGray, 50, 300);
			g.DrawString( "The properties of the diagram control and diagram entities are accessible via the PropertyGrid, you need to connect the graph control to the PropertyGrid via the OnShowCanvasProperties event.",ArtPallet.DefaultFont, Brushes.DimGray,new Rectangle(50,320, 500,300));

            g.DrawString("Design-time properties:", ArtPallet.DefaultBoldFont, Brushes.DimGray, 50, 370);
            g.DrawString("You can customize the control inside Visual Studio by means of the PropertyGrid and the SmartTag. Note in particular the events you can handle, see the Events tab in the PropertyGrid or shift to code-view to see the API of the diagram control.", ArtPallet.DefaultFont, Brushes.DimGray, new Rectangle(50, 390, 500, 360));

            g.DrawString("Diagram library API:", ArtPallet.DefaultBoldFont, Brushes.DimGray, 50, 450);
            g.DrawString("The control is just the UI portion of the diagramming library, there is a rich document object model inside the library as well as many utilities and graph analysis functionalities. Switch to code-view and start exploring the API!", ArtPallet.DefaultFont, Brushes.DimGray, new Rectangle(50, 470, 500, 360));
            #endregion
        }


		

		
		private void About(object sender, EventArgs e)
		{
			//Form frm = new Netron.GraphLib.AboutForm(true);
			//frm.ShowDialog();
		}

		private void NetronSite(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.netronproject.com");
		}
	
	}
}
