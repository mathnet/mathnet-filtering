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

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Traversing;
using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Events;
using MathNet.Symbolics.AutoEvaluation;

namespace MathNet.Symbolics.Core
{
    /// <summary>
    /// Represents an Yttrium Port. Ports connect signals by operations defined
    /// in interchangeable architectures. 
    /// </summary>
    public class Port : MathNet.Symbolics.Port, IPort_BuilderAdapter, IPort_CycleAnalysis
    {
        private SignalSet _inputSignalSet;
        private SignalSet _outputSignalSet;
        private BusSet _busSet;

        private readonly IEntity _entity;
        private IArchitecture _currentArchitecture;
        private int _tag;
        private bool _completelyConnected;

        internal Port(IEntity entity)
        {
            _entity = entity;

            _inputSignalSet = new SignalSet(entity.InputSignals.Length);
            _outputSignalSet = new SignalSet(entity.OutputSignals.Length);
            _busSet = new BusSet(entity.Buses.Length);

            _completelyConnected = _inputSignalSet.Count == 0 && _busSet.Count == 0;

            Service<IMediator>.Instance.NotifyNewPortCreated(this);

            for(int i = 0; i < _outputSignalSet.Count; i++)
            {
                _outputSignalSet[i] = new Signal();
                ((ISignal_Drive)_outputSignalSet[i]).DriveSignal(this, i);
            }
        }
        internal Port(IEntity entity, IEnumerable<MathNet.Symbolics.Signal> outputSignals)
        {
            _entity = entity;

            _inputSignalSet = new SignalSet(entity.InputSignals.Length);
            _outputSignalSet = new SignalSet(outputSignals);
            _busSet = new BusSet(entity.Buses.Length);

            System.Diagnostics.Debug.Assert(_outputSignalSet.Count == entity.OutputSignals.Length);

            Service<IMediator>.Instance.NotifyNewPortCreated(this);

            _completelyConnected = true;

            for(int i = 0; i < _outputSignalSet.Count; i++)
                if(_outputSignalSet[i] != null)
                    ((ISignal_Drive)_outputSignalSet[i]).DriveSignal(this, i);
                else
                    _completelyConnected = false;

            _completelyConnected &= _inputSignalSet.Count == 0 && _busSet.Count == 0;
            for(int i = 0; i < _outputSignalSet.Count && _completelyConnected; i++)
                _completelyConnected &= _outputSignalSet[i] != null;
        }

        /// <summary>
        /// The entity defining this port's interface and (indirectly) its operation.
        /// </summary>
        public override IEntity Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// The architecture currently attached to this port. Architectures are
        /// interchangeable as long as they implement this port's entity.
        /// </summary>
        public override IArchitecture CurrentArchitecture
        {
            get { return _currentArchitecture; }
        }

        #region Signal Access
        public override MathNet.Symbolics.Signal this[int outputIndex]
        {
            get { return _outputSignalSet[outputIndex]; }
        }

        public override int InputSignalCount
        {
            get { return _inputSignalSet.Count; }
        }
        public override int OutputSignalCount
        {
            get { return _outputSignalSet.Count; }
        }
        public override int BusCount
        {
            get { return _busSet.Count; }
        }

        public override ReadOnlySignalSet InputSignals
        {
            get { return _inputSignalSet.AsReadOnly; }
        }
        public override ReadOnlySignalSet OutputSignals
        {
            get { return _outputSignalSet.AsReadOnly; }
        }
        public override ReadOnlyBusSet Buses
        {
            get { return _busSet.AsReadOnly; }
        }

        public override int IndexOfOutputSignal(MathNet.Symbolics.Signal signal)
        {
            for(int i = 0; i < _outputSignalSet.Count; i++)
                if(_outputSignalSet[i] == signal)
                    return i;
            return -1;
        }
        #endregion

        #region Connected Signals Management
        public override bool IsCompletelyConnected
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

