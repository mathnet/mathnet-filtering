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

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.SystemBuilder;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Traversing;

namespace MathNet.Symbolics.Core
{
    /// <summary>
    /// Represents an Yttrium Port. Ports connect signals by operations defined
    /// in interchangeable architectures. 
    /// </summary>
    public class Port : IContextSensitive, IEquatable<Port>
    {
        private readonly Guid _iid;
        private readonly Context _context;

        private SignalSet _inputSignalSet;
        private SignalSet _outputSignalSet;
        private BusSet _busSet;

        private readonly Entity _entity;
        private Architecture _currentArchitecture;
        private int _tag;
        private bool _completelyConnected;

        internal Port(Context context, Entity entity)
        {
            _context = context;
            _iid = _context.GenerateInstanceId();
            _entity = entity;

            _inputSignalSet = new SignalSet(entity.InputSignals.Length);
            _outputSignalSet = new SignalSet(entity.OutputSignals.Length);
            _busSet = new BusSet(entity.Buses.Length);

            _completelyConnected = _inputSignalSet.Count == 0 && _busSet.Count == 0;

            context.NotifyNewPortConstructed(this);

            for(int i = 0; i < _outputSignalSet.Count; i++)
            {
                _outputSignalSet[i] = new Signal(context);
                _outputSignalSet[i].DriveSignal(this, i);
            }
        }
        internal Port(Context context, Entity entity, IEnumerable<Signal> outputSignals)
        {
            _context = context;
            _iid = _context.GenerateInstanceId();
            _entity = entity;

            _inputSignalSet = new SignalSet(entity.InputSignals.Length);
            _outputSignalSet = new SignalSet(outputSignals);
            _busSet = new BusSet(entity.Buses.Length);

            System.Diagnostics.Debug.Assert(_outputSignalSet.Count == entity.OutputSignals.Length);

            context.NotifyNewPortConstructed(this);

            _completelyConnected = true;

            for(int i = 0; i < _outputSignalSet.Count; i++)
                if(_outputSignalSet[i] != null)
                    _outputSignalSet[i].DriveSignal(this, i);
                else
                    _completelyConnected = false;

            _completelyConnected &= _inputSignalSet.Count == 0 && _busSet.Count == 0;
            for(int i = 0; i < _outputSignalSet.Count && _completelyConnected; i++)
                _completelyConnected &= _outputSignalSet[i] != null;
        }

        /// <summary>
        /// Unique identifier of this port (and class instance).
        /// </summary>
        public Guid InstanceId
        {
            get { return _iid; }
        }

        /// <summary>
        /// The context in which this port is defined and used.
        /// </summary>
        public Context Context
        {
            get { return _context; }
        }

        /// <summary>
        /// The entity defining this port's interface and (indirectly) its operation.
        /// </summary>
        public Entity Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// The architecture currently attached to this port. Architectures are
        /// interchangeable as long as they implement this port's entity.
        /// </summary>
        public Architecture CurrentArchitecture
        {
            get { return _currentArchitecture; }
        }

        #region Signal Access
        public Signal this[int outputIndex]
        {
            get { return _outputSignalSet[outputIndex]; }
        }

        public int InputSignalCount
        {
            get { return _inputSignalSet.Count; }
        }
        public int OutputSignalCount
        {
            get { return _outputSignalSet.Count; }
        }
        public int BusCount
        {
            get { return _busSet.Count; }
        }

        public ReadOnlySignalSet InputSignals
        {
            get { return _inputSignalSet.AsReadOnly; }
        }
        public ReadOnlySignalSet OutputSignals
        {
            get { return _outputSignalSet.AsReadOnly; }
        }
        public ReadOnlyBusSet Buses
        {
            get { return _busSet.AsReadOnly; }
        }

        public int IndexOfOutputSignal(Signal signal)
        {
            for(int i = 0; i < _outputSignalSet.Count; i++)
                if(_outputSignalSet[i] == signal)
                    return i;
            return -1;
        }
        #endregion

        #region Connected Signals Management
        public bool IsCompletelyConnected
        {
            get { return _completelyConnected; }
        }

        private bool UpdateIsCompletelyConnected()
        {
            _completelyConnected = true;
            for(int i = 0; i < _inputSignalSet.Count && _completelyConnected; i++)
                _completelyConnected &= _inputSignalSet[i] != null;
            for(int i = 0; i < _outputSignalSet.Count && _completelyConnected; i++)
                _completelyConnected &= _outputSignalSet[i] != null;
            for(int i = 0; i < _busSet.Count && _completelyConnected; i++)
                _completelyConnected &= _busSet[i] != null;
            return _completelyConnected;
        }

