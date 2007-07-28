using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Events;

namespace MathNet.Symbolics
{
    public sealed class NodeProperty
        : PropertyAspect<MathIdentifier, NodeEvent, NodeFlag, NodeProperty>
    {
        NodeEvent _dirtiedEvent;
        NodeEvent _changedEvent;

        public static NodeProperty Register(MathIdentifier id, Type valueType, Type ownerType)
        {
            NodeEvent dirtiedEvent = NodeEvent.Register
            (
                id.DerivePostfix("DirtiedEvent"),
                typeof(EventHandler<NodePropertyDirtiedEventArgs>),
                ownerType
            );
            NodeEvent changedEvent = NodeEvent.Register
            (
                id.DerivePostfix("ChangedEvent"),
                typeof(EventHandler<NodePropertyChangedEventArgs>),
                ownerType
            );
            NodeProperty np = new NodeProperty(id, valueType, ownerType, null, dirtiedEvent, changedEvent);
            return np;
        }

        public static NodeProperty Register(MathIdentifier id, Type valueType, Type ownerType, params NodeEventTrigger[] triggers)
        {
            NodeProperty np = Register(id, valueType, ownerType);
            foreach(NodeEventTrigger trigger in triggers)
            {
                if(trigger.RegisterAtRemoteFlag)
                {
                    trigger.RemoteFlag.AddRemoteEventTrigger(trigger, np);
                    continue;
                }
                if(trigger.RegisterAtRemoteProperty)
                {
                    trigger.RemoteProperty.AddRemoteEventTrigger(trigger, np);
                    continue;
                }
                np.AddEventPropertyTrigger(trigger, np);
            }
            return np;
        }

        private NodeProperty(MathIdentifier id, Type valueType, Type ownerType, IdentifierService<MathIdentifier> service,
                             NodeEvent dirtiedEvent, NodeEvent changedEvent)
            : base(id, valueType, ownerType, service)
        {
            _dirtiedEvent = dirtiedEvent;
            _changedEvent = changedEvent;
        }

        protected override void OnPropertyChanged<THost>(THost host, object oldValue, object newValue)
        {
            NodePropertyChangedEventArgs e = new NodePropertyChangedEventArgs(host as Node, this, oldValue, newValue);
            host.RaiseEvent(_changedEvent, e);
        }

        protected override void OnPropertyDirtied<THost>(THost host)
        {
            NodePropertyDirtiedEventArgs e = new NodePropertyDirtiedEventArgs(host as Node, this);
            host.RaiseEvent(_dirtiedEvent, e);
        }

        public NodeEvent PropertyDirtiedEvent
        {
            get { return _dirtiedEvent; }
        }

        public NodeEvent PropertyChangedEvent
        {
            get { return _changedEvent; }
        }

        internal void AddRemoteEventTrigger(NodeEventTrigger trigger, NodeFlag remoteFlag)
        {
            base.AddEventFlagTrigger(trigger, remoteFlag);
        }

        internal void AddRemoteEventTrigger(NodeEventTrigger trigger, NodeProperty remoteProperty)
        {
            base.AddEventPropertyTrigger(trigger, remoteProperty);
        }
    }
}
