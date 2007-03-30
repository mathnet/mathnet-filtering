
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    /// <summary>
    /// XML wrapper for the edge type
    /// </summary>
    public enum GraphEdgeDefaultType {
        /// <summary>
        /// Directed edge
        /// </summary>
        [XmlEnum(Name="directed")]
        Directed,
        /// <summary>
        /// Undirected edge
        /// </summary>
        [XmlEnum(Name="undirected")]
        Undirected,
    }
}
