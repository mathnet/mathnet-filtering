using System;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Icon shape material without <see cref="IMouseListener"/> service.
    /// <seealso cref="ClickableIconMaterial"/>
    /// </summary>
    public partial class IconMaterial : ShapeMaterialBase
    {
        #region Fields
        /// <summary>
        /// the Text field
        /// </summary>
        private Bitmap mIcon;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public Bitmap Icon
        {
            get
            {
                return mIcon;
            }
            set
            {
                mIcon = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="IconMaterial"/> class.
        /// </summary>
        /// <param name="resourceLocation">The resource location.</param>
        public IconMaterial(string resourceLocation)
            : base()
        {

            mIcon = GetBitmap(resourceLocation);
        }

        protected Bitmap GetBitmap(string resourceLocation)
        {
            if(resourceLocation.Length == 0)
                throw new InconsistencyException("Invalid icon specification.");
            try
            {
                return new Bitmap(this.GetType(), resourceLocation);
            }
            catch
            {

                throw;
            }
        }
        public IconMaterial()
            : base()
        {
        }

        #endregion

        #region Methods
        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(Graphics g)
        {
            if(!Visible)
                return;

            if(mIcon != null)
            {
                GraphicsContainer cto = g.BeginContainer();
                g.SetClip(Shape.Rectangle);
                g.DrawImage(mIcon, Rectangle);
                g.EndContainer(cto);
            }
        }
        #endregion

    }
}
