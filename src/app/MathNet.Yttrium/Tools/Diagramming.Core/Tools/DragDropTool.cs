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
    class DragDropTool : AbstractTool, IMouseListener, IDragDropListener
    {

        #region Fields
        Cursor feedbackCursor;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DragDropTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public DragDropTool(string name) : base(name)
        { 
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            Controller.View.CurrentCursor = Cursors.SizeAll;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            return false; //continue spreading the events
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
           
        }
        /// <summary>
        /// Handles the mouse up event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            
        }
        #endregion

        /// <summary>
        /// On dragdrop.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        public void OnDragDrop(DragEventArgs e)
        {
            Control control = (Control)this.Controller.ParentControl;
            Point p = control.PointToClient(new Point(e.X, e.Y));
            IShape shape = null;
            IDataObject iDataObject = e.Data;

            if(iDataObject.GetDataPresent(typeof(string)))
            {
                foreach(string shapeType in Enum.GetNames(typeof(ShapeTypes)))
                {
                    if(shapeType.ToString().ToLower() == iDataObject.GetData(typeof(string)).ToString().ToLower())
                    {
                        shape = ShapeFactory.GetShape(shapeType);
                        break;
                    }
                }
            }
            
            if(iDataObject.GetDataPresent(typeof(Bitmap)))
            {
                
                return;

            }

            if(shape != null)
            {
                shape.Move(new Point(p.X, p.Y));


                //ComplexRectangle shape = new ComplexRectangle();
                //shape.Rectangle = new Rectangle(p.X, p.Y, 150, 70);
                //shape.Text = "Just an example, work in progress.";

                //TextLabel shape = new TextLabel();
                //shape.Rectangle = new Rectangle(p.X, p.Y, 150, 70);
                //shape.Text = "Just an example, work in progress.";

                AddShapeCommand cmd = new AddShapeCommand(this.Controller, shape, p);
                this.Controller.UndoManager.AddUndoCommand(cmd);
                cmd.Redo();
                feedbackCursor = null;
            }
        }

        /// <summary>
        /// On drag leave.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        public void OnDragLeave(EventArgs e)
        {
            
        }

        /// <summary>
        /// On drag over.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyPressEventArgs"/> instance containing the event data.</param>
        public void OnDragOver(DragEventArgs e)
        {   
        }


        /// <summary>
        /// On drag enter
        /// </summary>
        /// <param name="e"></param>
        public void OnDragEnter(DragEventArgs e)
        {
            AnalyzeData(e);
        }

        private void AnalyzeData(DragEventArgs e)
        {
            IDataObject iDataObject = e.Data;
            if(iDataObject.GetDataPresent(typeof(string)))
            {
                foreach(string shapeType in Enum.GetNames(typeof(ShapeTypes)))
                {
                    if(shapeType.ToString().ToLower() == iDataObject.GetData(typeof(string)).ToString().ToLower())
                    {
                        feedbackCursor = CursorPallet.DropShape;
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }

                feedbackCursor = CursorPallet.DropText;
                e.Effect = DragDropEffects.Copy;
                return;


            }
            if(iDataObject.GetDataPresent(typeof(Bitmap)))
            {
                feedbackCursor = CursorPallet.DropImage;
                e.Effect = DragDropEffects.Copy;
                return;

            }
        }


        public void GiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = feedbackCursor;
        }
    }

}
