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
    public abstract class BusCommand : CommandBase, ICommand
    {
        private CommandReference _busRef;

        protected BusCommand() { }

        public CommandReference BusReference
        {
            get { return _busRef; }
            set { _busRef = value; }
        }

        protected abstract void Action(Bus bus);

        [System.Diagnostics.DebuggerStepThrough]
        public void Execute()
        {
            if(!BeginExecute())
                return;
            Bus bus = GetVerifyBus(_busRef);
            Action(bus);
            EndExecute();
        }

        #region Serialization
        protected BusCommand(SerializationInfo info, StreamingContext context)
        {
            _busRef = CommandReference.Deserialize("bus", info);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _busRef.Serialize("bus", info);
        }
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("BusCommand", Config.YttriumNamespace);
            _busRef = CommandReference.Deserialize("Bus", reader);
            reader.ReadEndElement();
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("BusCommand", Config.YttriumNamespace);
            _busRef.Serialize("Bus", writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
