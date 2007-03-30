using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// <para>Layout instance implementing the Fruchterman-Reingold algorithm for
    ///  force-directed placement of graph nodes. The computational complexity of
    ///  this algorithm is quadratic [O(n^2)] in the number of nodes, so should
    ///  only be applied for relatively small graphs, particularly in interactive
    ///  situations.</para>
    ///  
    ///  <para>This implementation was ported from the implementation in the
    ///  <a href="http://jung.sourceforge.net/">JUNG</a> framework.</para>
    /// </summary>
    class FruchtermanReingoldLayout : LayoutBase
    {
        #region Fields
        private double forceConstant;
        private double temp;
        private int maxIter = 300;

        protected int m_fidx;
        private Random rnd;
        private static double EPSILON = 0.000001D;
        private static double ALPHA = 0.1D;

        Dictionary<string, Params> Pars;
        double width;
        double height;
        BackgroundWorker worker;
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public FruchtermanReingoldLayout(IController controller)
            : base("FruchtermanReingold Layout", controller)
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Inits the calculational state
        /// </summary>
        private bool Init()
        {
            this.Graph = this.Model as IGraph;

            if (Graph == null)
                throw new InconsistencyException("The model has not been set and the Graph property is hence 'null'");

            //this.LayoutRoot = this.Controller.Model.LayoutRoot;//could be null if not set in the GUI
            //Graph.ClearSpanningTree();
            //Graph.MakeSpanningTree(LayoutRoot as INode);

            Pars = new Dictionary<string, Params>();
            if (Nodes.Count == 0)
                return false;
            if (Edges.Count == 0) //this layout is base on embedded springs in the connections
                return false;

            width = (double)Bounds.Width;
            height = (double)Bounds.Height;
            rnd = new Random();


            temp = width / 10;
            forceConstant = 0.75 * Math.Sqrt(height * width / Nodes.Count);

            // initialize node positions


            double scaleW = ALPHA * width / 2;
            double scaleH = ALPHA * height / 2;
            Params par;
            double[] loc;
            foreach (INode node in Nodes)
            {
                loc = new double[2];
                loc[0] = Center.X + rnd.NextDouble() * scaleW;
                loc[1] = Center.Y + rnd.NextDouble() * scaleH;
                par = new Params(loc, new double[2]);
                Pars.Add(node.Uid.ToString(), par);
            }
            return true;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            width = this.Bounds.Width;
            height = this.Bounds.Height;
            Run(1000);


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
        /// Runs the layout for a specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        public override void Run(int time)
        {
            if (Init())
            {

                worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerAsync(time);

            }

        }

        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.Controller.View.Suspend();
            DateTime start = DateTime.Now;
            while (DateTime.Now < start.AddMilliseconds((int)e.Argument))
            {
                RunStep();

            }
            //this.Controller.View.Resume();
            UpdateShapes();

            RunAnimation();
        }

        private void RunAnimation()
        {

        }

        /// <summary>
        /// Runs a single step.
        /// </summary>
        private void RunStep()
        {

            for (int curIter = 0; curIter < maxIter; curIter++)
            {

                // Calculate repulsion
                foreach (INode node in Nodes)
                {
                    if (node.IsFixed) continue;
                    CalculateRepulsion(node);
                }

                // Calculate attraction
                foreach (IEdge edge in Edges)
                {
                    CalculateAttraction(edge);
                }

                foreach (INode node in Nodes)
                {
                    if (node.IsFixed) continue;
                    CalculatePositions(node);
                }

                CoolDown(curIter);


            }


        }

        private void UpdateShapes()
        {
            int x, y;
            lock (Nodes)
                lock (Pars)
                {
                    foreach (INode node in Nodes)
                    {
                        x = Convert.ToInt32(Pars[node.Uid.ToString()].loc[0]) - node.Rectangle.X;
                        y = Convert.ToInt32(Pars[node.Uid.ToString()].loc[1]) - node.Rectangle.Y;
                        //if (node.Rectangle.X + x < width - node.Rectangle.Width - 10 && node.Rectangle.X + x > 10 && node.Rectangle.Y + y < height - node.Rectangle.Height - 10 && node.Rectangle.Y + y > 10)
                        node.Move(new Point(x, y));
                    }
                }
        }

        #region Calculations
        public void CalculatePositions(INode n)
        {
            Params np = Pars[n.Uid.ToString()];


            double deltaLength = Math.Max(EPSILON, Math.Sqrt(np.disp[0] * np.disp[0] + np.disp[1] * np.disp[1]));

            double xDisp = np.disp[0] / deltaLength * Math.Min(deltaLength, temp);

            if (Double.IsNaN(xDisp))
            {
                System.Diagnostics.Trace.WriteLine("Oops, the layout resulted in a NaN problem.");
                return;
            }

            double yDisp = np.disp[1] / deltaLength * Math.Min(deltaLength, temp);

            np.loc[0] += xDisp;
            np.loc[1] += yDisp;


            // don't let nodes leave the display
            double borderWidth = width / 50.0;
            double x = np.loc[0];
            if (x < borderWidth)
            {
                x = borderWidth + rnd.NextDouble() * borderWidth * 2.0;
            }
            else if (x > (width - borderWidth))
            {
                x = width - borderWidth - rnd.NextDouble() * borderWidth * 2.0;
            }

            double y = np.loc[1];
            if (y < borderWidth)
            {
                y = borderWidth + rnd.NextDouble() * borderWidth * 2.0;
            }
            else if (y > (height - borderWidth))
            {
                y = height - borderWidth - rnd.NextDouble() * borderWidth * 2.0;
            }

            np.loc[0] = x;
            np.loc[1] = y;
        }

        /// <summary>
        /// Calculates the attraction or tension on the given edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public void CalculateAttraction(IEdge edge)
        {
            INode n1, n2;
            Params n1p = new Params();
            Params n2p = new Params();
            if (edge.SourceNode != null)
            {
                n2 = edge.SourceNode;
                n2p = Pars[n2.Uid.ToString()];
            };
            if (edge.TargetNode != null)
            {
                n1 = edge.TargetNode;
                n1p = Pars[n1.Uid.ToString()];
            };



            double xDelta = n1p.loc[0] - n2p.loc[0];
            double yDelta = n1p.loc[1] - n2p.loc[1];

            double deltaLength = Math.Max(EPSILON, Math.Sqrt(xDelta * xDelta + yDelta * yDelta));
            double force = (deltaLength * deltaLength) / forceConstant;

            if (Double.IsNaN(force))
            {
                System.Diagnostics.Trace.WriteLine("Oops, the layout resulted in a NaN problem.");
                return;
            }

            double xDisp = (xDelta / deltaLength) * force;
            double yDisp = (yDelta / deltaLength) * force;

            n1p.disp[0] -= xDisp; n1p.disp[1] -= yDisp;
            n2p.disp[0] += xDisp; n2p.disp[1] += yDisp;
        }

        public void CalculateRepulsion(INode node)
        {
            Params np = Pars[node.Uid.ToString()];
            np.disp[0] = 0.0; np.disp[1] = 0.0;

            foreach (INode n2 in Nodes)
            {

                Params n2p = Pars[n2.Uid.ToString()];
                if (n2.IsFixed) continue;
                if (node != n2)
                {
                    double xDelta = np.loc[0] - n2p.loc[0];
                    double yDelta = np.loc[1] - n2p.loc[1];

                    double deltaLength = Math.Max(EPSILON, Math.Sqrt(xDelta * xDelta + yDelta * yDelta));

                    double force = (forceConstant * forceConstant) / deltaLength;

                    if (Double.IsNaN(force))
                    {
                        System.Diagnostics.Trace.WriteLine("Oops, the layout resulted in a NaN problem.");
                        return;
                    }

                    np.disp[0] += (xDelta / deltaLength) * force;
                    np.disp[1] += (yDelta / deltaLength) * force;
                }
            }
        }

        private void CoolDown(int curIter)
        {
            temp *= (1.0 - curIter / (double)maxIter);
        }
        #endregion


        /// <summary>
        /// Paramter blob to temporarily keep working data of one node.
        /// </summary>
        struct Params
        {
            public double[] loc;
            public double[] disp;

            public Params(double[] loc, double[] disp)
            {
                this.loc = loc;
                this.disp = disp;
            }


        }


        #endregion

    }
}
