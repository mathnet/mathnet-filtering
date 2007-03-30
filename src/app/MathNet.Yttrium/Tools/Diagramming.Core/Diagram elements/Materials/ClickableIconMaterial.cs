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
    /// Clickable icon shape material
    /// </summary>
    public partial class ClickableIconMaterial : IconMaterial, IMouseListener, IHoverListener
    {
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableIconMaterial"/> class.
        /// </summary>
        /// <param name="resourceLocation">The resource location.</param>
        public ClickableIconMaterial( string resourceLocation)   : base( resourceLocation)
        {
            Resizable = false;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClickableIconMaterial"/> class.
        /// </summary>
        public ClickableIconMaterial()
            : base()
        {
            Resizable = false;
        }
        #endregion

        #region Methods
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
            //if(this.Rectangle.Contains(e.Location))
            //    this.Shape.ShapeColor = Color.Yellow;            
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

        #endregion
    }
}
