using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool implement the action of moving shape connectors on the canvas.     
    /// </summary>
    class ConnectorMoverTool : AbstractTool, IMouseListener
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
        /// <summary>
        /// the connector being moved
        /// </summary>
        private IConnector fetchedConnector;
        /// <summary>
        /// whether we are currently moving something
        /// </summary>
        private bool motionStarted;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectorMoverTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public ConnectorMoverTool(string name) : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            Controller.View.CurrentCursor = CursorPallet.Select;
            motionStarted = false;
            fetchedConnector = null;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
            {
                fetchedConnector = Selection.FindShapeConnector(e.Location);
                if(fetchedConnector != null)
                {
                    
                        initialPoint = e.Location;
                        lastPoint = initialPoint;
                        motionStarted = true;
                        return true;                    
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
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            Point point = e.Location;
            if(IsActive && motionStarted)
            {

                fetchedConnector.Move(new Point(point.X - lastPoint.X, point.Y - lastPoint.Y));
                
                lastPoint = point;
            }            
        }
        /// <summary>
        /// Handles the mouse up event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {
                DeactivateTool();
                if(fetchedConnector == null)
                    return;
                Bundle bundle = new Bundle(Controller.Model);
                bundle.Entities.Add(fetchedConnector);
                MoveCommand cmd = new MoveCommand(this.Controller, bundle, new Point(lastPoint.X - initialPoint.X, lastPoint.Y - initialPoint.Y));
                Controller.UndoManager.AddUndoCommand(cmd);
                //not necessary to perform the Redo action of the command since the mouse-move already moved the bundle!
            }
        }
        #endregion
    }

}
