
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
    
    
namespace Netron.GraphLib.IO.NML 
{        
   /// <summary>
   /// XML wrapper of a Connector object
   /// </summary>
	[XmlRoot(ElementName="Connector", IsNullable=false, DataType="")]
	public class ConnectorType 
	{

		#region Fields
		/// <summary>
		/// the uid
		/// </summary>
		private string mUID;
		/// <summary>
		/// the name
		/// </summary>
		private string name;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the UID
		/// </summary>
		[XmlAttribute(AttributeName="UID")]
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
		/// <summary>
		/// Gets or sets the name of the connector
		/// </summary>
		[XmlAttribute(AttributeName="Name")]
		public virtual string Name 
		{
			get 
			{
				return this.name;
			}
			set 
			{
				this.name = value;
			}
		}

		#endregion

	}
}
