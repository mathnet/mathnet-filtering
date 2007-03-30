using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    /// Interface for force functions in a force simulation.
    /// </summary>
    public interface IForce
    {
        /// <summary>
        /// Initialize this force function.
        /// </summary>
        /// <param name="fsim"> the encompassing ForceSimulator</param>     
        void Init(ForceSimulator fsim);

        /// <summary>
        /// Returns the number of parameters (e.g., gravitational constant or
        /// spring force coefficient) affecting this force function. 
        /// </summary>
        int ParameterCount { get;}

        /// <summary>
        /// Returns the specified, numbered parameter.
        /// </summary>
        /// <param name="i">i the index of the parameter to return</param>
        /// <returns>the parameter value</returns>
        float GetParameter(int i);

        /// <summary>
        /// Get the suggested minimum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="param">the parameter index</param>
        /// <returns>the suggested minimum value.</returns>
        float GetMinValue(int param);

        /// <summary>
        /// Get the suggested maximum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="param">the parameter index</param>
        /// <returns>the suggested maximum value.</returns>
        float GetMaxValue(int param);

        /// <summary>
        /// Gets the text name of the requested parameter.
        /// </summary>
        /// <param name="i"> the index of the parameter</param>
        /// <returns>a String containing the name of this parameter</returns>
        String GetParameterName(int i);

        /// <summary>
        /// Sets the specified parameter value.
        /// </summary>
        /// <param name="i">  the index of the parameter</param>
        /// <param name="val">the new value of the parameter</param>
        void SetParameter(int i, float val);

        /// <summary>
        /// Set the suggested minimum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="i">the parameter index</param>
        /// <param name="val">the suggested minimum value to use</param>
        void SetMinValue(int i, float val);

        /// <summary>
        /// Set the suggested maximum value for a parameter. This value is not
        /// strictly enforced, but is used by interface components that allow force
        /// parameters to be varied.
        /// </summary>
        /// <param name="i">the parameter index</param>
        /// <param name="val">the suggested maximum value to use</param>
        void SetMaxValue(int i, float val);

        /// <summary>
        /// Indicates if this force function will compute forces on Spring instances.   
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this force function processes Spring instances; otherwise, <c>false</c>.
        /// </returns>
        bool IsSpringForce { get;}

        /// <summary>
        /// Indicates if this force function will compute forces on ForceItem instances
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this force function processes IForce instances; otherwise, <c>false</c>.
        /// </returns>
        bool IsItemForce { get;}

        /// <summary>
        /// Updates the force calculation on the given ForceItem
        /// </summary>
        /// <param name="item"> the ForceItem on which to compute updated forces</param>
        void GetForce(ForceItem item);

        /// <summary>
        /// Updates the force calculation on the given Spring. The ForceItems
        /// attached to Spring will have their force values updated appropriately.
        /// </summary>
        /// <param name="spring">spring the Spring on which to compute updated forces</param>
        void GetForce(Spring spring);

    }

}
