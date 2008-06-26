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
using MathNet.Numerics.RandomSources;
using MathNet.Numerics.Statistics;

namespace MathNet.Numerics.Distributions
{

    /// <summary>
    /// Pseudo-random generation of normal distributed deviates.
    /// </summary>
    /// 
    /// <remarks> 
    /// <para>For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Normal_distribution">
    /// Wikipedia - Normal distribution</a>.</para>
    /// 
    /// <para>This implementation is based on the <i>Box-Muller</i> algorithm
    /// for generating random deviates with a normal distribution.</para>
    /// 
    /// <para>For details of the algorithm, see
    /// <a href="http://www.library.cornell.edu/nr/">
    /// Numerical recipes in C</a> (chapter 7)</para>
    ///
    /// <para>pdf: f(x) = 1/(s*sqrt(2*Pi))*exp(-(x-m)^2/(2*s^2)); m = mu (location), s = sigma (scale)</para>
    /// </remarks>
    public sealed class NormalDistribution : ContinuousDistribution
    {
        double _mu;
        double _sigma;

        StandardDistribution _standard;

        #region Construction

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        NormalDistribution()
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
        NormalDistribution(
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
        NormalDistribution(
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
        /// Gets or sets the sigma (standard deviation) parameter.
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

        /// <summary>
        /// Estimate and set all distribution parameters based on a sample set.
        /// </summary>
        /// <param name="samples">Samples of this distribution.</param>
        public
        void
        EstimateDistributionParameters(
            IEnumerable<double> samples
            )
        {
            Accumulator accumulator = new Accumulator(samples);
            SetDistributionParameters(accumulator.Mean, accumulator.Sigma);
        }

        #endregion

        #region Distribution Properties

        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override double Minimum
        {
            get { return double.MinValue; }
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
            get { return _mu; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return _mu; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _sigma * _sigma; }
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
            double xmu = x - _mu;
            return Constants.InvSqrt2Pi / _sigma * Math.Exp(xmu * xmu / (-2.0 * _sigma * _sigma));
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
            return 0.5 * (1.0 + Fn.Erf((x - _mu) / (_sigma * Constants.Sqrt2)));
        }

        /// <summary>
        /// Inverse of the continuous cumulative distribution function of this probability distribution.
        /// </summary>
        /// <seealso cref="NormalDistribution.CumulativeDistribution"/>
        public
        double
        InverseCumulativeDistribution(
            double x
            )
        {
            return _sigma * Constants.Sqrt2 * Fn.ErfInverse(2.0 * x - 1.0) + _mu;
        }

        #endregion

        #region Generator

        /// <summary>
        /// Returns a normal/gaussian distributed floating point random number.
        /// </summary>
        /// <returns>A normal distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            return _mu + _standard.NextDouble() * _sigma;
        }

        #endregion
    }
}
