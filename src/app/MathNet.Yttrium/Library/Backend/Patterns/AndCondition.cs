using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class AndCondition : Condition
    {
        private IEnumerable<Condition> _conditions;

        public AndCondition(IEnumerable<Condition> conditions)
        {
            _conditions = conditions;
        }
        public AndCondition(params Condition[] conditions)
        {
            _conditions = conditions;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            foreach(Condition c in _conditions)
                if(!c.FulfillsCondition(output, port))
                    return false;
            return true;
        }

        public override bool Equals(Condition other)
        {
            AndCondition ot = other as AndCondition;
            if(ot == null)
                return false;
            return IteratorEquals(_conditions.GetEnumerator(), ot._conditions.GetEnumerator());
        }

        public override List<CoalescedTreeNode> MergeToCoalescedTree(List<CoalescedTreeNode> parents)
        {
            List<CoalescedTreeNode> res = new List<CoalescedTreeNode>();
            foreach(CoalescedTreeNode parent in parents)
            {
                List<Condition> conditions = new List<Condition>(_conditions);
                CoalescedTreeNode localParent = parent;
                while(conditions.Count > 0)
                {
                    bool found = false;
                    foreach(CoalescedTreeNode child in localParent.ConditionAxis)
                    {
                        if(conditions.Contains(child.Condition))
                        {
                            conditions.Remove(child.Condition);
                            localParent = child;
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        CoalescedTreeNode node = new CoalescedTreeNode(conditions[conditions.Count-1]);
                        conditions.RemoveAt(conditions.Count-1);
                        localParent.ConditionAxis.Add(node);
                        localParent = node;
                    }
                }
                res.Add(localParent);
            }
            return res;
        }
    }
}
