using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace MathNet.Symbolics.Mediator
{
    [Serializable]
    public struct CommandReference : ISerializable, IEquatable<CommandReference>
    {
        private readonly Guid _instanceId;
        private readonly int _index;

        public CommandReference(Guid instanceId, int index)
        {
            _instanceId = instanceId;
            _index = index;
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public int Index
        {
            get { return _index;  }
        }

        #region Full Serialization
        private CommandReference(SerializationInfo info, StreamingContext context)
        {
            _index = info.GetInt32("Index");
            _instanceId = new Guid(info.GetString("InstanceId"));
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Index", _index);
            info.AddValue("InstanceId", _instanceId);
        }
        #endregion
        #region Partial Serialization
        public void Serialize(string name, SerializationInfo info)
        {
            info.AddValue(name + "Index", _index);
            info.AddValue(name + "InstanceId", _instanceId);
        }
        public static CommandReference Deserialize(string name, SerializationInfo info)
        {
            int idx = info.GetInt32(name + "Index");
            Guid iid = new Guid(info.GetString(name + "InstanceId"));
            return new CommandReference(iid, idx);
        }
        public void Serialize(string name, XmlWriter writer)
        {
            writer.WriteElementString(name + "Index", _index.ToString());
            writer.WriteElementString(name + "InstanceId", _instanceId.ToString());
        }
        public static CommandReference Deserialize(string name, XmlReader reader)
        {
            int idx = int.Parse(reader.ReadElementString(name + "Index"));
            Guid iid = new Guid(reader.ReadElementString(name + "InstanceId"));
            return new CommandReference(iid, idx);
        }
        #endregion

        #region IEquatable<CommandReference> Members
        public bool Equals(CommandReference other)
        {
            return _index.Equals(other._index) && _instanceId.Equals(other._instanceId);
        }
        public override bool Equals(object obj)
        {
            if(!(obj is CommandReference))
                throw new ArgumentException();
            return Equals((CommandReference)obj);
        }
        public static bool operator==(CommandReference a, CommandReference b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(CommandReference a, CommandReference b)
        {
            return !a.Equals(b);
        }
        #endregion
    }
}
