using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Extended <see cref="PenStyle"/> with extra properties for the design of connections.
    /// <remarks>Although the .Net 2.0 framework allows you to set line caps the result is, humbly said, rather poor. The
    /// caps are not drawn proportionally and custom caps cannot be filled resulting in a 'not implemented' exception.
    /// So, the only working solution (read: hack) I found is to draw the caps with a secondary pen whose size is bigger than the base pen with which the
    /// line is drawn. Unfortunately this hack cannot solely be implemented in this class, you need to fix it in the painting routines.
    /// Hopefully a solution will be around later on.
    /// </remarks>
    /// </summary>
    public partial class LinePenStyle : PenStyle
    {

        #region Fields
        /// <summary>
        /// the custom end cap
        /// </summary>
        private CustomLineCap mCustomEndCap;
        /// <summary>
        /// the custom start cap
        /// </summary>
        private CustomLineCap mCustomStartCap;
        /// <summary>
        /// the start cap
        /// </summary>
        private LineCap mStartCap = LineCap.NoAnchor;
        /// <summary>
        /// the end cap
        /// </summary>
        private LineCap mEndCap = LineCap.NoAnchor;
        /// <summary>
        /// the static generalization cap
        /// </summary>
        private static CustomLineCap mGeneralizationCap;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the 'generallization cap'. This <see cref="CustomLineCap"/> is an arrow used in UML diagram to emphasize a generalization
        /// of an object, i.e. an object is inherited and expanded or generalized.
        /// </summary>
        /// <value>The generallization cap.</value>
        public static CustomLineCap GenerallizationCap
        {
            get {
                return mGeneralizationCap;
            }
        }

        

        /// <summary>
        /// Gets or sets the custom end cap.
        /// </summary>
        /// <value>The custom end cap.</value>
        public CustomLineCap CustomEndCap
        {
            get
            {
                return mCustomEndCap;
            }
            set
            {
                mCustomEndCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom start cap.
        /// </summary>
        /// <value>The custom start cap.</value>
        public CustomLineCap CustomStartCap
        {
            get
            {
                return mCustomStartCap;
            }
            set
            {
                mCustomStartCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the start cap.
        /// </summary>
        /// <value>The start cap.</value>
        public LineCap StartCap
        {
            get
            {
                return mStartCap;
            }
            set
            {
                mStartCap = value;
            }
        }

        /// <summary>
        /// Gets or sets the end cap.
        /// </summary>
        /// <value>The end cap.</value>
        public LineCap EndCap
        {
            get
            {
                return mEndCap;
            }
            set
            {
                mEndCap = value;
            }
        }
        /// <summary>
        /// The constructed pen
        /// </summary>
        /// <returns></returns>
        public override Pen DrawingPen()
        {
            Pen pen = base.DrawingPen();
            /*
             * Not a satisfying result, see the hack in the class comments above.
            pen.CustomEndCap = mCustomEndCap;
            pen.CustomStartCap = mCustomStartCap;
            pen.StartCap = mStartCap;
            pen.EndCap = mEndCap;
             */
            return pen;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="T:LinePenStyle"/> class.
        /// </summary>
        static LinePenStyle()
        {
            Point[] ps = new Point[3] { new Point(-2, 0), new Point(0, 4), new Point(2, 0) };
            GraphicsPath gpath = new GraphicsPath();
            gpath.AddPolygon(ps);
            gpath.CloseAllFigures();
            mGeneralizationCap = new CustomLineCap(null, gpath);
        }

        public LinePenStyle()
        { }
        #endregion
    }
}
