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
using System.Reflection;
using System.IO;

namespace MathNet.Symbolics.Backend.Persistence
{
    public static class Serializer
    {
        // TODO: Consider using the CustomDataRefs in the Library, so we don't have to relay on GetType-reflection any more.

        #region Single CustomData <-> XmlWriter/Reader
        public static void Serialize(ICustomData value, XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            if(value == null) throw new ArgumentNullException("value");
            if(writer == null) throw new ArgumentNullException("writer");
            if(signalMappings == null) throw new ArgumentNullException("signalMappings");
            if(busMappings == null) throw new ArgumentNullException("busMappings");

            writer.WriteStartElement("CustomData");
            writer.WriteAttributeString("type", TypeToDescription(value.GetType()));
            value.Serialize(writer, signalMappings, busMappings);
            writer.WriteEndElement();
        }

        public static T Deserialize<T>(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses) where T : ICustomData
        {
            if(reader == null) throw new ArgumentNullException("reader");
            if(signals == null) throw new ArgumentNullException("signals");
            if(buses == null) throw new ArgumentNullException("buses");

            if(!reader.IsStartElement("CustomData"))
                reader.ReadToDescendant("CustomData");
            Type type = DescriptionToType(reader.GetAttribute("type"));
            reader.Read();

            MethodInfo mi = type.GetMethod("Deserialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            T ret = (T)mi.Invoke(null, new object[] { reader, signals, buses });
            
            if(reader.LocalName=="CustomData" && reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();

            return ret;
        }
        #endregion
        #region Multiple CustomData <-> XmlWriter/Reader
        /// <param name="parentNode">Omitted if null, but required when working with more than one data item directly in the xml root.</param>
        public static void SerializeList<T>(IEnumerable<T> values, XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings, string parentXmlNode) where T : ICustomData
        {
            bool withRoot = !string.IsNullOrEmpty(parentXmlNode);
            if(withRoot)
                writer.WriteStartElement(parentXmlNode);

            foreach(ICustomData cd in values)
                Serialize(cd, writer, signalMappings, busMappings);

            if(withRoot)
                writer.WriteEndElement();
        }

        /// <param name="parentNode">Omitted if null, but required when working with more than one data item directly in the xml root.</param>
        public static List<T> DeserializeList<T>(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses, string parentXmlNode) where T : ICustomData
        {
            List<T> cd = new List<T>();

            bool withRoot = !string.IsNullOrEmpty(parentXmlNode);
            if(withRoot && !reader.IsStartElement(parentXmlNode))
                reader.ReadToDescendant(parentXmlNode);

            while(reader.IsStartElement("CustomData") || reader.ReadToDescendant("CustomData"))
                cd.Add(Deserialize<T>(reader, signals, buses));

            if(withRoot && reader.LocalName == parentXmlNode && reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();

            return cd;
        }
        #endregion
        #region Single CustomData <-> string
        public static string SerializeToString(ICustomData value, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Config.InternalEncoding;
            settings.Indent = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(sb, settings);

            Serialize(value, writer, signalMappings, busMappings);

            writer.Flush();
            writer.Close();
            return sb.ToString();
        }

        public static T DeserializeFromString<T>(string xml, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses) where T : ICustomData
        {
            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);

            T ret = Deserialize<T>(reader, signals, buses);

            reader.Close();
            sr.Close();
            return ret;
        }
        #endregion
        #region Multiple CustomData <-> string
        /// <param name="parentNode">Omitted if null, but required when working with more than one data item directly in the xml root.</param>
        public static string SerializeListToString<T>(IEnumerable<T> values, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings, string parentXmlNode) where T : ICustomData
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Config.InternalEncoding;
            settings.Indent = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(sb, settings);

            SerializeList<T>(values, writer, signalMappings, busMappings, parentXmlNode);
            
            //bool withRoot = !string.IsNullOrEmpty(parentXmlNode);
            //if(withRoot)
            //    writer.WriteStartElement(parentXmlNode);

            //foreach(ICustomData cd in values)
            //    Serialize(cd, writer, signalMappings, busMappings);

            //if(withRoot)
            //    writer.WriteEndElement();

            writer.Flush();
            writer.Close();
            return sb.ToString();
        }

        /// <param name="parentNode">Omitted if null, but required when working with more than one data item directly in the xml root.</param>
        public static List<T> DeserializeListFromString<T>(string xml, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses, string parentXmlNode) where T : ICustomData
        {
            //List<T> cd = new List<T>();

            StringReader sr = new StringReader(xml);
            XmlReader reader = XmlReader.Create(sr);

            List<T> cd = DeserializeList<T>(reader, signals, buses, parentXmlNode);

            //bool withRoot = !string.IsNullOrEmpty(parentXmlNode);
            //if(withRoot && !reader.IsStartElement(parentXmlNode))
            //    reader.ReadToDescendant(parentXmlNode);

            //while(reader.IsStartElement("CustomData") || reader.ReadToDescendant("CustomData"))
            //    cd.Add(Deserialize<T>(context, reader, signals, buses));

            //if(withRoot && reader.LocalName == parentXmlNode && reader.NodeType == XmlNodeType.EndElement)
            //    reader.ReadEndElement();

            return cd;
        }
        #endregion

        public static string TypeToDescription(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");
            return type.AssemblyQualifiedName;
        }

        public static Type DescriptionToType(string description)
        {
            return Type.GetType(description, true);
        }
    }
}
