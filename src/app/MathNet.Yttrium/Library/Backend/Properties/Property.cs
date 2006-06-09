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

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Properties
{
    public abstract class Property : IEquatable<Property>
    {
        public abstract MathIdentifier PropertyId { get;}

        public abstract bool StillValidAfterEvent(Signal signal);
        public abstract bool StillValidAfterDrive(Signal signal);
        public abstract bool StillValidAfterUndrive(Signal signal);

        #region Equality
        public abstract bool Equals(Property other);
        public override bool Equals(object obj)
        {
            Property vs = obj as Property;
            if(vs != null)
                return Equals(vs);
            return false;
        }

        public bool EqualsById(Property other)
        {
            return other != null && PropertyId.Equals(other.PropertyId);
        }

        public bool EqualsById(MathIdentifier otherPropertyId)
        {
            return PropertyId.Equals(otherPropertyId);
        }
        #endregion

        public override string ToString()
        {
            return PropertyId.ToString();
        }

        #region Serialization
        protected abstract void InnerSerialize(XmlWriter writer);
        public static void Serialize(XmlWriter writer, Property property)
        {
            if(property == null) throw new ArgumentNullException("property");
            if(writer == null) throw new ArgumentNullException("writer");
            writer.WriteStartElement("Property");
            writer.WriteAttributeString("type", property.GetType().AssemblyQualifiedName);
            property.InnerSerialize(writer);
            writer.WriteEndElement();
        }
        public static Property Deserialize(Context context, XmlReader reader)
        {
            if(reader == null) throw new ArgumentNullException("reader");
            reader.ReadToDescendant("Property");
            Type type = Type.GetType(reader.GetAttribute("type"), true);
            reader.Read();
            MethodInfo mi = type.GetMethod("InnerDeserialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            Property ret = (Property)mi.Invoke(null, new object[] { context, reader });
            return ret;
        }
        #endregion
    }
}
