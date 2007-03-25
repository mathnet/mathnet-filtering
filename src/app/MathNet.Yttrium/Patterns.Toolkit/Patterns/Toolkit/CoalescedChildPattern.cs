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
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Patterns.Toolkit
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

        public IList<CoalescedTreeNode> ChildrenAxis
        {
            get { return _childAxis; }
        }

        public MatchCollection MatchAll(Port port, int score)
        {
            int newScore = score + 3;

            ISignalSet inputs = port.InputSignals;
            if(_childAxis.Count != inputs.Count)
                return new MatchCollection();

            // Exact, Ordered Matching
            List<MatchCollection> list = new List<MatchCollection>(_childAxis.Count);
            for(int i = 0; i < _childAxis.Count; i++)
                list.Add(_childAxis[i].MatchAll(inputs[i], inputs[i].DrivenByPort, newScore));

            return MatchCollection.CombineIntersect(list); //CombineAnd(list);
        }

        public Match MatchFirst(Port port)
        {
            ISignalSet inputs = port.InputSignals;
            if(_childAxis.Count != inputs.Count)
                return null;

            // Exact, Ordered Matching
            List<MatchCollection> list = new List<MatchCollection>(_childAxis.Count);
            for(int i = 0; i < _childAxis.Count; i++)
                list.Add(_childAxis[i].MatchAll(inputs[i], inputs[i].DrivenByPort, 3));

            return MatchCollection.CombineIntersectFirst(list);
        }
    }
}
