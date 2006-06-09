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
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;
using MathNet.Symbolics.Backend.Simulation;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Events;
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.Backend.SystemBuilder;

namespace MathNet.Symbolics.Core
{
    /// <summary>
    /// Represents an Yttrium Signal. Signals are the core elements; yttrium is
    /// all about signals, their values and their relations to other signals.
    /// </summary>
    public class Signal : IContextSensitive, IEquatable<Signal>
    {
        private readonly Guid _iid;
        private readonly Context _context;
        private string _label = string.Empty;

        private Port _drivenByPort; // = null;
        private bool _hold; // = false;
        private bool _isSourceSignal = true;
        private bool _hasEvent; // = false;
        private int _cycleCount; // = 0;

        private PropertyBag _properties;
        private PropertyBag _constraints;

        private ValueStructure _presentStructure; // = null;
        private SignalDelayedEventTimeline _eventQueue;

        private SetValue _setValue;
        private SetEventFlag _setEventFlag;
        private NotifyNewValue _notifyNewValue;

        public event EventHandler<SignalEventArgs> SignalValueChanged;

        public Signal(Context context)
        {
            if(context == null) throw new ArgumentNullException("context");
            _context = context;
            _iid = _context.GenerateInstanceId();

            _properties = new PropertyBag(context);
            _constraints = new PropertyBag(context);
            _eventQueue = new SignalDelayedEventTimeline(context.Scheduler);

            // Prepare Scheduler Callbacks
            _setValue = delegate(ValueStructure newValue)
            {
                if(newValue == null)
                    return false;
                bool different = _presentStructure == null || !_presentStructure.Equals(newValue);
                _presentStructure = newValue;
                _properties.ValidatePropertiesAfterEvent(this);
                _context.NotifySignalValueChanged(this);
                return different;
            };
            _setEventFlag = delegate(bool flagEvent) { _hasEvent = flagEvent; };
            _notifyNewValue = delegate()
            {
                EventHandler<SignalEventArgs> handler = SignalValueChanged;
                if(handler != null)
                    handler(this, new SignalEventArgs(this));
            };

            context.NotifyNewSignalConstructed(this);
        }
        public Signal(Context context, ValueStructure value)
            : this(context)
        {
            _presentStructure = value;
            //this.setValue(Value);
        }

        /// <summary>
        /// Unique identifier of this signal (and this class instance). 
        /// </summary>
        public Guid InstanceId
        {
            get { return _iid; }
        }

        /// <summary>
        /// The context in which this signal is defined and used.
        /// </summary>
        public Context Context
        {
            get { return _context; }
        }

        /// <summary>
        /// The name of this signal. Arbitrary and changeable.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        #region Value & Scheduling
        /// <summary>
        /// True only in the scheduler process execution phase, and only if the
        /// there is an event related to this signal, that is it's value has changed.
        /// </summary>
        public bool HasEvent
        {
            get { return _hasEvent; }
        }

        /// <summary>
        /// The present value of this signal.
        /// </summary>
        public ValueStructure Value
        {
            get { return _presentStructure; }
            //set {PostNewValue(value);}
        }

        /// <summary>
        /// Request the scheduler to set a new value to this signal in the next delta-timestep.
        /// </summary>
        /// <remarks>The value is not set immediately. To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least one cycle or a time instant, by calling
        /// <see cref="Scheduler.SimulateInstant"/> or <see cref="Scheduler.SimulateFor"/>.</remarks>
        public void PostNewValue(ValueStructure newValue)
        {
            _context.Scheduler.ScheduleDeltaEvent(new SignalEventItem(this, newValue, _setValue, _setEventFlag, _notifyNewValue));
        }

        /// <summary>
        /// Request the scheduler to set a new value to this signal with a specified simulation-time delay.
        /// </summary>
        /// <remarks>The value is not set immediately, To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least at least the specified delay, by calling
        /// <see cref="Scheduler.SimulateFor"/>.</remarks>
        /// <param name="delay">The simulation-time delay.</param>
        public void PostNewValue(ValueStructure newValue, TimeSpan delay)
        {
            _context.Scheduler.ScheduleDelayedEvent(new TimedSignalEventItem(new SignalEventItem(this, newValue, _setValue, _setEventFlag, _notifyNewValue), delay));
        }

        /// <summary>
        /// The event queue for this signal. Lists all future delayed events associated with this event, excluding any delta-events.
        /// </summary>
        public SignalDelayedEventTimeline EventQueue
        {
            get { return _eventQueue; }
        }
        #endregion

        #region Property Bag
        public PropertyBag Properties
        {
            get
            {
                if(BehavesAsSourceSignal)
                    return _constraints;
                else
                    return _properties;
            }
        }

        /// <summary>Checks whether a property is set.</summary>
        public bool HasProperty(MathIdentifier propertyId)
        {
            return Properties.ContainsProperty(propertyId);
        }

