using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool implement the action of moving shapes on the canvas. 
    /// <para>Note that this tool is slightly different than other tools since it activates itself unless it has been suspended by another tool. </para>
    /// </summary>
    class MoveTool : AbstractTool, IMouseListener
    {

        #region Fields
        /// <summary>
        /// the location of the mouse when the motion starts
        /// </summary>
        private Point initialPoint;
        /// <summary>
        /// the intermediate location of the mouse during the motion
        /// </summary>
        private Point lastPoint;

        private IConnector hoveredConnector;
        /// <summary>
        /// the AsConnectorMover field
        /// </summary>
        private bool connectorMove;
        /// <summary>
        /// Gets or sets the AsConnectorMover
        /// </summary>
        public bool AsConnectorMover
        {
            get
            {
                return connectorMove;
            }
            set
            {
                connectorMove = value;
            }
        }

        public static bool Blocked = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoveTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public MoveTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            Controller.View.CurrentCursor = CursorPallet.Move;

        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            if(e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if(e.Button == MouseButtons.Left && Enabled && !IsSuspended && !Blocked)
            {
                if(Selection.SelectedItems.Count > 0)
                {
                    initialPoint = e.Location;
                    lastPoint = initialPoint;
                    connectorMove = false;
                    //while moving the shapes we'll clear the tracker
                    this.Controller.View.ResetTracker();
                    //now, go for it
                    this.ActivateTool();
                    return true;
                }
                else if(Selection.Connector != null && Selection.Connector.Parent is IConnection)
                {
                    if(!typeof(IShape).IsInstanceOfType(Selection.Connector.Parent)) //note that there is a separate tool to move shape-connectors!
                    {
                        initialPoint = e.Location;
                        lastPoint = initialPoint;
                        connectorMove = true; //keep this for the final packaging into the undo manager
                        this.ActivateTool();
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
            if(e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            Point point = e.Location;
            if(IsActive)
            {
                //can be a connector
                if(Selection.Connector != null)
                {
                    Selection.Connector.Move(new Point(point.X - lastPoint.X, point.Y - lastPoint.Y));
                    #region Do we hit something meaningful?

                    if (hoveredConnector != null)
                        hoveredConnector.Hovered = false;                        
                    
                    hoveredConnector = Selection.FindConnectorAt(e.Location);
                    if(hoveredConnector!=null)
                        hoveredConnector.Hovered = true;
                    if(hoveredConnector != null && hoveredConnector!=Selection.Connector)
                    {
                        Controller.View.CurrentCursor = CursorPallet.Grip;
                        
                    }
                    else
                        Controller.View.CurrentCursor = CursorPallet.Move;
                    #endregion
                }
                else //can be a selection
                {
                    foreach(IDiagramEntity entity in Selection.SelectedItems)
                    {

                        entity.Move(new Point(point.X - lastPoint.X, point.Y - lastPoint.Y));
                    }
                }
                lastPoint = point;
            }
        }
        /// <summary>
        /// When an entity is moved there are various possibilities:
        /// <list type="bullet">
        /// <item>
        /// <term>a connector is moved</term>
        /// <description>in this case the moved connector can only be part of a onnection because moving shape-connectors is not allowed unless by means of the <see cref="ConnectorMoverTool"/>.
        /// If a connector attached to a connection is moved we have the following fork:
        /// <list type="bullet">
        /// <item>the connector was attached</item>
        /// <description>the connector has a parent and needs to be detached before being moved and eventually attached to another connector</description>
        /// <item>the connector is moved</item>
        /// <description>this is a standard motion and is similar for any <see cref="IDiagramEntity"/>
        /// </description>
        /// <item>the connector ends up somewhere near another connector and will become attached to it</item>
        /// <description>the connector in the proximity of the moved connector will become the parent of it. Note that we previously detached any binding and that a connector can have only one parent.</description>
        /// </list>
        /// </description>
        /// </item>
        /// <item>an entity is moved</item>
        /// <description>the normal <see cref="MoveCommand"/> can be used</description>
        /// <item>a set of entities is moved</item>
        /// <description>we need to create a bundle to package the entities and then use the <see cref="MoveCommand"/></description>
        /// </list>
        /// Several important remarks are in order here:
        /// <list type="bullet">
        /// <item>a connector can have only one parent</item>
        /// <item>we need to package a move action in a command but this command needs NOT to be performed (i.e. call the Redo() method) because the motion already occured through the MouseMove handler. Because of this situation we cannot perform a Redo() on the full package since it would move the entities twice. Hence, commands are execute just after their creation (except for the move).
        /// <item>when the stack of actions are undone the stack has to be reverted</item>
        /// </item>
        /// <item>whatever the situation is, the most economical way to code the different cases is by means of a <see cref="CompoundCommand"/> object</item>
        /// </list>
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            if(IsActive)
            {
                DeactivateTool();
                //creation of the total undoredo package
                CompoundCommand package = new CompoundCommand(this.Controller);
                string message = string.Empty;
                //notice that the connector can only be a connection connector because of the MouseDown check above
                if(connectorMove)
                {
                    #region We are moving a connection-connector
                    
                    #region Is the connector attached?
                    //detach only if there is a parent different than a connection; the join of a chained connection is allowed to move
                    if( Selection.Connector.AttachedTo != null && !typeof(IConnection).IsInstanceOfType(Selection.Connector.AttachedTo))
                    {
                        DetachConnectorCommand detach = new DetachConnectorCommand(this.Controller, Selection.Connector.AttachedTo, Selection.Connector);
                        detach.Redo();
                        package.Commands.Add(detach);

                    }
                    #endregion 
                    
                    #region The moving part
                    //a bundle might look like overkill here but it makes the coding model uniform
                    Bundle bundle = new Bundle(Controller.Model);
                    bundle.Entities.Add(Selection.Connector);
                    MoveCommand move = new MoveCommand(this.Controller, bundle, new Point(lastPoint.X - initialPoint.X, lastPoint.Y - initialPoint.Y));
                    //no Redo() necessary here!
                    package.Commands.Add(move);
                    #endregion 

                    #region The re-attachment near another connector
                    //let's see if the connection endpoints hit other connectors (different than the selected one!)   
                    //define a predicate delegate to filter things out otherwise the hit will return the moved connector which would results
                    //in a stack overflow later on
                    Predicate<IConnector> predicate =
                        delegate(IConnector conn)
                        {
                            //whatever, except itself and any children of the moved connector
                            //since this would entail a child becoming a parent!
                            if(conn.Hit(e.Location) && conn != Selection.Connector && !Selection.Connector.AttachedConnectors.Contains(conn)) 
                                return true;
                            return false;
                        };
                    //find it!
                    IConnector parentConnector = Selection.FindConnector(predicate);

                    if(parentConnector != null) //aha, there's an attachment
                    {                        
                        BindConnectorsCommand binder = new BindConnectorsCommand(this.Controller, parentConnector, Selection.Connector);
                        package.Commands.Add(binder);
                        binder.Redo(); //this one is necessary since the redo cannot be performed on the whole compound command
                    }
                    #endregion 
			
                    message = "Connector move";
                    #endregion
                }
                else
                {
                    #region We are moving entities other than a connector
                    Bundle bundle = new Bundle(Controller.Model);
                    bundle.Entities.AddRange(Selection.SelectedItems);
                    MoveCommand cmd = new MoveCommand(this.Controller, bundle, new Point(lastPoint.X - initialPoint.X, lastPoint.Y - initialPoint.Y));
                    package.Commands.Add(cmd);
                    //not necessary to perform the Redo action of the command since the mouse-move already moved the bundle!
                    #endregion

                    message = "Entities move";
                }
                //reset the hovered connector, if any
                if (hoveredConnector != null)
                    hoveredConnector.Hovered = false; 
                package.Text = message;
                //whatever the content of the package we add it to the undo history                
                this.Controller.UndoManager.AddUndoCommand(package);

                //show the tracker again
                this.Controller.View.ShowTracker();

            }
        }
        #endregion
    }

}
