
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    
	/// <summary>
	/// XML wrapper for a default
	/// </summary>
    [XmlType(IncludeInSchema=true, TypeName="default.type")]
    [XmlRoot(ElementName="default", IsNullable=false, DataType="")]
    public class DefaultType {        
        
		/// <summary>
		/// the text
		/// </summary>
        private DataCollection mText = new DataCollection();
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        [XmlText(Type=typeof(string))]
        public virtual DataCollection Text {
            get {
                return this.mText;
            }
            set {
                this.mText = value;
            }
        }
        

    }
}
