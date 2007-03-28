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
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Events;

namespace MathNet.Symbolics
{
    public interface IMathSystem
    {
        event EventHandler<IndexedSignalEventArgs> OutputValueChanged;

        Guid InstanceId { get;}

        int SignalCount { get;}
        int BusCount { get;}
        int PortCount { get;}
        int InputCount { get;}
        int OutputCount { get;}

        bool ContainsSignal(Signal signal);
        bool ContainsBus(Bus bus);
        bool ContainsPort(Port port);

        Signal GetSignal(int index);
        Bus GetBus(int index);
        Port GetPort(int index);
        Signal GetInput(int index);
        Signal GetOutput(int index);

        ReadOnlySignalSet GetAllSignals();
        ReadOnlyBusSet GetAllBuses();
        ReadOnlyPortSet GetAllPorts();
        ReadOnlySignalSet GetAllInputs();
        ReadOnlySignalSet GetAllOutputs();
        ReadOnlySignalSet GetAllLeafSignals();

        void AddSignal(Signal signal);
        void AddSignalRange(IEnumerable<Signal> signals);
        void RemoveSignal(Signal signal, bool isolateFromDriver);
        void AddBus(Bus bus);
        void AddBusRange(IEnumerable<Bus> buses);
        void RemoveBus(Bus bus);
        void AddPort(Port port);
        void RemovePort(Port port, bool isolate);
        void AddSignalTree(Signal signal, bool setAsOutput, bool autoAddInputs);
        void AddSignalTree(Signal signal, ICollection<Signal> border, bool setAsOutput, bool autoAddInputs);
        void AddSignalTreeRange(IEnumerable<Signal> signals, bool setAsOutput, bool autoAddInputs);
        void AddSignalTreeRange(IEnumerable<Signal> signals, ICollection<Signal> border, bool setAsOutput, bool autoAddInputs);
        void AddPortTree(Port port, bool setAsOutput, bool autoAddInputs);
        
        int PromoteAsInput(Signal signal);
        void UnpromoteAsInput(Signal signal);
        int PromoteAsOutput(Signal signal);
        void UnpromoteAsOutput(Signal signal);

        Signal CreateNamedSignal(string name);
        Signal AddNamedSignal(string name, Signal signal);
        Signal AddNamedSignal(string name, IValueStructure value);
        Signal LookupNamedSignal(string name);
        bool TryLookupNamedSignal(string name, out Signal signal);
        bool ContainsNamedSignal(string name);
        void RemoveNamedSignal(string name);

        Bus CreateNamedBus(string name);
        void AddNamedBus(string name, Bus bus);
        Bus LookupNamedBus(string name);
        bool TryLookupNamedBus(string name, out Bus bus);
        bool ContainsNamedBus(string name);
        void RemoveNamedBus(string name);

        void PushInputValue(int inputIndex, IValueStructure value);
        void PushInputValue(int inputIndex, IValueStructure value, TimeSpan delay);
        void PushInputValueRange(IEnumerable<IValueStructure> values);
        void PushInputValueRange(IEnumerable<IValueStructure> values, TimeSpan delay);
        void PushInputValueRange(IEnumerable<Signal> signalsWithValues);
        void PushInputValueRange(IEnumerable<Signal> signalsWithValues, TimeSpan delay);

        IValueStructure[] Evaluate(params IValueStructure[] inputs);
        double[] Evaluate(params double[] inputs);

        bool RemoveUnusedObjects();

        void AcceptSystemBuilder(ISystemBuilder builder);

        //bool HasSystemMediator;
        //void LoadDefaultSystemMediator();

        void PublishToLibrary(MathIdentifier architectureId, MathIdentifier entityId);
        IEntity PublishToLibrary(string label, string symbol);
        IEntity PublishToLibraryAnonymous();
    }
}
