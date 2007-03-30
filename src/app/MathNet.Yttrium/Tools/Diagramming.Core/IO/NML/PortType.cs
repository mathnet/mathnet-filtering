
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    
	/// <summary>
	/// The NML template corresponding to the XML of a connector
	/// </summary>
    [XmlType(IncludeInSchema=true, TypeName="port.type")]
    [XmlRoot(ElementName="port", IsNullable=false, DataType="")]
    public class PortType {
		#region Fields
		/// <summary>
		/// the data items
		/// </summary>
        private DataCollection mItems = new DataCollection();
		/// <summary>
		/// the name of the port
		/// </summary>
        private string mName;
		/// <summary>
		/// the description of the port
		/// </summary>
        private string mDescription;
		#endregion
        
		#region Properties
        /// <summary>
        /// Gets or sets the description of the port
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
        /// Gets or sets the data items
        /// </summary>
        [XmlElement(ElementName="port", Type=typeof(PortType))]
        [XmlElement(ElementName="data", Type=typeof(DataType))]
        public virtual DataCollection Items {
            get {
                return this.mItems;
            }
            set {
                this.mItems = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the name of the port
        /// </summary>
        [XmlAttribute(AttributeName="name")]
        public virtual string Name {
            get {
                return this.mName;
            }
            set {
                this.mName = value;
            }
        }
		#endregion   
    }
}
