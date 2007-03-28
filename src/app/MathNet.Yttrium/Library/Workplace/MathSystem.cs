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
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Channels;
using MathNet.Symbolics.Traversing;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Simulation;
using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Events;
using MathNet.Symbolics.SystemBuilder.Toolkit;

namespace MathNet.Symbolics.Workplace
{
    public class MathSystem : IMathSystem, ISystemMediatorSource
    {
        private readonly Guid _iid;

        private readonly SignalSet _inputs;
        private readonly SignalSet _outputs;

        private readonly SignalSet _allSignals;
        private readonly BusSet _allBuses;
        private readonly PortSet _allPorts;

        private Dictionary<string, Signal> _namedSignals;
        private Dictionary<string, Bus> _namedBuses;

        private ISystemMediator _systemMediator;

        public event EventHandler<IndexedSignalEventArgs> OutputValueChanged;

        public MathSystem()
        {
            _iid = Config.GenerateInstanceId();

            _inputs = new SignalSet();
            _outputs = new SignalSet();
            _allSignals = new SignalSet();
            _allBuses = new BusSet();
            _allPorts = new PortSet();
        }

        #region Properties
        public bool HasSystemMediator
        {
            get { return _systemMediator != null; }
        }
        public ISystemMediator SystemMediator
        {
            get { return _systemMediator; }
            set
            {
                if(_systemMediator != null)
                    _systemMediator.UnsubscribeSystem(this);
                _systemMediator = value;
                if(value != null)
                    _systemMediator.SubscribeSystem(this);
            }
        }

        public Guid InstanceId
        {
            get { return _iid; }
        }
        #endregion

        #region Readonly Direct Access
        #region Count
        public int SignalCount
        {
            get { return _allSignals.Count; }
        }
        public int BusCount
        {
            get { return _allBuses.Count; }
        }
        public int PortCount
        {
            get { return _allPorts.Count; }
        }
        public int InputCount
        {
            get { return _inputs.Count; }
        }
        public int OutputCount
        {
            get { return _outputs.Count; }
        }
        #endregion
        #region Contains
        public bool ContainsSignal(Signal signal)
        {
            return _allSignals.Contains(signal);
        }
        public bool ContainsBus(Bus bus)
        {
            return _allBuses.Contains(bus);
        }
        public bool ContainsPort(Port port)
        {
            return _allPorts.Contains(port);
        }
        #endregion
        #region Get By Index
        public Signal GetSignal(int index)
        {
            return _allSignals[index];
        }
        public Bus GetBus(int index)
        {
            return _allBuses[index];
        }
        public Port GetPort(int index)
        {
            return _allPorts[index];
        }
        public Signal GetInput(int index)
        {
            return _inputs[index];
        }
        public Signal GetOutput(int index)
        {
            return _outputs[index];
        }
        #endregion
        #region GetAll Readonly Sets
        public ReadOnlySignalSet GetAllSignals()
        {
            return _allSignals.AsReadOnly;
        }
        public ReadOnlyBusSet GetAllBuses()
        {
            return _allBuses.AsReadOnly;
        }
        public ReadOnlyPortSet GetAllPorts()
        {
            return _allPorts.AsReadOnly;
        }
        public ReadOnlySignalSet GetAllInputs()
        {
            return _inputs.AsReadOnly;
        }
        public ReadOnlySignalSet GetAllOutputs()
        {
            return _outputs.AsReadOnly;
        }
        #endregion
        public ReadOnlySignalSet GetAllLeafSignals()
        {
            return new ReadOnlySignalSet(_allSignals.FindAll(delegate(Signal signal) { return signal.BehavesAsSourceSignal; })); // && !inputSignals.Exists(delegate(Signal s) { return signal.DependsOn(s); }); }).AsReadOnly();
        }
        #endregion

