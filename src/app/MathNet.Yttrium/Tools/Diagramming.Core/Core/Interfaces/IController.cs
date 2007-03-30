using System;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Interface of a controller
    /// </summary>
    public interface IController : IUndoSupport
    {
        #region Events
        event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        /// <summary>
        /// Occurs when the undo/redo history has changed
        /// </summary>
        event EventHandler<HistoryChangeEventArgs> OnHistoryChange;
        /// <summary>
        /// Occurs when a tool is activated
        /// </summary>
        event EventHandler<ToolEventArgs> OnToolActivate;
        /// <summary>
        /// Occurs when a tool is deactivated
        /// </summary>
        event EventHandler<ToolEventArgs> OnToolDeactivate;
        /// <summary>
        /// Occurs when the something got selected and the properties of it can/should be shown.
        /// </summary>
        event EventHandler<SelectionEventArgs> OnShowSelectionProperties;
                /// <summary>
        /// Occurs when an entity is added.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the controller receives a mouse-down notification of the surface. This event is raised before the
        /// event is broadcasted down to the tools.
        /// </summary>
        event EventHandler<MouseEventArgs> OnMouseDown;
        /// <summary>
        /// Occurs when the diagram control is aksed to show the context menu
        /// </summary>
        event EventHandler<EntityMenuEventArgs> OnShowContextMenu;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the model
        /// </summary>
        IModel Model { get; set;}

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        IView View { get;set;}

        /// <summary>
        /// Gets the tools.
        /// </summary>
        /// <value>The tools.</value>
        CollectionBase<ITool> Tools { get;}

        /// <summary>
        /// Gets the undo manager.
        /// </summary>
        /// <value>The undo manager.</value>
        UndoManager UndoManager { get;}

        /// <summary>
        /// Gets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
        IDiagramControl ParentControl { get;}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IController"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled
        {
            get;
            set;
        }
        #endregion

        #region Methods

      

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <param name="toolName">Name of the tool.</param>
        void ActivateTool(string toolName);

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        void AddTool(ITool tool);

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns></returns>
        bool DeactivateTool(ITool tool);

        /// <summary>
        /// Raises the OnShowSelectionProperties event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.SelectionEventArgs"/> instance containing the event data.</param>
        void RaiseOnShowSelectionProperties(SelectionEventArgs e);
        /// <summary>
        /// Suspends all tools
        /// </summary>
        void SuspendAllTools();

        /// <summary>
        /// Unsuspends all tools.
        /// </summary>
        void UnsuspendAllTools();
        /// <summary>
        /// Raises the OnShowContextMenu event
        /// </summary>
        /// <param name="e"></param>
        void RaiseOnShowContextMenu(EntityMenuEventArgs e);

        /// <summary>
        /// Changes the paint style of the selected entities
        /// </summary>
        /// <param name="paintStyle"></param>
        void ChangeStyle(IPaintStyle paintStyle);

        void ChangeStyle(IPenStyle penStyle);

        void RunActivity(string name);
        #endregion

        
    }
}
