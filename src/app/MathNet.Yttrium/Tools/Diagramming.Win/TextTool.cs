using System;
using System.Drawing;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
    /// <summary>
    /// Tool to draw an ellipse on the canvas
    /// <seealso cref="RectangleTool"/>
    /// </summary>
    class TextTool : AbstractDrawingTool
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextTool"/> class.
        /// </summary>
        public TextTool() : base("Text Tool")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextTool"/> class.
        /// </summary>
        /// <param name="name"></param>
        public TextTool(string name): base(name)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the mouse move event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsActive && started)
            {
                Point point = new Point(e.X, e.Y);
                Controller.View.PaintGhostRectangle(startingPoint, point);
                Controller.View.Invalidate(System.Drawing.Rectangle.Inflate(Controller.View.Ghost.Rectangle, 20, 20));
            }
        }


        /// <summary>
        /// This method will be called when the user has finished drawing a ghost rectangle or bundle
        /// and initiates the actual creation of a bundle and the addition to the model via the appropriate command.
        /// </summary>
        protected override void GhostDrawingComplete()
        {

            try
            {
                TextOnly shape = new TextOnly(this.Controller.Model);
                shape.Width = (int) Rectangle.Width;
                shape.Height = (int) Rectangle.Height;
                shape.Text = "New label";
                
                AddShapeCommand cmd = new AddShapeCommand(this.Controller, shape, new Point((int) Rectangle.X, (int)Rectangle.Y));
                this.Controller.UndoManager.AddUndoCommand(cmd);
                cmd.Redo();
                TextEditor.GetEditor(shape);
                TextEditor.Show();
            }
            catch
            {
                base.Controller.DeactivateTool(this);
                Controller.View.Invalidate();
                throw;
            }

            base.Controller.DeactivateTool(this);
        }


        #endregion
    }

}
