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
    /// Cubic Hermite Spline Interpolation Algorithm.
    /// </summary>
    /// <remarks>
    /// This algorithm supports both differentiation and interation.
    /// </remarks>
    public class CubicHermiteSplineInterpolation :
        IInterpolationMethod
    {
        SplineInterpolation _spline;

        /// <summary>
        /// Create an interpolation algorithm instance.
        /// </summary>
        public
        CubicHermiteSplineInterpolation()
        {
            _spline = new SplineInterpolation();
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return _spline.SupportsDifferentiation; }
        }

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        public bool SupportsIntegration
        {
            get { return _spline.SupportsIntegration; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given samples.
        /// </summary>
        /// <param name="t">Points t</param>
        /// <param name="x">Values x(t)</param>
        /// <param name="d">Derivatives x'(t)</param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x,
            IList<double> d
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

            if(null == d)
            {
                throw new ArgumentNullException("d");
            }

            if(t.Count < 2)
            {
                throw new ArgumentOutOfRangeException("t");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths, "x");
            }

            if(t.Count != d.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths, "d");
            }

            double[] tt = new double[t.Count];
            t.CopyTo(tt, 0);
            double[] xx = new double[x.Count];
            x.CopyTo(xx, 0);
            double[] dd = new double[d.Count];
            d.CopyTo(dd, 0);

            Sorting.Sort(tt, xx, dd);

            InitInternal(tt, xx, dd);
        }

        /// <summary>
        /// Internal Init, skip parameter validation and sorting.
        /// </summary>
        internal
        void
        InitInternal(
            double[] t,
            double[] x,
            double[] d
            )
        {
            double[] c = new double[4 * (t.Length - 1)];

            for(int i = 0, j = 0; i < t.Length - 1; i++, j += 4)
            {
                double delta = t[i + 1] - t[i];
                double delta2 = delta * delta;
                double delta3 = delta * delta2;
                c[j] = x[i];
                c[j + 1] = d[i];
                c[j + 2] = (3 * (x[i + 1] - x[i]) - 2 * d[i] * delta - d[i + 1] * delta) / delta2;
                c[j + 3] = (2 * (x[i] - x[i + 1]) + d[i] * delta + d[i + 1] * delta) / delta3;
            }

            _spline.Init(t, c);
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
            return _spline.Interpolate(t);
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
            return _spline.Differentiate(t, out first, out second);
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
            return _spline.Integrate(t);
        }
    }
}
