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
    /// <para>Layout that computes a circular "balloon-tree" layout of a tree.
    /// This layout places children nodes radially around their parents, and is
    ///equivalent to a top-down flattened view of a ConeTree.</para>
    /// 
    /// <para>The algorithm used is that of G. Melançon and I. Herman from their
    /// research paper Circular Drawings of Rooted Trees, Reports of the Centre for 
    /// Mathematics and Computer Sciences, Report Number INS–9817, 1998.</para>
    /// </summary>
    class BalloonTreeLayout : TreeLayoutBase
    {
        #region Fields

        private int m_minRadius = 2;
        private Dictionary<string, Params> Pars;
        BackgroundWorker worker;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the MinRadius
        /// </summary>
        public int MinRadius
        {
            get { return m_minRadius; }
            set { m_minRadius = value; }
        }

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public BalloonTreeLayout(IController controller)
            : base("Balloon TreeLayout", controller)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Layouts this instance.
        /// </summary>
        public void Layout()
        {
            FirstWalk(LayoutRoot as INode);
            SecondWalk(LayoutRoot as INode, null, LayoutRoot.Rectangle.X, LayoutRoot.Rectangle.Y, 1, 0);
        }
        /// <summary>
        /// Inits this instance.
        /// </summary>
        /// <returns></returns>
        private bool Init()
        {


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

        /// <summary>
        /// First traversal.
        /// </summary>
        /// <param name="n">The n.</param>
        private void FirstWalk(INode n)
        {
            Params np = Pars[n.Uid.ToString()];
            np.d = 0;
            double s = 0;
            if (n.Children != null)
            {
                foreach (INode c in n.Children)
                {
                    //if (!c.isVisible()) continue;
                    FirstWalk(c);
                    Params cp = Pars[c.Uid.ToString()];
                    np.d = Math.Max(np.d, cp.r);
                    cp.a = Math.Atan(((double)cp.r) / (np.d + cp.r));
                    s += cp.a;
                }
            }
            AdjustChildren(np, s);
            SetRadius(np);
        }
        /// <summary>
        /// Second traversal.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="r">The r.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="l">The l.</param>
        /// <param name="t">The t.</param>
        private void SecondWalk(INode n, INode r, double x, double y, double l, double t)
        {
            setX(n, r, x);
            setY(n, r, y);

            Params np = Pars[n.Uid.ToString()];

            double dd = l * np.d;
            double p = t + Math.PI;
            double fs = (n.ChildCount == 0 ? 0 : np.f / n.ChildCount);
            double pr = 0;
            if (n.Children != null)
            {
                foreach (INode c in n.Children)
                {

                    //if (!c.isVisible()) continue;
                    Params cp = Pars[c.Uid.ToString()];
                    double aa = np.c * cp.a;
                    double rr = np.d * Math.Tan(aa) / (1 - Math.Tan(aa));
                    p += pr + aa + fs;
                    double xx = (l * rr + dd) * Math.Cos(p);
                    double yy = (l * rr + dd) * Math.Sin(p);
                    pr = aa;
                    SecondWalk(c, n, x + xx, y + yy, l * np.c/*l*rr/cp.r*/, p);
                }
            }
        }
        /// <summary>
        /// Sets the radius.
        /// </summary>
        /// <param name="np">The np.</param>
        private void SetRadius(Params np)
        {
            np.r = Convert.ToInt32((Math.Max(np.d, m_minRadius) + 2 * np.d) / 2.1D);
        }
        /// <summary>
        /// Adjusts the children.
        /// </summary>
        /// <param name="np">The np.</param>
        /// <param name="s">The s.</param>
        private void AdjustChildren(Params np, double s)
        {
            if (s > Math.PI)
            {
                np.c = Math.PI / s;
                np.f = 0;
            }
            else
            {
                np.c = 1;
                np.f = Math.PI - s;
            }
        }
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            Run(2000);
        }
        /// <summary>
        /// Runs the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        public override void Run(int time)
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync(time);
        }
        /// <summary>
        /// Stops this instance.
        /// </summary>
        public override void Stop()
        {
            if (worker != null && worker.IsBusy)
                worker.CancelAsync();
        }
        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Init();
            Layout();
        }


        #endregion

        #region Classes

        /// <summary>
        /// Paramter blob to temporarily keep working data of one node.
        /// </summary>
        class Params
        {
            public int d;
            public int r;
            public double rx;
            public double ry;
            public double a;
            public double c;
            public double f;
        }

        #endregion
    }
}
