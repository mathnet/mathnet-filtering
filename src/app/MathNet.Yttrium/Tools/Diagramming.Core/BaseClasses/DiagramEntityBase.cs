using System;
using System.Drawing;
using System.ComponentModel;

using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for every diagram entity.
    /// </summary>
    public abstract partial class DiagramEntityBase : IDiagramEntity, IDisposable
    {

        #region Events
        /// <summary>
        /// Occurs when the user click on the entity.
        /// </summary>
        public event EventHandler<EntityEventArgs> OnClick;
        /// <summary>
        /// Occurs when the mouse is pressed while over the entity
        /// </summary>
        public event EventHandler<EntityMouseEventArgs> OnMouseDown;
        /// <summary>
        /// Occurs when the entity's properties have changed
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityChange;
        /// <summary>
        /// Occurs when the entity is selected. This can be different than the OnClick
        /// because the selector can select and entity without clicking on it.
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntitySelect;
        #endregion

        #region Fields
        /// <summary>
        /// General prupose tag
        /// </summary>
        private object mTag;
        /// <summary>
        /// tells whether the current entity is mHovered by the mouse
        /// </summary>
        private bool mHovered;
        /// <summary>
        /// the control to which the eneity belongs
        /// </summary>
        private IModel model;
        /// <summary>
        /// tells whether the entity is selected
        /// </summary>
        private bool mIsSelected;
        /// <summary>
        /// the current draw style
        /// </summary>
        private IPenStyle mPenStyle;
        /// <summary>
        /// the current paint style
        /// </summary>
        private IPaintStyle mPaintStyle;
        /// <summary>
        /// the default pen to be used by the Paint method
        /// </summary>
        private Pen mPen;
        /// <summary>
        /// the default brush to be used by the Paint method
        /// </summary>
        private Brush mBrush;
        /// <summary>
        /// the name of the entity
        /// </summary>
        private string mName;
        /// <summary>
        /// a weak reference to the parent
        /// </summary>
        private WeakReference mParent;
        /// <summary>
        /// the scene index, i.e. the index of this entity in the scene-graph.
        /// </summary>
        private int mSceneIndex;
        /// <summary>
        /// the top-group to underneath which this entity resides.
        /// </summary>
        private IGroup mGroup;

        /// <summary>
        /// the Resizable field
        /// </summary>
        private bool mResizable = true;
        /// <summary>
        /// the unique identifier of this entity
        /// </summary>
        private Guid mUid = Guid.NewGuid();
        /// <summary>
        /// whether the entity is visible
        /// </summary>
        private bool mVisible = true;
        /// <summary>
        /// the Enabled field
        /// </summary>
        private bool mEnabled = true;
        
        #endregion

        #region Properties


        
        /// <summary>
        /// Gets or sets whether this entity is Enabled
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this entity is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        /// <summary>
        /// Gets or sets the drawing style.
        /// </summary>
        /// <value>The draw style.</value>
        public IPenStyle PenStyle
        {
            get { return mPenStyle; }
            set { mPenStyle = value;
            UpdatePaintingMaterial();
            }
        }
        /// <summary>
        /// Gets or sets the paint style.
        /// </summary>
        /// <value>The paint style.</value>
        public IPaintStyle PaintStyle
        {
            get { return mPaintStyle; }
            set { mPaintStyle = value;
            UpdatePaintingMaterial();
            }
        }

       

        /// <summary>
        /// Gets the globally unique identifier of this entity
        /// </summary>
        /// <value></value>
        public Guid Uid
        {
            get
            {
                return mUid;
            }
         
        }

        /// <summary>
        /// Gets or sets the Resizable
        /// </summary>
        public bool Resizable
        {
            get { return mResizable; }
            set { mResizable = value; }
        }
        /// <summary>
        /// Gets the <see cref="Brush"/> to paint this entity.
        /// </summary>
        public Brush Brush
        {
            get { return mBrush; }
        }

        /// <summary>
        /// Gets the pen to draw this entity.
        /// </summary>
        /// <value>The pen.</value>
        public Pen Pen
        {
            get { return mPen; }
        }

        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public abstract string EntityName { get;}
        /// <summary>
        /// Gets or sets the general purpose tag
        /// </summary>
        public object Tag
        {
            get { return mTag; }
            set { mTag = value; }
        }
        /// <summary>
        /// Gets or sets the index of this entity in the scene-graph.
        /// </summary>
        /// <value>The index of the scene.</value>
        public int SceneIndex
        {
            get { return mSceneIndex; }
            set { mSceneIndex = value; }
        }
        /// <summary>
        /// Gets or sets the unique top-group to which this entity belongs.
        /// </summary>
        /// <value></value>
        public IGroup Group
        {
            get { return mGroup; }
            set
            {
                mGroup = value;
                //propagate downwards if this is a group shape, but not if the value is 'null' since
                //the group becomes the value of the Group property
                //Note that we could have used a formal depth-traversal algorithm.
                if (this is IGroup)
                {
                    if (value == null)//occurs on an ungroup action
                    {
                        foreach (IDiagramEntity entity in (this as IGroup).Entities)
                        {
                            entity.Group = this as IGroup;
                        }
                    }
                    else //occurs when grouping
                    {
                        foreach (IDiagramEntity entity in (this as IGroup).Entities)
                        {
                            entity.Group = value;
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Gets or sets whether the entity is hovered by the mouse
        /// </summary>
        public bool Hovered
        {
            get { return mHovered; }
            set
            {
                mHovered = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the parent of the entity
        /// </summary>
        public object Parent
        {
            get
            {
                if (mParent != null && mParent.IsAlive)
                {
                    return mParent.Target;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                mParent = new WeakReference(value);
            }
        }
        /// <summary>
        /// Gets or sets the name of the entity
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        /// <summary>
        /// The bounds of the paintable entity
        /// </summary>
        /// <value></value>
        public abstract Rectangle Rectangle { get; }
        /// <summary>
        /// Gets or sets whether the entity is selected
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; }
        }
        /// <summary>
        /// Gets or sets the canvas to which the entity belongs
        /// </summary>
        [Browsable(false)]
        public virtual IModel Model
        {
            get { return model; }
            set { model = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor with the model of the entity
        /// </summary>
        /// <param mName="model"></param>
        protected DiagramEntityBase(IModel model)
        {
            this.model = model;
        }
        /// <summary>
        /// The empty constructor is required to make deserialization work
        /// </summary>
        protected DiagramEntityBase() { }        
        #endregion

        #region Methods
        /// <summary>
        /// Generates a new Uid for this entity.
        /// </summary>
        /// <param name="recursive">if the Uid has to be changed recursively down to the sub-entities, set to true, otherwise false.</param>
        public virtual void NewUid(bool recursive)
        {
            this.mUid = Guid.NewGuid();
        }
        /// <summary>
        /// Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            return null;
        }
        /// <summary>
        /// Paints the entity on the control
        /// </summary>
        /// <param mName="g">the graphics object to paint on</param>
        public abstract void Paint(Graphics g);
        /// <summary>
        /// Tests whether the entity is hit by the mouse
        /// </summary>
        /// <param>a Point location</param>
        /// <param name="p"></param>
        public abstract bool Hit(Point p);
        /// <summary>
        /// Invalidates the entity
        /// </summary>
        public abstract void Invalidate();
        /// <summary>
        /// Invalidates a rectangle of the canvas
        /// </summary>
        /// <param name="rectangle"></param>
        public void Invalidate(Rectangle rectangle)
        {
            if (Model != null)
                Model.RaiseOnInvalidateRectangle(rectangle);
        }
        /// <summary>
        /// Updates pens and brushes
        /// </summary>
        protected virtual void UpdatePaintingMaterial()        
        {
            if(mPenStyle!=null)
                mPen = mPenStyle.DrawingPen();
            if(mPaintStyle!=null)
                mBrush = mPaintStyle.GetBrush(this.Rectangle);
            Invalidate();
        }
        /// <summary>
        /// Moves the entity on the canvas
        /// </summary>
        /// <param mName="p">the shifting vector, not an absolute position!</param>
        public abstract void Move(Point p);

        /// <summary>
        /// The custom elements to be added to the menu on a per-entity basis
        /// </summary>
        /// <returns></returns>
        public abstract MenuItem[] ShapeMenu();

        #region Raisers
        /// <summary>
        /// Raises the onclick event.
        /// </summary>
        /// <param name="e"></param>
        public void RaiseOnClick(EntityEventArgs e)
        {
            if (OnClick != null)
                OnClick(this, e);
        }
        /// <summary>
        /// Raises the onmousedown event
        /// </summary>
        /// <param name="e"></param>
        public void RaiseOnMouseDown(EntityMouseEventArgs e)
        {
            if (OnMouseDown != null)
                OnMouseDown(this, e);
        }

        /// <summary>
        /// Raises the OnSelect event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        protected internal void RaiseOnSelect(object sender, EntityEventArgs e)
        {
            if (OnEntitySelect != null)
                OnEntitySelect(sender, e);
        }
        /// <summary>
        /// Raises the OnChange event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        protected internal void RaiseOnChange(object sender, EntityEventArgs e)
        {
            if (OnEntityChange != null)
                OnEntityChange(sender, e);
        } 
        #endregion

        #endregion

        #region Standard IDispose implementation
        /// <summary>
        /// Disposes the entity.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);


        }

        /// <summary>
        /// Disposes the entity.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                #region free managed resources


                if (mPen != null)
                {
                    mPen.Dispose();
                    mPen = null;
                }
                if (mBrush != null)
                {
                    mBrush.Dispose();
                    mBrush = null;
                }
                #endregion
            }

        }

        #endregion

    }
}
