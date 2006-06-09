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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Templates
{
    public class CompoundArchitecture : Architecture
    {
        private MathSystem system;
        private ISignalSet inputSignals, outputSignals;

        public CompoundArchitecture(MathIdentifier id, MathIdentifier entityId, Port port, MathSystem system)
            : base(id, entityId, false)
        {
            this.inputSignals = port.InputSignals;
            this.outputSignals = port.OutputSignals;
            this.system = system;
            this.system.OutputValueChanged += system_OutputValueChanged;

            SetPort(port);

            for(int i = 0; i < inputSignals.Count; i++)
                inputSignals[i].SignalValueChanged += CompoundArchitecture_SignalValueChanged;
            system.PushInputValueRange(inputSignals);
        }

        void CompoundArchitecture_SignalValueChanged(object sender, MathNet.Symbolics.Backend.Events.SignalEventArgs e)
        {
            int idx = inputSignals.IndexOf(e.Signal);
            system.PushInputValue(idx, inputSignals[idx].Value);
        }

        void system_OutputValueChanged(object sender, MathNet.Symbolics.Backend.Events.IndexedSignalEventArgs e)
        {
            outputSignals[e.Index].PostNewValue(e.Signal.Value);
        }

        public override bool SupportsPort(Port port)
        {
            return port != null && port.InputSignalCount == system.InputCount && port.OutputSignalCount == system.OutputCount && port.BusCount == system.BusCount;
        }

        public override void UnregisterArchitecture()
        {
            foreach(Signal s in inputSignals)
                s.SignalValueChanged -= CompoundArchitecture_SignalValueChanged;
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            this.inputSignals = newPort.InputSignals;
            this.outputSignals = newPort.OutputSignals;
            for(int i = 0; i < inputSignals.Count; i++)
                inputSignals[i].SignalValueChanged += CompoundArchitecture_SignalValueChanged;
            system.PushInputValueRange(inputSignals);
        }
    }

    public sealed class CompoundArchitectureFactory : IArchitectureFactory
    {
        private readonly string _xml;
        private readonly MathIdentifier _architectureId;
        private readonly MathIdentifier _entityId;
        private readonly int _inputCnt, _outputCnt, _busCnt;

        public CompoundArchitectureFactory(MathIdentifier architectureId, MathIdentifier entityId, string xml, int inputCount, int outputCount, int busCount)
        {
            _architectureId = architectureId;
            _entityId = entityId;
            _inputCnt = inputCount;
            _outputCnt = outputCount;
            _busCnt = busCount;
            _xml = xml;
        }
        public CompoundArchitectureFactory(MathIdentifier architectureId, MathIdentifier entityId, string xml)
        {
            _architectureId = architectureId;
            _entityId = entityId;
            _xml = xml;
            Entity dummy = MathSystem.ReadXmlEntity(xml, new MathIdentifier("Dummy", "Temp"), string.Empty);
            _inputCnt = dummy.InputSignals.Length;
            _outputCnt = dummy.OutputSignals.Length;
            _busCnt = dummy.Buses.Length;
        }
        public CompoundArchitectureFactory(MathIdentifier architectureId, MathIdentifier entityId, MathSystem system)
        {
            _architectureId = architectureId;
            _entityId = entityId;
            _inputCnt = system.InputCount;
            _outputCnt = system.OutputCount;
            _busCnt = system.BusCount;
            _xml = system.WriteXml(false);
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public bool SupportsPort(Port port)
        {
            return port != null && port.InputSignalCount == _inputCnt && port.OutputSignalCount == _outputCnt && port.BusCount == _busCnt;
        }

        public Architecture InstantiateToPort(Port port)
        {
            if(port == null) throw new ArgumentNullException("port");
            MathSystem system = MathSystem.ReadXml(_xml, port.Context);
            return new CompoundArchitecture(_architectureId, _entityId, port, system);
        }
    }
}
