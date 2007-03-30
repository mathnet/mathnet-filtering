using System;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Tool to draw an ellipse on the canvas
    /// <seealso cref="RectangleTool"/>
    /// </summary>
    class EllipseTool : AbstractDrawingTool
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public EllipseTool() : base("Ellipse Tool")
        {
        }
        public EllipseTool(string name): base(name)
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
                Controller.View.PaintGhostEllipse(startingPoint, point);
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
                SimpleEllipse shape = new SimpleEllipse(this.Controller.Model);
                shape.Width = (int) Rectangle.Width;
                shape.Height = (int) Rectangle.Height;
                AddShapeCommand cmd = new AddShapeCommand(this.Controller, shape, new Point((int) Rectangle.X, (int)Rectangle.Y));
                this.Controller.UndoManager.AddUndoCommand(cmd);
                cmd.Redo();
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
