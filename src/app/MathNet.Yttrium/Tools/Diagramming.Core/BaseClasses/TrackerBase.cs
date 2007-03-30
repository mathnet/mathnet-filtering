using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base implementation of the <see cref="ITracker"/> interface.
    /// </summary>
    public abstract class TrackerBase : ITracker
    {
        #region Fields
        /// <summary>
        /// the Rectangle field
        /// </summary>
        private Rectangle mRectangle;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return mRectangle;
            }
            set
            {
                mRectangle = value;
            }
        }
        /// <summary>
        /// the ShowHandles field
        /// </summary>
        private bool mShowHandles;
        /// <summary>
        /// Gets or sets the ShowHandles
        /// </summary>
        public bool ShowHandles
        {
            get { return mShowHandles; }
            set { mShowHandles = value; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrackerBase"/> class.
        /// </summary>
        public TrackerBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrackerBase"/> class.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public TrackerBase(Rectangle rectangle)
        {
            mRectangle = rectangle;
        }
        #endregion
    
        #region Methods
        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public abstract void Paint(Graphics g);
        //{
        //    g.DrawRectangle(ArtPallet.HighlightPen, mRectangle);
        //}
        /// <summary>
        /// Returns the relative coordinate of the grip-point hit, if any, of the tracker.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract Point Hit(Point p);
        /// <summary>
        /// Transforms the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public abstract void Transform(Rectangle rectangle);
        #endregion

    }
}
