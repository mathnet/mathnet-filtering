using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
    public partial class Connection : IEdge
    {
        #region Fields
        private bool mIsDirected = false;

        #endregion


        bool IEdge.IsDirected
        {
            get { return mIsDirected; }
            set { mIsDirected = value; }
        }

        INode IEdge.SourceNode
        {
            get {
                if (From.AttachedTo == null)
                    return null;
                else
                    return From.AttachedTo.Parent as INode;
            }
        }

        INode IEdge.TargetNode
        {
            get {
                if (To.AttachedTo == null)
                    return null;
                else
                    return To.AttachedTo.Parent as INode; 
            }
        }

        INode IEdge.AdjacentNode(INode node)
        {
            if (From.AttachedTo != null && (node as IShape) == From.AttachedTo.Parent)
            {
                if (To.AttachedTo == null)
                    return null;
                else
                    return To.AttachedTo.Parent as INode;
            }
            else if (To.AttachedTo.Parent != null && (node as IShape) == To.AttachedTo.Parent)
            {
                if (From.AttachedTo == null)
                    return null;
                else
                    return From.AttachedTo.Parent as INode;
            }
            else
                throw new InconsistencyException("The given node is not part of the edge.");

        }
    }
}
