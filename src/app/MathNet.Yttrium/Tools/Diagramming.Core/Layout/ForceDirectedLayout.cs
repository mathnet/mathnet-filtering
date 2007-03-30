using System;
using System.Diagnostics;
using Netron.Diagramming.Core.Analysis;
using Netron.Diagramming.Core.Layout.Force;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// <para>Layout that positions graph elements based on a physics simulation of
    ///  interacting forces; by default, nodes repel each other, edges act as
    ///  springs, and drag forces (similar to air resistance) are applied. This
    ///  algorithm can be run for multiple iterations for a run-once layout
    ///  computation or repeatedly run in an animated fashion for a dynamic and
    ///  interactive layout.</para>
    ///  
    ///  <para>The running time of this layout algorithm is the greater of O(N log N)
    ///  and O(E), where N is the number of nodes and E the number of edges.
    ///  The addition of custom force calculation modules may, however, increase
    ///  this value.</para>
    ///  
    ///  <para>The <see cref="ForceSimulator"/> used to drive this layout
    ///  can be set explicitly, allowing any number of custom force directed layouts
    ///  to be created through the user's selection of included
    ///  <see cref="Force"/> components. Each node in the layout is
    ///  mapped to a <see cref="ForceItem"/> instance and each edge
    ///  to a <see cref="Spring"/> instance for storing the state
    ///  of the simulation. See the <see cref="Force"/> namespace for more.</para>
    /// </summary>
    class ForceDirectedLayout : LayoutBase
    {
        #region Fields
        private ForceSimulator m_fsim;
        private long m_lasttime = -1L;
        private long m_maxstep = 50L;
        private bool m_runonce;
        private int m_iterations = 100;
        private bool mEnforceBounds;
        BackgroundWorker worker;
        protected INode referrer;

        protected String m_nodeGroup;
        protected String m_edgeGroup;
        private Dictionary<string, ForceItem> Pars;
        #endregion

        #region Properties
        public long MaxTimeStep
        {
            get { return m_maxstep; }
            set { m_maxstep = value; }
        }

        public ForceSimulator getForceSimulator
        {
            get { return m_fsim; }
            set { m_fsim = value; }
        }

        public int Iterations
        {
            get { return m_iterations; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("The amount of iterations has to be bigger or equal to one.");
                m_iterations = value;
            }
        }



        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ForceDirectedLayout(IController controller)
            : base("ForceDirected Layout", controller)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Controller.View.Suspend();
            Init();
            Layout();
            this.Controller.View.Resume();
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
        ///<summary>
        /// Get the mass value associated with the given node. Subclasses should
        /// override this method to perform custom mass assignment.
        /// @param n the node for which to compute the mass value
        /// @return the mass value for the node. By default, all items are given
        /// a mass value of 1.0.
        ///</summary>
        protected float getMassValue(INode n)
        {
            return 1.0f;
        }

        ///<summary>
        /// Get the spring length for the given edge. Subclasses should
        /// override this method to perform custom spring length assignment.
        /// @param e the edge for which to compute the spring length
        /// @return the spring length for the edge. A return value of
        /// -1 means to ignore this method and use the global default.
        ///</summary>
        protected float getSpringLength(IEdge e)
        {
            return -1.0F;
        }

        ///<summary>
        /// Get the spring coefficient for the given edge, which controls the
        /// tension or strength of the spring. Subclasses should
        /// override this method to perform custom spring tension assignment.
        /// @param e the edge for which to compute the spring coefficient.
        /// @return the spring coefficient for the edge. A return value of
        /// -1 means to ignore this method and use the global default.
        ///</summary>
        protected float getSpringCoefficient(IEdge e)
        {
            return -1.0F;
        }

        private bool Init()
        {

            mEnforceBounds = false;
            m_runonce = true;
            m_fsim = new ForceSimulator();

            m_fsim.AddForce(new NBodyForce());
            m_fsim.AddForce(new SpringForce());
            m_fsim.AddForce(new DragForce());

            this.Graph = this.Model as IGraph;

            if (Graph == null)
                throw new InconsistencyException("The model has not been set and the Graph property is hence 'null'");

            //Graph.ClearSpanningTree();
            //Graph.MakeSpanningTree(LayoutRoot as INode);


            if (Graph.Nodes.Count == 0)
                return false;
            if (Graph.Edges.Count == 0) //this layout is base on embedded springs in the connections
                return false;

            Pars = new Dictionary<string, ForceItem>();

            foreach (INode node in Nodes)
            {
                Pars.Add(node.Uid.ToString(), new ForceItem());
            }
            return true;
        }

        /// <summary>
        /// Updates the node positions.
        /// </summary>
        private void UpdateNodePositions()
        {
            double x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            if (Bounds != null)
            {
                x1 = Bounds.X; y1 = Bounds.Top;
                x2 = Bounds.Right; y2 = Bounds.Bottom;
            }

            // update positions
            foreach (INode item in Nodes)
            {

                ForceItem fitem = Pars[item.Uid.ToString()];
                if (item.IsFixed)
                {
                    // clear any force computations
                    fitem.Force[0] = 0.0f;
                    fitem.Force[1] = 0.0f;
                    fitem.Velocity[0] = 0.0f;
                    fitem.Velocity[1] = 0.0f;

                    if (Double.IsNaN(item.X))
                    {
                        setX(item, referrer, 0.0D);
                        setY(item, referrer, 0.0D);
                    }
                    continue;
                }

                double x = fitem.Location[0];
                double y = fitem.Location[1];
                //do we need to check the bounding constraints
                if (mEnforceBounds && Bounds != null)
                {

                    double hw = item.Rectangle.Width / 2;
                    double hh = item.Rectangle.Height / 2;
                    if (x + hw > x2) x = x2 - hw;
                    if (x - hw < x1) x = x1 + hw;
                    if (y + hh > y2) y = y2 - hh;
                    if (y - hh < y1) y = y1 + hh;
                }

                // set the actual position
                setX(item, referrer, x);
                setY(item, referrer, y);
            }
        }

        ///<summary>
        /// Reset the force simulation state for all nodes processed by this layout.
        ///</summary>
        public void Reset()
        {
            foreach (INode item in Nodes)
            {
                ForceItem fitem = Pars[item.Uid.ToString()];
                if (fitem != null)
                {
                    fitem.Location[0] = (float)item.X;
                    fitem.Location[1] = (float)item.Y;
                    fitem.Force[0] = fitem.Force[1] = 0;
                    fitem.Velocity[0] = fitem.Velocity[1] = 0;
                }
            }
            m_lasttime = -1L;
        }

        /// <summary>
        /// Loads the simulator with all relevant force items and springs.
        /// </summary>
        /// <param name="fsim"> the force simulator driving this layout.</param>
        protected void InitializeSimulator(ForceSimulator fsim)
        {
            //TODO: some checks here...?

            float startX = (referrer == null ? 0f : (float)referrer.X);
            float startY = (referrer == null ? 0f : (float)referrer.Y);
            startX = float.IsNaN(startX) ? 0f : startX;
            startY = float.IsNaN(startY) ? 0f : startY;
            if (Nodes != null && Nodes.Count > 0)
            {
                foreach (INode item in Nodes)
                {
                    ForceItem fitem = Pars[item.Uid.ToString()];
                    fitem.Mass = getMassValue(item);
                    double x = item.X;
                    double y = item.Y;
                    fitem.Location[0] = (Double.IsNaN(x) ? startX : (float)x);
                    fitem.Location[1] = (Double.IsNaN(y) ? startY : (float)y);
                    fsim.addItem(fitem);
                }
            }
            if (Edges != null && Edges.Count > 0)
            {
                foreach (IEdge e in Edges)
                {
                    INode n1 = e.SourceNode;
                    if (n1 == null) continue;
                    ForceItem f1 = Pars[n1.Uid.ToString()];
                    INode n2 = e.TargetNode;
                    if (n2 == null) continue;
                    ForceItem f2 = Pars[n2.Uid.ToString()];
                    float coeff = getSpringCoefficient(e);
                    float slen = getSpringLength(e);
                    fsim.addSpring(f1, f2, (coeff >= 0 ? coeff : -1.0F), (slen >= 0 ? slen : -1.0F));
                }
            }
        }
        private void Layout()
        {
            // perform different actions if this is a run-once or
            // run-continuously layout
            if (m_runonce)
            {
                PointF anchor = new PointF(Bounds.Width / 2F, Bounds.Height / 2F);
                foreach (INode node in Nodes)
                {
                    setX(node, null, anchor.X);
                    setY(node, null, anchor.Y);
                }
                m_fsim.Clear();
                long timestep = 1000L;

                InitializeSimulator(m_fsim);

                for (int i = 0; i < m_iterations; i++)
                {
                    // use an annealing schedule to set time step
                    timestep *= Convert.ToInt64(1.0 - i / (double)m_iterations);
                    long step = timestep + 50;
                    // run simulator
                    m_fsim.RunSimulator(step);
                    // debugging output
                    //if (i % 10 == 0 ) {Trace.WriteLine("iter: "+i);}
                }
                UpdateNodePositions();
            }
            else
            {
                // get timestep
                if (m_lasttime == -1)
                    m_lasttime = DateTime.Now.Ticks * 10 - 20;
                long time = DateTime.Now.Ticks * 10;//how many milliseconds since the human race started to count things
                long timestep = Math.Min(m_maxstep, time - m_lasttime);
                m_lasttime = time;

                // run force simulator
                m_fsim.Clear();
                InitializeSimulator(m_fsim);
                m_fsim.RunSimulator(timestep);
                UpdateNodePositions();
            }
            //if ( frac == 1.0 ) {
            //    reset();
            //}
        }
        #endregion

    }
}
