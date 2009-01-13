//-----------------------------------------------------------------------
// <copyright file="RationalPoleFreeInterpolation.cs" company="Math.NET Project">
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
    /// Barycentric Rational Interpolation without poles using Floater and Hormann's Algorithm.
    /// </summary>
    /// <remarks>
    /// This algorithm neither supports differentiation nor integration.
    /// </remarks>
    public class RationalPoleFreeInterpolation :
        IInterpolationMethod
    {
        BarycentricInterpolation _barycentric;

        /// <summary>
        /// Initializes a new instance of the RationalPoleFreeInterpolation class.
        /// </summary>
        public
        RationalPoleFreeInterpolation()
        {
            _barycentric = new BarycentricInterpolation();
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return _barycentric.SupportsDifferentiation; }
        }

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        public bool SupportsIntegration
        {
            get { return _barycentric.SupportsIntegration; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given samples.
        /// </summary>
        /// <remarks>
        /// The interpolation scheme order will be set to 3.
        /// </remarks>
        /// <param name="t">Points t</param>
        /// <param name="x">Values x(t)</param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x)
        {
            Init(t, x, 3);
        }

        /// <summary>
        /// Initialize the interpolation method with the given samples.
        /// </summary>
        /// <param name="t">Points t</param>
        /// <param name="x">Values x(t)</param>
        /// <param name="order">
        /// Order of the interpolation scheme, 0 &lt;= order &lt;= N.
        /// In most cases a value between 3 and 8 gives good results.
        /// </param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x,
            int order)
        {
            if(null == t)
            {
                throw new ArgumentNullException("t");
            }

            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(t.Count < 1)
            {
                throw new ArgumentOutOfRangeException("t");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentVectorsSameLengths);
            }

            if(0 > order || t.Count < order)
            {
                throw new ArgumentOutOfRangeException("order");
            }

            double[] ww = new double[x.Count];
            double[] tt = new double[t.Count];
            t.CopyTo(tt, 0);

            // order: odd -> negative, even -> positive
            double sign = ((order & 0x1) == 0x1) ? -1.0 : 1.0; 

            // init permutation vector
            int[] perm = new int[ww.Length];
            for(int i = 0; i < perm.Length; i++)
            {
                perm[i] = i;
            }

            // sort and update permutation vector
            for(int i = 0; i < perm.Length - 1; i++)
            {
                for(int j = i + 1; j < perm.Length; j++)
                {
                    if(tt[j] < tt[i])
                    {
                        double s = tt[i];
                        tt[i] = tt[j];
                        tt[j] = s;
                        int k = perm[i];
                        perm[i] = perm[j];
                        perm[j] = k;
                    }
                }
            }

            // compute barycentric weights
            for(int k = 0; k < ww.Length; k++)
            {
                // Wk
                double s = 0;
                for(int i = Math.Max(k - order, 0); i <= Math.Min(k, ww.Length - 1 - order); i++)
                {
                    double v = 1;
                    for(int j = i; j <= i + order; j++)
                    {
                        if(j != k)
                        {
                            v = v / Math.Abs(tt[k] - tt[j]);
                        }
                    }

                    s = s + v;
                }

                ww[k] = sign * s;
                sign = -sign;
            }

            // reorder back to original order, based on the permutation vector.
            double[] w = new double[ww.Length];
            for(int i = 0; i < w.Length; i++)
            {
                w[perm[i]] = ww[i];
            }

            _barycentric.Init(t, x, w);
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        public
        double
        Interpolate(double t)
        {
            return _barycentric.Interpolate(t);
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
            return _barycentric.Differentiate(t, out first, out second);
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
            return _barycentric.Integrate(t);
        }
    }
}
