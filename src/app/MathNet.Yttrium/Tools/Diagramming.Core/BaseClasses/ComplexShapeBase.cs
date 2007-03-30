using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for complex shapes, cfr <see cref="SimpleShapeBase"/>
    /// </summary>
    public abstract partial class ComplexShapeBase : ShapeBase, IComplexShape, IMouseListener, IHoverListener
    {
        #region Fields
        /// <summary>
        /// the text on the bundle
        /// </summary>
        private string text = string.Empty;
        /// <summary>
        /// the shape children or sub-controls
        /// </summary>
        private CollectionBase<IShapeMaterial> mChildren;
        /// <summary>
        /// the services of this shape
        /// </summary>
        private Dictionary<Type, IInteraction> mServices;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text of the bundle
        /// </summary>
        [Browsable(true), Description("The text shown on the shape"), Category("Layout")]
        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets the services provided by the shape.
        /// </summary>
        /// <value>The services.</value>
        public Dictionary<Type, IInteraction> Services
        {
            get { return mServices; }
        }
     
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ComplexShapeBase() : base()
        {
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexShapeBase"/> class.
        /// </summary>
        /// <param name="model"></param>
        public ComplexShapeBase(IModel model)
            : base(model)
        {
            Init();
        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            mChildren = new CollectionBase<IShapeMaterial>();
            mChildren.OnItemAdded += new EventHandler<CollectionEventArgs<IShapeMaterial>>(mChildren_OnItemAdded);
            mServices = new Dictionary<Type, IInteraction>();
            //mChildren.Clear();
            //mServices.Clear();
            mServices[typeof(IMouseListener)] = this;
            mServices[typeof(IHoverListener)] = this;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Sets some contextual properties of the <see cref="Children"/> when a new item is added.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void mChildren_OnItemAdded(object sender, CollectionEventArgs<IShapeMaterial> e)
        {
            e.Item.Shape = this;
        }

        /// <summary>
        /// Overrides the abstract paint method
        /// </summary>
        /// <param name="g">a graphics object onto which to paint</param>
        public override void Paint(Graphics g)
        {
            
            base.Paint(g);

           

            foreach (IPaintable material in Children)
            {
                material.Paint(g);
            }
          
        }

        /// <summary>
        /// Moves the entity with the given shift
        /// </summary>
        /// <param name="p">represent a shift-vector, not the absolute position!</param>
        public override void Move(Point p)
        {
            base.Move(p);
            Rectangle rec;
            foreach (IShapeMaterial material in Children)
            {
                rec = material.Rectangle;

                rec.Offset(p.X, p.Y);
                material.Transform(rec);
            }
        }


        /// <summary>
        /// Transforms the entity to the given new rectangle.
        /// </summary>
        /// <param name="x">The x-coordinate of the new rectangle.</param>
        /// <param name="y">The y-coordinate of the new rectangle.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void Transform(int x, int y, int width, int height)
        {
            if (Children != null && Children.Count > 0)
            {

                int a, b, w, h;
                foreach (IShapeMaterial material in Children)
                {

                    if (material.Gliding)
                    {
                        a = Convert.ToInt32(Math.Round(((double)material.Rectangle.X - (double)Rectangle.X) / (double)Rectangle.Width * width, 1) + x);
                        b = Convert.ToInt32(Math.Round(((double)material.Rectangle.Y - (double)Rectangle.Y) / (double)Rectangle.Height * height, 1) + y);
                    }
                    else //shift the material, do not scale the position with respect to the sizing of the shape
                    {
                        a = material.Rectangle.X - Rectangle.X + x;
                        b = material.Rectangle.Y - Rectangle.Y + y;
                    }
                    if (material.Resizable)
                    {
                        w = Convert.ToInt32(Math.Round(((double)material.Rectangle.Width) / ((double)Rectangle.Width), 1) * width);
                        h = Convert.ToInt32(Math.Round(((double)material.Rectangle.Height) / ((double)Rectangle.Height), 1) * height);
                    }
                    else
                    {
                        w = material.Rectangle.Width;
                        h = material.Rectangle.Height;
                    }

                    material.Transform(new Rectangle(a, b, w, h));
                }
            }
            base.Transform(x, y, width, height);
        }
        #endregion

        

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public virtual CollectionBase<IShapeMaterial> Children
        {
            get
            {
                return mChildren;
            }
            set
            {
                throw new InconsistencyException("You cannot set the children, use the existing collection and the clear() method.");    
            }
        }

        /// <summary>
        /// Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            if (Services.ContainsKey(serviceType))
                return Services[serviceType];
            else
                return null;
        }

        /// <summary>
        /// <see cref="IMouseListener"/> implementation.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            
            IMouseListener listener ;
            foreach (IShapeMaterial material in mChildren)
            {
                if(material.Rectangle.Contains(e.Location) && material.Visible)
                {
                    listener = material.GetService(typeof(IMouseListener)) as IMouseListener;
                    if (listener != null)
                        if (listener.MouseDown(e))
                            return true;
                }

            }
            return false;
        }

        /// <summary>
        /// <see cref="IMouseListener"/> implementation.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            IMouseListener listener;
            foreach (IShapeMaterial material in mChildren)
            {
                if (material.Rectangle.Contains(e.Location) && material.Visible)
                {
                    listener = material.GetService(typeof(IMouseListener)) as IMouseListener;
                    if (listener != null) listener.MouseMove(e);
                }
            }
        }

        /// <summary>
        /// <see cref="IMouseListener"/> implementation.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            IMouseListener listener;
            foreach (IShapeMaterial material in mChildren)
            {
                if(material.Rectangle.Contains(e.Location) && material.Visible)
                {
                    listener = material.GetService(typeof(IMouseListener)) as IMouseListener;
                    if (listener != null) listener.MouseUp(e);
                }
            }
        }



        #region IHoverListener Members

        private IHoverListener currentHoveredMaterial;

        /// <summary>
        /// Handles the MouseHover event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHover(System.Windows.Forms.MouseEventArgs e)
        {
            IHoverListener listener;
            foreach(IShapeMaterial material in this.Children)
            {
                if(material.Rectangle.Contains(e.Location) && material.Visible) //we caught an material and it's visible
                {
                    listener = material.GetService(typeof(IHoverListener)) as IHoverListener;
                    if(listener != null) //the caught material does listen
                    {
                        if(currentHoveredMaterial == listener) //it's the same as the previous time
                            listener.MouseHover(e);
                        else //we moved from one material to another listening material
                        {
                            if(currentHoveredMaterial != null) //tell the previous material we are leaving
                                currentHoveredMaterial.MouseLeave(e);
                            listener.MouseEnter(e); //tell the current one we enter
                            currentHoveredMaterial = listener;
                        }
                    }
                    else //the caught material does not listen
                    {
                        if(currentHoveredMaterial != null)
                        {
                            currentHoveredMaterial.MouseLeave(e);
                            currentHoveredMaterial = null;
                        }
                    }
                    return; //only one material at a time
                }

            }
            if(currentHoveredMaterial != null)
            {
                currentHoveredMaterial.MouseLeave(e);
                currentHoveredMaterial = null;
            }
            
        }

        /// <summary>
        /// Handles the OnMouseEnter event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseEnter(System.Windows.Forms.MouseEventArgs e)
        {
            
        }

        /// <summary>
        /// Handles the OnMouseLeave event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseLeave(System.Windows.Forms.MouseEventArgs e)
        {
          
        }

        #endregion
    }
}
