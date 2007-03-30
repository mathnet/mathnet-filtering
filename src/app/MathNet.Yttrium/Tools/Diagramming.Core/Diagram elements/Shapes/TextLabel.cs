using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// A simple bundle to display textual information
	/// </summary>
	 public class TextLabel : SimpleShapeBase
	{
		#region Fields
        
         private Connector connector;
		#endregion

        #region Properties
         /// <summary>
         /// Gets the connector.
         /// </summary>
         /// <value>The connector.</value>
         public Connector Connector
         {
             get { return connector; }
         }
        
         /// <summary>
         /// Gets the friendly name of the entity to be displayed in the UI
         /// </summary>
         /// <value></value>
        public override string EntityName
        {
            get { return "Text Label"; }
        }
      
         #endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="s"></param>
		public TextLabel(IModel s) : base(s)
		{
            PaintStyle = ArtPallet.GetDefaultSolidPaintStyle();
            Resizable = false;
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextLabel"/> class.
        /// </summary>
         public TextLabel()
             : base()
         {
             PaintStyle = ArtPallet.GetDefaultSolidPaintStyle();
             Resizable = false;
             connector = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
             connector.Name = "Central connector";
             connector.Parent = this;
             Connectors.Add(connector);
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
		/// <param name="g">a Graphics object onto which to paint</param>
		public override void Paint(System.Drawing.Graphics g)
		{
            if(g == null)
                throw new ArgumentNullException("The Graphics object is 'null'.");

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //the shadow
            if (ArtPallet.EnableShadows)
                g.FillRectangle(ArtPallet.ShadowBrush, Rectangle.X + 5, Rectangle.Y + 5, Rectangle.Width, Rectangle.Height);
			//the container
			g.FillRectangle(Brush,Rectangle);
			//the edge
			if(Hovered || IsSelected)
				g.DrawRectangle(new Pen(Color.Red,2F),Rectangle);
			//the text		
			if(!string.IsNullOrEmpty(Text ))
                g.DrawString(Text, ArtPallet.DefaultFont, Brushes.Black, Rectangle);
		}

	
		

	


		#endregion	
        
	}
}
