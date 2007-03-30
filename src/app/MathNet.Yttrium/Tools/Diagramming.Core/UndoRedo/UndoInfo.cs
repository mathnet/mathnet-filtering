using System;

namespace Netron.Diagramming.Core
{
	/// <summary>
	/// CommandInfo is a class that is used as the
	/// data type in the undo list and redo stack. It just stores
	/// a reference to a command and an undo handler.
	/// </summary>
	public  class CommandInfo
	{
		private ICommand  mCommand;
		private IUndoSupport mHandler;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
		public ICommand Command
		{
			get{return mCommand;}
			set{mCommand = value;}
		}

        /// <summary>
        /// The handler or parent of the undo command
        /// </summary>
		public IUndoSupport Handler
		{
			get{return mHandler;}
			set{mHandler = value;}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CommandInfo"/> class.
        /// </summary>
		public CommandInfo()
		{  			
		}
	}
}
