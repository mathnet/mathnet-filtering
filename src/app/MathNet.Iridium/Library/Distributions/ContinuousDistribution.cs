//-----------------------------------------------------------------------
// <copyright file="ContinuousDistribution.cs" company="Math.NET Project">
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
//    Troschuetz.Random Class Library, Stefan Troschütz (stefan@troschuetz.de)
// </contribution>
//-----------------------------------------------------------------------

using System;

using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Declares common functionality for all continuous random number
    /// distributions based on a random source.
    /// </summary>
    public abstract class ContinuousDistribution :
        IContinuousGenerator,
        IContinuousProbabilityDistribution
    {
        RandomSource _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousDistribution"/> class, using a 
        /// <see cref="SystemRandomSource"/> as underlying random number generator.
        /// </summary>
        protected
        ContinuousDistribution()
            : this(new SystemRandomSource())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousDistribution"/> class, using the
        /// specified <see cref="RandomSource"/> as underlying random number generator.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="random"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        protected
        ContinuousDistribution(
            RandomSource random
            )
        {
            if(random == null)
            {
                throw new ArgumentNullException("generator", Properties.LocalStrings.ArgumentNull("generator"));
            }

            _random = random;
        }

        /// <summary>
        /// Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public abstract
        double
        NextDouble();

        /// <summary>
        /// Continuous probability density function (pdf) of this probability distribution.
        /// </summary>
        public abstract
        double
        ProbabilityDensity(
            double x
            );

        /// <summary>
        /// Continuous cumulative distribution function (cdf) of this probability distribution.
        /// </summary>
        public abstract
        double
        CumulativeDistribution(
            double x
            );

        /// <summary>
        /// Resets the random number distribution, so that it produces the same random number sequence again.
        /// </summary>
        public
        void
        Reset()
        {
            _random.Reset();
        }

        /// <summary>
        /// Lower limit of a random variable with this probability distribution.
        /// </summary>
        public abstract double Minimum
        {
            get;
        }

        /// <summary>
        /// Upper limit of a random variable with this probability distribution.
        /// </summary>
        public abstract double Maximum
        {
            get;
        }

        /// <summary>
        /// The expected value of a random variable with this probability distribution.
        /// </summary>
        public abstract double Mean
        {
            get;
        }

        /// <summary>
        /// The value separating the lower half part from the upper half part of a random variable with this probability distribution.
        /// </summary>
        public abstract double Median
        {
            get;
        }

        /// <summary>
        /// Average of the squared distances to the expected value of a random variable with this probability distribution.
        /// </summary>
        public abstract double Variance
        {
            get;
        }

        /// <summary>
        /// Measure of the asymmetry of this probability distribution.
        /// </summary>
        public abstract double Skewness
        {
            get;
        }

        /// <summary>
        /// Gets or sets a <see cref="RandomSource"/> object that can be used
        /// as underlying random number generator.
        /// </summary>
        public virtual RandomSource RandomSource
        {
            get { return _random; }
            set { _random = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the random number distribution can be reset,
        /// so that it produces the same  random number sequence again.
        /// </summary>
        public bool CanReset
        {
            get { return _random.CanReset; }
        }
    }
}
