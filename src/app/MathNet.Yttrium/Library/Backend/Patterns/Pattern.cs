using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class Pattern
    {
        private static Pattern _alwaysTrueInstance;

        private Condition _condition;
        private string _group;

        public Pattern()
        {
            _condition = AlwaysTrueCondition.Instance;
        }
        public Pattern(Condition condition)
        {
            _condition = condition;
        }

        public static Pattern AlwaysTrueInstance
        {
            get
            {
                if(_alwaysTrueInstance == null)
                    _alwaysTrueInstance = new Pattern();
                return _alwaysTrueInstance;
            }
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public Condition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public virtual bool Match(Signal output, Port port)
        {
            return _condition.FulfillsCondition(output, port);
        }

        protected void MergeGroupToCoalescedTree(MathIdentifier patternId, List<CoalescedTreeNode> nodes)
        {
            if(!string.IsNullOrEmpty(_group))
                foreach(CoalescedTreeNode node in nodes)
                    node.AddGroup(patternId, _group);
        }

        public virtual void MergeToCoalescedTree(MathIdentifier patternId, List<CoalescedTreeNode> parents)
        {
            List<CoalescedTreeNode> nodes = _condition.MergeToCoalescedTree(parents);
            MergeGroupToCoalescedTree(patternId, nodes);
            foreach(CoalescedTreeNode node in nodes)
                node.Subscribe(patternId);
        }
    }
}
