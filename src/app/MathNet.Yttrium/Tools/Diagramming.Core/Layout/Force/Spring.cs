using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    /// Represents a spring in a force simulation.
    /// </summary>
    public class Spring
    {
        #region Fields
        private static SpringFactory mFactory = new SpringFactory();
        private ForceItem item1;
        private ForceItem item2;
        private float length;
        private float coeff;

        #endregion

        #region Properties
        /// <summary>
        /// Retrieve the SpringFactory instance, which serves as an object pool
        /// for Spring instances.
        /// </summary>
        /// <value>The spring factory.</value>
        public static SpringFactory Factory
        {
            get
            {
                return mFactory;
            }
        }
        /// <summary> 
        /// The first ForceItem endpoint 
        /// </summary>

        public ForceItem Item1
        {
            get { return item1; }
            set { item1 = value; }
        }
        /// <summary>
        /// The second ForceItem endpoint 
        /// </summary> 
        public ForceItem Item2
        {
            get { return item2; }
            set { item2 = value; }
        }
        /// <summary> 
        /// The spring's resting length 
        /// </summary>
        public float Length
        {
            get { return length; }
            set { length = value; }
        }
        /// <summary> 
        /// The spring tension co-efficient
        /// </summary>
        public float Coeff
        {
            get { return coeff; }
            set { coeff = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new Spring instance
        ///<param name="fi1">  the first ForceItem endpoint</param>
        /// <param name="fi2"> the second ForceItem endpoint          </param>
        ///  <param name="k">the spring tension co-efficient                             </param>
        ///  <param name="len">the spring's resting length                                               </param>
        /// </summary>
        public Spring(ForceItem fi1, ForceItem fi2, float k, float len)
        {
            item1 = fi1;
            item2 = fi2;
            coeff = k;
            length = len;
        }

        #endregion

        #region Classes
        /// <summary>
        /// The SpringFactory is responsible for generating Spring instances
        /// and maintaining an object pool of Springs to reduce garbage collection
        /// overheads while force simulations are running.
        /// </summary>
        public sealed class SpringFactory
        {
            public const int Capacity = 5000;
            private List<Spring> springs = new List<Spring>(Capacity);

            /// <summary>
            /// Get a Spring instance and set it to the given parameters.
            /// </summary>
            public Spring getSpring(ForceItem f1, ForceItem f2, float k, float length)
            {
                if (springs.Count > 0)
                {
                    Spring s = springs[springs.Count - 1];
                    springs.Remove(s);
                    s.Item1 = f1;
                    s.Item2 = f2;
                    s.Coeff = k;
                    s.Length = length;
                    return s;
                }
                else
                {
                    return new Spring(f1, f2, k, length);
                }
            }
            /// <summary>
            /// Reclaim a Spring into the object pool.
            /// </summary>
            public void reclaim(Spring s)
            {
                s.Item1 = null;
                s.Item2 = null;
                if (springs.Count < Capacity)
                    springs.Add(s);
            }
        }
        #endregion
    }
}
