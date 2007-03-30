
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.GraphML {    
    
    [XmlType(IncludeInSchema=true, TypeName="hyperedge.type")]
    [XmlRoot(ElementName="hyperedge", IsNullable=false, DataType="")]
    public class HyperEdgeType {

        private DataCollection _items = new DataCollection();
        private string _desc;
        private string id;
        private GraphType _graph;
        
        [XmlElement(ElementName="desc")]
        public virtual string Desc {
            get {
                return this._desc;
            }
            set {
                this._desc = value;
            }
        }
        
        
        
        [XmlElement(ElementName="data", Type=typeof(DataType))]
        [XmlElement(ElementName="endpoint", Type=typeof(EndPointType))]
        public virtual DataCollection Items {
            get {
                return this._items;
            }
            set {
                this._items = value;
            }
        }
        
        
        
        [XmlElement(ElementName="graph")]
        public virtual GraphType Graph {
            get {
                return this._graph;
            }
            set {
                this._graph = value;
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
        

    }
}
