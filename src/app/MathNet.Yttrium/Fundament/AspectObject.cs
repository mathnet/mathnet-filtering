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

namespace MathNet.Symbolics
{
    public abstract class AspectObject<TIdentifier, TObject, TProperty, TFlag, TEvent>
        : IEventAspectHost<TIdentifier, TEvent>
        where TIdentifier : IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TProperty : PropertyAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TFlag : FlagAspect<TIdentifier, TEvent, TFlag, TProperty>
        where TEvent : EventAspect<TIdentifier, TEvent>
        where TObject : AspectObject<TIdentifier, TObject, TProperty, TFlag, TEvent>
    {
        private Dictionary<TProperty, Dirtyable<object>> _properties;
        private Dictionary<TFlag, Dirtyable<bool>> _flags;
        private Dictionary<TEvent, Delegate> _handlers;
        private Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>>> _propertyEventTriggers;
        private Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>>> _flagEventTriggers;

        // TODO: what we actually need for _handlers is not a list of delegates
        // but a list of WeakDelegates (Delegate with WeakReference to it's target).
        // Unfortunately the .Net Framework doesn't provide such a thing yet afaik...

        #region Property Aspects
        public void SetProperty(TProperty property, object value)
        {
            //if(!property.IsValidValue(value))
            //    throw new ArgumentException();
            if(_properties == null)
            {
                _properties = new Dictionary<TProperty, Dirtyable<object>>();
                _propertyEventTriggers = new Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>>>();
            }

            Dirtyable<object> old;
            object oldValue = null;
            if(_properties.TryGetValue(property, out old))
                oldValue = old.Value;
            else
                WirePropertyEventTriggers(property);

            _properties[property] = new Dirtyable<object>(value);

            if(value != oldValue)
                property.RaisePropertyChanged((TObject)this, oldValue, value);
        }

        public object GetProperty(TProperty property)
        {
            if(_properties == null)
                throw new System.ArgumentOutOfRangeException("property");
            return _properties[property].Value;
        }
        public object GetProperty(TProperty property, object defaultValue)
        {
            return GetProperty(property, defaultValue, false);
        }
        public object GetProperty(TProperty property, object defaultValue, bool acceptDirty)
        {
            object ret;
            if(TryGetProperty(property, out ret, acceptDirty))
                return ret;
            return defaultValue;
        }
        public bool TryGetProperty(TProperty property, out object value)
        {
            return TryGetProperty(property, out value, false);
        }
        public bool TryGetProperty(TProperty property, out object value, bool acceptDirty)
        {
            if(_properties == null)
            {
                value = null;
                return false;
            }
            Dirtyable<Object> obj;
            if(_properties.TryGetValue(property, out obj) && (acceptDirty || !obj.IsDirty))
            {
                value = obj.Value;
                return true;
            }
            value = null;
            return false;
        }

        public bool AskForProperty(TProperty property, out object value)
        {
            return AskForProperty(property, out value, false, false);
        }
        public bool AskForProperty(TProperty property, out object value, bool acceptDirty, bool forceRefresh)
        {
            if(forceRefresh)
                AutoEvaluateProperty(property);
            if(_properties == null)
            {
                value = null;
                return false;
            }
            Dirtyable<Object> obj;
            if(_properties.TryGetValue(property, out obj) && (acceptDirty || !obj.IsDirty))
            {
                value = obj.Value;
                return true;
            }
            if(!forceRefresh)
                AutoEvaluateProperty(property);
            if(_properties.TryGetValue(property, out obj) && (acceptDirty || !obj.IsDirty))
            {
                value = obj.Value;
                return true;
            }
            value = null;
            return false;
        }

        public void ClearProperty(TProperty property)
        {
            if(_properties == null)
                return;
            
            Dirtyable<object> old;
            object oldValue = null;
            if(_properties.TryGetValue(property, out old))
            {
                oldValue = old.Value;
                UnwirePropertyEventTriggers(property);
            }

            _properties.Remove(property);

            if(oldValue != null)
                property.RaisePropertyChanged((TObject)this, oldValue, null);
        }

        public void DirtyPropertyIfSet(TProperty property)
        {
            if(_properties == null)
                return;
            Dirtyable<Object> obj;
            if(_properties.TryGetValue(property, out obj))
            {
                obj.IsDirty = true;
                _properties[property] = obj;
            }
        }

        public bool IsPropertySet(TProperty property)
        {
            return IsPropertySet(property, false);
        }
        public bool IsPropertySet(TProperty property, bool acceptDirty)
        {
            if(_properties == null)
                return false;
            if(acceptDirty)
                return _properties.ContainsKey(property);
            Dirtyable<Object> obj;
            return _properties.TryGetValue(property, out obj) && !obj.IsDirty;
        }

        public bool IsPropertyDirty(TProperty property)
        {
            if(_properties == null)
                return false;
            Dirtyable<Object> obj;
            return _properties.TryGetValue(property, out obj) && obj.IsDirty;
        }

        public void AutoEvaluateProperty(TProperty property)
        {
            OnAutoEvaluateProperty(property);
        }

        protected virtual void OnAutoEvaluateProperty(TProperty property)
        {
        }

        private void WirePropertyEventTriggers(TProperty property)
        {
            foreach(KeyValuePair<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> trigger in property.EventPropertyTriggers)
            {
                List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> list;
                if(!_propertyEventTriggers.TryGetValue(trigger.Key, out list))
                {
                    list = new List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>>();
                    _propertyEventTriggers.Add(trigger.Key, list);
                }
                list.Add(trigger.Value);
            }
        }
        private void UnwirePropertyEventTriggers(TProperty property)
        {
            foreach(KeyValuePair<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> trigger in property.EventPropertyTriggers)
            {
                List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> list;
                if(!_propertyEventTriggers.TryGetValue(trigger.Key, out list))
                    continue;
                list.Remove(trigger.Value);
                if(list.Count == 0)
                    _propertyEventTriggers.Remove(trigger.Key);
            }
        }
        #endregion

        #region Threestate-Flag Aspects
        public void SetFlagState(TFlag flag, FlagState state)
        {
            if(_flags == null)
            {
                if(state == FlagState.Unknown)
                    return;
                _flags = new Dictionary<TFlag, Dirtyable<bool>>();
                _flagEventTriggers = new Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>>>();
            }
            FlagState oldState = GetFlagState(flag);
            if(state == FlagState.Unknown)
            {
                UnwireFlagEventTriggers(flag);
                _flags.Remove(flag);
            }
            else
            {
                if(!_flags.ContainsKey(flag))
                    WireFlagEventTriggers(flag);
                _flags[flag] = new Dirtyable<bool>(state == FlagState.Enabled);
            }

            if(oldState != state)
                flag.RaiseFlagChanged((TObject)this, oldState, state);
        }
        public void EnableFlag(TFlag flag)
        {
            if(_flags == null)
            {
                _flags = new Dictionary<TFlag, Dirtyable<bool>>();
                _flagEventTriggers = new Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>>>();
            }
            FlagState oldState = GetFlagState(flag);
            if(!_flags.ContainsKey(flag))
                WireFlagEventTriggers(flag);
            _flags[flag] = new Dirtyable<bool>(true);

            if(oldState != FlagState.Enabled)
                flag.RaiseFlagChanged((TObject)this, oldState, FlagState.Enabled);
        }
        public void DisableFlag(TFlag flag)
        {
            if(_flags == null)
            {
                _flags = new Dictionary<TFlag, Dirtyable<bool>>();
                _flagEventTriggers = new Dictionary<TEvent, List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>>>();
            }
            FlagState oldState = GetFlagState(flag);
            if(!_flags.ContainsKey(flag))
                WireFlagEventTriggers(flag);
            _flags[flag] = new Dirtyable<bool>(false);

            if(oldState != FlagState.Disabled)
                flag.RaiseFlagChanged((TObject)this, oldState, FlagState.Disabled);
        }
        public void ClearFlag(TFlag flag)
        {
            if(_flags == null)
                return;
            FlagState oldState = GetFlagState(flag);
            UnwireFlagEventTriggers(flag);
            _flags.Remove(flag);

            if(oldState != FlagState.Unknown)
                flag.RaiseFlagChanged((TObject)this, oldState, FlagState.Unknown);
        }
        public void DirtyFlagIfSet(TFlag flag)
        {
            if(_flags == null)
                return;
            Dirtyable<bool> obj;
            if(_flags.TryGetValue(flag, out obj))
            {
                obj.IsDirty = true;
                _flags[flag] = obj;
            }
        }
        public void ClearAllFlagsWhere(Predicate<TFlag> condition)
        {
            List<TFlag> flags = new List<TFlag>(_flags.Keys);
            foreach(TFlag flag in flags)
                if(condition(flag))
                    ClearFlag(flag);
        }

        public FlagState GetFlagState(TFlag flag)
        {
            if(_flags == null)
                return FlagState.Unknown;
            Dirtyable<bool> ret;
            if(_flags.TryGetValue(flag, out ret))
                return ret.Value ? FlagState.Enabled : FlagState.Disabled;
            return FlagState.Unknown;
        }

        public FlagState AskForFlag(TFlag flag)
        {
            return AskForFlag(flag, false, false);
        }
        public FlagState AskForFlag(TFlag flag, bool acceptDirty, bool forceRefresh)
        {
            if(forceRefresh)
                AutoEvaluateFlag(flag);
            if(_flags == null)
                return FlagState.Unknown;
            Dirtyable<bool> ret;
            if(_flags.TryGetValue(flag, out ret) && (acceptDirty || !ret.IsDirty))
                return ret.Value ? FlagState.Enabled : FlagState.Disabled;
            if(!forceRefresh)
                AutoEvaluateFlag(flag);
            if(_flags.TryGetValue(flag, out ret) && (acceptDirty || !ret.IsDirty))
                return ret.Value ? FlagState.Enabled : FlagState.Disabled;
            return FlagState.Unknown;
        }

        public bool IsFlagSet(TFlag flag)
        {
            return IsFlagSet(flag, false);
        }
        public bool IsFlagSet(TFlag flag, bool acceptDirty)
        {
            if(_flags == null)
                return false;
            if(acceptDirty)
                return _flags.ContainsKey(flag);
            Dirtyable<bool> ret;
            return _flags.TryGetValue(flag, out ret) && !ret.IsDirty;
        }
        public bool IsFlagEnabled(TFlag flag)
        {
            return IsFlagEnabled(flag, false);
        }
        public bool IsFlagEnabled(TFlag flag, bool acceptDirty)
        {
            if(_flags == null)
                return false;
            Dirtyable<bool> ret;
            return _flags.TryGetValue(flag, out ret) && ret.Value && (acceptDirty || !ret.IsDirty);
        }
        public bool IsFlagDisabled(TFlag flag)
        {
            return IsFlagDisabled(flag, false);
        }
        public bool IsFlagDisabled(TFlag flag, bool acceptDirty)
        {
            if(_flags == null)
                return false;
            Dirtyable<bool> ret;
            return _flags.TryGetValue(flag, out ret) && !ret.Value && (acceptDirty || !ret.IsDirty);
        }
        public bool IsFlagUnknown(TFlag flag)
        {
            if(_flags == null)
                return true;
            return !_flags.ContainsKey(flag);
        }
        public bool IsFlagDirty(TFlag flag)
        {
            if(_flags == null)
                return false;
            Dirtyable<bool> obj;
            return _flags.TryGetValue(flag, out obj) && obj.IsDirty;
        }

        public void AutoEvaluateFlag(TFlag flag)
        {
            OnAutoEvaluateFlag(flag);
        }

        protected virtual void OnAutoEvaluateFlag(TFlag flag)
        {
        }

        private void WireFlagEventTriggers(TFlag flag)
        {
            foreach(KeyValuePair<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> trigger in flag.EventFlagTriggers)
            {
                List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> list;
                if(!_flagEventTriggers.TryGetValue(trigger.Key, out list))
                {
                    list = new List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>>();
                    _flagEventTriggers.Add(trigger.Key, list);
                }
                list.Add(trigger.Value);
            }
        }
        private void UnwireFlagEventTriggers(TFlag flag)
        {
            foreach(KeyValuePair<TEvent, TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> trigger in flag.EventFlagTriggers)
            {
                List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> list;
                if(!_flagEventTriggers.TryGetValue(trigger.Key, out list))
                    continue;
                list.Remove(trigger.Value);
                if(list.Count == 0)
                    _flagEventTriggers.Remove(trigger.Key);
            }
        }
        #endregion

        #region Event Aspects
        public void AddHandler(TEvent eventId, Delegate handler)
        {
            if(!eventId.IsValidHandler(handler))
                throw new ArgumentException();
            if(_handlers == null)
                _handlers = new Dictionary<TEvent, Delegate>();
            Delegate list;
            if(_handlers.TryGetValue(eventId, out list))
                _handlers[eventId] = Delegate.Combine(list, handler);
            else
                _handlers[eventId] = handler;
        }
        public void RemoveHandler(TEvent eventId, Delegate handler)
        {
            if(_handlers == null)
                return;
            Delegate list;
            if(_handlers.TryGetValue(eventId, out list))
            {
                list = Delegate.Remove(list, handler);
                if(list == null || list.GetInvocationList().Length == 0)
                    _handlers.Remove(eventId);
                else
                    _handlers[eventId] = list;
            }
        }
        public void ClearHandlers(TEvent eventId)
        {
            if(_handlers == null)
                return;
            _handlers.Remove(eventId);
        }
        public bool HasHandler(TEvent eventId)
        {
            if(_handlers == null)
                return false;
            return _handlers.ContainsKey(eventId);
        }
        //public void RaiseEvent(TEvent eventId, EventArgs e)
        //{
        //    if(_handlers == null)
        //        return;
        //    Delegate list;
        //    if(_handlers.TryGetValue(eventId, out list))
        //    {
        //        list.DynamicInvoke(this, e);
        //    }
        //}
        public void RaiseEvent<TEventArgs>(TEvent eventId, TEventArgs e)
            where TEventArgs : EventArgs
        {
            // Call Event Handlers
            if(_handlers != null)
            {
                Delegate list;
                if(_handlers.TryGetValue(eventId, out list))
                {
                    EventHandler<TEventArgs> handler = (EventHandler<TEventArgs>)list;
                    handler(this, e);
                }
            }

            // Handle Property Event Triggers
            List<TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty>> propertyTriggers;
            if(_propertyEventTriggers != null
                && _propertyEventTriggers.TryGetValue(eventId, out propertyTriggers))
                for(int i= propertyTriggers.Count - 1; i>=0; i--)
                {
                    TriggerableEventTrigger<TIdentifier, TEvent, TProperty, TFlag, TProperty> trigger = propertyTriggers[i];
                    switch(trigger.Action)
                    {
                        case EventTriggerAction.Clear:
                            ClearProperty(trigger.Aspect);
                            break;
                        case EventTriggerAction.Dirty:
                            DirtyPropertyIfSet(trigger.Aspect);
                            break;
                        case EventTriggerAction.Reevaluate:
                            OnAutoEvaluateProperty(trigger.Aspect);
                            break;
                    }
                }

            // Handle Flag Event Triggers
            List<TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty>> flagTriggers;
            if(_flagEventTriggers != null
                && _flagEventTriggers.TryGetValue(eventId, out flagTriggers))
                for(int i= flagTriggers.Count - 1; i>=0; i--)
                {
                    TriggerableEventTrigger<TIdentifier, TEvent, TFlag, TFlag, TProperty> trigger = flagTriggers[i];
                    switch(trigger.Action)
                    {
                        case EventTriggerAction.Clear:
                            ClearFlag(trigger.Aspect);
                            break;
                        case EventTriggerAction.Dirty:
                            DirtyFlagIfSet(trigger.Aspect);
                            break;
                        case EventTriggerAction.Disable:
                            DisableFlag(trigger.Aspect);
                            break;
                        case EventTriggerAction.Enable:
                            EnableFlag(trigger.Aspect);
                            break;
                        case EventTriggerAction.Reevaluate:
                            OnAutoEvaluateFlag(trigger.Aspect);
                            break;
                    }
                }
        }
        #endregion
    }
}
