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

        public CoalescedChildPattern()
        {
            _childAxis = new List<CoalescedTreeNode>(4);
        }

        public void AddChild(CoalescedTreeNode node)
        {
            _childAxis.Add(node);
        }

        public List<CoalescedTreeNode> ChildrenAxis
        {
            get { return _childAxis; }
        }

        public MatchCollection MatchAll(Port port)
        {
            ReadOnlySignalSet inputs = port.InputSignals;
            if(_childAxis.Count != inputs.Count)
                return new MatchCollection();

            // Exact, Ordered Matching
            List<MatchCollection> list = new List<MatchCollection>(_childAxis.Count);
            for(int i = 0; i < _childAxis.Count; i++)
                list.Add(_childAxis[i].MatchAll(inputs[i], inputs[i].DrivenByPort));

            return MatchCollection.CombineIntersect(list); //CombineAnd(list);
        }

        public Match MatchFirst(Port port)
        {
            ReadOnlySignalSet inputs = port.InputSignals;
            if(_childAxis.Count != inputs.Count)
                return null;

            // Exact, Ordered Matching
            List<MatchCollection> list = new List<MatchCollection>(_childAxis.Count);
            for(int i = 0; i < _childAxis.Count; i++)
                list.Add(_childAxis[i].MatchAll(inputs[i], inputs[i].DrivenByPort));

            return MatchCollection.CombineIntersectFirst(list);
        }
    }
}
