using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the subscriptions attached to the nodes (e.g. ITransformationTheorem).</typeparam>
    public class CoalescedChildPattern
    {
        private List<CoalescedTreeNode> _childAxis;

        public CoalescedChildPattern(Guid cid)
        {
            _childAxis = new List<CoalescedTreeNode>(4);
        }

        public void AddChild(CoalescedTreeNode node)
        {
            _childAxis.Add(node);
        }

        public Dictionary<MathIdentifier, Match> MatchAll(Port port)
        {
            ReadOnlySignalSet inputs = port.InputSignals;
            if(_childAxis.Count != inputs.Count)
                return new Dictionary<MathIdentifier, Match>(0);

            // Exact, Ordered Matching
            List<Dictionary<MathIdentifier, Match>> list = new List<Dictionary<MathIdentifier, Match>>(_childAxis.Count);
            for(int i = 0; i < _childAxis.Count; i++)
                list.Add(_childAxis[i].MatchAll(inputs[i], inputs[i].DrivenByPort));

            return CombineAnd(list);
        }

        /// <summary>
        /// Combines Match lists from a list of children (inputs). To match the parent node,
        /// every match must succeed on every single child (intersection, logical AND).
        /// Labeled groups of the children are merged.
        /// </summary>
        private Dictionary<MathIdentifier, Match> CombineAnd(List<Dictionary<MathIdentifier, Match>> matchesList)
        {
            if(matchesList.Count == 1)
                return matchesList[0];
            Dictionary<MathIdentifier, Match> res = new Dictionary<MathIdentifier, Match>();
            if(matchesList.Count == 0)
                return res;
            Dictionary<MathIdentifier, Match> lastMatches = matchesList[matchesList.Count - 1];
            matchesList.RemoveAt(matchesList.Count - 1);
            foreach(KeyValuePair<MathIdentifier, Match> lastPair in lastMatches)
            {
                Match lastMatch = lastPair.Value;
                bool suitable = true;
                foreach(Dictionary<MathIdentifier, Match> matches in matchesList)
                {
                    // Ensure AND
                    Match match;
                    if(!matches.TryGetValue(lastMatch.PatternId, out match))
                    {
                        suitable = false;
                        break;
                    }
                    // Merge Groups
                    foreach(KeyValuePair<string, List<Tuple<Signal, Port>>> group in match)
                        lastMatch.AppendGroup(group.Key, group.Value);
                }
                if(suitable)
                    res.Add(lastMatch.PatternId, lastMatch);
            }
            return res;
        }
    }
}
