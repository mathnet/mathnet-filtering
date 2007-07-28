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
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Traversing;
using MathNet.Symbolics.Simulation;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.SystemBuilder.Toolkit;
using MathNet.Symbolics.AutoEvaluation;

namespace MathNet.Symbolics.Core
{
    /// <summary>
    /// Represents an Yttrium Signal. Signals are the core elements; yttrium is
    /// all about signals, their values and their relations to other signals.
    /// </summary>
    public class Signal : MathNet.Symbolics.Signal, ISchedulable, ISignal_CycleAnalysis, ISignal_BuilderAdapter, ISignal_Drive
    {
        private MathNet.Symbolics.Port _drivenByPort; // = null;
        private bool _hold; // = false;
        private bool _isSourceSignal = true;
        private int _cycleCount; // = 0;

        private PropertyBag _properties;
        private PropertyBag _constraints;

        internal Signal()
            : base()
        {
            _properties = new PropertyBag();
            _constraints = new PropertyBag();

            Service<IMediator>.Instance.NotifyNewSignalCreated(this);
        }
        internal Signal(IValueStructure value)
            : base(value)
        {
            _properties = new PropertyBag();
            _constraints = new PropertyBag();

            Service<IMediator>.Instance.NotifyNewSignalCreated(this);
        }

        #region Value & Scheduling
        /// <summary>
        /// Request the scheduler to set a new value to this signal in the next delta-timestep.
        /// </summary>
        /// <remarks>The value is not set immediately. To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least one cycle or a time instant, by calling
        /// <see cref="Scheduler.SimulateInstant"/> or <see cref="Scheduler.SimulateFor"/>.</remarks>
        public override void PostNewValue(IValueStructure newValue)
        {
            Service<ISimulationMediator>.Instance.ScheduleDeltaEvent(this, newValue);
        }

        /// <summary>
        /// Request the scheduler to set a new value to this signal with a specified simulation-time delay.
        /// </summary>
        /// <remarks>The value is not set immediately, To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least at least the specified delay, by calling
        /// <see cref="Scheduler.SimulateFor"/>.</remarks>
        /// <param name="delay">The simulation-time delay.</param>
        public override void PostNewValue(IValueStructure newValue, TimeSpan delay)
        {
            Service<ISimulationMediator>.Instance.ScheduleDelayedEvent(this, newValue, delay);
        }
        #endregion

        #region Property Bag
        public PropertyBag Properties
        {
            get { return _properties; }
        }

        public PropertyBag Constraints
        {
            get { return _constraints; }
        }

        /// <summary>Checks whether a property is set.</summary>
        public override bool HasProperty(MathIdentifier propertyId)
        {
            return _constraints.ContainsProperty(propertyId) || _properties.ContainsProperty(propertyId);
        }

        /// <summary>Updates the property accoring to propagation theorems and checks whether it is (or should be) set.</summary>
        public override bool AskForProperty(MathIdentifier propertyType)
        {
            if(_constraints.ContainsProperty(propertyType))
                return true;

            //ITheoremProvider provider;
            //if(Service<ILibrary>.Instance.TryLookupTheoremType(new MathIdentifier("", "TheoremProvider"), out provider))
            //{
            //}

            // TODO: IMPLEMENT, as soons as the new property provider infrastructure is available

            //PropertyProviderTable table;
            //if(((Library)_context.Library).Theorems.TryLookupPropertyProvider(propertyType, out table))
            //    return table.UpdateProperty(this);
            //else
            //    return Properties.ContainsProperty(propertyType);
            return false;
        }

        public override void AddConstraint(IProperty property)
        {
            _constraints.AddProperty(property);
        }

        public override void RemoveAllConstraints()
        {
            _constraints.RemoveAllProperties();
        }
        #endregion

        protected override void OnAutoEvaluateFlag(NodeFlag flag)
        {
            Service<IAutoEvaluator>.Instance.AutoEvaluateFlag(this, flag);
        }

