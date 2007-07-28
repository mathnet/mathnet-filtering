using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    public sealed class NodeEventTrigger
        : EventTrigger<MathIdentifier, NodeEvent>
    {
        private readonly NodeFlag _remoteFlag;
        private readonly NodeProperty _remoteProperty;

        /// <summary>
        /// Creates a trigger that is always activ when its flag or property is set
        /// </summary>
        public NodeEventTrigger(EventTriggerAction action, params NodeEvent[] events)
            : base(action, events)
        {
        }

        /// <summary>
        /// Creates a trigger that is always active when the other flag <see cref="remoteFlag"/> is set.
        /// </summary>
        public NodeEventTrigger(EventTriggerAction action, NodeFlag remoteFlag, params NodeEvent[] events)
            : base(action, events)
        {
            _remoteFlag = remoteFlag;
        }

        /// <summary>
        /// Creates a trigger that is always active when the other property <see cref="remoteProperty"/> is set.
        /// </summary>
        public NodeEventTrigger(EventTriggerAction action, NodeProperty remoteProperty, params NodeEvent[] events)
            : base(action, events)
        {
            _remoteProperty = remoteProperty;
        }

        public NodeFlag RemoteFlag
        {
            get { return _remoteFlag; }
        }
        public bool RegisterAtRemoteFlag
        {
            get { return _remoteFlag != null; }
        }

        public NodeProperty RemoteProperty
        {
            get { return _remoteProperty; }
        }
        public bool RegisterAtRemoteProperty
        {
            get { return _remoteProperty != null; }
        }
    }
}
