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
#region Derived From: Copyright 2006 Stefan Troschütz
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
    /// Provides generation of discrete uniformly distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The discrete uniform distribution generates only discrete numbers.<br />
    /// The implementation bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28discrete%29">
    ///   Wikipedia - Uniform distribution (discrete)</a>.
    /// </remarks>
    public sealed class DiscreteUniformDistribution : DiscreteDistribution
    {
        int _a;
        int _b;
        int _n;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        DiscreteUniformDistribution()
            : base()
        {
            SetDistributionParameters(0, 1);
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
        DiscreteUniformDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0, 1);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        DiscreteUniformDistribution(
            int lowerLimit,
            int upperLimit
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
        public int LowerLimit
        {
            get { return _a; }
            set { SetDistributionParameters(value, _b); }
        }

        /// <summary>
        /// Gets or sets the upper limit parameter.
        /// To set all parameters at once consider using
        /// <see cref="SetDistributionParameters"/> instead.
        /// </summary>
        public int UpperLimit
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
            int lowerLimit,
            int upperLimit
            )
        {
            if(!IsValidParameterSet(lowerLimit, upperLimit))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _a = lowerLimit;
            _b = upperLimit;
            _n = _b - _a + 1;
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
            int lowerLimit,
            int upperLimit
            )
        {
            return lowerLimit <= upperLimit;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return _a; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override int Maximum
        {
            get { return _b; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return 0.5 * _a + 0.5 * _b; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override int Median
        {
            get { return (_a + _b) / 2; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return ((double)_n * _n - 1.0) / 12.0; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Discrete probability mass function (pmf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityMass(
            int x
            )
        {
            if(_a <= x && x <= _b)
            {
                return 1.0 / _n;
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

            if(x <= _b)
            {
                return (x - _a + 1.0) / _n;
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
        int
        NextInt32()
        {
            return this.RandomSource.Next(_a, _b + 1);
        }
        #endregion
    }
}
