using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
    public abstract partial class ShapeBase : INode
    {
        #region Fields
        /// <summary>
        /// the parent node
        /// </summary>
        private INode mParentNode = null;
        private IEdge mParentEdge = null;
        private bool mIsExpanded = true;
        private CollectionBase<INode> treeChildren = null;
        private int mDepth = -1;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether this shape is fixed with respect to diagram layout
        /// </summary>
        /// <value></value>
        bool INode.IsFixed
        {
            get { return mIsFixed; }
            set { mIsFixed = value; }
        }
        
        /// <summary>
        /// Gets or sets the IsExpanded
        /// </summary>
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; }
        }


        /// <summary>
        /// Get the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        /// <value></value>
        int INode.InDegree
        {
            get
            {
                return (this as INode).InEdges.Count;
            }
        }

        /// <summary>
        /// Get the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        /// <value></value>
        int INode.OutDegree
        {
            get
            {
                return (this as INode).OutEdges.Count;
            }
        }

        /// <summary>
        /// Get the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        /// <value></value>
        int INode.Degree
        {
            get
            {
                return (this as INode).Edges.Count;
            }
        }
        

        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the target.
        /// </summary>
        /// <value></value>
        CollectionBase<IEdge> INode.InEdges
        {
            get
            {

                CollectionBase<IEdge> col = (this as INode).Edges;
                CollectionBase<IEdge> ret = new CollectionBase<IEdge>();
                col.ForEach(
                      delegate(IEdge edge)
                      {
                          if (edge.TargetNode == this)
                              ret.Add(edge);
                      }
                    );
                return ret;
            }
        }

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        /// <value></value>
        CollectionBase<IEdge> INode.OutEdges
        {
            get
            {

                CollectionBase<IEdge> col = (this as INode).Edges;
                CollectionBase<IEdge> ret = new CollectionBase<IEdge>();
                col.ForEach(
                      delegate(IEdge edge)
                      {
                          if (edge.SourceNode == this)
                              ret.Add(edge);
                      }
                    );
                return ret;
            }
        }

        /// <summary>
        /// Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        /// <value></value>
        CollectionBase<IEdge> INode.Edges
        {
            get
            {
                CollectionBase<IEdge> col = new CollectionBase<IEdge>();
                this.Connectors.ForEach(
                    delegate(IConnector cn)
                    {
                        foreach (IConnector cnn in cn.AttachedConnectors)
                            if (cnn.Parent is IEdge)
                                col.Add(cnn.Parent as IEdge);
                    }
                );

                return col;
            }
        }

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        /// <value></value>
        CollectionBase<INode> INode.InNeighbors
        {
            get
            {
                CollectionBase<IEdge> edges = (this as INode).InEdges;
                CollectionBase<INode> ret = new CollectionBase<INode>();
                edges.ForEach(
                      delegate(IEdge edge)
                      {
                          ret.Add(edge.SourceNode);
                      }
                    );
                return ret;
            }
        }

        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        /// <value></value>
        CollectionBase<INode> INode.OutNeighbors
        {
            get
            {
                CollectionBase<IEdge> edges = (this as INode).OutEdges;
                CollectionBase<INode> ret = new CollectionBase<INode>();
                edges.ForEach(
                      delegate(IEdge edge)
                      {
                          ret.Add(edge.TargetNode);
                      }
                    );
                return ret;
            }
        }


        /// <summary>
        /// Get an iterator over all nodes connected to this node.
        /// </summary>
        /// <value></value>
        CollectionBase<INode> INode.Neighbors
        {
            get
            {
                CollectionBase<IEdge> edges = (this as INode).Edges;
                CollectionBase<INode> ret = new CollectionBase<INode>();
                edges.ForEach(
                    delegate(IEdge edge)
                    {
                        if (edge.SourceNode == this)
                            ret.Add(edge.TargetNode);
                        else
                            ret.Add(edge.SourceNode);
                    }
                    );

                return ret;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the entity
        /// </summary>
        /// <value></value>
        INode INode.ParentNode
        {
            get
            {
                return mParentNode;
                ;
            }
            set
            {
                mParentNode = value;
            }
        }

        /// <summary>
        /// Get the edge between this node and its parent node in a tree
        /// structure.
        /// </summary>
        /// <value></value>
        IEdge INode.ParentEdge
        {
            get {
                //return (this.Model as IGraph).SpanningTree.ParentEdge(this); 
                return mParentEdge;
            }
            set {
                mParentEdge = value;
            }

        }

        /// <summary>
        /// Get the tree depth of this node.
        /// <remarks>The root's tree depth is
        /// zero, and each level of the tree is one depth level greater.
        /// </remarks>
        /// </summary>
        /// <value></value>
        int INode.Depth
        {
            get { return mDepth; }
            set { mDepth = value; }
        }

        /// <summary>
        /// Get the number of tree children of this node.
        /// </summary>
        /// <value></value>
        int INode.ChildCount
        {
            get {
                if (this.treeChildren == null)
                    return 0;
                else
                    return this.treeChildren.Count; 
            }
        }

        /// <summary>
        /// Get this node's first tree child.
        /// </summary>
        /// <value></value>
        INode INode.FirstChild
        {
            get
            {
                if (this.treeChildren == null)
                    return null;
                else
                    return treeChildren[0];
            }
        }

        /// <summary>
        /// Get this node's last tree child.
        /// </summary>
        /// <value></value>
        INode INode.LastChild
        {
            get {
                if (this.treeChildren == null)
                    return null;
                else
                    return treeChildren[this.treeChildren.Count-1];
            }
        }

        /// <summary>
        /// Get this node's previous tree sibling.
        /// </summary>
        /// <value></value>
        INode INode.PreviousSibling
        {
            get
            {
                if ((this as INode).ParentNode == null)
                    return null;
                else
                {
                    CollectionBase<INode> ch = (this as INode).ParentNode.Children;
                    int chi = ch.IndexOf(this as INode);
                    if (chi < 0 || chi > ch.Count - 1)
                        throw new IndexOutOfRangeException();
                    
                    if (chi == 0)
                        return null;
                    else
                        return ch[chi - 1];
                }
            }
        }

        /// <summary>
        /// Get this node's next tree sibling.
        /// </summary>
        /// <value></value>
        INode INode.NextSibling
        {
            get
            {
                if ((this as INode).ParentNode == null)
                    return null;
                else
                {
                    CollectionBase<INode> ch = (this as INode).ParentNode.Children;
                    int chi = ch.IndexOf(this as INode);
                    if (chi < 0 || chi > ch.Count - 1)
                        throw new IndexOutOfRangeException();
                    
                    if (chi == ch.Count-1)
                        return null;
                    else
                        return ch[chi + 1];
                }
            }
        }

        /// <summary>
        /// Gets the collection of this node's tree children.
        /// </summary>
        /// <value></value>
        CollectionBase<INode> INode.Children
        {
            get { return treeChildren; }
            set { treeChildren = value; }
        }

        /// <summary>
        /// Gets the edges from this node to its tree children.
        /// </summary>
        /// <value></value>
        CollectionBase<IEdge> INode.ChildEdges
        {
            get { return (this.Model as IGraph).SpanningTree.ChildEdges(this); }
        }

        #endregion
    }
}
