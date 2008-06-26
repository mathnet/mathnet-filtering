#region Math.NET Iridium (LGPL) by Ruegg, Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//                          Joannes Vermorel, http://www.vermorel.com
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Pseudo-random generation of standard distributed deviates.
    /// </summary>
    /// 
    /// <remarks> 
    /// <para>For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Normal_distribution">
    /// Wikipedia - Normal distribution</a>.</para>
    /// 
    /// <para>This implementation is based on the <i>Box-Muller</i> algorithm
    /// for generating random deviates with a normal distribution.</para>
    /// 
    /// <para>For details of the algorithm, see
    /// <a href="http://www.library.cornell.edu/nr/">
    /// Numerical recipes in C</a> (chapter 7)</para>
    ///
    /// <para>pdf: f(x) = 1/sqrt(2*Pi)*exp(-x^2/2)</para>
    /// </remarks>
    public sealed class StandardDistribution : ContinuousDistribution
    {
        double? _extraNormal;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        StandardDistribution()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance, using the specified <see cref="RandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="random"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public
        StandardDistribution(
            RandomSource random
            )
            : base(random)
        {
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override double Minimum
        {
            get { return double.MinValue; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override double Maximum
        {
            get { return double.MaxValue; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return 1.0; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Continuous probability density function (pdf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityDensity(
            double x
            )
        {
            return Constants.InvSqrt2Pi * Math.Exp(x * x / -2.0);
        }

        /// <summary>
        /// Continuous cumulative distribution function (cdf) of this probability distribution.
        /// </summary>
        public override
        double
        CumulativeDistribution(
            double x
            )
        {
            return 0.5 * (1.0 + Fn.Erf(x * Constants.Sqrt1_2));
        }

        /// <summary>
        /// Inverse of the continuous cumulative distribution function of this probability distribution.
        /// </summary>
        /// <seealso cref="StandardDistribution.CumulativeDistribution"/>
        public
        double
        InverseCumulativeDistribution(
            double x
            )
        {
            return Constants.Sqrt1_2 * Fn.ErfInverse(2.0 * x - 1.0);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a standard distributed floating point random number.
        /// </summary>
        /// <returns>A standard distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            // Note that this method generate two gaussian deviates
            // at once. The additional deviate is recorded in
            // <c>extraNormal</c>.

            // Using the extra gaussian deviate if available
            if(_extraNormal.HasValue)
            {
                double extraNormalCpy = _extraNormal.Value;
                _extraNormal = null;
                return extraNormalCpy;
            }
            else
            {
                // Generating two new gaussian deviates

                double fac, rsq, v1, v2;

                // We need a non-zero random point inside the unit circle.
                do
                {
                    v1 = 2.0 * this.RandomSource.NextDouble() - 1.0;
                    v2 = 2.0 * this.RandomSource.NextDouble() - 1.0;
                    rsq = v1 * v1 + v2 * v2;
                }
                while(rsq > 1.0 || rsq == 0);

                // Make the Box-Muller transformation
                fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);

                _extraNormal = v1 * fac;
                return (v2 * fac);
            }
        }
        #endregion
    }
}
