using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Simple label material
    /// </summary>
    public partial class LabelMaterial : ShapeMaterialBase
    {
        #region Fields
        /// <summary>
        /// the Text field
        /// </summary>
        private string mText;

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
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public LabelMaterial()
            : base()
        {
            stringFormat.Trimming = StringTrimming.EllipsisWord;
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
        }
        public LabelMaterial(string text)
            : this()
        {
            this.mText = text;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(System.Drawing.Graphics g)
        {
            if (!Visible) return;
            //the text		
            if (!string.IsNullOrEmpty(Text))
            {
                //g.DrawRectangle(Pens.Orange, Rectangle);

                g.DrawString(Text, ArtPallet.DefaultFont, Brushes.Black, Rectangle, stringFormat);
            }
        }
        #endregion

    }
}
