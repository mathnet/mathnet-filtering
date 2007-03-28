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

namespace MathNet.Symbolics.Mediator
{
    [Serializable]
    public class RemovePortCommand : ObjectModel.PortCommand
    {
        private bool _isolate;

        public RemovePortCommand() { }

        public bool Isolate
        {
            get { return _isolate; }
            set { _isolate = value; }
        }

        protected override void Action(Port port)
        {
            System.RemovePort(port, _isolate);
        }

        #region Serialization
        protected RemovePortCommand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _isolate = bool.Parse(info.GetString("isolate"));
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("isolate", _isolate.ToString());
        }
        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("RemovePortCommand", Config.YttriumNamespace);
            base.ReadXml(reader);
            _isolate = bool.Parse(reader.ReadElementString("Isolate"));
            reader.ReadEndElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("RemovePortCommand", Config.YttriumNamespace);
            base.WriteXml(writer);
            writer.WriteElementString("Isolate", _isolate.ToString());
            writer.WriteEndElement();
        }
        #endregion
    }
}
