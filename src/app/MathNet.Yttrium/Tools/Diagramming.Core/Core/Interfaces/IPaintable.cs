using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The interface of something that can be painted on the canvas.
    /// </summary>
    public interface IPaintable
    {
        /// <summary>
        /// The bounds of the paintable entity
        /// </summary>
        Rectangle Rectangle
        {
            get;           
        }
        /// <summary>
        /// Paints the entity using the given graphics object
        /// </summary>
        /// <param name="g"></param>
        void Paint(Graphics g);
    }
}
