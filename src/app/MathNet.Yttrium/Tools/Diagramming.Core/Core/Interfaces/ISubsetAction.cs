using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This defines what an action is on the model.
	/// </summary>
	public interface ISubsetAction : IAction
	{

		#region Events

		#endregion

		#region Properties
        /// <summary>
        /// Gets the subset.
        /// </summary>
        /// <value>The subset.</value>
        CollectionBase<IDiagramEntity> Subset {get;}
		#endregion

		#region Methods

		#endregion
	}
}
