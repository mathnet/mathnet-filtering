using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics.Transformations
{
    [Flags]
    public enum TransformationConvention : int
    {
        // FLAGS:

        /// <summary>
        /// Only scale by 1/N in the inverse direction; No scaling in forward direction.
        /// </summary>
        AsymmetricScaling = 0x01,

        /// <summary>
        /// Inverse integrand exponent (forward: positive sign; inverse: negative sign).
        /// </summary>
        InverseExponent = 0x02,

        /// <summary>
        /// Don't scale at all (neither on forward nor on inverse transformation).
        /// </summary>
        NoScaling = 0x04,


        // USABILITY POINTERS:

        /// <summary>
        /// Universal; Symmetric scaling and common exponent (used in Maple).
        /// </summary>
        Default = 0,

        /// <summary>
        /// Only scale by 1/N in the inverse direction; No scaling in forward direction (used in Matlab). [= AsymmetricScaling]
        /// </summary>
        Matlab = AsymmetricScaling,

        /// <summary>
        /// Inverse integrand exponent; No scaling at all (used in all Numerical Recipes based implementations). [= InverseExponent | NoScaling]
        /// </summary>
        NumericalRecipes = InverseExponent | NoScaling
    }
}
