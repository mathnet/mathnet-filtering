using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Interface of a <see cref="Bundle"/>
    /// </summary>
    public interface IBundle : IDiagramEntity
    {
        /// <summary>
        /// Gets the entities in the <see cref="Bundle"/>
        /// </summary>
        /// <value>The entities.</value>
        CollectionBase<IDiagramEntity> Entities { get;}


      
    }
}
