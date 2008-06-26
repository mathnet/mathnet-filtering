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
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Provides generation of student's t-distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="StudentsTDistribution"/> type bases upon information presented on
    /// <a href="http://en.wikipedia.org/wiki/Student's_t-distribution">Wikipedia - Student's t-distribution</a>
    /// </remarks>
    public sealed class StudentsTDistribution : ContinuousDistribution
    {
        int _degreesOfFreedom;

        double _factor;
        double _exponent;
        double _summand;

        StandardDistribution _standardDistribution;
        ChiSquareDistribution _chiSquareDistribution;

        #region Construction

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        StudentsTDistribution()
            : base()
        {
            _standardDistribution = new StandardDistribution(RandomSource);
            _chiSquareDistribution = new ChiSquareDistribution(RandomSource);
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
        StudentsTDistribution(
            RandomSource random
            )
            : base(random)
        {
            _standardDistribution = new StandardDistribution(RandomSource);
            _chiSquareDistribution = new ChiSquareDistribution(RandomSource);
            SetDistributionParameters(1);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        /// <param name="degreesOfFreedom">nu-parameter</param>
        public
        StudentsTDistribution(
            int degreesOfFreedom
            )
            : base()
        {
            _standardDistribution = new StandardDistribution(RandomSource);
            _chiSquareDistribution = new ChiSquareDistribution(RandomSource);
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
                _standardDistribution.RandomSource = value;
                _chiSquareDistribution.RandomSource = value;
            }
        }

        #region Distribution Parameters

        /// <summary>
        /// Gets or sets the degrees of freedom (nu) parameter.
        /// </summary>
        public int DegreesOfFreedom
        {
            get { return _degreesOfFreedom; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        /// <param name="degreesOfFreedom">nu-parameter</param>
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
            _chiSquareDistribution.SetDistributionParameters(degreesOfFreedom);

            double a = 0.5 * (_degreesOfFreedom + 1);
            double nLn = Fn.GammaLn(a);
            double dLn1 = Math.Log(Math.Sqrt(Math.PI * _degreesOfFreedom));
            double dLn2 = Fn.GammaLn(0.5 * _degreesOfFreedom);

            _exponent = -a;
            _factor = Math.Exp(nLn - dLn1 - dLn2);
            _summand = Fn.BetaRegularized(0.5 * _degreesOfFreedom, 0.5, 1);
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if degreesOfFreedom is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="degreesOfFreedom">nu-parameter</param>
        public static
        bool
        IsValidParameterSet(
            int degreesOfFreedom
            )
        {
            return degreesOfFreedom > 0;
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
        /// Throws <see cref="NotSupportedException"/> if <see cref="DegreesOfFreedom"/> &lt;= 2,
        /// since the value is not defined in this case.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Mean
        {
            get
            {
                if(_degreesOfFreedom < 2)
                {
                    throw new NotSupportedException();
                }

                return 0.0;
            }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// Throws <see cref="NotSupportedException"/> if <see cref="DegreesOfFreedom"/> &lt;= 3,
        /// since the value is not defined in this case.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Variance
        {
            get
            {
                if(_degreesOfFreedom < 3)
                {
                    throw new NotSupportedException();
                }

                return _degreesOfFreedom / (_degreesOfFreedom - 2.0);
            }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// Throws <see cref="NotSupportedException"/> if <see cref="DegreesOfFreedom"/> &lt;= 4,
        /// since the value is not defined in this case.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Skewness
        {
            get
            {
                if(_degreesOfFreedom < 4)
                {
                    throw new NotSupportedException();
                }

                return 0.0;
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
            return _factor * Math.Pow(1.0 + x * x / _degreesOfFreedom, _exponent);
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
            double beta = Fn.BetaRegularized(
                0.5 * _degreesOfFreedom,
                0.5,
                _degreesOfFreedom / (_degreesOfFreedom + x * x)
                );

            return 0.5 + 0.5 * Math.Sign(x) * (_summand - beta);
        }

        #endregion

        #region Generator

        /// <summary>
        /// Returns a student's t-distributed floating point random number.
        /// </summary>
        /// <returns>A student's t-distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double z = _standardDistribution.NextDouble();
            double v = _chiSquareDistribution.NextDouble();

            return z / Math.Sqrt(v / _degreesOfFreedom);
        }

        #endregion
    }
}
