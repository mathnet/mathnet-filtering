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
    public class CollectVisitor : AbstractScanVisitor, ICollectVisitor
    {
        private List<Signal> _signals;
        private List<Port> _ports;
        private List<Bus> _buses;

        public CollectVisitor()
        {
            _signals = new List<Signal>();
            _ports = new List<Port>();
            _buses = new List<Bus>();
        }

        public override IScanStrategy DefaultStrategy
        {
            get { return Strategies.AllSpanningTreeStrategy.Instance; }
        }

        public void Reset()
        {
            _signals.Clear();
            _ports.Clear();
            _buses.Clear();
        }

        public IList<Signal> Signals
        {
            get { return _signals; }
        }

        public IList<Port> Ports
        {
            get { return _ports; }
        }

        public IList<Bus> Buses
        {
            get { return _buses; }
        }

        public override bool EnterSignal(Signal signal, Port parent, bool again, bool root)
        {
            if(again)
                return false;
            _signals.Add(signal);
            return true;
        }

        public override bool EnterPort(Port port, Signal parent, bool again, bool root)
        {
            if(again)
                return false;
            _ports.Add(port);
            return true;
        }

        public override bool VisitLeaf(Bus bus, bool again)
        {
            if(!again)
                _buses.Add(bus);
            return true;
        }
    }
}
