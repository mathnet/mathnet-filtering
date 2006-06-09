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
    /// <remarks>This strategy activates the following visitor features: EnterSignal, LeaveSignal, EnterPort, LeavePort, VisitLeaf(Signal, Port, Bus).</remarks>
    public class AllStrategy : ScanStrategy
    {
        protected AllStrategy() : base(ConcurrencyMode.ParallelStateless) { }

        #region Singleton
        private static AllStrategy instance;
        public static AllStrategy Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if(instance == null)
                    instance = new AllStrategy();
                return instance;
            }
        }
        #endregion

        protected override void DoTraverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            List<Guid> ports = new List<Guid>();
            List<Guid> buses = new List<Guid>();
            TraverseSignal(signals, ports, buses, rootSignal, null, ignoreHold, visitor);
        }
        protected override void DoTraverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            List<Guid> ports = new List<Guid>();
            List<Guid> buses = new List<Guid>();
            TraversePort(signals, ports, buses, rootPort, null, ignoreHold, visitor);
        }
        protected override void DoTraverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            List<Guid> ports = new List<Guid>();
            List<Guid> buses = new List<Guid>();
            foreach(Signal rootSignal in rootSignals)
                if(!TraverseSignal(signals, ports, buses, rootSignal, null, ignoreHold, visitor))
                    break; // finished
        }

        private bool TraverseSignal(List<Guid> signals, List<Guid> ports, List<Guid> buses, Signal signal, Port target, bool ignoreHold, IScanVisitor visitor)
        {
            if(signal == null)
                return true;

            bool again = true;
            if(!signals.Contains(signal.InstanceId))
            {
                again = false;
                signals.Add(signal.InstanceId);
            }

            if(visitor.EnterSignal(signal, target, again, target == null))
            {
                // LEAF SIGNAL?
                if(!signal.BehavesAsBeingDriven(ignoreHold))
                {
                    if(!visitor.VisitLeaf(signal, again))
                        return false; // finished
                }
                else // HANDLE PORT
                {
                    TraversePort(signals, ports, buses, signal.DrivenByPort, signal, ignoreHold, visitor);
                }
            }
            return visitor.LeaveSignal(signal, target, again, target == null);
        }

        private bool TraversePort(List<Guid> signals, List<Guid> ports, List<Guid> buses, Port port, Signal target, bool ignoreHold, IScanVisitor visitor)
        {
            bool again = true;
            if(!ports.Contains(port.InstanceId))
            {
                again = false;
                ports.Add(port.InstanceId);
            }

            if(visitor.EnterPort(port, target, again, target == null))
            {
                // LEAF PORT?
                if(port.InputSignalCount == 0 && port.BusCount == 0)
                    if(!visitor.VisitLeaf(port, again))
                        return false; // finished

                // HANDLE BUSES
                foreach(Bus b in port.Buses)
                {
                    if(buses.Contains(b.InstanceId))
                    {
                        if(!visitor.VisitLeaf(b, true))
                            return false; // finished
                    }
                    else
                    {
                        buses.Add(b.InstanceId);
                        if(!visitor.VisitLeaf(b, false))
                            return false; // finished
                    }
                }

                // HANDLE INPUT SIGNALS
                foreach(Signal s in port.InputSignals)
                    if(!TraverseSignal(signals, ports, buses, s, port, ignoreHold, visitor))
                        return false; // finished
            }
            return visitor.LeavePort(port, target, again, target == null);
        }
    }
}
