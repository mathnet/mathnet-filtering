using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using Netron.Diagramming.Core;
using System.Diagnostics;
using System.Windows.Forms;

namespace Netron.Diagramming.Win
{
    /// <summary>
    /// WinForm implementation of the IView interface.
    /// <seealso cref="Netron.Diagramming.Core.Web.WebView"/>
    /// </summary>
    class View : ViewBase
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:View"/> class.
        /// </summary>
        public View(IDiagramControl control)
            : base(control)
        {
            
            Selection.OnNewSelection += new EventHandler(Selection_OnNewSelection);
           // this.Model.OnEntityAdded += new EventHandler<EntityEventArgs>(Model_OnEntityAdded);
            this.HorizontalRuler.Visible = true;
            this.VerticalRuler.Visible = true;
            GhostsFactory.View = this;
        }

        void Model_OnEntityAdded(object sender, EntityEventArgs e)
        {
            
        }

        void Selection_OnNewSelection(object sender, EventArgs e)
        {
            ShowTracker();
        }

        
        #endregion

        #region Methods
        public override void Paint(Graphics g)
        {
   
            base.Paint(g);

            //Rectangle rectangle = WorkArea;
            //g.SetClip(WorkArea);
            g.Transform = ViewMatrix;
            //draw the ghost and ants on top of the diagram
            if(Ants != null)
                Ants.Paint(g);
            if(Ghost != null)
                Ghost.Paint(g);
            if(Tracker != null)
                Tracker.Paint(g);

            g.Transform.Reset();
            //g.PageUnit = GraphicsUnit.Pixel;
            //g.PageScale = 1.0F;
        }
        
        public override void PaintGhostEllipse(Point ltPoint, Point rbPoint)
        {
            Ghost = GhostsFactory.GetGhost(new Point[] { ltPoint, rbPoint }, GhostTypes.Ellipse);
        }
        public override void PaintGhostRectangle(Point ltPoint, Point rbPoint)
        {
            Ghost = GhostsFactory.GetGhost(new Point[] { ltPoint, rbPoint }, GhostTypes.Rectangle);
        }
        public override void PaintAntsRectangle(Point ltPoint, Point rbPoint)
        {
            Ants = AntsFactory.GetAnts(new Point[] { ltPoint, rbPoint }, AntTypes.Rectangle);
        }
        public override void PaintGhostLine(Point ltPoint, Point rbPoint)
        {
            Ghost = GhostsFactory.GetGhost(new Point[] { ltPoint, rbPoint }, GhostTypes.Line);
        }
        public override void PaintGhostLine(MultiPointType curveType, Point[] points)
        {
            switch(curveType)
            {
                case MultiPointType.Straight:
                    Ghost = GhostsFactory.GetGhost(points, GhostTypes.MultiLine);
                    break;
                case MultiPointType.Polygon:
                    Ghost = GhostsFactory.GetGhost(points, GhostTypes.Polygon);
                    break;
                case MultiPointType.Curve:
                    Ghost = GhostsFactory.GetGhost(points, GhostTypes.CurvedLine);
                    break;
                
            }
            
        }

        public override void PaintTracker(Rectangle rectangle, bool showHandles)
        {
            Tracker = TrackerFactory.GetTracker(rectangle, TrackerTypes.Default, showHandles);
            rectangle.Inflate(20, 20);
            this.Invalidate(rectangle);
        }

        #endregion

        #region Tracker

        private enum TrackerTypes
        {
            Default
        }

        private class TrackerFactory
        {

            private static ITracker defTracker;

            public static ITracker GetTracker(Rectangle rectangle, TrackerTypes type, bool showHandles)
            {
                switch(type)
                {
                    case TrackerTypes.Default:
                        if(defTracker == null) defTracker = new DefaultTracker();
                        defTracker.Transform(rectangle);
                        defTracker.ShowHandles = showHandles;
                        return defTracker;
                    default:
                        return null;
                }

            }
        }

        private class DefaultTracker  : TrackerBase
        {

            private const int gripSize = 4;
            private const int hitSize = 6;
            float mx, my, sx, sy;
          

            #region Constructor
            ///<summary>
            ///Default constructor
            ///</summary>
            public DefaultTracker(Rectangle rectangle) : base(rectangle)
            {
                
            }
            public DefaultTracker()
                : base()
            { }
            #endregion


            public override void Transform(Rectangle rectangle)
            {
                this.Rectangle = rectangle;
            }


