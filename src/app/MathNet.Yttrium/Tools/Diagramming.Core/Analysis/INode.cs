using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Analysis
{
    /// <summary>
    /// This interface describes the <see cref="IShape"/> from the point of view of graph analysis.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Get the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        int InDegree { get;}
        /// <summary>
        /// Get the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        int OutDegree { get;}
        /// <summary>
        /// Get the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        int Degree { get;}
        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the target.
        /// </summary>
        CollectionBase<IEdge> InEdges { get;}
        /// <summary>
        /// Gets or sets whether this shape is fixed with respect to diagram layout
        /// </summary>           
        bool IsFixed { get; set;}
        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsExpanded { get; set;}
        /// <summary>
        /// Gets the unique identifier of this node.
        /// </summary>
        /// <value>The uid.</value>
        Guid Uid { get;}
        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        CollectionBase<IEdge> OutEdges { get;}
        /// <summary>
        ///Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        CollectionBase<IEdge> Edges { get;}
        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        CollectionBase<INode> InNeighbors { get;}
        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        CollectionBase<INode> OutNeighbors { get;}

        /// <summary>
        /// Get an iterator over all nodes connected to this node.
        /// </summary>
        CollectionBase<INode> Neighbors { get;}

        /// <summary>
        /// Get the parent node of this node in a tree structure.
        /// </summary>
        INode ParentNode { get; set;}
        /// <summary>
        /// Get the edge between this node and its parent node in a tree
        /// structure.
        /// </summary>
        IEdge ParentEdge { get; set;}
        /// <summary>
        /// Get the tree depth of this node.
        ///<remarks>The root's tree depth is
        /// zero, and each level of the tree is one depth level greater.
        /// </remarks> 
        /// </summary>
        int Depth { get; set;}
        /// <summary>
        /// Get the number of tree children of this node.
        /// </summary>
        int ChildCount { get;}
        /// <summary>
        /// Get this node's first tree child.
        /// </summary>
        INode FirstChild { get;}
        /// <summary>
        /// Get this node's last tree child.
        /// </summary>
        INode LastChild { get;}

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X.</value>
        int X { get;}
        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y.</value>
        int Y { get;}

        /// <summary>
        /// Get this node's previous tree sibling.
        /// </summary>
        INode PreviousSibling { get;}
         /// <summary>
        /// Get this node's next tree sibling.
         /// </summary>
        INode NextSibling { get;}

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        Rectangle Rectangle { get;}
        /// <summary>
        /// Get an iterator over this node's tree children.
        /// </summary>
        CollectionBase<INode> Children { get; set;}
        /// <summary>
        /// Get an iterator over the edges from this node to its tree children.
        /// </summary>
        CollectionBase<IEdge> ChildEdges { get;}

        /// <summary>
        /// Moves the node, the argument being the motion vector.
        /// </summary>
        /// <param name="p">The p.</param>
        void Move(Point p);
    }
  
   
  
  
}
