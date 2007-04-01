using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implementation of the <see cref="IModel"/> interface; the 'database' of the control.
    /// </summary>
    public sealed partial class Model : IModel, IDisposable
    {
        #region Events
        /// <summary>
        /// Occurs when an entity is removed from the model
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when an entity is added to the model.
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an element of the diagram requests a refresh of a region/rectangle of the canvas
        /// </summary>
        public event EventHandler<RectangleEventArgs> OnInvalidateRectangle;
        /// <summary>
        /// Occurs when an element of the diagram requests a refresh
        /// </summary>
        public event EventHandler OnInvalidate;
        /// <summary>
        /// Occurs when the ConnectionCollection has changed
        /// </summary>
        public event EventHandler<ConnectionCollectionEventArgs> OnConnectionCollectionChanged;
        /// <summary>
        /// Occurs when the cursor is changed and the surface is supposed to set the cursor accordingly.
        /// </summary>
        public event EventHandler<CursorEventArgs> OnCursorChange;

        public event EventHandler<ConnectorsEventArgs> OnConnectorAttached;
        public event EventHandler<ConnectorsEventArgs> OnConnectorDetached;


        /// <summary>
        /// Raises the <see cref="OnConnectionCollectionChanged"/> event
        /// </summary>
        /// <param name="e">ConnectionCollection event argument</param>
        private void RaiseOnConnectionCollectionChanged(ConnectionCollectionEventArgs e)
        {
            EventHandler<ConnectionCollectionEventArgs> handler = OnConnectionCollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RaiseOnCursorChange(Cursor cursor)
        {
            EventHandler<CursorEventArgs> handler = OnCursorChange;
            if(handler != null)
                handler(this, new CursorEventArgs(cursor));
        }

        /// <summary>
        /// Raises the on invalidate.
        /// </summary>
        public void RaiseOnInvalidate()
        {
            if (OnInvalidate != null)
                OnInvalidate(this, EventArgs.Empty);
        }
        /// <summary>
        /// Raises the OnInvalidateRectangle event.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public void RaiseOnInvalidateRectangle(Rectangle rectangle)
        {
            EventHandler<RectangleEventArgs> handler = OnInvalidateRectangle;
            if (handler != null)
            {
                handler(this, new RectangleEventArgs(rectangle));
            }
        }
        /// <summary>
        /// Occurs when the bounding region (aka client-rectangle) of the canvas has changed
        /// </summary>
        public event EventHandler<RectangleEventArgs> OnRectangleChanged;
        /// <summary>
        /// Raises the <see cref="OnRectangleChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnRectangleChanged(RectangleEventArgs e)
        {
            EventHandler<RectangleEventArgs> handler = OnRectangleChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Occurs when the diagram info (aka user metadata) has changed
        /// </summary>
        public event EventHandler<DiagramInformationEventArgs> OnDiagramInformationChanged;
        /// <summary>
        /// Raises the <see cref="OnDiagramInformationChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnDiagramInformationChanged(DiagramInformationEventArgs e)
        {
            EventHandler<DiagramInformationEventArgs> handler = OnDiagramInformationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        /// <summary>
        /// Occurs when the Ambience has changed
        /// </summary>
        public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        /// <summary>
        /// Raises the <see cref="OnAmbienceChanged"/> event
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnAmbienceChanged(AmbienceEventArgs e)
        {
            EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnEntityAdded"/> event
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        private void RaiseOnEntityAdded(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnEntityRemoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        private void RaiseOnEntityRemoved(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Fields

        private GraphicsUnit measurementUnits = GraphicsUnit.Pixel;
        /// <summary>
        /// the LayoutRoot field
        /// </summary>
        private IShape mLayoutRoot;
        /// <summary>
        /// the DefaultPage field
        /// </summary>
        [NonSerialized]
        private IPage mDefaultPage;
        /// <summary>
        /// the page collection
        /// </summary>
        [NonSerialized]
        private CollectionBase<IPage> mPages;
        ///<summary>
        /// the shapes of the diagram
        /// </summary>
        [NonSerialized]
        private CollectionBase<IShape> mShapes;
        /// <summary>
        /// the bounding rectangle
        /// </summary>
        [NonSerialized]        
        private Rectangle mRectangle;
        /// <summary>
        /// the metadata of the diagram
        /// </summary>
        [NonSerialized]
        private DocumentInformation mInformation;
      
        /// <summary>
        /// the collection of to-be-painted diagram entities
        /// </summary>
        [NonSerialized]
        private CollectionBase<IDiagramEntity> mPaintables;
        /// <summary>
        /// the CurrentPage field
        /// </summary>
        [NonSerialized]
        private IPage mCurrentPage;

        private float mMeasurementScale = 1.0F;

        [NonSerialized]
        private Dictionary<IConnector, IDiagramEntity> mConnectorHolders;
        #endregion

        #region Properties

        [Browsable(true)]
        [Description("Scaling value for logical units.")]
        public float MeasurementScale
        {
            get
            {
                return mMeasurementScale;
            }

            set
            {
                mMeasurementScale = value;
            }
        }
        [BrowsableAttribute(true)]
        [Description("Logical unit of measurement")]
        public  GraphicsUnit MeasurementUnits
        {
            get
            {
                return measurementUnits;
            }

            set
            {
                measurementUnits = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the LayoutRoot
        /// </summary>
        public IShape LayoutRoot
        {
            get { return mLayoutRoot; }
            set { mLayoutRoot = value; 
                
            }
        }

        
        /// <summary>
        /// Gets or sets the CurrentPage
        /// </summary>
        public IPage CurrentPage
        {
            get { return mCurrentPage; }
            set { mCurrentPage = value; }
        }

        /// <summary>
        /// Gets the paintables.
        /// </summary>
        /// <value>The paintables.</value>
        public CollectionBase<IDiagramEntity> Paintables
        {
            get { return mPaintables; }
        }
        /// <summary>
        /// Gets the pages of the diagram control.
        /// </summary>
        /// <value>The pages.</value>
        public CollectionBase<IPage> Pages
        {
            get { return mPages; }
        }
        /// <summary>
        /// Gets or sets the default page
        /// </summary>
        public  IPage DefaultPage
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return mDefaultPage; }
            set { mDefaultPage = value; }
        }
        /// <summary>
        /// Gets or sets the shapes of the diagram
        /// </summary>
        public CollectionBase<IShape> Shapes
        {
            get
            {
                return mShapes;
            }
            internal set
            {
                mShapes = value;
            }
        }

        [Obsolete("Use .Parent instead")]
        public Dictionary<IConnector, IDiagramEntity> ConnectorHolders
        {
            // TODO: Get rid of this, complete nonsense. Obviously had some bad 5 minutes... (chris)

            get
            {
                return mConnectorHolders;
            }
        }

        

        /// <summary>
        /// Gets or sets the information of the diagram
        /// </summary>
        internal DocumentInformation Information
        {
            get
            {
                return mInformation;
            }
            set
            {
                mInformation = value;
            }
        }
        

    
  
        /// <summary>
        /// Gets the bounding rectangle of the diagram (client rectangle)
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return mRectangle;
            }
            set
            {
                mRectangle = value;
                RaiseOnRectangleChanged(new RectangleEventArgs(value));
            }
        }

        /// <summary>
        /// Gets the horizontal coordinate of the diagram
        /// </summary>
        public float X
        {
            get
            {
                return mRectangle.X;
            }            
        }

        /// <summary>
        /// Gets the verticla coordinate of the diagram
        /// </summary>
        public float Y
        {
            get
            {
                return mRectangle.Y;
            }
            
        }

        /// <summary>
        /// Gets the width of the diagram
        /// </summary>
        public float Width
        {
            get
            {
                return mRectangle.Width;
            }
        }

        /// <summary>
        /// Gets the height of the diagram
        /// </summary>
        public float Height
        {
            get
            {
                return mRectangle.Height;
            }            
        }

        /// <summary>
        /// Gets or sets the collection of connections
        /// </summary>
        public CollectionBase<IConnection> Connections
        {
            get
            {
                throw new System.NotImplementedException();
            }

        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Model()
        {
            //here I'll have to work on the scene graph
            //this.mShapes = new CollectionBase<IShape>();
            //the default page
            
            //the page collection
            mPages = new CollectionBase<IPage>();
            mPages.Add(new Page("Default Page", this));

            mConnectorHolders = new Dictionary<IConnector, IDiagramEntity>();

            Init();
        }
        /// <summary>
        /// Initializes this object
        /// <remarks>See also the <see cref="OnDeserialized"/> event for post-deserialization actions to which this method is related.
        /// </remarks>
        /// </summary>
        private void Init()
        {
            if(mPages == null)
                throw new InconsistencyException("The page collection is 'null'.");
            if(mPages.Count == 0)
                throw new InconsistencyException("The page collection should contain at least one page.");

            foreach(IPage page in mPages)
                AttachToPage(page);
            mDefaultPage = mPages[0];            
            //initially the current page is the zero-th page in the collection
            SetCurrentPage(0);
            #endregion
        }

        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void SetCurrentPage(IPage page)
        {
            mCurrentPage = page;
            RaiseOnAmbienceChanged(new AmbienceEventArgs(page.Ambience));
            //change the paintables as well
            mPaintables = new CollectionBase<IDiagramEntity>();

            #region Reload of the z-order, usually only necessary after deserialization

            CollectionBase<IDiagramEntity> collected = new CollectionBase<IDiagramEntity>();
            //pick up the non-group entities
            foreach (IDiagramEntity entity in mCurrentPage.DefaultLayer.Entities)
                if (!typeof(IGroup).IsInstanceOfType(entity))
                    collected.Add(entity);
            
            if(collected.Count > 0)
            {
                Algorithms.SortInPlace<IDiagramEntity>(collected, new SceneIndexComparer<IDiagramEntity>());
                mPaintables.AddRange(collected);
            }
            #endregion
        }
        /// <summary>
        /// Sets the current page.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        public void SetCurrentPage(int pageIndex)
        {
            if(mPages == null || mPages.Count == 0 || pageIndex >= mPages.Count || pageIndex < 0)
                throw new IndexOutOfRangeException("The page index is outside the page range.");
            SetCurrentPage(mPages[pageIndex]);
        }
        private void AttachToPage(IPage page)
        {
            page.OnEntityAdded += new EventHandler<EntityEventArgs>(mDefaultPage_OnEntityAdded);
            page.OnEntityRemoved += new EventHandler<EntityEventArgs>(mDefaultPage_OnEntityRemoved);
            page.OnClear += new EventHandler(mDefaultPage_OnClear);
            page.OnAmbienceChanged += new EventHandler<AmbienceEventArgs>(mDefaultPage_OnAmbienceChanged);
            page.Model = this;
        }

        void mDefaultPage_OnAmbienceChanged(object sender, AmbienceEventArgs e)
        {
            RaiseOnAmbienceChanged(e);
        }

        #region Paintables transfers on Page changes

        /// <summary>
        /// Handles the OnClear event of the DefaultPage.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void mDefaultPage_OnClear(object sender, EventArgs e)
        {
            mPaintables.Clear();
        }

        /// <summary>
        /// Handles the OnEntityRemoved event of the DefaultPage.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        void mDefaultPage_OnEntityRemoved(object sender, EntityEventArgs e)
        {
            //IShape sp = e.Entity as IShape;
            //if(sp != null)
            //{
            //    foreach(IConnector cr in sp.Connectors)
            //        mConnectorHolders.Remove(cr);
            //}

            if(mPaintables.Contains(e.Entity))
            {
                //shift the entities above the one to be removed
                int index = e.Entity.SceneIndex;
                foreach(IDiagramEntity entity in mPaintables)
                {
                    if(entity.SceneIndex > index)
                        entity.SceneIndex--;
                }
                mPaintables.Remove(e.Entity);
            }
            //if the selection contains the shape we have to remove it from the selection
            if (Selection.SelectedItems.Contains(e.Entity))
            {
                Selection.SelectedItems.Remove(e.Entity);               
            }

            RaiseOnEntityRemoved(e);

            IConnection cn = e.Entity as IConnection;
            if(cn != null)
            {
                mConnectorHolders.Remove(cn.From);
                mConnectorHolders.Remove(cn.To);
            }
        }

        /// <summary>
        /// Handles the OnEntityAdded event of the Page and adds the new entity to the Paintables.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        void mDefaultPage_OnEntityAdded(object sender, EntityEventArgs e)
        {
            IConnection cn = e.Entity as IConnection;
            if(cn != null)
            {
                mConnectorHolders.Add(cn.From, cn);
                mConnectorHolders.Add(cn.To, cn);
            }
            //IShape sp = e.Entity as IShape;
            //if(sp != null)
            //{
            //    foreach(IConnector cr in sp.Connectors)
            //        mConnectorHolders.Add(cr, sp);
            //}

            //don't add it if it's already there or if it's a group (unless you want to deploy something special to emphasize a group shape).
            if (!mPaintables.Contains(e.Entity) )
            {
                if ((e.Entity is IGroup) && !(e.Entity as IGroup).EmphasizeGroup)
                {
                    return;
                }
                //set the new entity on top of the stack
                e.Entity.SceneIndex = mPaintables.Count;
                mPaintables.Add(e.Entity);
            }
            #region Addition callback
            IAdditionCallback callback = e.Entity.GetService(typeof(IAdditionCallback)) as IAdditionCallback;
            if (callback != null)
                callback.OnAddition();
            #endregion
            RaiseOnEntityAdded(e);
        }
        #endregion




        #region Methods
        #region Ordering methods
        /// <summary>
        /// Re-sets the scene-index of the paintables
        /// </summary>
        private void ReAssignSceneIndex()
        {
            for(int i = 0; i < mPaintables.Count; i++)
            {
                mPaintables[i].SceneIndex = i;
            }
        }
        /// <summary>
        /// Sends to entity to the bottom of the z-order stack.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SendToBack(IDiagramEntity entity)
        {
            if(mPaintables.Contains(entity))
            {
                mPaintables.Remove(entity);
                mPaintables.Insert(0, entity);
                ReAssignSceneIndex();
                Rectangle rec = entity.Rectangle;
                rec.Inflate(20, 20);
                this.RaiseOnInvalidateRectangle(Rectangle);
            }
        }

        /// <summary>
        /// Sends the entity down the z-order stack with the specified amount.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        public void SendBackwards(IDiagramEntity entity, int zShift)
        {
            if (mPaintables.Contains(entity))
            {
                int newpos = mPaintables.IndexOf(entity) - zShift;
                //if this is the first in the row you cannot move it lower
                if (newpos >= 0)
                {
                    mPaintables.Remove(entity);
                    mPaintables.Insert(newpos, entity);
                    ReAssignSceneIndex();
                    Rectangle rec = entity.Rectangle;
                    rec.Inflate(20, 20);
                    this.RaiseOnInvalidateRectangle(Rectangle);
                }

            }
        }
        /// <summary>
        /// Sends the entity one level down the z-order stack.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SendBackwards(IDiagramEntity entity)
        {
            SendBackwards(entity, 1);
        }

        /// <summary>
        /// Sends the entity to the top of the z-order stack.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SendForwards(IDiagramEntity entity)
        {
            SendForwards(entity, 1);
        }
        /// <summary>
        /// Sends the entity up the z-order stack with the specified amount.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="zShift">The z shift.</param>
        public void SendForwards(IDiagramEntity entity, int zShift)
        {
            if (mPaintables.Contains(entity) && zShift>=1)
            {
                int newpos = mPaintables.IndexOf(entity) + zShift;
                //if this is the last in the row you cannot move it higher
                if (newpos < mPaintables.Count)
                {
                    mPaintables.Remove(entity);
                    mPaintables.Insert(newpos, entity); //does it works when this is an addition at the top?
                    ReAssignSceneIndex();
                    Rectangle rec = entity.Rectangle;
                    rec.Inflate(20, 20);
                    this.RaiseOnInvalidateRectangle(Rectangle);
                }
            }
        }

        /// <summary>
        /// Sends the entity to the front of the z-order stack.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SendToFront(IDiagramEntity entity)
        {
            if(mPaintables.Contains(entity))
            {
                mPaintables.Remove(entity);
                mPaintables.Add(entity);
                ReAssignSceneIndex();
                Rectangle rec = entity.Rectangle;
                rec.Inflate(20, 20);
                this.RaiseOnInvalidateRectangle(Rectangle);
            }
        }
        #endregion

        #region Diagram manipulation actions
        /// <summary>
        /// Adds a shape to the diagram
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public IShape AddShape(IShape shape)
        {
           
            SetModel(shape);
            //By default the new shape is added to the default in the current page
            //For now, there is only a single default page;
            DefaultPage.DefaultLayer.Entities.Add(shape);
            return shape;
            
        }
        /// <summary>
        /// Adds a connection to the diagram
        /// </summary>
        /// <param name="connection">a connection</param>
        public IConnection AddConnection(IConnection connection)
        {
            SetModel(connection);
            DefaultPage.DefaultLayer.Entities.Add(connection);
            return connection;
        }
        
        
        /// <summary>
        /// Adds a connection between two shape connectors.        
        /// </summary>
        /// <param name="from">From connector.</param>
        /// <param name="to">To connector.</param>
        public IConnection AddConnection(IConnector from, IConnector to)
        {
            Connection con = new Connection(from.Point, to.Point);
            this.AddConnection(con);
            return con;
        }

        public void NotifyConnectorAttached(IConnector subj, IConnector obj)
        {
            EventHandler<ConnectorsEventArgs> handler = OnConnectorAttached;
            if(handler != null)
                handler(this, new ConnectorsEventArgs(subj, obj));
        }

        public void NotifyConnectorDetached(IConnector subj, IConnector obj)
        {
            EventHandler<ConnectorsEventArgs> handler = OnConnectorDetached;
            if(handler != null)
                handler(this, new ConnectorsEventArgs(subj, obj));
        }

        /// <summary>
        /// Sets the model (recursively) on the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SetModel(IDiagramEntity entity)
        {
            if(entity is IConnector)
            {
                (entity as IConnector).Model = this;
            }
            else if(entity is IConnection)
            {
                IConnection con = entity as IConnection;
                con.Model = this;
                Debug.Assert(con.From != null, "The 'From' connector is not set.");
                con.From.Model = this;
                Debug.Assert(con.From != null, "The 'To' connector is not set.");
                con.To.Model = this;
            }
            else if(entity is IShape)
            {
                IShape shape = entity as IShape;
                shape.Model = this;
                foreach(IConnector co in shape.Connectors)
                {
                    co.Model = this;
                }
            }
            
        }
        /// <summary>
        /// Removes the shape from the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        public void RemoveShape(IShape shape)
        {
            //remove it from the layer(s)
            DefaultPage.DefaultLayer.Entities.Remove(shape);
        }
        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(IDiagramEntity entity)
        {
            if (DefaultPage.DefaultLayer.Entities.Contains(entity))
                DefaultPage.DefaultLayer.Entities.Remove(entity);
        }

        /// <summary>
        /// Adds a collection of entities to the diagram
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void AddEntities(CollectionBase<IDiagramEntity> collection)
        {
            foreach (IDiagramEntity entity in collection)
            {
                SetModel(entity);
                DefaultPage.DefaultLayer.Entities.Add(entity);
            }
        }
        /// <summary>
        /// Unwraps an entity
        /// <list type="bullet">
        /// <term>Uid</term><description>Generates a new <see cref="IDiagramEntity.Uid"/> for the entity. </description>
        /// <tem>Model</tem><description>Assigns the Model property to the entity.</description>
        /// 
        /// </list>
        /// </summary>
        public void Unwrap(IDiagramEntity entity)
        {
            //set a new unique identifier for this copied object
            entity.NewUid(true);
            //this assignment will be recursive if needed
            SetModel(entity);
            DefaultPage.DefaultLayer.Entities.Add(entity);
            
        }
        /// <summary>
        /// Unwraps the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void Unwrap(CollectionBase<IDiagramEntity> collection)
        {
            if (collection == null)
                return;
            foreach (IDiagramEntity entity in collection)
            {
                Unwrap(entity);
            }
            //reconnect the connectors, just like the deserialization of a filed diagram
            Dictionary<Guid, Anchor>.Enumerator enumer = Anchors.GetEnumerator();
            System.Collections.Generic.KeyValuePair<Guid, Anchor> pair;
            Anchor anchor;
            while (enumer.MoveNext())
            {
                pair = enumer.Current;
                anchor = pair.Value;
                if (anchor.Parent != Guid.Empty) //there's a parent connector
                {
                    if (Anchors.ContainsKey(anchor.Parent))
                    {
                        Anchors.GetAnchor(anchor.Parent).Instance.AttachConnector(anchor.Instance);
                    }
                }
            }
            //clean up the anchoring matrix
            Anchors.Clear();
        }

        /// <summary>
        /// Clears the model
        /// </summary>
        public void Clear()
        {
            //clear the scene-graph
            this.DefaultPage.DefaultLayer.Entities.Clear();

        }
        #endregion

      

      

        

     
        #endregion

        #region Standard IDispose implementation
        /// <summary>
        /// Disposes the view
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);


        }
        /// <summary>
        /// Disposes the view
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }

        }

        #endregion
    }
}
