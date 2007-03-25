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

namespace MathNet.Symbolics.Patterns.Toolkit
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
