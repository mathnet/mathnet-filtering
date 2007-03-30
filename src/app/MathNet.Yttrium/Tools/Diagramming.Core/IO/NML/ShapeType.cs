
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
    
    
namespace Netron.GraphLib.IO.NML 
{    
    /// <summary>
    /// XML wrapper for a shape object
    /// </summary>
	[XmlType(IncludeInSchema=true, TypeName="node.type")]
	[XmlRoot(ElementName="Shape", IsNullable=false, DataType="")]
	public class ShapeType 
	{
		#region Fields
		/// <summary>
		/// data collection
		/// </summary>
		private DataCollection mData = new DataCollection();
		/// <summary>
		/// the description
		/// </summary>
		private string mDescription;
		/// <summary>
		/// the mUID
		/// </summary>
		private string mUID;
		/// <summary>
		/// the unique key to the shape to be instantiated
		/// </summary>
		private string mInstanceKey = string.Empty;
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the key to use when instantiating/deserializing the node again
        /// </summary>
		[XmlElement(ElementName="InstanceKey")]
		public string InstanceKey
		{
			get{return mInstanceKey;}
			set{mInstanceKey = value;}
		}
		/// <summary>
		/// Gets or sets the description
		/// </summary>
		[XmlElement(ElementName="desc")]
		public virtual string Desc 
		{
			get 
			{
				return this.mDescription;
			}
			set 
			{
				this.mDescription = value;
			}
		}
        
		/// <summary>
		/// Gets or sets the data collection
		/// </summary>
		[XmlElement(ElementName="locator", Type=typeof(LocatorType))]
		[XmlElement(ElementName="graph", Type=typeof(GraphType))]
		[XmlElement(ElementName="property", Type=typeof(DataType))]
		[XmlElement(ElementName="port", Type=typeof(PortType))]
		[XmlElement(ElementName="connector", Type=typeof(ConnectorType))]		
		public virtual DataCollection Data 
		{
			get 
			{
				return this.mData;
			}
			set 
			{
				this.mData = value;
			}
		}
        /// <summary>
        /// Gets or sets the mUID
        /// </summary>
		[XmlElement(ElementName="UID")]
		public virtual string UID 
		{
			get 
			{
				return this.mUID;
			}
			set 
			{
				this.mUID = value;
			}
		}
		#endregion
	}
}

