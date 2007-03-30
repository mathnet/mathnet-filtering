
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.GraphML {    
    
    public enum EndPointTypeType {
        
        
        
        [XmlEnum(Name="in")]
        IN,
        
        
        
        [XmlEnum(Name="out")]
        Out,
        
        
        
        [XmlEnum(Name="undir")]
        Undir,
    }
}
