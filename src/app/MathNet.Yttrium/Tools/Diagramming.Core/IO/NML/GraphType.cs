
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
    
namespace Netron.GraphLib.IO.NML
{    
    
   /// <summary>
   /// XML wrapper for a graph
   /// </summary>
    [XmlRoot(ElementName="Graph", IsNullable=false, DataType="")]
    public class GraphType {
		#region Fields
		/// <summary>
		/// the items
		/// </summary>
        private DataCollection mItems = new DataCollection();		

		private GraphInformationType mGraphInformation;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the graph-info of the graph
		/// </summary>
		[XmlElement(ElementName="GraphInformation", Type=typeof(GraphInformationType))]
		public GraphInformationType GraphInformation
		{
			get{return mGraphInformation;}
			set{mGraphInformation = value;}
		}
        
        /// <summary>
        /// Gets or sets the item collection
        /// </summary>
        [XmlElement(ElementName="locator", Type=typeof(LocatorType))]
        [XmlElement(ElementName="Connection", Type=typeof(ConnectionType))]
        [XmlElement(ElementName="Shape", Type=typeof(ShapeType))]
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

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public GraphType()
		{}
		#endregion

    }
}
