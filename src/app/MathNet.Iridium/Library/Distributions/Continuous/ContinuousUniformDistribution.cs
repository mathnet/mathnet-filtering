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

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Provides generation of continuous uniformly distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="ContinuousUniformDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28continuous%29">
    ///   Wikipedia - Uniform distribution (continuous)</a>.
    /// </remarks>
    public sealed class ContinuousUniformDistribution : ContinuousDistribution
    {
        double _a;
        double _b;
        double _diff;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ContinuousUniformDistribution()
            : base()
        {
            SetDistributionParameters(0.0, 1.0);
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
        ContinuousUniformDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.0, 1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ContinuousUniformDistribution(
            double lowerLimit,
            double upperLimit
            )
            : base()
        {
            SetDistributionParameters(lowerLimit, upperLimit);
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
            set { SetDistributionParameters(value, _b); }
        }

        /// <summary>
        /// Gets or sets the upper limit parameter.
        /// To set all parameters at once consider using
        /// <see cref="SetDistributionParameters"/> instead.
        /// </summary>
        public double UpperLimit
        {
            get { return _b; }
            set { SetDistributionParameters(_a, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double lowerLimit,
            double upperLimit
            )
        {
            if(!IsValidParameterSet(lowerLimit, upperLimit))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _a = lowerLimit;
            _b = upperLimit;
            _diff = _b - _a;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if lowerLimit &lt;= upperLimit; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double lowerLimit,
            double upperLimit
            )
        {
            return lowerLimit <= upperLimit;
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
            get { return 0.5 * (_a + _b); }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return 0.5 * (_a + _b); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _diff * _diff / 12.0; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 0.0; }
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
            if(_a <= x && x <= _b)
            {
                return 1.0 / _diff;
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
            if(x < _a)
            {
                return 0.0;
            }

            if(x < _b)
            {
                return (x - _a) / _diff;
            }

            return 1.0;
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a uniformly distributed floating point random number.
        /// </summary>
        /// <returns>A uniformly distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            return _a + this.RandomSource.NextDouble() * _diff;
        }
        #endregion
    }
}
