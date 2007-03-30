using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The gradient paint style
    /// </summary>
    public sealed partial class GradientPaintStyle : IPaintStyle
    {
        #region Fields
        /// <summary>
        /// the Angle field.
        /// </summary>
        private float mAngle;
        /// <summary>
        /// the EndColor field
        /// </summary>
        private Color mEndColor;
        /// <summary>
        /// the StartColor field
        /// </summary>
        private Color mStartColor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the StartColor
        /// </summary>
        public Color StartColor
        {
            get { return mStartColor; }
            set { mStartColor = value; }
        }
        /// <summary>
        /// Gets or sets the EndColor
        /// </summary>
        public Color EndColor
        {
            get { return mEndColor; }
            set { mEndColor = value; }
        }
        /// <summary>
        /// Gets or sets the Angle
        /// </summary>
        public float Angle
        {
            get { return mAngle; }
            set { mAngle = value; }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public GradientPaintStyle(Color startColor, Color endColor, float angle)
        {
            mStartColor = startColor;
            mEndColor = endColor;
            mAngle = angle;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GradientPaintStyle"/> class.
        /// </summary>
        public GradientPaintStyle()
        {
            mStartColor = ArtPallet.RandomLowSaturationColor;
            mEndColor = Color.White;
            mAngle = -135;
        }
        #endregion


        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns></returns>
        public Brush GetBrush(Rectangle rectangle)
        {
            if (rectangle.Equals(Rectangle.Empty))
                return new SolidBrush(mStartColor);
            else
                return new LinearGradientBrush(rectangle, mStartColor, mEndColor, mAngle, true);
        }
    }
}
