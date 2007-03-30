
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections; 
    
namespace Netron.GraphLib.IO.NML 
{    
    /// <summary>
    /// XML wrapper for a shape object
    /// </summary>
	
	public class ShapeType : IXmlSerializable
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
		
		public string InstanceKey
		{
			get{return mInstanceKey;}
			set{mInstanceKey = value;}
		}
		/// <summary>
		/// Gets or sets the description
		/// </summary>
	
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

		#region IXmlSerializable Members

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("att","123");
			writer.WriteComment("here it comes");
			writer.WriteElementString("InstanceKey",this.mInstanceKey);
			writer.WriteElementString("UID",this.mUID);
			for(int k=0;k<mData.Count;k++)
			{
				if(typeof(ConnectorType).IsInstanceOfType(mData[k]))
				{
					writer.WriteStartElement("Connector");
					writer.WriteAttributeString("UID", (mData[k] as ConnectorType).UID);
					writer.WriteAttributeString("Name", (mData[k] as ConnectorType).Name);
					writer.WriteEndElement();
				}
				else if(typeof(DataType).IsInstanceOfType(mData[k]))
				{
					writer.WriteStartElement("data");
					writer.WriteAttributeString("key", (mData[k] as DataType).Key.ToString());					
					if(typeof(DataCollection).IsInstanceOfType((mData[k] as DataType).Text))
					{
						WriteData(writer, (mData[k] as DataType).Text);						
					}
					else if(typeof(string).IsInstanceOfType((mData[k] as DataType).Text))
						writer.WriteString((mData[k] as DataType).Text.ToString());
				
					writer.WriteEndElement();
				}
				else if(typeof(CollectionBase).IsInstanceOfType(mData[k]))
				{
					(mData[k] as IXmlSerializable).WriteXml(writer);
				}
			

			}

			
		}


		private void WriteData(XmlWriter writer,DataCollection collection)
		{
			for(int m=0; m<collection.Count; m++)
			{
				if(typeof(DataType).IsInstanceOfType(collection[m]))
				{
					writer.WriteStartElement("data");
					writer.WriteAttributeString("key",(collection[m] as DataType).Key);
					WriteData(writer,(collection[m] as DataType).Text);
					writer.WriteEndElement();
				}	
				else if(typeof(string).IsInstanceOfType(collection[m]))					
					writer.WriteString(collection[m].ToString());	
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			// TODO:  Add ShapeType.GetSchema implementation
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			// TODO:  Add ShapeType.ReadXml implementation
		}

		#endregion
	}
}

