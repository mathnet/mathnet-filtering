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
    /// Provides generation of chi-square distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="ChiSquareDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Chi-square_distribution">Wikipedia - Chi-square distribution</a>.
    /// </remarks>
    public sealed class ChiSquareDistribution : ContinuousDistribution
    {
        int _degreesOfFreedom;
        double _lngammaDegreesOfFreedomHalf;

        StandardDistribution _standard;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ChiSquareDistribution()
            : base()
        {
            _standard = new StandardDistribution(this.RandomSource);
            SetDistributionParameters(1);
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
        ChiSquareDistribution(
            RandomSource random
            )
            : base(random)
        {
            _standard = new StandardDistribution(random);
            SetDistributionParameters(1);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ChiSquareDistribution(
            int degreesOfFreedom
            )
            : base()
        {
            _standard = new StandardDistribution(this.RandomSource);
            SetDistributionParameters(degreesOfFreedom);
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
        /// Gets or sets the degrees of freedom (the number of standard distributed random variables) parameter.
        /// </summary>
        public int DegreesOfFreedom
        {
            get { return _degreesOfFreedom; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            int degreesOfFreedom
            )
        {
            if(!IsValidParameterSet(degreesOfFreedom))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid, "degreesOfFreedom");
            }

            _degreesOfFreedom = degreesOfFreedom;
            _lngammaDegreesOfFreedomHalf = Fn.GammaLn(0.5 * degreesOfFreedom);
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if degreesOfFreedom is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            int degreesOfFreedom
            )
        {
            return degreesOfFreedom > 0.0;
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
            get { return _degreesOfFreedom; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return _degreesOfFreedom - 2.0 / 3.0; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return 2.0 * _degreesOfFreedom; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return Math.Sqrt(8.0 / _degreesOfFreedom); }
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
            return Math.Exp(
                (-0.5 * _degreesOfFreedom) * Constants.Ln2
                + (0.5 * _degreesOfFreedom + 1.0) * Math.Log(x)
                - (0.5 * x)
                - _lngammaDegreesOfFreedomHalf
                );
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
            return Fn.GammaRegularized(0.5 * _degreesOfFreedom, 0.5 * x);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a chi-square distributed floating point random number.
        /// </summary>
        /// <returns>A chi-square distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double sum = 0.0;
            for(int i = 0; i < _degreesOfFreedom; i++)
            {
                double std = _standard.NextDouble();
                sum += std * std;
            }

            return sum;
        }
        #endregion
    }
}
