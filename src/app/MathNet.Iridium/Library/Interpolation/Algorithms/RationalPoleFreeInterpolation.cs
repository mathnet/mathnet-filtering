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
#region Some algorithms based on: Copyright 2007 Bochkanov
// ALGLIB
// Copyright by Sergey Bochkanov
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// - Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer listed
//   in this license in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the copyright holders nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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
        /// Create an interpolation algorithm instance.
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
            IList<double> x
            )
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
            int order
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

            if(t.Count < 1)
            {
                throw new ArgumentOutOfRangeException("t");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths);
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
        Interpolate(
            double t
            )
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
            out double second
            )
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
        Integrate(
            double t
            )
        {
            return _barycentric.Integrate(t);
        }
    }
}
