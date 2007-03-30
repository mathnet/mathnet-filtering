using System;

namespace Netron.GraphLib.IO.GraphML
{
	/// <summary>
	/// Summary description for TextCollection.
	/// </summary>
	public class Textcollection : System.Collections.CollectionBase 
	{
            
		/// <summary />
		/// <remarks />
		public Textcollection() 
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
		public virtual void AddString(string o) 
		{
			this.List.Add(o);
		}
            
		/// <summary />
		/// <remarks />
		public virtual bool ContainsString(string o) 
		{
			return this.List.Contains(o);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void RemoveString(string o) 
		{
			this.List.Remove(o);
		}
            
		
	}
}
