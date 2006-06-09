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
using System.Text;
using System.Xml;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.SystemBuilder
{
    /// <summary>
    /// Concrete System Builder for constructing a complete Xml representation.
    /// </summary>
    public class XmlSystemWriter : ISystemBuilder
    {
        private Context _context;
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
                        _writer.WriteStartElement(after.ToString(), Context.YttriumNamespace);
                }
            }
        }

        public XmlSystemWriter(Context context, XmlWriter writer)
        {
            _context = context;
            _writer = writer;
            _fsm = new StateMachine(writer);
        }

        public Context Context
        {
            get { return _context; }
            set { _context = value; }
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
            _writer.WriteAttributeString("inputCount", inputSignalCount.ToString(Context.NumberFormat));
            _writer.WriteAttributeString("outputCount", outputSignalCount.ToString(Context.NumberFormat));
            _writer.WriteAttributeString("busCount", busCount.ToString(Context.NumberFormat));
        }

        public Guid BuildSignal(string label, bool hold, bool isSource)
        {
            _fsm.AdvanceTo(BuilderState.Signals);
            Guid guid = Guid.NewGuid();
            _writer.WriteStartElement("Signal", Context.YttriumNamespace);
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
            _writer.WriteStartElement("Bus", Context.YttriumNamespace);
            _writer.WriteAttributeString("iid", guid.ToString());
            _writer.WriteAttributeString("label", label);
            _writer.WriteEndElement();
            return guid;
        }

        public Guid BuildPort(MathIdentifier entityId, InstanceIdSet inputSignals, InstanceIdSet outputSignals, InstanceIdSet buses)
        {
            _fsm.AdvanceTo(BuilderState.Ports);
            Guid guid = Guid.NewGuid();
            _writer.WriteStartElement("Port", Context.YttriumNamespace);
            _writer.WriteAttributeString("iid", guid.ToString());
            _writer.WriteAttributeString("entityId", entityId.ToString());

            _writer.WriteStartElement("InputSignals", Context.YttriumNamespace);
            foreach(Guid id in inputSignals)
                _writer.WriteElementString("SignalRef", Context.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("OutputSignals", Context.YttriumNamespace);
            foreach(Guid id in outputSignals)
                _writer.WriteElementString("SignalRef", Context.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("Buses", Context.YttriumNamespace);
            foreach(Guid id in buses)
                _writer.WriteElementString("BusRef", Context.YttriumNamespace, id.ToString());
            _writer.WriteEndElement();

            _writer.WriteEndElement();
            return guid;
        }

        public void AppendSignalValue(Guid iid, StructurePack value)
        {
            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalValue", Context.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(value.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSignalProperty(Guid iid, PropertyPack property)
        {
            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalProperty", Context.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(property.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSignalConstraint(Guid iid, PropertyPack constraint)
        {
            _fsm.AdvanceTo(BuilderState.SignalDetails);
            _writer.WriteStartElement("SignalConstraint", Context.YttriumNamespace);
            _writer.WriteAttributeString("iid", iid.ToString());
            _writer.WriteRaw(constraint.SerializedXmlFragment);
            _writer.WriteEndElement();
        }

        public void AppendSystemInputSignal(Guid iid)
        {
            _fsm.AdvanceTo(BuilderState.InputSignals);
            _writer.WriteElementString("SignalRef", Context.YttriumNamespace, iid.ToString());
        }

        public void AppendSystemOutputSignal(Guid iid)
        {
            _fsm.AdvanceTo(BuilderState.OutputSignals);
            _writer.WriteElementString("SignalRef", Context.YttriumNamespace, iid.ToString());

        }

        public void AppendSystemNamedSignal(Guid iid, string name)
        {
            _fsm.AdvanceTo(BuilderState.NamedSignals);
            _writer.WriteStartElement("SignalRef", Context.YttriumNamespace);
            _writer.WriteAttributeString("name", name);
            _writer.WriteString(iid.ToString());
            _writer.WriteEndElement();
        }

        public void AppendSystemNamedBus(Guid iid, string name)
        {
            _fsm.AdvanceTo(BuilderState.NamedBuses);
            _writer.WriteStartElement("BusRef", Context.YttriumNamespace);
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
