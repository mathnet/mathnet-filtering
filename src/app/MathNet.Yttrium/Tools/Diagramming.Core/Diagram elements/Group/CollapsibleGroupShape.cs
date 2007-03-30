using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This is an extension of the <see cref="GroupShape"/> which paints itself and allows to collapse/expand
    /// its content. Note that it does not inherit from the <see cref="GroupShape"/> though because it requires 
    /// the <see cref="ShapeMaterialBase"/> mechanism and, hence, inherits from the <see cref="ComplexShapeBase"/> class.
    /// </summary>
    partial class CollapsibleGroupShape : ComplexShapeBase, IGroup
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
        private bool mEmphasizeGroup = true;
        /// <summary>
        /// the collapse/expand icon
        /// </summary>
        private SwitchIconMaterial xicon;
        /// <summary>
        /// the rectangle before it's being collapsed
        /// </summary>
        private Rectangle rectangleMemory;
        /// <summary>
        /// the location memory of the externally connected connectors
        /// </summary>
        private Dictionary<IConnector, Point> connectorMemory;
        /// <summary>
        /// global offset when a collapsed group is moved around
        /// </summary>
        private Point groupConnectorOffset = Point.Empty;
        /// <summary>
        /// the Collapsed field
        /// </summary>
        private bool mCollapsed;
        /// <summary>
        /// the expand hyperlink
        /// </summary>
        private ClickableLabelMaterial expandLabel;
        #endregion

        #region Properties



        /// <summary>
        /// Gets or sets whether the group is in a collapsed state
        /// </summary>
        public bool Collapsed
        {
            get { return mCollapsed; }
            set { mCollapsed = value; }
        }


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
                return mRectangle;
            }
        }
        /// <summary>
        /// Gets or sets the entities directly underneath. To get the whole branch of entities in case of nested groups, <see cref="Leafs"/>.
        /// </summary>
        public CollectionBase<IDiagramEntity> Entities
        {
            get { return mEntities; }
            set
            {

                throw new InconsistencyException("You cannot set the entities, use the already instantiated collection to add or remove items.");


            }
        }
        /// <summary>
        /// Gets the whole branch of entities if this group has sub-groups. Contrary to the <see cref="GroupShape"/> the result WILLl contain the group shapes.
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
                    {                        
                        Utils.TraverseCollect(entity as IGroup, ref flatList);
                    }
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
        public CollapsibleGroupShape(IModel model)
            : base(model)
        {
            this.mEntities = new CollectionBase<IDiagramEntity>();
            this.mEntities.OnItemAdded += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemAdded);
            this.mEntities.OnClear += new EventHandler(mEntities_OnClear);
            this.mEntities.OnItemRemoved += new EventHandler<CollectionEventArgs<IDiagramEntity>>(mEntities_OnItemRemoved);

            xicon = new SwitchIconMaterial(SwitchIconType.PlusMinus);
            this.Children.Add(xicon);
            //do this before attaching the next event handler, otherwise the shape will be added twice to the paintables
            xicon.Collapsed = false;

            xicon.OnCollapse += new EventHandler(xicon_OnCollapse);
            xicon.OnExpand += new EventHandler(xicon_OnExpand);

            connectorMemory = new Dictionary<IConnector, Point>();

            expandLabel = new ClickableLabelMaterial("Expand...");
            this.Children.Add(expandLabel);
            expandLabel.Visible = false;
            expandLabel.OnClick += new EventHandler(expandLabel_OnClick);
        }

        void expandLabel_OnClick(object sender, EventArgs e)
        {
            xicon.Collapsed = false;
        }

        void xicon_OnExpand(object sender, EventArgs e)
        {
            Expand();
        }

        void xicon_OnCollapse(object sender, EventArgs e)
        {
            Collapse();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Collapses this instance.
        /// </summary>
        public void Collapse()
        {
            rectangleMemory = mRectangle;
            mRectangle = new Rectangle(mRectangle.Location, new Size(110, 25));

            foreach (IDiagramEntity entity in Entities)
            {
                RemoveFromPaintables(entity);
            }
            this.Resizable = false;
          

            connectorMemory.Clear();//forget the previous memory
            groupConnectorOffset = Point.Empty;
            //use the flattened collection (Leafs) in case there are sub-groups
            MoveConnectors(Leafs, new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom));

            Rectangle rec = rectangleMemory;
            rec.Inflate(20, 20);
            Invalidate(rec);
            mCollapsed = true;
            expandLabel.Visible = true;
        }
        internal void RemoveFromPaintables(IDiagramEntity entity)
        {
            this.Model.Paintables.Remove(entity);
            if (entity is CollapsibleGroupShape && !(entity as CollapsibleGroupShape).Collapsed)
            {
                foreach (IDiagramEntity  ent in (entity as CollapsibleGroupShape).Entities)
                {
                    RemoveFromPaintables(ent);
                }
                
            }
        }

        internal void AddToPaintables(IDiagramEntity entity)
        {
            this.Model.Paintables.Add(entity);
            if (entity is CollapsibleGroupShape && !(entity as CollapsibleGroupShape).Collapsed)
            {
                foreach (IDiagramEntity ent in (entity as CollapsibleGroupShape).Entities)
                {
                    AddToPaintables(ent);
                }

            }

        }
        /// <summary>
        /// Expands this instance.
        /// </summary>
        public void Expand()
        {
            mRectangle = rectangleMemory;
            foreach (IDiagramEntity entity in Entities)
            {
                AddToPaintables(entity);
            }
            this.Resizable = true;
            UnMoveConnectors();
            Invalidate(mRectangle);
            mCollapsed = false;
            connectorMemory.Clear();
            expandLabel.Visible = false;
        }

    

        /// <summary>
        /// Moves the connectors of the children to the central group connector location.
        /// </summary>
        internal void MoveConnectors(CollectionBase<IDiagramEntity> entities, Point point)
        {
            IConnection cnn;
            foreach (IDiagramEntity entity in entities)
            {
                if (entity is IGroup)
                {
                    continue; //since we use a flattened collection (the Leafs) we don't care about the groups here
                    //the inclusion of the subgroups in the Leafs is however important to make the subgroups
                    //(in)visible when collapsed/expanded.
                }
                if (entity is IShape )
                {
                    foreach (IConnector cn in (entity as IShape).Connectors)
                    {
                        if (cn.AttachedConnectors.Count > 0)
                        {
                            foreach (IConnector cn2 in cn.AttachedConnectors)
                            {
                                if (cn2.Parent is IConnection)
                                {
                                    cnn = cn2.Parent as IConnection;
                                    //the ends have to be connected
                                    if (cnn.From.AttachedTo.Parent is IShape && cnn.To.AttachedTo.Parent is IShape)
                                    {
                                        if (entities.Contains(cnn.From.AttachedTo.Parent as IShape) && entities.Contains(cnn.To.AttachedTo.Parent as IShape))
                                            continue;//both endconnectors are internal
                                        else//one of the connectors is external
                                        {
                                            if (entities.Contains(cnn.From.AttachedTo.Parent as IShape)) //the From is internal
                                            {
                                                MoveConnector(cnn.From, point);
                                            }
                                            else //the To is internal                            
                                            {
                                                MoveConnector(cnn.To, point);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves the connector to the given location.
        /// </summary>
        /// <param name="cn">The cn.</param>
        /// <param name="point">The point.</param>
        private void MoveConnector(IConnector cn, Point point)
        {
            connectorMemory.Add(cn, cn.Point);//luckily the Point is a struct so the value will be actually copied and we can safely change the value next
            cn.Move(new Point(point.X - cn.Point.X, point.Y - cn.Point.Y));
            cn.Enabled = false;
        }

        /// <summary>
        /// Moves the connectors back from the central group connector to their original location.
        /// </summary>
        private void UnMoveConnectors()
        {
            //some things will go wrong here if the content of the Entities has changed

            //recall the memory and move them;

            Dictionary<IConnector, Point>.KeyCollection keys = connectorMemory.Keys;
            Point p;
            foreach (IConnector cn in keys)
            {
                p = connectorMemory[cn];
                cn.Move(new Point(p.X - cn.Point.X + groupConnectorOffset.X, p.Y - cn.Point.Y + groupConnectorOffset.Y));
                cn.Enabled = true;
            }
        }

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
            //if(mEntities.Count == 1)
            //    mRectangle = e.Item.Rectangle;
            //else
            //{
            //    mRectangle = Rectangle.Union((Rectangle) mRectangle, e.Item.Rectangle);
            //}
            CalculateRectangle();
        }

        /// <summary>
        /// Calculates the bounding rectangle of this group.
        /// </summary>
        public void CalculateRectangle()
        {
            if (mEntities == null || mEntities.Count == 0)
                return;
            Rectangle rec = mEntities[0].Rectangle;
            foreach (IDiagramEntity entity in Entities)
            {
                //cascade the calculation if necessary
                if ((entity is CollapsibleGroupShape) && !(entity as CollapsibleGroupShape).Collapsed) (entity as IGroup).CalculateRectangle();

                rec = Rectangle.Union(rec, entity.Rectangle);
            }
            rec.Inflate(20, 20);
            this.mRectangle = rec;
            rec.Offset(5, 5);
            xicon.Transform(new Rectangle(rec.Location, new Size(16, 16)));

            expandLabel.Transform(new Rectangle(rec.Location.X + 20, rec.Location.Y,50, 12));
        }


        /// <summary>
        /// Paints the entity on the control
        /// <remarks>This method should not be called since the painting occurs via the <see cref="Model.Paintables"/>.</remarks>
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(System.Drawing.Graphics g)
        {
            Rectangle rec = Rectangle;
            Utils.DrawRoundRect(g, ArtPallet.ConnectionShadow, rec);

            base.Paint(g);
        }

        /// <summary>
        /// Tests whether the group is hit by the mouse
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            if (mCollapsed)
                return mRectangle.Contains(p);

            foreach (IDiagramEntity entity in mEntities)
            {
                if (entity.Hit(p))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public override void Invalidate()
        {

            if (mRectangle == null)
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
            base.Move(p);

            Rectangle recBefore = mRectangle;
            recBefore.Inflate(20, 20);

            //no need to invalidate since it'll be done by the individual move actions
            foreach (IDiagramEntity entity in mEntities)
            {
                entity.Move(p);
            }

            mRectangle.X += p.X;
            mRectangle.Y += p.Y;
            //shift the latent copy of the real rectangle if the group is in a collapsed state
            if (mCollapsed)
            {
                rectangleMemory.X += p.X;
                rectangleMemory.Y += p.Y;
                //the global offset has to be memorized; when we'll expand the group this offset has to be added to the original values
                groupConnectorOffset.Offset(p);
            }


            //refresh things
            this.Invalidate(recBefore);//position before the move
            this.Invalidate();//current position

        }


        #endregion
    }


}
