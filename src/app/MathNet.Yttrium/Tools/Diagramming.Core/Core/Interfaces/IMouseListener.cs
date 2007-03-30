using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Describes the three fundamental method of a mouse handler object.
    /// </summary>
    public interface IMouseListener : IInteraction
    {
        /// <summary>
        /// Handles the mouse-down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        bool MouseDown(MouseEventArgs e);
        /// <summary>
        /// Handles the mouse-move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void MouseMove(MouseEventArgs e);
        /// <summary>
        /// Handles the mouse-up event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void MouseUp(MouseEventArgs e);
    }
}
