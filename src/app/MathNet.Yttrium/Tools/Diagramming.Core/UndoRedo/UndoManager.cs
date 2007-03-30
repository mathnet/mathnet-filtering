
using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// UndoManager is a concrete class that maintains the undo list
    /// and redo stack data structures. It also provides methods that
    /// tell you whether there is something to undo or redo. The class
    /// is designed to be used directly in undo/redo menu item handlers,
    /// and undo/redo menu item state update functions.
    /// </summary>
    public class UndoManager : IUndoSupport
    {
        /// <summary>
        /// Occurs when the undo/redo history has changed. 
        /// </summary>
        public event EventHandler OnHistoryChange;

		#region Fields

        /// <summary>
        /// the max level to keep history
        /// </summary>
        private int       undoLevel;
        /// <summary>
        /// the undo list
        /// </summary>
        private UndoCollection undoList;

        
        /// <summary>
        /// the internal stack of redoable operations
        /// </summary>
        private StackBase<CommandInfo>  redoStack;

		#endregion

		#region Constructor
        /// <summary>
        /// Constructor which initializes the manager with up to 8 levels
        /// of undo/redo.
        /// </summary>
        /// <param name="level">the undo level</param>
        public UndoManager(int level)
        {
            undoLevel = level;
            undoList = new UndoCollection();
            redoStack = new StackBase<CommandInfo>();
            //the following events are linked in the sense that when an item is popped from the stack it'll be put in the undo collection 
            //again. So, strictly speaking, you need only two of the four events to make the surface aware of the history changes, but
            //we'll keep it as is. It depends on your own perception of the undo/reo process.
            redoStack.OnItemPopped += new EventHandler<CollectionEventArgs<CommandInfo>>(HistoryChanged);
            redoStack.OnItemPushed += new EventHandler<CollectionEventArgs<CommandInfo>>(HistoryChanged);
            undoList.OnItemAdded += new EventHandler<CollectionEventArgs<CommandInfo>>(HistoryChanged);
            undoList.OnItemRemoved += new EventHandler<CollectionEventArgs<CommandInfo>>(HistoryChanged);

        }

     

        private void HistoryChanged(object sender, CollectionEventArgs<CommandInfo> e)
        {
            RaiseHistoryChange();
        }

		#endregion

		#region Properties
        /// <summary>
        /// Property for the maximum undo level.
        /// </summary>
        public int MaxUndoLevel
        {
            get
            {
                return undoLevel;
            }
            set
            {
                Debug.Assert( value >= 0 );
				
                // To keep things simple, if you change the undo level,
                // we clear all outstanding undo/redo commands.
                if ( value != undoLevel )
                {
                    ClearUndoRedo();
                    undoLevel = value;
                }
            }
        }

        /// <summary>
        /// Gets the undo list.
        /// </summary>
        /// <value>The undo list.</value>
        internal UndoCollection UndoList
        {
            get { return undoList; }
        }
		#endregion

		#region Methods
        /// <summary>
        /// Raises the OnHistoryChange event
        /// </summary>
        private void RaiseHistoryChange()
        {
            if (OnHistoryChange != null)
                OnHistoryChange(this, EventArgs.Empty);
        }
        /// <summary>
        /// Register a new undo command. Use this method after your
        /// application has performed an operation/command that is
        /// undoable.
        /// </summary>
        /// <param name="cmd">New command to add to the manager.</param>
        public void AddUndoCommand(ICommand cmd)
        {
            Debug.Assert( cmd != null );
            Debug.Assert( undoList.Count <= undoLevel );

            if ( undoLevel == 0 )
                return;

            CommandInfo info = null;
            if ( undoList.Count == undoLevel )
            {
                // Remove the oldest entry from the undo list to make room.
                info = (CommandInfo) undoList[0];
                undoList.RemoveAt(0);
            }

            // Insert the new undoable command into the undo list.
            if ( info == null )
                info = new CommandInfo();
            info.Command = cmd;
            info.Handler = null;
            undoList.Add(info);

            // Clear the redo stack.
            ClearRedo();
        }

        /// <summary>
        /// Register a new undo command along with an undo handler. The
        /// undo handler is used to perform the actual undo or redo
        /// operation later when requested.
        /// </summary>
        /// <param name="cmd">New command to add to the manager.</param>
        /// <param name="undoHandler">Undo handler to perform the actual undo/redo operation.</param>
        public void AddUndoCommand(ICommand cmd, IUndoSupport undoHandler)
        {
            AddUndoCommand(cmd);

            if ( undoList.Count > 0 )
            {
                CommandInfo info = (CommandInfo) undoList[undoList.Count-1];
                Debug.Assert( info != null );
                info.Handler = undoHandler;
            }
        }

        /// <summary>
        /// Clear the internal undo/redo data structures. Use this method
        /// when your application performs an operation that cannot be undone.
        /// For example, when the user "saves" or "commits" all the changes in
        /// the application.
        /// </summary>
        public void ClearUndoRedo()
        {
            ClearUndo();
            ClearRedo();
        }

        /// <summary>
        /// Check if there is something to undo. Use this method to decide
        /// whether your application's "Undo" menu item should be enabled
        /// or disabled.
        /// </summary>
        /// <returns>Returns true if there is something to undo, false otherwise.</returns>
        public bool CanUndo()
        {
            return undoList.Count > 0;
        }

        /// <summary>
        /// Check if there is something to redo. Use this method to decide
        /// whether your application's "Redo" menu item should be enabled
        /// or disabled.
        /// </summary>
        /// <returns>Returns true if there is something to redo, false otherwise.</returns>
        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        /// <summary>
        /// Perform the undo operation. If an undo handler is specified, it
        /// will be used to perform the actual operation. Otherwise, the command
        /// instance is asked to perform the undo.
        /// </summary>
        public void Undo()
        {
            if ( !CanUndo() )
                return;
    
            // Remove newest entry from the undo list.
            CommandInfo info = (CommandInfo) undoList[undoList.Count-1];
            undoList.RemoveAt(undoList.Count-1);
            
            // Perform the undo.
            Debug.Assert( info.Command != null );
           
                info.Command.Undo();

            // Now the command is available for redo. Push it onto
            // the redo stack.
            redoStack.Push(info);
        }

        /// <summary>
        /// Perform the redo operation. If an undo handler is specified, it
        /// will be used to perform the actual operation. Otherwise, the command
        /// instance is asked to perform the redo.
        /// </summary>
        public void Redo()
        {
            if ( !CanRedo() )
                return;
    
            // Remove newest entry from the redo stack.
            CommandInfo info = (CommandInfo) redoStack.Pop();
            
            // Perform the redo.
            Debug.Assert( info.Command != null );
           
                info.Command.Redo();

            // Now the command is available for undo again. Put it back
            // into the undo list.
            undoList.Add(info);
        }

        /// <summary>
        /// Get the text value of the next undo command. Use this method
        /// to update the Text property of your "Undo" menu item if
        /// desired. For example, the text value for a command might be
        /// "Draw Circle". This allows you to change your menu item Text
        /// property to "Undo Draw Circle".
        /// </summary>
        /// <returns>Text value of the next undo command.</returns>
        public string UndoText
        {
            get
            {
                ICommand cmd = NextUndoCommand;
                if (cmd == null)
                    return "";
                return cmd.Text;
            }
        }

        /// <summary>
        /// <para>
        /// Get the text value of the next redo command. Use this method
        /// to update the Text property of your "Redo" menu item if desired.
        /// For example, the text value for a command might be "Draw Line".
        /// This allows you to change your menu item text to "Redo Draw Line".
        /// </para>
        /// </summary>
        /// <value>The redo text.</value>
        /// <returns>Text value of the next redo command.</returns>
        public string RedoText
        {
            get
            {
                ICommand cmd = NextRedoCommand;
                if (cmd == null)
                    return "";
                return cmd.Text;
            }
        }

        /// <summary>
        /// Get the next (or newest) undo command. This is like a "Peek"
        /// method. It does not remove the command from the undo list.
        /// </summary>
        /// <returns>The next undo command.</returns>
        public ICommand NextUndoCommand
        {
            get
            {
                if(undoList.Count == 0)
                    return null;
                CommandInfo info = (CommandInfo) undoList[undoList.Count - 1];
                return info.Command;
            }
        }

        /// <summary>
        /// Get the next redo command. This is like a "Peek"
        /// method. It does not remove the command from the redo stack.
        /// </summary>
        /// <returns>The next redo command.</returns>
        public ICommand NextRedoCommand
        {
            get
            {
                if(redoStack.Count == 0)
                    return null;
                CommandInfo info = (CommandInfo) redoStack.Peek();
                return info.Command;
            }

        }

        /// <summary>
        /// Retrieve all of the undo commands. Useful for debugging,
        /// to analyze the contents of the undo list.
        /// </summary>
        /// <returns>Array of commands for undo.</returns>
        public ICommand[] GetUndoCommands()
        {
            if ( undoList.Count == 0 )
                return null;

            ICommand[] cmdList = new ICommand[undoList.Count];
            object[] objList = undoList.ToArray();
            for (int i = 0; i < objList.Length; i++)
            {
                CommandInfo info = (CommandInfo) objList[i];
                cmdList[i] = info.Command;
            }

            return cmdList;
        }

        /// <summary>
        /// Retrieve all of the redo commands. Useful for debugging,
        /// to analyze the contents of the redo stack.
        /// </summary>
        /// <returns>Array of commands for redo.</returns>
        public ICommand[] GetRedoCommands()
        {
            if ( redoStack.Count == 0 )
                return null;

            ICommand[] cmdList = new ICommand[redoStack.Count];
            object[] objList = redoStack.ToArray();
            for (int i = 0; i < objList.Length; i++)
            {
                CommandInfo info = (CommandInfo) objList[i];
                cmdList[i] = info.Command;
            }

            return cmdList;
        }

        /// <summary>
        /// Clear the contents of the undo list.
        /// </summary>
        private void ClearUndo()
        {
            while( undoList.Count > 0 )
            {
                CommandInfo info = (CommandInfo) undoList[undoList.Count-1];
                undoList.RemoveAt(undoList.Count-1);
                info.Command = null;
                info.Handler = null;
            }
        }

        /// <summary>
        /// Clear the contents of the redo stack.
        /// </summary>
        private void ClearRedo()
        {
            while( redoStack.Count > 0 )
            {
                CommandInfo info = (CommandInfo) redoStack.Pop();
                info.Command = null;
                info.Handler = null;
            }
        }
		#endregion
    }

    class UndoCollection : CollectionBase<CommandInfo>
    {
        
    }
}


