using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for shape materials
    /// </summary>
    public abstract partial class ShapeMaterialBase : IShapeMaterial
    {
        #region Fields
        /// <summary>
        /// the Gliding field
        /// </summary>
        private bool mGliding = true;
        /// <summary>
        /// the Resizable field
        /// </summary>
        private bool mResizable = true;
        /// <summary>
        /// the Rectangle field
        /// </summary>
        private Rectangle mRectangle = Rectangle.Empty;
        /// <summary>
        /// the Shape field
        /// </summary>
        private IShape mShape;
          
        /// <summary>
        /// the Visible field
        /// </summary>
        private bool mVisible = true;
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the Gliding
        /// </summary>
        public bool Gliding
        {
            get { return mGliding; }
            set { mGliding = value; }
        }                                  
        /// <summary>
        /// Gets or sets the Resizable
        /// </summary>
        public bool Resizable
        {
            get { return mResizable; }
            set { mResizable = value; }
        }
        
        /// <summary>
        /// Gets or sets the Rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            get { return mRectangle; }
            internal set
            {
                mRectangle = value;
            }
        }
        /// <summary>
        /// Gets or sets the Shape
        /// </summary>
        public virtual IShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }
        /// <summary>
        /// Gets or sets the Visible
        /// </summary>
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        protected ShapeMaterialBase()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Transforms the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public virtual void Transform(Rectangle rectangle)
        {
            this.mRectangle = rectangle;
        }
        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public abstract void Paint(System.Drawing.Graphics g);


        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            return null;
        }

        #endregion

    }
}
