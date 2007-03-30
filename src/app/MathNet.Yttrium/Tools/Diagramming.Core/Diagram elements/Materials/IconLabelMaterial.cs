using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Icon shape material without <see cref="IMouseListener"/> service.
    /// <seealso cref="ClickableIconMaterial"/>
    /// </summary>
    public partial class IconLabelMaterial : ShapeMaterialBase
    {

        /// <summary>
        /// The distance between the icon and the text.
        /// </summary>
        public const int constTextShift = 2;

        #region Fields
        /// <summary>
        /// the Text field
        /// </summary>
        private Bitmap mIcon;
        /// <summary>
        /// the Text field
        /// </summary>
        private string mText = string.Empty;

        private Rectangle textRectangle = Rectangle.Empty;
        private StringFormat stringFormat = StringFormat.GenericTypographic;
        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
            }
        }

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
        public IconLabelMaterial(string text) : base()
        {
            mText = text;
            stringFormat.Trimming = StringTrimming.EllipsisWord;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="IconLabelMaterial"/> class.
        /// </summary>
        /// <param name="resourceLocation">The resource location.</param>
        /// <param name="text">The text.</param>
        public IconLabelMaterial(string text, string resourceLocation)
            : this(text)
        {
            mIcon = GetBitmap(resourceLocation);
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <param name="resourceLocation">The resource location.</param>
        /// <returns></returns>
        protected Bitmap GetBitmap(string resourceLocation)
        {
            if (resourceLocation.Length == 0)
                return null;
            try
            {
                 //first try if it's defined in this assembly somewhere                
                return new Bitmap(this.GetType(), resourceLocation);
            }
            catch
            {

                if (File.Exists(resourceLocation))
                {
                    return new Bitmap(resourceLocation);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:IconLabelMaterial"/> class.
        /// </summary>
        public IconLabelMaterial()
            : base()
        {
        }

        #endregion

        #region Methods
        public override void Transform(Rectangle rectangle)
        {
            textRectangle = new Rectangle(rectangle.X + (mIcon == null ? 0 : mIcon.Width) + constTextShift, rectangle.Y, rectangle.Width - (mIcon == null ? 0 : mIcon.Width) - constTextShift, rectangle.Height);
            base.Transform(rectangle);

        }
        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(Graphics g)
        {
            if(!Visible)
                return;
            GraphicsContainer cto = g.BeginContainer();
            g.SetClip(Shape.Rectangle);
            if(mIcon != null)
            {
                g.DrawImage(mIcon, new Rectangle(Rectangle.Location, mIcon.Size));
            }
            g.DrawString(mText, ArtPallet.DefaultFont, Brushes.Black, textRectangle, stringFormat);
            g.EndContainer(cto);
        }
        #endregion

    }
}
