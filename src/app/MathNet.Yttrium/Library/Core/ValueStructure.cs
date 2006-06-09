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
using System.Xml;
using System.Reflection;

using MathNet.Symbolics.Backend;

namespace MathNet.Symbolics.Core
{
    public abstract class ValueStructure : IEquatable<ValueStructure>
    {
        public abstract MathIdentifier StructureId { get;}

        #region Equality
        public abstract bool Equals(ValueStructure other);
        public override bool Equals(object obj)
        {
            ValueStructure vs = obj as ValueStructure;
            if(vs != null)
                return Equals(vs);
            return false;
        }

        public bool EqualsById(ValueStructure other)
        {
            return other != null && StructureId.Equals(other.StructureId);
        }
        public bool EqualsById(MathIdentifier otherStructureId)
        {
            return StructureId.Equals(otherStructureId);
        }
        #endregion

        public override string ToString()
        {
            return StructureId.ToString();
        }

        #region Serialization
        protected abstract void InnerSerialize(XmlWriter writer);
        public static void Serialize(XmlWriter writer, ValueStructure value)
        {
            if(value == null) throw new ArgumentNullException("value");
            if(writer == null) throw new ArgumentNullException("writer");
            writer.WriteStartElement("Structure");
            writer.WriteAttributeString("type", value.GetType().AssemblyQualifiedName);
            value.InnerSerialize(writer);
            writer.WriteEndElement();
        }
        public static ValueStructure Deserialize(Context context, XmlReader reader)
        {
            if(reader == null) throw new ArgumentNullException("reader");
            reader.ReadToDescendant("Structure");
            Type type = Type.GetType(reader.GetAttribute("type"), true);
            reader.Read();
            MethodInfo mi = type.GetMethod("InnerDeserialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            ValueStructure ret = (ValueStructure)mi.Invoke(null, new object[] { context, reader });
            return ret;
        }
        #endregion
    }
}
