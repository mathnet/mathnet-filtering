#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Patterns.Toolkit
{
    public abstract class Condition : IEquatable<Condition>
    {
        public abstract bool FulfillsCondition(Signal output, Port port);
        public abstract bool Equals(Condition other);

        /// <param name="parents">A list of nodes we have to subscribe to for matching the parent condition.</param>
        /// <returns>A list of nodes you'll have to subscribe to for matching this condition.</returns>
        public IList<CoalescedTreeNode> MergeToCoalescedTree(IList<CoalescedTreeNode> parents)
        {
            List<CoalescedTreeNode> res = new List<CoalescedTreeNode>();
            foreach(CoalescedTreeNode parent in parents)
                MergeToCoalescedTreeNode(parent, res);
            return res;
        }

        public IList<CoalescedTreeNode> MergeToCoalescedTree()
        {
            List<CoalescedTreeNode> res = new List<CoalescedTreeNode>();
            CoalescedTreeNode sentinel = new CoalescedTreeNode(AlwaysTrueCondition.Instance);
            MergeToCoalescedTreeNode(sentinel, res);
            return res;
        }

        protected virtual void MergeToCoalescedTreeNode(CoalescedTreeNode parent, List<CoalescedTreeNode> children)
        {
            CoalescedTreeNode child;
            if(!TryGetExistingNode(parent, this, out child))
            {
                child = new CoalescedTreeNode(this);
                parent.ConditionAxis.Add(child);
            }
            children.Add(child);
        }

        public virtual int Score
        {
            get { return 1; }
        }

        public bool CouldMergeToCoalescedTree(CoalescedTreeNode node)
        {
            if(node == null)
                throw new ArgumentNullException("node");

            if(node.Condition.Equals(AlwaysTrueCondition.Instance))
                return true;
            return CouldMergeToCoalescedTreeNode(node.Condition);
        }

        protected virtual bool CouldMergeToCoalescedTreeNode(Condition condition)
        {
            return Equals(condition);
        }

        protected bool IteratorEquals(IEnumerator<Condition> a, IEnumerator<Condition> b)
        {
            while(true)
            {
                if(a.MoveNext())
                {
                    if(!(b.MoveNext() && a.Current.Equals(b.Current)))
                        return false;
                }
                else // EOF(a)
                    return !b.MoveNext();
            }
        }

        protected bool TryGetExistingNode(CoalescedTreeNode parent, Condition condition, out CoalescedTreeNode node)
        {
            foreach(CoalescedTreeNode child in parent.ConditionAxis)
            {
                if(Equals(child.Condition))
                {
                    node = child;
                    return true;
                }
            }
            node = null;
            return false;
        }
    }
}
