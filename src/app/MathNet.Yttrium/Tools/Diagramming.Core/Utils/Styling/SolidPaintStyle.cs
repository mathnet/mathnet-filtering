using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The solid paint style
    /// </summary>
    public sealed partial class SolidPaintStyle : IPaintStyle
    {
        #region Fields
        /// <summary>
        /// the SolidColor field
        /// </summary>
        private Color mSolidColor;
        /// <summary>
        /// the Alpha field
        /// </summary>
        private int mAlpha = 255;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the SolidColor
        /// </summary>
        public Color SolidColor
        {
            get { return mSolidColor; }
            set { mSolidColor = value; }
        }
        /// <summary>
        /// Gets or sets the Alpha
        /// </summary>
        public int Alpha
        {
            get { return mAlpha; }
            set { mAlpha = value; }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public SolidPaintStyle(Color color)
        {
            mSolidColor = color;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolidPaintStyle"/> class.
        /// </summary>
        public SolidPaintStyle()
        {
            mSolidColor = ArtPallet.RandomLowSaturationColor;
        }
        #endregion

        /// <summary>
        /// Gets the brush with which an entity can fe painted.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns></returns>
        public Brush GetBrush(Rectangle rectangle)
        {
            return new SolidBrush(Color.FromArgb(mAlpha, mSolidColor));
        }
    }
}
