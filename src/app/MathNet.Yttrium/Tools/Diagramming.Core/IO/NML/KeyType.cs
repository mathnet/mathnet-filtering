
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    
	/// <summary>
	/// XML wrapper for the key-type
	/// </summary>
    [XmlType(IncludeInSchema=true, TypeName="key.type")]
    [XmlRoot(ElementName="key", IsNullable=false, DataType="")]
    public class KeyType {
		#region Fields
		/// <summary>
		/// the default type
		/// </summary>
        private DefaultType mDefault;
		/// <summary>
		/// the ide
		/// </summary>
        private string id;
		/// <summary>
		/// the description
		/// </summary>
        private string mDescription;
		/// <summary>
		/// the for key-type
		/// </summary>
        private KeyForType mFor;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the description
		/// </summary>
        [XmlElement(ElementName="desc")]
        public virtual string Desc {
            get {
                return this.mDescription;
            }
            set {
                this.mDescription = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the default type
        /// </summary>
        [XmlElement(ElementName="default")]
        public virtual DefaultType Default {
            get {
                return this.mDefault;
            }
            set {
                this.mDefault = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [XmlAttribute(AttributeName="id")]
        public virtual string ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the for key-type
        /// </summary>
        
        [XmlAttribute(AttributeName="for")]
        public virtual KeyForType For {
            get {
                return this.mFor;
            }
            set {
                this.mFor = value;
            }
        }
		#endregion
    }
}
