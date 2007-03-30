using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Analysis
{
    /// <summary>
    /// This interface describes the <see cref="Model"/> from the point of view of graph analysis.
    /// </summary>
    public interface IGraph
    {
        /// <summary>
        /// Get the in-degree of a node, the number of edges for which the node
        /// is the target.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int InDegree(INode node);
        /// <summary>
        /// Get the out-degree of a node, the number of edges for which the node
        /// is the source.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int OutDegree(INode node);
        /// <summary>
        /// Get the degree of a node, the number of edges for which a node
        /// is either the source or the target.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int Degree(INode node);
        /// <summary>
        /// Indicates if the edges of this graph are directed or undirected.
        /// </summary>
        bool IsDirected { get;}
        /// <summary>
        /// Returns the nodes for the current page. This is the same as the <see cref="Model.Shapes"/> property except that the return type is
        /// here <see cref="INode"/> rather than <see cref="IShape"/> type.
        /// </summary>
        CollectionBase<INode> Nodes { get;}
        /// <summary>
        /// Get the source Node for the given Edge instance.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        INode FromNode(IEdge edge);
        /// <summary>
        /// Get the target Node for the given Edge instance.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        INode ToNode(IEdge edge);
        /// <summary>
        ///  Given an Edge and an incident Node, return the other Node
        /// connected to the edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        INode AdjacentNode(IEdge edge, INode node);
        /// <summary>
        /// Gets the collection of all adjacent nodes connected to the given node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        CollectionBase<INode> InNeighbors(INode node);
        /// <summary>
        /// Gets the collection of adjacent nodes connected to the given node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        CollectionBase<INode> OutNeighbors(INode node);

        /// <summary>
        /// Get an iterator over all nodes connected to the given node.
        /// </summary>
        CollectionBase<INode> Neighbors(INode node);
        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        CollectionBase<IEdge> Edges { get;}
        /// <summary>
        /// Returns the edges connected to the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> EdgesOf(INode node);

        /// <summary>
        /// Returns the incoming edges of the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> InEdges(INode node);

        /// <summary>
        /// Returns the outgoing edges attached to the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> OutEdges(INode node);

        /// <summary>
        /// Gets a spanning tree of the model or graph.
        /// </summary>
        /// <value>The spanning tree.</value>
        ITree SpanningTree { get;}

        /// <summary>
        /// Makes the spanning tree.
        /// </summary>
        /// <param name="node">The node.</param>
        void MakeSpanningTree(INode node);

        /// <summary>
        /// Clears the spanning tree.
        /// </summary>
        void ClearSpanningTree();







    }
}
