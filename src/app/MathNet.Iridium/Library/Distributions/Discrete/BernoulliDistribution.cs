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
    /// Provides generation of bernoulli distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The bernoulli distribution generates only discrete numbers.<br />
    /// The implementation bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Bernoulli_distribution">Wikipedia - Bernoulli distribution</a>.
    /// </remarks>
    public sealed class BernoulliDistribution : DiscreteDistribution
    {
        double _p;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BernoulliDistribution()
            : base()
        {
            SetDistributionParameters(0.5);
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
        BernoulliDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.5);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BernoulliDistribution(
            double probabilityOfSuccess
            )
            : base()
        {
            SetDistributionParameters(probabilityOfSuccess);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the success probability parameter.
        /// </summary>
        public double ProbabilityOfSuccess
        {
            get { return _p; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double probabilityOfSuccess
            )
        {
            if(!IsValidParameterSet(probabilityOfSuccess))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid, "probabilityOfSuccess");
            }

            _p = probabilityOfSuccess;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if value is greater than or equal to 0.0, and less than or equal to 1.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double probabilityOfSuccess
            )
        {
            return probabilityOfSuccess >= 0.0 && probabilityOfSuccess <= 1.0;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override int Maximum
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return _p; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// Throws <see cref="NotSupportedException"/> since
        /// the value is not defined for this distribution.
        /// </summary>
        /// <exception cref="NotSupportedException">Always.</exception>
        public override int Median
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _p * (1.0 - _p); }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get
            {
                double q = 1 - _p;
                return (q - _p) / Math.Sqrt(q * _p);
            }
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
            if(x == 1)
            {
                return _p;
            }

            if(x == 0)
            {
                return 1 - _p;
            }

            return 0;
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
            if(x < 0)
            {
                return 0;
            }

            if(x < 1)
            {
                return 1 - _p;
            }

            return 1;
        }

        #endregion

        #region Generator
        /// <summary>
        /// Returns a bernoulli distributed random number.
        /// </summary>
        /// <returns>A bernoulli distributed 32-bit signed integer.</returns>
        public override
        int
        NextInt32()
        {
            if(this.RandomSource.NextDouble() < _p)
            {
                return 1;
            }

            return 0;
        }
        #endregion
    }
}
