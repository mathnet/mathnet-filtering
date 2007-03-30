using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Abstract base class for a connection
	/// </summary>
	public abstract partial class ConnectionBase : DiagramEntityBase, IConnection
	{
		#region Fields

		/// <summary>
		/// the start connector
		/// </summary>
		private IConnector mFrom;
		/// <summary>
		/// the end connector
		/// </summary>
		private IConnector mTo;

		#endregion

		#region Properties
		/// <summary>
		/// The initial or start connector
		/// </summary>
		public IConnector From
		{
			get{return mFrom;}
			set{mFrom = value;}
		}
		/// <summary>
		/// The final or end connector
		/// </summary>
		/// <example>
		/// <code>
		/// Connection connection
		/// </code>
		/// </example>
		public IConnector To
		{
			get{return mTo;}
			set{mTo = value;}
		}


      

		#endregion

		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="site"></param>
		protected ConnectionBase(IModel site) : base(site)
		{
            PenStyle = ArtPallet.GetDefaultPenStyle();

		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionBase"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        protected ConnectionBase(Point from, Point to) : base()
        {
            this.mFrom = new Connector(from);
            this.mFrom.Parent = this;
            this.mTo = new Connector(to);
            this.mTo.Parent = this;
            PenStyle = ArtPallet.GetDefaultPenStyle();
        }

        public ConnectionBase() : base() { }
        
		#endregion

        #region Methods
        /// <summary>
        /// Paints the entity on the control
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("The Graphics object is 'null'");
            From.Paint(g);
            To.Paint(g);
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
        /// Invalidates the entity
        /// </summary>
        public override void Invalidate()
        {
            

            Rectangle r = Rectangle;
            r.Offset(-10, -10);
            r.Inflate(40, 40);
            if (Model != null)
                Model.RaiseOnInvalidateRectangle(r);
        }
       
        #endregion

    }
}
