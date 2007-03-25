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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.SystemBuilder.Toolkit
{
    /// <summary>
    /// Concrete System Builder for constructing a <see cref="MathSystem"/>.
    /// </summary>
    public class SystemWriter : ISystemBuilder
    {
        private Queue<IMathSystem> _writtenSystems;
        private IMathSystem _system;
        private Dictionary<Guid, Signal> _signalMappings;
        private Dictionary<Guid, Bus> _busMappings;

        public SystemWriter()
        {
            _writtenSystems = new Queue<IMathSystem>();
            _signalMappings = new Dictionary<Guid, Signal>();
            _busMappings = new Dictionary<Guid, Bus>();
        }

        public Queue<IMathSystem> WrittenSystems
        {
            get { return _writtenSystems; }
        }

        private SignalSet MapSignals(InstanceIdSet signals)
        {
            return SignalSet.ConvertAllFromInstanceIds(signals, delegate(Guid id) { return _signalMappings[id]; });
        }
        private BusSet MapBuses(InstanceIdSet buses)
        {
            return BusSet.ConvertAllFromInstanceIds(buses, delegate(Guid id) { return _busMappings[id]; });
        }

        public void BeginBuildSystem(int inputSignalCount, int outputSignalCount, int busCount)
        {
            _system = Binder.CreateSystem();
            _signalMappings.Clear();
            _busMappings.Clear();
        }

        public Guid BuildSignal(string label, bool hold, bool isSource)
        {
            Signal s = Binder.CreateSignal();
            _system.AddSignal(s);
            _signalMappings.Add(s.InstanceId, s);
            return s.InstanceId;
        }

        public Guid BuildBus(string label)
        {
            Bus b = Binder.CreateBus();
            _system.AddBus(b);
            _busMappings.Add(b.InstanceId, b);
            return b.InstanceId;
        }

        public Guid BuildPort(MathIdentifier entityId, InstanceIdSet inputSignals, InstanceIdSet outputSignals, InstanceIdSet buses)
        {
            IEntity entity = Service<ILibrary>.Instance.LookupEntity(entityId);
            Port p = entity.InstantiatePort(MapSignals(inputSignals), MapSignals(outputSignals), MapBuses(buses));
            _system.AddPort(p);
            return p.InstanceId;
        }

        public void AppendSignalValue(Guid iid, ICustomDataPack<IValueStructure> value)
        {
            IValueStructure structure = value.Unpack(_signalMappings, _busMappings);
            ((ISignal_BuilderAdapter)_signalMappings[iid]).BuilderSetValue(structure); 
        }

        public void AppendSignalProperty(Guid iid, ICustomDataPack<IProperty> property)
        {
            IProperty prop = property.Unpack(_signalMappings, _busMappings);
            ((ISignal_BuilderAdapter)_signalMappings[iid]).BuilderAppendProperty(prop);
        }

        public void AppendSignalConstraint(Guid iid, ICustomDataPack<IProperty> constraint)
        {
            IProperty prop = constraint.Unpack(_signalMappings, _busMappings);
            ((ISignal_BuilderAdapter)_signalMappings[iid]).BuilderAppendConstraint(prop);
        }

        public void AppendSystemInputSignal(Guid iid)
        {
            _system.PromoteAsInput(_signalMappings[iid]);
        }

        public void AppendSystemOutputSignal(Guid iid)
        {
            _system.PromoteAsOutput(_signalMappings[iid]);
        }

        public void AppendSystemNamedSignal(Guid iid, string name)
        {
            _system.AddNamedSignal(name, _signalMappings[iid]); 
        }

        public void AppendSystemNamedBus(Guid iid, string name)
        {
            _system.AddNamedBus(name, _busMappings[iid]);
        }

        public void EndBuildSystem()
        {
            _writtenSystems.Enqueue(_system);
            _system = null;
            _signalMappings.Clear();
            _busMappings.Clear();
        }
    }
}
