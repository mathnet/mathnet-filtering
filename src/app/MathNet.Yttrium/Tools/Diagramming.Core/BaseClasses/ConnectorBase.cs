using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Abstract base class for a connector.
	/// </summary>
	public abstract partial class ConnectorBase : DiagramEntityBase, IConnector
	{
		#region Fields
		/// <summary>
		/// the location of this connector
		/// </summary>
		private Point mPoint;
		/// <summary>
		/// the connectors attached to this connector
		/// </summary>
        private CollectionBase<IConnector> attachedConnectors;
		/// <summary>
		/// the connector, if any, to which this connector is attached to
		/// </summary>
		private IConnector attachedTo;
		
		#endregion

		#region Properties

		/// <summary>
		/// Gets the attached connectors to this connector
		/// </summary>
        public CollectionBase<IConnector> AttachedConnectors
		{
			get{return attachedConnectors;}		
		}
	

		/// <summary>
		/// If the connector is attached to another connector
		/// </summary>
		public IConnector AttachedTo
		{		
			get{return attachedTo;}
			set{attachedTo = value;}
		}

		/// <summary>
		/// The location of this connector
		/// </summary>
		public Point Point
		{
			get{return mPoint;}
			set{
                //throw new NotSupportedException("Use the Move() method instead.");

             mPoint = value;
            //foreach(IConnector con in  attachedConnectors)
            //{
            //    con.Point = value;
            //}
            }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Default connector
		/// </summary>
        protected ConnectorBase(IModel model)
            : base(model)
		{
            attachedConnectors = new CollectionBase<IConnector>();
		}

        /// <summary>
        /// Constructs a connector, passing its location
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="model">The model.</param>
        protected ConnectorBase(Point p, IModel model)
            : base(model)
		{
            attachedConnectors = new CollectionBase<IConnector>();
			mPoint = p;
		}
      
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectorBase"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        protected ConnectorBase(Point p) : base()
        {
            attachedConnectors = new CollectionBase<IConnector>();
            this.mPoint = p;
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
		/// Attaches the given connector to this connector. 
        /// <remarks>The method will remove a previous binding, if any, before creating the attachment. This method prohibits mutliple identical connections; 
        /// you can attach a connector only once.
        /// </remarks>
		/// </summary>
		/// <param name="connector"></param>
		public void AttachConnector(IConnector connector)
		{
            if(connector == null || !Enabled)
                return;
            //only attach'm if not already present and not the parent
            if(!attachedConnectors.Contains(connector) && connector!=attachedTo)
            {
                connector.DetachFromParent();
                attachedConnectors.Add(connector);
                //make sure the attached connector is centered at this connector
                connector.Point = this.mPoint;
                connector.AttachedTo = this;

                Model.NotifyConnectorAttached(this, connector);
            }
            
		}

		/// <summary>
		/// Detaches the given connector from this connector. No exception will be thrown if the given connector is not a child.
        /// 
		/// </summary>
		/// <param name="connector"></param>
		public void DetachConnector(IConnector connector)
		{
            if(connector == null || !Enabled)
                return;

            if(attachedConnectors.Contains(connector))
            {
                attachedConnectors.Remove(connector);
                connector.AttachedTo = null;

                Model.NotifyConnectorDetached(this, connector);
            }
		}

        /// <summary>
        /// Detaches from its parent (if any). No exception will be thrown if this connector is not attached to any parent.
        /// </summary>
        public void DetachFromParent()
        {
            if(this.AttachedTo != null && Enabled)
            {
                this.AttachedTo.AttachedConnectors.Remove(this);
                this.AttachedTo = null;
            }

        }

        /// <summary>
        /// Attaches to another connector. 
        /// <remarks>If this connector is already attached it will be detached first and not exception will be thrown.
        /// The method does not allow mutliplt connections; you cannot attach to a parent if it already does
        /// </remarks>
        /// </summary>
        /// <param name="parent">The new parent connector.</param>
        public void AttachTo(IConnector parent)
        {

            if(parent == null || !Enabled)
                return;
            //donnot re-attach and the parent cannot be an already attached child
            if(this.AttachedTo != parent && !AttachedConnectors.Contains(parent))
            {
                //remove any other binding
                DetachFromParent();
                //attach to the given parent
                parent.AttachedConnectors.Add(this);
                this.AttachedTo = parent;
            }
        }
		

		
		
		#endregion

    }
}
