using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Events;

namespace MathNet.Symbolics
{
    public enum FlagKind
    {
        Default = 0,
        Constraint
    }

    public sealed class NodeFlag
        : FlagAspect<MathIdentifier, NodeEvent, NodeFlag, NodeProperty>
    {
        NodeEvent _dirtiedEvent;
        NodeEvent _changedEvent;
        NodeEvent _enabledEvent;
        NodeEvent _noLongerEnabledEvent;
        FlagKind _kind;

        public static NodeFlag Register(MathIdentifier id, Type ownerType, FlagKind kind)
        {
            NodeEvent dirtiedEvent = NodeEvent.Register
            (
                id.DerivePostfix("DirtiedEvent"),
                typeof(EventHandler<NodeFlagDirtiedEventArgs>),
                ownerType
            );
            NodeEvent changedEvent = NodeEvent.Register
            (
                id.DerivePostfix("ChangedEvent"),
                typeof(EventHandler<NodeFlagChangedEventArgs>),
                ownerType
            );
            NodeEvent enabledEvent = NodeEvent.Register
            (
                id.DerivePostfix("EnabledEvent"),
                typeof(EventHandler<NodeFlagChangedEventArgs>),
                ownerType
            );
            NodeEvent noLongerEnabledEvent = NodeEvent.Register
            (
                id.DerivePostfix("NoLongerEnabledEvent"),
                typeof(EventHandler<NodeFlagChangedEventArgs>),
                ownerType
            );
            NodeFlag nf = new NodeFlag(id, ownerType, null, kind, dirtiedEvent, changedEvent, enabledEvent, noLongerEnabledEvent);
            return nf;
        }

        public static NodeFlag Register(MathIdentifier id, Type ownerType)
        {
            return Register(id, ownerType, FlagKind.Default);
        }

        public static NodeFlag Register(MathIdentifier id, Type ownerType, FlagKind kind, params NodeEventTrigger[] triggers)
        {
            NodeFlag nf = Register(id, ownerType, kind);
            foreach(NodeEventTrigger trigger in triggers)
            {
                if(trigger.RegisterAtRemoteFlag)
                {
                    trigger.RemoteFlag.AddRemoteEventTrigger(trigger, nf);
                    continue;
                }
                if(trigger.RegisterAtRemoteProperty)
                {
                    trigger.RemoteProperty.AddRemoteEventTrigger(trigger, nf);
                    continue;
                }
                nf.AddEventFlagTrigger(trigger, nf);
            }
            return nf;
        }

        private NodeFlag(MathIdentifier id, Type ownerType, IdentifierService<MathIdentifier> service, FlagKind kind,
                         NodeEvent dirtiedEvent, NodeEvent changedEvent, NodeEvent enabledEvent, NodeEvent noLongerEnabledEvent)
            : base(id, ownerType, service)
        {
            _kind = kind;
            _dirtiedEvent = dirtiedEvent;
            _changedEvent = changedEvent;
            _enabledEvent = enabledEvent;
            _noLongerEnabledEvent = noLongerEnabledEvent;
        }

        protected override void OnFlagChanged<THost>(THost host, FlagState oldState, FlagState newState)
        {
            NodeFlagChangedEventArgs e = new NodeFlagChangedEventArgs(host as Node, this, oldState, newState);
            host.RaiseEvent(_changedEvent, e);
            if(newState == FlagState.Enabled)
                host.RaiseEvent(_enabledEvent, e);
            else if(oldState == FlagState.Enabled)
                host.RaiseEvent(_noLongerEnabledEvent, e);
        }

        protected override void OnFlagDirtied<THost>(THost host)
        {
            NodeFlagDirtiedEventArgs e = new NodeFlagDirtiedEventArgs(host as Node, this);
            host.RaiseEvent(_dirtiedEvent, e);
        }

        public NodeEvent FlagDirtiedEvent
        {
            get { return _dirtiedEvent; }
        }

        public NodeEvent FlagChangedEvent
        {
            get { return _changedEvent; }
        }

        public NodeEvent FlagEnabledEvent
        {
            get { return _enabledEvent; }
        }

        public NodeEvent FlagNoLongerEnabledEvent
        {
            get { return _noLongerEnabledEvent; }
        }

        public FlagKind Kind
        {
            get { return _kind; }
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
