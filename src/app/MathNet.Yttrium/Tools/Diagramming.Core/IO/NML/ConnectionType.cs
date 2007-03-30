
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML {    
    
	/// <summary>
	/// The XML wrappper of an edge or connection
	/// </summary>
    [XmlType(IncludeInSchema=true, TypeName="edge.type")]
    [XmlRoot(ElementName="Connection", IsNullable=false, DataType="")]
    public class ConnectionType {

		#region Fields
		
		/// <summary>
		/// the data collection
		/// </summary>
        private DataCollection mData = new DataCollection();
		/// <summary>
		/// the source
		/// </summary>
        private string mSource;
		/// <summary>
		/// the mUID
		/// </summary>
        private string mUID;
		/// <summary>
		/// the source-connector
		/// </summary>
        private string mSourcePort;
		/// <summary>
		/// the target-connector
		/// </summary>
        private string mTargetPort;		
		/// <summary>
		/// the target
		/// </summary>
        private string mTarget;
		/// <summary>
		/// the graph
		/// </summary>
        private GraphType mGraph;
		
		/// <summary>
		/// the unique key to the shape to be instantiated
		/// </summary>
		private string mInstanceKey = string.Empty;
		#endregion 

		#region Properties

		/// <summary>
		/// Gets or sets which key to use to instantiate the connection
		/// </summary>
		[XmlElement(ElementName="InstanceKey")]
		public string InstanceKey
		{
			get{return mInstanceKey;}
			set{mInstanceKey = value;}
		}
	
        /// <summary>
        /// Gets or sets the data collection
        /// </summary>
        [XmlElement(ElementName="property", Type=typeof(DataType))]
        public virtual DataCollection Data {
            get {
                return this.mData;
            }
            set {
                this.mData = value;
            }
        }
		/// <summary>
		/// Gets or sets the graph the connection belongs to
		/// </summary>
        [XmlElement(ElementName="graph")]
        public virtual GraphType Graph {
            get {
                return this.mGraph;
            }
            set {
                this.mGraph = value;
            }
        }
		/// <summary>
		/// Gets or sets the mUID
		/// </summary>
        [XmlElement(ElementName="UID")]
        public virtual string ID {
            get {
                return this.mUID;
            }
            set {
                this.mUID = value;
            }
        }        
		
		/// <summary>
		/// Gets or sets the source
		/// </summary>
        [XmlElement(ElementName="source")]
        public virtual string Source {
            get {
                return this.mSource;
            }
            set {
                this.mSource = value;
            }
        }
		/// <summary>
		/// Gets or sets the target
		/// </summary>
        [XmlElement(ElementName="target")]
        public virtual string Target {
            get {
                return this.mTarget;
            }
            set {
                this.mTarget = value;
            }
        }
		/// <summary>
		/// Gets or sets the source-connector
		/// </summary>
        [XmlElement(ElementName="sourceport")]
        public virtual string Sourceport {
            get {
                return this.mSourcePort;
            }
            set {
                this.mSourcePort = value;
            }
        }
        /// <summary>
        /// Gets or sets the target-connector
        /// </summary>
        [XmlElement(ElementName="targetport")]
        public virtual string Targetport {
            get {
                return this.mTargetPort;
            }
            set {
                this.mTargetPort = value;
            }
        }
		#endregion  
   
    }
}
