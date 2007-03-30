using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
    public partial class Model : IGraph, ITree
    {
        #region Fields
        private bool mIsDirected;
        private ITree mSpanningTree;
        #endregion

        /// <summary>
        /// Get the in-degree of a node, the number of edges for which the node
        /// is the target.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int IGraph.InDegree(INode node)
        {
            return (node as INode).InDegree;
        }

        /// <summary>
        /// Get the out-degree of a node, the number of edges for which the node
        /// is the source.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int IGraph.OutDegree(INode node)
        {
            return (node as INode).OutDegree;
        }

        /// <summary>
        /// Get the degree of a node, the number of edges for which a node
        /// is either the source or the target.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int IGraph.Degree(INode node)
        {
            return (node as INode).Degree;
        }

        /// <summary>
        /// Indicates if the edges of this graph are directed or undirected.
        /// </summary>
        /// <value></value>
        bool IGraph.IsDirected
        {
            get { return mIsDirected; }
        }

        /// <summary>
        /// Returns the nodes for the current page. This is the same as the <see cref="Model.Shapes"/> property except that the return type is
        /// here <see cref="INode"/> rather than <see cref="IShape"/> type.
        /// </summary>
        /// <value></value>
        CollectionBase<INode> IGraph.Nodes
        {
            get {
                CollectionBase<INode> nodes = new CollectionBase<INode>();
                foreach (IShape shape in this.CurrentPage.Shapes)
                {
                    nodes.Add(shape as INode);
                }
                return nodes;
            }
        }

        /// <summary>
        /// Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        CollectionBase<IEdge> IGraph.Edges
        {
            get
            {
                CollectionBase<IEdge> edges = new CollectionBase<IEdge>();
                foreach (IConnection cn in this.CurrentPage.Connections)
                {
                    edges.Add(cn as IEdge);
                }
                return edges;
            }
        }
        /// <summary>
        /// Get the source Node for the given Edge instance.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        INode IGraph.FromNode(IEdge edge)
        {
            return edge.SourceNode;
        }

        INode IGraph.ToNode(IEdge edge)
        {
            return edge.TargetNode;
        }

        /// <summary>
        /// Given an Edge and an incident Node, return the other Node
        /// connected to the edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        INode IGraph.AdjacentNode(IEdge edge, INode node)
        {
            if (edge.SourceNode == node)
                return edge.TargetNode;
            else if (edge.TargetNode == node)
                return edge.SourceNode;
            else
                throw new InconsistencyException("The node is not a target or source node of the given edge.");

        }

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to the given node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        CollectionBase<INode> IGraph.InNeighbors(INode node)
        {
            return node.InNeighbors;
        }

        /// <summary>
        /// Gets the collection of adjacent nodes connected to the given node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        CollectionBase<INode> IGraph.OutNeighbors(INode node)
        {
            return node.OutNeighbors;
        }

        /// <summary>
        /// Get an iterator over all nodes connected to the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        CollectionBase<INode> IGraph.Neighbors(INode node)
        {
            return node.Neighbors;
        }

        /// <summary>
        /// Edgeses the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> IGraph.EdgesOf(INode node)
        {
            return node.Edges;
        }

        /// <summary>
        /// Ins the edges.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> IGraph.InEdges(INode node)
        {
            return node.InEdges;
        }

        /// <summary>
        /// Outs the edges.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        CollectionBase<IEdge> IGraph.OutEdges(INode node)
        {
            return node.OutEdges;
        }


        /// <summary>
        /// Gets the spanning tree.
        /// </summary>
        /// <value>The spanning tree.</value>
        ITree IGraph.SpanningTree
        {
            get { return mSpanningTree; }
        }

        /// <summary>
        /// Makes the spanning tree.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        void IGraph.MakeSpanningTree(INode node)
        {
            // build unweighted spanning tree by BFS
            LinkedList<INode> q = new LinkedList<INode>();
            BitArray visit = new BitArray(this.CurrentPage.Shapes.Count);
            q.AddFirst(node); 
            visit[this.CurrentPage.Shapes.IndexOf(node as IShape)] = true ;
            CollectionBase<IEdge> edges = (this as IGraph).Edges;
            INode n;
            while (q.Count>0)
            {
                INode p = q.First.Value;
                q.RemoveFirst();
                foreach (IEdge edge in p.Edges)
                {
                    n = edge.AdjacentNode(p);
                    if (n == null) continue;
                    try
                    {
                        if (!visit[this.CurrentPage.Shapes.IndexOf(n as IShape)])
                        {
                            q.AddLast(n);
                            visit[this.CurrentPage.Shapes.IndexOf(n as IShape)] = true;
                            n.ParentNode = p;
                            n.ParentEdge = edge;
                            if (p.Children == null)
                                p.Children = new CollectionBase<INode>();
                            p.Children.Add(n);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    { continue; }
                }
            
            }
            mSpanningTree = this as ITree;
           
        }


        void ITree.ForEach<T>(Action<T> action, INode startNode) 
        {
            (action as Action<INode>).Invoke(startNode);
            if (startNode.Children == null)
                return;
            
            foreach (INode node in startNode.Children)
            {                                
                //(action as Action<INode>).Invoke(node);
                (this as ITree).ForEach<INode>((action as Action<INode>), node);
            }

        }




        /// <summary>
        /// Clears the spanning tree.
        /// </summary>
        void IGraph.ClearSpanningTree()
        {
            mSpanningTree = null;
        }
        IEdge ITree.ParentEdge(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.ParentEdge(node);
        }

        /// <summary>
        /// Indicates if the edges of this graph are directed or undirected.
        /// </summary>
        /// <value></value>
        bool ITree.IsDirected
        {
            get { return mIsDirected; }
            set { mIsDirected = value; }
        }

        INode ITree.Root
        {
            get
            {
                return mLayoutRoot as INode;
            }
            set
            {
                mLayoutRoot = value as IShape;
            }
        }

        CollectionBase<INode> ITree.Children(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.Children(node);
        }

        CollectionBase<IEdge> ITree.ChildEdges(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.ChildEdges(node);
        }

        INode ITree.NextSibling(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return  mSpanningTree.NextSibling(node);
        }

        INode ITree.PreviousSibling(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.PreviousSibling(node);
        }

        INode ITree.LastChild(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.LastChild(node);
        }

        INode ITree.FirstChild(INode node)
        {
            if (mSpanningTree == null)
                return null;
            return mSpanningTree.FirstChild(node);
        }

        int ITree.ChildCount(INode node)
        {
            if (mSpanningTree == null)
                return 0;
            return mSpanningTree.ChildCount(node);
        }

        int ITree.Depth(INode node)
        {
            if (mSpanningTree == null)
                return 0;
            return mSpanningTree.Depth(node);
        }

        
    }
}
