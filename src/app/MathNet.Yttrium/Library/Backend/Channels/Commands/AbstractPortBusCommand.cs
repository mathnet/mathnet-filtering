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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Channels.Commands
{
    [Serializable]
    public abstract class AbstractPortBusCommand : CommandBase, ICommand
    {
        private CommandReference _busRef,_portRef;

        protected AbstractPortBusCommand() { }

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

        protected abstract void Action(Port port, Bus bus);

        public void Execute()
        {
            if(!BeginExecute())
                return;
            Bus bus = GetVerifyBus(_busRef);
            Port port = GetVerifyPort(_portRef);
            Action(port, bus);
            EndExecute();
        }

        #region Serialization
        protected AbstractPortBusCommand(SerializationInfo info, StreamingContext context)
        {
            _portRef = CommandReference.Deserialize("port", info);
            _busRef = CommandReference.Deserialize("bus", info);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _portRef.Serialize("port", info);
            _busRef.Serialize("bus", info);
        }
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("PortBusCommand", Context.YttriumNamespace);
            _portRef = CommandReference.Deserialize("Port", reader);
            _busRef = CommandReference.Deserialize("Bus", reader);
            reader.ReadEndElement();
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PortBusCommand", Context.YttriumNamespace);
            _portRef.Serialize("Port", writer);
            _busRef.Serialize("Bus", writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
