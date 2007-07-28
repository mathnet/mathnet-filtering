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

namespace MathNet.Symbolics
{
    public abstract class Node
        : AspectObject<MathIdentifier, Node, NodeProperty, NodeFlag, NodeEvent>, IEquatable<Node>
    {
        private readonly Guid _iid;
        private string _label;

        protected Node()
        {
            _iid = Guid.NewGuid();
            _label = _iid.ToString();
        }

        /// <summary>
        /// Unique identifier of this instance. 
        /// </summary>
        public Guid InstanceId
        {
            get { return _iid; }
        }

        /// <summary>
        /// The name of this instance. Arbitrary and changeable.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public bool Equals(Node other)
        {
            return _iid.Equals(other._iid);
        }

        public void ClearAllFlagsWhere(FlagKind kind)
        {
            ClearAllFlagsWhere(delegate(NodeFlag flag)
            {
                return flag.Kind == kind;
            });
        }
    }
}