        protected override void OnAutoEvaluateProperty(NodeProperty property)
        {
            Service<IAutoEvaluator>.Instance.AutoEvaluateProperty(this, property);
        }

        #region Drive State
        /// <summary>
        /// True if this signal is driven by a port (attached to a port output).
        /// </summary>
        public override bool IsDriven
        {
            get { return _drivenByPort != null; }
        }

        

        /// <summary>
        /// Writeable. True if this signal is either undriven or declared as being a source signal.
        /// </summary>
        public override bool IsSourceSignal
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
        public override bool Hold
        {
            get { return _hold; }
            set { _hold = value; }
        }

        /// <summary>
        /// The port this signal is attached to.
        /// </summary>
        public override MathNet.Symbolics.Port DrivenByPort
        {
            get { return _drivenByPort; }
        }

        void ISignal_Drive.DriveSignal(MathNet.Symbolics.Port source, int outputIndex)
        {
            if(IsDriven)
                Service<IMediator>.Instance.NotifyPortDrivesSignalNoLonger(this, _drivenByPort, _drivenByPort.OutputSignals.IndexOf(this));

            _drivenByPort = source;
            _isSourceSignal = false;
            _properties.ValidatePropertiesAfterDrive(this);

            Service<IMediator>.Instance.NotifyPortDrivesSignal(this, source, outputIndex);
        }
        void ISignal_Drive.UndriveSignal(int outputIndex)
        {
            if(IsDriven)
                Service<IMediator>.Instance.NotifyPortDrivesSignalNoLonger(this, _drivenByPort, outputIndex);

            _drivenByPort = null;
            _isSourceSignal = true;
            _properties.ValidatePropertiesAfterUndrive(this);
        }
        #endregion

        #region Operators: Builder Shortcuts
        protected override MathNet.Symbolics.Signal AddSignalCore(MathNet.Symbolics.Signal summand)
        {
            return Std.Add(this, summand);
        }

        protected override MathNet.Symbolics.Signal NegateSignalCore()
        {
            return StdBuilder.Negate(this);
        }

        protected override MathNet.Symbolics.Signal SubtractSignalCore(MathNet.Symbolics.Signal subtrahend)
        {
            return Std.Subtract(this, subtrahend);
        }

        protected override MathNet.Symbolics.Signal MultiplySignalCore(MathNet.Symbolics.Signal multiplier)
        {
            return Std.Multiply(this, multiplier);
        }

        protected override MathNet.Symbolics.Signal DivideSignalCore(MathNet.Symbolics.Signal divisor)
        {
            return Std.Divide(this, divisor);
        }
        #endregion

        #region Cycle Tracking [TODO: Port to new Traversing System]
        public override bool IsCyclic
        {
            get { return _cycleCount > 0; }
        }
        public override int Cycles
        {
            get { return _cycleCount; }
        }

