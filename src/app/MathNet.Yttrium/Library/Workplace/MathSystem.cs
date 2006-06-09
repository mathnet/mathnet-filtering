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
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Events;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.SystemBuilder;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Channels;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.Workplace
{
    public class MathSystem : IContextSensitive
    {
        private readonly Guid _iid;
        private readonly Context _context;

        private readonly SignalSet _inputs;
        private readonly SignalSet _outputs;

        private readonly SignalSet _allSignals;
        private readonly BusSet _allBuses;
        private readonly PortSet _allPorts;

        private Dictionary<string, Signal> _namedSignals;
        private Dictionary<string, Bus> _namedBuses;

        private Mediator _mediator;

        public event EventHandler<IndexedSignalEventArgs> OutputValueChanged;

        public MathSystem(Context context)
        {
            if(context == null) throw new ArgumentNullException("context");
            _context = context;
            _iid = _context.GenerateInstanceId();

            _inputs = new SignalSet();
            _outputs = new SignalSet();
            _allSignals = new SignalSet();
            _allBuses = new BusSet();
            _allPorts = new PortSet();
        }

        #region Properties
        public Context Context
        {
            get { return _context; }
        }

        public bool HasMediator
        {
            get { return _mediator != null; }
        }
        public Mediator Mediator
        {
            get { return _mediator; }
            set
            {
                if(_mediator != null)
                    _mediator.UnsubscribeSystem(this);
                _mediator = value;
                if(value != null)
                    _mediator.SubscribeSystem(this);
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
            return (ReadOnlySignalSet)_allSignals.FindAll(delegate(Signal signal) { return signal.BehavesAsSourceSignal; }).AsReadOnly; // && !inputSignals.Exists(delegate(Signal s) { return signal.DependsOn(s); }); }).AsReadOnly();
        }
        #endregion

        #region Private Write Direct Access
        private void InternalAddSignal(Signal signal)
        {
            int idx = _allSignals.Count;
            _allSignals.Add(signal);
            if(_mediator != null)
                _mediator.OnSignalAdded(signal, idx);
        }
        private void InternalRemoveSignal(Signal signal)
        {
            if(_inputs.Contains(signal))
                InternalRemoveInput(signal);
            if(_outputs.Contains(signal))
                InternalRemoveOutput(signal);
            int idx = _allSignals.IndexOf(signal);
            _allSignals.RemoveAt(idx);
            if(_mediator != null)
            {
                _mediator.OnSignalRemoved(signal, idx);
                for(int i = idx; i < _allSignals.Count; i++)
                    _mediator.OnSignalMoved(_allSignals[i], idx + 1, idx);
            }
        }
        private void InternalAddBus(Bus bus)
        {
            int idx = _allBuses.Count;
            _allBuses.Add(bus);
            if(_mediator != null)
                _mediator.OnBusAdded(bus, idx);
        }
        private void InternalRemoveBus(Bus bus)
        {
            int idx = _allBuses.IndexOf(bus);
            _allBuses.RemoveAt(idx);
            if(_mediator != null)
            {
                _mediator.OnBusRemoved(bus, idx);
                for(int i = idx; i < _allBuses.Count; i++)
                    _mediator.OnBusMoved(_allBuses[i], idx + 1, idx);
            }
        }
        private void InternalAddPort(Port port)
        {
            int idx = _allPorts.Count;
            _allPorts.Add(port);
            if(_mediator != null)
                _mediator.OnPortAdded(port, idx);
        }
        private void InternalRemovePort(Port port)
        {
            int idx = _allPorts.IndexOf(port);
            _allPorts.RemoveAt(idx);
            if(_mediator != null)
            {
                _mediator.OnPortRemoved(port, idx);
                for(int i = idx; i < _allPorts.Count; i++)
                    _mediator.OnPortMoved(_allPorts[i], idx + 1, idx);
            }
        }
        private void InternalAddInput(Signal signal)
        {
            int idx = _inputs.Count;
            _inputs.Add(signal);
            if(_mediator != null)
                _mediator.OnInputAdded(signal, idx);
        }
        private void InternalRemoveInput(Signal signal)
        {
            int idx = _inputs.IndexOf(signal);
            _inputs.RemoveAt(idx);
            if(_mediator != null)
            {
                _mediator.OnInputRemoved(signal, idx);
                for(int i = idx; i < _inputs.Count; i++)
                    _mediator.OnInputMoved(_inputs[i], idx + 1, idx);
            }
        }
        private void InternalAddOutput(Signal signal)
        {
            int idx = _outputs.Count;
            _outputs.Add(signal);
            if(_mediator != null)
                _mediator.OnOutputAdded(signal, idx);
        }
        private void InternalRemoveOutput(Signal signal)
        {
            int idx = _outputs.IndexOf(signal);
            _outputs.RemoveAt(idx);
            if(_mediator != null)
            {
                _mediator.OnOutputRemoved(signal, idx);
                for(int i = idx; i < _outputs.Count; i++)
                    _mediator.OnOutputMoved(_outputs[i], idx + 1, idx);
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
            System.Diagnostics.Debug.Assert(buses != null);
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

            Scanner.ForEachSignal(signal, delegate(Signal s)
            {
                if(!_allSignals.Contains(s))
                {
                    InternalAddSignal(s);
                    if(autoAddInputs && !s.IsDriven && !StdPackage.Std.IsConstant(s))
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
                        InternalAddPort(p);
                        AddBusRange(p.Buses);
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

            Scanner.ForEachSignal(signal, delegate(Signal s)
            {
                if(!_allSignals.Contains(s))
                {
                    InternalAddSignal(s);
                    if(autoAddInputs && !s.IsDriven && !StdPackage.Std.IsConstant(s))
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
                        InternalAddPort(p);
                        AddBusRange(p.Buses);
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
            Signal s = new Signal(_context);
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
        public Signal AddNamedSignal(string name, ValueStructure value)
        {
            if(_namedSignals == null)
                _namedSignals = new Dictionary<string, Signal>();
            Signal signal = new Signal(_context, value);
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
            Bus b = new Bus(_context);
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
        public void PushInputValue(int inputIndex, ValueStructure value)
        {
            _inputs[inputIndex].PostNewValue(value);
        }
        public void PushInputValue(int inputIndex, ValueStructure value, TimeSpan delay)
        {
            _inputs[inputIndex].PostNewValue(value, delay);
        }
        public void PushInputValueRange(IEnumerable<ValueStructure> values)
        {
            int idx = 0;
            foreach(ValueStructure value in values)
                _inputs[idx++].PostNewValue(value);
        }
        public void PushInputValueRange(IEnumerable<ValueStructure> values, TimeSpan delay)
        {
            int idx = 0;
            foreach(ValueStructure value in values)
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
            s.SignalValueChanged += s_SignalValueChanged;
        }
        //private void RegisterOutputSignalRange(IEnumerable<Signal> signals)
        //{
        //    foreach(Signal s in signals)
        //        RegisterOutputSignal(s);
        //}

        private void UnregisterOutputSignal(Signal s)
        {
            s.SignalValueChanged -= s_SignalValueChanged;
        }
        //private void UnregisterOutputSignalRange(IEnumerable<Signal> signals)
        //{
        //    foreach(Signal s in signals)
        //        UnregisterOutputSignal(s);
        //}

        void s_SignalValueChanged(object sender, SignalEventArgs e)
        {
            EventHandler<IndexedSignalEventArgs> handler = OutputValueChanged;
            if(handler != null && _outputs.Contains(e.Signal))
                handler(this, new IndexedSignalEventArgs(e.Signal, _outputs.IndexOf(e.Signal)));
        }
        #endregion
        #endregion

        #region Evaluation - Systems may be used as "Functions"
        public ValueStructure[] Evaluate(params ValueStructure[] inputs)
        {
            if(inputs.Length != _inputs.Count)
                throw new System.ArgumentException("The count of inputs doesn't match the systems count of input signals.", "inputs");

            for(int i = 0; i < inputs.Length; i++)
                _inputs[i].PostNewValue(inputs[i]);

            _context.Scheduler.SimulateInstant();

            ValueStructure[] outputs = new ValueStructure[_outputs.Count];
            for(int i = 0; i < outputs.Length; i++)
                outputs[i] = _outputs[i].Value;
            return outputs;
        }

        public double[] Evaluate(params double[] inputs)
        {
            if(inputs.Length != _inputs.Count)
                throw new System.ArgumentException("The count of inputs doesn't match the systems count of input signals.", "inputs");

            for(int i = 0; i < inputs.Length; i++)
                _inputs[i].PostNewValue(new RealValue(inputs[i]));

            _context.Scheduler.SimulateInstant();

            double[] outputs = new double[_outputs.Count];
            for(int i = 0; i < outputs.Length; i++)
                outputs[i] = RealValue.ConvertFrom(_outputs[i].Value).Value;
            return outputs;
        }
        #endregion

        #region System Builder
        internal void AcceptSystemBuilder(ISystemBuilder builder)
        {
            Dictionary<Guid, Guid> signalMappings = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> busMappings = new Dictionary<Guid, Guid>();

            builder.BeginBuildSystem(_inputs.Count, _outputs.Count, _allBuses.Count);

            // Before
            foreach(Signal s in _allSignals)
                signalMappings.Add(s.InstanceId, s.AcceptSystemBuilderBefore(builder));
            foreach(Bus b in _allBuses)
                busMappings.Add(b.InstanceId, b.AcceptSystemBuilderBefore(builder));
            foreach(Port p in _allPorts)
                p.AcceptSystemBuilder(builder, signalMappings, busMappings);

            // After
            foreach(Signal s in _allSignals)
                s.AcceptSystemBuilderAfter(builder, signalMappings, busMappings);

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
        public MathSystem Clone()
        {
            SystemWriter writer = new SystemWriter(_context);
            SystemReader reader = new SystemReader(writer);
            reader.ReadSystem(this);
            return writer.WrittenSystems.Dequeue();
        }
        #endregion
        #region Write Xml
        public void WriteXml(XmlWriter writer)
        {
            XmlSystemWriter xwriter = new XmlSystemWriter(_context, writer);
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
            settings.Encoding = Context.DefaultEncoding;
            XmlWriter writer = XmlWriter.Create(sb, settings);
            WriteXml(writer);
            writer.Close();
            return sb.ToString();
        }
        public void WriteXml(FileInfo file)
        {
            XmlWriter writer = XmlWriter.Create(file.CreateText());
            WriteXml(writer);
            writer.Close();
        }
        #endregion
        #region Read Xml
        public static MathSystem ReadXml(XmlReader reader, Context context)
        {
            SystemWriter xwriter = new SystemWriter(context);
            XmlSystemReader xreader = new XmlSystemReader(xwriter);
            xreader.ReadSystems(reader, false);
            return xwriter.WrittenSystems.Dequeue();
        }
        public static MathSystem ReadXml(string xml, Context context)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXml(reader, context);
        }
        public static MathSystem ReadXml(FileInfo file, Context context)
        {
            XmlReader reader = XmlReader.Create(file.OpenText());
            MathSystem sys = ReadXml(reader, context);
            reader.Close();
            return sys;
        }
        #endregion
        #region Read Xml -> Build Entity
        public static Entity ReadXmlEntity(XmlReader reader, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            reader.ReadToFollowing("System", Context.YttriumNamespace);
            int inputCnt = int.Parse(reader.GetAttribute("inputCount"), Context.NumberFormat);
            int outputCnt = int.Parse(reader.GetAttribute("outputCount"), Context.NumberFormat);
            int busCnt = int.Parse(reader.GetAttribute("busCount"), Context.NumberFormat);
            reader.Close();

            string[] inputSignalLabels = new string[inputCnt];
            for(int i = 0; i < inputCnt; i++)
                inputSignalLabels[i] = "In_" + i.ToString(Context.NumberFormat);

            string[] outputSignalLabels = new string[outputCnt];
            for(int i = 0; i < outputCnt; i++)
                outputSignalLabels[i] = "Out_" + i.ToString(Context.NumberFormat);

            string[] busLabels = new string[busCnt];
            for(int i = 0; i < busCnt; i++)
                busLabels[i] = "Bus_" + i.ToString(Context.NumberFormat);

            return new Entity(symbol, entityId.Label, entityId.Domain, notation, precedence, inputSignalLabels, outputSignalLabels, busLabels);
        }
        public static Entity ReadXmlEntity(XmlReader reader, MathIdentifier entityId, string symbol)
        {
            return ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
        }
        public static Entity ReadXmlEntity(string xml, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXmlEntity(reader, entityId, symbol, notation, precedence);
        }
        public static Entity ReadXmlEntity(string xml, MathIdentifier entityId, string symbol)
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);
            return ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
        }
        public static Entity ReadXmlEntity(FileInfo file, MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            XmlReader reader = XmlReader.Create(file.OpenText());
            Entity entity = ReadXmlEntity(reader, entityId, symbol, notation, precedence);
            reader.Close();
            return entity;
        }
        public static Entity ReadXmlEntity(FileInfo file, MathIdentifier entityId, string symbol)
        {
            XmlReader reader = XmlReader.Create(file.OpenText());
            Entity entity = ReadXmlEntity(reader, entityId, symbol, InfixNotation.None, -1);
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

        public Entity BuildEntity(MathIdentifier entityId, string symbol)
        {
            return BuildEntity(entityId, symbol, InfixNotation.None, -1);
        }
        public Entity BuildEntity(MathIdentifier entityId, string symbol, InfixNotation notation, int precedence)
        {
            string[] inputSignalLabels = new string[_inputs.Count];
            for(int i = 0; i < inputSignalLabels.Length; i++)
                inputSignalLabels[i] = string.IsNullOrEmpty(_inputs[i].Label) ? "In_" + i.ToString(Context.NumberFormat) : _inputs[i].Label;

            string[] outputSignalLabels = new string[_outputs.Count];
            for(int i = 0; i < outputSignalLabels.Length; i++)
                outputSignalLabels[i] = string.IsNullOrEmpty(_outputs[i].Label) ? "Out_" + i.ToString(Context.NumberFormat) : _outputs[i].Label;

            string[] busLabels = new string[_allBuses.Count];
            for(int i = 0; i < busLabels.Length; i++)
                busLabels[i] = string.IsNullOrEmpty(_allBuses[i].Label) ? "Bus_" + i.ToString(Context.NumberFormat) : _allBuses[i].Label;

            return new Entity(symbol, entityId.Label, entityId.Domain, notation, precedence, inputSignalLabels, outputSignalLabels, busLabels);
        }

        public void PublishToLibrary(MathIdentifier architectureId, MathIdentifier entityId)
        {
            IArchitectureFactory factory = BuildArchitectureFactory(architectureId, entityId);
            _context.Library.Architectures.AddArchitectureBuilder(factory);
        }
        public Entity PublishToLibrary(string label, string symbol)
        {
            return this.PublishToLibrary("Work", label + "Compound", label, symbol);
        }
        public Entity PublishToLibrary(string domain, string architectureLabel, string entityLabel, string entitySymbol)
        {
            Entity entity = BuildEntity(new MathIdentifier(entityLabel, domain), entitySymbol);
            IArchitectureFactory factory = BuildArchitectureFactory(new MathIdentifier(architectureLabel, domain), entity.EntityId);
            _context.Library.Entities.Add(entity);
            _context.Library.Architectures.AddArchitectureBuilder(factory);
            return entity;
        }
        public static void PublishToLibrary(string xml, Library library, MathIdentifier architectureId, MathIdentifier entityId)
        {
            IArchitectureFactory factory = BuildArchitectureFactory(xml, architectureId, entityId);
            library.Architectures.AddArchitectureBuilder(factory);
        }
        public static Entity PublishToLibrary(string xml, Library library, string label, string symbol)
        {
            return PublishToLibrary(xml, library, "Work", label + "Compound", label, symbol);
        }
        public static Entity PublishToLibrary(string xml, Library library, string domain, string architectureLabel, string entityLabel, string entitySymbol)
        {
            Entity entity = ReadXmlEntity(xml, new MathIdentifier(entityLabel, domain), entitySymbol);
            IArchitectureFactory factory = BuildArchitectureFactory(xml, new MathIdentifier(architectureLabel, domain), entity.EntityId, entity.InputSignals.Length, entity.OutputSignals.Length, entity.Buses.Length);
            library.Entities.Add(entity);
            library.Architectures.AddArchitectureBuilder(factory);
            return entity;
        }

        public Entity PublishToLibraryAnonymous()
        {
            return this.PublishToLibrary("Anonymous_" + Guid.NewGuid().ToString("N"), "anonymous");
        }
        public static Entity PublishToLibraryAnonymous(string xml, Library library)
        {
            return PublishToLibrary(xml, library, "Anonymous_" + Guid.NewGuid().ToString("N"), "anonymous");
        }
        #endregion

        #region Maintenance
        /// <returns>True if some objects where removed.</returns>
        public bool RemoveUnusedObjects()
        {
            SignalSet signals;
            PortSet ports;
            Scanner.FindAll(_outputs, false, out signals, out ports);
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
