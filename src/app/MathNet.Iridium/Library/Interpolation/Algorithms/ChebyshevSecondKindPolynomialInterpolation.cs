//-----------------------------------------------------------------------
// <copyright file="ChebyshevSecondKindPolynomialInterpolation.cs" company="Math.NET Project">
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
    /// Barycentric Polynomial Interpolation where the given sample points are chebyshev nodes of the second kind.
    /// </summary>
    /// <remarks>
    /// This algorithm neither supports differentiation nor integration.
    /// </remarks>
    public class ChebyshevSecondKindPolynomialInterpolation :
        IInterpolationMethod
    {
        BarycentricInterpolation _barycentric;
        double _transformSummand;
        double _transformFactor;

        /// <summary>
        /// Initializes a new instance of the ChebyshevSecondKindPolynomialInterpolation class.
        /// </summary>
        public
        ChebyshevSecondKindPolynomialInterpolation()
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
        /// of the second kind in the interval [a,b].
        /// </summary>
        /// <param name="a">Left bound of the sample point interval.</param>
        /// <param name="b">Right bound of the sample point interval.</param>
        /// <param name="x">Values x(t) where t are chebyshev nodes over [a,b], i.e. x[i] = x(0.5*(b+a) + 0.5*(b-a)*cos(Pi*i/n))</param>
        public
        void
        Init(
            double a,
            double b,
            IList<double> x)
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
            double a0 = 0.0;
            double delta = Math.PI / (x.Count - 1);
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
            w[0] = 0.5;

            for(int i = 1; i < t.Length; i++)
            {
                double temps = sa - ((alpha * sa) - (beta * ca));
                double tempc = ca - ((alpha * ca) + (beta * sa));
                sa = temps;
                ca = tempc;
                sign = -sign;
                t[i] = ca;
                w[i] = sign;
            }

            w[w.Length - 1] *= 0.5;

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
            out double second)
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
        Integrate(double t)
        {
            return _barycentric.Integrate((t + _transformSummand) * _transformFactor);
        }

        /// <summary>
        /// Generate a set of chebyshev points of the second kind in the interval [a,b].
        /// These are the expected points t for the values v(t) to be provided in <see cref="Init"/>.
        /// </summary>
        /// <param name="a">Left bound of the interval.</param>
        /// <param name="b">Right bound of the interval.</param>
        /// <param name="numberOfPoints">Number of sample nodes to generate.</param>
        /// <returns>Chebyshev points (second kind) in the interval [a,b], i.e. 0.5*(b+a) + 0.5*(b-a)*cos(Pi*i/n)</returns>
        public static
        double[]
        GenerateSamplePoints(
            double a,
            double b,
            int numberOfPoints)
        {
            if(b <= a)
            {
                throw new ArgumentOutOfRangeException("b");
            }

            if(numberOfPoints < 1)
            {
                throw new ArgumentOutOfRangeException("numberOfPoints", Properties.LocalStrings.ArgumentPositive);
            }

            double transformSummand = 0.5 * (a + b);
            double transformFactor = (b - a) / 2.0;
            double angleFactor = Math.PI / (numberOfPoints - 1);

            double[] nodes = new double[numberOfPoints];
            for(int i = 0; i < numberOfPoints; i++)
            {
                nodes[i] = transformSummand + (transformFactor * Math.Cos(i * angleFactor));
            }

            return nodes;
        }
    }
}