        /// <remarks>needs to be called as soon as soon as it becomes connected through its driven port to an output signal -> run with each output signal as source parameter.</remarks>
        public int AddCycles(MathNet.Symbolics.Signal source, int tag)
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
                if(!((IPort_CycleAnalysis)_drivenByPort).TagWasTagged(tag)) //was not tagged with this tag before.
                {
                    foreach(Signal item in _drivenByPort.InputSignals)
                        if(item != null)
                            ret += item.AddCycles(source, tag);
                    ((IPort_CycleAnalysis)_drivenByPort).DeTag(tag);
                }
                _cycleCount += ret;
                return ret;
            }
        }

        /// <remarks>needs to be called as soon as soon as it becomes disconnected through its driven port to an output signal -> run with each output signal as source parameter.</remarks>
        public int RemoveCycles(MathNet.Symbolics.Signal source, int tag)
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
                if(!((IPort_CycleAnalysis)_drivenByPort).TagWasTagged(tag)) //was not tagged with this tag before.
                {
                    foreach(Signal item in _drivenByPort.InputSignals)
                        ret += item.RemoveCycles(source, tag);
                    ((IPort_CycleAnalysis)_drivenByPort).DeTag(tag);
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
        public override bool DependsOn(MathNet.Symbolics.Signal signal)
        {
            return Service<IScanner>.Instance.ExistsSignal(this, signal.Equals, true);
        }
        /// <summary>
        /// True if the signal depends on a specified port.
        /// </summary>
        public override bool DependsOn(MathNet.Symbolics.Port port)
        {
            return Service<IScanner>.Instance.ExistsPort(this, port.Equals, true);
        }
        /// <summary>
        /// True if the signal depends on a port with the specified entity.
        /// </summary>
        public override bool DependsOn(MathIdentifier portEntity)
        {
            return Service<IScanner>.Instance.ExistsPort(this, delegate(MathNet.Symbolics.Port p) { return p.Entity.EntityId.Equals(portEntity); }, true);
        }
        #endregion

        #region System Builder
        Guid ISignal_BuilderAdapter.AcceptSystemBuilderBefore(ISystemBuilder builder)
        {
            return builder.BuildSignal(Label, _hold, _isSourceSignal);
        }
        void ISignal_BuilderAdapter.AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            Guid guid = signalMappings[InstanceId];
            if(Value != null)
                builder.AppendSignalValue(guid, CustomDataPack<IValueStructure>.Pack(Value, signalMappings, busMappings));
            foreach(IProperty property in _properties)
                builder.AppendSignalProperty(guid, CustomDataPack<IProperty>.Pack(property, signalMappings, busMappings));
            foreach(IProperty constraint in _constraints)
                builder.AppendSignalConstraint(guid, CustomDataPack<IProperty>.Pack(constraint, signalMappings, busMappings));
        }
        void ISignal_BuilderAdapter.BuilderSetValue(IValueStructure structure)
        {
            SetPresentValue(structure);
        }
        void ISignal_BuilderAdapter.BuilderAppendProperty(IProperty property)
        {
            _properties.AddProperty(property);
        }
        void ISignal_BuilderAdapter.BuilderAppendConstraint(IProperty constraint)
        {
            _constraints.AddProperty(constraint);
        }
        #endregion

        //#region Instance Equality
        ///// <remarks>Two signals are equal only if they are the same instance. Use <see cref="IsEquivalentTo"/> instead if you need to check for equivalent signals.</remarks>
        //public bool Equals(Signal other)
        //{
        //    return other != null && _iid.Equals(other.InstanceId);
        //}
        ///// <remarks>Two signals are equal only if they are the same instance. Use <see cref="IsEquivalentTo"/> instead if you need to check for equivalent signals.</remarks>
        //public override bool Equals(object obj)
        //{
        //    Signal other = obj as Signal;
        //    if(other == null)
        //        return false;
        //    else
        //        return _iid.Equals(other._iid);
        //}
        //public override int GetHashCode()
        //{
        //    return _iid.GetHashCode();
        //}
        //#endregion

        public override string ToString()
        {
            string name = string.IsNullOrEmpty(Label) ? "Signal " + InstanceId.ToString() : "Signal " + Label;
            if(Value != null)
                name += " (" + Value.ToString() + ")";
            string driven = IsDriven ? "driven" : "undriven";
            string cyclic = IsCyclic ? "cyclic" : "noncyclic";
            string hold = Hold ? "hold" : "nonhold";
            return name + " [" + driven + ";" + cyclic + ";" + hold + "]";
        }

        #region ISchedulable Members

        bool ISchedulable.HasEvent
        {
            get { return HasEvent; }
            set { base.SetHasEvent(value); }
        }

        IValueStructure ISchedulable.CurrentValue
        {
            get { return Value; }
            set { base.SetPresentValue(value); }
        }

        void ISchedulable.NotifyOutputsNewValue()
        {
            OnValueChanged();
        }
        #endregion
    }
}