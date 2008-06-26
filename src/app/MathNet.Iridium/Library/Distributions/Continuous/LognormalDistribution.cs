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
// boost random/lognormal_distribution.hpp header file
//
// Copyright Jens Maurer 2000-2001
// Distributed under the Boost Software License, Version 1.0. (See
// accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)
//
// See http://www.boost.org for most recent version including documentation.
//
// $Id: lognormal_distribution.hpp,v 1.16 2004/07/27 03:43:32 dgregor Exp $
//
// Revision history
// 2001-02-18  moved to individual header files
#endregion

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Provides generation of lognormal distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="LognormalDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Log-normal_distribution">Wikipedia - Lognormal Distribution</a> and
    ///   the implementation in the <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    /// </remarks>
    public sealed class LognormalDistribution : ContinuousDistribution
    {
        double _mu;
        double _sigma;
        double _sigma2;

        StandardDistribution _standard;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        LognormalDistribution()
            : base()
        {
            _standard = new StandardDistribution(this.RandomSource);
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
        LognormalDistribution(
            RandomSource random
            )
            : base(random)
        {
            _standard = new StandardDistribution(random);
            SetDistributionParameters(0.0, 1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        LognormalDistribution(
            double mu,
            double sigma
            )
            : base()
        {
            _standard = new StandardDistribution(this.RandomSource);
            SetDistributionParameters(mu, sigma);
        }
        #endregion

        /// <summary>
        /// Gets or sets a <see cref="RandomSource"/> object that can be used
        /// as underlying random number generator.
        /// </summary>
        public override RandomSource RandomSource
        {
            get
            {
                return base.RandomSource;
            }

            set
            {
                base.RandomSource = value;
                _standard.RandomSource = value;
            }
        }

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the mu parameter.
        /// </summary>
        public double Mu
        {
            get { return _mu; }
            set { SetDistributionParameters(value, _sigma); }
        }

        /// <summary>
        /// Gets or sets the sigma parameter.
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set { SetDistributionParameters(_mu, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double mu,
            double sigma
            )
        {
            if(!IsValidParameterSet(mu, sigma))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _mu = mu;
            _sigma = sigma;
            _sigma2 = sigma * sigma;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if sigma is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double mu,
            double sigma
            )
        {
            return sigma > 0.0;
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
            get { return Math.Exp(_mu + 0.5 * _sigma2); }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return Math.Exp(_mu); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return (Math.Exp(_sigma2) - 1.0) * Math.Exp(_mu + _mu + _sigma2); }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get
            {
                double expsigma2 = Math.Exp(_sigma2);
                return (expsigma2 + 2.0) * Math.Sqrt(expsigma2 - 1);
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
            double a = (Math.Log(x) - _mu) / _sigma;
            return Math.Exp(-0.5 * a * a) / (x * _sigma * Constants.Sqrt2Pi);
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
            return 0.5 * (1.0 + Fn.Erf((Math.Log(x) - _mu) / (_sigma * Constants.Sqrt2)));
        }

        /// <summary>
        /// Inverse of the continuous cumulative distribution function of this probability distribution.
        /// </summary>
        /// <seealso cref="LognormalDistribution.CumulativeDistribution"/>
        public
        double
        InverseCumulativeDistribution(
            double x
            )
        {
            return Math.Exp(_sigma * Constants.Sqrt2 * Fn.ErfInverse(2.0 * x - 1.0) + _mu);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a lognormal distributed floating point random number.
        /// </summary>
        /// <returns>A lognormal distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            return Math.Exp(_standard.NextDouble() * _sigma + _mu);
        }
        #endregion
    }
}
