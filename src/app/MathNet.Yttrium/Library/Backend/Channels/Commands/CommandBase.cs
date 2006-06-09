using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Channels.Commands
{
    public abstract class CommandBase
    {
        private bool _done;
        private MathSystem _system;

        public event EventHandler Executed;

        protected CommandBase()
        {
        }

        public MathSystem System
        {
            get { return _system; }
            set { _system = value; }
        }

        public bool Done
        {
            get { return _done; }
        }

        /// <returns>true if and only if the command should continue executing.</returns>
        public bool BeginExecute()
        {
            return !_done;
        }

        public void EndExecute()
        {
            _done = true;
            if(Executed != null)
                Executed(this, EventArgs.Empty);
        }

        protected Signal GetVerifySignal(CommandReference signalReference)
        {
            Signal signal = System.GetSignal(signalReference.Index);
            LazyVerify(signal.InstanceId, signalReference.InstanceId);
            return signal;
        }
        protected Bus GetVerifyBus(CommandReference busReference)
        {
            Bus bus = System.GetBus(busReference.Index);
            LazyVerify(bus.InstanceId, busReference.InstanceId);
            return bus;
        }
        protected Port GetVerifyPort(CommandReference portReference)
        {
            Port port = System.GetPort(portReference.Index);
            LazyVerify(port.InstanceId, portReference.InstanceId);
            return port;
        }

        protected void LazyVerify(Guid found, Guid target)
        {
            if(!(Guid.Empty.Equals(target) || found.Equals(target)))
                throw new MathNet.Symbolics.Backend.Exceptions.CommandException("Command Failed: InstanceId mismatch.");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
