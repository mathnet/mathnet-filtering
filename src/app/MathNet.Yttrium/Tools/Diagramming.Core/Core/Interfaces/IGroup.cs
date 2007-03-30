using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The group interface
    /// </summary>
    public interface IGroup : IDiagramEntity
    {
        /// <summary>
        /// Gets or sets the entities contained in this group.
        /// </summary>
        CollectionBase<IDiagramEntity> Entities { get; set;}

        /// <summary>
        /// Gets or sets whether the group as a shape should be painted on the canvas.
        /// </summary>
        bool EmphasizeGroup { get;set;}

        void CalculateRectangle();
    }
}
