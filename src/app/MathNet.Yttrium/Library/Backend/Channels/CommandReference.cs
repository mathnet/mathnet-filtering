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

namespace MathNet.Symbolics.Backend.Channels
{
    public struct CommandReference
    {
        public Guid InstanceId;
        public int Index;

        public CommandReference(Guid instanceId, int index)
        {
            this.InstanceId = instanceId;
            this.Index = index;
        }

        #region Partial Serialization
        public void Serialize(string name, SerializationInfo info)
        {
            info.AddValue(name + "Index", Index);
            info.AddValue(name + "InstanceId", InstanceId);
        }
        public static CommandReference Deserialize(string name, SerializationInfo info)
        {
            int idx = info.GetInt32(name + "Index");
            Guid iid = new Guid(info.GetString(name + "InstanceId"));
            return new CommandReference(iid, idx);
        }
        public void Serialize(string name, XmlWriter writer)
        {
            writer.WriteElementString(name + "Index", Index.ToString());
            writer.WriteElementString(name + "InstanceId", InstanceId.ToString());
        }
        public static CommandReference Deserialize(string name, XmlReader reader)
        {
            int idx = int.Parse(reader.ReadElementString(name + "Index"));
            Guid iid = new Guid(reader.ReadElementString(name + "InstanceId"));
            return new CommandReference(iid, idx);
        }
        #endregion
    }
}
