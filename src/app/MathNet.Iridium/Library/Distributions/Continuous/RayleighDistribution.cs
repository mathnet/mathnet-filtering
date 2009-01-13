//-----------------------------------------------------------------------
// <copyright file="RayleighDistribution.cs" company="Math.NET Project">
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
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Distributions
{
    using MathNet.Numerics.RandomSources;

    /// <summary>
    /// Rayleigh distribution, including density functions and pseudo-random Rayleigh number generation.
    /// </summary>
    /// <remarks> 
    /// <para>
    /// For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Rayleigh_distribution">
    /// Wikipedia - Rayleigh distribution</a>.
    /// </para>
    /// </remarks>
    public sealed class RayleighDistribution : ContinuousDistribution
    {
        double _sigma;
        double _helper;

        #region Construction
        /// <summary>
        /// Initializes a new instance of the RayleighDistribution class,
        /// using a <see cref="SystemRandomSource"/> as underlying random number generator.
        /// </summary>
        public
        RayleighDistribution()
            : base()
        {
            SetDistributionParameters(1.0);
        }

        /// <summary>
        /// Initializes a new instance of the RayleighDistribution class,
        /// using the specified <see cref="RandomSource"/> as underlying random number generator.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="random"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public
        RayleighDistribution(RandomSource random)
            : base(random)
        {
            SetDistributionParameters(1.0);
        }

        /// <summary>
        /// Initializes a new instance of the RayleighDistribution class,
        /// using a <see cref="SystemRandomSource"/> as underlying random number generator.
        /// </summary>
        public
        RayleighDistribution(double sigma)
            : base()
        {
            SetDistributionParameters(sigma);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the sigma distribution parameter.
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(double sigma)
        {
            if(!IsValidParameterSet(sigma))
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentParameterSetInvalid, "sigma");
            }

            _sigma = sigma;
            _helper = 1 / (2 * sigma * sigma);
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(double sigma)
        {
            return sigma > 0;
        }

        /// <summary>
        /// Estimate and set all distribution parameters based on a sample set (maximum likelihood).
        /// </summary>
        /// <param name="samples">Samples of this distribution.</param>
        public
        void
        EstimateDistributionParameters(IEnumerable<double> samples)
        {
            int n = 0;
            double sumOfSquares = 0.0;

            // TODO (ruegg, 2009-01-04): consider to sort the samples by abs(s) for better numeric accuracy.
            // Unfortunately that would remove the late-evaluation goodness and we'd actually have to store all data.
            foreach(double sample in samples)
            {
                n++;
                sumOfSquares += sample * sample;
            }

            SetDistributionParameters(Math.Sqrt(sumOfSquares / (2 * n)));
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override double Minimum
        {
            get { return 0; }
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
            get { return _sigma * (Constants.SqrtPi / Constants.Sqrt2); }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return _sigma * Math.Sqrt(Math.Log(4)); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return (2 - Constants.Pi_2) * _sigma * _sigma; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 2 * Constants.SqrtPi * (Constants.Pi - 3) / Math.Pow(4 - Constants.Pi, 1.5); }
        }

        /// <summary>
        /// Continuous probability density function (pdf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityDensity(double x)
        {
            return x * Math.Exp(-(x * x * _helper)) * (2 * _helper);
        }

        /// <summary>
        /// Continuous cumulative distribution function (cdf) of this probability distribution.
        /// </summary>
        public override
        double
        CumulativeDistribution(double x)
        {
            return 1 - Math.Exp(-(x * x * _helper));
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
            return _sigma * Math.Sqrt(-2 * Math.Log(1 - this.RandomSource.NextDouble()));
        }
        #endregion
    }
}
