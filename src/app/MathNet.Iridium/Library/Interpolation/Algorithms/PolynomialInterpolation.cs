//-----------------------------------------------------------------------
// <copyright file="PolynomialInterpolation.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
// <contribution>
//    Numerical Recipes in C++, Second Edition [2003]
//    Handbook of Mathematical Functions [1965]
//    ALGLIB, Sergey Bochkanov
// </contribution>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Interpolation.Algorithms
{
    /// <summary>
    /// Lagrange Polynomial Interpolation using Neville's Algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This algorithm supports differentiation, but doesn't support integration.
    /// </para>
    /// <para>
    /// Consider to use the <see cref="RationalPoleFreeInterpolation"/> or at least 
    /// one of the specially spaced barycentric alternatives like the 
    /// <see cref="EquidistantPolynomialInterpolation"/> or 
    /// <see cref="ChebyshevFirstKindPolynomialInterpolation"/> instead.
    /// </para>
    /// </remarks>
    public class PolynomialInterpolation :
        IInterpolationMethod
    {
        IList<double> _t;
        IList<double> _x;

        /// <summary>
        /// Initializes a new instance of the PolynomialInterpolation class.
        /// </summary>
        public
        PolynomialInterpolation()
        {
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return true; }
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
            IList<double> x)
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
                throw new ArgumentException(Properties.LocalStrings.ArgumentVectorsSameLengths);
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
        Interpolate(double t)
        {
            double[] x = new double[_x.Count];
            _x.CopyTo(x, 0);

            for(int level = 1; level < x.Length; level++)
            {
                for(int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - _t[i + level];
                    double ho = _t[i] - t;
                    double den = _t[i] - _t[i + level];
                    x[i] = ((hp * x[i]) + (ho * x[i + 1])) / den;
                }
            }

            return x[0];
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
            out double second)
        {
            double[] x = new double[_x.Count];
            _x.CopyTo(x, 0);

            double[] dx = new double[x.Length];
            double[] d2x = new double[x.Length];

            for(int level = 1; level < x.Length; level++)
            {
                for(int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - _t[i + level];
                    double ho = _t[i] - t;
                    double den = _t[i] - _t[i + level];
                    d2x[i] = ((hp * d2x[i]) + (ho * d2x[i + 1]) + (2 * dx[i]) - (2 * dx[i + 1])) / den;
                    dx[i] = ((hp * dx[i]) + x[i] + (ho * dx[i + 1]) - x[i + 1]) / den;
                    x[i] = ((hp * x[i]) + (ho * x[i + 1])) / den;
                }
            }

            first = dx[0];
            second = d2x[0];
            return x[0];
        }

        /// <summary>
        /// Definite Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integeral over the interval [a,t].</returns>
        /// <seealso cref="SupportsIntegration"/>
        public
        double
        Integrate(double t)
        {
            throw new NotSupportedException();
        }
    }
}
