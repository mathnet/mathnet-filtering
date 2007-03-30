using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for simple shapes. 
    /// <para>A simple shape does not contain sub-elements (aka shape material, see <see cref="IShapeMaterial"/>) but this does
    /// not mean you cannot paint sub-elements. In fact, this shape type corresponds to the old base class in the previous Netron graph library (i.e. before version 2.3).
    /// 
    /// <seealso cref="ComplexShapeBase"/>
    /// </para>
    /// </summary>
    public abstract partial class SimpleShapeBase : ShapeBase, ISimpleShape
    {

        private const int TextRectangleInflation = 10;

        #region Fields


        private bool mAutoSize = true;
        /// <summary>
        /// the text on the bundle
        /// </summary>
        private string mText = string.Empty;
        /// <summary>
        /// the mRectangle inside which the text is drawn
        /// </summary>
        private Rectangle mTextRectangle;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to autosize the label in function of the string content.
        /// </summary>
        /// <value><c>true</c> if [auto size]; otherwise, <c>false</c>.</value>
        public bool AutoSize
        {
            get { return mAutoSize; }
            set { 
                mAutoSize = value;
                AutoReSize(mText);
            }
        }
        /// <summary>
        /// Gets or sets the text of the bundle
        /// </summary>
        [Browsable(true), Description("The text shown on the shape"), Category("Layout")]
        public virtual string Text
        {
            get
            {
                return mText;
            }
            set
            {
                if(value == null)
                    throw new InconsistencyException("The text property cannot be 'null'");
                mText = value;
                if (mAutoSize)
                {
                    AutoReSize(value);
                }
                this.Invalidate();
            }
        }

        private void AutoReSize(string value)
        {
            SizeF size = TextRenderer.MeasureText(value, ArtPallet.DefaultFont);
            Rectangle rec = new Rectangle(Rectangle.Location, Size.Round(size));
            rec.Inflate(TextRectangleInflation, TextRectangleInflation);
            rec.Offset(TextRectangleInflation, TextRectangleInflation);
            Transform(rec);
        }
        /// <summary>
        /// Gets or sets the text rectangle.
        /// </summary>
        /// <value>The text rectangle.</value>
        public Rectangle TextRectangle
        {
            get
            {
                return mTextRectangle;
            }
            set
            {
                mTextRectangle = value;
            }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public SimpleShapeBase() : base()
        {
            mTextRectangle = Rectangle;

            mTextRectangle.Inflate(-TextRectangleInflation, -TextRectangleInflation);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleShapeBase"/> class.
        /// </summary>
        /// <param name="model">the <see cref="IModel"/></param>
        public SimpleShapeBase(IModel model)
            : base(model)
        {
            mTextRectangle = Rectangle;

            mTextRectangle.Inflate(-TextRectangleInflation, -TextRectangleInflation);
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Moves the entity with the given shift vector
        /// </summary>
        /// <param name="p">Represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {
            base.Move(p);
            this.mTextRectangle.X += p.X;
            this.mTextRectangle.Y += p.Y;
        }

        public override void Transform(int x, int y, int width, int height)
        {
            base.Transform(x, y, width, height);
            mTextRectangle = Rectangle;

            mTextRectangle.Inflate(-TextRectangleInflation, -TextRectangleInflation);
        }
        #endregion
    }
}
