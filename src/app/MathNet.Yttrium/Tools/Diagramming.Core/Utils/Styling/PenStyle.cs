using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Basic <see cref="IPenStyle"/> implementation; this is the minimal structure for a pen, add other <see cref="Pen"/> properties as needed.
    /// </summary>
    public partial class PenStyle : IPenStyle
    {
        #region Fields

        /// <summary>
        /// the pen's color
        /// </summary>
        private Color mColor = Color.Gray;
        /// <summary>
        /// the dashstyle of the pen
        /// </summary>
        private DashStyle mDashStyle = DashStyle.Solid;
        /// <summary>
        /// the pen's width
        /// </summary>
        private float mWidth = 1F;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the dash style.
        /// </summary>
        /// <value>The dash style.</value>
        public DashStyle DashStyle
        {
            get
            {
                return mDashStyle;
            }
            set
            {
                mDashStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                return mWidth;
            }
            set
            {
                mWidth = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PenStyle"/> class.
        /// </summary>
        public PenStyle()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PenStyle"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="dashStyle">The dash style.</param>
        /// <param name="width">The width.</param>
        public PenStyle(Color color, DashStyle dashStyle, float width)
        {
            this.mColor = color;
            this.mWidth = width;
            this.mDashStyle = dashStyle;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Returns the constructed pen.
        /// </summary>
        /// <returns></returns>
        public virtual Pen DrawingPen()
        {
            Pen pen = new Pen(mColor, mWidth);
            pen.DashStyle = mDashStyle;
            return pen;
        }
        #endregion
    }
}
