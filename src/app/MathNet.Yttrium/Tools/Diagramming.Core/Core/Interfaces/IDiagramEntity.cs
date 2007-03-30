using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// The diagram entity is base entity of all elements of a diagram.
	/// </summary>
	public interface IDiagramEntity : IPaintable, IServiceProvider, ISerializable
	{

		#region Events
		/// <summary>
		/// Occurs when the entity's properties have changed
		/// </summary>
		event EventHandler<EntityEventArgs> OnEntityChange;
		/// <summary>
		/// Occurs when the entity is selected. 
		/// This can be different than the OnClick because the selector
		/// can select and entity without clicking on it.
		/// </summary>
        event EventHandler<EntityEventArgs> OnEntitySelect;
		/// <summary>
		/// Occurs when the user click on the entity.
		/// </summary>
        event EventHandler<EntityEventArgs> OnClick;
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IDiagramEntity"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get;set;}
		/// <summary>
		/// Gets or sets whether the entity is selected
		/// </summary>
		bool IsSelected {get; set;}
		/// <summary>
		/// Gets or sets the canvas to which the entity belongs
		/// </summary>
		IModel Model {get; set;}
		/// <summary>
		/// Gets or sets the parent of the entity.
        /// <remarks>If the parent is null the entity is a branch node in the scene graph. The layer is never a parent because an entity can 
        /// belong to multiple layers.
        /// </remarks>
		/// </summary>
		object Parent {get; set;}
		/// <summary>
		/// Gets or sets the name of the entity
		/// </summary>
		string Name {get; set;}
		/// <summary>
		/// Gets or sets whether the entity is hovered by the mouse
		/// </summary>
		bool Hovered {get; set;}
		/// <summary>
		/// General purpose tag
		/// </summary>
		object Tag {get; set;}
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        string EntityName { get;}
        /// <summary>
        /// Gets or sets the index of this entity in the scene-graph.
        /// </summary>
        /// <value>The index of the scene.</value>
        int SceneIndex { get; set;}
        /// <summary>
        /// Gets or sets the unique top-group to which this entity belongs.
        /// </summary>
        IGroup Group { get; set;}
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IDiagramEntity"/> is resizable.
        /// </summary>
        /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
        bool Resizable { get;set;}
        /// <summary>
        /// Gets the globally unique identifier of this entity
        /// </summary>
        Guid Uid { get;}

        /// <summary>
        /// Gets or sets the paint style.
        /// </summary>
        /// <value>The paint style.</value>
        IPaintStyle PaintStyle { get; set;}
        /// <summary>
        /// Gets or sets the pen style.
        /// </summary>
        /// <value>The pen style.</value>
        IPenStyle PenStyle { get; set;}
		#endregion

		#region Methods
        /// <summary>
        /// Generates a new Uid for this entity.
        /// </summary>
        /// <param name="recursive">if the Uid has to be changed recursively down to the sub-entities, set to true, otherwise false.</param>
        void NewUid(bool recursive);
        
        /// <summary>
        /// The custom elements to be added to the menu on a per-entity basis
        /// </summary>
        /// <returns></returns>
        MenuItem[] ShapeMenu();
		/// <summary>
		/// Raises the onclick event.
		/// </summary>
		/// <param name="e"></param>
        void RaiseOnClick(EntityEventArgs e);
        /// <summary>
        /// Raises the OnMouseDown event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityMouseEventArgs"/> instance containing the event data.</param>
        void RaiseOnMouseDown(EntityMouseEventArgs e);		
		/// <summary>
		/// Returns whether the entity was hit at the given location
		/// </summary>
		/// <param name="p">a Point location</param>
		/// <returns></returns>
		bool Hit(Point p);
		/// <summary>
		/// Invalidates the entity
		/// </summary>
		void Invalidate();		
		/// <summary>
		/// Invalidates a certain rectangle of the canvas
		/// </summary>
		/// <param name="rectangle"></param>
		void Invalidate(Rectangle rectangle);
		/// <summary>
		/// Moves the entity to the given location
		/// </summary>
		/// <param name="p">a Point location</param>
		void Move(Point p);
		#endregion
	}
}
