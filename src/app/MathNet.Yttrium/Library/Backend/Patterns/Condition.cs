using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public abstract class Condition : IEquatable<Condition>
    {
        public abstract bool FulfillsCondition(Signal output, Port port);
        public abstract bool Equals(Condition other);

        /// <param name="parents">A list of nodes we have to subscribe to for matching the parent condition.</param>
        /// <returns>A list of nodes you'll have to subscribe to for matching this condition.</returns>
        public virtual List<CoalescedTreeNode> MergeToCoalescedTree(List<CoalescedTreeNode> parents)
        {
            List<CoalescedTreeNode> res = new List<CoalescedTreeNode>();
            foreach(CoalescedTreeNode parent in parents)
            {
                CoalescedTreeNode child;
                if(!TryGetExistingNode(parent, this, out child))
                {
                    child = new CoalescedTreeNode(this);
                    parent.ConditionAxis.Add(child);
                }
                res.Add(child);
            }
            return res;
        }

        protected bool IteratorEquals(IEnumerator<Condition> a, IEnumerator<Condition> b)
        {
            while(true)
            {
                if(a.MoveNext())
                {
                    if(! (b.MoveNext() && a.Current.Equals(b.Current)))
                        return false;
                }
                else // EOF(a)
                    return !b.MoveNext();
            }
        }

        protected bool TryGetExistingNode(CoalescedTreeNode parent, Condition condition,out CoalescedTreeNode node)
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
