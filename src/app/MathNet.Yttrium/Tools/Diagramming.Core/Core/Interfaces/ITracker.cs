using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The tracker tracks the resize and move of an entity or a group of entities.
    /// </summary>
    public interface ITracker : IPaintable
    {

       
        /// <summary>
        /// Returns the relative coordinate of the grip-point hit, if any, of the tracker.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        Point Hit(Point p);

        /// <summary>
        /// Maps the tracker specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        void Transform(Rectangle rectangle);

        /// <summary>
        /// Gets or sets a value indicating whether to show the resizing handles.
        /// </summary>
        /// <value><c>true</c> if [show handles]; otherwise, <c>false</c>.</value>
        bool ShowHandles { get; set;}
    }
}
