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
    /// Barycentric Polynomial Interpolation where the given sample points are chebyshev nodes of the first kind.
    /// </summary>
    /// <remarks>
    /// This algorithm neither supports differentiation nor integration.
    /// </remarks>
    public class ChebyshevFirstKindPolynomialInterpolation :
        IInterpolationMethod
    {
        BarycentricInterpolation _barycentric;
        double _transformSummand;
        double _transformFactor;

        /// <summary>
        /// Create an interpolation algorithm instance.
        /// </summary>
        public
        ChebyshevFirstKindPolynomialInterpolation()
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
        /// Initialize the interpolation method with the given samples on chebyshev nodes
        /// of the first kind in the interval [a,b].
        /// </summary>
        /// <param name="a">Left bound of the sample point interval.</param>
        /// <param name="b">Right bound of the sample point interval.</param>
        /// <param name="x">Values x(t) where t are chebyshev nodes over [a,b], i.e. x[i] = x(0.5*(b+a) + 0.5*(b-a)*cos(Pi*(2*i+1)/(2*n)))</param>
        public
        void
        Init(
            double a,
            double b,
            IList<double> x
            )
        {
            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(x.Count < 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if(b <= a)
            {
                throw new ArgumentOutOfRangeException("b");
            }

            _transformSummand = -0.5 * (a + b);
            _transformFactor = 2.0 / (b - a);

            // trigonometric recurrence
            double a0 = Math.PI / (2 * (x.Count - 1) + 2);
            double delta = 2 * Math.PI / (2 * (x.Count - 1) + 2);
            double alpha = Math.Sin(delta / 2);
            alpha = 2 * alpha * alpha;
            double beta = Math.Sin(delta);

            double ca = Math.Cos(a0);
            double sa = Math.Sin(a0);
            double sign = 1.0;

            // construct chebyshev points and barycentric weights
            double[] t = new double[x.Count];
            double[] w = new double[x.Count];
            t[0] = ca;
            w[0] = sa;
            for(int i = 1; i < t.Length; i++)
            {
                double temps = sa - (alpha * sa - beta * ca);
                double tempc = ca - (alpha * ca + beta * sa);
                sa = temps;
                ca = tempc;
                sign = -sign;
                t[i] = ca;
                w[i] = sign * sa;
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
            return _barycentric.Interpolate((t + _transformSummand) * _transformFactor);
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
            return _barycentric.Differentiate((t + _transformSummand) * _transformFactor, out first, out second);
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
            return _barycentric.Integrate((t + _transformSummand) * _transformFactor);
        }

        /// <summary>
        /// Generate a set of chebyshev points of the first kind in the interval [a,b].
        /// These are the expected points t for the values v(t) to be provided in <see cref="Init"/>.
        /// </summary>
        /// <param name="a">Left bound of the interval.</param>
        /// <param name="b">Right bound of the interval.</param>
        /// <param name="numberOfPoints">Number of sample nodes to generate.</param>
        /// <returns>Chebyshev points (first kind) in the interval [a,b], i.e. 0.5*(b+a) + 0.5*(b-a)*cos(Pi*(2*i+1)/(2*n))</returns>
        public static
        double[]
        GenerateSamplePoints(
            double a,
            double b,
            int numberOfPoints
            )
        {
            if(b <= a)
            {
                throw new ArgumentOutOfRangeException("b");
            }

            if(numberOfPoints < 1)
            {
                throw new ArgumentOutOfRangeException("numberOfPoints", Properties.Resources.ArgumentPositive);
            }

            double transformSummand = 0.5 * (a + b);
            double transformFactor = (b - a) / 2.0;
            double angleFactor = Math.PI / (2 * (numberOfPoints - 1) + 2);

            double[] nodes = new double[numberOfPoints];
            for(int i = 0; i < numberOfPoints; i++)
            {
                nodes[i] = transformSummand + transformFactor * Math.Cos((2 * i + 1) * angleFactor);
            }

            return nodes;
        }
    }
}
