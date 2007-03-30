using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    /// Represents a point particle in a force simulation, maintaining values for
    /// mass, forces, velocity, and position.
    /// </summary>
    public class ForceItem : ICloneable
    {
        #region Fields
        /// <summary>
        /// Temporary variables for Runge-Kutta integration 
        /// </summary>
        private float[,] l;
        /// <summary>
        /// The mass value of this ForceItem. 
        /// </summary>
        private float mass;
        /// <summary> The values of the forces acting on this ForceItem. 
        /// </summary>
        private float[] force;
        /// <summary>
        /// The velocity values of this ForceItem. 
        /// </summary>
        private float[] velocity;
        /// <summary> 
        /// The location values of this ForceItem. 
        /// </summary>
        private float[] location;
        /// <summary> 
        /// The previous location values of this ForceItem.
        /// /// </summary>
        private float[] plocation;
        /// <summary> 
        /// Temporary variables for Runge-Kutta integration
        /// /// </summary>
        private float[,] k;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the temporary Runge-Kutta integration value.
        /// </summary>
        /// <value>The runge kutta temp1.</value>
        public float[,] RungeKuttaTemp1
        {
            get { return k; }
            set { k = value; }
        }


        /// <summary>
        /// Gets or sets the temporary Runge-Kutta integration value.
        /// </summary>
        /// <value>The runge kutta temp2.</value>
        public float[,] RungeKuttaTemp2
        {
            get { return l; }
            set { l = value; }
        }
        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        /// <value>The mass.</value>
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <value>The force.</value>
        public float[] Force
        {
            get { return force; }
            set { force = value; }
        }
        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>The velocity.</value>
        public float[] Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public float[] Location
        {
            get { return location; }
            set { location = value; }
        }
        /// <summary>
        /// Gets or sets the previous location.
        /// </summary>
        /// <value>The previous location.</value>
        public float[] PreviousLocation
        {
            get { return plocation; }
            set { plocation = value; }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Create a new ForceItem.
        /// </summary>
        public ForceItem()
        {
            mass = 1.0f;
            force = new float[] { 0.0F, 0.0F };
            velocity = new float[] { 0.0F, 0.0F };
            location = new float[] { 0.0F, 0.0F };
            plocation = new float[] { 0.0F, 0.0F };
            k = new float[4, 2];
            l = new float[4, 2];
        }

        #endregion



        #region Methods
        /// <summary>
        /// Checks a ForceItem to make sure its values are all valid numbers(i.e., not NaNs).
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified item is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool isValid(ForceItem item)
        {
            return
              !(float.IsNaN(item.location[0]) || float.IsNaN(item.location[1]) ||
                 float.IsNaN(item.plocation[0]) || float.IsNaN(item.plocation[1]) ||
                 float.IsNaN(item.velocity[0]) || float.IsNaN(item.velocity[1]) ||
                 float.IsNaN(item.force[0]) || float.IsNaN(item.force[1]));
        }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            //ForceItem item = new ForceItem();
            //item.mass = this.mass;
            //Array.Copy(force, 0, item.force, 0, 2);
            //Array.Copy(velocity, 0, item.velocity, 0, 2);
            //Array.Copy(location, 0, item.location, 0, 2);
            //Array.Copy(plocation, 0, item.plocation, 0, 2);
            //for (int i = 0; i < k.Length; ++i)
            //{
            //    Array.Copy(k[i,], 0, item.k[i,], 0, 2);
            //    Array.Copy(l[i], 0, item.l[i], 0, 2);
            //    Array
            //}
            //return item;
            throw new NotImplementedException("Do you really need this feature?");
        }
        #endregion
    }
}
