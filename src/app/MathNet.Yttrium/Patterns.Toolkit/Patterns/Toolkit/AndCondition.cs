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

        protected override void MergeToCoalescedTreeNode(CoalescedTreeNode parent, List<CoalescedTreeNode> children)
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
                    CoalescedTreeNode node = new CoalescedTreeNode(conditions[conditions.Count - 1]);
                    conditions.RemoveAt(conditions.Count - 1);
                    localParent.ConditionAxis.Add(node);
                    localParent = node;
                }
            }
            children.Add(localParent);
        }

        protected override bool CouldMergeToCoalescedTreeNode(Condition condition)
        {
            foreach(Condition c in _conditions)
                if(c.Equals(condition))
                    return true;
            return false;
        }
    }
}
