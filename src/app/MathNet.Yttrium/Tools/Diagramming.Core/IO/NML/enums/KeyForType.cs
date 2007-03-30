
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    /// <summary>
    /// For key-types
    /// </summary>
    public enum KeyForType {
        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(Name="all")]
        All,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="graph")]
        Graph,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="Shape")]
        Node,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="Connection")]
        Edge,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="hyperedge")]
        HyperEdge,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="port")]
        Port,
		/// <summary>
		/// 
		/// </summary>
        [XmlEnum(Name="endpoint")]
        EndPoint,
    }
}
