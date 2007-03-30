using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
    /// <summary>
    /// The toolbox diagram control (aka surface control).
    /// </summary>
    [
    ToolboxBitmap(typeof(DiagramControl), "DiagramControl.bmp"),
    ToolboxItem(true),
    Description("Generic diagramming control for .Net"),
    Designer(typeof(Netron.Diagramming.Win.DiagramControlDesigner)),
    DefaultProperty("Name"),
    DefaultEvent("OnMouseDown")]
    public sealed class DiagramControl : DiagramControlBase
    {

        #region Constants
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;

        #endregion

        #region Events
       
        #endregion

        #region Fields
        /// <summary>
        /// the context menu of the control
        /// </summary>
        private ContextMenu menu;
        /// <summary>
        /// the tooltip control
        /// </summary>
        public ToolTip toolTip;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DiagramControl"/> class.
        /// </summary>
        public DiagramControl() : base()
        {
            

            #region double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            #endregion
            if(!DesignMode)
            {

                //init the MVC, see the Visio diagram for an overview of the instantiation proces
                Controller = new Controller(this);
                //Create the view. This is not done in the base class because the view and the controller depend on the medium (web/win...)
                View = new View(this);
                //The diagram document is the total serializable package and contains in particular the model (which will be instantiated in the following line).
                Document = new Document();
                AttachToDocument(Document);
                Controller.View = View;
                TextEditor.Init(this);
                
                //Selection.Controller = Controller;
                //menu
                menu = new ContextMenu();
                BuildMenu();

                View.OnCursorChange += new EventHandler<CursorEventArgs>(mView_OnCursorChange);
                View.OnBackColorChange += new EventHandler<ColorEventArgs>(View_OnBackColorChange);
                Controller.OnShowContextMenu += new EventHandler<EntityMenuEventArgs>(Controller_OnShowContextMenu);
                this.AllowDrop = true;
               
                
                this.toolTip = new ToolTip();
                toolTip.IsBalloon = true;
                toolTip.UseAnimation = true;
                toolTip.UseFading = true;
                toolTip.ToolTipIcon = ToolTipIcon.Info;
                toolTip.ToolTipTitle = "Info";
                toolTip.Active = false;
                toolTip.BackColor = Color.OrangeRed;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"></see> event.
        /// <list type="bullet">
        /// <item><term>Control-key</term><description>the control (CTRL) modifier will zoom/magnify the diagram.</description></item>
        /// <item><term>Shift-key</term><description>the shift (SHFT) modifier will pan/translate the diagram</description></item>
        /// </list>
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {  
            Point p = Origin;
            int newValue=0;

           
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {   
                #region Zooming
                SizeF s = Magnification;
                float alpha = e.Delta > 0 ? 0.9F : 1.1F;                
                Magnification = new SizeF(s.Width * alpha, s.Height * alpha);
                float w = (float)AutoScrollPosition.X / (float)AutoScrollMinSize.Width;
                float h = (float)AutoScrollPosition.Y / (float)AutoScrollMinSize.Height;
                //resize the scrollbars proportionally to keep the actual canvas constant
                s = new SizeF(AutoScrollMinSize.Width * alpha, AutoScrollMinSize.Height * alpha);
                AutoScrollMinSize = Size.Round(s);

                //Point v = e.Location;
                //Point newOrigin = Origin;
                //newOrigin.Offset(Convert.ToInt32((alpha - 1) * v.X), Convert.ToInt32((alpha - 1) * v.Y));
                //Origin = newOrigin;
              
                #endregion  
            }
            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                #region Pan horizontal
                newValue = p.X - Math.Sign(e.Delta) * 20;
                if (newValue > 0)
                    Origin = new Point(newValue, p.Y);
                else
                    Origin = new Point(0, Origin.Y);
                
                #endregion
            }
            else
            {
                #region Default vertical scroll
                newValue = Origin.Y - Math.Sign(e.Delta) * 20;
                if (newValue > 0)
                    Origin = new Point(Origin.X, newValue);
                else
                    Origin = new Point(Origin.X, 0);
                #endregion
            }

            this.AutoScrollPosition = Origin;
            
            //Origin = new Point(Origin.X, Origin.Y - 10);
            //this.AutoScrollPosition = Origin;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ScrollableControl.Scroll"></see> event.
        /// </summary>
        /// <param name="se">A <see cref="T:System.Windows.Forms.ScrollEventArgs"></see> that contains the event data.</param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            //base.OnScroll(se);
            if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                Origin = new Point(se.NewValue, Origin.Y);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
            else
            {
                Origin = new Point(Origin.X, se.NewValue);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
        }

        /// <summary>
        /// Handles the OnShowContextMenu event of the Controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityMenuEventArgs"/> instance containing the event data.</param>
        void Controller_OnShowContextMenu(object sender, EntityMenuEventArgs e)
        {

            menu.Show(this, e.MouseEventArgs.Location);
        }

         

        

       
        #endregion

        #region Methods

    



       
        void mView_OnCursorChange(object sender, CursorEventArgs e)
        {
           this.Cursor = e.Cursor;
        }
        /// <summary>
        /// Builds the context menu
        /// </summary>
        private void BuildMenu()
        {
            menu.MenuItems.Clear();

            MenuItem mnuDelete = new MenuItem("Delete", new EventHandler(OnDelete));
            menu.MenuItems.Add(mnuDelete);

            MenuItem mnuProps = new MenuItem("Properties", new EventHandler(OnProperties));
            menu.MenuItems.Add(mnuProps);

            MenuItem mnuDash = new MenuItem("-");
            menu.MenuItems.Add(mnuDash);

            if (EnableAddConnection)
            {
                MenuItem mnuNewConnection = new MenuItem("Add connection", new EventHandler(OnNewConnection));
                menu.MenuItems.Add(mnuNewConnection);
            }

            MenuItem mnuShapes = new MenuItem("Shapes");
            menu.MenuItems.Add(mnuShapes);

            MenuItem mnuRecShape = new MenuItem("Rectangular", new EventHandler(OnRecShape));
            mnuShapes.MenuItems.Add(mnuRecShape);
            /*
            MenuItem mnuOvalShape = new MenuItem("Oval", new EventHandler(OnOvalShape));
            mnuShapes.MenuItems.Add(mnuOvalShape);

            MenuItem mnuTLShape = new MenuItem("Text label", new EventHandler(OnTextLabelShape));
            mnuShapes.MenuItems.Add(mnuTLShape);

            MenuItem mnuClassShape = new MenuItem("Class bundle", new EventHandler(OnClassShape));
            mnuShapes.MenuItems.Add(mnuClassShape);

            */

        }
        /// <summary>
        /// Called on delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void OnDelete(object sender, EventArgs e)
        { }

        private void OnProperties(object sender, EventArgs e)
        {
            this.RaiseOnShowCanvasProperties(new SelectionEventArgs(new object[] { this }));
        }
        private void OnRecShape(object sender, EventArgs e)
        {
        }
        private void OnNewConnection(object sender, EventArgs e)
        {

        }

       
        
        #endregion

        #region API visible members
        /// <summary>
        /// Adds a shape to the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public IShape AddShape(IShape shape)
        {
            this.Controller.Model.AddShape(shape);
            return shape;
        }
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public IConnection AddConnection(IConnection connection)
        {
            this.Controller.Model.AddConnection(connection);
            return connection;
        }
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public IConnection AddConnection(IConnector from, IConnector to)
        {
            Connection cn = new Connection(Point.Empty, Point.Empty);
            this.AddConnection(cn);
            from.AttachConnector(cn.From);
            to.AttachConnector(cn.To);
            return cn;
        }

        /// <summary>
        ///  Changes the paint style of the selected entities.
        /// </summary>
        /// <param name="paintStyle">The paint style.</param>
        public void ChangeStyle(IPaintStyle paintStyle)
        {
            this.Controller.ChangeStyle(paintStyle);
        }
        /// <summary>
        /// Changes the pen style of the selected entities.
        /// </summary>
        /// <param name="penStyle"></param>
        public void ChangeStyle(IPenStyle penStyle)
        {
            this.Controller.ChangeStyle(penStyle);
        }

        public void Layout(LayoutType type)
        {
            if (this.Controller.Model.CurrentPage.Shapes.Count == 0)
                throw new InconsistencyException("There are no shapes on the canvas; there's nothing to lay out.");

            switch (type)
            {
                case LayoutType.ForceDirected:
                    RunActivity("ForceDirected Layout");
                    break;
                case LayoutType.FruchtermanRheingold:
                    RunActivity("FruchtermanReingold Layout");
                    break;
                case LayoutType.RadialTree:
                    SetLayoutRoot();
                    RunActivity("Radial TreeLayout");
                    break;
                case LayoutType.Balloon:
                    SetLayoutRoot();
                    RunActivity("Balloon TreeLayout");
                    break;
                case LayoutType.ClassicTree:
                    SetLayoutRoot();
                    RunActivity("Standard TreeLayout");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the layout root.
        /// </summary>
        private void SetLayoutRoot()
        {
            //this layout needs a root, you should assign one before calling this method
            if (this.Controller.Model.LayoutRoot == null)
            {
                //if a shape is selected we'll take that one as the root for the layout
                if (this.SelectedItems.Count > 0)
                    this.Controller.Model.LayoutRoot = this.SelectedItems[0] as IShape;
                else //use the zero-th shape
                    this.Controller.Model.LayoutRoot = this.Controller.Model.CurrentPage.Shapes[0];
            }
        }

        public void Unwrap(IBundle bundle)
        {
            if (bundle != null)
            {
                #region Unwrap the bundle
                Anchors.Clear();
                this.Controller.Model.Unwrap(bundle.Entities);
                Rectangle rec = Utils.BoundingRectangle(bundle.Entities);
                rec.Inflate(30, 30);
                this.Controller.View.Invalidate(rec);
                #endregion
            }
        }
        #endregion

   
    }
}
