using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    ///Abstract base class for force functions in a force simulation. This
    ///skeletal version provides support for storing and retrieving float-valued
    ///parameters of the force function. Subclasses should use the protected
    ///field <code>parms</code> to store parameter values.
    /// </summary>
    public abstract class AbstractForce : IForce
    {
        #region Fields
        /// <summary>
        /// temporary paramters blobs
        /// </summary>
        protected float[] parms;
        /// <summary>
        /// min values
        /// </summary>
        protected float[] minValues;
        /// <summary>
        /// max values
        /// </summary>
        protected float[] maxValues;

        #endregion

        #region Properties
        /// <summary>
        ///Returns false.
        /// </summary>
        public virtual bool IsSpringForce
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of parameters (e.g., gravitational constant or
        /// spring force coefficient) affecting this force function.
        /// </summary>
        /// <value></value>
        /// <returns>the number of parameters</returns>
        public int ParameterCount
        {
            get
            {
                return (parms == null ? 0 : parms.Length);
            }
        }

        /// <summary>
        ///Gets whether this is a force.
        /// </summary>
        public virtual bool IsItemForce
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize this force function. This default implementation does nothing.
        /// Subclasses should override this method with any needed initialization.
        /// </summary>
        /// <param name="fsim">the encompassing ForceSimulator</param>
        public virtual void Init(ForceSimulator fsim)
        {
            // do nothing.
        }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <returns></returns>
        protected abstract String[] ParameterNames { get;}


        /// <summary>
        /// Returns the specified, numbered parameter.
        /// </summary>
        /// <param name="i">i the index of the parameter to return</param>
        /// <returns>the parameter value</returns>
        public float GetParameter(int i)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return parms[i];
            }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public float GetMinValue(int i)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return minValues[i];
            }
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public float GetMaxValue(int i)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return maxValues[i];
            }
        }

        /// <summary>
        /// Gets the text name of the requested parameter.
        /// </summary>
        /// <param name="i">i the index of the parameter</param>
        /// <returns>
        /// a String containing the name of this parameter
        /// </returns>
        public String GetParameterName(int i)
        {
            String[] pnames = this.ParameterNames;
            if (i < 0 || pnames == null || i >= pnames.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return pnames[i];
            }
        }

        /// <summary>
        /// Sets the specified parameter value.
        /// </summary>
        /// <param name="i">the index of the parameter</param>
        /// <param name="val">the new value of the parameter</param>
        public void SetParameter(int i, float val)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                parms[i] = val;
            }
        }


        /// <summary>
        /// Set the suggested minimum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="i">the parameter index</param>
        /// <param name="val">the suggested minimum value to use</param>
        public void SetMinValue(int i, float val)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                minValues[i] = val;
            }
        }

        /// <summary>
        /// Set the suggested maximum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="i">the parameter index</param>
        /// <param name="val">the suggested maximum value to use</param>
        public void SetMaxValue(int i, float val)
        {
            if (i < 0 || parms == null || i >= parms.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                maxValues[i] = val;
            }
        }

        /// <summary>
        ///Returns the force acting on the given item.
        /// </summary>
        public virtual void GetForce(ForceItem item)
        {
            throw new NotImplementedException("This class does not support this operation");
        }

        /// <summary>
        /// Updates the force calculation on the given Spring. The ForceItems
        /// attached to Spring will have their force values updated appropriately.
        /// </summary>
        /// <param name="spring">spring the Spring on which to compute updated forces</param>
        public virtual void GetForce(Spring spring)
        {
            throw new NotImplementedException("This class does not support this operation");
        }
        #endregion

    }
}
