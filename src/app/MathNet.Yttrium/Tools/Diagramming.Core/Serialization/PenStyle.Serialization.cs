using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Schema;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Complementary partial class related to (de)serialization.
    /// </summary>
   [Serializable]
    public partial class PenStyle : ISerializable, IXmlSerializable, IDeserializationCallback
    {
        #region Deserialization constructor
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected  PenStyle(SerializationInfo info, StreamingContext context)
           
        {

            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("Deserializing the fields of 'PenStyle'.");
            mWidth = info.GetSingle("Width");
            mColor = (Color) info.GetValue("Color", typeof(Color));
            mDashStyle = (System.Drawing.Drawing2D.DashStyle)Enum.Parse(typeof(System.Drawing.Drawing2D.DashStyle), info.GetString("DashStyle"));
        }
        #endregion

        #region Serialization events
       /*
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            Trace.WriteLine("Starting to serializing the 'PenStyle' class...");
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            Trace.WriteLine("...serialization of 'PenStyle' finished");
        }
        */
        #endregion

        #region Deserialization events
       /*
        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("Starting deserializing the 'PenStyle' class...");
        }
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
         {
             Trace.WriteLine("...deserialization of 'PenStyle' finished");
             Trace.Unindent();
        }
       */
        #endregion

        #region Serialization
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual  void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(Tracing.BinarySerializationSwitch.Enabled)
                Trace.WriteLine("Serializing the fields of 'PenStyle'.");
      
            info.AddValue("Color", this.mColor, typeof(Color));
            info.AddValue("Width", this.mWidth);
            info.AddValue("DashStyle", this.mDashStyle.ToString());
        }
        #endregion

        #region Xml serialization
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public  virtual XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public  virtual void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public virtual  void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public virtual void OnDeserialization(object sender)
        {
            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("IDeserializationCallback of 'PenStyle' called.");
        }
    }
}
