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
    /// <remarks>This strategy activates the following visitor features: EnterPort, LeavePort.</remarks>
    public class AllPortsStrategy : ScanStrategy
    {
        protected AllPortsStrategy() : base(ConcurrencyMode.ParallelStateless) { }

        #region Singleton
        private static AllPortsStrategy instance;
        public static AllPortsStrategy Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if(instance == null)
                    instance = new AllPortsStrategy();
                return instance;
            }
        }
        #endregion

        protected override void DoTraverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            if(rootSignal.BehavesAsBeingDriven(ignoreHold))
            {
                List<Guid> ports = new List<Guid>();
                TraversePort(ports, rootSignal.DrivenByPort, rootSignal, ignoreHold, visitor);
            }
        }
        protected override void DoTraverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> ports = new List<Guid>();
            TraversePort(ports, rootPort, null, ignoreHold, visitor);
        }
        protected override void DoTraverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> ports = new List<Guid>();
            foreach(Signal rootSignal in rootSignals)
                if(rootSignal.BehavesAsBeingDriven(ignoreHold))
                    if(!TraversePort(ports, rootSignal.DrivenByPort, rootSignal, ignoreHold, visitor))
                        break; // finished
        }

        private bool TraversePort(List<Guid> ports, Port port, Signal target, bool ignoreHold, IScanVisitor visitor)
        {
            if(ports.Contains(port.InstanceId))
                return true;
            ports.Add(port.InstanceId);
            if(visitor.EnterPort(port, target, false, target == null))
            {
                foreach(Signal input in port.InputSignals)
                    if(input != null && input.BehavesAsBeingDriven(ignoreHold))
                        if(!TraversePort(ports, input.DrivenByPort, input, ignoreHold, visitor))
                            return false; // finished
            }
            return visitor.LeavePort(port, target, false, target == null);
        }
    }
}
