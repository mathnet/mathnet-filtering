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

namespace MathNet.Symbolics.Manipulation
{
    [System.Diagnostics.DebuggerStepThrough]
    public class BasicManipulationVisitor : IManipulationVisitor
    {
        private EstimatePlan _plan;
        private ManipulatePort _port;
        private ManipulateSignal _signal;

        public BasicManipulationVisitor(EstimatePlan plan, ManipulatePort port, ManipulateSignal signal)
        {
            _plan = plan;
            _port = port;
            _signal = signal;
        }

        public ManipulationPlan EstimatePlan(Port port)
        {
            return _plan(port);
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
        {
            return _port(port, manipulatedInputs, hasManipulatedInputs);
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            return _signal(original, replacement, isReplaced);
        }
    }
}
