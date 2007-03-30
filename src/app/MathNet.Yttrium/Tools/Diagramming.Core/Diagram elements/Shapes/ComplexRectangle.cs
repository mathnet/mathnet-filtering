using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// A complex rectangular shape
	/// </summary>
	 public partial class ComplexRectangle : ComplexShapeBase, IAdditionCallback
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
            get { return "Complex Rectangle"; }
        }
        #endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="s"></param>
		public ComplexRectangle(IModel s) : base(s)
		{
            Init();
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ComplexRectangle"/> class.
        /// </summary>
         public ComplexRectangle()
             : base()
         {
             Init();
         }

         /// <summary>
         /// Initializes this instance.
         /// </summary>
         private  void Init()
         {            

             cTop = new Connector(new Point((int) (Rectangle.Left + Rectangle.Width / 2), Rectangle.Top), Model);
             cTop.Name = "Top connector";
             cTop.Parent = this;
             Connectors.Add(cTop);

             cRight = new Connector(new Point(Rectangle.Right, (int) (Rectangle.Top + Rectangle.Height / 2)), Model);
             cRight.Name = "Right connector";
             cRight.Parent = this;
             Connectors.Add(cRight);

             cBottom = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
             cBottom.Name = "Bottom connector";
             cBottom.Parent = this;
             Connectors.Add(cBottom);

             cLeft = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
             cLeft.Name = "Left connector";
             cLeft.Parent = this;
             Connectors.Add(cLeft);

             #region Some examples of materials; feel free to add/remove what you wish
             /*
             ClickableIconMaterial cicon = new ClickableIconMaterial("Resources.Schema.ico");
             cicon.Resizable = false;
             cicon.Gliding = false;
             cicon.Transform(new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, 16, 16));
             Children.Add(cicon);
              */
             /*
             SwitchIconMaterial xicon = new SwitchIconMaterial(SwitchIconType.PlusMinus);
             xicon.Gliding = false;
             xicon.Resizable = false;
             xicon.Transform(new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, 16, 16));
             Children.Add(xicon);
             */
             /*
             IconLabelMaterial ilab = new IconLabelMaterial("Resources.PublicMethod.ico", "ISerializable.GetObjectData");
             ilab.Gliding = false;
             ilab.Resizable = false;
             ilab.Transform(new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, Rectangle.Width - 20, Rectangle.Height - 30));
             Children.Add(ilab);
             */
             /*
             IconMaterial icon = new IconMaterial("Resources.Web.png");
             icon.Resizable = false;
             icon.Gliding = false;
             if(icon.Icon!=null)
                icon.Transform(new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, icon.Icon.Width , icon.Icon.Height));
            Children.Add(icon);
             */
             /*
             LabelMaterial label = new LabelMaterial();
             label.Text = "Complex rectangle example";
             label.Transform( new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, Rectangle.Width - 20, Rectangle.Height-30));
             this.Children.Add(label);
              */
             
             string[] stuff = new string[]{"Wagner", "van Beethoven", "Sibelius", "Lutovski", "Haydn", "Prokofiev", "Karduso"};
             FolderMaterial folder = new FolderMaterial("Expand me!", stuff);
             folder.Transform(new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, Rectangle.Width - 20, Rectangle.Height - 30));
             folder.OnFolderChanged += new EventHandler<RectangleEventArgs>(folder_OnFolderChanged);
             this.Children.Add(folder);
            
             #endregion
             Resizable = true;

             Services[typeof(IAdditionCallback)] = this;
         }

         void folder_OnFolderChanged(object sender, RectangleEventArgs e)
         {
             Rectangle rec = e.Rectangle;
             Transform(new Rectangle(Rectangle.Location, new Size(Rectangle.Width, rec.Height + 20)));
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
            g.FillRectangle(ArtPallet.ShadowBrush, Rectangle.X + 5, Rectangle.Y + 5, Rectangle.Width, Rectangle.Height);
			//the actual bundle
			g.FillRectangle(Brush,Rectangle);
			//the edge of the bundle
			if(Hovered || IsSelected)
				g.DrawRectangle(ArtPallet.HighlightPen,Rectangle);
			else
                g.DrawRectangle(ArtPallet.BlackPen, Rectangle);
			//the connectors
			for(int k=0;k<Connectors.Count;k++)
			{
				Connectors[k].Paint(g);
			}
            foreach (IPaintable material in Children)
            {
                material.Paint(g);
            }
			
		
        
        }

        /// <summary>
        /// Called when the shape is added to the canvas.
        /// </summary>
         public void OnAddition()
         {
             Transform(new Rectangle(Rectangle.Location, new Size(150, FolderMaterial.constHeaderHeight + 20)));
             //use a light color rather than the random low saturation color to show off the connection lines
             PaintStyle = ArtPallet.GetDefaultSolidPaintStyle();

              (PaintStyle as SolidPaintStyle).SolidColor= Color.WhiteSmoke;

         }

	

	

       
		#endregion	

     
	
        
    }
}
