using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    /// <summary>
    /// A 4D tree of coalesced patterns, useful to find all patterns of a given type and matching a signal/port tree, in an efficient manner.
    /// </summary>
    public class CoalescedTreeNode
    {
        private Guid _iid;
        private Condition _condition;
        private List<MathIdentifier> _subscriptionAxis;
        private Dictionary<MathIdentifier, string> _groupAxis;
        private List<CoalescedTreeNode> _conditionAxis;
        private List<CoalescedChildPattern> _patternAxis;

        public CoalescedTreeNode(Condition condition)
        {
            _iid = Guid.NewGuid();
            _condition = condition;
            _subscriptionAxis = new List<MathIdentifier>(4);
            _groupAxis = new Dictionary<MathIdentifier, string>(4);
            _conditionAxis = new List<CoalescedTreeNode>(4);
            _patternAxis = new List<CoalescedChildPattern>(4);
        }

        public static List<CoalescedTreeNode> CreateRootTree()
        {
            CoalescedTreeNode root = new CoalescedTreeNode(AlwaysTrueCondition.Instance);
            List<CoalescedTreeNode> list = new List<CoalescedTreeNode>();
            list.Add(root);
            return list;
        }
        public static List<CoalescedTreeNode> CreateRootTree(out CoalescedTreeNode root)
        {
            root = new CoalescedTreeNode(AlwaysTrueCondition.Instance);
            List<CoalescedTreeNode> list = new List<CoalescedTreeNode>();
            list.Add(root);
            return list;
        }

        public Guid InstanceId
        {
            get { return _iid; }
        }

        public Condition Condition
        {
            get { return _condition; }
        }

        public List<CoalescedTreeNode> ConditionAxis
        {
            get { return _conditionAxis; }
        }

        public List<CoalescedChildPattern> PatternAxis
        {
            get { return _patternAxis; }
        }

        public Dictionary<MathIdentifier, string> GroupAxis
        {
            get { return _groupAxis; }
        }

        public List<MathIdentifier> SubscriptionAxis
        {
            get { return _subscriptionAxis; }
        }

        public void Subscribe(MathIdentifier patternId)
        {
            _subscriptionAxis.Add(patternId);
        }

        public void Unsubscribe(MathIdentifier patternId)
        {
            _subscriptionAxis.Remove(patternId);
        }

        public void AddGroup(MathIdentifier patternId, string label)
        {
            _groupAxis.Add(patternId, label);
        }

        public void RemoveGroup(MathIdentifier patternId)
        {
            _groupAxis.Remove(patternId);
        }

        public MatchCollection MatchAll(Signal output, Port port, int score)
        {
            int newScore = score + _condition.Score;

            // Check Node Condition
            if(!_condition.FulfillsCondition(output, port))
                return new MatchCollection();

            List<MatchCollection> matches = new List<MatchCollection>();

            // Follow Condition Axis
            foreach(CoalescedTreeNode condition in _conditionAxis)
                matches.Add(condition.MatchAll(output, port, newScore));

            // Follow Pattern Axis
            foreach(CoalescedChildPattern pattern in _patternAxis)
                matches.Add(pattern.MatchAll(port, newScore));

            // Combine Matches
            MatchCollection res = MatchCollection.CombineUnion(matches);

            // Check Subscription Axis
            foreach(MathIdentifier id in _subscriptionAxis)
                if(!res.Contains(id))
                    res.Add(new Match(id, newScore));

            // Check Group Axis
            foreach(KeyValuePair<MathIdentifier, string> group in _groupAxis)
            {
                Match m;
                if(res.TryGetValue(group.Key, out m))
                    m.AppendGroup(group.Value, new Tuple<Signal, Port>(output, port));
            }

            return res;
        }

        /// <returns>Null, if no match was found.</returns>
        public Match MatchFirst(Signal output, Port port)
        {
            // 1. Trial: Try Local Subscriptions
            if(_subscriptionAxis.Count > 0)
            {
                Match m = new Match(_subscriptionAxis[0], 1 + _condition.Score);

                // Check Group Axis
                string label;
                if(_groupAxis.TryGetValue(m.PatternId, out label))
                    m.AppendGroup(label, new Tuple<Signal, Port>(output, port));

                return m;
            }

            // 2. Trial: Follow Condition Axis
            foreach(CoalescedTreeNode condition in _conditionAxis)
            {
                Match m = condition.MatchFirst(output, port);

                if(m != null)
                {
                    // Check Group Axis
                    string label;
                    if(_groupAxis.TryGetValue(m.PatternId, out label))
                        m.AppendGroup(label, new Tuple<Signal, Port>(output, port));

                    return m;
                }
            }

            // 3. Trial: Follow Pattern Axis.
            foreach(CoalescedChildPattern pattern in _patternAxis)
            {
                Match m = pattern.MatchFirst(port);

                if(m != null)
                {
                    // Check Group Axis
                    string label;
                    if(_groupAxis.TryGetValue(m.PatternId, out label))
                        m.AppendGroup(label, new Tuple<Signal, Port>(output, port));

                    return m;
                }
            }

            // No match found
            return null;
        }
    }
}
