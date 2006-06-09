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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Properties;

namespace MathNet.Symbolics.Backend.SystemBuilder
{
    /// <summary>
    /// intermediate property description.
    /// </summary>
    [Serializable]
    public sealed class PropertyPack : ISerializable, IXmlSerializable
    {
        private string _serializedXmlFragment;

        private PropertyPack(string serializedXml)
        {
            _serializedXmlFragment = serializedXml;
        }

        public string SerializedXmlFragment
        {
            get { return _serializedXmlFragment; }
            //set { _serializedXml = value; }
        }

        public static PropertyPack Pack(Property property, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Context.DefaultEncoding;
            settings.Indent = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(sb, settings);
            Property.Serialize(writer, property);
            writer.Flush();
            writer.Close();
            return new PropertyPack(sb.ToString());
        }

        public static PropertyPack Repack(string serializedXml, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder(serializedXml);
            foreach(KeyValuePair<Guid, Guid> pair in signalMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            foreach(KeyValuePair<Guid, Guid> pair in busMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            return new PropertyPack(sb.ToString());
        }

        public Property Unpack(Context context, Dictionary<Guid, Signal> signals, Dictionary<Guid, Bus> buses)
        {
            StringReader sr = new StringReader(_serializedXmlFragment);
            XmlReader reader = XmlReader.Create(sr);
            return Property.Deserialize(context, reader);
        }

        #region Serialization
        private PropertyPack()
        {
        }
        private PropertyPack(SerializationInfo info, StreamingContext context)
        {
            _serializedXmlFragment = info.GetString("xml");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("xml", _serializedXmlFragment);
        }
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing("PropertyPack", Context.YttriumNamespace);
            _serializedXmlFragment = reader.ReadInnerXml();
            reader.ReadEndElement();
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PropertyPack", Context.YttriumNamespace);
            writer.WriteRaw(_serializedXmlFragment);
            writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// intermediate value structure description.
    /// </summary>
    [Serializable]
    public sealed class StructurePack : ISerializable, IXmlSerializable
    {
        private string _serializedXmlFragment;

        private StructurePack(string serializedXml)
        {
            _serializedXmlFragment = serializedXml;
        }

        public string SerializedXmlFragment
        {
            get { return _serializedXmlFragment; }
            //set { _serializedXml = value; }
        }

        public static StructurePack Pack(ValueStructure structure, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Context.DefaultEncoding;
            settings.Indent = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(sb, settings);
            ValueStructure.Serialize(writer, structure);
            writer.Flush();
            writer.Close();
            return new StructurePack(sb.ToString());
        }

        public static StructurePack Repack(string serializedXml, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder(serializedXml);
            foreach(KeyValuePair<Guid, Guid> pair in signalMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            foreach(KeyValuePair<Guid, Guid> pair in busMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            return new StructurePack(sb.ToString());
        }

        public ValueStructure Unpack(Context context, Dictionary<Guid, Signal> signals, Dictionary<Guid, Bus> buses)
        {
            StringReader sr = new StringReader(_serializedXmlFragment);
            XmlReader reader = XmlReader.Create(sr);
            return ValueStructure.Deserialize(context, reader);
        }

        #region Serialization
        private StructurePack()
        {
        }
        private StructurePack(SerializationInfo info, StreamingContext context)
        {
            _serializedXmlFragment = info.GetString("xml");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("xml", _serializedXmlFragment);
        }
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing("StructurePack", Context.YttriumNamespace);
            _serializedXmlFragment = reader.ReadInnerXml();
            reader.ReadEndElement();
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("StructurePack", Context.YttriumNamespace);
            writer.WriteRaw(_serializedXmlFragment);
            writer.WriteEndElement();
        }
        #endregion
    }
}
