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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using MathNet.Symbolics.Backend.Persistence;

namespace MathNet.Symbolics.SystemBuilder.Toolkit
{
    /// <summary>
    /// intermediate custom data description.
    /// </summary>
    [Serializable]
    public sealed class CustomDataPack<T> : ICustomDataPack<T>, ISerializable, IXmlSerializable where T : ICustomData
    {
        private string _serializedXmlFragment;

        private CustomDataPack(string serializedXml)
        {
            _serializedXmlFragment = serializedXml;
        }

        public string SerializedXmlFragment
        {
            get { return _serializedXmlFragment; }
            //set { _serializedXml = value; }
        }

        public static CustomDataPack<T> Pack(T property, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            string xml = Serializer.SerializeToString(property, signalMappings, busMappings);
            return new CustomDataPack<T>(xml);
        }

        public static CustomDataPack<T> Repack(string serializedXml, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            StringBuilder sb = new StringBuilder(serializedXml);
            foreach(KeyValuePair<Guid, Guid> pair in signalMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            foreach(KeyValuePair<Guid, Guid> pair in busMappings)
                sb.Replace(pair.Key.ToString(), pair.Value.ToString());
            return new CustomDataPack<T>(sb.ToString());
        }

        public T Unpack(IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return Serializer.DeserializeFromString<T>(_serializedXmlFragment, signals, buses);
        }

        #region Serialization
        private CustomDataPack()
        {
        }
        private CustomDataPack(SerializationInfo info, StreamingContext context)
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
            reader.ReadToFollowing("CustomDataPack", Config.YttriumNamespace);
            _serializedXmlFragment = reader.ReadInnerXml();
            reader.ReadEndElement();
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("CustomDataPack", Config.YttriumNamespace);
            writer.WriteRaw(_serializedXmlFragment);
            writer.WriteEndElement();
        }
        #endregion
    }

    ///// <summary>
    ///// intermediate property description.
    ///// </summary>
    //[Serializable]
    //[Obsolete("Use generic CustomDataPack instead")]
    //public sealed class PropertyPack : ISerializable, IXmlSerializable
    //{
    //    private string _serializedXmlFragment;

    //    private PropertyPack(string serializedXml)
    //    {
    //        _serializedXmlFragment = serializedXml;
    //    }

    //    public string SerializedXmlFragment
    //    {
    //        get { return _serializedXmlFragment; }
    //        //set { _serializedXml = value; }
    //    }

    //    public static PropertyPack Pack(IProperty property, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
    //    {
    //        string xml = Serializer.SerializeToString(property, signalMappings, busMappings);
    //        return new PropertyPack(xml);

    //        //StringBuilder sb = new StringBuilder();
    //        //XmlWriterSettings settings = new XmlWriterSettings();
    //        //settings.Encoding = Context.DefaultEncoding;
    //        //settings.Indent = false;
    //        //settings.ConformanceLevel = ConformanceLevel.Fragment;
    //        //settings.NewLineHandling = NewLineHandling.Entitize;
    //        //settings.OmitXmlDeclaration = true;
    //        //XmlWriter writer = XmlWriter.Create(sb, settings);
    //        //Property.Serialize(writer, property);
    //        //writer.Flush();
    //        //writer.Close();
    //        //return new PropertyPack(sb.ToString());
    //    }

    //    public static PropertyPack Repack(string serializedXml, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
    //    {
    //        StringBuilder sb = new StringBuilder(serializedXml);
    //        foreach(KeyValuePair<Guid, Guid> pair in signalMappings)
    //            sb.Replace(pair.Key.ToString(), pair.Value.ToString());
    //        foreach(KeyValuePair<Guid, Guid> pair in busMappings)
    //            sb.Replace(pair.Key.ToString(), pair.Value.ToString());
    //        return new PropertyPack(sb.ToString());
    //    }

    //    public IProperty Unpack(IContext context, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
    //    {
    //        return Serializer.DeserializeFromString<IProperty>(_serializedXmlFragment, context, signals, buses);
            
    //        //StringReader sr = new StringReader(_serializedXmlFragment);
    //        //XmlReader reader = XmlReader.Create(sr);
    //        //return Property.Deserialize(context, reader);
    //    }

