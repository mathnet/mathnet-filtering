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
    /// <remarks>This strategy activates the following visitor features: EnterSignal, LeaveSignal, EnterPort, LeavePort, VisitLeaf(Signal, Port, Bus), VisitCycle.</remarks>
    public class AllPathsStrategy : ScanStrategy
    {
        protected AllPathsStrategy() : base(ConcurrencyMode.ParallelStateless) { }

        #region Singleton
        private static AllPathsStrategy instance;
        public static AllPathsStrategy Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if(instance == null)
                    instance = new AllPathsStrategy();
                return instance;
            }
        }
        #endregion

        protected override void DoTraverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            Stack<Guid> signals = new Stack<Guid>();
            TraverseSignal(signals, rootSignal, null, ignoreHold, visitor);
        }
        protected override void DoTraverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            Stack<Guid> signals = new Stack<Guid>();
            TraverseRootPort(signals, rootPort, ignoreHold, visitor);
        }
        protected override void DoTraverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            Stack<Guid> signals = new Stack<Guid>();
            foreach(Signal rootSignal in rootSignals)
            {
                signals.Clear();
                if(!TraverseSignal(signals, rootSignal, null, ignoreHold, visitor))
                    break;
            }
        }

        private void TraverseRootPort(Stack<Guid> signals, Port rootPort, bool ignoreHold, IScanVisitor visitor)
        {
            if(visitor.EnterPort(rootPort, null, false, true))
            {
                foreach(Bus b in rootPort.Buses)
                    if(!visitor.VisitLeaf(b, false))
                        return; // finished
                foreach(Signal s in rootPort.InputSignals)
                    if(!TraverseSignal(signals, s, rootPort, ignoreHold, visitor))
                        return; // finished
            }
            visitor.LeavePort(rootPort, null, false, true);
        }

        private bool TraverseSignal(Stack<Guid> signals, Signal signal, Port target, bool ignoreHold, IScanVisitor visitor)
        {
            if(signal == null)
                return true;

            if(visitor.EnterSignal(signal, target, false, target == null))
            {
                signals.Push(signal.InstanceId);

                // HANDLE PORT
                if(signal.BehavesAsBeingDriven(ignoreHold))
                {
                    Port port = signal.DrivenByPort;

                    if(visitor.EnterPort(port, signal, false, false))
                    {
                        // LEAF PORT?
                        if(port.InputSignalCount == 0 && port.BusCount == 0)
                            if(!visitor.VisitLeaf(port, false))
                                return false; // finished

                        // HANDLE BUSES
                        foreach(Bus b in port.Buses)
                            if(!visitor.VisitLeaf(b, false))
                                return false; // finished

                        // HANDLE INPUT SIGNALS
                        foreach(Signal s in port.InputSignals)
                        {
                            if(signals.Contains(s.InstanceId))
                            {
                                if(!visitor.VisitCycle(port, signal, s))
                                    return false; //finished
                            }
                            else if(!TraverseSignal(signals, s, port, ignoreHold, visitor))
                                return false; // finished
                        }
                    }
                    if(!visitor.LeavePort(port, signal, false, false))
                        return false; // finished
                }
                else if(!visitor.VisitLeaf(signal, false))
                    return false; // finished

                signals.Pop();
            }
            return visitor.LeaveSignal(signal, target, false, target == null);
        }
    }
}
