using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// A mutli-point curve whose appearance depends on some predefined <see cref="MultiPointType"/> type.
    /// </summary>
    public partial class MultiPointShape : SimpleShapeBase
    {
        #region Fields
        private MultiPointType mCurveType = MultiPointType.Straight;
        private Point[] mPoints;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the points on which the curve is based.
        /// </summary>
        /// <value>The points.</value>
        public Point[] Points
        {
            get
            {
                return mPoints;
            }
            set
            {
                mPoints = value;
                CalculateRectangle();
            }
        }

        /// <summary>
        /// Gets or sets the type of the curve.
        /// </summary>
        /// <value>The type of the curve.</value>
        public MultiPointType CurveType
        {
            get
            {
                return mCurveType;
            }
            set
            {
                mCurveType = value;
            }
        }
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "MultiPoint Curve"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="points">The points.</param>
        /// <param name="curveType">Type of the curve.</param>
        public MultiPointShape(IModel s, Point[] points, MultiPointType curveType)
            : base(s)
        {
            mPoints = points;
            mCurveType = curveType;
            CalculateRectangle();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MultiPointShape"/> class.
        /// </summary>
        public MultiPointShape()
            : base()
        {

        }



        #endregion

        #region Methods
        /// <summary>
        /// Calculates the bounding rectangle of the points
        /// </summary>
        private void CalculateRectangle()
        {
            //presume the extreme opposite
            Point lt = new Point(int.MaxValue, int.MaxValue);
            Point rb = Point.Empty;
            Point p;
            for (int k = 0; k < mPoints.Length; k++)
            {
                p = mPoints[k];
                if (p.X < lt.X)
                    lt.X = p.X;
                if (p.Y < lt.Y)
                    lt.Y = p.Y;
                if (p.Y > rb.Y)
                    rb.Y = p.Y;
                if (p.X > rb.X)
                    rb.X = p.X;
            }
            //note that calling the other overload causes problems.
            //While usually the Rectangle dictates the shape,
            //here the Rectangle is deduced from the shape.
            //This is a bit tricky....
            base.Transform(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
        }

        /// <summary>
        /// Tests whether the mouse hits this bundle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            Rectangle r = new Rectangle(p, new Size(5, 5));
            return Rectangle.Contains(r);
        }



        /// <summary>
        /// Paints the bundle on the canvas
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(System.Drawing.Graphics g)
        {

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //the shadow

            switch (mCurveType)
            {
                case MultiPointType.Straight:
                    g.DrawLines(ArtPallet.BlackPen, mPoints);
                    break;
                case MultiPointType.Polygon:
                    g.DrawPolygon(ArtPallet.BlackPen, mPoints);
                    break;
                case MultiPointType.Curve:
                    //note that you can specify a tension of the curve here (greater than 0.0F)
                    g.DrawCurve(ArtPallet.BlackPen, mPoints);
                    break;
            }
        }

        /// <summary>
        /// Maps the shape to another rectangle, including all its sub-entities and materials.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void Transform(int x, int y, int width, int height)
        {
            //transform the curve points
            double a, b;
            Point p;
            for (int k = 0; k < mPoints.Length; k++)
            {
                a = Math.Round(((double)mPoints[k].X - (double)Rectangle.X) / (double)Rectangle.Width, 1) * width + x - mPoints[k].X;
                b = Math.Round(((double)mPoints[k].Y - (double)Rectangle.Y) / (double)Rectangle.Height, 1) * height + y - mPoints[k].Y;
                p = new Point(Convert.ToInt32(a), Convert.ToInt32(b));
                mPoints[k].Offset(p);
            }


            base.Transform(x, y, width, height);
        }

        /// <summary>
        /// Moves the entity with the given shift vector
        /// </summary>
        /// <param name="p">Represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {

            for (int k = 0; k < mPoints.Length; k++)
            {
                mPoints[k].Offset(p);
            }
            base.Move(p);
        }



        #endregion
    }

    public enum MultiPointType
    {
        /// <summary>
        /// Straight line interpolation.
        /// </summary>
        Straight,
        /// <summary>
        /// Plogonal interpolation.
        /// </summary>
        Polygon,
        /// <summary>
        /// Bezier-like interpolation.
        /// </summary>
        Curve
    }
}