    //    #region Serialization
    //    private PropertyPack()
    //    {
    //    }
    //    private PropertyPack(SerializationInfo info, StreamingContext context)
    //    {
    //        _serializedXmlFragment = info.GetString("xml");
    //    }
    //    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    //    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        info.AddValue("xml", _serializedXmlFragment);
    //    }
    //    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    //    {
    //        throw new NotImplementedException();
    //    }
    //    void IXmlSerializable.ReadXml(XmlReader reader)
    //    {
    //        reader.ReadToFollowing("PropertyPack", Context.YttriumNamespace);
    //        _serializedXmlFragment = reader.ReadInnerXml();
    //        reader.ReadEndElement();
    //    }
    //    void IXmlSerializable.WriteXml(XmlWriter writer)
    //    {
    //        writer.WriteStartElement("PropertyPack", Context.YttriumNamespace);
    //        writer.WriteRaw(_serializedXmlFragment);
    //        writer.WriteEndElement();
    //    }
    //    #endregion
    //}

    ///// <summary>
    ///// intermediate value structure description.
    ///// </summary>
    //[Serializable]
    //[Obsolete("Use generic CustomDataPack instead")]
    //public sealed class StructurePack : ISerializable, IXmlSerializable
    //{
    //    private string _serializedXmlFragment;

    //    private StructurePack(string serializedXml)
    //    {
    //        _serializedXmlFragment = serializedXml;
    //    }

    //    public string SerializedXmlFragment
    //    {
    //        get { return _serializedXmlFragment; }
    //        //set { _serializedXml = value; }
    //    }

    //    public static StructurePack Pack(IValueStructure structure, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
    //    {
    //        string xml = Serializer.SerializeToString(structure, signalMappings, busMappings);
    //        return new StructurePack(xml);

    //        //StringBuilder sb = new StringBuilder();
    //        //XmlWriterSettings settings = new XmlWriterSettings();
    //        //settings.Encoding = Context.DefaultEncoding;
    //        //settings.Indent = false;
    //        //settings.ConformanceLevel = ConformanceLevel.Fragment;
    //        //settings.NewLineHandling = NewLineHandling.Entitize;
    //        //settings.OmitXmlDeclaration = true;
    //        //XmlWriter writer = XmlWriter.Create(sb, settings);
    //        //ValueStructure.Serialize(writer, structure);
    //        //writer.Flush();
    //        //writer.Close();
    //        //return new StructurePack(sb.ToString());
    //    }

    //    public static StructurePack Repack(string serializedXml, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
    //    {
    //        StringBuilder sb = new StringBuilder(serializedXml);
    //        foreach(KeyValuePair<Guid, Guid> pair in signalMappings)
    //            sb.Replace(pair.Key.ToString(), pair.Value.ToString());
    //        foreach(KeyValuePair<Guid, Guid> pair in busMappings)
    //            sb.Replace(pair.Key.ToString(), pair.Value.ToString());
    //        return new StructurePack(sb.ToString());
    //    }

    //    public IValueStructure Unpack(IContext context, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
    //    {
    //        return Serializer.DeserializeFromString<IValueStructure>(_serializedXmlFragment, context, signals, buses);

    //        //StringReader sr = new StringReader(_serializedXmlFragment);
    //        //XmlReader reader = XmlReader.Create(sr);
    //        //return ValueStructure.Deserialize(context, reader);
    //    }

    //    #region Serialization
    //    private StructurePack()
    //    {
    //    }
    //    private StructurePack(SerializationInfo info, StreamingContext context)
    //    {
    //        _serializedXmlFragment = info.GetString("xml");
    //    }
    //    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    //    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        info.AddValue("xml", _serializedXmlFragment);
    //    }
    //    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    //    {
    //        throw new NotImplementedException();
    //    }
    //    void IXmlSerializable.ReadXml(XmlReader reader)
    //    {
    //        reader.ReadToFollowing("StructurePack", Context.YttriumNamespace);
    //        _serializedXmlFragment = reader.ReadInnerXml();
    //        reader.ReadEndElement();
    //    }
    //    void IXmlSerializable.WriteXml(XmlWriter writer)
    //    {
    //        writer.WriteStartElement("StructurePack", Context.YttriumNamespace);
    //        writer.WriteRaw(_serializedXmlFragment);
    //        writer.WriteEndElement();
    //    }
    //    #endregion
    //}
}
