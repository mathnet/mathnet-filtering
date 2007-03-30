using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The base class for a drawing element
    /// </summary>
    public abstract class AbstractDrawingTool : AbstractTool, IMouseListener, IKeyboardListener
    {

        #region Fields
        /// <summary>
        /// the starting point of the rectangle being drawn
        /// </summary>
        protected Point startingPoint;
        /// <summary>
        /// says whether the startingPoint was set, otherwise the ghost will appear even before an initial point was set!
        /// </summary>
        protected bool started;

        /// <summary>
        /// the actual rectangle which serves as a basis for the drawing of ellipses, rectangles, etc.
        /// </summary>
        private RectangleF mRectangle;


        #endregion

        #region Properties
        protected RectangleF Rectangle
        {
            get { return mRectangle; }
            set { mRectangle = value; }
        }
        
        

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        protected AbstractDrawingTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        protected override void OnActivateTool()
        {
        
            Controller.View.CurrentCursor = CursorPallet.Add;
        
        }

        protected override void OnDeactivateTool()
        {
            
            base.OnDeactivateTool();
        }

        #region Explicit implementation of IKeyboardListener
        void IKeyboardListener.KeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        void IKeyboardListener.KeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        void IKeyboardListener.KeyPress(KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled) return;

            if (e.KeyData == System.Windows.Forms.Keys.Escape)
            {
                DeactivateTool();
                Controller.View.ResetGhost();
                e.Handled = true;
            }
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (e.Handled) return;
        }

        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.Handled) return;
        }
        #endregion

        #region Explicit implementation of IMouseListener
        bool IMouseListener.MouseDown(MouseEventArgs e)
        {
            return OnMouseDown(e);
        }

        void IMouseListener.MouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        void IMouseListener.MouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }


        protected virtual bool OnMouseDown(MouseEventArgs e)
        {
            if (IsActive && e.Button == MouseButtons.Left)
            {
                startingPoint = new Point(e.X, e.Y);
                started = true;
                return true;
            }
            return false;
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {

        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (IsActive )
            {
               
                base.RestoreCursor();
                Point point = new Point(e.X, e.Y);
                //mRectangle = new Rectangle(startingPoint.X, startingPoint.Y, point.X - startingPoint.X, point.Y - startingPoint.Y);
                //mRectangle = base.Controller.View.ViewToWorld(base.Controller.View.DeviceToView(rectangle));
                mRectangle = Controller.View.Ghost.Rectangle;
                GhostDrawingComplete();
                Controller.View.ResetGhost();
                started = false;
            }
        }
        #endregion

        /// <summary>
        /// This method will be called when the user has finished drawing a ghost rectangle or bundle
        /// and initiates the actual creation of a bundle and the addition to the model via the appropriate command.
        /// </summary>
        protected abstract void GhostDrawingComplete();

        #endregion    
    
      
    
       
    }

    }
