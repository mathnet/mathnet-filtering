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
    public class RemoveSignalCommand : AbstractSignalCommand
    {
        private bool _isolate;

        public RemoveSignalCommand() { }

        public bool Isolate
        {
            get { return _isolate; }
            set { _isolate = value; }
        }

        protected override void Action(Signal signal)
        {
            System.RemoveSignal(signal, _isolate);
        }

        #region Serialization
        protected RemoveSignalCommand(SerializationInfo info, StreamingContext context)
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
            reader.ReadStartElement("RemoveSignalCommand", Context.YttriumNamespace);
            base.ReadXml(reader);
            _isolate = bool.Parse(reader.ReadElementString("Isolate"));
            reader.ReadEndElement();
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("RemoveSignalCommand", Context.YttriumNamespace);
            base.WriteXml(writer);
            writer.WriteElementString("Isolate", _isolate.ToString());
            writer.WriteEndElement();
        }
        #endregion
    }
}
