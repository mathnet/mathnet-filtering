using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    ///  Manages a simulation of physical forces acting on bodies. To create a
    /// custom ForceSimulator, add the desired <see cref="IForce"/> functions and choose an
    /// appropriate <see cref="Integrator"/>.
    /// </summary>
    public class ForceSimulator
    {
        #region Fields
        /// <summary>
        /// the force items
        /// </summary>
        private List<ForceItem> mItems;
        /// <summary>
        /// the spring items
        /// </summary>
        private List<Spring> springs;
        /// <summary>
        /// the forces
        /// </summary>
        private IForce[] iforces;
        /// <summary>
        /// the spring forces
        /// </summary>
        private IForce[] sforces;

        private int iflen;
        private int sflen;
        private IIntegrator mIntegrator;
        /// <summary>
        /// the maximum speed allowed
        /// </summary>
        private float mSpeedLimit = 1.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Get an iterator over all registered ForceItems.
        /// </summary>
        /// <value> the ForceItems.</value>
        public List<ForceItem> Items
        {
            get
            {
                return mItems;
            }
        }
        /// <summary>
        /// Get an array of all the IForce functions used in this simulator.
        /// </summary>
        /// <value> an array of IForce functions</value>
        public IForce[] Forces
        {
            get
            {

                IForce[] rv = new IForce[iflen + sflen];
                Array.Copy(iforces, 0, rv, 0, iflen);
                Array.Copy(sforces, 0, rv, iflen, sflen);
                return rv;
            }
        }
        /// <summary>
        /// Gets all registered Springs.
        /// </summary>
        public List<Spring> Springs
        {
            get
            {
                return springs;
            }
        }
        /// <summary>
        /// Get or sets the speed limit, or maximum velocity value allowed by this
        /// simulator.
        /// </summary>
        /// <returns>the "speed limit" maximum velocity value</returns>
        public float SpeedLimit
        {
            get
            {
                return mSpeedLimit;
            }
            set { mSpeedLimit = value; }
        }
        /// <summary>
        /// Get or sets the Integrator used by this simulator.
        /// </summary>
        public IIntegrator Integrator
        {
            get
            {
                return mIntegrator;
            }
            set { mIntegrator = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new, empty ForceSimulator. A RungeKuttaIntegrator is used
        /// by default.
        /// </summary>
        public ForceSimulator()
            : this(new RungeKuttaIntegrator())
        {
        }

        /// <summary>
        /// Create a new, empty ForceSimulator.
        /// </summary>
        /// <param name="integr">the Integrator to use</param>
        public ForceSimulator(IIntegrator integr)
        {
            mIntegrator = integr;
            iforces = new IForce[5];
            sforces = new IForce[5];
            iflen = 0;
            sflen = 0;
            mItems = new List<ForceItem>();
            springs = new List<Spring>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Clear this simulator, removing all ForceItem and Spring instances
        /// for the simulator.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
            Spring.SpringFactory f = Spring.Factory;
            foreach (Spring spring in springs)
                f.reclaim(spring);
            springs.Clear();
        }

        /// <summary>
        /// Add a new IForce function to the simulator.
        /// </summary>
        /// <param name="f">the IForce function to add</param>
        public void AddForce(IForce f)
        {
            if (f.IsItemForce)
            {
                if (iforces.Length == iflen)
                {
                    // resize necessary
                    IForce[] newf = new IForce[iflen + 10];
                    Array.Copy(iforces, 0, newf, 0, iforces.Length);
                    iforces = newf;
                }
                iforces[iflen++] = f;
            }
            if (f.IsSpringForce)
            {
                if (sforces.Length == sflen)
                {
                    // resize necessary
                    IForce[] newf = new IForce[sflen + 10];
                    Array.Copy(sforces, 0, newf, 0, sforces.Length);
                    sforces = newf;
                }
                sforces[sflen++] = f;
            }
        }

        /// <summary>
        /// Add a ForceItem to the simulation.
        /// </summary>
        /// <param name="item"> item the ForceItem to add.</param>
        public void addItem(ForceItem item)
        {
            mItems.Add(item);
        }

        /// <summary>
        /// Remove a ForceItem to the simulation.
        /// </summary>
        /// <param name="item">Item the ForceItem to remove.</param>
        /// <returns></returns>
        public bool removeItem(ForceItem item)
        {
            return mItems.Remove(item);
        }

        /// <summary>
        /// Add a Spring to the simulation.
        /// </summary>
        /// <param name="item1">the first endpoint of the spring</param>
        /// <param name="item2">the second endpoint of the spring</param>
        /// <returns>the Spring added to the simulation</returns>
        public Spring addSpring(ForceItem item1, ForceItem item2)
        {
            return addSpring(item1, item2, -1.0F, -1.0F);
        }

        /// <summary>
        /// Add a Spring to the simulation.
        /// </summary>
        /// <param name="item1">the first endpoint of the spring</param>
        /// <param name="item2">the second endpoint of the spring</param>
        /// <param name="length">the spring length</param>
        /// <returns>the Spring added to the simulation</returns>
        public Spring addSpring(ForceItem item1, ForceItem item2, float length)
        {
            return addSpring(item1, item2, -1.0F, length);
        }

        /// <summary>
        /// Add a Spring to the simulation.
        /// </summary>
        /// <param name="item1">the first endpoint of the spring</param>
        /// <param name="item2"> the second endpoint of the spring</param>
        /// <param name="coeff">the spring coefficient</param>
        /// <param name="length">the spring length</param>
        /// <returns> the Spring added to the simulation</returns>
        public Spring addSpring(ForceItem item1, ForceItem item2, float coeff, float length)
        {
            if (item1 == null || item2 == null)
                throw new ArgumentException("ForceItems must be non-null");
            Spring s = Spring.Factory.getSpring(item1, item2, coeff, length);
            springs.Add(s);
            return s;
        }

        /// <summary>
        /// Run the simulator for one timestep.
        /// </summary>
        /// <param name="timestep">the span of the timestep for which to run the simulator</param>
        public void RunSimulator(long timestep)
        {
            Accumulate();
            mIntegrator.Integrate(this, timestep);
        }

        /// <summary>
        /// Accumulate all forces acting on the items in this simulation
        /// </summary>
        public void Accumulate()
        {
            for (int i = 0; i < iflen; i++)
                iforces[i].Init(this);
            for (int i = 0; i < sflen; i++)
                sforces[i].Init(this);
            foreach (ForceItem item in mItems)
            {
                item.Force[0] = 0.0f; item.Force[1] = 0.0f;
                for (int i = 0; i < iflen; i++)
                    iforces[i].GetForce(item);
            }
            foreach (Spring s in springs)
            {
                for (int i = 0; i < sflen; i++)
                {
                    sforces[i].GetForce(s);
                }
            }
        }
        #endregion

    }
}
