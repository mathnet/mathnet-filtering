using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The simple shape interface.
    /// </summary>
    public interface ISimpleShape : IShape
    {
        /// <summary>
        /// Gets or sets the text of the simple shape
        /// </summary>
        string Text
        {
            get;
            set;
        }
    }
}
