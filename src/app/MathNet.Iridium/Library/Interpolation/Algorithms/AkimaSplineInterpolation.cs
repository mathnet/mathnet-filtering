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
    /// Akima Spline Interpolation Algorithm.
    /// </summary>
    public class AkimaSplineInterpolation :
        IInterpolationMethod
    {
        CubicHermiteSplineInterpolation _hermiteSpline;

        /// <summary>
        /// Create an interpolation algorithm instance.
        /// </summary>
        public
        AkimaSplineInterpolation()
        {
            _hermiteSpline = new CubicHermiteSplineInterpolation();
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return _hermiteSpline.SupportsDifferentiation; }
        }

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        public bool SupportsIntegration
        {
            get { return _hermiteSpline.SupportsIntegration; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given samples and natural boundaries.
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

            if(t.Count < 5)
            {
                throw new ArgumentOutOfRangeException("t");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths, "x");
            }

            int n = t.Count;

            double[] tt = new double[n];
            t.CopyTo(tt, 0);
            double[] xx = new double[n];
            x.CopyTo(xx, 0);

            Sorting.Sort(tt, xx);

            // Prepare W (weights), Diff (divided differences)

            double[] w = new double[n - 1];
            double[] diff = new double[n - 1];

            for(int i = 0; i < diff.Length; i++)
            {
                diff[i] = (xx[i + 1] - xx[i]) / (tt[i + 1] - tt[i]);
            }

            for(int i = 1; i < w.Length; i++)
            {
                w[i] = Math.Abs(diff[i] - diff[i - 1]);
            }

            // Prepare Hermite interpolation scheme

            double[] d = new double[n];

            for(int i = 2; i < d.Length - 2; i++)
            {
                if(!Number.AlmostZero(w[i - 1]) || !Number.AlmostZero(w[i + 1]))
                {
                    d[i] = (w[i + 1] * diff[i - 1] + w[i - 1] * diff[i]) / (w[i + 1] + w[i - 1]);
                }
                else
                {
                    d[i] = ((tt[i + 1] - tt[i]) * diff[i - 1] + (tt[i] - tt[i - 1]) * diff[i]) / (tt[i + 1] - tt[i - 1]);
                }
            }

            d[0] = DifferentiateThreePoint(tt[0], tt[0], xx[0], tt[1], xx[1], tt[2], xx[2]);
            d[1] = DifferentiateThreePoint(tt[1], tt[0], xx[0], tt[1], xx[1], tt[2], xx[2]);
            d[n - 2] = DifferentiateThreePoint(tt[n - 2], tt[n - 3], xx[n - 3], tt[n - 2], xx[n - 2], tt[n - 1], xx[n - 1]);
            d[n - 1] = DifferentiateThreePoint(tt[n - 1], tt[n - 3], xx[n - 3], tt[n - 2], xx[n - 2], tt[n - 1], xx[n - 1]);

            // Build Akima spline using Hermite interpolation scheme

            _hermiteSpline.InitInternal(tt, xx, d);
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
            return _hermiteSpline.Interpolate(t);
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
            return _hermiteSpline.Differentiate(t, out first, out second);
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
            return _hermiteSpline.Integrate(t);
        }

        /// <summary>
        /// Three-Point Differentiation Helper.
        /// </summary>
        /// <param name="t">The point of the differentiation.</param>
        /// <param name="t0">First Point t0.</param>
        /// <param name="x0">Value of first point x0 = x(t0).</param>
        /// <param name="t1">Second Point t0.</param>
        /// <param name="x1">Value of second point x1 = x(t1).</param>
        /// <param name="t2">Third Point t0.</param>
        /// <param name="x2">Value of third point x2 = x(t2).</param>
        static
        double
        DifferentiateThreePoint(
            double t,
            double t0,
            double x0,
            double t1,
            double x1,
            double t2,
            double x2
            )
        {
            // TODO: Optimization potential, but keep numeric stability in mind!
            t = t - t0;
            t1 = t1 - t0;
            t2 = t2 - t0;
            double a = (x2 - x0 - t2 / t1 * (x1 - x0)) / (t2 * t2 - t1 * t2);
            double b = (x1 - x0 - a * t1 * t1) / t1;
            return 2 * a * t + b;
        }
    }
}
