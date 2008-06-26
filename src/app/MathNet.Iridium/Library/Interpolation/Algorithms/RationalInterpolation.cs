#region Math.NET Iridium (LGPL) by Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//
// Contribution: Numerical Recipes in C++, Second Edition [2003]
//               Handbook of Mathematical Functions [1965]
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
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Interpolation.Algorithms
{
    /// <summary>
    /// Rational Interpolation (with poles) using Bulirsch &amp; Stoer's Algorithm.
    /// </summary>
    public class RationalInterpolation :
        IInterpolationMethod
    {
        IList<double> _t;
        IList<double> _x;

        /// <summary>
        /// Create a neville polynomial interpolation algorithm instance.
        /// </summary>
        /// <remarks>
        /// This algorithm neither supports differentiation nor interation.
        /// </remarks>
        public
        RationalInterpolation()
        {
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return false; }
        }

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        public bool SupportsIntegration
        {
            get { return false; }
        }
        
        /// <summary>
        /// Initialize the interpolation method with the given sample set.
        /// </summary>
        /// <param name="t">Points t</param>
        /// <param name="x">Values x(t)</param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x
            )
        {
            if(null == t)
            {
                throw new ArgumentNullException("t");
            }

            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths);
            }

            _t = t;
            _x = x;
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        /// <seealso cref="Differentiate"/>
        public
        double
        Interpolate(
            double t
            )
        {
            const double tiny = 1.0e-25;
            int n = _t.Count;

            double[] c = new double[n];
            double[] d = new double[n];

            int nearestIndex = 0;
            double nearestDistance = Math.Abs(t - _t[0]);

            for(int i = 0; i < n; i++)
            {
                double distance = Math.Abs(t - _t[i]);
                if(Number.AlmostZero(distance))
                {
                    return _x[i];
                }

                if(distance < nearestDistance)
                {
                    nearestIndex = i;
                    nearestDistance = distance;
                }

                c[i] = _x[i];
                d[i] = _x[i] + tiny;
            }

            double x = _x[nearestIndex];

            for(int level = 1; level < n; level++)
            {
                for(int i = 0; i < n - level; i++)
                {
                    double hp = _t[i + level] - t;
                    double ho = (_t[i] - t) * d[i] / hp;

                    double den = ho - c[i + 1];
                    if(Number.AlmostZero(den))
                    {
                        return double.NaN; // zero-div, singularity
                    }

                    den = (c[i + 1] - d[i]) / den;
                    d[i] = c[i + 1] * den;
                    c[i] = ho * den;
                }

                x += (2 * nearestIndex) < (n - level)
                    ? c[nearestIndex]
                    : d[--nearestIndex];
            }

            return x;
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <param name="first">Interpolated first derivative at point t.</param>
        /// <param name="second">Interpolated second derivative at point t.</param>
        /// <returns>Interpolated value x(t).</returns>
        public
        double
        Differentiate(
            double t,
            out double first,
            out double second
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Definite Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integeral over the interval [a,t].</returns>
        /// <seealso cref="SupportsIntegration"/>
        public
        double
        Integrate(
            double t
            )
        {
            throw new NotSupportedException();
        }
    }
}