        public void AddInputSignalBinding(int index, Signal signal)
        {
            if(_inputSignalSet[index] == null)
            {
                _inputSignalSet[index] = signal;
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        _inputSignalSet[index].AddCycles(_outputSignalSet[i], _context.GenerateTag());
                if(UpdateIsCompletelyConnected())
                    LookupAndLinkNewArchitecture();
            }
            else
            {
                _inputSignalSet[index] = signal;
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        _inputSignalSet[index].AddCycles(_outputSignalSet[i], _context.GenerateTag());
                if(_completelyConnected && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                    LookupAndLinkNewArchitecture();
            }
            _context.NotifySignalDrivesPort(signal, this, index);
        }
        public void RemoveInputSignalBinding(int index)
        {
            Signal signal = _inputSignalSet[index];
            _inputSignalSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();
            if(signal != null)
            {
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        signal.RemoveCycles(_outputSignalSet[i], _context.GenerateTag());
                _context.NotifySignalNoLongerDrivesPort(signal, this, index);
            }
        }
        public void ReplaceInputSignalBinding(int index, Signal replacement)
        {
            // TODO: could be optimized...
            RemoveInputSignalBinding(index);
            AddInputSignalBinding(index, replacement);
        }

        public void AddOutputSignalBinding(int index, Signal signal)
        {
            if(signal == null) throw new ArgumentNullException("signal");
            if(_outputSignalSet[index] == null)
            {
                _outputSignalSet[index] = signal;
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        _inputSignalSet[i].AddCycles(_outputSignalSet[index], _context.GenerateTag());
                if(UpdateIsCompletelyConnected())
                    LookupAndLinkNewArchitecture();
            }
            else
            {
                _outputSignalSet[index] = signal;
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        _inputSignalSet[i].AddCycles(_outputSignalSet[index], _context.GenerateTag());
                if(_completelyConnected && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                    LookupAndLinkNewArchitecture();
            }
            signal.DriveSignal(this, index);
        }
        public void RemoveOutputSignalBinding(int index)
        {
            Signal signal = _outputSignalSet[index];
            _outputSignalSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();
            if(signal != null)
            {
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        _inputSignalSet[i].RemoveCycles(signal, _context.GenerateTag());
                signal.UndriveSignal(index);
            }
        }
        public void ReplaceOutputSignalBinding(int index, Signal replacement)
        {
            // TODO: could be optimized...
            RemoveOutputSignalBinding(index);
            AddOutputSignalBinding(index, replacement);
        }

        public void AddBusBinding(int index, Bus bus)
        {
            if(_busSet[index] == null)
            {
                _busSet[index] = bus;
                if(UpdateIsCompletelyConnected())
                    LookupAndLinkNewArchitecture();
            }
            else
            {
                _busSet[index] = bus;
                if(_completelyConnected && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                    LookupAndLinkNewArchitecture();
            }
            _context.NotifyBusAttachedToPort(bus, this, index);
        }
        public void RemoveBusBinding(int index)
        {
            Bus bus = _busSet[index];
            _busSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();
            if(bus != null)
                _context.NotifyBusNoLongerAttachedToPort(bus, this, index);
        }
        public void ReplaceBusBinding(int index, Bus replacement)
        {
            // TODO: could be optimized...
            RemoveBusBinding(index);
            AddBusBinding(index, replacement);
        }

        public void RemoveAllBindings()
        {
            for(int i = 0; i < _inputSignalSet.Count; i++)
                if(_inputSignalSet[i] != null)
                    RemoveInputSignalBinding(i);
            for(int i = 0; i < _outputSignalSet.Count; i++)
                if(_outputSignalSet[i] != null)
                    RemoveOutputSignalBinding(i);
            for(int i = 0; i < _busSet.Count; i++)
                if(_busSet[i] != null)
                    RemoveBusBinding(i);
        }
        #endregion

        #region Signal Binding

        public void BindInputSignals(IEnumerable<Signal> inputSignals)
        {
            for(int i = 0; i < _inputSignalSet.Count; i++)
                if(_inputSignalSet[i] != null)
                {
                    _context.NotifySignalNoLongerDrivesPort(_inputSignalSet[i], this, i);

                    _inputSignalSet[i].SignalValueChanged -= Port_SignalValueChanged;
                    for(int j = 0; j < _outputSignalSet.Count; j++)
                        _inputSignalSet[i].RemoveCycles(_outputSignalSet[j], _context.GenerateTag());
                }

            _inputSignalSet.ReplaceRange(inputSignals);

            for(int i = 0; i < _inputSignalSet.Count; i++)
                if(_inputSignalSet[i] != null)
                {
                    _inputSignalSet[i].SignalValueChanged += Port_SignalValueChanged;
                    for(int j = 0; j < _outputSignalSet.Count; j++)
                        _inputSignalSet[i].AddCycles(_outputSignalSet[j], _context.GenerateTag());

                    _context.NotifySignalDrivesPort(_inputSignalSet[i], this, i);
                }

            if(UpdateIsCompletelyConnected() && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                LookupAndLinkNewArchitecture();
        }

        public void BindBuses(IEnumerable<Bus> buses)
        {
            for(int i = 0; i < _busSet.Count; i++)
                _context.NotifyBusNoLongerAttachedToPort(_busSet[i], this, i);

            _busSet.ReplaceRange(buses);

            for(int i = 0; i < _busSet.Count; i++)
                _context.NotifyBusAttachedToPort(_busSet[i], this, i);

            if(UpdateIsCompletelyConnected() && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                LookupAndLinkNewArchitecture();
        }

        private void Port_SignalValueChanged(object sender, EventArgs e)
        {
            EnsureArchitectureLink();
        }
        #endregion

        #region Architecture Link

        public bool HasArchitectureLink
        {
            get { return _currentArchitecture != null; }
        }

        /// <summary>
        /// Checks if the current architecture still matches the bound signals
        /// and tries to find a matching architecture if not.
        /// </summary>
        /// <returns>True if there's a matching architecture linked after the call.</returns>
        public bool EnsureArchitectureLink()
        {
            bool ok = false;

            if(_currentArchitecture != null && _currentArchitecture.SupportsPort(this))
                ok = true;
            else if(_completelyConnected && _context.Library.ContainsArchitecture(this))
            {
                if(_currentArchitecture != null)
                {
                    _currentArchitecture.UnregisterArchitecture();
                    _currentArchitecture = null;
                }
                _currentArchitecture = _context.Library.LookupArchitecture(this).InstantiateToPort(this);
                ok = true;
            }

            return ok && _currentArchitecture.IsInstance;
        }

        private bool LookupAndLinkNewArchitecture()
        {
            if(_context.Library.ContainsArchitecture(this))
            {
                RemoveLinkedArchitecture();
                _currentArchitecture = _context.Library.LookupArchitecture(this).InstantiateToPort(this);
                return true;
            }
            else
                return false;
        }

        public void RemoveLinkedArchitecture()
        {
            if(_currentArchitecture != null)
            {
                _currentArchitecture.UnregisterArchitecture();
                _currentArchitecture = null;
            }
        }
        #endregion

        #region Dependency Analysis
        public bool DependsOn(Signal signal)
        {
            return Scanner.ExistsSignal(this, signal.Equals, true);
        }
        public bool DependsOn(Port port)
        {
            return Scanner.ExistsPort(this, port.Equals, true);
        }
        public bool DependsOn(MathIdentifier portEntity)
        {
            return Scanner.ExistsPort(this, delegate(Port p) { return p.Entity.EntityId.Equals(portEntity); }, true);
        }
        #endregion

        #region Walk System [To Be Replaced]

        // TODO: Replace with the new Traversing System (will be much easier to use)

        /// <summary>Set the tag.</summary>
        /// <returns>True if it was already tagged with this tag.</returns>
        internal bool TagWasTagged(int tag)
        {
            if(_tag == tag)
                return true;
            _tag = tag;
            return false;
        }
        /// <summary>Remove the tag.</summary>
        internal void DeTag(int tag)
        {
            if(_tag == tag)
                _tag++;
        }
        #endregion

        public Port CloneWithNewInputs(IList<Signal> newInputs)
        {
            return _entity.InstantiatePort(_context, newInputs);
        }

        #region System Builder
        internal Guid AcceptSystemBuilder(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            return builder.BuildPort(_entity.EntityId,
                MapSignals(_inputSignalSet, signalMappings),
                MapSignals(_outputSignalSet, signalMappings),
                MapBuses(_busSet, busMappings));
        }
        private InstanceIdSet MapSignals(SignalSet signals, Dictionary<Guid, Guid> signalMappings)
        {
            return signals.ConvertAllToInstanceIds(delegate(Signal s) { return signalMappings[s.InstanceId]; });
        }
        private InstanceIdSet MapBuses(BusSet buses, Dictionary<Guid, Guid> busMappings)
        {
            return buses.ConvertAllToInstanceIds(delegate(Bus b) { return busMappings[b.InstanceId]; });
        }
        #endregion

        #region Instance Equality
        /// <remarks>Two ports are equal only if they are the same instance.</remarks>
        public bool Equals(Port other)
        {
            return other != null && _iid.Equals(other._iid);
        }
        /// <remarks>Two ports are equal only if they are the same instance.</remarks>
        public override bool Equals(object obj)
        {
            Port other = obj as Port;
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
            if(_currentArchitecture == null)
                return _entity.ToString();
            else
                return _currentArchitecture.ToString();
        }
    }
}