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

        protected void MergeGroupToCoalescedTree(MathIdentifier patternId, IList<CoalescedTreeNode> nodes)
        {
            if(!string.IsNullOrEmpty(_group))
                foreach(CoalescedTreeNode node in nodes)
                    node.AddGroup(patternId, _group);
        }

        public virtual void MergeToCoalescedTree(MathIdentifier patternId, IList<CoalescedTreeNode> parents)
        {
            IList<CoalescedTreeNode> nodes = _condition.MergeToCoalescedTree(parents);
            MergeGroupToCoalescedTree(patternId, nodes);
            foreach(CoalescedTreeNode node in nodes)
                node.Subscribe(patternId);
        }
    }
}
