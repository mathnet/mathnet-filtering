using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// A complex shape is just an IShape with <see cref="IShapeMaterial"/> children
    /// </summary>
    public interface IComplexShape : IShape
    {
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        CollectionBase<IShapeMaterial> Children
        {            
            get;
            set;
        }
    }
}
