using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using MathNet.Symbolics.Presentation.Connectors;

namespace MathNet.Symbolics.Presentation.Connections
{
	/// <summary>
	/// Represents the connection between two connectors
	/// </summary>
	public sealed partial class YttriumConnection : ConnectionBase
    {
        private ConnectionStates _connectionState;
        private bool _reverse;

        #region Hack for the caps
        private const float capslength = 0.01F;
        private const float standardsshift = 7F;
        private const float arrowshift = 0.1F;
        /// <summary>
        /// the ration between the arrow width to the line width
        /// </summary>
        private const float capsratio = 5.5F;
        private const float generalizationration = 2.2F;
        private float capsshift;       
        private Pen leftPen;
        private Pen rightPen;
        private PointF unitvector;
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "Yttrium Connection"; }
        }
        /// <summary>
        /// The bounds of the paintable entity
        /// </summary>
        /// <value></value>
        public override Rectangle Rectangle
        {
            get
            {
                return System.Drawing.Rectangle.FromLTRB(Math.Min(From.Point.X, To.Point.X), Math.Min(From.Point.Y, To.Point.Y), Math.Max(From.Point.X, To.Point.X), Math.Max(From.Point.Y, To.Point.Y));
            }
        }
        public ConnectionStates State
        {
            get { return _connectionState; }
        }

        public bool HasPort
        {
            get { return (_connectionState & ConnectionStates.HasPort) == ConnectionStates.HasPort; }
        }
        public bool HasSignal
        {
            get { return (_connectionState & ConnectionStates.HasSignal) == ConnectionStates.HasSignal; }
        }
        public bool HasBus
        {
            get { return (_connectionState & ConnectionStates.HasBus) == ConnectionStates.HasBus; }
        }

        public bool IsFullyAssigned
        {
            get { return (_connectionState & ConnectionStates.BothAssigned) == ConnectionStates.BothAssigned; }
        }
        public bool IsHalfAssigned
        {
            get { return (_connectionState & ConnectionStates.SingleAssigned) == ConnectionStates.SingleAssigned; }
        }
        public bool IsUnassigned
        {
            get { return _connectionState == ConnectionStates.Unassigned; }
        }

        public YttriumConnector AttachedPortConnector
        {
            get { return (YttriumConnector)(_reverse ? To.AttachedTo : From.AttachedTo); }
        }
        public YttriumConnector AttachedValueConnector
        {
            get { return (YttriumConnector)(_reverse ? From.AttachedTo : To.AttachedTo); }
        }

        public bool IsAttachedToMultiplePorts
        {
            get { return _reverse ? To.AttachedConnectors.Count > 1 : From.AttachedConnectors.Count > 1; }
        }
        #endregion

		#region Constructor

        /// <summary>
        /// Constructs a connection between the two given points
        /// </summary>
        /// <param name="mFrom">the starting point of the connection</param>
        /// <param name="mTo">the end-point of the connection</param>
        /// <param name="model">The model.</param>
		public YttriumConnection(Point mFrom, Point mTo, IModel model):base(model)
		{
            _connectionState = ConnectionStates.Unassigned;
            _reverse = false;
			this.From = new Connector(mFrom, model);
			this.From.Name = "From";
			this.From.Parent = this;
			this.To = new Connector(mTo, model);
			this.To.Name = "To";
			this.To.Parent = this;
            PenStyle = ArtPallet.GetDefaultPenStyle();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Connection"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public YttriumConnection(Point from, Point to)
            : base(from, to)
        {
        }

        public YttriumConnection() : base(new Point(10,10), new Point(20,20)) { 
        
        }
		#endregion

		#region Methods

		/// <summary>
		/// Paints the connection on the canvas
		/// </summary>
		/// <param name="g"></param>
		public override void Paint(System.Drawing.Graphics g)
		{  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            base.Paint(g);
            
			if(Hovered || IsSelected)
                using (Pen highlightPen = Pen.Clone() as Pen)
                {
                    highlightPen.Color = Color.OrangeRed;
                    g.DrawLine(highlightPen, From.Point, To.Point);
                }
			else
				g.DrawLine(Pen,From.Point,To.Point);

            if(ArtPallet.EnableShadows)
                g.DrawLine(ArtPallet.ConnectionShadow, From.Point.X + 5, From.Point.Y + 5, To.Point.X + 5, To.Point.Y + 5);

            if (leftPen != null)
            {
                g.DrawLine(leftPen, From.Point.X + capsshift * unitvector.X, From.Point.Y + capsshift * unitvector.Y, From.Point.X + (capsshift + capslength) * unitvector.X, From.Point.Y + (capsshift + capslength) * unitvector.Y);
            }
            if (rightPen != null)
            {
                g.DrawLine(rightPen, To.Point.X - (capsshift + capslength) * unitvector.X, To.Point.Y - (capsshift + capslength) * unitvector.Y, To.Point.X - capsshift * unitvector.X, To.Point.Y - capsshift * unitvector.Y);
            }
		}
		/// <summary>
		/// Invalidates the connection
		/// </summary>
		public override void Invalidate()
		{

            float x = 0, y = 0;
            try
            {
                if (To == null || From == null) return;
                double length = Math.Sqrt((To.Point.X - From.Point.X) * (To.Point.X - From.Point.X) + (To.Point.Y - From.Point.Y) * (To.Point.Y - From.Point.Y));
                x = Convert.ToSingle(Convert.ToDouble(To.Point.X - From.Point.X) / length);
                y = Convert.ToSingle(Convert.ToDouble(To.Point.Y - From.Point.Y) / length);
            }
            catch (OverflowException exc)
            {
                throw new InconsistencyException("So, you tried to shrink the connection too much...", exc);
            }
            unitvector = new PointF(x, y);
            /* the old way
			Rectangle f = new Rectangle(From.Point,new Size(10,10));
			Rectangle t = new Rectangle(To.Point,new Size(10,10));
			this.Invalidate(Rectangle.Union(f,t));
             */
            base.Invalidate();

		}

		/// <summary>
		/// Tests if the mouse hits this connection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(Point p)
		{
			Point p1,p2, s;
			RectangleF r1, r2;
			float o,u;
			p1 = From.Point; p2 = To.Point;
	
			// p1 must be the leftmost point.
			if (p1.X > p2.X) { s = p2; p2 = p1; p1 = s; }

			r1 = new RectangleF(p1.X, p1.Y, 0, 0);
			r2 = new RectangleF(p2.X, p2.Y, 0, 0);
			r1.Inflate(3, 3);
			r2.Inflate(3, 3);
			//this is like a topological neighborhood
			//the connection is shifted left and right
			//and the point under consideration has to be in between.						
			if (RectangleF.Union(r1, r2).Contains(p))
			{
				if (p1.Y < p2.Y) //SWNE
				{
					o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
					u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
					return ((p.X > o) && (p.X < u));
				}
				else //NWSE
				{
					o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
					u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
					return ((p.X > o) && (p.X < u));
				}
			}
			return false;
		}

		/// <summary>
		/// Moves the connection with the given shift
		/// </summary>
		/// <param name="p"></param>
		public override void Move(Point p)
		{
            if (From.AttachedTo != null || To.AttachedTo != null) return;

            Rectangle rec = this.Rectangle;
            rec.Inflate(20, 20);
            this.From.Move(p);
            this.To.Move(p);           
            this.Invalidate();
            this.Invalidate(rec);
		}
        /// <summary>
        /// Updates pens and brushes.
        /// <remarks>The .Net API allows you to simply set caps but the visual results are less than satisfactory, to say the least. So, there is
        /// a hack here (unfortunately) which amounts to use two pen for one connection; one pen for the line and one for the (custom) cap. This is
        /// the easiest way to have visible arrow which otherwise is miniaturistic. There is also a custom shift of the caps since the location is sometime
        /// inappropriate; the arrow is drawn with the tip at the end of the connection while the diamond or circle caps are drawn with their center at the connection
        /// end. So, altogether a lot of tweaking and I really find it regrettable that the out-of-the-box caps are not what they should be (besides some obvious bugs like
        /// the 'not implemented' one if you try to fill a custom cap...).
        /// </remarks>
        /// </summary>
        protected override void UpdatePaintingMaterial()
        {
            base.UpdatePaintingMaterial();
            #region Hack
            //see the code comments of the LinePenStyle to understand the problem and this hack
            if (this.PenStyle is LinePenStyle)
            { 
                
                LinePenStyle lp = PenStyle as LinePenStyle;
                if (lp.StartCap == System.Drawing.Drawing2D.LineCap.NoAnchor)
                {                      
                    leftPen = null;
                }
                else
                {
                    

                    if (lp.StartCap == System.Drawing.Drawing2D.LineCap.Custom)
                    {
                        //leftPen.StartCap = System.Drawing.Drawing2D.LineCap.Custom;
                        //AdjustableArrowCap ccap = new AdjustableArrowCap(lp.Width+2, lp.Width+2, true);
                        //leftPen.CustomStartCap = ccap;
                        leftPen = new Pen(lp.Color, lp.Width * generalizationration);
                        leftPen.CustomStartCap = LinePenStyle.GenerallizationCap; //change to something like lp.CustomStartCap if you have more than one custom cap
                        capsshift = standardsshift;
                    }
                    else if (lp.StartCap == LineCap.ArrowAnchor)
                    {
                        leftPen = new Pen(lp.Color, lp.Width * capsratio);
                        leftPen.StartCap = lp.StartCap;
                        capsshift = arrowshift;
                    }
                    else
                    {
                        leftPen = new Pen(lp.Color, lp.Width * capsratio);
                        leftPen.StartCap = lp.StartCap;
                        capsshift = standardsshift;
                    }
                }

                if (lp.EndCap == System.Drawing.Drawing2D.LineCap.NoAnchor)
                {
                    rightPen = null;
                }
                else
                {
                    
                    if (lp.EndCap == System.Drawing.Drawing2D.LineCap.Custom)
                    {
                        //leftPen.StartCap = System.Drawing.Drawing2D.LineCap.Custom;
                        //AdjustableArrowCap ccap = new AdjustableArrowCap(lp.Width+2, lp.Width+2, true);
                        //leftPen.CustomStartCap = ccap;
                        //rightPen = new Pen(lp.Color, lp.Width * generalizationration);                        
                        //rightPen.CustomEndCap = lp.CustomEndCap;
                        //capsshift = standardsshift;
                        Pen.CustomEndCap = LinePenStyle.GenerallizationCap;
                    }

                    else if (lp.EndCap == LineCap.ArrowAnchor)
                    {
                        rightPen = new Pen(lp.Color, lp.Width * capsratio);
                        rightPen.EndCap = lp.EndCap;
                        capsshift = arrowshift;
                    }
                    else
                    {
                        rightPen = new Pen(lp.Color, lp.Width * capsratio);
                        rightPen.EndCap = lp.EndCap;
                        capsshift = standardsshift;
                    }
                }

            }
            #endregion
        }         

        
        
		#endregion

	}
}
