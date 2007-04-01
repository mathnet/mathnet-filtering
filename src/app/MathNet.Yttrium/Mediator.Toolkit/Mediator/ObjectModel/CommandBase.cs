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
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;

namespace MathNet.Symbolics.Mediator.ObjectModel
{
    public abstract class CommandBase
    {
        private bool _done;
        private IMathSystem _system;

        public event EventHandler Executed;

        protected CommandBase()
        {
        }

        public IMathSystem System
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _system; }
            [System.Diagnostics.DebuggerStepThrough]
            set { _system = value; }
        }

        public bool Done
        {
            get { return _done; }
        }

        /// <returns>true if and only if the command should continue executing.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public bool BeginExecute()
        {
            return !_done;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void EndExecute()
        {
            _done = true;
            if(Executed != null)
                Executed(this, EventArgs.Empty);
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected Signal GetVerifySignal(CommandReference signalReference)
        {
            Signal signal = System.GetSignal(signalReference.Index);
            LazyVerify(signal.InstanceId, signalReference.InstanceId);
            return signal;
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected Bus GetVerifyBus(CommandReference busReference)
        {
            Bus bus = System.GetBus(busReference.Index);
            LazyVerify(bus.InstanceId, busReference.InstanceId);
            return bus;
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected Port GetVerifyPort(CommandReference portReference)
        {
            Port port = System.GetPort(portReference.Index);
            LazyVerify(port.InstanceId, portReference.InstanceId);
            return port;
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected void LazyVerify(Guid found, Guid target)
        {
            if(!(Guid.Empty.Equals(target) || found.Equals(target)))
                throw new MathNet.Symbolics.Exceptions.CommandException("Command Failed: InstanceId mismatch.");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
