using System;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for shapes.
    /// </summary>
    public abstract partial class ShapeBase : DiagramEntityBase, IShape
    {
        #region Constants
        /// <summary>
        /// The minimum size of the shape
        /// </summary>
        public const int constCutOff = 30;
        #endregion

        #region Events



        #endregion

        #region Fields
        /// <summary>
        /// the mRectangle on which any bundle lives
        /// </summary>
        private Rectangle mRectangle = Rectangle.Empty;        
        /// <summary>
        /// the collection of mConnectors onto which you can attach a connection
        /// </summary>
        private CollectionBase<IConnector> mConnectors;
        private bool mIsFixed = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the canvas to which the entity belongs
        /// </summary>
        /// <value></value>
        public override IModel Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                if (Connectors == null)
                    throw new InconsistencyException("The connectors collection is 'null'.");
                foreach (IConnector con in Connectors)
                {
                    con.Model = value;
                }
            }
        }

    

        /// <summary>
        /// Gets or sets the mConnectors of this bundle
        /// </summary>		
        public CollectionBase<IConnector> Connectors
        {
            get { return mConnectors; }
            set { mConnectors = value; }
        }

        /// <summary>
        /// Gets or sets the width of the bundle
        /// </summary>
        [Browsable(true), Description("The width of the shape"), Category("Layout")]
        public int Width
        {
            get { return this.mRectangle.Width; }
            set { Transform(this.Rectangle.X, this.Rectangle.Y, value, this.Height); }
        }

        /// <summary>
        /// Gets or sets the height of the bundle
        /// </summary>		
        [Browsable(true), Description("The height of the shape"), Category("Layout")]
        public int Height
        {
            get { return this.mRectangle.Height; }
            set
            {
                Transform(this.Rectangle.X, this.Rectangle.Y, this.Width, value);
            }
        }



        /// <summary>
        /// the x-coordinate of the upper-left corner
        /// </summary>
        [Browsable(true), Description("The x-coordinate of the upper-left corner"), Category("Layout")]
        public int X
        {
            get { return mRectangle.X; }
            set
            {
                Point p = new Point(value - mRectangle.X, mRectangle.Y);
                this.Move(p);

                //if(Model!=null)
                //  Model.RaiseOnInvalidate(); //note that 'this.Invalidate()' will not be enough
            }
        }

        /// <summary>
        /// the y-coordinate of the upper-left corner
        /// </summary>
        [Browsable(true), Description("The y-coordinate of the upper-left corner"), Category("Layout")]
        public int Y
        {
            get { return mRectangle.Y; }
            set
            {
                Point p = new Point(mRectangle.X, value - mRectangle.Y);
                this.Move(p);
                //Model.RaiseOnInvalidate();
            }
        }
     

        /// <summary>
        /// Gets or sets the location of the bundle;
        /// </summary>
        [Browsable(false)]
        public Point Location
        {
            get { return new Point(this.mRectangle.X, this.mRectangle.Y); }
            set
            {
                //we use the move method but it requires the delta value, not an absolute position!
                Point p = new Point(value.X - mRectangle.X, value.Y - mRectangle.Y);
                //if you'd use this it would indeed move the bundle but not the connector s of the bundle
                //this.mRectangle.X = value.X; this.mRectangle.Y = value.Y; Invalidate();
                this.Move(p);
            }
        }

        /// <summary>
        /// The bounds of the paintable entity
        /// </summary>
        /// <value></value>
        public override Rectangle Rectangle
        {
            get { return mRectangle; }
            //set{mRectangle = value;

            //this.Invalidate();              }
        }

      
        #endregion

        #region Constructor


        /// <summary>
        /// Constructor with the site of the bundle
        /// </summary>
        /// <param name="model"></param>
        protected ShapeBase(IModel model)
            : base(model)
        {
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ShapeBase"/> class.
        /// </summary>
        protected ShapeBase()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init()
        {
            mConnectors = new CollectionBase<IConnector>();
            mConnectors.OnItemAdded += new EventHandler<CollectionEventArgs<IConnector>>(mConnectors_OnItemAdded);
            mConnectors.OnItemRemoved += new EventHandler<CollectionEventArgs<IConnector>>(mConnectors_OnItemRemoved);
            mRectangle = new Rectangle(0, 0, 100, 70);

            PaintStyle = ArtPallet.GetDefaultPaintStyle();
            PenStyle = ArtPallet.GetDefaultPenStyle();
        }

        

        #endregion

        #region Methods
        /// <summary>
        /// Generates a new Uid for this entity.
        /// </summary>
        /// <param name="recursive">if the Uid has to be changed recursively down to the sub-entities, set to true, otherwise false.</param>
        public override void NewUid(bool recursive)
        {
            
            if (recursive)
            {
                foreach (IConnector connector in Connectors)
                {
                    connector.NewUid(recursive);
                }
                base.NewUid(recursive);
            }
            else
                base.NewUid(recursive);
        }
        /// <summary>
        /// Part of the initialization, this method connects newly added connectors to the parenting shape.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mConnectors_OnItemAdded(object sender, CollectionEventArgs<IConnector> e)
        {
            e.Item.Parent = this;
            Model.ConnectorHolders.Add(e.Item, this);
        }

        void mConnectors_OnItemRemoved(object sender, CollectionEventArgs<IConnector> e)
        {
            Model.ConnectorHolders.Remove(e.Item);
        }

        /// <summary>
        /// The custom menu to be added to the base menu of this entity
        /// </summary>
        /// <returns></returns>
        public override MenuItem[] ShapeMenu()
        {
            return null;
        }

        /// <summary>
        /// Returns the connector hit by the mouse, if any
        /// </summary>
        /// <param name="p">the mouse coordinates</param>
        /// <returns>the connector hit by the mouse</returns>
        public IConnector HitConnector(Point p)
        {
            for (int k = 0; k < mConnectors.Count; k++)
            {
                if (mConnectors[k].Hit(p))
                {
                    mConnectors[k].Hovered = true;
                    mConnectors[k].Invalidate();
                    return mConnectors[k];
                }
                else
                {
                    mConnectors[k].Hovered = false;
                    mConnectors[k].Invalidate();

                }


            }
            return null;

        }

       

        /// <summary>
        /// Overrides the abstract paint method
        /// </summary>
        /// <param name="g">a graphics object onto which to paint</param>
        public override void Paint(System.Drawing.Graphics g)
        {           
            return;
        }

        /// <summary>
        /// Override the abstract Hit method
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            return false;
        }

        /// <summary>
        /// Overrides the abstract Invalidate method
        /// </summary>
        public override void Invalidate()
        {
            Rectangle r = Rectangle;
            r.Offset(-10, -10);
            r.Inflate(40, 40);
            if (Model != null)
                Model.RaiseOnInvalidateRectangle(r);
        }



        /// <summary>
        /// Moves the entity with the given shift
        /// </summary>
        /// <param name="p">represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {
            Rectangle recBefore = mRectangle;
            recBefore.Inflate(20, 20);

            this.mRectangle.X += p.X;
            this.mRectangle.Y += p.Y;

            UpdatePaintingMaterial();

            for (int k = 0; k < this.mConnectors.Count; k++)
            {
                mConnectors[k].Move(p);
            }
            //refresh things
            this.Invalidate(recBefore);//position before the move
            this.Invalidate();//current position

        }


        /// <summary>
        /// Scales the entity with the given factor at the given origin.
        /// <remarks>More an historical milestone than used code.</remarks>
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="scaleX">The scale X.</param>
        /// <param name="scaleY">The scale Y.</param>
        public void Scale(Point origin, double scaleX, double scaleY)
        {
            #region Variables
            //temporary variables to assign the new location of the mConnectors            
            double a, b;
            //the new location of the connector
            Point p;
            //calculated/scaled/biased corners of the new rectangle
            double ltx = 0, lty = 0, rbx = 0, rby = 0;

            Rectangle currentRectangle = Rectangle;
            //we only need to transform the LT and RB corners since the rest of the rectangle can be deduced from that
            /*
            PointF[] corners = new PointF[]{new PointF(currentRectangle.X, currentRectangle.Y),                                                            
                                                            new PointF(currentRectangle.Right, currentRectangle.Bottom),                                                            
            
             };
             */
            Rectangle newRectangle;
            #endregion

            #region Transformation matrix

            ltx = Math.Round((currentRectangle.X - origin.X) * scaleX, 1) + origin.X;
            lty = Math.Round((currentRectangle.Y - origin.Y) * scaleY, 1) + origin.Y;

            rbx = Math.Round((currentRectangle.Right - origin.X) * scaleX, 1) + origin.X;
            rby = Math.Round((currentRectangle.Bottom - origin.Y) * scaleY, 1) + origin.Y;

            //corners[0] = new PointF
            //Matrix m = new Matrix();
            // m.Translate(-origin.X, -origin.Y,MatrixOrder.Append);
            // m.Scale(scaleX, scaleY, MatrixOrder.Append);
            // m.Translate(origin.X, origin.Y, MatrixOrder.Append);
            #endregion

            //transfor the LTRB points of the current rectangle
            //m.TransformPoints(corners);

            #region Bias
            /*
            if(currentRectangle.Y <= origin.Y + ViewBase.TrackerOffset && origin.Y - ViewBase.TrackerOffset <= currentRectangle.Y)
            {
                //do not scale in the Y-direction
                lty = currentRectangle.Y;
            }
            
            if(currentRectangle.X <= origin.X+ ViewBase.TrackerOffset && origin.X - ViewBase.TrackerOffset <= currentRectangle.X)
            {
                //do not scale in the X-direction
                ltx = currentRectangle.X;
            }
            
            if(currentRectangle.Right <= origin.X + ViewBase.TrackerOffset && origin.X - ViewBase.TrackerOffset <= currentRectangle.Right)
            {
                //do not scale in the X-direction
                rbx = currentRectangle.Right;
            }
            
            if(currentRectangle.Bottom <= origin.Y + ViewBase.TrackerOffset && origin.Y - ViewBase.TrackerOffset <= currentRectangle.Bottom)
            {
                //do not scale in the Y-direction            
                rby = currentRectangle.Bottom;
            } 
             */
            #endregion
            /*
            ltx = Math.Round(ltx);
            lty = Math.Round(lty);
            rbx = Math.Round(rbx);
            rby = Math.Round(rby);
             * */
            //now we can re-create the rectangle of this shape            
            //newRectangle = RectangleF.FromLTRB(ltx, lty, rbx, rby);
            newRectangle = Rectangle.FromLTRB(Convert.ToInt32(ltx), Convert.ToInt32(lty), Convert.ToInt32(rbx), Convert.ToInt32(rby));
            //if ((newRectangle.Width <= 50 && scaleX < 1) || (newRectangle.Height <= 50 && scaleY < 1))
            //    return;
            #region Scaling of the mConnectors
            //Note that this mechanism is way easier than the calculations in the old Netron library
            //and it also allows dynamic mConnectors.
            foreach (IConnector cn in this.mConnectors)
            {
                //De profundis: ge wilt het gewoon nie weten hoeveel bloed, zweet en tranen ik in de onderstaande berekeningen heb gestoken...
                //met al de afrondingen en meetkundinge schaalafwijkingen..tis een wonder dat ik eruit ben geraakt.

                //Scaling preserves proportions, so we calculate the proportions before the rectangle was resized and
                //re-assign the same proportion after the rectangle is resized.
                //I have tried many, many different possibilities but the accumulation of double-to-int conversions is a real pain.
                //The only working solution I found was to cut things off after the first decimal.
                a = Math.Round(((double)cn.Point.X - (double)mRectangle.X) / (double)mRectangle.Width, 1) * newRectangle.Width + ltx;
                b = Math.Round(((double)cn.Point.Y - (double)mRectangle.Y) / (double)mRectangle.Height, 1) * newRectangle.Height + lty;
                p = new Point(Convert.ToInt32(a), Convert.ToInt32(b));
                cn.Point = p;
            }
            #endregion

            //assign the new calculated rectangle to this shape
            this.mRectangle = newRectangle;
            //invalidate the space before the resize; very important if the scaling is a contraction!
            this.Invalidate(currentRectangle);
            //invalidate the current situation
            this.Invalidate();
        }



        /// <summary>
        /// Transforms the entity to the given new rectangle
        /// </summary>
        /// <param name="x">The x-coordinate of the new rectangle.</param>
        /// <param name="y">The y-coordinate of the new rectangle.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public virtual void Transform(int x, int y, int width, int height)
        {
           
            //cut-off
            //if(width < constCutOff || height < constCutOff)
            //    return;
            double a, b;
            Point p;
            Rectangle before = mRectangle;
            before.Inflate(20, 20);
            foreach (IConnector cn in this.mConnectors)
            {
                a = Math.Round(((double)cn.Point.X - (double)mRectangle.X) / (double)mRectangle.Width, 1) * width + x - cn.Point.X;
                b = Math.Round(((double)cn.Point.Y - (double)mRectangle.Y) / (double)mRectangle.Height, 1) * height + y - cn.Point.Y;
                p = new Point(Convert.ToInt32(a), Convert.ToInt32(b));
                cn.Move(p);
            }
            mRectangle = new Rectangle(x, y, width, height);
            //update the material; the gradient depends on the rectangle
            UpdatePaintingMaterial();
            Invalidate(before);
            Invalidate(mRectangle);
            
        }

        /// <summary>
        /// Transforms the entity to the given new rectangle
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public void Transform(Rectangle rectangle)
        {
           
            Transform(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        #endregion


    }
}
