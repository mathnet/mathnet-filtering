    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    namespace Netron.GraphLib.IO.GraphML {
	/// <summary>
	/// 
	/// </summary>
    [XmlType(IncludeInSchema=true, TypeName="data-extension.type")]
    [XmlRoot(ElementName="dataextensiontype")]
    public class DataExtensionType {
        
		private DataCollection _text = new DataCollection();
        
        [XmlText(Type=typeof(string))]
        public virtual DataCollection Text {
            get {
                return this._text;
            }
            set {
                this._text = value;
            }
        }
        
    
    }
}
