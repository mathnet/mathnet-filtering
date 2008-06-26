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
    /// Cubic Spline Interpolation Algorithm with continuous first and second derivatives.
    /// </summary>
    public class CubicSplineInterpolation :
        IInterpolationMethod
    {
        CubicHermiteSplineInterpolation _hermiteSpline;

        /// <summary>
        /// Create an interpolation algorithm instance.
        /// </summary>
        public
        CubicSplineInterpolation()
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
            Init(
                t,
                x,
                SplineBoundaryCondition.SecondDerivative,
                0.0,
                SplineBoundaryCondition.SecondDerivative,
                0.0
                );
        }

        /// <summary>
        /// Initialize the interpolation method with the given samples.
        /// </summary>
        /// <param name="t">Points t</param>
        /// <param name="x">Values x(t)</param>
        /// <param name="leftBoundaryCondition">Condition of the left boundary.</param>
        /// <param name="leftBoundary">Left boundary value. Ignored in the parabolic case.</param>
        /// <param name="rightBoundaryCondition">Condition of the right boundary.</param>
        /// <param name="rightBoundary">Right boundary value. Ignored in the parabolic case.</param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x,
            SplineBoundaryCondition leftBoundaryCondition,
            double leftBoundary,
            SplineBoundaryCondition rightBoundaryCondition,
            double rightBoundary
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

            if(t.Count < 2)
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

            // normalize special cases
            if((n == 2)
                && (leftBoundaryCondition == SplineBoundaryCondition.ParabolicallyTerminated)
                && (rightBoundaryCondition == SplineBoundaryCondition.ParabolicallyTerminated))
            {
                leftBoundaryCondition = SplineBoundaryCondition.SecondDerivative;
                leftBoundary = 0d;
                rightBoundaryCondition = SplineBoundaryCondition.SecondDerivative;
                rightBoundary = 0d;
            }

            if(leftBoundaryCondition == SplineBoundaryCondition.Natural)
            {
                leftBoundaryCondition = SplineBoundaryCondition.SecondDerivative;
                leftBoundary = 0d;
            }

            if(rightBoundaryCondition == SplineBoundaryCondition.Natural)
            {
                rightBoundaryCondition = SplineBoundaryCondition.SecondDerivative;
                rightBoundary = 0d;
            }

            double[] a1 = new double[n];
            double[] a2 = new double[n];
            double[] a3 = new double[n];
            double[] b = new double[n];

            // Left Boundary
            switch(leftBoundaryCondition)
            {
                case SplineBoundaryCondition.ParabolicallyTerminated:
                    a1[0] = 0;
                    a2[0] = 1;
                    a3[0] = 1;
                    b[0] = 2 * (xx[1] - xx[0]) / (tt[1] - tt[0]);
                    break;
                case SplineBoundaryCondition.FirstDerivative:
                    a1[0] = 0;
                    a2[0] = 1;
                    a3[0] = 0;
                    b[0] = leftBoundary;
                    break;
                case SplineBoundaryCondition.SecondDerivative:
                    a1[0] = 0;
                    a2[0] = 2;
                    a3[0] = 1;
                    b[0] = 3 * (xx[1] - xx[0]) / (tt[1] - tt[0]) - 0.5 * leftBoundary * (tt[1] - tt[0]);
                    break;
                default:
                    throw new NotSupportedException(Properties.Resources.InvalidLeftBoundaryCondition);
            }

            // Central Conditions
            for(int i = 1; i < tt.Length - 1; i++)
            {
                a1[i] = tt[i + 1] - tt[i];
                a2[i] = 2 * (tt[i + 1] - tt[i - 1]);
                a3[i] = tt[i] - tt[i - 1];
                b[i] = 3 * (xx[i] - xx[i - 1]) / (tt[i] - tt[i - 1]) * (tt[i + 1] - tt[i]) + 3 * (xx[i + 1] - xx[i]) / (tt[i + 1] - tt[i]) * (tt[i] - tt[i - 1]);
            }

            // Right Boundary
            switch(rightBoundaryCondition)
            {
                case SplineBoundaryCondition.ParabolicallyTerminated:
                    a1[n - 1] = 1;
                    a2[n - 1] = 1;
                    a3[n - 1] = 0;
                    b[n - 1] = 2 * (xx[n - 1] - xx[n - 2]) / (tt[n - 1] - tt[n - 2]);
                    break;
                case SplineBoundaryCondition.FirstDerivative:
                    a1[n - 1] = 0;
                    a2[n - 1] = 1;
                    a3[n - 1] = 0;
                    b[n - 1] = rightBoundary;
                    break;
                case SplineBoundaryCondition.SecondDerivative:
                    a1[n - 1] = 1;
                    a2[n - 1] = 2;
                    a3[n - 1] = 0;
                    b[n - 1] = 3 * (xx[n - 1] - xx[n - 2]) / (tt[n - 1] - tt[n - 2]) + 0.5 * rightBoundary * (tt[n - 1] - tt[n - 2]);
                    break;
                default:
                    throw new NotSupportedException(Properties.Resources.InvalidRightBoundaryCondition);
            }

            // Build Spline
            double[] dd = SolveTridiagonal(a1, a2, a3, b);
            _hermiteSpline.InitInternal(tt, xx, dd);
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
        /// Tridiagonal Solve Helper.
        /// </summary>
        /// <param name="a">a-vector[n].</param>
        /// <param name="b">b-vector[n], will be modified by this function.</param>
        /// <param name="c">c-vector[n].</param>
        /// <param name="d">d-vector[n], will be modified by this function.</param>
        /// <returns>x-vector[n]</returns>
        static
        double[]
        SolveTridiagonal(
            double[] a,
            double[] b,
            double[] c,
            double[] d
            )
        {
            double[] x = new double[a.Length];

            for(int k = 1; k < a.Length; k++)
            {
                double t = a[k] / b[k - 1];
                b[k] = b[k] - t * c[k - 1];
                d[k] = d[k] - t * d[k - 1];
            }

            x[x.Length - 1] = d[d.Length - 1] / b[b.Length - 1];
            for(int k = x.Length - 2; k >= 0; k--)
            {
                x[k] = (d[k] - c[k] * x[k + 1]) / b[k];
            }

            return x;
        }
    }
}
