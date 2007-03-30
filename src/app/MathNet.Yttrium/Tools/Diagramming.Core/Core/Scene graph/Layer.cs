using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implements the ILayer interface/mechanism
    /// </summary>
    partial class Layer : ILayer
    {
        #region Events
        /// <summary>
        /// Occurs when an entity is added to the layer
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed from the layer 
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the layer is cleared
        /// </summary>
        public event EventHandler OnClear;
        #endregion

        #region Fields
        /// <summary>
        /// the Model field
        /// </summary>
        [NonSerialized]
        private IModel mModel;

        /// <summary>
        /// the Entities field
        /// </summary>
        private CollectionBase<IDiagramEntity> mEntities;
        /// <summary>
        /// the Name field
        /// </summary>
        private string mName;
        
        #endregion

        #region Properties

          /// <summary>
          /// Gets all shapes in this layer.
          /// </summary>
        public CollectionBase<IShape> Shapes
        {
            get
            {
                CollectionBase<IShape> shapes = new CollectionBase<IShape>();
                foreach (IDiagramEntity entity in mEntities)
                {
                    if (entity is IShape)
                        shapes.Add(entity as IShape);
                }
                return shapes;
            }
        }
        /// <summary>
        /// Gets all connections in this layer.
        /// </summary>
        public CollectionBase<IConnection> Connections
        {
            get
            {
                CollectionBase<IConnection> cons = new CollectionBase<IConnection>();
                foreach (IDiagramEntity entity in mEntities)
                {
                    if (entity is IConnection)
                        cons.Add(entity as IConnection);
                }
                return cons;
            }
        }

        
        /// <summary>
        /// Gets or sets the Model
        /// </summary>
        public IModel Model
        {
            get { return mModel; }
            set {
                if (value == null)
                    throw new InconsistencyException("The model you want to set has value 'null'.");
            
                mModel = value;
                foreach (IDiagramEntity entity in mEntities)
                    entity.Model = value;
            }
        }


        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        /// <summary>
        /// Gets or sets the Entities
        /// </summary>
        public CollectionBase<IDiagramEntity> Entities
        {
            get { return mEntities; }
            //set { mEntities = value; }
        }

        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Layer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Layer(string name)
        {
            mName = name;
            mEntities = new CollectionBase<IDiagramEntity>();
            Init();
        }
        private void AttachToEntityCollection(CollectionBase<IDiagramEntity> collection)
        {
            collection.OnItemAdded += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemAdded);
            collection.OnItemRemoved += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemRemoved);
            collection.OnClear += new EventHandler(mEntities_OnClear);
        }
        /// <summary>
        /// Initializes this object
        /// <remarks>See also the <see cref="OnDeserialized"/> event for post-deserialization actions to which this method is related.
        /// </remarks>
        /// </summary>
        private void Init()
        {
            if(mEntities == null)
                throw new InconsistencyException("The entity collection is 'null'");            
            AttachToEntityCollection(mEntities);
        }
        /// <summary>
        /// Handles the OnClear event of the Entities.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void mEntities_OnClear(object sender, EventArgs e)
        {
            EventHandler handler = OnClear;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Handles the OnItemRemoved event of the Entities
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void mEntities_OnItemRemoved(object sender, CollectionEventArgs<IDiagramEntity> e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityRemoved;
            if (handler != null)
                handler(this, new EntityEventArgs(e.Item));
        }

        /// <summary>
        /// Bubbles the event up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mEntities_OnItemAdded(object sender, CollectionEventArgs<IDiagramEntity> e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityAdded;
            if (handler != null)
                handler(this, new EntityEventArgs(e.Item));
        }
        #endregion  
    }
}
