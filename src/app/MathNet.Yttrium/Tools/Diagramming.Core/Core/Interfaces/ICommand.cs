using System;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// ICommand is an interface for undo/redo support that is
	/// implemented by the <see cref="Command"/> class.
	/// </summary>
	public interface ICommand
	{
        /// <summary>
        /// Executes the action corresponding to an undo
        /// </summary>
		void Undo();
        /// <summary>
        /// Executes the actual action or equivalently the redo in case it had been undone
        /// </summary>
		void Redo();
        /// <summary>
        /// A description of the action
        /// </summary>
        /// <remarks>The text can be used in an undo list or undo stack description</remarks>
		string Text{get; set;}
	}
}