        public override void AddInputSignalBinding(int index, MathNet.Symbolics.Signal signal)
        {
            if(_inputSignalSet[index] == null)
            {
                _inputSignalSet[index] = signal;
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)_inputSignalSet[index]).AddCycles(_outputSignalSet[i], Config.Random.Next());
                if(UpdateIsCompletelyConnected())
                    LookupAndLinkNewArchitecture();
            }
            else
            {
                _inputSignalSet[index] = signal;
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)_inputSignalSet[index]).AddCycles(_outputSignalSet[i], Config.Random.Next());
                if(_completelyConnected && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                    LookupAndLinkNewArchitecture();
            }

            OnInputTreeChanged(index);
            Service<IMediator>.Instance.NotifySignalDrivesPort(signal, this, index);
        }
        public override void RemoveInputSignalBinding(int index)
        {
            MathNet.Symbolics.Signal signal = _inputSignalSet[index];
            _inputSignalSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();
            if(signal != null)
            {
                for(int i = 0; i < _outputSignalSet.Count; i++)
                    if(_outputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)signal).RemoveCycles(_outputSignalSet[i], Config.Random.Next());

                OnInputTreeChanged(index);
                Service<IMediator>.Instance.NotifySignalDrivesPortNoLonger(signal, this, index);
            }
        }
        public override void ReplaceInputSignalBinding(int index, MathNet.Symbolics.Signal replacement)
        {
            // TODO: could be optimized...
            RemoveInputSignalBinding(index);
            AddInputSignalBinding(index, replacement);
        }

        public override void AddOutputSignalBinding(int index, MathNet.Symbolics.Signal signal)
        {
            if(signal == null) throw new ArgumentNullException("signal");
            if(_outputSignalSet[index] == null)
            {
                _outputSignalSet[index] = signal;
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)_inputSignalSet[i]).AddCycles(_outputSignalSet[index], Config.Random.Next());
                if(UpdateIsCompletelyConnected())
                    LookupAndLinkNewArchitecture();
            }
            else
            {
                _outputSignalSet[index] = signal;
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)_inputSignalSet[i]).AddCycles(_outputSignalSet[index], Config.Random.Next());
                if(_completelyConnected && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                    LookupAndLinkNewArchitecture();
            }
            ((ISignal_Drive)signal).DriveSignal(this, index);
        }
        public override void RemoveOutputSignalBinding(int index)
        {
            MathNet.Symbolics.Signal signal = _outputSignalSet[index];
            _outputSignalSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();
            if(signal != null)
            {
                for(int i = 0; i < _inputSignalSet.Count; i++)
                    if(_inputSignalSet[i] != null)
                        ((ISignal_CycleAnalysis)_inputSignalSet[i]).RemoveCycles(signal, Config.Random.Next());
                ((ISignal_Drive)signal).UndriveSignal(index);
            }
        }
        public override void ReplaceOutputSignalBinding(int index, MathNet.Symbolics.Signal replacement)
        {
            // TODO: could be optimized...
            RemoveOutputSignalBinding(index);
            AddOutputSignalBinding(index, replacement);
        }

        public override void AddBusBinding(int index, MathNet.Symbolics.Bus bus)
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

            OnBusChanged(index);
            Service<IMediator>.Instance.NotifyBusAttachedToPort(bus, this, index);
        }
        public override void RemoveBusBinding(int index)
        {
            MathNet.Symbolics.Bus bus = _busSet[index];
            _busSet[index] = null;
            _completelyConnected = false;
            RemoveLinkedArchitecture();

            if(bus != null)
            {
                OnBusChanged(index);
                Service<IMediator>.Instance.NotifyBusDetachedFromPort(bus, this, index);
            }
        }
        public override void ReplaceBusBinding(int index, MathNet.Symbolics.Bus replacement)
        {
            // TODO: could be optimized...
            RemoveBusBinding(index);
            AddBusBinding(index, replacement);
        }

        public override void RemoveAllBindings()
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

        public override void BindInputSignals(IEnumerable<MathNet.Symbolics.Signal> inputSignals)
        {
            for(int i = 0; i < _inputSignalSet.Count; i++)
                if(_inputSignalSet[i] != null)
                {
                    Service<IMediator>.Instance.NotifySignalDrivesPortNoLonger(_inputSignalSet[i], this, i);

                    _inputSignalSet[i].ValueChanged -= Port_SignalValueChanged;
                    for(int j = 0; j < _outputSignalSet.Count; j++)
                        ((ISignal_CycleAnalysis)_inputSignalSet[i]).RemoveCycles(_outputSignalSet[j], Config.Random.Next());
                }

            _inputSignalSet.ReplaceRange(inputSignals);

            for(int i = 0; i < _inputSignalSet.Count; i++)
            {
                OnInputTreeChanged(i);
                if(_inputSignalSet[i] != null)
                {
                    _inputSignalSet[i].ValueChanged += Port_SignalValueChanged;
                    for(int j = 0; j < _outputSignalSet.Count; j++)
                        ((ISignal_CycleAnalysis)_inputSignalSet[i]).AddCycles(_outputSignalSet[j], Config.Random.Next());

                    Service<IMediator>.Instance.NotifySignalDrivesPort(_inputSignalSet[i], this, i);
                }
            }

            if(UpdateIsCompletelyConnected() && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                LookupAndLinkNewArchitecture();
        }

        public override void BindBuses(IEnumerable<MathNet.Symbolics.Bus> buses)
        {
            for(int i = 0; i < _busSet.Count; i++)
                Service<IMediator>.Instance.NotifyBusDetachedFromPort(_busSet[i], this, i);

            _busSet.ReplaceRange(buses);

            for(int i = 0; i < _busSet.Count; i++)
            {
                OnBusChanged(i);
                Service<IMediator>.Instance.NotifyBusAttachedToPort(_busSet[i], this, i);
            }

            if(UpdateIsCompletelyConnected() && _currentArchitecture != null && !_currentArchitecture.RebindToPortIfSupported(this))
                LookupAndLinkNewArchitecture();
        }

        private void Port_SignalValueChanged(object sender, ValueNodeEventArgs e)
        {
            EnsureArchitectureLink();
        }
        #endregion

        #region Architecture Link

        public override bool HasArchitectureLink
        {
            get { return _currentArchitecture != null; }
        }

        /// <summary>
        /// Checks if the current architecture still matches the bound signals
        /// and tries to find a matching architecture if not.
        /// </summary>
        /// <returns>True if there's a matching architecture linked after the call.</returns>
        public override bool EnsureArchitectureLink()
        {
            bool ok = false;
            ILibrary lib = Service<ILibrary>.Instance;

            if(_currentArchitecture != null && _currentArchitecture.SupportsPort(this))
                ok = true;
            else if(_completelyConnected && lib.ContainsArchitecture(this))
            {
                IArchitecture oldArchitecture = _currentArchitecture;
                if(_currentArchitecture != null)
                {
                    _currentArchitecture.UnregisterArchitecture();
                    _currentArchitecture = null;
                }
                _currentArchitecture = lib.LookupArchitecture(this).InstantiateToPort(this);
                OnArchitectureChanged(oldArchitecture, _currentArchitecture);
                ok = true;
            }

            return ok && _currentArchitecture.IsInstance;
        }

        private bool LookupAndLinkNewArchitecture()
        {
            ILibrary lib = Service<ILibrary>.Instance;

            if(lib.ContainsArchitecture(this))
            {
                IArchitecture oldArchitecture = _currentArchitecture;
                RemoveLinkedArchitecture();
                _currentArchitecture = lib.LookupArchitecture(this).InstantiateToPort(this);
                OnArchitectureChanged(oldArchitecture, _currentArchitecture);
                return true;
            }
            else
                return false;
        }

        public override void RemoveLinkedArchitecture()
        {
            if(_currentArchitecture != null)
            {
                IArchitecture oldArchitecture = _currentArchitecture;
                _currentArchitecture.UnregisterArchitecture();
                _currentArchitecture = null;
                OnArchitectureChanged(oldArchitecture, null);
            }
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

        #region Dependency Analysis
        public override bool DependsOn(MathNet.Symbolics.Signal signal)
        {
            return Service<IScanner>.Instance.ExistsSignal(this, signal.Equals, true);
        }
        public override bool DependsOn(MathNet.Symbolics.Port port)
        {
            return Service<IScanner>.Instance.ExistsPort(this, port.Equals, true);
        }
        public override bool DependsOn(MathIdentifier portEntity)
        {
            return Service<IScanner>.Instance.ExistsPort(this, delegate(MathNet.Symbolics.Port p) { return p.Entity.EntityId.Equals(portEntity); }, true);
        }
        #endregion

        #region Walk System [To Be Replaced]

        // TODO: Replace with the new Traversing System (will be much easier to use)

        /// <summary>Set the tag.</summary>
        /// <returns>True if it was already tagged with this tag.</returns>
        bool IPort_CycleAnalysis.TagWasTagged(int tag)
        {
            if(_tag == tag)
                return true;
            _tag = tag;
            return false;
        }
        /// <summary>Remove the tag.</summary>
        void IPort_CycleAnalysis.DeTag(int tag)
        {
            if(_tag == tag)
                _tag++;
        }
        #endregion

        public override MathNet.Symbolics.Port CloneWithNewInputs(IList<MathNet.Symbolics.Signal> newInputs)
        {
            return _entity.InstantiatePort(newInputs);
        }

        #region System Builder
        Guid IPort_BuilderAdapter.AcceptSystemBuilder(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            return builder.BuildPort(_entity.EntityId,
                BuilderMapSignals(_inputSignalSet, signalMappings),
                BuilderMapSignals(_outputSignalSet, signalMappings),
                BuilderMapBuses(_busSet, busMappings));
        }
        private InstanceIdSet BuilderMapSignals(SignalSet signals, Dictionary<Guid, Guid> signalMappings)
        {
            return signals.ConvertAllToInstanceIds(delegate(MathNet.Symbolics.Signal s) { return signalMappings[s.InstanceId]; });
        }
        private InstanceIdSet BuilderMapBuses(BusSet buses, Dictionary<Guid, Guid> busMappings)
        {
            return buses.ConvertAllToInstanceIds(delegate(MathNet.Symbolics.Bus b) { return busMappings[b.InstanceId]; });
        }
        #endregion

        #region Instance Equality
        /// <remarks>Two ports are equal only if they are the same instance.</remarks>
        public override bool Equals(object obj)
        {
            Port other = obj as Port;
            if(other == null)
                return false;
            else
                return InstanceId.Equals(other.InstanceId);
        }
        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
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