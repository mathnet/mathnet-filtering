using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool implements the standard rectangular selection mechanism. There are two modes (much like Visio)
    /// <list type="bullet">
    /// <item><term>inclusive</term><description>elements are selected if they are contained in the selection rectangle</description></item>
    /// <item><term>touching</term><description>elements are selected if the selection rectangle has an overlap with the element</description></item>
    /// </list>
    /// <para>Note that this tool is slightly different than other tools since it activates itself unless it has been suspended by another tool. </para>
    /// </summary>
    class SelectionTool : AbstractTool, IMouseListener
    {

        #region Fields
        /// <summary>
        /// the location of the mouse when the motion starts
        /// </summary>
        private Point initialPoint;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SelectionTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public SelectionTool(string name) : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            Controller.View.CurrentCursor = CursorPallet.Selection;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
            {
                if(Selection.SelectedItems.Count == 0 && Selection.Connector==null)
                {
                    initialPoint = e.Location;
                    ActivateTool();
                    return true;//this tells the tool-loop to stop looking for another handler, which keeps the CPU low
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
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            IView view = this.Controller.View;
            Point point = e.Location;
            if(IsActive && !IsSuspended)
            {
                Controller.View.PaintGhostRectangle(initialPoint, point);
                Rectangle rec = System.Drawing.Rectangle.Inflate(Controller.View.Ghost.Rectangle, 20, 20); // the ghost rectangle is in World coordinates
                //rec = Rectangle.Round(view.WorldToView(rec));//reconvert the world rectangle to a viewspace rectangle to invalidate
                Controller.View.Invalidate(rec);
            }            
        }
        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {
                DeactivateTool();
                if(Controller.View.Ghost != null)
                {
                    Selection.CollectEntitiesInside(Controller.View.Ghost.Rectangle);//world space
                    Controller.RaiseOnShowSelectionProperties(new SelectionEventArgs(Selection.SelectedItems.ToArray()));
                }
                Controller.View.ResetGhost();                
            }
        }
        #endregion
    }

}