        #region Private Write Direct Access
        private void InternalAddSignal(Signal signal)
        {
            int idx = _allSignals.Count;
            _allSignals.Add(signal);
            if(_systemMediator != null)
                _systemMediator.NotifySignalAdded(signal, idx);
        }
        private void InternalRemoveSignal(Signal signal)
        {
            if(_inputs.Contains(signal))
                InternalRemoveInput(signal);
            if(_outputs.Contains(signal))
                InternalRemoveOutput(signal);
            int idx = _allSignals.IndexOf(signal);
            _allSignals.RemoveAt(idx);
            if(_systemMediator != null)
            {
                _systemMediator.NotifySignalRemoved(signal, idx);
                for(int i = idx; i < _allSignals.Count; i++)
                    _systemMediator.NotifySignalMoved(_allSignals[i], idx + 1, idx);
            }
        }
        private void InternalAddBus(Bus bus)
        {
            int idx = _allBuses.Count;
            _allBuses.Add(bus);
            if(_systemMediator != null)
                _systemMediator.NotifyBusAdded(bus, idx);
        }
        private void InternalRemoveBus(Bus bus)
        {
            int idx = _allBuses.IndexOf(bus);
            _allBuses.RemoveAt(idx);
            if(_systemMediator != null)
            {
                _systemMediator.NotifyBusRemoved(bus, idx);
                for(int i = idx; i < _allBuses.Count; i++)
                    _systemMediator.NotifyBusMoved(_allBuses[i], idx + 1, idx);
            }
        }
        private void InternalAddPort(Port port)
        {
            int idx = _allPorts.Count;
            _allPorts.Add(port);
            if(_systemMediator != null)
            {
                _systemMediator.NotifyPortAdded(port, idx);
                idx = 0;
                foreach(Signal s in port.InputSignals)
                    if(_allSignals.Contains(s))
                        _systemMediator.NotifySignalDrivesPort(s, port, idx++);
                idx = 0;
                foreach(Signal s in port.OutputSignals)
                    if(_allSignals.Contains(s))
                        _systemMediator.NotifyPortDrivesSignal(s, port, idx++);
                idx = 0;
                foreach(Bus b in port.Buses)
                    if(_allBuses.Contains(b))
                        _systemMediator.NotifyBusAttachedToPort(b, port, idx++);
            }
        }
        private void InternalRemovePort(Port port)
        {
            int idx = _allPorts.IndexOf(port);
            _allPorts.RemoveAt(idx);
            if(_systemMediator != null)
            {
                _systemMediator.NotifyPortRemoved(port, idx);
                for(int i = idx; i < _allPorts.Count; i++)
                    _systemMediator.NotifyPortMoved(_allPorts[i], idx + 1, idx);
            }
        }
        private void InternalAddInput(Signal signal)
        {
            int idx = _inputs.Count;
            _inputs.Add(signal);
            if(_systemMediator != null)
                _systemMediator.NotifyInputAdded(signal, idx);
        }
        private void InternalRemoveInput(Signal signal)
        {
            int idx = _inputs.IndexOf(signal);
            _inputs.RemoveAt(idx);
            if(_systemMediator != null)
            {
                _systemMediator.NotifyInputRemoved(signal, idx);
                for(int i = idx; i < _inputs.Count; i++)
                    _systemMediator.NotifyInputMoved(_inputs[i], idx + 1, idx);
            }
        }
        private void InternalAddOutput(Signal signal)
        {
            int idx = _outputs.Count;
            _outputs.Add(signal);
            if(_systemMediator != null)
                _systemMediator.NotifyOutputAdded(signal, idx);
        }
        private void InternalRemoveOutput(Signal signal)
        {
            int idx = _outputs.IndexOf(signal);
            _outputs.RemoveAt(idx);
            if(_systemMediator != null)
            {
                _systemMediator.NotifyOutputRemoved(signal, idx);
                for(int i = idx; i < _outputs.Count; i++)
                    _systemMediator.NotifyOutputMoved(_outputs[i], idx + 1, idx);
            }
        }
        #endregion

