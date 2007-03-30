using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Interface of the MVC model
    /// </summary>    
    public interface  IModel
    {

        #region Events
        /// <summary>
        /// Occurs when the <see cref="Ambience"/> is changed.
        /// 
        /// </summary>
        event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        /// <summary>
        /// Occurs the collection of connections is changed
        /// </summary>
        event EventHandler<ConnectionCollectionEventArgs> OnConnectionCollectionChanged;
        /// <summary>
        /// Occurs when the diagram information has changed
        /// </summary>
        event EventHandler<DiagramInformationEventArgs> OnDiagramInformationChanged;
        /// <summary>
        /// Occurs when an underlying element (usually an entity) asks to repaint the whole canvas
        /// </summary>
        event EventHandler OnInvalidate;
        /// <summary>
        /// Occurs when an underlying element (usually an entity) asks to repaint part of the canvas.
        /// </summary>
        event EventHandler<RectangleEventArgs> OnInvalidateRectangle;
        /// <summary>
        /// Occurs when an entity is added to the model.
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed from the model.
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the cursor changes and the surface has to effectively show a different cursor
        /// </summary>
        event EventHandler<CursorEventArgs> OnCursorChange;

        event EventHandler<ConnectorsEventArgs> OnConnectorAttached;
        event EventHandler<ConnectorsEventArgs> OnConnectorDetached;
        #endregion

        #region Properties
        float MeasurementScale { get; set;}
        GraphicsUnit MeasurementUnits { get;set;}
        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        IShape LayoutRoot { get;set;}
        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        CollectionBase<IConnection> Connections { get; }
        /// <summary>
        /// Gets the shapes.
        /// </summary>
        /// <value>The shapes.</value>
        CollectionBase<IShape> Shapes { get; }

        Dictionary<IConnector, IDiagramEntity> ConnectorHolders { get; }

        /// <summary>
        /// Gets the paintables.
        /// </summary>
        /// <value>The paintables.</value>
        CollectionBase<IDiagramEntity> Paintables { get;}
        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        IPage CurrentPage { get;}
        
        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        CollectionBase<IPage> Pages { get;}

        /// <summary>
        /// Gets the default page.
        /// </summary>
        /// <value>The default page.</value>
        IPage DefaultPage { get;}

        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="page">The page.</param>
        void SetCurrentPage(IPage page);
        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        void SetCurrentPage(int pageIndex);
        #endregion

        #region Methods
        #region Diagram actions
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="connection">The connection.</param>
        IConnection AddConnection(IConnection connection);

        /// <summary>
        /// Adds a shape to the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        IShape AddShape(IShape shape);


        void NotifyConnectorAttached(IConnector subj, IConnector obj);

        void NotifyConnectorDetached(IConnector subj, IConnector obj);

        /// <summary>
        /// Clears the diagram.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes the shape from the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        void RemoveShape(IShape shape);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Remove(IDiagramEntity entity);

        /// <summary>
        /// Sends the given entity to the front of the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SendToFront(IDiagramEntity entity);

        /// <summary>
        /// Sends the given entity backwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        void SendBackwards(IDiagramEntity entity, int zShift);
        /// <summary>
        /// Sends the given entity to the back of the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SendToBack(IDiagramEntity entity);
        /// <summary>
        /// Sends the given entity forwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SendForwards(IDiagramEntity entity);
        /// <summary>
        /// Sends the given entity forwards in the z-order.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        void SendForwards(IDiagramEntity entity, int zShift);
        /// <summary>
        /// Unwraps the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        void Unwrap(CollectionBase<IDiagramEntity> collection);
        #endregion

        /// <summary>
        /// Raises the on invalidate.
        /// </summary>
        void RaiseOnInvalidate();
        /// <summary>
        /// Raises the OnInvalidateRectangle event.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        void RaiseOnInvalidateRectangle(Rectangle rectangle);
        /// <summary>
        /// Raises the OnCursorChange event.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        void RaiseOnCursorChange(Cursor cursor);
        #endregion
    }
}