        /// <summary>Updates the property accoring to propagation theorems and checks whether it is (or should be) set.</summary>
        public bool AskForProperty(string propertyLabel, string propertyDomain)
        {
            return AskForProperty(new MathIdentifier(propertyLabel, propertyDomain));
        }
        public bool AskForProperty(MathIdentifier propertyType)
        {
            PropertyProviderTable table;
            if(_context.Library.Theorems.TryLookupPropertyProvider(propertyType, out table))
                return table.UpdateProperty(this);
            else
                return Properties.ContainsProperty(propertyType);
        }

        public void AddConstraint(Property property)
        {
            _constraints.AddProperty(property);
        }

        public void RemoveAllConstraints()
        {
            _constraints.RemoveAllProperties();
        }
        #endregion

        #region Drive State
        /// <summary>
        /// True if this signal is driven by a port (attached to a port output).
        /// </summary>
        public bool IsDriven
        {
            get { return _drivenByPort != null; }
        }

        /// <summary>
        /// True if this signal is driven by a port and, if <paramref name="ignoreHold"/> is false, the signal is not being hold.
        /// </summary>
        /// <param name="ignoreHold">if true, this method returns the same value as the <see cref="IsDriven"/> property.</param>
        public bool BehavesAsBeingDriven(bool ignoreHold)
        {
            return (_drivenByPort != null) && (ignoreHold || !_hold);
        }

        /// <summary>
        /// Writeable. True if this signal is either undriven or declared as being a source signal.
        /// </summary>
        public bool IsSourceSignal
        {
            get { return _isSourceSignal; }
            set
            {
                _isSourceSignal = value;
                if(_drivenByPort == null)
                    _isSourceSignal = true;
            }
        }

        /// <summary>
        /// Writeable. True if the signal should behave as if it weren't driven.
        /// </summary>
        public bool Hold
        {
            get { return _hold; }
            set { _hold = value; }
        }

        /// <summary>
        /// True if this signal is either a source signal or just behaves so because of being hold.
        /// </summary>
        public bool BehavesAsSourceSignal
        {
            get { return _hold || _isSourceSignal; }
        }

        /// <summary>
        /// The port this signal is attached to.
        /// </summary>
        public Port DrivenByPort
        {
            get { return _drivenByPort; }
        }

        internal void DriveSignal(Port source, int outputIndex)
        {
            if(IsDriven)
                _context.NotifySignalNoLongerDrivenByPort(this, _drivenByPort, _drivenByPort.OutputSignals.IndexOf(this));

            _drivenByPort = source;
            _isSourceSignal = false;
            _properties.ValidatePropertiesAfterDrive(this);

            _context.NotifySignalDrivenByPort(this, source, outputIndex);
        }
        internal void UndriveSignal(int outputIndex)
        {
            if(IsDriven)
                _context.NotifySignalNoLongerDrivenByPort(this, _drivenByPort, outputIndex);
            _drivenByPort = null;
            _isSourceSignal = true;
            _properties.ValidatePropertiesAfterUndrive(this);
        }

        /// <summary>
        /// True if the signal is driven by a port and this port has the specified entity.
        /// </summary>
        public bool IsDrivenByPortEntity(MathIdentifier entityId)
        {
            if(!IsDriven)
                return false;
            return _drivenByPort.Entity.EntityId.Equals(entityId);
        }
        /// <summary>
        /// True if the signal is driven by a port and this port has the specified entity.
        /// </summary>
        public bool IsDrivenByPortEntity(string entityLabel, string entityDomain)
        {
            if(!IsDriven)
                return false;
            return _drivenByPort.Entity.EntityId.Equals(entityLabel, entityDomain);
        }
        #endregion

        #region Operators: Builder Shortcuts
        /// <summary>
        /// Shortcut for the binary addition operation of the standard package, Std.Add.
        /// </summary>
        public static Signal operator +(Signal summand1, Signal summand2)
        {
            if(summand1 == null) throw new ArgumentNullException("summand1");
            return summand1.Context.Builder.AddSimplified(summand1, summand2);
        }
        /// <summary>
        /// Unary add, just returns the signal (does nothing).
        /// </summary>
        public static Signal operator +(Signal summand)
        {
            return summand;
        }

        /// <summary>
        /// Shortcut for the binary subtraction operation of the standard package, Std.Subtract.
        /// </summary>
        public static Signal operator -(Signal minuend, Signal subtrahend)
        {
            if(minuend == null) throw new ArgumentNullException("minuend");
            return minuend.Context.Builder.SubtractSimplified(minuend, subtrahend);
        }
        /// <summary>
        /// Shortcut for the unary subtraction operation of the standard package, Std.Subtract.
        /// </summary>
        public static Signal operator -(Signal subtrahend)
        {
            if(subtrahend == null) throw new ArgumentNullException("subtrahend");
            return subtrahend.Context.Builder.Negate(subtrahend);
        }

        /// <summary>
        /// Shortcut for the binary multiplication operation of the standard package, Std.Multiply.
        /// </summary>
        public static Signal operator *(Signal multiplicand, Signal multiplier)
        {
            if(multiplicand == null) throw new ArgumentNullException("multiplicand");
            return multiplicand.Context.Builder.MultiplySimplified(multiplicand, multiplier);
        }

