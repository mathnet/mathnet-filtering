
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
namespace Netron.GraphLib.IO.NML {    
    
    /// <summary>
    /// Generic XML wrapper for any diagram data
    /// </summary>
    [XmlType(IncludeInSchema=false, TypeName="aaaaaaa")]
    [XmlRoot(ElementName="property", IsNullable=false, DataType="")]
    public class DataType {

		#region Fields
		/// <summary>
		/// data collection
		/// </summary>
        private DataCollection mValue = new DataCollection();
		/// <summary>
		/// the key
		/// </summary>
        private string mName;
		/// <summary>
		/// the mIsCollection
		/// </summary>
        private bool mIsCollection;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the key of the data type
		/// </summary>
        [XmlAttribute(AttributeName="Name")]
        public virtual string Name {
            get {
                return this.mName;
            }
            set {
                this.mName = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the mIsCollection
        /// </summary>
        [XmlAttribute(AttributeName="IsCollection")]
        public virtual bool IsCollection {
            get {
                return this.mIsCollection;
            }
            set {
                this.mIsCollection = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        
		[XmlElement(ElementName="Collection", Type=typeof(DataCollection), IsNullable=false)]	
		[XmlElement(ElementName="string", Type=typeof(string), IsNullable=false)]	
		[XmlElement(ElementName="Data", Type=typeof(DataType), IsNullable=false)]	
		public virtual DataCollection Value {
            get {
                return this.mValue;
            }
            set {
                this.mValue = value;
            }
        }
		#endregion        

		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public DataType(string key, string value)
		{
			mName = key;
			mValue.Add(value);
		}
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public DataType(){}
		#endregion

		#endregion

    }
}
