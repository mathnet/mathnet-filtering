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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;

using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Mediator
{
    [Serializable]
    public class NewPortCommand : ObjectModel.NewCommand
    {
        private MathIdentifier _entityId;
        private int _inputCnt, _busCnt;
        private int? _outputCnt;

        public NewPortCommand() { }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
            set { _entityId = value; }
        }
        public int NumberOfInputs
        {
            get { return _inputCnt; }
            set { _inputCnt = value; }
        }
        public int NumberOfBuses
        {
            get { return _busCnt; }
            set { _busCnt = value; }
        }
        public int? NumberOfOutputs
        {
            get { return _outputCnt; }
            set { _outputCnt = value; }
        }

        protected override CommandReference Action()
        {
            int idx = System.PortCount;
            IEntity e =  Service<ILibrary>.Instance.LookupEntity(_entityId);
            if(e.IsGeneric)
                e = e.CompileGenericEntity(_inputCnt, _busCnt, _outputCnt);
            Port p = e.InstantiateUnboundPort();
            Guid iid = p.InstanceId;
            System.AddPort(p);
            System.AddSignalRange(p.OutputSignals);
            return new CommandReference(iid, idx);
        }

        #region Serialization
        protected NewPortCommand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _entityId = MathIdentifier.Parse(info.GetString("entityId"));
            _inputCnt = info.GetInt32("inputCount");
            _busCnt = info.GetInt32("busCount");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("entityId", _entityId.ToString());
            info.AddValue("inputCount", _inputCnt);
            info.AddValue("busCount", _busCnt);
        }
        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("NewPortCommand", Config.YttriumNamespace);
            base.ReadXml(reader);
            _entityId = MathIdentifier.Parse(reader.ReadElementString("EntityId"));
            _inputCnt = int.Parse(reader.ReadElementString("InputCount"));
            _busCnt = int.Parse(reader.ReadElementString("BusCount"));
            reader.ReadEndElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("NewPortCommand", Config.YttriumNamespace);
            base.WriteXml(writer);
            writer.WriteElementString("EntityId", _entityId.ToString());
            writer.WriteElementString("InputCount", _inputCnt.ToString());
            writer.WriteElementString("BusCount", _busCnt.ToString());
            writer.WriteEndElement();
        }
        #endregion
    }
}
