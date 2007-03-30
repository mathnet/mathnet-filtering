using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// A simple rectangular bundle
	/// </summary>
    [Shape("Rectangle", "SimpleRectangle", "Standard", "A simple rectangular shape.")]  
    public partial class SimpleRectangle : SimpleShapeBase
	{
		#region Fields
		/// <summary>
		/// holds the bottom connector
		/// </summary>
		Connector cBottom, cLeft, cRight, cTop;
		#endregion

        #region Properties
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "Simple Rectangle"; }
        }
        #endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="s"></param>
		public  SimpleRectangle(IModel s) : base(s)
		{
            AddConnectors();
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleRectangle"/> class.
        /// </summary>
         public SimpleRectangle()
             : base()
         {
             AddConnectors();
         }

         private void AddConnectors()
         {
             cTop = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Top), Model);
             cTop.Name = "Top connector";
             Connectors.Add(cTop);

             cRight = new Connector(new Point(Rectangle.Right, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
             cRight.Name = "Right connector";
             Connectors.Add(cRight);

             cBottom = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
             cBottom.Name = "Bottom connector";
             Connectors.Add(cBottom);

             cLeft = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
             cLeft.Name = "Left connector";
             Connectors.Add(cLeft);

             foreach (IConnector con in Connectors)
             {
                 con.Parent = this;

             }         
         }

		#endregion

		#region Methods
		/// <summary>
		/// Tests whether the mouse hits this bundle
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(System.Drawing.Point p)
		{
			Rectangle r= new Rectangle(p, new Size(5,5));
			return Rectangle.Contains(r);			
		}



		/// <summary>
		/// Paints the bundle on the canvas
		/// </summary>
		/// <param name="g"></param>
		public override void Paint(System.Drawing.Graphics g)
		{
            /*
            Matrix or = g.Transform;
            Matrix m = new Matrix();
            m.RotateAt(20, Rectangle.Location);            
            g.MultiplyTransform(m, MatrixOrder.Append);
             */
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			//the shadow
            if (ArtPallet.EnableShadows)
                g.FillRectangle(ArtPallet.ShadowBrush, Rectangle.X + 5, Rectangle.Y + 5, Rectangle.Width, Rectangle.Height);
			//the actual shape
			g.FillRectangle(Brush,Rectangle);
			//the edge of the bundle
			if(Hovered || IsSelected)
				g.DrawRectangle(ArtPallet.HighlightPen,Rectangle);
			else
                g.DrawRectangle(Pen, Rectangle);
			//the connectors
			for(int k=0;k<Connectors.Count;k++)
			{
				Connectors[k].Paint(g);
			}
			
			//here we keep it really simple:
			if(!string.IsNullOrEmpty(Text))
                g.DrawString(Text, ArtPallet.DefaultFont, Brushes.Black, TextRectangle);  
            //g.Transform = or;
		}

	

		

		#endregion	
	}
}
