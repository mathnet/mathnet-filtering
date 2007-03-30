using System;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Like a <see cref="TextLabel"/> but without shadows and colors.
    /// </summary>
    partial class TextOnly : SimpleShapeBase
    {
        #region Fields

        #endregion

        #region Properties
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get { return "TextOnly Shape"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s"></param>
        public TextOnly(IModel s)
            : base(s)
        {
            this.PaintStyle = ArtPallet.GetTransparentPaintStyle();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextOnly"/> class.
        /// </summary>
        public TextOnly()
            : base()
        { }
        #endregion

        #region Methods
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
        /// <param name="g">a Graphics object onto which to paint</param>
        public override void Paint(System.Drawing.Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("The Graphics object is 'null'.");

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //the edge
            if (Hovered || IsSelected)
                g.DrawRectangle(new Pen(Color.Red, 2F), Rectangle);
            //the text		
            if (!string.IsNullOrEmpty(Text))
                g.DrawString(Text, ArtPallet.DefaultFont, Brushes.Black, Rectangle);
        }







        #endregion

    }
}
