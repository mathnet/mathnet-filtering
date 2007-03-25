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

namespace MathNet.Symbolics.Traversing.Visitors
{
    public class PortPathCollectVisitor : AbstractScanVisitor, IPortPathCollectVisitor
    {
        private List<List<Port>> _paths;
        private Stack<Port> _currentPath;
        private Port _source;

        public PortPathCollectVisitor(Port source)
        {
            _paths = new List<List<Port>>();
            _currentPath = new Stack<Port>();
            _source = source;
        }

        public override IScanStrategy DefaultStrategy
        {
            get { return Strategies.AllPathsStrategy.Instance; }
        }

        public void Reset(Port source)
        {
            _paths.Clear();
            _currentPath.Clear();
            _source = source;
        }

        public List<List<Port>> Paths
        {
            get { return _paths; }
        }

        public override bool EnterPort(Port port, Signal parent, bool again, bool root)
        {
            _currentPath.Push(port);
            if(_source.Equals(port))
            {
                List<Port> p = new List<Port>(_currentPath);
                _paths.Add(p);
                return false;
            }
            return base.EnterPort(port, parent, again, root);
        }

        public override bool LeavePort(Port port, Signal parent, bool again, bool root)
        {
            _currentPath.Pop();
            return base.LeavePort(port, parent, again, root);
        }
    }
}
