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
namespace MathNet.Symbolics.Backend.Traversing
{
    [System.Diagnostics.DebuggerStepThrough]
    public class BasicScanVisitor : IScanVisitor
    {
        private ProcessSignal _enterSignal, _leaveSignal;
        private ProcessPort _enterPort, _leavePort;
        private ProcessLeafSignal _leafSignal;
        private ProcessLeafPort _leafPort;
        private ProcessLeafBus _leafBus;
        private ProcessCycle _cycle;

        public BasicScanVisitor(ProcessSignal enterSignal, ProcessSignal leaveSignal,
            ProcessPort enterPort, ProcessPort leavePort,
            ProcessLeafSignal leafSignal, ProcessLeafPort leafPort, ProcessLeafBus leafBus,
            ProcessCycle cycle)
        {
            _enterSignal = enterSignal; _leaveSignal = leaveSignal;
            _enterPort = enterPort; _leavePort = leavePort;
            _leafSignal = leafSignal; _leafPort = leafPort; _leafBus = leafBus;
            _cycle = cycle;
        }

        public bool EnterSignal(Signal signal, Port parent, bool again, bool root)
        {
            return _enterSignal(signal, parent, again, root);
        }

        public bool LeaveSignal(Signal signal, Port parent, bool again, bool root)
        {
            return _leaveSignal(signal, parent, again, root);
        }

        public bool EnterPort(Port port, Signal parent, bool again, bool root)
        {
            return _enterPort(port, parent, again, root);
        }

        public bool LeavePort(Port port, Signal parent, bool again, bool root)
        {
            return _leavePort(port, parent, again, root);
        }

        public bool VisitLeaf(Signal signal, bool again)
        {
            return _leafSignal(signal, again);
        }

        public bool VisitLeaf(Port port, bool again)
        {
            return _leafPort(port, again);
        }

        public bool VisitLeaf(Bus bus, bool again)
        {
            return _leafBus(bus, again);
        }

        public bool VisitCycle(Port port, Signal target, Signal source)
        {
            return _cycle(port, target, source);
        }
    }
}