        #region System Components
        public void AddSignal(Signal signal)
        {
            if(!_allSignals.Contains(signal))
                InternalAddSignal(signal);
        }
        public void AddSignalRange(IEnumerable<Signal> signals)
        {
            foreach(Signal signal in signals)
                if(!_allSignals.Contains(signal))
                    InternalAddSignal(signal);
        }
        public void RemoveSignal(Signal signal, bool isolateFromDriver)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(isolateFromDriver && signal.IsDriven)
                signal.DrivenByPort.RemoveOutputSignalBinding(signal.DrivenByPort.OutputSignals.IndexOf(signal));
            if(_allSignals.Contains(signal))
                InternalRemoveSignal(signal);
        }
        public void AddBus(Bus bus)
        {
            if(!_allBuses.Contains(bus))
                InternalAddBus(bus);
        }
        public void AddBusRange(IEnumerable<Bus> buses)
        {
            if(buses == null)
                throw new ArgumentNullException("buses");

            foreach(Bus bus in buses)
                if(!_allBuses.Contains(bus))
                    InternalAddBus(bus);
        }
        public void RemoveBus(Bus bus)
        {
            if(_allBuses.Contains(bus))
                InternalRemoveBus(bus);
        }
        public void AddPort(Port port)
        {
            if(!_allPorts.Contains(port))
                InternalAddPort(port);
        }
        public void RemovePort(Port port, bool isolate)
        {
            if(port == null)
                throw new ArgumentNullException("port");

            if(isolate)
                port.RemoveAllBindings();
            if(_allPorts.Contains(port))
                InternalRemovePort(port);
        }
        public void AddSignalTree(Signal signal, bool setAsOutput, bool autoAddInputs)
        {
            if(signal == null)
                return;

            AddSignal(signal);
            if(setAsOutput)
                PromoteAsOutput(signal);

            Service<IScanner>.Instance.ForEachSignal(signal, delegate(Signal s)
            {
                if(!_allSignals.Contains(s))
                {
                    InternalAddSignal(s);
                    if(autoAddInputs && !s.IsDriven && !Std.IsConstant(s))
                        InternalAddInput(s);
                }
                return true;
            }, true);

            foreach(Signal s in _allSignals)
            {
                if(s.BehavesAsBeingDriven(false))
                {
                    Port p = s.DrivenByPort;
                    if(!_allPorts.Contains(p))
                    {
                        AddBusRange(p.Buses);
                        InternalAddPort(p);
                    }
                }
            }
        }
        public void AddSignalTree(Signal signal, ICollection<Signal> border, bool setAsOutput, bool autoAddInputs)
        {
            if(signal == null)
                return;

            AddSignal(signal);
            if(setAsOutput)
                PromoteAsOutput(signal);

            Service<IScanner>.Instance.ForEachSignal(signal, delegate(Signal s)
            {
                if(!_allSignals.Contains(s))
                {
                    InternalAddSignal(s);
                    if(autoAddInputs && !s.IsDriven && !Std.IsConstant(s))
                        InternalAddInput(s);
                }
                if(border.Contains(s))
                {
                    PromoteAsInput(s);
                    return false;
                }
                return true;
            }, true);

            foreach(Signal s in _allSignals)
            {
                if(s.BehavesAsBeingDriven(false))
                {
                    Port p = s.DrivenByPort;
                    if(!_allPorts.Contains(p))
                    {
                        AddBusRange(p.Buses);
                        InternalAddPort(p);
                    }
                }
            }
        }
        public void AddSignalTreeRange(IEnumerable<Signal> signals, bool setAsOutput, bool autoAddInputs)
        {
            foreach(Signal signal in signals)
                AddSignalTree(signal, setAsOutput, autoAddInputs);
        }
        public void AddSignalTreeRange(IEnumerable<Signal> signals, ICollection<Signal> border, bool setAsOutput, bool autoAddInputs)
        {
            foreach(Signal signal in signals)
                AddSignalTree(signal, border, setAsOutput, autoAddInputs);
        }
        
        public void AddPortTree(Port port, bool setAsOutput, bool autoAddInputs)
        {
            AddSignalTreeRange(port.OutputSignals, setAsOutput, autoAddInputs);
        }

        /// <returns>the input index of the input signal</returns>
        public int PromoteAsInput(Signal signal)
        {
            AddSignal(signal);
            if(!_inputs.Contains(signal))
                InternalAddInput(signal);
            return _inputs.LastIndexOf(signal);
        }
        public void UnpromoteAsInput(Signal signal)
        {
            if(_inputs.Contains(signal))
                InternalRemoveInput(signal);
        }

        /// <returns>the output index of the output signal</returns>
        public int PromoteAsOutput(Signal signal)
        {
            if(!_outputs.Contains(signal))
            {
                AddSignal(signal);
                InternalAddOutput(signal);
                RegisterOutputSignal(signal);
            }
            return _outputs.LastIndexOf(signal);
        }
        public void UnpromoteAsOutput(Signal signal)
        {
            if(_outputs.Contains(signal))
            {
                UnregisterOutputSignal(signal);
                InternalRemoveOutput(signal);
            }
        }
        #endregion

