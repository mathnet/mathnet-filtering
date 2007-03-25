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
using System.Xml;
using System.Reflection;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Utils;

namespace MathNet.Symbolics.Core
{
    public abstract class ValueStructure : IValueStructure
    {
        public abstract MathIdentifier TypeId { get;}

        #region Equality
        public abstract bool Equals(IValueStructure other);
        public override bool Equals(object obj)
        {
            ValueStructure vs = obj as ValueStructure;
            if(vs != null)
                return Equals(vs);
            return false;
        }

        public bool EqualsById(IValueStructure other)
        {
            return other != null && TypeId.Equals(other.TypeId);
        }
        public bool EqualsById(MathIdentifier otherStructureId)
        {
            return TypeId.Equals(otherStructureId);
        }
        #endregion

        public override string ToString()
        {
            return TypeId.ToString();
        }

        #region Serialization
        //protected abstract void InnerSerialize(XmlWriter writer);
        //public static void Serialize(XmlWriter writer, ValueStructure value)
        //{
        //    if(value == null) throw new ArgumentNullException("value");
        //    if(writer == null) throw new ArgumentNullException("writer");
        //    writer.WriteStartElement("Structure");
        //    writer.WriteAttributeString("type", value.GetType().AssemblyQualifiedName);
        //    value.InnerSerialize(writer);
        //    writer.WriteEndElement();
        //}
        //public static ValueStructure Deserialize(IContext context, XmlReader reader)
        //{
        //    if(reader == null) throw new ArgumentNullException("reader");
        //    reader.ReadToDescendant("Structure");
        //    Type type = Type.GetType(reader.GetAttribute("type"), true);
        //    reader.Read();
        //    MethodInfo mi = type.GetMethod("InnerDeserialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        //    ValueStructure ret = (ValueStructure)mi.Invoke(null, new object[] { context, reader });
        //    return ret;
        //}
        #endregion

        #region ICustomData Members
        public virtual void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
        }
        //private static ValueStructure Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses);

        public virtual bool ReferencesCoreObjects
        {
            get { return false; }
        }

        public virtual IEnumerable<MathNet.Symbolics.Signal> CollectSignals()
        {
            return SingletonProvider<EmptyIterator<MathNet.Symbolics.Signal>>.Instance;
        }
        public virtual IEnumerable<MathNet.Symbolics.Bus> CollectBuses()
        {
            return SingletonProvider<EmptyIterator<MathNet.Symbolics.Bus>>.Instance;
        }
        #endregion
    }
}
