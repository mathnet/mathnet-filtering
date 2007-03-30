using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Schema;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Complementary partial class related to (de)serialization.
    /// </summary>
   [Serializable]
    public partial class Model : ISerializable, IXmlSerializable, IDeserializationCallback
    {
        #region Deserialization constructor
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Model(SerializationInfo info, StreamingContext context)
        {
            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("Deserializing the fields of 'Model'.");
            mPages = info.GetValue("Pages", typeof(CollectionBase<IPage>)) as CollectionBase<IPage>;
        }
        #endregion

        #region Serialization events
       /*
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("Starting to serializing the 'Model' class...");
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
           
            Trace.WriteLine("...serialization of 'Model' finished");
            Trace.Unindent();
        }
        */
        #endregion

        #region Deserialization events
       
        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
          if(Tracing.BinaryDeserializationSwitch.Enabled)
            Trace.WriteLine("Starting deserializing the 'Model' class...");
            //the anchors is a temporary collection of Uid's and parent entities to re-connect connectors
            //to their parent. The serialization process does serialize parenting because you need on deserialization an
            //instance in order to connect to it.

            Anchors.Clear();
        }
      
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
         {
             if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("...deserialization of 'Model' finished");
         }
   
        #endregion

        #region Serialization
         /// <summary>
         /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
         /// </summary>
         /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
         /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
         /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(Tracing.BinarySerializationSwitch.Enabled)
                Trace.WriteLine("Serializing the fields of 'Model'.");
            info.AddValue("Pages", this.Pages, typeof(CollectionBase<IPage>));
        }
        #endregion

        #region Xml serialization
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public void OnDeserialization(object sender)
        {
           
            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("IDeserializationCallback of 'Model' called.");
            Init();
            #region Binding of connectors

            Dictionary<Guid, Anchor>.Enumerator enumer = Anchors.GetEnumerator();
            System.Collections.Generic.KeyValuePair<Guid, Anchor> pair;
            Anchor anchor;
            while(enumer.MoveNext())
            {
                pair =  enumer.Current;
                anchor = pair.Value;
                if(anchor.Parent!=Guid.Empty) //there's a parent connector
                {
                    if(Anchors.ContainsKey(anchor.Parent))
                    {
                        Anchors.GetAnchor(anchor.Parent).Instance.AttachConnector(anchor.Instance);
                    }
                }
            }
            //clean up the anchoring matrix
            Anchors.Clear();
            #endregion
        }

        #endregion
    }
}
