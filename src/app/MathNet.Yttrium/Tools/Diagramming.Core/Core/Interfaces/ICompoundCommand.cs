using System;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// ICommand is an interface for undo/redo support that is
	/// implemented by the Command class.
	/// </summary>
	public interface ICompoundCommand : ICommand
	{
        /// <summary>
        /// Gets or sets the commands in this compound action.
        /// </summary>
        /// <value>The commands.</value>
        CollectionBase<ICommand> Commands
        {
            get;
            set;
        }


	}
}
