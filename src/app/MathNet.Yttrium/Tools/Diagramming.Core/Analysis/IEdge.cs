using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Analysis
{
    /// <summary>
    /// Describes a connection from the point of view of graph analysis.
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// Indicates if this edge is directed or undirected.
        /// </summary>
        bool IsDirected { get; set;}
        /// <summary>
        /// Returns the first, or source, node upon which this Edge is incident.
        /// </summary>
        INode SourceNode { get;}
        /// <summary>
        /// Returns the second, or target, node upon which this Edge is incident.
        /// </summary>
        INode TargetNode { get;}
        /// <summary>
        /// Given a Node upon which this Edge is incident, the opposite incident
        /// Node is returned. Throws an exception if the input node is not incident
        /// on this Edge.
        /// </summary>
        INode AdjacentNode(INode node);
    }
}
