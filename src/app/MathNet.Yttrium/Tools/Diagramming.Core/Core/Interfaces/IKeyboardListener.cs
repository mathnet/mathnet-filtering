using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    interface IKeyboardListener : IInteraction
    {

        /// <summary>
        /// on key down.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        void KeyDown(KeyEventArgs e);

        /// <summary>
        /// On keys up.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        void KeyUp(KeyEventArgs e);

        /// <summary>
        /// On key pressed.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyPressEventArgs"/> instance containing the event data.</param>
        void KeyPress(KeyPressEventArgs e);

    }
}
