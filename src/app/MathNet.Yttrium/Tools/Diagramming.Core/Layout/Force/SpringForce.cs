using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{

    /// <summary>
    ///  Force function that computes the force acting on ForceItems due to a  given Spring.
    /// </summary>
    public class SpringForce : AbstractForce
    {

        #region Fields
        Random rnd = new Random();
        private static String[] pnames = new String[] { "SpringCoefficient", "DefaultSpringLength" };
        public static float DefaultSpriongCoeff = 1E-4f;
        public static float DefaultMaxSpringCoeff = 1E-3f;
        public static float DefaultMinSpringCoeff = 1E-5f;
        public static float DefaultSpringLength = 50;
        public static float DefaultMinSpringLength = 0;
        public static float DefaultMaxSpringLength = 200;
        public static int SpringCoeff = 0;
        public static int SpringLength = 1;

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether [spring force].
        /// </summary>
        /// <value><c>true</c> if [spring force]; otherwise, <c>false</c>.</value>
        public override bool IsSpringForce
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
        ///  Create a new SpringForce.
        /// </summary>
        /// <param name="springCoeff"> the default spring co-efficient to use. This will be used if the spring's own co-efficient is less than zero..</param>
        /// <param name="defaultLength">the default spring length to use. This will be used if the spring's own length is less than zero.</param>
        public SpringForce(float springCoeff, float defaultLength)
        {
            parms = new float[] { springCoeff, defaultLength };
            minValues = new float[] { DefaultMinSpringCoeff, DefaultMinSpringLength };
            maxValues = new float[] { DefaultMaxSpringCoeff, DefaultMaxSpringLength };
        }

        /// <summary>
        /// Constructs a new SpringForce instance with default parameters.
        /// </summary>
        public SpringForce()
            : this(DefaultSpriongCoeff, DefaultSpringLength)
        {
        } 
        #endregion

        #region Methdos
        /// <summary>
        /// Gets the force.
        /// </summary>
        /// <param name="s">The s.</param>
        public override void GetForce(Spring s)
        {
            ForceItem item1 = s.Item1;
            ForceItem item2 = s.Item2;
            float length = (s.Length < 0 ? parms[SpringLength] : s.Length);
            float x1 = item1.Location[0], y1 = item1.Location[1];
            float x2 = item2.Location[0], y2 = item2.Location[1];
            float dx = x2 - x1, dy = y2 - y1;
            float r = (float)Math.Sqrt(dx * dx + dy * dy);
            if (r == 0.0)
            {
                dx = ((float)rnd.NextDouble() - 0.5f) / 50.0f;
                dy = ((float)rnd.NextDouble() - 0.5f) / 50.0f;
                r = (float)Math.Sqrt(dx * dx + dy * dy);
            }
            float d = r - length;
            float coeff = (s.Coeff < 0 ? parms[SpringCoeff] : s.Coeff) * d / r;
            item1.Force[0] += coeff * dx;
            item1.Force[1] += coeff * dy;
            item2.Force[0] += -coeff * dx;
            item2.Force[1] += -coeff * dy;
        }

        #endregion

    }
}
