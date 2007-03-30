using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool implement the action of hitting an entity on the canvas. 
    /// </summary>
    class HitTool : AbstractTool, IMouseListener
    {

        #region Fields
       

     

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HitTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public HitTool(string name)
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
        public bool MouseDown(MouseEventArgs e)
        {
            if(e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if(e.Button == MouseButtons.Left  && Enabled && !IsSuspended)
            {
               
                //if(e.Clicks == 1)
                {
                    //TextEditor.Hide();
                    Selection.CollectEntitiesAt(e.Location);
                    if(Selection.SelectedItems.Count > 0)
                    {
                        IMouseListener listener = Selection.SelectedItems[0].GetService(typeof(IMouseListener)) as IMouseListener;
                        if(listener != null)
                        {

                            if (listener.MouseDown(e))
                                return true;
                        }
                    }
                }
                if(e.Clicks == 2)
                {
                    if(Selection.SelectedItems.Count > 0 && Selection.SelectedItems[0] is TextOnly)
                    {
                        ActivateTool();
                        //TextEditor.GetEditor(Selection.SelectedItems[0] as IShape);
                        //TextEditor.Show();
                        return true;
                    }
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
            if(IsActive)
            {
                DeactivateTool();
            }
        }
        #endregion
    }

}
