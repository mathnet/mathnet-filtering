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
    public abstract class AbstractNewCommand : CommandBase, ICommand
    {
        private CommandReference _ref;

        protected AbstractNewCommand() { }

        public CommandReference NewReference
        {
            get { return _ref; }
            set { _ref = value; }
        }

        protected abstract CommandReference Action();

        public void Execute()
        {
            if(!BeginExecute())
                return;
            _ref = Action();
            EndExecute();
        }

        #region Serialization
        protected AbstractNewCommand(SerializationInfo info, StreamingContext context)
        {
            _ref = CommandReference.Deserialize("new", info);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ref.Serialize("new", info);
        }
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("NewCommand", Context.YttriumNamespace);
            _ref = CommandReference.Deserialize("New", reader);
            reader.ReadEndElement();
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("NewCommand", Context.YttriumNamespace);
            _ref.Serialize("New", writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
