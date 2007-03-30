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
    /// <para>TreeLayout that computes a tidy layout of a node-link tree
    ///  diagram. This algorithm lays out a rooted tree such that each
    ///  depth level of the tree is on a shared line. The orientation of the
    ///  tree can be set such that the tree goes left-to-right (default),
    ///  right-to-left, top-to-bottom, or bottom-to-top.</para>
    ///  
    ///  <para>The algorithm used is that of Christoph Buchheim, Michael Jünger,
    ///  and Sebastian Leipert from their research paper
    ///  <a href="http://citeseer.ist.psu.edu/buchheim02improving.html">
    ///  Improving Walker's Algorithm to Run in Linear Time</a>, Graph Drawing 2002.
    ///  This algorithm corrects performance issues in Walker's algorithm, which
    ///  generalizes Reingold and Tilford's method for tidy drawings of trees to
    ///  support trees with an arbitrary number of children at any given node.</para>
    /// </summary>
    class StandardTreeLayout : TreeLayoutBase
    {

        

        #region Fields
        BackgroundWorker worker;
        private static TreeOrientation mOrientation = TreeOrientation.TopBottom;  // the orientation of the tree
        private static float mBreadth = 50F;   // the spacing between sibling nodes
        private static float mSubTreeSpacing = 50F;  // the spacing between subtrees
        private static float mDepth = 50F;  // the spacing between depth levels
        private float m_offset = 50;  // pixel offset for root node position
        private PointF m_anchor;
        private PointF m_tmpa;
        private float[] m_depths = new float[10];
        private int mMaxDepth = 0;
        private Dictionary<string, Params> Pars;
        private float m_ax, m_ay; // for holding anchor co-ordinates
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the orientation of the tree.
        /// </summary>
        /// <value>The orientation.</value>
        public static TreeOrientation Orientation
        {
            get
            {
                return mOrientation;
            }
            set
            {
                mOrientation = value;
            }
        }
        /// <summary>
        /// Gets or sets the depth spacing.
        /// </summary>
        /// <value>The depth spacing.</value>
        public static float DepthSpacing
        {
            get { return mDepth; }
            set { mDepth = value; }
        }

        /// <summary>
        /// Gets or sets the breadth spacing.
        /// </summary>
        /// <value>The breadth spacing.</value>
        public static float BreadthSpacing
        {
            get { return mBreadth; }
            set { mBreadth = value; }
        }
        /// <summary>
        /// Gets or sets the spacing between subtrees.
        /// </summary>
        /// <value>The sub tree spacing.</value>
        public static float SubTreeSpacing
        {
            get { return mSubTreeSpacing; }
            set { mSubTreeSpacing = value; }

        }

        public float RootOffset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }

        #endregion

        #region Constructor
        public StandardTreeLayout(IController controller)
            : base("Standard TreeLayout", controller)
        {



        }
        #endregion

        #region Methods

        public override void Run()
        {
            Run(2000);
        }
        public override void Run(int time)
        {
            if (worker != null && worker.IsBusy)
                worker.CancelAsync();                
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
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
        /// <summary>
        /// Runs a single step.
        /// </summary>
        private void RunStep()
        {

        }
        private bool Init()
        {

            this.Graph = this.Model as IGraph;

            if (Graph == null)
                throw new InconsistencyException("The model has not been set and the Graph property is hence 'null'");

            this.LayoutRoot = this.Controller.Model.LayoutRoot;//could be null if not set in the GUI
            Graph.ClearSpanningTree();
            Graph.MakeSpanningTree(LayoutRoot as INode);

            Trace.WriteLine((LayoutRoot as INode).ChildCount);

            if (Graph.SpanningTree == null)
                throw new InconsistencyException("The spanning tree is not set (se the root of the tree layout first)");

            
            


            Pars = new Dictionary<string, Params>();
            if (Graph.Nodes.Count == 0)
                return false;
            if (Graph.Edges.Count == 0) //this layout is base on embedded springs in the connections
                return false;


            Params par;

            foreach (INode node in Graph.Nodes)
            {
                par = new Params();
                par.init(node);
                Pars.Add(node.Uid.ToString(), par);
            }
            return true;
        }
        public PointF LayoutAnchor
        {
            get
            {
                if (m_anchor != null)
                {
                    return m_anchor;
                }

                m_tmpa = PointF.Empty;
                if (Graph != null)
                {
                    Rectangle b = Bounds;

                    switch (mOrientation)
                    {
                        case TreeOrientation.LeftRight:
                            m_tmpa = new PointF(m_offset, b.Height / 2F);
                            break;
                        case TreeOrientation.RightLeft:
                            m_tmpa = new PointF(b.Width - m_offset, b.Height / 2F);
                            break;
                        case TreeOrientation.TopBottom:
                            m_tmpa = new PointF(b.Width / 2F, m_offset);
                            break;
                        case TreeOrientation.BottomTop:
                            m_tmpa = new PointF(b.Width / 2F, b.Height - m_offset);
                            break;
                        case TreeOrientation.Center:
                            m_tmpa = new PointF(b.Width / 2F, b.Height / 2F);
                            break;
                        default:
                            break;
                    }
                    //TODO: set the center of the layout here on the view
                    //d.getInverseTransform().transform(m_tmpa, m_tmpa);
                }
                return m_tmpa;
            }
            set
            {
                m_anchor = value;
            }
        }

        /// <summary>
        /// Returns the location of halfway the two given nodes
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="siblings"></param>
        /// <returns></returns>
        private double spacing(INode l, INode r, bool siblings)
        {
            bool w = (mOrientation == TreeOrientation.TopBottom || mOrientation == TreeOrientation.BottomTop);
            return (siblings ? mBreadth : mSubTreeSpacing) + 0.5 * (w ? l.Rectangle.Width + r.Rectangle.Width : l.Rectangle.Height + r.Rectangle.Height);
        }
        private void updateDepths(int depth, INode item)
        {
            bool v = (mOrientation == TreeOrientation.TopBottom || mOrientation == TreeOrientation.BottomTop);
            double d = (v ? item.Rectangle.Height : item.Rectangle.Width);
            if (m_depths.Length <= depth)
                Array.Resize<float>(ref m_depths, (int)(3 * depth / 2));
            m_depths[depth] = (float)Math.Max(m_depths[depth], d);
            mMaxDepth = (int)Math.Max(mMaxDepth, depth);
        }

        private void determineDepths()
        {
            for (int i = 1; i < mMaxDepth; ++i)
                m_depths[i] += m_depths[i - 1] + mDepth;
        }

        public void Layout()
        {
            m_depths.Initialize();

            mMaxDepth = 0;

            PointF a = LayoutAnchor;
            m_ax = a.X;
            m_ay = a.Y;

            INode root = LayoutRoot as INode;
            Params rp = Pars[root.Uid.ToString()];

            // do first pass - compute breadth information, collect depth info
            firstWalk(root, 0, 1);

            // sum up the depth info
            determineDepths();

            // do second pass - assign layout positions
            secondWalk(root, null, -rp.prelim, 0);
        }

        private void firstWalk(INode n, int num, int depth)
        {
            Trace.WriteLine("depthj: " + depth);
            Params np = Pars[n.Uid.ToString()];

            np.number = num;
            updateDepths(depth, n);

            bool expanded = n.IsExpanded;
            if (n.ChildCount == 0 || !expanded) // is leaf
            {
                INode l = (INode)n.PreviousSibling;
                if (l == null)
                {
                    np.prelim = 0;
                }
                else
                {
                    np.prelim = Pars[l.Uid.ToString()].prelim + spacing(l, n, true);
                }
            }
            else if (expanded)
            {
                INode leftMost = n.FirstChild;
                INode rightMost = n.LastChild;
                INode defaultAncestor = leftMost;
                INode c = leftMost;
                for (int i = 0; c != null; ++i, c = c.NextSibling)
                {
                    firstWalk(c, i, depth + 1);
                    defaultAncestor = apportion(c, defaultAncestor);
                }

                executeShifts(n);

                double midpoint = 0.5 *
                    (Pars[leftMost.Uid.ToString()].prelim + Pars[rightMost.Uid.ToString()].prelim);

                INode left = (INode)n.PreviousSibling;
                if (left != null)
                {
                    np.prelim = Pars[left.Uid.ToString()].prelim + spacing(left, n, true);
                    np.mod = np.prelim - midpoint;
                }
                else
                {
                    np.prelim = midpoint;
                }
            }
        }
        private INode apportion(INode v, INode a)
        {
            INode w = (INode)v.PreviousSibling;
            if (w != null)
            {
                INode vip, vim, vop, vom;
                double sip, sim, sop, som;

                vip = vop = v;
                vim = w;
                vom = (INode)vip.ParentNode.FirstChild;

                sip = Pars[vip.Uid.ToString()].mod;
                sop = Pars[vop.Uid.ToString()].mod;
                sim = Pars[vim.Uid.ToString()].mod;
                som = Pars[vom.Uid.ToString()].mod;
                Params parms;
                INode nr = nextRight(vim);
                INode nl = nextLeft(vip);
                while (nr != null && nl != null)
                {
                    vim = nr;
                    vip = nl;
                    vom = nextLeft(vom);
                    vop = nextRight(vop);
                    parms = Pars[vop.Uid.ToString()];
                    parms.ancestor = v;
                    double shift = (Pars[vim.Uid.ToString()].prelim + sim) -
                        (Pars[vip.Uid.ToString()].prelim + sip) + spacing(vim, vip, false);
                    if (shift > 0)
                    {
                        moveSubtree(ancestor(vim, v, a), v, shift);
                        sip += shift;
                        sop += shift;
                    }
                    sim += Pars[vim.Uid.ToString()].mod;
                    sip += Pars[vip.Uid.ToString()].mod;
                    som += Pars[vom.Uid.ToString()].mod;
                    sop += Pars[vop.Uid.ToString()].mod;

                    nr = nextRight(vim);
                    nl = nextLeft(vip);
                }
                if (nr != null && nextRight(vop) == null)
                {
                    Params vopp = Pars[vop.Uid.ToString()];
                    vopp.thread = nr;
                    vopp.mod += sim - sop;
                }
                if (nl != null && nextLeft(vom) == null)
                {
                    Params vomp = Pars[vom.Uid.ToString()];
                    vomp.thread = nl;
                    vomp.mod += sip - som;
                    a = v;
                }
            }
            return a;
        }
        private INode nextLeft(INode n)
        {
            INode c = null;
            if (n.IsExpanded) c = n.FirstChild;
            return (c != null ? c : Pars[n.Uid.ToString()].thread);
        }

        private INode nextRight(INode n)
        {
            INode c = null;
            if (n.IsExpanded) c = n.LastChild;
            return (c != null ? c : Pars[n.Uid.ToString()].thread);
        }

        private void moveSubtree(INode wm, INode wp, double shift)
        {
            Params wmp = Pars[wm.Uid.ToString()];
            Params wpp = Pars[wp.Uid.ToString()];
            double subtrees = wpp.number - wmp.number;
            wpp.change -= shift / subtrees;
            wpp.shift += shift;
            wmp.change += shift / subtrees;
            wpp.prelim += shift;
            wpp.mod += shift;
        }

        private void executeShifts(INode n)
        {
            double shift = 0, change = 0;
            for (INode c = n.LastChild; c != null; c = c.PreviousSibling)
            {
                Params cp = Pars[c.Uid.ToString()];
                cp.prelim += shift;
                cp.mod += shift;
                change += cp.change;
                shift += cp.shift + change;
            }
        }

        private INode ancestor(INode vim, INode v, INode a)
        {
            INode p = (INode)v.ParentNode;
            Params vimp = Pars[vim.Uid.ToString()];
            if (vimp.ancestor != null && vimp.ancestor.ParentNode == p)
            {
                return vimp.ancestor;
            }
            else
            {
                return a;
            }
        }

        private void secondWalk(INode n, INode p, double m, int depth)
        {
            Params np = Pars[n.Uid.ToString()];
            setBreadth(n, p, np.prelim + m);
            setDepth(n, p, m_depths[depth]);

            if (n.IsExpanded)
            {
                depth += 1;
                for (INode c = n.FirstChild; c != null; c = c.NextSibling)
                {
                    if (worker.CancellationPending)
                        break;
                    secondWalk(c, n, m + np.mod, depth);
                }
            }

            np.clear();
        }

        private void setBreadth(INode n, INode p, double b)
        {
            switch (mOrientation)
            {
                case TreeOrientation.LeftRight:
                case TreeOrientation.RightLeft:
                    setY(n, p, m_ay + b);
                    break;
                case TreeOrientation.TopBottom:
                case TreeOrientation.BottomTop:
                    setX(n, p, m_ax + b);
                    break;
                default:
                    throw new InconsistencyException();
            }
        }

        private void setDepth(INode n, INode p, double d)
        {
            switch (mOrientation)
            {
                case TreeOrientation.LeftRight:
                    setX(n, p, m_ax + d);
                    break;
                case TreeOrientation.RightLeft:
                    setX(n, p, m_ax - d);
                    break;
                case TreeOrientation.TopBottom:
                    setY(n, p, m_ay + d);
                    break;
                case TreeOrientation.BottomTop:
                    setY(n, p, m_ay - d);
                    break;
                default:
                    throw new InconsistencyException();
            }
        }


        #endregion

        /// <summary>
        /// Paramter blob to temporarily keep working data of one node.
        /// </summary>
        class Params
        {
            public double prelim;
            public double mod;
            public double shift;
            public double change;
            public int number;
            public INode ancestor;
            public INode thread;

            public void init(INode item)
            {
                ancestor = item;
                number = -1;
                ancestor = thread = null;
            }

            public void clear()
            {
                number = -2;
                prelim = mod = shift = change = 0;
                ancestor = thread = null;
            }
        }
    }

    public enum TreeOrientation
    {
        LeftRight,
        RightLeft,
        TopBottom,
        BottomTop,
        Center
    }
}
