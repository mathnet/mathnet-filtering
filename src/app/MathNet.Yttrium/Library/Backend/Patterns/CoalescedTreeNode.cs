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

        public Guid InstanceId
        {
            get { return _iid; }
        }

        public Dictionary<MathIdentifier, Match> MatchAll(Signal output, Port port)
        {
            // Check Node Condition
            if(!_condition.FulfillsCondition(output, port))
                return new Dictionary<MathIdentifier, Match>(0);

            List<Dictionary<MathIdentifier, Match>> matches = new List<Dictionary<MathIdentifier, Match>>();

            // Follow Condition Axis
            foreach(CoalescedTreeNode condition in _conditionAxis)
                matches.Add(condition.MatchAll(output, port));

            // Follow Pattern Axis
            foreach(CoalescedChildPattern pattern in _patternAxis)
                matches.Add(pattern.MatchAll(port));

            // Combine Matches
            Dictionary<MathIdentifier, Match> res = CombineOr(matches);

            // Check Subscription Axis
            foreach(MathIdentifier id in _subscriptionAxis)
                if(!res.ContainsKey(id))
                    res.Add(id, new Match(id));

            // Check Group Axis
            foreach(KeyValuePair<MathIdentifier, string> group in _groupAxis)
            {
                Match m;
                if(res.TryGetValue(group.Key, out m))
                    m.AppendGroup(group.Value, new Tuple<Signal, Port>(output, port));
            }

            return res;
        }

        /// <summary>
        /// Combines Match lists (union, logical OR). Labeled groups are merged.
        /// </summary>
        private Dictionary<MathIdentifier, Match> CombineOr(List<Dictionary<MathIdentifier, Match>> matchesList)
        {
            if(matchesList.Count == 0)
                return new Dictionary<MathIdentifier, Match>();
            if(matchesList.Count == 1)
                return matchesList[0];

            Dictionary<MathIdentifier, Match> res = matchesList[matchesList.Count - 1];
            matchesList.RemoveAt(matchesList.Count - 1);

            foreach(Dictionary<MathIdentifier, Match> matches in matchesList)
            {
                foreach(KeyValuePair<MathIdentifier, Match> match in matches)
                {
                    Match m;
                    if(res.TryGetValue(match.Key, out m))
                        foreach(KeyValuePair<string, List<Tuple<Signal, Port>>> group in match.Value)
                            m.AppendGroup(group.Key, group.Value);
                    else
                        res.Add(match.Key, match.Value);
                }
            }

            return res;
        }
    }
}
