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

namespace MathNet.Symbolics.Mediator.ObjectModel
{
    [Serializable]
    public abstract class PortBusIndexCommand : CommandBase, ICommand
    {
        private CommandReference _busRef, _portRef;
        private int _index;

        protected PortBusIndexCommand() { }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public CommandReference BusReference
        {
            get { return _busRef; }
            set { _busRef = value; }
        }
        public CommandReference PortReference
        {
            get { return _portRef; }
            set { _portRef = value; }
        }

        protected abstract void Action(Port port, Bus bus, int index);

        public void Execute()
        {
            if(!BeginExecute())
                return;
            Bus bus = GetVerifyBus(_busRef);
            Port port = GetVerifyPort(_portRef);
            Action(port, bus, _index);
            EndExecute();
        }

        #region Serialization
        protected PortBusIndexCommand(SerializationInfo info, StreamingContext context)
        {
            _index = info.GetInt32("idx");
            _portRef = CommandReference.Deserialize("port", info);
            _busRef = CommandReference.Deserialize("bus", info);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("idx", _index);
            _portRef.Serialize("port", info);
            _busRef.Serialize("bus", info);
        }
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("PortBusIndexCommand", Config.YttriumNamespace);
            _index = int.Parse(reader.ReadElementString("Index"));
            _portRef = CommandReference.Deserialize("Port", reader);
            _busRef = CommandReference.Deserialize("Bus", reader);
            reader.ReadEndElement();
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PortBusIndexCommand", Config.YttriumNamespace);
            writer.WriteElementString("Index", _index.ToString());
            _portRef.Serialize("Port", writer);
            _busRef.Serialize("Bus", writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
