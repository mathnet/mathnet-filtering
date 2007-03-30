using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Defines a custom pen style to draw entities and sub-entities.
    /// <remarks>The style can be extended with many properties and .Net 2.0 has added several new ones in comparison to .Net 1.1. Feel free to extend
    /// as you need.</remarks>
    /// </summary>
    public interface IDrawStyle
    {
        /// <summary>
        /// Gets or sets the custom end cap.
        /// </summary>
        /// <value>The custom end cap.</value>
        CustomLineCap CustomEndCap { get; set; }
        /// <summary>
        /// Gets or sets the custom start cap.
        /// </summary>
        /// <value>The custom start cap.</value>
        CustomLineCap CustomStartCap { get; set; }
        /// <summary>
        /// Gets or sets the start cap.
        /// </summary>
        /// <value>The start cap.</value>
        LineCap StartCap { get; set; }
        /// <summary>
        /// Gets or sets the end cap.
        /// </summary>
        /// <value>The end cap.</value>
        LineCap EndCap { get; set; }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        Color Color { get; set; }
        /// <summary>
        /// Gets or sets the dash style.
        /// </summary>
        /// <value>The dash style.</value>
        DashStyle DashStyle { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        float Width { get; set; }
        /// <summary>
        /// Gets the drawing pen of this style.
        /// </summary>
        /// <returns></returns>
        Pen DrawingPen();
    }

    /// <summary>
    /// Describes a pen style.
    /// </summary>
    public interface IPenStyle
    {
        Pen DrawingPen();
    }
}
