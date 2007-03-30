using System;

namespace Netron.GraphLib.IO.GraphML
{
	/// <summary>
	/// Summary description for ItemCollection.
	/// </summary>
	public class ItemCollection : System.Collections.CollectionBase 
	{
            
		/// <summary />
		/// <remarks />
		public ItemCollection() 
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
		public virtual void AddGraph(GraphType _graph) 
		{
			this.List.Add(_graph);
		}
            
		/// <summary />
		/// <remarks />
		public virtual bool ContainsGraph(GraphType _graph) 
		{
			return this.List.Contains(_graph);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void RemoveGraph(GraphType _graph) 
		{
			this.List.Remove(_graph);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void AddData(DataType _data) 
		{
			this.List.Add(_data);
		}
            
		/// <summary />
		/// <remarks />
		public virtual bool ContainsData(DataType _data) 
		{
			return this.List.Contains(_data);
		}
            
		/// <summary />
		/// <remarks />
		public virtual void RemoveData(DataType _data) 
		{
			this.List.Remove(_data);
		}
	}
        
}
