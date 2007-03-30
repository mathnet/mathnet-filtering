using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This entity represents a group of entities and corresponds to the '(un)grouping' feature.
    /// </summary>
    partial class GroupShape : DiagramEntityBase, IGroup
    {
        #region Fields
        /// <summary>
        /// the Entities field
        /// </summary>
        private CollectionBase<IDiagramEntity> mEntities;
        /// <summary>
        /// the bounding rectangle
        /// </summary>
        private Rectangle mRectangle;
        /// <summary>
        /// in essence, whether the group shape should be painted
        /// </summary>
        private bool mEmphasizeGroup = false;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the group as a shape should be painted on the canvas.
        /// </summary>
        /// <value><c>true</c> to paint the group shape; otherwise, <c>false</c>.</value>
        public bool EmphasizeGroup
        {
            get { return mEmphasizeGroup; }
            set { mEmphasizeGroup = value; }
        }
        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get
            {
                return "Group shape";
            }
        }

        /// <summary>
        /// Gets or sets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public override Rectangle Rectangle
        {
            get
            {            
                return  mRectangle;
            }
        }
        /// <summary>
        /// Gets or sets the entities directly underneath. To get the whole branch of entities in case of nested groups, <see cref="Leafs"/>.
        /// </summary>
        public CollectionBase<IDiagramEntity> Entities
        {
            get { return mEntities; }
            set {
                
                    throw new InconsistencyException("You cannot set the entities, use the already instantiated collection to add or remove items.");

                
            }
        }
        /// <summary>
        /// Gets the whole branch of entities if this group has sub-groups. The result will not contain the group shapes.
        /// </summary>
        /// <value>The branch.</value>
        public CollectionBase<IDiagramEntity> Leafs
        {
            get
            {
                CollectionBase<IDiagramEntity> flatList = new CollectionBase<IDiagramEntity>();
                foreach (IDiagramEntity entity in mEntities)
                {
                    if (entity is IGroup)
                        Utils.TraverseCollect(entity as IGroup, ref flatList);
                    else
                        flatList.Add(entity);
                }
                return flatList;
            }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public GroupShape(IModel model) : base(model)
        {
            this.mEntities = new CollectionBase<IDiagramEntity>();
            this.mEntities.OnItemAdded += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemAdded);
            this.mEntities.OnClear += new EventHandler(mEntities_OnClear);
            this.mEntities.OnItemRemoved += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemRemoved);
        }
        #endregion

        #region Methods
        /// <summary>
        /// The custom menu to be added to the base menu of this entity
        /// </summary>
        /// <returns></returns>
        public override MenuItem[] ShapeMenu()
        {
            return null;
        }
        /// <summary>
        /// Handles the OnItemRemoved of the Entities
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void mEntities_OnItemRemoved(object sender, CollectionEventArgs<IDiagramEntity> e)
        {
            CalculateRectangle();
        }

        /// <summary>
        /// Handles the OnClear event of the Entities.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void mEntities_OnClear(object sender, EventArgs e)
        {
            mRectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Handles the OnItemAdded event of the Entities
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void mEntities_OnItemAdded(object sender, CollectionEventArgs<IDiagramEntity> e)
        {
            if(mEntities.Count == 1)
                mRectangle = e.Item.Rectangle;
            else
            {
                mRectangle = Rectangle.Union((Rectangle) mRectangle, e.Item.Rectangle);
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle of this group.
        /// </summary>
        public  void CalculateRectangle()
        {
            if (mEntities == null || mEntities.Count == 0)
                return;
            Rectangle rec = mEntities[0].Rectangle;                        
            foreach (IDiagramEntity entity in Entities)
            {
                //cascade the calculation if necessary
                if (entity is IGroup) (entity as IGroup).CalculateRectangle();

                rec = Rectangle.Union(rec, entity.Rectangle);
            }            
            this.mRectangle = rec;                      
        }


        /// <summary>
        /// Paints the entity on the control
        /// <remarks>This method should not be called since the painting occurs via the <see cref="Model.Paintables"/>.
        /// Use the <see cref="CollapsibleGroupShape"/> if you want a visible group.
        /// </remarks>
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(System.Drawing.Graphics g)
        {
            throw new InconsistencyException("This method should not be called since the painting occurs via the Paintables collection.");
        }

        /// <summary>
        /// Tests whether the group is hit by the mouse
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            foreach(IDiagramEntity entity in mEntities)
            {
                if(entity.Hit(p))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public override void Invalidate()
        {

            if(mRectangle == null) 
                return;

            Rectangle rec = mRectangle;
            rec.Inflate(20, 20);
            Model.RaiseOnInvalidateRectangle(rec);
        }

        /// <summary>
        /// Moves the entity on the canvas
        /// </summary>
        /// <param name="p"></param>
        public override void Move(System.Drawing.Point p)
        {
            
            Rectangle recBefore = mRectangle;
            recBefore.Inflate(20, 20);

            //no need to invalidate since it'll be done by the individual move actions
            foreach(IDiagramEntity entity in mEntities)
            {
                entity.Move(p);
            }
            mRectangle.X += p.X;
            mRectangle.Y += p.Y;

            //refresh things
            this.Invalidate(recBefore);//position before the move
            this.Invalidate();//current position

        }
      

        #endregion
    }
}
