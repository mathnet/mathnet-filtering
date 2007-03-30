using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This shape allows you to use any picture as inside a rectangular area.
	/// </summary>	
    public partial class ImageShape : SimpleShapeBase
	{
		#region Fields

		#region the connectors
        private Connector cBottom, cLeft, cRight, cTop;	
		private string mImagePath;
		private Image mImage;
		#endregion
		#endregion
		
		#region Properties
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get
            {
                return "Image shape";
            }
        }
        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        /// <value>The image path.</value>
		public string ImagePath
		{
			get{return mImagePath;}
			set{
				if(value==string.Empty) return;
				if(File.Exists(value))
				{
					try
					{
						mImage = Image.FromFile(value);
						Transform(Rectangle.X, Rectangle.Y, mImage.Width, mImage.Height);
					}
					catch(Exception exc)
					{
                        Trace.WriteLine(exc.Message);
					}
					mImagePath = value;
				}
				else
					MessageBox.Show("The specified file does not exist.","Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
		public Image Image
		{
			get{return mImage;}
			set{
				if(value==null) return;
				mImage = value;
				mImagePath = string.Empty;
				Transform(Rectangle.X, Rectangle.Y, mImage.Width, mImage.Height);			
			}
		}
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageShape() : base()
		{
            AddConnectors();
		}
		/// <summary>
		/// This is the default constructor of the class.
		/// </summary>
		public ImageShape(IModel site) : base(site)
		{
            AddConnectors();
		}
        /// <summary>
        /// Adds the connectors.
        /// </summary>
        public void AddConnectors()
        {
            //the initial size
            Transform(0, 0, 200, 50);
            #region Connectors
            cTop = new Connector(new Point((int) (Rectangle.Left + Rectangle.Width / 2), Rectangle.Top), Model);
            cTop.Name = "Top connector";
            cTop.Parent = this;
            Connectors.Add(cTop);

            cRight = new Connector(new Point(Rectangle.Right, (int) (Rectangle.Top + Rectangle.Height / 2)), Model);
            cRight.Name = "Right connector";
            cRight.Parent = this;
            Connectors.Add(cRight);

            cBottom = new Connector(new Point((int) (Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
            cBottom.Name = "Bottom connector";
            cBottom.Parent = this;
            Connectors.Add(cBottom);

            cLeft = new Connector(new Point(Rectangle.Left, (int) (Rectangle.Top + Rectangle.Height / 2)), Model);
            cLeft.Name = "Left connector";
            cLeft.Parent = this;
            Connectors.Add(cLeft);
            #endregion
        }
		
		#endregion	

		#region Properties
		
	
		#endregion

		#region Methods
		/// <summary>
		/// Adds additional stuff to the shape's menu
		/// </summary>
		/// <returns></returns>
		public override MenuItem[] ShapeMenu()
		{
	
			MenuItem item= new MenuItem("Reset image size",new EventHandler(OnResetImageSize));
			MenuItem[] items = new MenuItem[]{item};

			return items;
		}
		/// <summary>
		/// Resets the original image size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnResetImageSize(object sender, EventArgs e)
		{
			Transform(Rectangle.X, Rectangle.Y, mImage.Width, mImage.Height);
			this.Invalidate();
		}

		/// <summary>
		/// Overrides the default bitmap used in the shape viewer
		/// </summary>
		/// <returns></returns>
		public  Bitmap GetThumbnail()
		{
			Bitmap bmp=null;
			try
			{
				Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.GraphLib.BasicShapes.Resources.ImageShape.gif");
					
				bmp= Bitmap.FromStream(stream) as Bitmap;
				stream.Close();
				stream=null;
			}
			catch(Exception exc)
			{
				Trace.WriteLine(exc.Message,"ImageShape.GetThumbnail");
			}
			return bmp;
		}
		
		/// <summary>
		/// Paints the shape of the person object in the plex. Here you can let your imagination go.
		/// MAKE IT PERFORMANT, this is a killer method called 200.000 times a minute!
		/// </summary>
		/// <param name="g">The graphics canvas onto which to paint</param>
		public override void Paint(Graphics g)
		{
			base.Paint(g);
            //if(RecalculateSize)
            //{
            //    Rectangle = new RectangleF(new PointF(Rectangle.X,Rectangle.Y),
            //        g.MeasureString(this.Text,Font));	
            //    Rectangle = System.Drawing.RectangleF.Inflate(Rectangle,10,10);
            //    RecalculateSize = false; //very important!
            //}
			if(mImage==null)
			{
				g.FillRectangle(Brush, Rectangle);
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				g.DrawString("Image shape", ArtPallet.DefaultFont, Brushes.Black, Rectangle.X + (Rectangle.Width / 2), Rectangle.Y + 3, sf);
			}
			else
				g.DrawImage(mImage, Rectangle);			

			
				
					
			
		}

	


	
		


	


		


		
		#endregion
	}

}







		
