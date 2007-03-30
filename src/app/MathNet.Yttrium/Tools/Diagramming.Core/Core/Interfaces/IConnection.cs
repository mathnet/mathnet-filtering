
namespace Netron.Diagramming.Core {
	/// <summary>
	/// This interface sets the members of a connection.
	/// </summary>
	public interface IConnection : IDiagramEntity {

		/// <summary>
		/// The 'from' or starting connector.
		/// </summary>
		IConnector From{
			get;
			set;
		}

		/// <summary>
		/// The 'to' or ending connector.
		/// </summary>
		IConnector To{
			get;
			set;
		}

        //void Attach(IConnector from, IConnector to);
        //void Detach();

	} 

}