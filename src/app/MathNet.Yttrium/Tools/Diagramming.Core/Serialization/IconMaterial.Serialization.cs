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
    public partial class IconMaterial : ISerializable, IXmlSerializable, IDeserializationCallback
    {
        #region Deserialization constructor
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected IconMaterial(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("Deserializing the fields of 'IconMaterial'.");
            try
            {
                mIcon = info.GetValue("Icon", typeof(Bitmap)) as Bitmap;
            }
            catch
            {      
                //never mind, there was no icon originally in the material
            }

        }
        #endregion

        #region Serialization events
        /*
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("Starting to serializing the 'IconMaterial' class...");
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            Trace.WriteLine("...serialization of 'IconMaterial' finished");
            Trace.Unindent();
        }
        */
        #endregion

        #region Deserialization events
        /*
        [OnDeserializing]      
        void OnDeserializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("IDeserializationCallback of 'IconMaterial' called.");
        }
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
         {
             Trace.WriteLine("...deserialization of 'IconMaterial' finished");
             Trace.Unindent();
         }
       */
        #endregion

        #region Serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Tracing.BinarySerializationSwitch.Enabled)
                Trace.WriteLine("Serializing the fields of 'IconMaterial'.");
            base.GetObjectData(info, context);
            if (mIcon != null)
                info.AddValue("Icon", mIcon, typeof(Bitmap));
        }
        #endregion

        #region Xml serialization
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public override XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion



        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            if (Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("IDeserializationCallback of 'IconMaterial' called.");


        }
    }
}
