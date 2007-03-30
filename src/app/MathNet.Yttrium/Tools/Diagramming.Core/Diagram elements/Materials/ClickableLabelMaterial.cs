using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{  
    /// <summary>
    /// Clickable label shape material, i.e. an hyperlink.
    /// </summary>
    public partial class ClickableLabelMaterial : LabelMaterial, IMouseListener, IHoverListener
    {

        /// <summary>
        /// Occurs when the lable is clicked
        /// </summary>
        public event EventHandler OnClick;

        #region Fields
        /// <summary>
        /// the url of this link
        /// </summary>
        private string mUrl = string.Empty;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the URL of this clickable material. Note that the material can handle more than just Url's by overriding the mouse event handlers.
        /// Settings this property is just a convenient way to accelerate the development or customization of shapes.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableLabelMaterial"/> class.
        /// </summary>
        public ClickableLabelMaterial()   : base()
        {
            //Resizable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableLabelMaterial"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ClickableLabelMaterial(string text) : base(text) { }
        
        #endregion

        #region Methods

        private void RaiseOnClicked()
        {
            if (OnClick != null)
                OnClick(this, EventArgs.Empty);
        }

        #region IMouseListener Members
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType.Equals(typeof(IMouseListener)))
                return this;
            else if (serviceType.Equals(typeof(IHoverListener)))
                return this;
            else
                return null;
        }

        /// <summary>
        /// Handles the mouse-down event
        /// </summary>
        /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
        public virtual bool MouseDown(MouseEventArgs e)
        {
            if (this.Rectangle.Contains(e.Location))
            {
                if (mUrl.Length > 0)
                    Process.Start(mUrl);
                RaiseOnClicked();
                return true;//let the rest of the loop go, the event was handled
            }
            return false;
        }

        /// <summary>
        /// Handles the mouse-move event
        /// </summary>
        /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.Location.ToString());
        }

        /// <summary>
        /// Handles the mouse-up event
        /// </summary>
        /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
        public virtual void MouseUp(MouseEventArgs e)
        {

        }

        #endregion

        #region IHoverListener Members
        /// <summary>
        /// Handles the <see cref="OnMouseHover"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHover(MouseEventArgs e)
        {
           
        }
        private Cursor previousCursor;

        /// <summary>
        /// Handles the OnMouseEnter event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseEnter(MouseEventArgs e)
        {
                previousCursor = Cursor.Current;                
                this.Shape.Model.RaiseOnCursorChange(Cursors.Hand);
        }

        /// <summary>
        /// Handles the OnMouseLeave event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseLeave(MouseEventArgs e)
        {
            this.Shape.Model.RaiseOnCursorChange(previousCursor);
        }

        //public override void Paint(Graphics g)
        //{
        //    g.DrawRectangle(Pens.Violet, Rectangle);
        //    base.Paint(g);
        //}
        #endregion            
        #endregion
    }
}
