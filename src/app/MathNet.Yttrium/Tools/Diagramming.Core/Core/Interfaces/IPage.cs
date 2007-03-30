using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The interface of a page
    /// </summary>
    public interface IPage
    {
        #region Events
        /// <summary>
        /// Occurs when an entity is added.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the page is cleared
        /// </summary>
        event EventHandler OnClear;
        /// <summary>
        /// Occurs when the Ambience has changed
        /// </summary>
        event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets all the connections in the page.
        /// </summary>
        CollectionBase<IConnection> Connections { get;}
        /// <summary>
        /// Gets all shapes in this page.
        /// </summary>
        CollectionBase<IShape> Shapes { get;}
        /// <summary>
        /// Gets the layers.
        /// </summary>
        /// <value>The layers.</value>
        CollectionBase<ILayer> Layers { get;}
        /// <summary>
        /// Gets the default layer.
        /// </summary>
        /// <value>The default layer.</value>
        ILayer DefaultLayer { get;}
        /// <summary>
        /// Gets the ambience.
        /// </summary>
        /// <value>The ambience.</value>
        Ambience Ambience { get;}
        /// <summary>
        /// Gets a reference to the model
        /// </summary>
        IModel Model
        {
            get;
            set;
        }
        #endregion
        
    }
}
