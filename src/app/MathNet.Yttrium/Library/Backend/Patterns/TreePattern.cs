using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class TreePattern : Pattern
    {
        private List<Pattern> _children = new List<Pattern>(4);
        //private Pattern _catchAll = Pattern.AlwaysTrueInstance;
        //private bool _exactMatch = true;
        //private bool _ordered = true;

        public TreePattern() : base() {}
        public TreePattern(Condition condition) : base(condition) {}

        ///// <summary>
        ///// If true (default), there's a 1:1 relation between child patterns and child signals.
        ///// If false, there may be more child signals than child patterns, which are then processed by the CatchAll pattern.
        ///// </summary>
        //public bool ExactMatch
        //{
        //    get { return _exactMatch; }
        //    set { _exactMatch = value; }
        //}

        ///// <summary>
        ///// If true (default), the child signals must appear in the right order.
        ///// If false, the patterns are permutated until they fit.
        ///// </summary>
        //public bool Ordered
        //{
        //    get { return _ordered; }
        //    set { _ordered = value; }
        //}

        //public Pattern CatchAllPattern
        //{
        //    get { return _catchAll; }
        //    set { _catchAll = value; }
        //}

        public void Add(Pattern child) //, bool optional)
        {
            _children.Add(child);
        }

        public override bool Match(Signal output, Port port)
        {
            if(!base.Match(output, port))
                return false;

            //if(_exactMatch)
            //{
            //    if(_ordered)
            //    {
                    ReadOnlySignalSet inputs = port.InputSignals;
                    if(_children.Count != inputs.Count)
                        return false;
                    for(int i = 0; i < _children.Count; i++)
                    {
                        if(!_children[i].Match(inputs[i], inputs[i].DrivenByPort))
                            return false;
                    }
                    return true;
            //    }
            //    else // permutation
            //    {
            //        throw new NotImplementedException("Ineaxct matching is not implemented yet.");
            //    }
            //}
            //else // catch all
            //{
            //    throw new NotImplementedException("Out of order matching is not implemented yet.");
            //    //if(_ordered)
            //    //{
            //    //}
            //    //else // permutation
            //    //{
            //    //}
            //}
        }

        public override void MergeToCoalescedTree(MathIdentifier patternId, List<CoalescedTreeNode> parents)
        {
            if(_children.Count == 0)
                base.MergeToCoalescedTree(patternId, parents);
            else
            {
                // Merge Conditions & Groups -> "node"-List
                List<CoalescedTreeNode> nodes = Condition.MergeToCoalescedTree(parents);
                MergeGroupToCoalescedTree(patternId, nodes);
                
                // children: find matching pattern or create one
                AlwaysTrueCondition atc = AlwaysTrueCondition.Instance;
                foreach(CoalescedTreeNode node in nodes)
                {
                    // check all patterns the current node already has
                    List<CoalescedChildPattern> nodePatterns = node.PatternAxis;
                    bool nodeMatch = false;
                    foreach(CoalescedChildPattern pattern in nodePatterns)
                    {
                        // check all nodes of the current pattern whether they match.
                        List<CoalescedTreeNode> patternChildren = pattern.ChildrenAxis;
                        if(patternChildren.Count != _children.Count)
                            continue;
                        bool patternMatch = true;
                        for(int i = 0; i < _children.Count; i++)
                            if(!_children[i].Condition.CouldMergeToCoalescedTree(patternChildren[i]))
                            {
                                patternMatch = false;
                                break;
                            }
                        if(patternMatch)
                        {
                            // we found a matching pattern. merge our tree pattern to this pattern.
                            nodeMatch = true;
                            for(int i = 0; i < _children.Count; i++)
                            {
                                List<CoalescedTreeNode> list = new List<CoalescedTreeNode>();
                                list.Add(patternChildren[i]);
                                _children[i].MergeToCoalescedTree(patternId, list);
                            }
                        }
                    }
                    if(!nodeMatch)
                    {
                        // we didn't find a matching pattern. build a new such pattern.
                        CoalescedChildPattern pattern = new CoalescedChildPattern();
                        for(int i = 0; i < _children.Count; i++)
                        {
                            CoalescedTreeNode parent = new CoalescedTreeNode(AlwaysTrueCondition.Instance);
                            pattern.AddChild(parent);
                            List<CoalescedTreeNode> list = new List<CoalescedTreeNode>();
                            list.Add(parent);
                            _children[i].MergeToCoalescedTree(patternId, list);
                        }
                        node.PatternAxis.Add(pattern);
                    }
                }
            }
        }
    }
}
