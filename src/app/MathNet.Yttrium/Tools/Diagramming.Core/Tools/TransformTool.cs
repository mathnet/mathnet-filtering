using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool implement the action of scaling and/or rotating shapes on the canvas. 
    /// </summary>
    class TransformTool : AbstractTool, IMouseListener
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
        /// whether or not there is a transformation going on
        /// </summary>
        private bool changing;
        /// <summary>
        /// the entities being transformed
        /// </summary>
        private Hashtable transformers;
        /// <summary>
        /// the origin vector where the scaling has its start
        /// </summary>
        double ox, oy; 
        /// <summary>
        /// the scale with which the entities will be scaled
        /// </summary>
        double scale , scalex, scaley;
        /// <summary>
        /// the eight possible types of transformations
        /// </summary>
        private TransformTypes transform;
        /// <summary>
        /// dummy which is calculated in function of the current resizing location
        /// </summary>
        Point origin = Point.Empty; 
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TransformTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public TransformTool(string name)
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
            //nothing to do

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

                if (Selection.SelectedItems.Count > 0)
                {
                    //let the tracker tell us if anything was hit
                    Point gripPoint = this.Controller.View.Tracker.Hit(e.Location);
                    Cursor c = null;
                    changing = false;

                    #region determine and set the corresponding cursor
                    switch (gripPoint.X)
                    {
                        case -1:
                            switch (gripPoint.Y)
                            {
                                case -1:
                                    c = Cursors.SizeNWSE;
                                    transform = TransformTypes.NW;
                                    //the origin is the right-bottom of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Right - ViewBase.TrackerOffset;
                                    oy = this.Controller.View.Tracker.Rectangle.Bottom - ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                                case 0:
                                    c = Cursors.SizeWE;
                                    transform = TransformTypes.W;
                                    //the origin is the right-top of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Right - ViewBase.TrackerOffset;// +this.Controller.View.Tracker.Rectangle.Width / 2;
                                    oy = this.Controller.View.Tracker.Rectangle.Top + ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                                case 1:
                                    c = Cursors.SizeNESW;
                                    transform = TransformTypes.SW;
                                    //the origin is the right-top of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Right - ViewBase.TrackerOffset;
                                    oy = this.Controller.View.Tracker.Rectangle.Top + ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                            }
                            break;
                        case 0:
                            switch (gripPoint.Y)
                            {
                                case -1:
                                    c = Cursors.SizeNS;
                                    transform = TransformTypes.N;
                                    //the origin is the center-bottom of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Left + ViewBase.TrackerOffset;// +this.Controller.View.Tracker.Rectangle.Width / 2;
                                    oy = this.Controller.View.Tracker.Rectangle.Bottom - ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                                case 1:
                                    c = Cursors.SizeNS;
                                    transform = TransformTypes.S;
                                    //the origin is the left-top of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Left + ViewBase.TrackerOffset;// +this.Controller.View.Tracker.Rectangle.Width / 2;
                                    oy = this.Controller.View.Tracker.Rectangle.Top + ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                            }
                            break;
                        case 1:
                            switch (gripPoint.Y)
                            {
                                case -1:
                                    c = Cursors.SizeNESW;
                                    transform = TransformTypes.NE;
                                    //the origin is the left-bottom of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Left + ViewBase.TrackerOffset;
                                    oy = this.Controller.View.Tracker.Rectangle.Bottom - ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                                case 0:
                                    c = Cursors.SizeWE;
                                    transform = TransformTypes.E;
                                    //the origin is the left-top of the rectangle
                                    ox = this.Controller.View.Tracker.Rectangle.Left + ViewBase.TrackerOffset;// +this.Controller.View.Tracker.Rectangle.Width / 2;
                                    oy = this.Controller.View.Tracker.Rectangle.Top + ViewBase.TrackerOffset;
                                    changing = true;
                                    break;
                                case 1:
                                    c = Cursors.SizeNWSE;
                                    transform = TransformTypes.SE;
                                    //the origin is the left-top of the tracker rectangle plus the little tracker offset
                                    ox = this.Controller.View.Tracker.Rectangle.X + ViewBase.TrackerOffset;
                                    oy = this.Controller.View.Tracker.Rectangle.Y + ViewBase.TrackerOffset;

                                    changing = true;
                                    break;
                            }
                            break;
                    }

                    #endregion


                    if (changing)
                    {
                        #region Changing/transforming
                        //the point where the scaling or dragging of the tracker started
                        initialPoint = e.Location;
                        //recursive location of the dragging location
                        lastPoint = initialPoint;
                        //start the tool
                        this.ActivateTool();
                        //set the cursor corresponding to the grip
                        Controller.View.CurrentCursor = c;
                        //create a new collection for the transforming entities
                        transformers = new Hashtable();
                        //the points of the connectors
                        Point[] points = null;
                        //the entity bone
                        EntityBone bone;
                        //take the flat selection and keep the current state as a reference for the transformation
                        //until the mouseup pins down the final scaling
                        foreach (IDiagramEntity entity in Selection.FlattenedSelectionItems)
                        {
                            if (!entity.Resizable) continue;//only take what's resizable

                            bone = new EntityBone();

                            if (entity is IShape)
                            { 
                                IShape shape = entity as IShape;
                                if (shape.Connectors.Count > 0)
                                {
                                    points = new Point[shape.Connectors.Count];
                                    for (int m = 0; m < shape.Connectors.Count; m++)
                                    {
                                        points[m] = shape.Connectors[m].Point;
                                    }
                                }
                                else
                                    points = null;
                                bone.Rectangle = entity.Rectangle;
                                bone.ConnectorPoints = points;
                            }
                            else if (entity is IConnection)
                            {
                                IConnection con = entity as IConnection;
                                points = new Point[2] { Point.Empty, Point.Empty };
                                //Only non-attached connection connectors have to be scaled
                                //Attached connectors will move with their parent.
                                if (con.From.AttachedTo == null)
                                    points[0] = con.From.Point;
                                if (con.To.AttachedTo == null)
                                    points[1] = con.To.Point;
                                //We define a connection as a bone with empty rectangle
                                //One could use some additional members to label it but it's only overhead at this point.
                                bone.Rectangle = Rectangle.Empty;
                                bone.ConnectorPoints = points;
                            }
                            transformers.Add(entity, bone);
                        }
                        #endregion

                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Resizes the selected entities. 
        /// <remarks>The whole logic and geometry behind the resizing is quite involved, there is a detailed Visio diagram
        /// showing the elements of the calculations and how the various switch cases are linked together.
        /// </remarks>
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
            #region Checking the incoming data
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            #endregion

            #region Definition to make the code more readable

            //some annoying but necessary type conversions here to double precision
            double lastLength = 0; //the diagonal length of the current rectangle we are resizing
            double eX = e.X, eY = e.Y; //the vector corresponding to the mouse location
            double lx = lastPoint.X, ly = lastPoint.Y; // the lastpoint vector
            double iX = initialPoint.X, iY = initialPoint.Y; // the vactor when the motion started

            double mx, my; //the motion vector
            double rx = 0, ry = 0; //the resulting vector
            
            double sign = 1;
            #endregion

            if (IsActive)
            {

                #region The current motion vector is computed
                mx = eX - lx;
                my = eY - ly;
                #endregion

                #region Switching between the different compass directions of the grips
                //the transform is the compass direction of the grip used to resize the entities
                switch (transform)
                {
                    #region NW
                    case TransformTypes.NW:



                        //the region above the X-Y=0 diagonal uses the horizontal to project the motion vector onto the diagonal
                        if (mx - my < 0)
                        {
                            scale = (ox - (double)e.X) / (ox - (double)initialPoint.X);
                        }
                        else
                        {
                            scale = (oy - (double)e.Y) / (oy - (double)initialPoint.Y);
                        }
                        //now we can pass the info the the scaling method of the selected entities
                        scalex = scale;
                        scaley = scale;
                        break;
                    #endregion

                    #region N
                    case TransformTypes.N:
                        //remember that the coordinate system has its Y-axis pointing downwards                        
                        scale = (oy - (double)e.Y) / (oy - (double)initialPoint.Y);

                        //now we can pass the info the the scaling method of the selected entities
                        scalex = 1F;
                        scaley = scale;
                        break;
                    #endregion

                    #region NE
                    case TransformTypes.NE:
                        //the region above the X-Y=0 diagonal uses the horizontal to project the motion vector onto the diagonal
                        if (mx + my > 0)
                        {
                            scale = ((double)e.X - ox) / ((double)initialPoint.X - ox);

                        }
                        else
                        {
                            scale = ((double)e.Y - oy) / ((double)initialPoint.Y - oy);

                        }
                        //now we can pass the info the the scaling method of the selected entities
                        scalex = scale;
                        scaley = scale;
                        break;
                    #endregion

                    #region E
                    case TransformTypes.E:
                        //remember that the coordinate system has its Y-axis pointing downwards                        
                        scale = ((double)e.X - ox) / ((double)initialPoint.X - ox);

                        //now we can pass the info the the scaling method of the selected entities
                        scalex = scale;
                        scaley = 1F;
                        break;
                    #endregion

                    #region SE
                    case TransformTypes.SE:
                        //I'd call this the Visio effect...
                        if (mx - my > 0)
                        {
                            scale = ((double)e.X - ox) / ((double)initialPoint.X - ox);
                        }
                        else
                            scale = ((double)e.Y - oy) / ((double)initialPoint.Y - oy);

                        scalex = scale;
                        scaley = scale;
                        break;
                    #endregion

                    #region S
                    case TransformTypes.S:
                        //remember that the coordinate system has its Y-axis pointing downwards                        
                        scale = ((double)e.Y - oy) / ((double)initialPoint.Y - oy);

                        //now we can pass the info the the scaling method of the selected entities
                        scalex = 1F;
                        scaley = scale;
                        break;
                    #endregion

                    #region SW
                    case TransformTypes.SW:

                        //the region above the X-Y=0 diagonal uses the horizontal to project the motion vector onto the diagonal
                        if (mx + my < 0)
                        {
                            scale = ((double)e.X - ox) / ((double)initialPoint.X - ox);
                        }
                        else
                        {
                            scale = ((double)e.Y - oy) / ((double)initialPoint.Y - oy);

                        }
                        //now we can pass the info the the scaling method of the selected entities
                        scalex = scale;
                        scaley = scale;
                        break;
                    #endregion

                    #region W
                    case TransformTypes.W:
                        //remember that the coordinate system has its Y-axis pointing downwards                        
                        scale = (ox - (double)e.X) / (ox - (double)initialPoint.X);

                        //now we can pass the info the the scaling method of the selected entities
                        scalex = scale;
                        scaley = 1F;
                        break;
                    #endregion

                }
                #endregion

                #region Scale the selected entities
                //block scaling below some minimum
                if (lastLength <= 70 && sign == -1)
                    return;
                //no need to use the rounding Convert method since the ox and oy doubles are really integers
                //but the calculations above requires double data types.
                origin = new Point(Convert.ToInt32(ox), Convert.ToInt32(oy));
                //update the location of the last point
                lastPoint.Offset(Convert.ToInt32(rx), Convert.ToInt32(ry));

                TransformCommand.Transform(origin, scalex, scaley, transformers);

                #endregion

                //since we used the flattened selection the group shapes are unaware of the the resize, so we 
                //have to recalculate the group rectangles
                foreach (IDiagramEntity entity in Selection.SelectedItems)
                {
                    //the calculation will cascade to subgroups if necessary
                    if (entity is IGroup) (entity as IGroup).CalculateRectangle();
                }
                //update the state of the tracker, i.e show it again and it'll be recalculated
                this.Controller.View.ShowTracker();
            }
        }


        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {
                DeactivateTool();
                TransformCommand cmd = new TransformCommand(this.Controller, origin, scalex, scaley, transformers);
                this.Controller.UndoManager.AddUndoCommand(cmd);
            }
        }
        #endregion

        #region Structure

        #endregion
    }


}
