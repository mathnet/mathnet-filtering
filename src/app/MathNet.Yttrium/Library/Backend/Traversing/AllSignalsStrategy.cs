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
    /// <remarks>This strategy activates the following visitor features: EnterSignal, LeaveSignal.</remarks>
    public class AllSignalsStrategy : ScanStrategy
    {
        protected AllSignalsStrategy() : base(ConcurrencyMode.ParallelStateless) { }

        #region Singleton
        private static AllSignalsStrategy instance;
        public static AllSignalsStrategy Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if(instance == null)
                    instance = new AllSignalsStrategy();
                return instance;
            }
        }
        #endregion

        protected override void DoTraverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            TraverseSignal(signals, rootSignal, null, ignoreHold, visitor);
        }
        protected override void DoTraverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            foreach(Signal input in rootPort.InputSignals)
                if(!TraverseSignal(signals, input, rootPort, ignoreHold, visitor))
                    break; // finished
        }
        protected override void DoTraverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            List<Guid> signals = new List<Guid>();
            foreach(Signal rootSignal in rootSignals)
                if(!TraverseSignal(signals, rootSignal, null, ignoreHold, visitor))
                    break; // finished
        }

        private bool TraverseSignal(List<Guid> signals, Signal signal, Port target, bool ignoreHold, IScanVisitor visitor)
        {
            if(signal == null || signals.Contains(signal.InstanceId))
                return true;
            signals.Add(signal.InstanceId);
            if(visitor.EnterSignal(signal, target, false, target == null))
            {
                if(signal.BehavesAsBeingDriven(ignoreHold))
                {
                    Port port = signal.DrivenByPort;
                    foreach(Signal input in port.InputSignals)
                        if(!TraverseSignal(signals, input, port, ignoreHold, visitor))
                            return false; // finished
                }
            }
            return visitor.LeaveSignal(signal, target, false, target == null);
        }
    }
}
