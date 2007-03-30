using System;

namespace Netron.GraphLib.IO.NML
{
	/// <summary>
	/// Utility class to speed up the deserialization of connections
	/// This struct keeps unattached connections, the connections have the UID of the From and the To connector
	/// but the respective connectors are still null
	/// </summary>
	public struct FromTo
	{
		/// <summary>
		/// Default ctor
		/// </summary>		
		public FromTo( string from, string to)
		{
			this.From = from;
			this.To = to;	
			this.Connection = null;
		}
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="from">the UID of the From-connector</param>
		/// <param name="to">the UID of the To-Connector</param>
		/// <param name="con">the instantiated connection</param>
		public FromTo(string from, string to, Connection con) : this(from,to)
		{
			Connection = con;
		}

		/// <summary>
		/// Gets or sets the parent in this relation
		/// </summary>
		public string From;		
		/// <summary>
		/// Gets or sets the child in this relation
		/// </summary>
		public string To;
		/// <summary>
		/// Gets or sets the connection
		/// </summary>
		public Connection Connection;
	}
}
