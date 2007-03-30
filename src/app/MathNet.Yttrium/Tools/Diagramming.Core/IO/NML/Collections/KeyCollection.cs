using System;

namespace Netron.GraphLib.IO.GraphML
{
	/// <summary>
	/// Summary description for KeyCollection.
	/// </summary>
	public class KeyCollection : System.Collections.CollectionBase 
	{
            
		/// <summary />
		/// <remarks />
		public KeyCollection() 
		{
		}
            
		/// <summary />
		/// <remarks />
		public virtual object this[int index] 
		{
			get 
			{
				return this.List[index];
			}
			set 
			{
				this.List[index] = value;
			}
		}
            
		/// <summary />
		/// <remarks />
		public virtual void Add(object o) 
		{
			this.List.Add(o);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void AddKeyType(KeyType o) 
		{
			this.List.Add(o);
		}
            
		/// <summary />
		/// <remarks />
		public virtual bool ContainsKeyType(KeyType o) 
		{
			return this.List.Contains(o);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void RemoveKeyType(KeyType o) 
		{
			this.List.Remove(o);
		}
	}
}