        /// <summary>
        /// Shortcut for the binary division operation of the standard package, Std.Divide.
        /// </summary>
        public static Signal operator /(Signal dividend, Signal divisor)
        {
            if(dividend == null) throw new ArgumentNullException("dividend");
            return dividend.Context.Builder.DivideSimplified(dividend, divisor);
        }
        #endregion

        #region Cycle Tracking [TODO: Port to new Traversing System]
        public bool IsCyclic
        {
            get { return _cycleCount > 0; }
        }
        public int Cycles
        {
            get { return _cycleCount; }
        }

        /// <remarks>needs to be called as soon as soon as it becomes connected through its driven port to an output signal -> run with each output signal as source parameter.</remarks>
        internal int AddCycles(Signal source, int tag)
        {
            if(this == source)
            {
                _cycleCount++;
                return 1;
            }
            if(IsSourceSignal || !IsDriven)
                return 0;
            else
            {
                int ret = 0;
                if(!_drivenByPort.TagWasTagged(tag)) //was not tagged with this tag before.
                {
                    foreach(Signal item in _drivenByPort.InputSignals)
                        if(item != null)
                            ret += item.AddCycles(source, tag);
                    _drivenByPort.DeTag(tag);
                }
                _cycleCount += ret;
                return ret;
            }
        }

        /// <remarks>needs to be called as soon as soon as it becomes disconnected through its driven port to an output signal -> run with each output signal as source parameter.</remarks>
        internal int RemoveCycles(Signal source, int tag)
        {
            if(this == source)
            {
                System.Diagnostics.Debug.Assert(_cycleCount >= 1);
                _cycleCount--;
                return 1;
            }
            if(IsSourceSignal || !IsDriven)
                return 0;
            else
            {
                int ret = 0;
                if(!_drivenByPort.TagWasTagged(tag)) //was not tagged with this tag before.
                {
                    foreach(Signal item in _drivenByPort.InputSignals)
                        ret += item.RemoveCycles(source, tag);
                    _drivenByPort.DeTag(tag);
                }
                System.Diagnostics.Debug.Assert(_cycleCount >= ret);
                _cycleCount -= ret;
                return ret;
            }
        }
        #endregion

        #region Dependency Analysis
        /// <summary>
        /// True if the signal depends on a specified signal, and thus posting a new value to the specified signal may influence this signal's value.
        /// </summary>
        public bool DependsOn(Signal signal)
        {
            return Scanner.ExistsSignal(this, signal.Equals, true);
        }
        /// <summary>
        /// True if the signal depends on a specified port.
        /// </summary>
        public bool DependsOn(Port port)
        {
            return Scanner.ExistsPort(this, port.Equals, true);
        }
        /// <summary>
        /// True if the signal depends on a port with the specified entity.
        /// </summary>
        public bool DependsOn(MathIdentifier portEntity)
        {
            return Scanner.ExistsPort(this, delegate(Port p) { return p.Entity.EntityId.Equals(portEntity); }, true);
        }
        #endregion

        #region System Builder
        internal Guid AcceptSystemBuilderBefore(ISystemBuilder builder)
        {
            return builder.BuildSignal(_label, _hold, _isSourceSignal);
        }
        internal void AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            Guid guid = signalMappings[_iid];
            if(_presentStructure != null)
                builder.AppendSignalValue(guid, StructurePack.Pack(_presentStructure, signalMappings, busMappings));
            foreach(Property property in _properties)
                builder.AppendSignalProperty(guid, PropertyPack.Pack(property, signalMappings, busMappings));
            foreach(Property constraint in _constraints)
                builder.AppendSignalConstraint(guid, PropertyPack.Pack(constraint, signalMappings, busMappings));
        }
        internal void BuilderSetValue(ValueStructure structure)
        {
            _presentStructure = structure;
        }
        internal void BuilderAppendProperty(Property property)
        {
            _properties.AddProperty(property);
        }
        internal void BuilderAppendConstraint(Property constraint)
        {
            _constraints.AddProperty(constraint);
        }
        #endregion

        #region Instance Equality
        /// <remarks>Two signals are equal only if they are the same instance. Use <see cref="IsEquivalentTo"/> instead if you need to check for equivalent signals.</remarks>
        public bool Equals(Signal other)
        {
            return other != null && _iid.Equals(other._iid);
        }
        /// <remarks>Two signals are equal only if they are the same instance. Use <see cref="IsEquivalentTo"/> instead if you need to check for equivalent signals.</remarks>
        public override bool Equals(object obj)
        {
            Signal other = obj as Signal;
            if(other == null)
                return false;
            else
                return _iid.Equals(other._iid);
        }
        public override int GetHashCode()
        {
            return _iid.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            string name = string.IsNullOrEmpty(_label) ? "Signal " + _iid.ToString() : "Signal " + _label;
            if(_presentStructure != null)
                name += " (" + _presentStructure.ToString() + ")";
            string driven = IsDriven ? "driven" : "undriven";
            string cyclic = IsCyclic ? "cyclic" : "noncyclic";
            string hold = Hold ? "hold" : "nonhold";
            return name + " [" + driven + ";" + cyclic + ";" + hold + "]";
        }
    }
}