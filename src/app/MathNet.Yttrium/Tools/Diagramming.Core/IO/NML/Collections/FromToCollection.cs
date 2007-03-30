using System;
using System.Collections;

namespace Netron.GraphLib.IO.NML
{
	/// <summary>
	/// STC of FromTo collection, related to deserailization of connections
	/// </summary>
	public class FromToCollection : CollectionBase
	{		
		/// <summary>
		/// integer indexer
		/// </summary>
		public FromTo this[int index]
		{
			get{return (FromTo) this.InnerList[index] ;}
		}

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="ft">a ParentChild object</param>
		/// <returns></returns>
		public int Add(FromTo ft)
		{
			return this.InnerList.Add(ft);
		}
	}
}
