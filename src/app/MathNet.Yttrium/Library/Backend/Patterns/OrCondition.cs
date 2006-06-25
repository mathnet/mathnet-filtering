using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class OrCondition : Condition
    {
        private IEnumerable<Condition> _conditions;

        public OrCondition(IEnumerable<Condition> conditions)
        {
            _conditions = conditions;
        }
        public OrCondition(params Condition[] conditions)
        {
            _conditions = conditions;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            foreach(Condition c in _conditions)
                if(c.FulfillsCondition(output, port))
                    return true;
            return false;
        }

        public override bool Equals(Condition other)
        {
            OrCondition ot = other as OrCondition;
            if(ot == null)
                return false;
            return IteratorEquals(_conditions.GetEnumerator(), ot._conditions.GetEnumerator());
        }

        protected override void MergeToCoalescedTreeNode(CoalescedTreeNode parent, List<CoalescedTreeNode> children)
        {
            foreach(Condition condition in _conditions)
            {
                CoalescedTreeNode child;
                if(!TryGetExistingNode(parent, condition, out child))
                {
                    child = new CoalescedTreeNode(condition);
                    parent.ConditionAxis.Add(child);
                }
                children.Add(child);
            }
        }

        protected override bool CouldMergeToCoalescedTreeNode(Condition condition)
        {
            return false;
        }
    }
}
