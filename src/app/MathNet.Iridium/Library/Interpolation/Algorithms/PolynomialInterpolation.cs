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
        /// Create a neville polynomial interpolation algorithm instance.
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
            double[] x = new double[_x.Count];
            _x.CopyTo(x, 0);

            for(int level = 1; level < x.Length; level++)
            {
                for(int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - _t[i + level];
                    double ho = _t[i] - t;
                    double den = _t[i] - _t[i + level];
                    x[i] = (hp * x[i] + ho * x[i + 1]) / den;
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
            out double second
            )
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
                    d2x[i] = (hp * d2x[i] + ho * d2x[i + 1] + 2 * dx[i] - 2 * dx[i + 1]) / den;
                    dx[i] = (hp * dx[i] + x[i] + ho * dx[i + 1] - x[i + 1]) / den;
                    x[i] = (hp * x[i] + ho * x[i + 1]) / den;
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
        Integrate(
            double t
            )
        {
            throw new NotSupportedException();
        }
    }
}
