using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This defines what an action is on the model.
	/// </summary>
	public interface IAction : IActivity
	{

		#region Events

		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        IModel Model {get; set;}
		#endregion

		#region Methods

		#endregion
	}
}