        #region Named Signals
        public Signal CreateNamedSignal(string name)
        {
            Signal s = Binder.CreateSignal();
            AddNamedSignal(name, s);
            return s;
        }
        public Signal AddNamedSignal(string name, Signal signal)
        {
            if(_namedSignals == null)
                _namedSignals = new Dictionary<string, Signal>();
            AddSignal(signal);
            signal.Label = name;
            _namedSignals.Add(name, signal);
            return signal;
        }
        public Signal AddNamedSignal(string name, IValueStructure value)
        {
            if(_namedSignals == null)
                _namedSignals = new Dictionary<string, Signal>();
            Signal signal = Binder.CreateSignal(value);
            AddSignal(signal);
            signal.Label = name;
            _namedSignals.Add(name, signal);
            return signal;
        }
        public Signal LookupNamedSignal(string name)
        {
            if(_namedSignals == null)
                throw new KeyNotFoundException();
            return _namedSignals[name];
        }
        public bool TryLookupNamedSignal(string name, out Signal signal)
        {
            if(_namedSignals == null)
            {
                signal = null;
                return false;
            }
            return _namedSignals.TryGetValue(name, out signal);
        }
        public bool ContainsNamedSignal(string name)
        {
            if(_namedSignals == null)
                return false;
            return _namedSignals.ContainsKey(name);
        }
        public void RemoveNamedSignal(string name)
        {
            if(_namedSignals == null)
                return;
            _namedSignals.Remove(name);
        }
        #endregion
        #region Named Buses
        public Bus CreateNamedBus(string name)
        {
            Bus b = Binder.CreateBus();
            AddNamedBus(name, b);
            return b;
        }
        public void AddNamedBus(string name, Bus bus)
        {
            if(_namedBuses == null)
                _namedBuses = new Dictionary<string, Bus>();
            AddBus(bus);
            bus.Label = name;
            _namedBuses.Add(name, bus);
        }
        public Bus LookupNamedBus(string name)
        {
            if(_namedBuses == null)
                throw new KeyNotFoundException();
            return _namedBuses[name];
        }
        public bool TryLookupNamedBus(string name, out Bus bus)
        {
            if(_namedBuses == null)
            {
                bus = null;
                return false;
            }
            return _namedBuses.TryGetValue(name, out bus);
        }
        public bool ContainsNamedBus(string name)
        {
            if(_namedBuses == null)
                return false;
            return _namedBuses.ContainsKey(name);
        }
        public void RemoveNamedBus(string name)
        {
            if(_namedBuses == null)
                return;
            _namedBuses.Remove(name);
        }
        #endregion

        #region Value Forwarding
        #region Input Signals
        public void PushInputValue(int inputIndex, IValueStructure value)
        {
            _inputs[inputIndex].PostNewValue(value);
        }
        public void PushInputValue(int inputIndex, IValueStructure value, TimeSpan delay)
        {
            _inputs[inputIndex].PostNewValue(value, delay);
        }
        public void PushInputValueRange(IEnumerable<IValueStructure> values)
        {
            int idx = 0;
            foreach(IValueStructure value in values)
                _inputs[idx++].PostNewValue(value);
        }
        public void PushInputValueRange(IEnumerable<IValueStructure> values, TimeSpan delay)
        {
            int idx = 0;
            foreach(IValueStructure value in values)
                _inputs[idx++].PostNewValue(value, delay);
        }
        public void PushInputValueRange(IEnumerable<Signal> signalsWithValues)
        {
            int idx = 0;
            foreach(Signal signal in signalsWithValues)
                _inputs[idx++].PostNewValue(signal.Value);
        }
        public void PushInputValueRange(IEnumerable<Signal> signalsWithValues, TimeSpan delay)
        {
            int idx = 0;
            foreach(Signal signal in signalsWithValues)
                _inputs[idx++].PostNewValue(signal.Value, delay);
        }
        #endregion
        #region Output Signals
        private void RegisterOutputSignal(Signal s)
        {
            s.ValueChanged += s_SignalValueChanged;
        }
        //private void RegisterOutputSignalRange(IEnumerable<Signal> signals)
        //{
        //    foreach(Signal s in signals)
        //        RegisterOutputSignal(s);
        //}

        private void UnregisterOutputSignal(Signal s)
        {
            s.ValueChanged -= s_SignalValueChanged;
        }
        //private void UnregisterOutputSignalRange(IEnumerable<Signal> signals)
        //{
        //    foreach(Signal s in signals)
        //        UnregisterOutputSignal(s);
        //}

