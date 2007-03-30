    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    namespace Netron.GraphLib.IO.NML {
    
	/// <summary>
	/// The base-template class for the NML serialization,
	/// this class corresponds to the root of the XML
	/// </summary>
    [XmlRoot(ElementName="NML", IsNullable=false, DataType="")]
    public class NMLType {
        
		#region Fields
        
        /// <summary>
        /// the keys
        /// </summary>
        private DataCollection mKeys = new DataCollection();
		/// <summary>
		/// the graph node
		/// </summary>
		private GraphType mGraph;
		/// <summary>
		/// the graphlib version
		/// </summary>
		private string mVersion;
        /// <summary>
        /// the data items
        /// </summary>
        private DataCollection mItems = new DataCollection();
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the NML version
        /// </summary>
		[XmlElement("Version",typeof(string))]
		public string Version
		{
			get{return mVersion;}
			set{mVersion = value;}
		}
		/// <summary>
		/// Gets or sets the serialized graph
		/// </summary>
		[XmlElement("Graph",typeof(GraphType))]
		public GraphType Graph
		{
			get{return mGraph;}
			set{mGraph = value;}
		}
        
        /// <summary>
        /// Gets or sets the key-collection
        /// </summary>
        [XmlElement(ElementName="key", Type=typeof(KeyType))]
        public virtual DataCollection Key {
            get {
                return this.mKeys;
            }
            set {
                this.mKeys = value;
            }
        }
        
        
        /// <summary>
        /// Gets or sets the data collection for the graph
        /// </summary>        
        [XmlElement(ElementName="data", Type=typeof(DataType))]
        public virtual DataCollection Items {
            get {
                return this.mItems;
            }
            set {
                this.mItems = value;
            }
        }
		#endregion

		
    
    
    }
}
