using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    internal class TriggerableEventTrigger<TIdentifier, TEvent, TAspect, TFlag, TProperty>
        where TIdentifier : IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TEvent : EventAspect<TIdentifier, TEvent>
        where TFlag : FlagAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TProperty : PropertyAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TAspect : TriggerableAspect<TIdentifier, TEvent, TFlag, TProperty>
    {
        private TEvent _event;
        private TAspect _aspect;
        private EventTriggerAction _action;

        internal TriggerableEventTrigger(TEvent eventId, TAspect aspect, EventTriggerAction action)
        {
            _event = eventId;
            _aspect = aspect;
            _action = action;
        }

        public TEvent Event
        {
            get { return _event; }
        }

        public TAspect Aspect
        {
            get { return _aspect; }
        }

        public EventTriggerAction Action
        {
            get { return _action; }
        }
    }

    internal class TriggerableTriggers<TIdentifier, TEvent, TAspect, TFlag, TProperty>
        where TIdentifier : IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TEvent : EventAspect<TIdentifier, TEvent>
        where TFlag : FlagAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TProperty : PropertyAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TAspect : TriggerableAspect<TIdentifier, TEvent, TFlag, TProperty>
    {
        private Dictionary<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TAspect, TFlag, TProperty>> _eventTriggers;

        private void EnsureInit()
        {
            if(_eventTriggers == null)
                _eventTriggers = new Dictionary<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TAspect, TFlag, TProperty>>();
        }

        public Dictionary<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TAspect, TFlag, TProperty>> EventTriggers
        {
            get
            {
                EnsureInit();
                return _eventTriggers;
            }
        }

        public void AddEventTrigger(EventTrigger<TIdentifier, TEvent> trigger, TAspect aspect)
        {
            EnsureInit();
            AddEventTriggerImpl(trigger, aspect);
        }
        public void AddEventTrigger(IEnumerable<EventTrigger<TIdentifier, TEvent>> triggers, TAspect aspect)
        {
            EnsureInit();
            foreach(EventTrigger<TIdentifier, TEvent> trigger in triggers)
                AddEventTriggerImpl(trigger, aspect);
        }
        private void AddEventTriggerImpl(EventTrigger<TIdentifier, TEvent> trigger, TAspect aspect)
        {
            EventTriggerAction action = trigger.Action;
            foreach(TEvent eventId in trigger.Events)
                _eventTriggers.Add(eventId,
                    new TriggerableEventTrigger<TIdentifier, TEvent, TAspect, TFlag, TProperty>(eventId, aspect, action));
        }
    }

    public class TriggerableAspect<TIdentifier, TEvent, TFlag, TProperty>
        : Identifier<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TEvent : EventAspect<TIdentifier, TEvent>
        where TFlag : FlagAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TProperty : PropertyAspect<TIdentifier, TEvent, TFlag, TProperty>
    {
        private Type _ownerType;
        TriggerableTriggers<TIdentifier, TEvent, TFlag, TFlag, TProperty> _eventFlagTriggers;
        TriggerableTriggers<TIdentifier, TEvent, TProperty, TFlag, TProperty> _eventPropertyTriggers;

        public TriggerableAspect(TIdentifier friendly, Type ownerType, IdentifierService<TIdentifier> service)
            : base(friendly, service)
        {
            _ownerType = ownerType;
            _eventFlagTriggers = new TriggerableTriggers<TIdentifier, TEvent, TFlag, TFlag, TProperty>();
            _eventPropertyTriggers = new TriggerableTriggers<TIdentifier, TEvent, TProperty, TFlag, TProperty>();
        }

        public Type OwnerType
        {
            get { return _ownerType; }
        }

        internal Dictionary<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> EventFlagTriggers
        {
            get { return _eventFlagTriggers.EventTriggers; }
        }
        internal Dictionary<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> EventPropertyTriggers
        {
            get { return _eventPropertyTriggers.EventTriggers; }
        }

        protected void AddEventFlagTrigger(EventTrigger<TIdentifier, TEvent> trigger, TFlag flag)
        {
            _eventFlagTriggers.AddEventTrigger(trigger, flag);
        }
        protected void AddEventPropertyTrigger(EventTrigger<TIdentifier, TEvent> trigger, TProperty property)
        {
            _eventPropertyTriggers.AddEventTrigger(trigger, property);
        }

    }
}
