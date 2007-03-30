using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Analysis
{
    /// <summary>
    /// This interface describes the additional members useful if the underlying graph is a tree.
    /// </summary>
    public interface ITree : IGraph
    {

        /// <summary>
        /// Gets or sets a value indicating whether this tree is directed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is directed; otherwise, <c>false</c>.
        /// </value>
        bool IsDirected { get; set;}
        /// <summary>
        /// Gets or sets the root of the tree
        /// </summary>
        INode Root { get;set;}
        /// <summary>
        /// Gets the children of the given node in the tree hierarchy.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        CollectionBase<INode> Children(INode node);
        /// <summary>
        /// Gets the edges connecting the children of the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        CollectionBase<IEdge> ChildEdges(INode node);

        /// <summary>
        /// Returns the next sibling node of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        INode NextSibling(INode node);

        /// <summary>
        /// Returns the previous sibling node of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        INode PreviousSibling(INode node);

        /// <summary>
        /// Returns the last child of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        INode LastChild(INode node);

        /// <summary>
        /// Returns the first child of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        INode FirstChild(INode node);
        /// <summary>
        /// Returns how many children the given node has.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        int ChildCount(INode node);
        /// <summary>
        /// Returns the depth the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        int Depth(INode node);
        /// <summary>
        /// Returns the parent edge of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        IEdge ParentEdge(INode node);
        /// <summary>
        /// Executes a the given <see cref="System.Action"/> on each node, starting from the given node, by means of a traversal
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="startNode">The start node.</param>
        void ForEach<T>(Action<T> action, INode startNode) where T : INode;
    }
}