            public override  void Paint(Graphics g)
            {
                //the main rectangle
                g.DrawRectangle(ArtPallet.TrackerPen, Rectangle);
                #region Recalculate the size and location of the grips
                mx = Rectangle.X + Rectangle.Width / 2;
                my = Rectangle.Y + Rectangle.Height / 2;
                sx = Rectangle.Width /2;
                sy = Rectangle.Height /2;
                #endregion
                #region draw the grips
                if (!ShowHandles) return;
                
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1 ; y++)
                    {
                        if (x != 0 || y != 0) //not the middle one
                        {
                            g.FillRectangle(ArtPallet.GripBrush, mx + x * sx - gripSize / 2, my + y * sy - gripSize / 2, gripSize, gripSize);
                            g.DrawRectangle(ArtPallet.BlackPen, mx + x * sx - gripSize / 2, my + y * sy - gripSize / 2, gripSize, gripSize);
                        }
                    }
                }
                #endregion
            }

            public override Point Hit(Point p)
            {
                //no need to test if the handles are not shown
                if (!ShowHandles) return Point.Empty;

                for (int x = -1; x <= +1; x++)
                    for (int y = -1; y <= +1; y++)
                        if ((x != 0) || (y != 0))
                        {
                            if(new RectangleF(mx + x * sx - hitSize/2, my + y * sy - hitSize/2, hitSize, hitSize).Contains(p))
                                return new Point(x, y);
                        }
                return Point.Empty;
            }

        }

        #endregion

        #region Factories of visual effects

        private static class AntsFactory
        {
            #region Static fields
            /// <summary>
            /// pointer to the rectangular ants concrete
            /// </summary>
            private static RectAnts mRectangular;
            /// <summary>
            /// Gets the ants pen
            /// </summary>
            public readonly static Pen Pen = new Pen(Color.Black, 1f);
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes the <see cref="T:AntsFactory"/> class.
            /// </summary>
            static AntsFactory()
            {
                Pen.DashStyle = DashStyle.Dash;
            }
            #endregion

            #region Methods

            /// <summary>
            /// Returns the ant of the specified type
            /// </summary>
            /// <param name="pars"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static IAnts GetAnts(object pars, AntTypes type)
            {
                switch(type)
                {
                    case AntTypes.Rectangle:
                        if(mRectangular == null)
                            mRectangular = new RectAnts();
                        Point[] points = (Point[]) pars;
                        mRectangular.Start = points[0];
                        mRectangular.End = points[1];
                        return mRectangular;
                    default:
                        return null;
                }
            }
            #endregion

            #region Concretes
            /// <summary>
            /// Rectangular ants
            /// </summary>
            private class RectAnts : AbstractAnt
            {

               

                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                public RectAnts(Point s, Point e)
                    : this()
                {
                    this.Start = s;
                    this.End = e;

                }
                public RectAnts() : base()
                {
                    Pen.DashStyle = DashStyle.Dash;
                }
                #endregion

                #region IPaintable Members

                /// <summary>
                /// Paints the ants on the Graphics object
                /// </summary>
                /// <param name="g"></param>
                public override void Paint(Graphics g)
                {
                    if(g == null)
                        return;
                    g.DrawRectangle(AntsFactory.Pen, Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y);

                }

                #endregion
            }
            #endregion

            private abstract class AbstractAnt : IAnts
            {
                #region Fields
                /// <summary>
                /// the Start field
                /// </summary>
                private Point mStart;


                /// <summary>
                /// the End field
                /// </summary>
                private Point mEnd;

                #endregion

                #region Properties

                /// <summary>
                /// Gets or sets the Start
                /// </summary>
                public Point Start
                {
                    get
                    {
                        return mStart;
                    }
                    set
                    {
                        mStart = value;
                    }
                }
                /// <summary>
                /// Gets or sets the End
                /// </summary>
                public Point End
                {
                    get
                    {
                        return mEnd;
                    }
                    set
                    {
                        mEnd = value;
                    }
                }
               
                /// <summary>
                /// Gets or sets the Rectangle
                /// </summary>
                public Rectangle Rectangle
                {
                    get { return new Rectangle(mStart.X, mStart.Y, mEnd.X - mStart.X, mEnd.Y - mStart.Y); }
                    set {
                        mStart = new Point(value.X, value.Y);
                        mEnd = new Point(value.Right, value.Bottom);
                    }
                }
                #endregion

                #region Methods
                public abstract void Paint(Graphics g);
                #endregion

            }
        }

        private enum AntTypes
        {
            Rectangle
        }

        private static class GhostsFactory
        {
            #region Static fields
          
            /// <summary>
            /// pointer to the rectangular ghost
            /// </summary>
            private static RectGhost mRectangular;
            /// <summary>
            /// pointer to the single line ghost
            /// </summary>
            private static LineGhost mLine;
            /// <summary>
            /// pointer to the ellipse ghost
            /// </summary>
            private static EllipticGhost mEllipse;
            /// <summary>
            /// pointer to the multi-line ghost
            /// </summary>
            private static MultiLineGhost mMultiLine;
            /// <summary>
            /// pointer to the curved line ghost
            /// </summary>
            private static CurvedLineGhost mCurvedLine;
            /// <summary>
            /// pointer to the polygon ghost
            /// </summary>
            private static PolygonGhost mPolygon;

            private static IView mView;

          
            #endregion

            #region Constructor

            #endregion

            #region Properties
               public static IView View
            {
                get { return mView; }
                set { mView = value; }
            }
            #endregion

            #region Methods (only static allowed in a static class)
            /// <summary>
            /// Return a ghost of the specific type
            /// </summary>
            /// <param name="pars"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
            public static IGhost GetGhost(object pars, GhostTypes type)
            {
                Point[] points;
                switch(type)
                {
                    case GhostTypes.Rectangle:
                        if(mRectangular == null)
                            mRectangular = new RectGhost(View);
                        points = (Point[]) pars;
                        mRectangular.Start = points[0];
                        mRectangular.End = points[1];
                        return mRectangular;
                    case GhostTypes.Ellipse:
                        if(mEllipse == null)
                            mEllipse = new EllipticGhost(View);
                        points = (Point[]) pars;
                        mEllipse.Start = points[0];
                        mEllipse.End = points[1];
                        return mEllipse;
                    case GhostTypes.Line:
                        if(mLine == null)
                            mLine = new LineGhost(View);
                        points = (Point[]) pars;
                        mLine.Start = points[0];
                        mLine.End = points[1];
                        return mLine;
                    case GhostTypes.MultiLine:
                        if(mMultiLine == null)
                            mMultiLine = new MultiLineGhost(View);
                        points = (Point[]) pars;
                        mMultiLine.Points = points;
                        return mMultiLine;
                    case GhostTypes.CurvedLine:
                        if(mCurvedLine == null)
                            mCurvedLine = new CurvedLineGhost(View);
                        points = (Point[]) pars;
                        mCurvedLine.Points = points;
                        return mCurvedLine;
                    case GhostTypes.Polygon:
                        if(mPolygon == null)
                            mPolygon = new PolygonGhost(View);
                        points = (Point[]) pars;
                        mPolygon.Points = points;
                        return mPolygon;
                    default:
                        return null;
                }
            }
            #endregion

            #region Concretes
            /// <summary>
            /// The polygon line ghost
            /// </summary>
            private class PolygonGhost : MultiLineGhost
            {
                public PolygonGhost(IView view) : base(view) { }
                public override void Paint(Graphics g)
                {
                    g.Transform = View.ViewMatrix;
                    g.DrawPolygon(ArtPallet.GhostPen, Points);
                }
            }


            /// <summary>
            /// The curved line ghost
            /// </summary>
            private class CurvedLineGhost : MultiLineGhost
            {
                public CurvedLineGhost(IView view)
                    : base(view)
                { }
                public override void Paint(Graphics g)
                {
                    g.Transform = View.ViewMatrix;
                    g.DrawCurve(ArtPallet.GhostPen, Points);
                }
            }

            /// <summary>
            /// Multiline ghost
            /// </summary>
            private class MultiLineGhost : AbstractGhost
            {
                private Point[] points;
                
                /// <summary>
                /// Gets or sets the points of the multiline.
                /// </summary>
                /// <value>The points.</value>
                public Point[] Points
                {
                    get
                    {
                        return points;
                    }
                    set
                    {
                        points = value;
                        Start = value[0];
                        End = value[value.Length-1];
                    }
                }

                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                public MultiLineGhost(IView view, Point[] points) : base(view)                   
                {
                    this.points = points;
                }
                public MultiLineGhost(IView view):base(view)
                {
                    
                }
                #endregion

                #region IPaintable Members
                /// <summary>
                /// Paints the ghost on the given Graphics object
                /// </summary>
                /// <param name="g"></param>
                public override void Paint(Graphics g)
                {
                    if(g == null)
                        return;
                    g.Transform = View.ViewMatrix;
                    g.DrawLines(ArtPallet.GhostPen, points);                    
                }

                #endregion
            }
            /// <summary>
            /// Rectangular ghost
            /// </summary>
            private class RectGhost : AbstractGhost
            {



                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                public RectGhost(IView view, Point s, Point e)
                    : base(view, s, e)
                {

                }
                public RectGhost(IView view):base(view)
                {
                    
                }
                #endregion

                #region IPaintable Members
                /// <summary>
                /// Paints the ghost on the given Graphics object
                /// </summary>
                /// <param name="g"></param>
                public override void Paint(Graphics g)
                {
                    if(g == null)
                        return;                    
                    g.FillRectangle(ArtPallet.GhostBrush, Rectangle);
                    g.DrawRectangle(ArtPallet.GhostPen, Rectangle);

                }

                #endregion
            }

            /// <summary>
            /// Rectangular ghost
            /// </summary>
            private class EllipticGhost : AbstractGhost
            {
                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                public EllipticGhost(IView view, Point s, Point e)
                    : base(view, s, e)
                {


                }
                public EllipticGhost(IView view):base(view)
                {
                }
                #endregion

                #region IPaintable Members
                /// <summary>
                /// Paints the ghost on the given Graphics object
                /// </summary>
                /// <param name="g"></param>
                public override void Paint(Graphics g)
                {
                    if(g == null)
                        return;
                    g.Transform = View.ViewMatrix;
                    g.FillEllipse(ArtPallet.GhostBrush, Rectangle);
                    g.DrawEllipse(ArtPallet.GhostPen, Rectangle);

                }

                #endregion
            }

            /// <summary>
            /// Just a line really
            /// </summary>
            private class LineGhost : AbstractGhost
            {
                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                public LineGhost(IView view, Point s, Point e)
                    : base(view, s, e)
                {


                }
                public LineGhost(IView view):base(view)
                {
                }
                #endregion

                #region IPaintable Members
                /// <summary>
                /// Paints the ghost on the given Graphics object
                /// </summary>
                /// <param name="g"></param>
                public override void Paint(Graphics g)
                {
                    if(g == null)
                        return;
                    g.Transform = View.ViewMatrix;
                    g.DrawLine(ArtPallet.GhostPen, Start, End);
                }

                #endregion
            }

            private abstract class AbstractGhost : IGhost
            {
                #region Fields
                /// <summary>
                /// the Start field
                /// </summary>
                private Point mStart;


                /// <summary>
                /// the End field
                /// </summary>
                private Point mEnd;

                private IView mView;

               

                #endregion

                #region Properties
                /// <summary>
                /// Gets or sets the view.
                /// </summary>
                /// <value>The view.</value>
                public IView View
                {
                    get { return mView; }
                    set { mView = value; }
                }
                /// <summary>
                /// The bounds of the paintable entity
                /// </summary>
                /// <value></value>
                public Rectangle Rectangle
                {
                    get
                    {
                        return Rectangle.FromLTRB(Math.Min(mStart.X, mEnd.X), Math.Min(mStart.Y, mEnd.Y), Math.Max(mStart.X, mEnd.X), Math.Max(mStart.Y, mEnd.Y));
                    }
                    set { }
                }

                /// <summary>
                /// Gets or sets the Start
                /// </summary>
                public Point Start
                {
                    get
                    {
                        return mStart;
                    }
                    set
                    {
                        mStart = value;
                    }
                }
                /// <summary>
                /// Gets or sets the End
                /// </summary>
                public Point End
                {
                    get
                    {
                        return mEnd;
                    }
                    set
                    {
                        mEnd = value;
                    }
                }
                #endregion

                #region Constructor
                ///<summary>
                ///Default constructor
                ///</summary>
                protected AbstractGhost(IView view, Point s, Point e) : this(view)
                {
                    this.mStart = s;
                    this.mEnd = e;
                }
                protected AbstractGhost(IView view)
                {
                    mView = view;
                }
                #endregion

                #region Methods

                public abstract void Paint(Graphics g);
                #endregion

            }
            #endregion
        }
        private enum GhostTypes
        {
            Rectangle,
            Ellipse,
            Line, 
            MultiLine,
            CurvedLine,
            Polygon
        }
        #endregion
    }

}
