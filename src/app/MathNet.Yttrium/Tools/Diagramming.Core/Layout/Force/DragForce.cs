using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    ///  Implements a viscosity/drag force to help stabilize items.
    /// </summary>
    public class DragForce : AbstractForce
    {
        #region Fields
        /// <summary>
        /// the parameter names
        /// </summary>
        private static String[] pnames = new String[] { "DragCoefficient" };
        /// <summary>
        /// dragging coefficient
        /// </summary>
        public static float DefaultDragCoeff = 0.01f;
        /// <summary>
        /// minimum dragging coefficient
        /// </summary>
        public static float DefaultMinDragCoeff = 0.0f;
        /// <summary>
        /// maximum drag coefficient
        /// </summary>
        public static float DefaultMaxDragCoeff = 0.1f;
        /// <summary>
        /// current dragging coefficient
        /// </summary>
        public static int DragCoeff = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Returns true.
        /// </summary>
        /// <value></value>
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

        #region Methods
        /// <summary>
        ///  Create a new DragForce.
        /// </summary>
        /// <param name="dragCoeff">The drag coefficient.</param>
        public DragForce(float dragCoeff)
        {
            parms = new float[] { dragCoeff };
            minValues = new float[] { DefaultMinDragCoeff };
            maxValues = new float[] { DefaultMaxDragCoeff };
        }
          
         /// <summary>
        /// Create a new DragForce with default drag co-efficient.
         /// </summary>
        public DragForce()
            : this(DefaultDragCoeff)
        {

        }

        /// <summary>
        /// Returns the force acting on the given item.
        /// </summary>
        /// <param name="item"></param>
        public override void GetForce(ForceItem item)
        {
            item.Force[0] -= parms[DragCoeff] * item.Velocity[0];
            item.Force[1] -= parms[DragCoeff] * item.Velocity[1];
        }
        #endregion
        

    }
}
