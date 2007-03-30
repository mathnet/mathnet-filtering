using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Hover service
    /// </summary>
    public interface IHoverListener : IInteraction
    {          
        void MouseHover(MouseEventArgs e);
        void MouseEnter(MouseEventArgs e);
        void MouseLeave(MouseEventArgs e);

    }
}
