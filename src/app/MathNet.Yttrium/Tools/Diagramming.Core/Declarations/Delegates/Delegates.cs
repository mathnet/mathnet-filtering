using System;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
	
	/// <summary>
	/// Information regarding the addition of a new item to the collection
	/// </summary>
    public delegate void CollectionAddInfo<T>(CollectionBase<T> collection, EntityEventArgs e);
	/// <summary>
	/// Information regarding the removal of an item from the collection
	/// </summary>
    public delegate void CollectionRemoveInfo<T>(CollectionBase<T> collection, EntityEventArgs e);
	/// <summary>
	/// Information regarding the removal/clear of all items from the collection
	/// </summary>
    public delegate void CollectionClearInfo<T>(CollectionBase<T> collection, EventArgs e);	
	/// <summary>
	/// the info coming with the show-props event
	/// </summary>
	public delegate void PropertiesInfo(object ent);	
	
}
