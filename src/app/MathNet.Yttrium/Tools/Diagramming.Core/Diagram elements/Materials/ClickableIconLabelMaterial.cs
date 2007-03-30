using System;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{  
    /// <summary>
    /// Clickable icon-and-label shape material, i.e. an hyperlink with an icon. This material combines
    /// the <see cref="IconMaterial"/> and the <see cref="ClcikableLabelMaterial"/>
    /// </summary>
    public partial class ClickableIconLabelMaterial : IconLabelMaterial, IMouseListener, IHoverListener
    {

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
        /// Initializes a new instance of the <see cref="T:ClickableIconLabelMaterial"/> class.
        /// </summary>
        public ClickableIconLabelMaterial()   : base()
        {
            //Resizable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableIconLabelMaterial"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ClickableIconLabelMaterial(string text) : base(text) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableIconLabelMaterial"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="resourceLocation">The resource location.</param>
        public ClickableIconLabelMaterial(string text, string resourceLocation)
            : base(text, resourceLocation)
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableIconLabelMaterial"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="resourceLocation">The resource location.</param>
        /// <param name="url">The URL.</param>
        public ClickableIconLabelMaterial(string text, string resourceLocation, string url)
            : base(text, resourceLocation)
        {
            this.mUrl = url;
         }
        #endregion

        #region Methods
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
                {
                    try
                    {
                        Process.Start(mUrl);
                    }
                   
                    catch (ObjectDisposedException oex)
                    {
                        Trace.WriteLine(oex.Message);
                    }
                    catch (System.ComponentModel.Win32Exception wex)
                    {
                        Trace.WriteLine(wex.Message);
                    }
                    catch (ArgumentException aex)
                    {
                        Trace.WriteLine(aex.Message);
                    }
                    catch (InvalidOperationException iex)
                    {
                        Trace.WriteLine(iex.Message);
                    }
                }
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
