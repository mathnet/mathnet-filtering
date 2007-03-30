using System;
using System.Diagnostics;
using Netron.Diagramming.Core.Analysis;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// <para>TreeLayout instance that computes a radial layout, laying out subsequent
    ///  depth levels of a tree on circles of progressively increasing radius.
    ///  </para>
    ///  
    ///  <para>The algorithm used is that of Ka-Ping Yee, Danyel Fisher, Rachna Dhamija,
    ///  and Marti Hearst in their research paper
    ///  <a href="http://citeseer.ist.psu.edu/448292.html">Animated Exploration of
    ///  Dynamic Graphs with Radial Layout</a>, InfoVis 2001. This algorithm computes
    ///  a radial layout which factors in possible variation in sizes, and maintains
    ///  both orientation and ordering constraints to facilitate smooth and
    ///  understandable transitions between layout configurations.
    ///  </para>
    /// </summary>
    class RadialTreeLayout : TreeLayoutBase
    {
        #region Fields
        public static int DEFAULT_RADIUS = 550;
        private static int MARGIN = 30;

        protected int m_maxDepth = 0;
        protected double m_radiusInc;
        protected double m_theta1, m_theta2;
        protected bool m_setTheta = false;
        protected bool m_autoScale = true;

        protected PointF m_origin;
        protected INode m_prevRoot;
        private Dictionary<string, Params> Pars;
        BackgroundWorker worker;
        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the RadiusIncrement
        /// </summary>
        public double RadiusIncrement
        {
            get { return m_radiusInc; }
            set { m_radiusInc = value; }
        }

        public bool AutoScale
        {
            get { return m_autoScale; }
            set { m_autoScale = value; }
        }

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public RadialTreeLayout(IController controller)
            : base("Radial TreeLayout", controller)
        {

        }
        private bool Init()
        {
            m_radiusInc = DEFAULT_RADIUS;
            m_prevRoot = null;
            m_theta1 = 0;
            m_theta2 = m_theta1 + Math.PI * 2;

            this.Graph = this.Model as IGraph;

            if (Graph == null)
                throw new InconsistencyException("The model has not been set and the Graph property is hence 'null'");

            this.LayoutRoot = this.Controller.Model.LayoutRoot;//could be null if not set in the GUI
            Graph.ClearSpanningTree();
            Graph.MakeSpanningTree(LayoutRoot as INode);

            Pars = new Dictionary<string, Params>();
            if (Graph.Nodes.Count == 0)
                return false;
            if (Graph.Edges.Count == 0) //this layout is base on embedded springs in the connections
                return false;


            Params par;

            foreach (INode node in Graph.Nodes)
            {
                par = new Params();
                Pars.Add(node.Uid.ToString(), par);
            }
            return true;
        }
        #endregion

        #region Methods
        public void setAngularBounds(double theta, double width)
        {
            m_theta1 = theta;
            m_theta2 = theta + width;
            m_setTheta = true;
        }

        private void Layout()
        {


            INode n = LayoutRoot as INode;
            Params np = Pars[n.Uid.ToString()];
            // calc relative widths and maximum tree depth
            // performs one pass over the tree
            m_maxDepth = 0;
            calcAngularWidth(n, 0);

            if (m_autoScale) setScale(Bounds);
            if (!m_setTheta) calcAngularBounds(n);

            // perform the layout
            if (m_maxDepth > 0)
                layout(n, m_radiusInc, m_theta1, m_theta2);

            // update properties of the root node
            setX(n, null, m_origin.X);
            setY(n, null, m_origin.Y);
            np.angle = m_theta2 - m_theta1;
        }

        protected void setScale(RectangleF bounds)
        {
            double r = Math.Min(Bounds.Width, Bounds.Height) / 2.0D;
            if (m_maxDepth > 0)
                m_radiusInc = 3 * (r - MARGIN) / m_maxDepth;
        }
        private void calcAngularBounds(INode r)
        {
            if (m_prevRoot == null || r == m_prevRoot)             //|| !m_prevRoot.isValid() 
            {
                m_prevRoot = r;
                return;
            }

            // try to find previous parent of root
            INode p = m_prevRoot;
            while (true)
            {
                INode pp = (INode)p.ParentNode;
                if (pp == r)
                {
                    break;
                }
                else if (pp == null)
                {
                    m_prevRoot = r;
                    return;
                }
                p = pp;
            }

            // compute offset due to children's angular width
            double dt = 0;
            CollectionBase<INode> iter = sortedChildren(r);
            foreach (INode n in iter)
            {
                if (n == p) break;
                dt += Pars[n.Uid.ToString()].width;
            }
            double rw = Pars[r.Uid.ToString()].width;
            double pw = Pars[p.Uid.ToString()].width;
            dt = -Math.PI * 2 * (dt + pw / 2) / rw;

            // set angular bounds
            m_theta1 = dt + Math.Atan2(p.Y - r.Y, p.X - r.X);
            m_theta2 = m_theta1 + Math.PI * 2;
            m_prevRoot = r;
        }
        private double calcAngularWidth(INode n, int d)
        {
            if (d > m_maxDepth) m_maxDepth = d;
            double aw = 0;

            RectangleF bounds = n.Rectangle;
            double w = Bounds.Width, h = Bounds.Height;
            double diameter = d == 0 ? 0 : Math.Sqrt(w * w + h * h) / d;

            if (n.IsExpanded && n.ChildCount > 0)
            {

                foreach (INode c in n.Children)
                {
                    aw += calcAngularWidth(c, d + 1);
                }
                aw = Math.Max(diameter, aw);
            }
            else
            {
                aw = diameter;
            }
            Pars[n.Uid.ToString()].width = aw;
            return aw;
        }

        private static double normalize(double angle)
        {
            while (angle > Math.PI * 2)
            {
                angle -= Math.PI * 2;
            }
            while (angle < 0)
            {
                angle += Math.PI * 2;
            }
            return angle;
        }

        private CollectionBase<INode> sortedChildren(INode n)
        {
            double basevalue = 0;
            // update basevalue angle for node ordering
            INode p = n.ParentNode;
            if (p != null)
            {
                basevalue = normalize(Math.Atan2(p.Y - n.Y, p.X - n.X));
            }
            int cc = n.ChildCount;
            if (cc == 0) return null;

            INode c = (INode)n.FirstChild;

            // TODO: this is hacky and will break when filtering
            // how to know that a branch is newly expanded?
            // is there an alternative property we should check?
            //if ( !c.isStartVisible() ) 
            //{
            //    // use natural ordering for previously invisible nodes
            //    return n.Children;
            //}



            double[] angle = new double[cc];
            int[] idx = new int[cc];
            for (int i = 0; i < cc; ++i, c = c.NextSibling)
            {
                idx[i] = i;
                angle[i] = normalize(-basevalue + Math.Atan2(c.Y - n.Y, c.X - n.X));
            }

            Array.Sort(angle, idx);//or is it the other way around
            CollectionBase<INode> col = new CollectionBase<INode>();
            CollectionBase<INode> children = n.Children;
            for (int i = 0; i < cc; ++i)
            {
                col.Add(children[idx[i]]);
            }
            return col;

            // return iterator over sorted children
            //return new Iterator() {
            //    int cur = 0;
            //    public Object next() {
            //        return n.getChild(idx[cur++]);
            //    }
            //    public bool hasNext() {
            //        return cur < idx.Length;
            //    }
            //    public void remove() {
            //        throw new UnsupportedOperationException();
            //    }
            //};
        }
        protected void layout(INode n, double r, double theta1, double theta2)
        {
            double dtheta = (theta2 - theta1);
            double dtheta2 = dtheta / 2.0;
            double width = Pars[n.Uid.ToString()].width;
            double cfrac, nfrac = 0.0;
            foreach (INode c in sortedChildren(n))
            {
                Params cp = Pars[c.Uid.ToString()];
                cfrac = cp.width / width;
                if (c.IsExpanded && c.ChildCount > 0)
                {
                    layout(c, r + m_radiusInc, theta1 + nfrac * dtheta, theta1 + (nfrac + cfrac) * dtheta);
                }
                setPolarLocation(c, n, r, theta1 + nfrac * dtheta + cfrac * dtheta2);
                cp.angle = cfrac * dtheta;
                nfrac += cfrac;
            }

        }
        protected void setPolarLocation(INode n, INode p, double r, double t)
        {
            setX(n, p, m_origin.X + r * Math.Cos(t));
            setY(n, p, m_origin.Y + r * Math.Sin(t));
        }
        public override void Run()
        {
            Run(2000);
        }
        public override void Run(int time)
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(time);
        }

        public override void Stop()
        {
            if (worker != null && worker.IsBusy)
                worker.CancelAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Controller.View.Suspend();
            Init();
            Layout();
            this.Controller.View.Resume();
        }


        #endregion

        /// <summary>
        /// Paramter blob to temporarily keep working data of one node.
        /// </summary>
        class Params
        {
            public double width = 0;
            public double angle = 0;
            public Object clone()
            {
                Params p = new Params();
                p.width = this.width;
                p.angle = this.angle;
                return p;
            }
        }

    }

}
