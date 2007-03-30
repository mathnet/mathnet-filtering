using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    /// <para>
    /// Force function which computes an n-body force such as gravity,
    /// anti-gravity, or the results of electric charges. This function implements
    /// the the Barnes-Hut algorithm for efficient n-body force simulations,
    /// using a quad-tree with aggregated mass values to compute the n-body
    /// force in O(N log N) time, where N is the number of ForceItems.</para>
    /// 
    /// <para>The algorithm used is that of J. Barnes and P. Hut, in their research
    /// paper <i>A Hierarchical  O(n log n) force calculation algorithm</i>, Nature, 
    ///  v.324, December 1986. For more details on the algorithm, see one of
    ///  the following links --
    /// <list type="bullet">
    /// 
    ///   <item><a href="http://www.cs.berkeley.edu/~demmel/cs267/lecture26/lecture26.html">James Demmel's UC Berkeley lecture notes</a></item>
    ///   <item><a href="http://www.physics.gmu.edu/~large/lr_forces/desc/bh/bhdesc.html">Description of the Barnes-Hut algorithm</a></item>
    ///   <item><a href="http://www.ifa.hawaii.edu/~barnes/treecode/treeguide.html">Joshua Barnes' recent implementation</a></item>
    /// </list></para>
    /// </summary>
    public class NBodyForce : AbstractForce
    {

        /* 
          The indexing scheme for quadtree child nodes goes row by row.
            0 | 1    0 -> top left,    1 -> top right
           -------
            2 | 3    2 -> bottom left, 3 -> bottom right
         */

        #region Fields
        private static String[] pnames = new String[] { "GravitationalConstant", 
            "Distance", "BarnesHutTheta"  };

        public static float DEFAULT_GRAV_CONSTANT = -1.0f;
        public static float DEFAULT_MIN_GRAV_CONSTANT = -10f;
        public static float DEFAULT_MAX_GRAV_CONSTANT = 10f;

        public static float DEFAULT_DISTANCE = -1f;
        public static float DEFAULT_MIN_DISTANCE = -1f;
        public static float DEFAULT_MAX_DISTANCE = 500f;

        public static float DEFAULT_THETA = 0.9f;
        public static float DEFAULT_MIN_THETA = 0.0f;
        public static float DEFAULT_MAX_THETA = 1.0f;

        public static int GRAVITATIONAL_CONST = 0;
        public static int MIN_DISTANCE = 1;
        public static int BARNES_HUT_THETA = 2;

        private float xMin, xMax, yMin, yMax;
        private QuadTreeNodeFactory factory = new QuadTreeNodeFactory();
        private QuadTreeNode root;

        private Random rand = new Random(12345678); // deterministic randomness

        #endregion

        #region Properties

        /**
         * Gets whether this is a force item.
         */
        public override bool IsItemForce
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        protected override String[] ParameterNames
        {
            get
            {
                return pnames;
            }
        }
        #endregion

        #region Constructor



        /// <summary>
        /// Create a new NBodyForce with default parameters.
        /// </summary>
        public NBodyForce()
            : this(DEFAULT_GRAV_CONSTANT, DEFAULT_DISTANCE, DEFAULT_THETA)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a new NBodyForce.
        /// </summary>
        /// <param name="gravConstant">the gravitational constant to use. Nodes will attract each other if this value is positive, and will repel each other if it is negative.</param>
        /// <param name="minDistance">the distance within which two particles will  interact. If -1, the value is treated as infinite.</param>
        /// <param name="theta">the Barnes-Hut parameter theta, which controls when an aggregated mass is used rather than drilling down to individual  item mass values.</param>
        public NBodyForce(float gravConstant, float minDistance, float theta)
        {
            parms = new float[] { gravConstant, minDistance, theta };
            minValues = new float[] { DEFAULT_MIN_GRAV_CONSTANT,
            DEFAULT_MIN_DISTANCE, DEFAULT_MIN_THETA };
            maxValues = new float[] { DEFAULT_MAX_GRAV_CONSTANT,
            DEFAULT_MAX_DISTANCE, DEFAULT_MAX_THETA };
            root = factory.GetQuadTreeNode();
        }




        /// <summary>
        /// Set the bounds of the region for which to compute the n-body simulation
        /// </summary>
        /// <param name="xMin">the minimum x-coordinate</param>
        /// <param name="yMin">the minimum y-coordinate/param>
        /// <param name="xMax"> the maximum x-coordinate</param>
        /// <param name="yMax">the maximum y-coordinate</param>
        private void setBounds(float xMin, float yMin, float xMax, float yMax)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;
        }


        /// <summary>
        ///Clears the quadtree of all entries.
        /// </summary>
        public void clear()
        {
            ClearHelper(root);
            root = factory.GetQuadTreeNode();
        }

        /// <summary>
        /// Clearing aid
        /// </summary>
        /// <param name="n">The n.</param>
        private void ClearHelper(QuadTreeNode n)
        {
            for (int i = 0; i < n.children.Length; i++)
            {
                if (n.children[i] != null)
                    ClearHelper(n.children[i]);
            }
            factory.Reclaim(n);
        }

        /// <summary>
        /// Initialize the simulation with the provided enclosing simulation. After
        /// this call has been made, the simulation can be queried for the
        /// n-body force acting on a given item.
        /// </summary>
        /// <param name="fsim">the encompassing ForceSimulator</param>        
        public override void Init(ForceSimulator fsim)
        {
            clear(); // clear internal state

            // compute and squarify bounds of quadtree
            float x1 = float.MaxValue, y1 = float.MaxValue;
            float x2 = float.MinValue, y2 = float.MinValue;
            foreach (ForceItem item in fsim.Items)
            {
                float x = item.Location[0];
                float y = item.Location[1];
                if (x < x1) x1 = x;
                if (y < y1) y1 = y;
                if (x > x2) x2 = x;
                if (y > y2) y2 = y;
            }
            float dx = x2 - x1, dy = y2 - y1;
            if (dx > dy) { y2 = y1 + dx; } else { x2 = x1 + dy; }
            setBounds(x1, y1, x2, y2);

            // Insert items into quadtree
            foreach (ForceItem item in fsim.Items)
            {
                Insert(item);
            }

            // calculate magnitudes and centers of mass
            CalculateMass(root);
        }

        /// <summary>
        /// Inserts an item into the quadtree.
        /// </summary>
        /// <param name="item"> the ForceItem to add.</param>
        public void Insert(ForceItem item)
        {
            // Insert item into the quadtrees
            try
            {
                Insert(item, root, xMin, yMin, xMax, yMax);
            }
            catch (StackOverflowException e)
            {
                // TODO: safe to remove?
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Inserts the specified force.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        private void Insert(ForceItem p, QuadTreeNode n, float x1, float y1, float x2, float y2)
        {
            // try to Insert particle p at node n in the quadtree
            // by construction, each leaf will contain either 1 or 0 particles
            if (n.hasChildren)
            {
                // n contains more than 1 particle
                InsertHelper(p, n, x1, y1, x2, y2);
            }
            else if (n.value != null)
            {
                // n contains 1 particle
                if (IsSameLocation(n.value, p))
                {
                    InsertHelper(p, n, x1, y1, x2, y2);
                }
                else
                {
                    ForceItem v = n.value; n.value = null;
                    InsertHelper(v, n, x1, y1, x2, y2);
                    InsertHelper(p, n, x1, y1, x2, y2);
                }
            }
            else
            {
                // n is empty, so is a leaf
                n.value = p;
            }
        }

        /// <summary>
        /// Determines whether the two force are at the same location.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        /// <returns>
        /// 	<c>true</c> if [is same location] [the specified f1]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSameLocation(ForceItem f1, ForceItem f2)
        {
            float dx = Math.Abs(f1.Location[0] - f2.Location[0]);
            float dy = Math.Abs(f1.Location[1] - f2.Location[1]);
            return (dx < 0.01 && dy < 0.01);
        }

        /// <summary>
        /// Inserts helper method.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="n">The n.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        private void InsertHelper(ForceItem p, QuadTreeNode n, float x1, float y1, float x2, float y2)
        {
            float x = p.Location[0], y = p.Location[1];
            float splitx = (x1 + x2) / 2;
            float splity = (y1 + y2) / 2;
            int i = (x >= splitx ? 1 : 0) + (y >= splity ? 2 : 0);
            // create new child node, if necessary
            if (n.children[i] == null)
            {
                n.children[i] = factory.GetQuadTreeNode();
                n.hasChildren = true;
            }
            // update bounds
            if (i == 1 || i == 3) x1 = splitx; else x2 = splitx;
            if (i > 1) y1 = splity; else y2 = splity;
            // recurse 
            Insert(p, n.children[i], x1, y1, x2, y2);
        }

        /// <summary>
        /// Calculates the mass.
        /// </summary>
        /// <param name="n">The n.</param>
        private void CalculateMass(QuadTreeNode n)
        {
            float xcom = 0, ycom = 0;
            n.mass = 0;
            if (n.hasChildren)
            {
                for (int i = 0; i < n.children.Length; i++)
                {
                    if (n.children[i] != null)
                    {
                        CalculateMass(n.children[i]);
                        n.mass += n.children[i].mass;
                        xcom += n.children[i].mass * n.children[i].com[0];
                        ycom += n.children[i].mass * n.children[i].com[1];
                    }
                }
            }
            if (n.value != null)
            {
                n.mass += n.value.Mass;
                xcom += n.value.Mass * n.value.Location[0];
                ycom += n.value.Mass * n.value.Location[1];
            }
            n.com[0] = xcom / n.mass;
            n.com[1] = ycom / n.mass;
        }

        /**
         * Calculates the force vector acting on the given item.
         * @param item the ForceItem for which to compute the force
         */
        public override void GetForce(ForceItem item)
        {
            try
            {
                ForceHelper(item, root, xMin, yMin, xMax, yMax);
            }
            catch (StackOverflowException e)
            {
                // TODO: safe to remove?
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Utility method.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="n">The n.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        private void ForceHelper(ForceItem item, QuadTreeNode n, float x1, float y1, float x2, float y2)
        {
            float dx = n.com[0] - item.Location[0];
            float dy = n.com[1] - item.Location[1];
            float r = (float)Math.Sqrt(dx * dx + dy * dy);
            bool same = false;
            if (r == 0.0f)
            {
                // if items are in the exact same place, add some noise
                dx = Convert.ToSingle((rand.NextDouble() - 0.5D) / 50.0D);
                dy = Convert.ToSingle((rand.NextDouble() - 0.5D) / 50.0D);
                r = (float)Math.Sqrt(dx * dx + dy * dy);
                same = true;
            }
            bool minDist = parms[MIN_DISTANCE] > 0f && r > parms[MIN_DISTANCE];

            // the Barnes-Hut approximation criteria is if the ratio of the
            // size of the quadtree box to the distance between the point and
            // the box's center of mass is beneath some threshold theta.
            if ((!n.hasChildren && n.value != item) ||
                 (!same && (x2 - x1) / r < parms[BARNES_HUT_THETA]))
            {
                if (minDist) return;
                // either only 1 particle or we meet criteria
                // for Barnes-Hut approximation, so calc force
                float v = parms[GRAVITATIONAL_CONST] * item.Mass * n.mass
                            / (r * r * r);
                item.Force[0] += v * dx;
                item.Force[1] += v * dy;
            }
            else if (n.hasChildren)
            {
                // recurse for more accurate calculation
                float splitx = (x1 + x2) / 2;
                float splity = (y1 + y2) / 2;
                for (int i = 0; i < n.children.Length; i++)
                {
                    if (n.children != null && n.children[i] != null)
                    {
                        ForceHelper(item, n.children[i],
                            (i == 1 || i == 3 ? splitx : x1), (i > 1 ? splity : y1),
                            (i == 1 || i == 3 ? x2 : splitx), (i > 1 ? y2 : splity));
                    }
                }
                if (minDist) return;
                if (n.value != null && n.value != item)
                {
                    float v = parms[GRAVITATIONAL_CONST] * item.Mass * n.value.Mass
                                / (r * r * r);
                    item.Force[0] += v * dx;
                    item.Force[1] += v * dy;
                }
            }
        }
        #endregion

        #region Classes
        /// <summary>
        /// Represents a node in the quadtree.
        /// </summary>
        public sealed class QuadTreeNode
        {
            public QuadTreeNode()
            {
                com = new float[] { 0.0f, 0.0f };
                children = new QuadTreeNode[4];
            } //
            public bool hasChildren = false;
            public float mass; // total mass held by this node
            public float[] com; // center of mass of this node 
            public ForceItem value; // ForceItem in this node, null if node has children
            public QuadTreeNode[] children; // children nodes
        }




        ///<summary>
        ///Helper class to minimize number of object creations across multiple uses of the quadtree.
        ///</summary>
        public sealed class QuadTreeNodeFactory
        {
            #region Fields
            private int maxNodes = 50000;
            private List<QuadTreeNode> nodes = new List<QuadTreeNode>();
            #endregion

            /// <summary>
            /// Gets the quadtree node.
            /// </summary>
            /// <returns></returns>
            public QuadTreeNode GetQuadTreeNode()
            {
                if (nodes.Count > 0)
                {
                    QuadTreeNode node = this.nodes[nodes.Count - 1];
                    nodes.Remove(node);
                    return node;
                }
                else
                {
                    return new QuadTreeNode();
                }
            }
            /// <summary>
            /// Reclaims the specified node.
            /// </summary>
            /// <param name="n">The n.</param>
            public void Reclaim(QuadTreeNode n)
            {
                n.mass = 0;
                n.com[0] = 0.0f; n.com[1] = 0.0f;
                n.value = null;
                n.hasChildren = false;
                for (int i = 0; i < n.children.Length; i++)
                {
                    n.children[i] = null;
                }
                //n.children = null;
                if (nodes.Count < maxNodes)
                    nodes.Add(n);
            }
        }
        #endregion

    }

}
