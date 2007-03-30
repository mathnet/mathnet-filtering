using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implementation of the <see cref="IPage"/> interface. The page represents one page the diagram control, pages being usually visualized as tabs in the control.
    /// 
    /// </summary>
    partial class Page : IPage
    {

        #region Events
        /// <summary>
        /// Occurs when the Ambience has changed
        /// </summary>
        public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        /// <summary>
        /// Occurs when an entity is added.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the page is cleared
        /// </summary>
        public event EventHandler OnClear;
        /// <summary>
        /// Occurs before an entity is transformed
        /// </summary>
        public event EventHandler<CancelableEntityEventArgs> OnBeforeResize;
        #endregion

        #region Fields
        /// <summary>
        /// the Name field
        /// </summary>
        private string mName;
        /// <summary>
        /// the default layer
        /// </summary>
        [NonSerialized]
        private ILayer mDefaultLayer;
        /// <summary>
        /// the Layers field
        /// </summary>
        private CollectionBase<ILayer> mLayers;
        /// <summary>
        /// the Ambience field
        /// </summary>
        private Ambience mAmbience;
        #endregion

        #region Properties
        /// <summary>
        /// Gets all shapes in this page.
        /// </summary>
        public CollectionBase<IShape> Shapes
        {
            get {
                CollectionBase<IShape> shapes = new CollectionBase<IShape>();
                foreach (ILayer  layer in mLayers)
                {
                    shapes.AddRange(layer.Shapes);
                }
                return shapes;
            }
        }
        /// <summary>
        /// Gets all connections in this page.
        /// </summary>
        public CollectionBase<IConnection> Connections
        {
            get
            {
                CollectionBase<IConnection> cons = new CollectionBase<IConnection>();
                foreach (ILayer layer in mLayers)
                {
                    cons.AddRange(layer.Connections);
                }
                return cons;
            }
        }

        /// <summary>
        /// Gets or sets the type of the background.
        /// </summary>
        /// <value>The type of the background.</value>
        [Browsable(true), Description("The background type"), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public CanvasBackgroundTypes BackgroundType
        {
            get
            {
                return Ambience.BackgroundType;
            }
            set
            {
                Ambience.BackgroundType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ambience
        /// </summary>
        public Ambience Ambience
        {
            get
            {
                return mAmbience;
            }
            set
            {
                mAmbience = value;
                RaiseOnAmbienceChanged(new AmbienceEventArgs(mAmbience));
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the back.</value>
        [Browsable(true), Description("The background color of the canvas if the type is set to 'flat'"), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor
        {
            get
            {
                return Ambience.BackgroundColor;
            }
            set
            {
                Ambience.BackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets the default layer.
        /// </summary>
        /// <value>The default layer.</value>
        public ILayer DefaultLayer
        {
            get
            {
                return mDefaultLayer;
            }
        }
        /// <summary>
        /// Gets or sets the Layers
        /// </summary>
        public CollectionBase<ILayer> Layers
        {
            get
            {
                return mLayers;
            }
            set
            {
                mLayers = value;
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
        /// the Model field
        /// </summary>
        private IModel mModel;
        /// <summary>
        /// Gets or sets the Model
        /// </summary>
        public IModel Model
        {
            get
            {
                return mModel;
            }
            set
            {
                if (value == null)
                    throw new InconsistencyException("The model you want to set has value 'null'.");
                mModel = value;
                //this will cascade the setting down the hierarchy
                foreach (ILayer layer in mLayers)
                    layer.Model = value;
            }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public Page(string name, IModel model)
        {
            this.mModel = model;

            mName = name;
            mAmbience = new Ambience(this);
            

            //the one and only and indestructible layer
            
            mLayers = new CollectionBase<ILayer>();
            mLayers.Add(new Layer("Default Layer")); 
            
            Init();

            
        }
        /// <summary>
        /// Initializes this object
        /// <remarks>See also the <see cref="OnDeserialized"/> event for post-deserialization actions to which this method is related.
        /// </remarks>
        /// </summary>
        private void Init()
        {
            if (mLayers == null || mLayers.Count == 0)
                throw new InconsistencyException("The page object does not contain the expected default layer.");
            mDefaultLayer = mLayers[0];
                       
            //listen to events
            AttachToAmbience(mAmbience);

            foreach(ILayer layer in mLayers)
                AttachToLayer(layer);
        }
        private void AttachToLayer(ILayer layer)
        {
            layer.OnEntityAdded += new EventHandler<EntityEventArgs>(defaultLayer_OnEntityAdded);
            layer.OnEntityRemoved += new EventHandler<EntityEventArgs>(mDefaultLayer_OnEntityRemoved);
            layer.OnClear += new EventHandler(mDefaultLayer_OnClear);
        }
        /// <summary>
        /// Attaches the model to the ambience class.
        /// </summary>
        /// <param name="ambience">The ambience.</param>
        private void AttachToAmbience(Ambience ambience)
        {
            if(ambience == null)
                throw new ArgumentNullException("The ambience object assigned to the model cannot be 'null'");

            mAmbience.OnAmbienceChanged += new EventHandler<AmbienceEventArgs>(mAmbience_OnAmbienceChanged);
        }
        /// <summary>
        /// Raises the <see cref="OnAmbienceChanged"/> event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseOnAmbienceChanged(AmbienceEventArgs e)
        {
            EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handles the OnClear event of the DefaultLayer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void mDefaultLayer_OnClear(object sender, EventArgs e)
        {
            EventHandler handler = OnClear;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Handles the OnEntityRemoved event of the DefaultLayer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        void mDefaultLayer_OnEntityRemoved(object sender, EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityRemoved;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Handles the OnEntityAdded event of the defaultLayer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        void defaultLayer_OnEntityAdded(object sender, EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityAdded;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// Handles the OnAmbienceChanged event of the <see cref="Ambience"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.AmbienceEventArgs"/> instance containing the event data.</param>
        void mAmbience_OnAmbienceChanged(object sender, AmbienceEventArgs e)
        {
            //pass on the good news, eventually the View will be notified and the canvas will be redrawn
            RaiseOnAmbienceChanged(e);
        }
        #endregion
  
    }
}
