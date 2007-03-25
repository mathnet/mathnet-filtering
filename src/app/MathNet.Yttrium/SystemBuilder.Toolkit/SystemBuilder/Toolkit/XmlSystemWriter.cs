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
using System.Xml;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.SystemBuilder.Toolkit
{
    /// <summary>
    /// Concrete System Builder for constructing a complete Xml representation.
    /// </summary>
    public class XmlSystemWriter : ISystemBuilder
    {
        private XmlWriter _writer;
        private StateMachine _fsm;

        private class StateMachine : BuilderStateMachine
        {
            private XmlWriter _writer;

            public StateMachine(XmlWriter writer)
            {
                _writer = writer;
            }

            public XmlWriter Writer
            {
                get { return _writer; }
                set { _writer = value; }
            }

            protected override void OnAfterAdvance(BuilderState before, BuilderState after, bool wasInsideSystem, bool wasInsideGroup)
            {
                if(after != before)
                {
                    if(wasInsideGroup)
                        _writer.WriteEndElement();
                    if(after == BuilderState.Idle)
                        _writer.WriteEndElement();
                    else
                        _writer.WriteStartElement(after.ToString(), Config.YttriumNamespace);
                }
            }
        }

        public XmlSystemWriter(XmlWriter writer)
        {
            _writer = writer;
            _fsm = new StateMachine(writer);
        }

        public XmlWriter Writer
        {
            get { return _writer; }
            set
            {
                _writer = value;
                _fsm.Writer = value;
            }
        }

        public BuilderState State
        {
            get { return _fsm.CurrentState; }
        }

        public void BeginBuildSystem(int inputSignalCount, int outputSignalCount, int busCount)
        {
            _fsm.AdvanceTo(BuilderState.System);
            _writer.WriteAttributeString("inputCount", inputSignalCount.ToString(Config.InternalNumberFormat));
            _writer.WriteAttributeString("outputCount", outputSignalCount.ToString(Config.InternalNumberFormat));
            _writer.WriteAttributeString("busCount", busCount.ToString(Config.InternalNumberFormat));
        }

        public Guid BuildSignal(string label, bool hold, bool isSource)
        {
            _fsm.AdvanceTo(BuilderState.Signals);
            Guid guid = Guid.NewGuid();
            _writer.WriteStartElement("Signal", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", guid.ToString());
            _writer.WriteAttributeString("label", label);
            _writer.WriteAttributeString("hold", hold.ToString());
            _writer.WriteAttributeString("isSource", isSource.ToString());
            _writer.WriteEndElement();
            return guid;
        }

        public Guid BuildBus(string label)
        {
            _fsm.AdvanceTo(BuilderState.Buses);
            Guid guid = Guid.NewGuid();
            _writer.WriteStartElement("Bus", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", guid.ToString());
            _writer.WriteAttributeString("label", label);
            _writer.WriteEndElement();
            return guid;
        }

        public Guid BuildPort(MathIdentifier entityId, InstanceIdSet inputSignals, InstanceIdSet outputSignals, InstanceIdSet buses)
        {
            _fsm.AdvanceTo(BuilderState.Ports);
            Guid guid = Guid.NewGuid();
            _writer.WriteStartElement("Port", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", guid.ToString());
            _writer.WriteAttributeString("entityId", entityId.ToString());

            _writer.WriteStartElement("InputSignals", Config.YttriumNamespace);
            foreach(Guid id in inputSignals)
                _writer.WriteElementString("SignalRef", Config.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("OutputSignals", Config.YttriumNamespace);
            foreach(Guid id in outputSignals)
                _writer.WriteElementString("SignalRef", Config.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("Buses", Config.YttriumNamespace);
            foreach(Guid id in buses)
                _writer.WriteElementString("BusRef", Config.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteEndElement();
            return guid;
        }

        public void AppendSignalValue(Guid iid, ICustomDataPack<IValueStructure> value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalValue", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(value.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSignalProperty(Guid iid, ICustomDataPack<IProperty> property)
        {
            if(property == null)
                throw new ArgumentNullException("property");

            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalProperty", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(property.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSignalConstraint(Guid iid, ICustomDataPack<IProperty> constraint)
        {
            if(constraint == null)
                throw new ArgumentNullException("constraint");

            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalConstraint", Config.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(constraint.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSystemInputSignal(Guid iid)
        {
            _fsm.AdvanceTo(BuilderState.InputSignals);
            _writer.WriteElementString("SignalRef", Config.YttriumNamespace, iid.ToString());
        }

        public void AppendSystemOutputSignal(Guid iid)
        {
            _fsm.AdvanceTo(BuilderState.OutputSignals);
            _writer.WriteElementString("SignalRef", Config.YttriumNamespace, iid.ToString());

        }

        public void AppendSystemNamedSignal(Guid iid, string name)
        {
            _fsm.AdvanceTo(BuilderState.NamedSignals);
            _writer.WriteStartElement("SignalRef", Config.YttriumNamespace);
            _writer.WriteAttributeString("name", name);
            _writer.WriteString(iid.ToString());
            _writer.WriteEndElement();
        }

        public void AppendSystemNamedBus(Guid iid, string name)
        {
            _fsm.AdvanceTo(BuilderState.NamedBuses);
            _writer.WriteStartElement("BusRef", Config.YttriumNamespace);
            _writer.WriteAttributeString("name", name);
            _writer.WriteString(iid.ToString());
            _writer.WriteEndElement();
        }

        public void EndBuildSystem()
        {
            _fsm.AdvanceTo(BuilderState.Idle);
            _writer.Flush();
        }
    }
}
