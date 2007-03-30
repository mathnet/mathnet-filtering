using System;
using System.ComponentModel;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base classes for the rulers.
    /// </summary>
    public abstract class RulerBase : IPaintable
    {

        #region Fields
        private IView mView = null;

       
        private bool mVisible;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Ruler"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [BrowsableAttribute(true)]
        public bool Visible
        {
            get
            {
                return mVisible;
            }

            set
            {
                mVisible = value;
            }
        }
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        [BrowsableAttribute(false)]
        public abstract Rectangle Rectangle { get; }
        /// <summary>
        /// Gets or sets the view associated to this ruler.
        /// </summary>
        /// <value>The IView.</value>
        public IView View
        {
            get { return mView; }
            set { mView = value; }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public RulerBase(IView mView)
        {
            this.mView = mView;
        }
        #endregion

        #region Methods
        

        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public abstract void Paint(Graphics g);
        #endregion
        
    }

}