        void s_SignalValueChanged(object sender, ValueNodeEventArgs e)
        {
            EventHandler<IndexedSignalEventArgs> handler = OutputValueChanged;
            if(handler != null && _outputs.Contains((Signal)e.ValueNode))
                handler(this, new IndexedSignalEventArgs((Signal)e.ValueNode, _outputs.IndexOf((Signal)e.ValueNode)));
        }
        #endregion
        #endregion

        #region Evaluation - Systems may be used as "Functions"
        public IValueStructure[] Evaluate(params IValueStructure[] inputs)
        {
            if(inputs == null)
                throw new ArgumentNullException("inputs");

            if(inputs.Length != _inputs.Count)
                throw new System.ArgumentException("The count of inputs doesn't match the systems count of input signals.", "inputs");

            for(int i = 0; i < inputs.Length; i++)
                _inputs[i].PostNewValue(inputs[i]);

            Service<ISimulationMediator>.Instance.SimulateInstant();

            IValueStructure[] outputs = new IValueStructure[_outputs.Count];
            for(int i = 0; i < outputs.Length; i++)
                outputs[i] = _outputs[i].Value;
            return outputs;
        }

        public double[] Evaluate(params double[] inputs)
        {
            if(inputs == null)
                throw new ArgumentNullException("inputs");

            if(inputs.Length != _inputs.Count)
                throw new System.ArgumentException("The count of inputs doesn't match the systems count of input signals.", "inputs");

            for(int i = 0; i < inputs.Length; i++)
                _inputs[i].PostNewValue(new RealValue(inputs[i]));

            Service<ISimulationMediator>.Instance.SimulateInstant();

            double[] outputs = new double[_outputs.Count];
            for(int i = 0; i < outputs.Length; i++)
                outputs[i] = RealValue.ConvertFrom(_outputs[i].Value).Value;
            return outputs;
        }
        #endregion

        #region System Builder
        public void AcceptSystemBuilder(ISystemBuilder builder)
        {
            Dictionary<Guid, Guid> signalMappings = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> busMappings = new Dictionary<Guid, Guid>();

            builder.BeginBuildSystem(_inputs.Count, _outputs.Count, _allBuses.Count);

            // Before
            foreach(Signal s in _allSignals)
                signalMappings.Add(s.InstanceId, ((ISignal_BuilderAdapter)s).AcceptSystemBuilderBefore(builder));
            foreach(Bus b in _allBuses)
                busMappings.Add(b.InstanceId, ((IBus_BuilderAdapter)b).AcceptSystemBuilderBefore(builder));
            foreach(Port p in _allPorts)
                ((IPort_BuilderAdapter)p).AcceptSystemBuilder(builder, signalMappings, busMappings);

            // After
            foreach(Signal s in _allSignals)
                ((ISignal_BuilderAdapter)s).AcceptSystemBuilderAfter(builder, signalMappings, busMappings);

            // System Interface
            foreach(Signal si in _inputs)
                builder.AppendSystemInputSignal(signalMappings[si.InstanceId]);
            foreach(Signal so in _outputs)
                builder.AppendSystemOutputSignal(signalMappings[so.InstanceId]);

            // Named Signals
            if(_namedBuses != null)
                foreach(KeyValuePair<string, Signal> ns in _namedSignals)
                    builder.AppendSystemNamedSignal(signalMappings[ns.Value.InstanceId], ns.Key);
            if(_namedBuses != null)
                foreach(KeyValuePair<string, Bus> nb in _namedBuses)
                    builder.AppendSystemNamedBus(busMappings[nb.Value.InstanceId], nb.Key);

            builder.EndBuildSystem();
        }
        #endregion

