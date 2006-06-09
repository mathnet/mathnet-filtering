#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Traversing
{
    public class CollectVisitor : AbstractScanVisitor
    {
        private SignalSet _signals;
        private PortSet _ports;
        private BusSet _buses;

        public CollectVisitor()
        {
            _signals = new SignalSet();
            _ports = new PortSet();
            _buses = new BusSet();
        }

        public void Reset()
        {
            _signals.Clear();
            _ports.Clear();
            _buses.Clear();
        }

        public SignalSet Signals
        {
            get { return _signals; }
        }

        public PortSet Ports
        {
            get { return _ports; }
        }

        public BusSet Buses
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
