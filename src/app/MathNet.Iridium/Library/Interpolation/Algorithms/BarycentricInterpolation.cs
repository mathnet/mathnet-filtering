//-----------------------------------------------------------------------
// <copyright file="BarycentricInterpolation.cs" company="Math.NET Project">
//    Copyright (c) 2002-2008, Christoph Rüegg.
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
    /// Barycentric Interpolation Algorithm.
    /// </summary>
    /// <remarks>
    /// This algorithm neither supports differentiation nor interation.
    /// </remarks>
    public class BarycentricInterpolation : 
        IInterpolationMethod
    {
        IList<double> _t;
        IList<double> _x;
        IList<double> _w;

        /// <summary>
        /// Initializes a new instance of the BarycentricInterpolation class.
        /// </summary>
        public
        BarycentricInterpolation()
        {
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return false; }
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
        /// <param name="w">Barycentric weights w(t)</param>
        public
        void
        Init(
            IList<double> t,
            IList<double> x,
            IList<double> w)
        {
            if(null == t)
            {
                throw new ArgumentNullException("t");
            }

            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(null == w)
            {
                throw new ArgumentNullException("w");
            }

            if(t.Count < 1)
            {
                throw new ArgumentOutOfRangeException("t");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentVectorsSameLengths);
            }

            if(t.Count != w.Count)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentVectorsSameLengths);
            }

            _t = t;
            _x = x;
            _w = w;
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
            /*
            First, decide: should we use "safe" formula (guarded
            against overflow) or fast one?
            */

            if(_t.Count == 1)
            {
                return _x[0];
            }

            int j = 0;
            double s = t - _t[0];
            for(int i = 1; i < _t.Count; i++)
            {
                if(Math.Abs(t - _t[i]) < Math.Abs(s))
                {
                    s = t - _t[i];
                    j = i;
                }
            }

            if(Number.AlmostZero(s))
            {
                return _x[j];
            }

            if(Math.Abs(s) > 1e-150)
            {
                // use fast formula
                j = -1;
                s = 1.0;
            }

            // Calculate using safe or fast barycentric formula
            double s1 = 0;
            double s2 = 0;
            for(int i = 0; i < _t.Count; i++)
            {
                if(i != j)
                {
                    double v = s * _w[i] / (t - _t[i]);
                    s1 = s1 + (v * _x[i]);
                    s2 = s2 + v;
                }
                else
                {
                    double v = _w[i];
                    s1 = s1 + (v * _x[i]);
                    s2 = s2 + v;
                }
            }

            return s1 / s2;
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
            throw new NotSupportedException();
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
