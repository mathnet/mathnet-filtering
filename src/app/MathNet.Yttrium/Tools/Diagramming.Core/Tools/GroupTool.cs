using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Group tool
    /// </summary>
    class GroupTool : AbstractTool, IMouseListener
    {

        #region Fields
     
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GroupTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public GroupTool(string name)
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
            bool valid = true;
            //make sure we have the correct stuff on the table
            if (Selection.SelectedItems == null || Selection.SelectedItems.Count == 0)
            {
                MessageBox.Show("Nothing is selected, you need to select at least two items to create a group.", "Nothing selected.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                valid = false;
            }
            else if (Selection.SelectedItems.Count <= 1)
            {
                MessageBox.Show("You need at least two items to create a group.", "Multiple items.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                valid = false;
            }
            if (valid)
            {
                Bundle bundle = new Bundle(Selection.SelectedItems);

                GroupCommand cmd = new GroupCommand(this.Controller, bundle);

                this.Controller.UndoManager.AddUndoCommand(cmd);

                cmd.Redo();
            }
            DeactivateTool();
            return;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
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
