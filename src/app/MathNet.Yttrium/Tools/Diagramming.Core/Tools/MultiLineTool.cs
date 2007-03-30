using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{

    class MultiLineTool : AbstractTool, IMouseListener, IKeyboardListener
    {

        #region Fields
        /// <summary>
        /// the location of the mouse when the motion starts
        /// </summary>

        private bool doDraw;
        private Point[] points;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MultiLineTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public MultiLineTool(string name)
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
            Controller.View.CurrentCursor = CursorPallet.Cross;
            this.SuspendOtherTools();
            doDraw = false;
            points = new Point[1];
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Returns 'true' if the event was handled, otherwise 'false'.</returns>
        public bool MouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
            {
                Point p = e.Location;
                if (Closure(p))
                {
                    points[points.Length - 1] = points[0];
                    doDraw = false;
                    DeactivateTool();
                    Package();

                    return true;
                }
                Array.Resize<Point>(ref points, points.Length + 1);
                points[points.Length - 2] = p;
                doDraw = true;
                return true;
            }
            return false;
        }
        private bool Closure(Point p)
        {
            for (int k = 0; k < points.Length - 1; k++)
            {
                if (new Rectangle(points[k].X - 6, points[k].Y - 6, 12, 12).Contains(p))
                {
                    DialogResult res = MessageBox.Show("Do you wish to close the curve?", "Closure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
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
            if (IsActive && doDraw)
            {
                points[points.Length - 1] = e.Location;
                Controller.View.PaintGhostLine(MultiPointType.Straight, points);
                //Controller.View.Invalidate(System.Drawing.Rectangle.Inflate(Controller.View.Ghost.Rectangle, 20, 20));
                //TODO: find a more performant way to invalidate the area
                Controller.View.Invalidate();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {

            }
        }
        private void Package()
        {
            MultiPointShape shape = new MultiPointShape(this.Controller.Model, points, MultiPointType.Straight);
            AddMultiPointShapeCommand cmd = new AddMultiPointShapeCommand(this.Controller, shape);
            this.Controller.UndoManager.AddUndoCommand(cmd);
            this.Controller.View.ResetGhost();
            cmd.Redo();
        }
        #endregion

        #region IKeyboardListener Members

        public void KeyDown(KeyEventArgs e)
        {


            if (e.KeyData == System.Windows.Forms.Keys.Escape && IsActive)
            {
                DeactivateTool();
                Package();
                e.Handled = true;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
        }

        public void KeyPress(KeyPressEventArgs e)
        {
        }

        #endregion
    }

}
