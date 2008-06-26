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
#region Derived From: Copyright 2006 Troschütz
/* 
 * Derived from the Troschuetz.Random Class Library,
 * Copyright © 2006 Stefan Troschütz (stefan@troschuetz.de)
 * 
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
 */
#endregion
#region Derived From: Copyright 2001 Maurer
// boost random/triangle_distribution.hpp header file
//
// Copyright Jens Maurer 2000-2001
// Distributed under the Boost Software License, Version 1.0. (See
// accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)
//
// See http://www.boost.org for most recent version including documentation.
//
// $Id: triangle_distribution.hpp,v 1.11 2004/07/27 03:43:32 dgregor Exp $
//
// Revision history
// 2001-02-18  moved to individual header files
#endregion

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Provides generation of triangular distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="TriangularDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Triangular_distribution">Wikipedia - Triangular distribution</a>
    ///   and the implementation in the <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    /// </remarks>
    public sealed class TriangularDistribution : ContinuousDistribution
    {
        double _a;
        double _b;
        double _c;
        double _diff, _lowerPart, _upperPart, helper3, helper4;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        TriangularDistribution()
            : base()
        {
            SetDistributionParameters(0.0, 1.0, 0.5);
        }

        /// <summary>
        /// Initializes a new instance, using the specified <see cref="RandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="random"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public
        TriangularDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.0, 1.0, 0.5);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        TriangularDistribution(
            double lowerLimit,
            double upperLimit,
            double center
            )
            : base()
        {
            SetDistributionParameters(lowerLimit, upperLimit, center);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the lower limit parameter.
        /// To set all parameters at once consider using
        /// <see cref="SetDistributionParameters"/> instead.
        /// </summary>
        public double LowerLimit
        {
            get { return _a; }
            set { SetDistributionParameters(value, _b, _c); }
        }

        /// <summary>
        /// Gets or sets the upper limit parameter.
        /// To set all parameters at once consider using
        /// <see cref="SetDistributionParameters"/> instead.
        /// </summary>
        public double UpperLimit
        {
            get { return _b; }
            set { SetDistributionParameters(_a, value, _c); }
        }

        /// <summary>
        /// Gets or sets the center parameter.
        /// To set all parameters at once consider using
        /// <see cref="SetDistributionParameters"/> instead.
        /// </summary>
        public double Center
        {
            get { return _c; }
            set { SetDistributionParameters(_a, _b, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double lowerLimit,
            double upperLimit,
            double center
            )
        {
            if(!IsValidParameterSet(lowerLimit, upperLimit, center))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _a = lowerLimit;
            _b = upperLimit;
            _c = center;

            _diff = upperLimit - lowerLimit;
            _lowerPart = center - lowerLimit;
            _upperPart = upperLimit - center;
            this.helper3 = Math.Sqrt(this._lowerPart * this._diff);
            this.helper4 = Math.Sqrt(upperLimit - center);
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if both shape and rate are greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double lowerLimit,
            double upperLimit,
            double center
            )
        {
            return lowerLimit < upperLimit && lowerLimit <= center && center <= upperLimit;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override double Minimum
        {
            get { return _a; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override double Maximum
        {
            get { return _b; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return (_a + _b + _c) / 3.0; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get
            {
                if(_c >= 0.5 * _diff)
                {
                    return _a + Math.Sqrt(0.5 * _diff * _lowerPart);
                }

                return _b - Math.Sqrt(0.5 * _diff * _upperPart);
            }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return (_a * _a + _b * _b + _c * _c - _a * _b - _a * _c - _b * _c) / 18.0; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get
            {
                double a = Constants.Sqrt2 * (_a + _b - 2 * _c) * (2 * _a - _b - _c) * (_a - 2 * _b + _c);
                double b = 5.0 * Math.Pow(_a * _a + _b * _b + _c * _c - _a * _b - _a * _c - _b * _c, 1.5);
                return a / b;
            }
        }

        /// <summary>
        /// Continuous probability density function (pdf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityDensity(
            double x
            )
        {
            if(x <= _a)
            {
                return 0.0;
            }

            if(x <= _c)
            {
                return 2 * (x - _a) / (_diff * _lowerPart);
            }

            if(x < _b)
            {
                return 2 * (_b - x) / (_diff * _upperPart);
            }

            return 0.0;
        }

        /// <summary>
        /// Continuous cumulative distribution function (cdf) of this probability distribution.
        /// </summary>
        public override
        double
        CumulativeDistribution(
            double x
            )
        {
            if(x <= _a)
            {
                return 0.0;
            }

            if(x <= _c)
            {
                double diff = x - _a;
                return diff * diff / (_diff * _lowerPart);
            }

            if(x < _b)
            {
                double diff = _b - x;
                return 1.0 - diff * diff / (_diff * _upperPart);
            }

            return 1.0;
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a triangular distributed floating point random number.
        /// </summary>
        /// <returns>A triangular distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double genNum = this.RandomSource.NextDouble();
            if(genNum <= _lowerPart / _diff)
            {
                return _a + Math.Sqrt(genNum) * this.helper3;
            }
            else
            {
                return _b - Math.Sqrt(genNum * _diff - _lowerPart) * this.helper4;
            }
        }
        #endregion
    }
}
