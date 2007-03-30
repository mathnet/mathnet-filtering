using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The layout algorithms you can use to lay out a diagram
    /// </summary>
    public enum LayoutType
    { 
        /// <summary>
        /// A layout based on embedding physical forces like springs and gravitation in the diagram.
        /// </summary>
        ForceDirected,
        /// <summary>
        /// The Fruchterman-Rheingold layout algorithm.
        /// </summary>
        FruchtermanRheingold,
        /// <summary>
        /// Children are layed out in shells around the root node.
        /// </summary>
        RadialTree,
        /// <summary>
        /// Children are layed out in shells around the parent node.
        /// </summary>
        Balloon,
        /// <summary>
        /// The classic hierarchical layout of tree-like data.
        /// </summary>
        ClassicTree
    }
}
