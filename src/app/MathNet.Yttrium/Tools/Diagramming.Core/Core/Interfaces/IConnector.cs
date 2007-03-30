using System;
using System.Drawing;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This interface describes the public members of a connector
	/// </summary>
	public interface IConnector : IDiagramEntity
	{
		#region Properties
		/// <summary>
		/// Gets the point at which the connector is located
		/// </summary>
		Point Point {get; set;}
		/// <summary>
		/// Gets or sets the connector to which this connector is attached to
		/// </summary>
		IConnector AttachedTo {get; set;}
		/// <summary>
		/// Gets the connectors (if any) attached to this connector
		/// </summary>
		CollectionBase<IConnector> AttachedConnectors {get;}
		#endregion

		#region Methods
        /// <summary>
        /// Attaches the given connector to this one
        /// </summary>
        /// <param name="child">The child connector to attach.</param>		
		void AttachConnector(IConnector child);

        /// <summary>
        /// Detaches the given connector from this connector.
        /// </summary>
        /// <param name="child">The child connector to detach.</param>        
        void DetachConnector(IConnector child);

        /// <summary>
        /// Detaches from its parent (if any).
        /// </summary>
        void DetachFromParent();

        /// <summary>
        /// Attaches to another connector
        /// </summary>
        /// <param name="parent">The new parent connector.</param>        
        void AttachTo(IConnector parent);
		#endregion
	}
}
