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
    public partial class ClassShape : ISerializable, IXmlSerializable, IDeserializationCallback
    {
        #region Deserialization constructor
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ClassShape(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            
            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("Deserializing the fields of 'ClassShape'.");

            this.Resizable = false;
            this.mTitle = info.GetString("Title");
            this.mSubTitle = info.GetString("SubTitle");
            sf.Trimming = StringTrimming.EllipsisCharacter;
            this.textMaterial = info.GetValue("TextMaterial", typeof(LabelMaterial)) as LabelMaterial;
            mFolders = new CollectionBase<FolderMaterial>();
            this.Resizable = false;
            this.mCollapsed = info.GetBoolean("Collapsed");
            this.mBodyType = (BodyType) Enum.Parse(typeof(BodyType), info.GetString("BodyType"));
            
        }
        #endregion

        #region Serialization events
       /*
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            Trace.WriteLine("Starting to serializing the 'ClassShape' class...");
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            Trace.WriteLine("...serialization of 'ClassShape' finished");
        }
        */
        #endregion

        #region Deserialization events
       /*
        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("Starting deserializing the 'ClassShape' class...");
        }
        */
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
         {
             if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("...deserialization of 'ClassShape' finished");
            
             #region Loop over the children and make re-attach the events
            foreach(IShapeMaterial material in Children)
             {
                 //set the parent
                material.Shape = this;

                 if(typeof(FolderMaterial).IsInstanceOfType(material))
                 {
                     (material as FolderMaterial).OnFolderChanged += new EventHandler<RectangleEventArgs>(folders_OnFolderChanged);
                     mFolders.Add(material as FolderMaterial);
                     bodyHeight += material.Rectangle.Height + 3;
                 }
                 else if(typeof(SwitchIconMaterial).IsInstanceOfType(material))
                 {
                     xicon = material as SwitchIconMaterial;
                     xicon.Collapsed = mCollapsed;
                     (material as SwitchIconMaterial).OnExpand += new EventHandler(xicon_OnExpand);
                     (material as SwitchIconMaterial).OnCollapse += new EventHandler(xicon_OnCollapse);
                 }
             }
            #endregion

             #region Set the initial state
             if(mCollapsed)
             {
                 Collapse();
             }
             else
             {
                 Expand();
             }
             #endregion

             #region Update the body, this will set the bodytype for example
             UpdateBody();
             #endregion

             #region Finally, position everything
             Transform(Rectangle);
             #endregion
         }
       
        #endregion

        #region Serialization
         /// <summary>
         /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
         /// </summary>
         /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
         /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
         /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
         public override void  GetObjectData(SerializationInfo info, StreamingContext context)
        {

 	        base.GetObjectData(info, context);
            if (Tracing.BinarySerializationSwitch.Enabled)
                Trace.WriteLine("Serializing the fields of 'ClassShape'.");

            info.AddValue("Title", this.Title);
            info.AddValue("SubTitle", this.SubTitle);
            info.AddValue("TextMaterial", this.textMaterial, typeof(LabelMaterial));
            info.AddValue("Collapsed", mCollapsed);
            info.AddValue("BodyType", mBodyType.ToString());
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
            
            if(Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("IDeserializationCallback of 'ClassShape' called.");
        }
    }
}
