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
    public abstract class SignalCommand : CommandBase, ICommand
    {
        private CommandReference _signalRef;

        protected SignalCommand() { }

        public CommandReference SignalReference
        {
            get { return _signalRef; }
            set { _signalRef = value; }
        }

        protected abstract void Action(Signal signal);

        public void Execute()
        {
            if(!BeginExecute())
                return;
            Signal signal = GetVerifySignal(_signalRef);
            Action(signal);
            EndExecute();
        }

        #region Serialization
        protected SignalCommand(SerializationInfo info, StreamingContext context)
        {
            _signalRef = CommandReference.Deserialize("signal", info);
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _signalRef.Serialize("signal", info);
        }
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("SignalCommand", Config.YttriumNamespace);
            _signalRef = CommandReference.Deserialize("Signal", reader);
            reader.ReadEndElement();
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("SignalCommand", Config.YttriumNamespace);
            _signalRef.Serialize("Signal", writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
