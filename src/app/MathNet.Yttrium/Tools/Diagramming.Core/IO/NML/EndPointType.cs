
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.GraphML {    
    

    [XmlType(IncludeInSchema=true, TypeName="endpoint.type")]
    [XmlRoot(ElementName="endpoint", IsNullable=false, DataType="")]
    public class EndPointType {
		
		#region Fields
		private string _node;
        private string id;
        private string _port;
        private string _desc;
        private EndPointTypeType _type;
		#endregion

		#region Properties
        [XmlElement(ElementName="desc")]
        public virtual string Desc {
            get {
                return this._desc;
            }
            set {
                this._desc = value;
            }
        }
        
        
        
        [XmlAttribute(AttributeName="id")]
        public virtual string ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
            }
        }
        
        
        
        [XmlAttribute(AttributeName="port")]
        public virtual string Port {
            get {
                return this._port;
            }
            set {
                this._port = value;
            }
        }
        
        
        
        [XmlAttribute(AttributeName="node")]
        public virtual string Node {
            get {
                return this._node;
            }
            set {
                this._node = value;
            }
        }
        
        
        
        [XmlAttribute(AttributeName="type")]
        public virtual EndPointTypeType Type {
            get {
                return this._type;
            }
            set {
                this._type = value;
            }
        }
		#endregion
    }
}
