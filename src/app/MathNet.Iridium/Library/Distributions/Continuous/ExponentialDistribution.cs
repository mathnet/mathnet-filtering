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
    /// Pseudo-random generation of exponentially distributed deviates.
    /// </summary>
    /// 
    /// <remarks> 
    /// <para>For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Exponential_distribution">
    /// Wikipedia - Exponential distribution</a>.</para>
    /// 
    /// <para>For details of the algorithm, see
    /// <a href="http://www.library.cornell.edu/nr/">
    /// Numerical recipes in C</a> (chapter 7)</para>
    ///
    /// <para>pdf: f(x) = l*exp(-l*x); l = lambda</para>
    /// </remarks>
    public sealed class ExponentialDistribution : ContinuousDistribution
    {
        double _rate;
        double _helper1;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ExponentialDistribution()
            : base()
        {
            SetDistributionParameters(1.0);
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
        ExponentialDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ExponentialDistribution(
            double rate
            )
            : base()
        {
            SetDistributionParameters(rate);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the average rate parameter, also known as lambda.
        /// </summary>
        public double Rate
        {
            get { return _rate; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double rate
            )
        {
            if(!IsValidParameterSet(rate))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid, "rate");
            }

            _rate = rate;
            _helper1 = -1.0 / rate;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double rate
            )
        {
            return rate > 0.0;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override double Minimum
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override double Maximum
        {
            get { return double.MaxValue; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return -_helper1; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return -Math.Log(2.0) * _helper1; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _helper1 * _helper1; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 2.0; }
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
            return _rate * Math.Exp(-_rate * x);
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
            return 1.0 - Math.Exp(-_rate * x);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a exponential distributed floating point random number.
        /// </summary>
        /// <returns>A exponential distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            // Subtract random number from 1.0 to avoid Math.Log(0.0)
            return _helper1 * Math.Log(1.0 - this.RandomSource.NextDouble());
        }
        #endregion
    }
}
