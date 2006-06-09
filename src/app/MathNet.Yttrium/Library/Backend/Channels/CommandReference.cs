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
