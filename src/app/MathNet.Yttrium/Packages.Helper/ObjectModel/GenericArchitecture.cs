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

namespace MathNet.Symbolics.Packages.ObjectModel
{
    public class GenericArchitecture : ArchitectureBase
    {
        private ProcessBase[] _processes;
        private Signal[] _internalSignals;
        private Predicate<Port> _portSupport;

        public GenericArchitecture(MathIdentifier id, MathIdentifier entityId, bool isMathematicalOperator, Port port, Predicate<Port> portSupport, int internalSignalCount, params ProcessBase[] processes)
            : base(id, entityId, isMathematicalOperator)
        {
            _processes = processes;
            _portSupport = portSupport;
            _internalSignals = new Signal[internalSignalCount];
            for(int i = 0; i < _internalSignals.Length; i++)
                _internalSignals[i] = Binder.CreateSignal();

            //System.Diagnostics.Debug.Assert(SupportsPort(port));
            SetPort(port);

            for(int i = 0; i < processes.Length; i++)
                processes[i].Register(port.InputSignals, port.OutputSignals, _internalSignals, port.Buses);
        }

        public override bool SupportsPort(Port port)
        {
            return _portSupport(port);
        }

        public override void UnregisterArchitecture()
        {
            foreach(ProcessBase process in _processes)
                process.Unregister();
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            for(int i = 0; i < _processes.Length; i++)
                _processes[i].Register(newPort.InputSignals, newPort.OutputSignals, _internalSignals, newPort.Buses);
        }
    }
}
