using System;
using System.Collections;
namespace Netron.GraphLib.IO.NML
{
	/// <summary>
	/// STC of string-value pairs, helpful in keeping a collection of properties with their value
	/// </summary>
	public class PropertiesHashtable : DictionaryBase
	{
		/// <summary>
		/// keeps the collection of keys
		/// </summary>
		private StringCollection mKeys = new StringCollection();

		/// <summary>
		/// Gets the keys of the hashtable and
		/// allows to loop over the keys without
		/// boxing/unboxing.
		/// </summary>
		public StringCollection Keys
		{	
			get{return mKeys;}
		
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public PropertiesHashtable()
		{
			
		}
		

		
		/// <summary>
		/// Adds a property-value pair
		/// </summary>
		/// <param name="key"></param>
		/// <param name="propertyValue"></param>
		public void Add(string key, object propertyValue)
		{
			this.InnerHashtable.Add(key, propertyValue);
			mKeys.Add(key);
		}
		/// <summary>
		/// Integer indexer
		/// </summary>
		public object this[int index]
		{
			get{
			
				if(mKeys[index]!=null)
				{		
					return this.InnerHashtable[mKeys[index]];
				}
				else return null;
			}
		}

		/// <summary>
		/// Removes an elements based on a key
		/// </summary>
		/// <param name="key">a (string) key</param>
		/// <returns></returns>
		public bool Remove(string key)
		{
			int index;
			if((index=mKeys.Contains(key))>-1)
			{
				this.InnerHashtable.Remove(key);
				this.mKeys.RemoveAt(index);
				return true;
			}
			return false;
		}
	}
}
