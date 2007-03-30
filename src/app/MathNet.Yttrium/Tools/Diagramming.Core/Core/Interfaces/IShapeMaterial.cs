using System;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The interface of shape material.
    /// </summary>
    public interface IShapeMaterial: IPaintable, IServiceProvider
    {

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IShapeMaterial"/> is resizable.
        /// </summary>
        /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
        bool Resizable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the material should remain a fixed position with respect to the shape or should
        /// glide proportionally wih the scaling.
        /// </summary>
        /// <value><c>true</c> if [no gliding]; otherwise, <c>false</c>.</value>
        bool Gliding { get; set;}
        /// <summary>
        /// Gets or sets the shape to which this material belongs.
        /// </summary>
        /// <value>The shape.</value>
        IShape Shape { get; set;}

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        new Rectangle Rectangle
        {         
            get;
        }

        /// <summary>
        /// Transforms the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        void Transform(Rectangle rectangle);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IShapeMaterial"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        bool Visible { get; set;}
    }
}
