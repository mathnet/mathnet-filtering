using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool allows entities to give feedback (e.g. via a change of cursor) when the mouse is hovering over them.
    /// </summary>
    class ContextTool : AbstractTool, IMouseListener
    {

        #region Fields

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public ContextTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            

        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Returns 'true' if the event was handled, otherwise 'false'.</returns>
        public bool MouseDown(MouseEventArgs e)
        {
            if(!IsSuspended && this.Enabled)
            {
                if(e.Button == MouseButtons.Right && e.Clicks == 1)
                {
                    //just the base menu for the moment
                    MenuItem[] additionalItems = null;
                    this.Controller.RaiseOnShowContextMenu(new EntityMenuEventArgs(null, e, ref additionalItems));
                    return true;
                }
            }
            return false;
            
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {

           
        }
      
        public void MouseUp(MouseEventArgs e)
        {
           
        }
        #endregion
    }

}
