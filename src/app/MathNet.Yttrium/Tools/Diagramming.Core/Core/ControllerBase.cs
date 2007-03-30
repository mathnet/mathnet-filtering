using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base implementation the <see cref="IController"/> interface.
    /// </summary>
    public abstract class ControllerBase : IUndoSupport, IController
    {
        #region Events
        /// <summary>
        /// Occurs when the context menu is shown.
        /// </summary>
        public event EventHandler<EntityMenuEventArgs> OnShowContextMenu;
        /// <summary>
        /// Occurs when a tool is asked to be deactivated
        /// </summary>
        public event EventHandler<ToolEventArgs> OnToolDeactivate;
        /// <summary>
        /// Occurs when a tool is asked to be activated
        /// </summary>
        public event EventHandler<ToolEventArgs> OnToolActivate;
        /// <summary>
        /// Occurs when the history has changed in the undo/redo mechanism
        /// </summary>
        public event EventHandler<HistoryChangeEventArgs> OnHistoryChange;
        /// <summary>
        /// Occurs when the something got selected and the properties of it can/should be shown.
        /// </summary>
        public event EventHandler<SelectionEventArgs> OnShowSelectionProperties;
        /// <summary>
        /// Occurs when an entity is added.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityAdded;
        /// <summary>
        /// Occurs when an entity is removed.
        /// <remarks>This event usually is bubbled from one of the layers</remarks>
        /// </summary>
        public event EventHandler<EntityEventArgs> OnEntityRemoved;
        /// <summary>
        /// Occurs when the controller receives a mouse-down notification of the surface. This event is raised before the
        /// event is broadcasted down to the tools.
        /// </summary>
        public event EventHandler<MouseEventArgs> OnMouseDown;
        /// <summary>
        /// Occurs when the Ambience has changed
        /// </summary>
        public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        
        
        #endregion

        #region Fields

        private bool eventsEnabled = true;
        private bool controllerEnabled = true;

        private IModel mModel;
        private UndoManager mUndoManager;
        /// <summary>
        /// the View field
        /// </summary>
        private IView mView;
        private CollectionBase<IMouseListener> mouseListeners;
        private CollectionBase<IKeyboardListener> keyboardListeners;
        private CollectionBase<IDragDropListener> dragdropListeners;
        private IDiagramControl parentControl;
        private CollectionBase<ITool> registeredTools;
        private CollectionBase<IActivity> registeredActivity;
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:ControllerBase"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get
            {
                return controllerEnabled;
            }
            set
            {
                controllerEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
         public IDiagramControl ParentControl
        {
            get { return parentControl; }
            internal set { parentControl = value; }
        }
        /// <summary>
        /// Gets the registered tools.
        /// </summary>
        /// <value>The tools.</value>
        public CollectionBase<ITool> Tools
        {
            get { return registeredTools; }
        }



        /// <summary>
        /// Gets the undo manager.
        /// </summary>
        /// <value>The undo manager.</value>
        public  UndoManager UndoManager
        {
            get
            {
                return mUndoManager;
            }

        }

        /// <summary>
        /// Gets or sets the model
        /// </summary>
        /// <value></value>
        public IModel Model
        {
            get
            {
                return mModel;
            }
            set
            {
                AttachToModel(value);
            }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public IView View
        {
            get
            {
                return mView;
            }
            set
            {
                AttachToView(value);
            }
        }

        /// <summary>
        /// Attaches to the given view.
        /// </summary>
        /// <param name="view">The view.</param>
        private void AttachToView(IView view)
        {
            if (view == null)
                throw new ArgumentNullException();

            mView = view;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        protected ControllerBase(IDiagramControl surface)
        {
            //doesn't work if you supply a null reference
            if(surface==null)
                throw new NullReferenceException("The diagram control assigned to the controller cannot be 'null'");

            
            //create the undo/redo manager
            mUndoManager = new UndoManager(15);
            mUndoManager.OnHistoryChange += new EventHandler(mUndoManager_OnHistoryChange);

            #region Instantiation of listeners
            mouseListeners = new CollectionBase<IMouseListener>();
            keyboardListeners = new CollectionBase<IKeyboardListener>();
            dragdropListeners = new CollectionBase<IDragDropListener>();
            #endregion
            //keep a reference to the parent control
            parentControl = surface;

            AttachToSurface(parentControl);
            
           

            //Make sure the static selection class knows about the model
            Selection.Controller = this;
            //Initialize the colorscheme
            ArtPallet.Init();

            #region Tools: the registration order matters!
            /*
             The order in in which the tools are added matters, at least some of them.
             * The TransformTool should come before the HitToom and the MoveTool after the HitTool.
             * The order of the drawing tools does not matter.
             * It's also important to remark that the tools do not depend on the Model.
             */

            registeredTools = new CollectionBase<ITool>();

            this.AddTool(new TransformTool("Transform Tool"));

            this.AddTool(new HitTool("Hit Tool"));

            this.AddTool(new MoveTool("Move Tool"));

            this.AddTool(new RectangleTool("Rectangle Tool"));

            this.AddTool(new ComplexRectangleTool("ComplexRectangle Tool"));

            this.AddTool(new EllipseTool("Ellipse Tool"));

            this.AddTool(new SelectionTool("Selection Tool"));

            this.AddTool(new DragDropTool("DragDrop Tool"));

            this.AddTool(new ConnectionTool("Connection Tool"));

            this.AddTool(new ConnectorMoverTool("Connector Mover Tool"));

            this.AddTool( new GroupTool("Group Tool"));

            this.AddTool(new UngroupTool("Ungroup Tool"));

            this.AddTool(new SendToBackTool("SendToBack Tool"));

            this.AddTool(new SendBackwardsTool("SendBackwards Tool"));

            this.AddTool(new SendForwardsTool("SendForwards Tool"));

            this.AddTool(new SendToFrontTool("SendToFront Tool"));

          

            this.AddTool(new HoverTool("Hover Tool"));

            this.AddTool(new ContextTool("Context Tool"));

            this.AddTool(new CopyTool("Copy Tool"));

            this.AddTool(new PasteTool("Paste Tool"));

            this.AddTool(new ScribbleTool("Scribble Tool"));

            this.AddTool(new PolygonTool("Polygon Tool"));

            this.AddTool(new MultiLineTool("MultiLine Tool"));

            #endregion

            #region Hotkeys
            HotKeys keys = new HotKeys(this);
            this.keyboardListeners.Add(keys);
            #endregion

            #region Activities
            //this is in a way a waste of memory; the layouts should not necessarily be loaded
            //before they are actually requested. You could register only the (string) names instead.
            //But for just a few algorithms this is OK and the advantage of this registration is that
            //one can register actions from outside the library, in the hosting form for example.
            registeredActivity = new CollectionBase<IActivity>();
            AddActivity(new RandomLayout(this));
            AddActivity(new FruchtermanReingoldLayout(this));
            AddActivity(new StandardTreeLayout(this));
            AddActivity(new RadialTreeLayout(this));
            AddActivity(new BalloonTreeLayout(this));
            AddActivity(new ForceDirectedLayout(this));
            #endregion
        }

        /// <summary>
        /// Attaches the given model to the controller.
        /// </summary>
        /// <param name="model"></param>
        private void AttachToModel(IModel model)
        {
            if (model == null)
                throw new ArgumentNullException();

            mModel = model;
            mModel.OnEntityAdded += new EventHandler<EntityEventArgs>(mModel_OnEntityAdded);
            mModel.OnEntityRemoved += new EventHandler<EntityEventArgs>(mModel_OnEntityRemoved);
            mModel.OnAmbienceChanged += new EventHandler<AmbienceEventArgs>(mModel_OnAmbienceChanged);
        }

        void mModel_OnAmbienceChanged(object sender, AmbienceEventArgs e)
        {
            RaiseOnAmbienceChanged(e);
        }

        void mModel_OnEntityRemoved(object sender, EntityEventArgs e)
        {
            RaiseOnEntityRemoved(e);
        }

        void mModel_OnEntityAdded(object sender, EntityEventArgs e)
        {
            RaiseOnEntityAdded(e);
        }

        /// <summary>
        /// Bubbles the OnHistoryChange event
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void mUndoManager_OnHistoryChange(object sender, EventArgs e)
        {
            RaiseOnHistoryChange();
        }
        /// <summary>
        /// Raises the OnShowContextMenu event
        /// </summary>
        /// <param name="e"></param>
        public void RaiseOnShowContextMenu(EntityMenuEventArgs e)
        {
            EventHandler<EntityMenuEventArgs> handler = OnShowContextMenu;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the OnHistory change.
        /// </summary>
        private void RaiseOnHistoryChange()
        {
            EventHandler<HistoryChangeEventArgs> handler = OnHistoryChange;
            if(handler!=null)
            {                                      
                handler(this, new HistoryChangeEventArgs(this.UndoManager.RedoText, this.UndoManager.UndoText));
            }
        }
        #endregion
        
        #region Methods

        /// <summary>
        /// Changes the paint style of the selected entities
        /// </summary>
        /// <param name="paintStyle"></param>
        public void ChangeStyle(IPaintStyle paintStyle)
        {

            //note that you need a copy of the selected item otherwise the undo/redo will fail once the selection has changed
            FillStyleCommand cmd = new FillStyleCommand(this, Selection.SelectedItems.Copy(), paintStyle);            
            this.UndoManager.AddUndoCommand(cmd);
            cmd.Redo();
        }

        /// <summary>
        /// Changes the style.
        /// </summary>
        /// <param name="penStyle">The pen style.</param>
        public void ChangeStyle(IPenStyle penStyle)
        {
            PenStyleCommand cmd = new PenStyleCommand(this, Selection.SelectedItems.Copy(), penStyle);
            this.UndoManager.AddUndoCommand(cmd);
            cmd.Redo();
        }

        /// <summary>
        /// Raises the <see cref="OnToolDeactivate"/> event
        /// </summary>
        /// <param name="e">ConnectionCollection event argument</param>
        public  virtual void RaiseOnToolDeactivate(ToolEventArgs e)
        {
            EventHandler<ToolEventArgs> handler = OnToolDeactivate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the OnShowSelectionProperties event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.SelectionEventArgs"/> instance containing the event data.</param>
        public  virtual void RaiseOnShowSelectionProperties(SelectionEventArgs e)
        {
            EventHandler<SelectionEventArgs> handler = OnShowSelectionProperties;
            if(handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="OnToolActivate"/> event
        /// </summary>
        /// <param name="e">ConnectionCollection event argument</param>
        protected virtual void RaiseOnToolActivate(ToolEventArgs e)
        {
            EventHandler<ToolEventArgs> handler = OnToolActivate;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="OnMouseDown "/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void RaiseOnMouseDown(MouseEventArgs e)
        {
            if(OnMouseDown != null)
                OnMouseDown(this, e);
        }
        /// <summary>
        /// Raises the <see cref="OnEntityAdded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        protected virtual void RaiseOnEntityAdded(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="OnEntityRemoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityEventArgs"/> instance containing the event data.</param>
        protected virtual void RaiseOnEntityRemoved(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = OnEntityRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="OnAmbienceChanged"/> event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseOnAmbienceChanged(AmbienceEventArgs e)
        {
            EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        #region Tool (de)activation methods
        /// <summary>
        /// Deactivates the given tool
        /// </summary>
        /// <param name="tool">a registered ITool</param>
        /// <returns></returns>
        public bool DeactivateTool(ITool tool)
        {
            bool flag = false;
            if (tool != null && tool.Enabled && tool.IsActive)
            {
                //IEnumerator iEnumerator = tools.GetEnumerator();
                //Tool tool2 = null;
                //while (iEnumerator.MoveNext())
                //{
                //    tool2 = iEnumerator.Current is Tool;
                //    if (tool2 != null && tool2 != tool)
                //    {
                //        tool2.ToolDeactivating(tool);
                //    }
                //}
                flag = tool.DeactivateTool();
                if (flag && eventsEnabled)
                {
                    RaiseOnToolDeactivate(new ToolEventArgs(tool));
                }
            }
            return flag;
        }
        /// <summary>
        /// Activates the tool with the given name
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        public void ActivateTool(string toolName)
        {
            if(!controllerEnabled)
                return;

            //using anonymous method here
            Predicate<ITool> predicate = delegate(ITool tool)
            {
                if (tool.Name.ToLower() == toolName.ToLower())//not case sensitive
                    return true;
                else
                    return false;
            };
            ITool foundTool= this.registeredTools.Find(predicate);
            ActivateTool(foundTool);
        }

        /// <summary>
        /// Suspends all tools
        /// </summary>
        public void SuspendAllTools()
        {
            foreach(ITool tool in this.Tools)
            {
                tool.IsSuspended = true;
            }
        }
        /// <summary>
        /// Unsuspends all tools.
        /// </summary>
        public void UnsuspendAllTools()
        {
            foreach(ITool tool in this.Tools)
            {
                tool.IsSuspended = false;
                ;
            }
        }
        
        /// <summary>
        /// Activates a registered tool
        /// </summary>
        /// <param name="tool">a registered ITool</param>
        /// <returns></returns>
        private bool ActivateTool(ITool tool)
		{
            if(!controllerEnabled)
                return false;
			bool flag = false;
			if (tool != null && tool.CanActivate)
			{                  
				flag = tool.ActivateTool();
				if (flag && eventsEnabled )
				{
					RaiseOnToolActivate(new ToolEventArgs(tool));
				}
			}
			return flag;
		}
        #endregion



        /// <summary>
        /// Adds the given tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        public void AddTool(ITool tool)
        {
            tool.Controller = this;
            //add the tool to the collection even if it doesn't attach to anything (yet)
            registeredTools.Add(tool);

            IMouseListener mouseTool = null;
            if ((mouseTool = tool as IMouseListener) != null)
                mouseListeners.Add(mouseTool);                   

            IKeyboardListener keyboardTool = null;
            if ((keyboardTool = tool as IKeyboardListener) != null)
                keyboardListeners.Add(keyboardTool);

            IDragDropListener dragdropTool = null;
            if ((dragdropTool = tool as IDragDropListener) != null)
                dragdropListeners.Add(dragdropTool);
        }

        #region Activity
        /// <summary>
        /// Adds the given activity to the controller.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public void AddActivity(IActivity activity)
        {
           
            registeredActivity.Add(activity);
        }
        /// <summary>
        /// Runs the given activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        protected void RunActivity(IActivity activity)
        {
            if (activity == null) return;
            PrepareActivity(activity);
            
            activity.Run();
            
        }
        /// <summary>
        /// Runs the given activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        protected void RunActivity(IActivity activity, int milliseconds)
        {
            if (activity == null) return;
            PrepareActivity(activity);
            activity.Run(milliseconds);
        }
        /// <summary>
        /// Prepares the activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        private void PrepareActivity(IActivity activity)
        {
            if (activity is IAction)
                (activity as IAction).Model = this.Model;
            if (activity is ILayout)
            {
                (activity as ILayout).Bounds = parentControl.ClientRectangle;
                (activity as ILayout).Center = new PointF(parentControl.ClientRectangle.Width / 2, parentControl.ClientRectangle.Height / 2);
            }
        }

        /// <summary>
        /// Runs the activity.
        /// </summary>
        /// <param name="activityName">Name of the activity.</param>
        public void RunActivity(string activityName)
        {
            if (!controllerEnabled)
                return;

            this.View.CurrentCursor = Cursors.WaitCursor;
            controllerEnabled = false;
            
            IActivity foundActivity = FindActivity(activityName);
            if(foundActivity!=null)
                RunActivity(foundActivity);

            controllerEnabled = true;
            this.View.CurrentCursor = Cursors.Default;
        }
        /// <summary>
        /// Finds the activity witht the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected IActivity FindActivity(string name)
        {
            //using anonymous method here
            Predicate<IActivity> predicate = delegate(IActivity activity)
            {
                if (activity.Name.ToLower() == name.ToLower())//not case sensitive
                    return true;
                else
                    return false;
            };
            return this.registeredActivity.Find(predicate);

        }
        /// <summary>
        /// Runs the given activity for the specified time span.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public void RunActivity(string name, int milliseconds)
        {
            if (!controllerEnabled)
                return;

            IActivity foundActivity = FindActivity(name);
            if (foundActivity != null)
                RunActivity(foundActivity, milliseconds);
        }
        #endregion

        /// <summary>
        /// Attaches this controller to the surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        private void AttachToSurface(IDiagramControl surface)
        {

            #region Mouse events
            surface.MouseDown += new System.Windows.Forms.MouseEventHandler(surface_MouseDown);
            surface.MouseUp += new System.Windows.Forms.MouseEventHandler(surface_MouseUp);
            surface.MouseMove += new System.Windows.Forms.MouseEventHandler(surface_MouseMove);
            surface.MouseHover += new EventHandler(surface_MouseHover);
            surface.MouseWheel += new MouseEventHandler(surface_MouseWheel);
            #endregion

            #region Keyboard events
            surface.KeyDown += new System.Windows.Forms.KeyEventHandler(surface_KeyDown);
            surface.KeyUp += new System.Windows.Forms.KeyEventHandler(surface_KeyUp);
            surface.KeyPress += new System.Windows.Forms.KeyPressEventHandler(surface_KeyPress); 
            #endregion

            #region Dragdrop events
            surface.DragDrop += new DragEventHandler(surface_DragDrop);
            surface.DragEnter += new DragEventHandler(surface_DragEnter);
            surface.DragLeave += new EventHandler(surface_DragLeave);
            surface.DragOver += new DragEventHandler(surface_DragOver);
            surface.GiveFeedback += new GiveFeedbackEventHandler(surface_GiveFeedback);
            #endregion
        }

        /// <summary>
        /// Handles the MouseWheel event of the surface control.
        /// <remarks>In the WinForm implementation this routine is not called because it gives some flickering effects; the hotkeys are implemented in the overriden OnMouseWheel method instead.</remarks>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void surface_MouseWheel(object sender, MouseEventArgs e)
        {
            #region Zooming
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                SizeF s = View.Magnification;
                float alpha = e.Delta>0? 0.9F: 1.1F;
                View.Magnification = new SizeF(s.Width * alpha, s.Height * alpha);
                
                return;
            }
            #endregion
            
            Point p = View.Origin;
            int newValue = p.Y - Math.Sign(e.Delta) * 10;
            if (newValue <= 0) return;

            #region Pan horizontal
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                View.Origin = new Point(newValue, p.Y);
                return;
            }
            #endregion

            #region Pan vertical

            View.Origin = new Point(p.X, newValue);

            #endregion
              
        }

        

      
        #region DragDrop event handlers
        /// <summary>
        /// Handles the GiveFeedback event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.GiveFeedbackEventArgs"/> instance containing the event data.</param>
        void surface_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!controllerEnabled)
                return;
            foreach (IDragDropListener listener in dragdropListeners)
            {
                listener.GiveFeedback(e);
            }            
        }
        /// <summary>
        /// Handles the DragOver event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        void surface_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if(!controllerEnabled)
                return;
            foreach (IDragDropListener listener in dragdropListeners)
            {
                listener.OnDragOver(e);
            }            
        }

        /// <summary>
        /// Handles the DragLeave event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void surface_DragLeave(object sender, EventArgs e)
        {
            if(!controllerEnabled)
                return;
            foreach (IDragDropListener listener in dragdropListeners)
            {
                listener.OnDragLeave(e);
            }            
        }

        /// <summary>
        /// Handles the DragEnter event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        void surface_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if(!controllerEnabled)
                return;
            foreach (IDragDropListener listener in dragdropListeners)
            {
                listener.OnDragEnter(e);
            }            
        }

        /// <summary>
        /// Handles the DragDrop event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        void surface_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if(!controllerEnabled)
                return;
            foreach (IDragDropListener listener in dragdropListeners)
            {
                listener.OnDragDrop(e);
            }            
        }
        #endregion


        #region Keyboard event handlers
        void surface_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            
            foreach (IKeyboardListener listener in keyboardListeners)
            {
                listener.KeyPress(e);
            }
        }

        void surface_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            
            foreach (IKeyboardListener listener in keyboardListeners)
            {
                listener.KeyUp(e);
            }
        }

        void surface_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
             foreach (IKeyboardListener listener in keyboardListeners)
            {
                listener.KeyDown(e);
            }
        }
        
        #endregion

        #region Mouse event handlers
        /// <summary>
        /// Implements the observer pattern for the mouse hover event, communicating the event to all listeners implementing the necessary interface.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void surface_MouseHover(object sender, EventArgs e)
        {
            //if (eventsEnabled)
            //    RaiseOnMouseHover(e);
            //if (!controllerEnabled)
                return;  
        }
        /// <summary>
        /// Implements the observer pattern for the mouse down event, communicating the event to all listeners implementing the necessary interface.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void surface_MouseDown(object sender, MouseEventArgs e)
        {
            #region Coordinates logic
            // Get a point adjusted by the current scroll position and zoom factor            
            Point p = Point.Round(this.View.ViewToWorld(this.View.DeviceToView(e.Location)));
            MouseEventArgs ce = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);            
            #endregion

            if (eventsEnabled)
                RaiseOnMouseDown(ce);
            if(!controllerEnabled)
                return;            
            this.parentControl.Focus();

            //(parentControl as Win.DiagramControl).toolTip.Show("Yihaaa", parentControl as Win.DiagramControl, ce.Location);

            //this selection process will work independently of the tools because
            //some tools need the current selection or hit entity
            //On the other hand, when drawing a simple rectangle for example the selection
            //should be off, so there is an overhead.
            //Selection.CollectEntitiesAt(e.Location);

            //raise the event to give the host the opportunity to show the properties of the selected item(s)
            //Note that if the selection is empty the property grid will show 'nothing'.
            RaiseOnShowSelectionProperties(new SelectionEventArgs(Selection.SelectedItems.ToArray()));

            foreach(IMouseListener listener in mouseListeners)
            {
                if (listener.MouseDown(ce))
                    break;
            }
        }
        /// <summary>
        /// Handles the MouseMove event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void surface_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(!controllerEnabled)
                return;

            #region Coordinates logic
            // Get a point adjusted by the current scroll position and zoom factor
            //Point p = new Point(e.X - parentControl.AutoScrollPosition.X, e.Y - parentControl.AutoScrollPosition.Y);
            Point p = Point.Round(this.View.ViewToWorld(this.View.DeviceToView(e.Location)));
            MouseEventArgs ce = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);                     
            #endregion
            foreach (IMouseListener listener in mouseListeners)
            {
                listener.MouseMove(ce);
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the surface control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void surface_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(!controllerEnabled)
                return;
            #region Coordinates logic
            // Get a point adjusted by the current scroll position and zoom factor
            //Point p = new Point(e.X - parentControl.AutoScrollPosition.X, e.Y - parentControl.AutoScrollPosition.Y);
            Point p = Point.Round(this.View.ViewToWorld(this.View.DeviceToView(e.Location)));
            MouseEventArgs ce = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
            #endregion
            foreach (IMouseListener listener in mouseListeners)
            {
                listener.MouseUp(ce);
            }
        }

        
        #endregion



        /// <summary>
        /// Undo of the last action
        /// </summary>
        /// <remarks>Calling this on a class level will call the Undo method of the last ICommand in the stack.</remarks>
        public void Undo()
        {
            //reset the tracker or show the tracker after the undo operation since the undo does not take care of it
            this.View.ResetTracker();
            mUndoManager.Undo();
            this.View.ShowTracker();
        }

        /// <summary>
        /// Performs the actual action or redo in case the actions was undoe before
        /// </summary>
        /// <remarks>Calling this on a class level will call the Redo method of the last ICommand in the stack.</remarks>
        public void Redo()
        {
            mUndoManager.Redo();
            this.View.ShowTracker();
        }
        #endregion

        
    }
}
