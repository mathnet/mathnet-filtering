using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// A bundle is a collection of diagram entities which does not necessarily has a place in the scene graph but merely serves
    /// to bundle items together. Various actions and functions are made possible by bundles; cut/copy/paste, templates, user selections and so on.
    /// While a group can have sub-groups, bundles cannot be contained in another and are not necessarily connected (in the sense of a connected branch in the scene graph).
    /// </summary>
    [Serializable()]
    public class Bundle : DiagramEntityBase, IBundle, ISerializable
    {

        #region Fields
        /// <summary>
        /// the Shapes field
        /// </summary>
        private CollectionBase<IDiagramEntity> mEntities;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The m entities.</value>
        public CollectionBase<IDiagramEntity> Entities
        {
            get
            {
                return mEntities;
            }
        }
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public override string EntityName
        {
            get { return "bundle"; }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public override Rectangle Rectangle
        {
            get {
                System.Drawing.Rectangle union = System.Drawing.Rectangle.Empty;
                foreach (IDiagramEntity entity in mEntities)
                {
                    union = System.Drawing.Rectangle.Union(union,entity.Rectangle);
                }
                //maybe this should be inflated a little here?
                return union;
            }
             //set { throw new InconsistencyException("The rectangle of a bundle is the union of its constituents and cannot be set."); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Bundle"/> class.
        /// </summary>
        public Bundle(IModel model) : base(model)
        {
            mEntities = new CollectionBase<IDiagramEntity>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Bundle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Bundle(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Bundle"/> class.
        /// </summary>
        public Bundle() {
            mEntities = new CollectionBase<IDiagramEntity>();
        }

        /// <summary>
        /// Creates a new bundle using a given collection of diagram entities.
        /// </summary>
        /// <param name="collection"></param>
        public Bundle(CollectionBase<IDiagramEntity> collection)
        {
            
            mEntities = new CollectionBase<IDiagramEntity>();
            //we could assign it directly but let's make sure the collection does not
            //contain unwanted elements
            foreach (IDiagramEntity entity in collection)
            {
                if ((entity is IShape) || (entity is IConnection) || (entity is IGroup))
                    mEntities.Add(entity);
            }
             
            //the following line would give problem. The event handler attached to the Selection would be triggered when
            //the mEntities collection is changed!
            //mEntities = collection;

        }

        #endregion

        #region Methods
        /// <summary>
        /// The custom menu to be added to the base menu of this entity
        /// </summary>
        /// <returns></returns>
        public override  MenuItem[] ShapeMenu()
        {
            return null;
        }

      

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Paints the entity on the control
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(System.Drawing.Graphics g)
        {
            foreach (IDiagramEntity entity in mEntities)
            {
                entity.Paint(g);
            }
        }

        /// <summary>
        /// Tests whether the bundle is hit by the mouse
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            foreach (IDiagramEntity entity in mEntities)
            {
                if (entity.Hit(p)) return true;
            }
            return false;
        }

        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public override void Invalidate()
        {
            foreach (IDiagramEntity entity in mEntities)
            {
                entity.Invalidate();
            }
        }

        /// <summary>
        /// Moves the entity on the canvas
        /// </summary>
        /// <param name="p">a shift vector</param>
        public override void Move(System.Drawing.Point p)
        {
            foreach (IDiagramEntity entity in mEntities)
            {
                entity.Move(p);
            }
        }
      
       
        #endregion
    }
}
