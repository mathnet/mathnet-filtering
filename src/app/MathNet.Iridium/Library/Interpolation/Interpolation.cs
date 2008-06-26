#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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
using MathNet.Numerics.Interpolation.Algorithms;

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Interpolation Facade.
    /// </summary>
    /// <remarks>
    /// For most cases it is recommended to use the default scheme, see <see cref="Create"/>.
    /// </remarks>
    public static class Interpolation
    {
        /// <summary>
        /// Create a rational pole-free interpolation based on arbitrary points. This is the default interpolation scheme.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        Create(
            IList<double> points,
            IList<double> values
            )
        {
            RationalPoleFreeInterpolation method = new RationalPoleFreeInterpolation();
            method.Init(points, values);
            return method;
        }

        /// <summary>
        /// Create a polynomial (neville) interpolation based on arbitrary points.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreatePolynomial(
            IList<double> points,
            IList<double> values
            )
        {
            PolynomialInterpolation method = new PolynomialInterpolation();
            method.Init(points, values);
            return method;
        }

        /// <summary>
        /// Create a rational (with poles; Bulirsch &amp; Stoer) interpolation based on arbitrary points.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateRational(
            IList<double> points,
            IList<double> values
            )
        {
            RationalInterpolation method = new RationalInterpolation();
            method.Init(points, values);
            return method;
        }

        /// <summary>
        /// Create a linear spline interpolation based on arbitrary points.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateLinearSpline(
            IList<double> points,
            IList<double> values
            )
        {
            LinearSplineInterpolation method = new LinearSplineInterpolation();
            method.Init(points, values);
            return method;
        }

        /// <summary>
        /// Create a cubic spline interpolation based on arbitrary points, with specified boundary conditions.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <param name="leftBoundaryCondition">Condition of the left boundary.</param>
        /// <param name="leftBoundary">Left boundary value. Ignored in the parabolic case.</param>
        /// <param name="rightBoundaryCondition">Condition of the right boundary.</param>
        /// <param name="rightBoundary">Right boundary value. Ignored in the parabolic case.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateCubicSpline(
            IList<double> points,
            IList<double> values,
            SplineBoundaryCondition leftBoundaryCondition,
            double leftBoundary,
            SplineBoundaryCondition rightBoundaryCondition,
            double rightBoundary
            )
        {
            CubicSplineInterpolation method = new CubicSplineInterpolation();
            method.Init(
                points,
                values,
                leftBoundaryCondition,
                leftBoundary,
                rightBoundaryCondition,
                rightBoundary
                );
            return method;
        }

        /// <summary>
        /// Create a natural cubic spline interpolation based on arbitrary points.
        /// Natural splines are cubic splines with zero second derivative at the boundaries (i.e. straigth lines).
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateNaturalCubicSpline(
            IList<double> points,
            IList<double> values
            )
        {
            CubicSplineInterpolation method = new CubicSplineInterpolation();
            method.Init(
                points,
                values
                );
            return method;
        }

        /// <summary>
        /// Create an akima cubic spline interpolation based on arbitrary points.
        /// Akima splines are cubic splines which are stable to outliers.
        /// </summary>
        /// <param name="points">The sample points t. Supports both lists and arrays.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateAkimaCubicSpline(
            IList<double> points,
            IList<double> values
            )
        {
            AkimaSplineInterpolation method = new AkimaSplineInterpolation();
            method.Init(points, values);
            return method;
        }


        /// <summary>
        /// Create a polynomial interpolation based on equidistant sample points.
        /// </summary>
        /// <param name="leftBound">The leftmost (smallest) sample point t.</param>
        /// <param name="rightBound">The rightmost (biggest) sample point t.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateOnEquidistantPoints(
            double leftBound,
            double rightBound,
            IList<double> values
            )
        {
            EquidistantPolynomialInterpolation method = new EquidistantPolynomialInterpolation();
            method.Init(leftBound, rightBound, values);
            return method;
        }

        /// <summary>
        /// Create a polynomial interpolation based on chebyshev (first kind) points, that is, "t(i) = 0.5*(b+a) + 0.5*(b-a)*cos(Pi*(2*i+1)/(2*n))".
        /// </summary>
        /// <param name="leftBound">The left (smallest) sample point t bound.</param>
        /// <param name="rightBound">The right (biggest) sample point t bound.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateOnChebyshevFirstKindPoints(
            double leftBound,
            double rightBound,
            IList<double> values
            )
        {
            ChebyshevFirstKindPolynomialInterpolation method = new ChebyshevFirstKindPolynomialInterpolation();
            method.Init(leftBound, rightBound, values);
            return method;
        }

        /// <summary>
        /// Generate a set of chebyshev points of the first kind in the interval [a,b].
        /// These are the expected points t for the values v(t) to be provided in <see cref="CreateOnChebyshevFirstKindPoints"/>.
        /// </summary>
        /// <param name="a">Left bound of the interval.</param>
        /// <param name="b">Right bound of the interval.</param>
        /// <param name="numberOfPoints">Number of sample nodes to generate.</param>
        /// <returns>Chebyshev points (first kind) in the interval [a,b], i.e. 0.5*(b+a) + 0.5*(b-a)*cos(Pi*(2*i+1)/(2*n))</returns>
        public static
        double[]
        GenerateChebyshevFirstKindSamplePoints(
            double a,
            double b,
            int numberOfPoints
            )
        {
            return ChebyshevFirstKindPolynomialInterpolation.GenerateSamplePoints(a, b, numberOfPoints);
        }

        /// <summary>
        /// Create a polynomial interpolation based on chebyshev (second kind) points, that is, "t(i) = 0.5*(b+a) + 0.5*(b-a)*cos(Pi*i/n)".
        /// </summary>
        /// <param name="leftBound">The left (smallest) sample point t bound.</param>
        /// <param name="rightBound">The right (biggest) sample point t bound.</param>
        /// <param name="values">The sample point values x(t). Supports both lists and arrays.</param>
        /// <returns>
        /// An interpolation scheme optimized for the given sample points and values,
        /// which can then be used to compute interpolations and extrapolations
        /// on arbitrary points.
        /// </returns>
        public static
        IInterpolationMethod
        CreateOnChebyshevSecondKindPoints(
            double leftBound,
            double rightBound,
            IList<double> values
            )
        {
            ChebyshevSecondKindPolynomialInterpolation method = new ChebyshevSecondKindPolynomialInterpolation();
            method.Init(leftBound, rightBound, values);
            return method;
        }

        /// <summary>
        /// Generate a set of chebyshev points of the second kind in the interval [a,b].
        /// These are the expected points t for the values v(t) to be provided in <see cref="CreateOnChebyshevSecondKindPoints"/>.
        /// </summary>
        /// <param name="a">Left bound of the interval.</param>
        /// <param name="b">Right bound of the interval.</param>
        /// <param name="numberOfPoints">Number of sample nodes to generate.</param>
        /// <returns>Chebyshev points (second kind) in the interval [a,b], i.e. 0.5*(b+a) + 0.5*(b-a)*cos(Pi*i/n)</returns>
        public static
        double[]
        GenerateChebyshevSecondKindSamplePoints(
            double a,
            double b,
            int numberOfPoints
            )
        {
            return ChebyshevSecondKindPolynomialInterpolation.GenerateSamplePoints(a, b, numberOfPoints);
        }
    }
}
