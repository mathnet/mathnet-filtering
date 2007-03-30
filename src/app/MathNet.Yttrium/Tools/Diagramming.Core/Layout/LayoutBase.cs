using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Base class for graph layout
	/// </summary>
	abstract class LayoutBase : ActionBase, ILayout
    {
        #region Fields
        private IGraph graph;
        
        private string mName;
        private Rectangle mBounds;
        private PointF mCenter;
        private const int defaultSpan = 400;
        private IController mController = null;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the nodes of the graph. This is another casting of the shapes collection of the model.
        /// </summary>
        /// <value>The nodes.</value>
        public CollectionBase<INode> Nodes
        {
            get { return graph.Nodes; }
        }

        /// <summary>
        /// Gets the edges of the graph. This is just another casting of the connection collection of the model.
        /// </summary>
        /// <value>The edges.</value>
        public CollectionBase<IEdge> Edges
        {
            get { return graph.Edges; }
        }

        public IController Controller
        {
            get { return mController; }
        }

        protected int DefaultRunSpan
        {
            get { return defaultSpan; }
        }
        
        /// <summary>
        /// Gets or sets the center of the layout. This can be the arithmetic middle
        /// of the bounding area or can be set independently.
        /// </summary>
        public PointF Center
        {
            get { return mCenter; }
            set { mCenter = value; }
        }

        public IGraph Graph
        {
            get { return graph; }
            set { graph = value; }
        }
        /// <summary>
        /// Gets or sets the bounds of the layout surface.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds
        {
            get { return mBounds; }
            set { mBounds = value; }
        
        }
        public string LayoutName
        {
            get { return mName; }
        }
        #endregion

		#region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LayoutBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="controller">The controller.</param>
        protected LayoutBase(string name, IController controller ) : base(name)
		{
            mName = name;
            mController = controller;
		}

		#endregion

        #region Methods
        protected void setX(INode item, INode referrer, double x)
        {

            //float sx = item.Rectangle.X;
            //if(float.IsNaN(sx))
            //    sx = (referrer != null ? referrer.Rectangle.X : x);

            item.Move(new Point(Convert.ToInt32(x - item.Rectangle.X), 0));
            //Trace.WriteLine("setX(" + x + ",0)");

        }
        protected void setY(INode item, INode referrer, double y)
        {

            //float sx = item.Rectangle.X;
            //if(float.IsNaN(sx))
            //    sx = (referrer != null ? referrer.Rectangle.X : x);

            item.Move(new Point(0, Convert.ToInt32(y - item.Rectangle.Y)));
            //Trace.WriteLine("setY(0," + y +")");
        }
        #endregion


      
    }

}