        #region System Builder Templates: Cloning, Xml IO, Serialization
        #region Clone
        public IMathSystem Clone()
        {
            SystemWriter writer = new SystemWriter();
            SystemReader reader = new SystemReader(writer);
            reader.ReadSystem(this);
            return writer.WrittenSystems.Dequeue();
        }
        #endregion
        #region Write Xml
        public void WriteXml(XmlWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            XmlSystemWriter xwriter = new XmlSystemWriter(writer);
            SystemReader xreader = new SystemReader(xwriter);
            xreader.ReadSystem(this);
            writer.Flush();
        }
        public string WriteXml(bool xmlFragmentOnly)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = xmlFragmentOnly ? ConformanceLevel.Fragment : ConformanceLevel.Document;
            settings.OmitXmlDeclaration = xmlFragmentOnly;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.Encoding = Config.InternalEncoding;
            XmlWriter writer = XmlWriter.Create(sb, settings);
            WriteXml(writer);
            writer.Close();
            return sb.ToString();
        }
        public void WriteXml(FileInfo file)
        {
            if(file == null)
                throw new ArgumentNullException("file");

            XmlWriter writer = XmlWriter.Create(file.CreateText());
            WriteXml(writer);
            writer.Close();
        }
        #endregion
        #region Read Xml
        public static IMathSystem ReadXml(XmlReader reader)
        {
            SystemWriter xwriter = new SystemWriter();
            XmlSystemReader xreader = new XmlSystemReader(xwriter);
            xreader.ReadSystems(reader, false);
            return xwriter.WrittenSystems.Dequeue();
        }
        public static IMathSystem ReadXml(string xml)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXml(reader);
        }
        public static IMathSystem ReadXml(FileInfo file)
        {
            if(file == null)
                throw new ArgumentNullException("file");

            XmlReader reader = XmlReader.Create(file.OpenText());
            IMathSystem sys = ReadXml(reader);
            reader.Close();
            return sys;
        }
        #endregion
        #region Read Xml -> Build Entity
        public static IEntity ReadXmlEntity(XmlReader reader, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            if(reader == null)
                throw new ArgumentNullException("reader");

            reader.ReadToFollowing("System", Config.YttriumNamespace);
            int inputCnt = int.Parse(reader.GetAttribute("inputCount"), Config.InternalNumberFormat);
            int outputCnt = int.Parse(reader.GetAttribute("outputCount"), Config.InternalNumberFormat);
            int busCnt = int.Parse(reader.GetAttribute("busCount"), Config.InternalNumberFormat);
            reader.Close();

            string[] inputSignalLabels = new string[inputCnt];
            for(int i = 0; i < inputCnt; i++)
                inputSignalLabels[i] = "In_" + i.ToString(Config.InternalNumberFormat);

            string[] outputSignalLabels = new string[outputCnt];
            for(int i = 0; i < outputCnt; i++)
                outputSignalLabels[i] = "Out_" + i.ToString(Config.InternalNumberFormat);

            string[] busLabels = new string[busCnt];
            for(int i = 0; i < busCnt; i++)
                busLabels[i] = "Bus_" + i.ToString(Config.InternalNumberFormat);

            return new Core.Entity(symbol, entityId.Label, entityId.Domain, notation, precedence, inputSignalLabels, outputSignalLabels, busLabels);
        }
        public static IEntity ReadXmlEntity(XmlReader reader, MathIdentifier entityId, string symbol)
        {
            return ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
        }
        public static IEntity ReadXmlEntity(string xml, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXmlEntity(reader, entityId, symbol, notation, precedence);
        }
        public static IEntity ReadXmlEntity(string xml, MathIdentifier entityId, string symbol)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
        }
        public static IEntity ReadXmlEntity(FileInfo file, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            if(file == null)
                throw new ArgumentNullException("file");

            XmlReader reader = XmlReader.Create(file.OpenText());
            IEntity entity = ReadXmlEntity(reader, entityId, symbol, notation, precedence);
            reader.Close();
            return entity;
        }
        public static IEntity ReadXmlEntity(FileInfo file, MathIdentifier entityId, string symbol)
        {
            if(file == null)
                throw new ArgumentNullException("file");

            XmlReader reader = XmlReader.Create(file.OpenText());
            IEntity entity = ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
            reader.Close();
            return entity;
        }
        #endregion
        #endregion

        #region Compound Integration
        public IArchitectureFactory BuildArchitectureFactory(MathIdentifier architectureId, MathIdentifier entityId)
        {
            return new CompoundArchitectureFactory(architectureId, entityId, this);
        }
        public static IArchitectureFactory BuildArchitectureFactory(string xml, MathIdentifier architectureId, MathIdentifier entityId)
        {
            return new CompoundArchitectureFactory(architectureId, entityId, xml);
        }
        public static IArchitectureFactory BuildArchitectureFactory(string xml, MathIdentifier architectureId, MathIdentifier entityId, int inputCount, int outputCount, int busCount)
        {
            return new CompoundArchitectureFactory(architectureId, entityId, xml, inputCount, outputCount, busCount);
        }

        public IEntity BuildEntity(MathIdentifier entityId, string symbol)
        {
            return BuildEntity(entityId, symbol, InfixNotation.None, -1);
        }
        public IEntity BuildEntity(MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            string[] inputSignalLabels = new string[_inputs.Count];
            for(int i = 0; i < inputSignalLabels.Length; i++)
                inputSignalLabels[i] = string.IsNullOrEmpty(_inputs[i].Label) ? "In_" + i.ToString(Config.InternalNumberFormat) : _inputs[i].Label;

            string[] outputSignalLabels = new string[_outputs.Count];
            for(int i = 0; i < outputSignalLabels.Length; i++)
                outputSignalLabels[i] = string.IsNullOrEmpty(_outputs[i].Label) ? "Out_" + i.ToString(Config.InternalNumberFormat) : _outputs[i].Label;

            string[] busLabels = new string[_allBuses.Count];
            for(int i = 0; i < busLabels.Length; i++)
                busLabels[i] = string.IsNullOrEmpty(_allBuses[i].Label) ? "Bus_" + i.ToString(Config.InternalNumberFormat) : _allBuses[i].Label;

            return new Core.Entity(symbol, entityId.Label, entityId.Domain, notation, precedence, inputSignalLabels, outputSignalLabels, busLabels);
        }

        public void PublishToLibrary(MathIdentifier architectureId, MathIdentifier entityId)
        {
            IArchitectureFactory factory = BuildArchitectureFactory(architectureId, entityId);
            Service<ILibrary>.Instance.AddArchitecture(factory);
        }
        public IEntity PublishToLibrary(string label, string symbol)
        {
            return this.PublishToLibrary("Work", label + "Compound", label, symbol);
        }
        public IEntity PublishToLibrary(string domain, string architectureLabel, string entityLabel, string entitySymbol)
        {
            IEntity entity = BuildEntity(new MathIdentifier(entityLabel, domain), entitySymbol);
            IArchitectureFactory factory = BuildArchitectureFactory(new MathIdentifier(architectureLabel, domain), entity.EntityId);
            ILibrary lib = Service<ILibrary>.Instance;
            lib.AddEntity(entity);
            lib.AddArchitecture(factory);
            return entity;
        }
        public static void PublishToLibrary(string xml, ILibrary library, MathIdentifier architectureId, MathIdentifier entityId)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            IArchitectureFactory factory = BuildArchitectureFactory(xml, architectureId, entityId);
            library.AddArchitecture(factory);
        }
        public static IEntity PublishToLibrary(string xml, ILibrary library, string label, string symbol)
        {
            return PublishToLibrary(xml, library, "Work", label + "Compound", label, symbol);
        }
        public static IEntity PublishToLibrary(string xml, ILibrary library, string domain, string architectureLabel, string entityLabel, string entitySymbol)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            IEntity entity = ReadXmlEntity(xml, new MathIdentifier(entityLabel, domain), entitySymbol);
            IArchitectureFactory factory = BuildArchitectureFactory(xml, new MathIdentifier(architectureLabel, domain), entity.EntityId, entity.InputSignals.Length, entity.OutputSignals.Length, entity.Buses.Length);
            library.AddEntity(entity);
            library.AddArchitecture(factory);
            return entity;
        }

        public IEntity PublishToLibraryAnonymous()
        {
            return this.PublishToLibrary("Anonymous_" + Guid.NewGuid().ToString("N"), "anonymous");
        }
        public static IEntity PublishToLibraryAnonymous(string xml, ILibrary library)
        {
            return PublishToLibrary(xml, library, "Anonymous_" + Guid.NewGuid().ToString("N"), "anonymous");
        }
        #endregion

        #region Maintenance
        /// <returns>True if some objects where removed.</returns>
        public bool RemoveUnusedObjects()
        {
            IList<Signal> signals;
            IList<Port> ports;
            Service<IScanner>.Instance.FindAll(_outputs, false, out signals, out ports);
            bool ret = false;
            for(int i = _allPorts.Count - 1; i >= 0; i--)
            {
                Port p = _allPorts[i];
                if(!ports.Contains(p))
                {
                    p.RemoveAllBindings();
                    InternalRemovePort(p); //_allPorts.RemoveAt(i);
                    ret = true;
                }
            }
            for(int i = _allSignals.Count - 1; i >= 0; i--)
            {
                Signal s = _allSignals[i];
                if(!signals.Contains(s)) // && !s.IsDriven && !_inputs.Contains(s))
                {
                    InternalRemoveSignal(s); //_allSignals.RemoveAt(i);
                    ret = true;
                }
            }
            return ret;
        }
        #endregion
    }
}
