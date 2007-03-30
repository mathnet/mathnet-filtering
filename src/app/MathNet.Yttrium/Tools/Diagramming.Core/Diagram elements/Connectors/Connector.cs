using System;
using System.Drawing;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Represents an endpoint of a connection or a location of a bundle to which
	/// a connection can be attached
	/// </summary>
	public partial class Connector : ConnectorBase
    {
        #region Properties
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "Default Connector"; }
        }
        /// <summary>
        /// The bounds of the paintable entity
        /// </summary>
        /// <value></value>
        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Point.X - 2, Point.Y - 2, 4, 4);
            }
            //set {               Point = value.Location;                //TODO: think about what to do when setting the size            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Connector"/> class.
        /// </summary>
        /// <param name="site">The site.</param>
        public Connector(IModel site):base(site){}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Connector"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="site">The site.</param>
		public Connector(Point p, IModel site):base(p, site){}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Connector"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        public Connector(Point p) : base(p)
        {
        }
		#endregion

		#region Methods
        
		/// <summary>
		/// Paints the connector on the canvas
		/// </summary>
		/// <param name="g"></param>
		public override void Paint(Graphics g)
		{
            if (g == null)
                throw new ArgumentNullException("The Graphics object is 'null'");
            if (!Visible) return;
			if(Hovered || IsSelected)
				g.FillRectangle(Brushes.DarkGreen,Point.X-4,Point.Y-4,8,8);
            //else
            //    g.FillRectangle(Brushes.DimGray,Point.X-1,Point.Y-1,2,2);
				//g.FillRectangle(Brushes.WhiteSmoke,Point.X-2,Point.Y-2,4,4);

//			this is useful when debugging, but annoying otherwise (though you might like it)			
//			if(name!=string.Empty)
//				g.DrawString(name,new Font("verdana",10),Brushes.Black,Point.X+7,Point.Y+4);
		}

		/// <summary>
		/// Tests if the mouse hits this connector
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(Point p)
		{
			Point a = p;
			Point b  = Point;
			b.Offset(-7,-7);
			//a.Offset(-1,-1);
			Rectangle r = new Rectangle(a,new Size(0,0));
			Rectangle d = new Rectangle(b, new Size(15,15));
			return d.Contains(r);
		}

		/// <summary>
		/// Invalidates the connector
		/// </summary>
		public override void Invalidate()
		{
			Point p = Point;
			p.Offset(-5,-5);
            if(Model!=null)
			    Model.RaiseOnInvalidateRectangle(new Rectangle(p,new Size(10,10)));
		}

		/// <summary>
		/// Moves the connector with the given shift-vector
		/// </summary>
		/// <param name="p"></param>
		public override void Move(Point p)
		{
            Point pt = new Point(this.Point.X + p.X, this.Point.Y + p.Y);            
            IConnection con = null;
            Point p1 = Point.Empty, p2 = Point.Empty;
            Rectangle rec = new Rectangle(Point.X - 10, Point.Y - 10, 20, 20);
            this.Point = pt;
            
            #region Case of connection
            if (typeof(IConnection).IsInstanceOfType(this.Parent))
            {
                (Parent as IConnection).Invalidate();
            }
            #endregion

            #region Case of attached connectors
            for (int k = 0; k < AttachedConnectors.Count; k++)
            {
                if (typeof(IConnection).IsInstanceOfType(AttachedConnectors[k].Parent))
                {
                    //keep a reference to the two points so we can invalidate the region afterwards
                    con = AttachedConnectors[k].Parent as IConnection;
                    p1 = con.From.Point;
                    p2 = con.To.Point;
                }
                AttachedConnectors[k].Move(p);
                if (con != null)
                {
                    //invalidate the 'before the move'-region                     
                    Rectangle f = new Rectangle(p1, new Size(10, 10));
                    Rectangle t = new Rectangle(p2, new Size(10, 10));
                    Model.RaiseOnInvalidateRectangle(Rectangle.Union(f, t));
                    //finally, invalidate the region where the connection is now
                    (AttachedConnectors[k].Parent as IConnection).Invalidate();
                }
            } 
            #endregion
            //invalidate this connector, since it's been moved
            Invalidate(rec);//before the move
            this.Invalidate();//after the move
            
		}

        /// <summary>
        /// Moves the connector with the given shift-vector
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
		public  void Move(int x, int y)
		{
			Point pt = new Point( x, y);
            Move(pt);
		}
       
	

		#endregion
	}
}
