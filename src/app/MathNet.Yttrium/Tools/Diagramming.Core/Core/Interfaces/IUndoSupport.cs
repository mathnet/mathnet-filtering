using System;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Interface that your application classes can implement
	/// in order to perform the actual undo/redo functionality.	
	/// </summary>
	public interface IUndoSupport
	{
        /// <summary>
        /// Undo of the last action
        /// </summary>
        /// <remarks>Calling this on a class level will call the Undo method of the last ICommand in the stack.</remarks>
		void Undo();
        /// <summary>
        /// Performs the actual action or redo in case the actions was undoe before
        /// </summary>
        /// <remarks>Calling this on a class level will call the Redo method of the last ICommand in the stack.</remarks>
		void Redo();
	}
}
