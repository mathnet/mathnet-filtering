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
    /// Provides generation of levy skew alpha-stable distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="StableDistribution"/> type bases upon information presented on
    /// <a href="http://en.wikipedia.org/wiki/L%C3%A9vy_skew_alpha-stable_distribution">Wikipedia - Levy skew alpha-stable distribution</a>
    /// </remarks>
    public sealed class StableDistribution : ContinuousDistribution
    {
        double _location; // mu
        double _scale; // c
        double _exponent; // alpha
        double _skewness; // beta

        double _factor;
        double _theta;

        ContinuousUniformDistribution _uniformDistribution;
        ExponentialDistribution _exponentialDistribution;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        StableDistribution()
            : base()
        {
            SetDistributionParameters(0.0, 1.0, 1.0, 0.0);
            InitDistributions();
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
        StableDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.0, 1.0, 1.0, 0.0);
            InitDistributions();
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        /// <param name="location">mu-parameter</param>
        /// <param name="scale">c-parameter</param>
        /// <param name="exponent">alpha-parameter</param>
        /// <param name="skewness">beta-parameter</param>
        public
        StableDistribution(
            double location,
            double scale,
            double exponent,
            double skewness
            )
            : base()
        {
            SetDistributionParameters(location, scale, exponent, skewness);
            InitDistributions();
        }

        void
        InitDistributions()
        {
            _uniformDistribution = new ContinuousUniformDistribution(RandomSource);
            _uniformDistribution.SetDistributionParameters(-Constants.Pi_2, Constants.Pi_2);
            _exponentialDistribution = new ExponentialDistribution(RandomSource);
            _exponentialDistribution.SetDistributionParameters(1.0);
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
                _uniformDistribution.RandomSource = value;
                _exponentialDistribution.RandomSource = value;
            }
        }

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the location mu parameter.
        /// </summary>
        public double Location
        {
            get { return _location; }
            set { SetDistributionParameters(value, _scale, _exponent, _skewness); }
        }

        /// <summary>
        /// Gets or sets the scale c parameter.
        /// </summary>
        public double Scale
        {
            get { return _scale; }
            set { SetDistributionParameters(_location, value, _exponent, _skewness); }
        }

        /// <summary>
        /// Gets or sets the exponent alpha parameter.
        /// </summary>
        public double Exponent
        {
            get { return _exponent; }
            set { SetDistributionParameters(_location, _scale, value, _skewness); }
        }

        /// <summary>
        /// Gets or sets the skeqness beta parameter.
        /// </summary>
        public double Beta
        {
            get { return _skewness; }
            set { SetDistributionParameters(_location, _scale, _exponent, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        /// <param name="location">mu-parameter</param>
        /// <param name="scale">c-parameter</param>
        /// <param name="exponent">alpha-parameter</param>
        /// <param name="skewness">beta-parameter</param>
        public
        void
        SetDistributionParameters(
            double location,
            double scale,
            double exponent,
            double skewness
            )
        {
            if(!IsValidParameterSet(location, scale, exponent, skewness))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _location = location;
            _scale = scale;
            _exponent = exponent;
            _skewness = skewness;

            double angle = Constants.Pi_2 * exponent;
            double part1 = skewness * Math.Tan(angle);
            _factor = Math.Pow(1.0 + part1 * part1, 1.0 / (2.0 * exponent));
            _theta = (1.0 / exponent) * Math.Atan(skewness * Math.Tan(angle));
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if scale is greater than or equal to 0.0, skewness is between -1.0 and 1.0 and exponent is between 0.0 and 2.0; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="location">mu-parameter</param>
        /// <param name="scale">c-parameter</param>
        /// <param name="exponent">alpha-parameter</param>
        /// <param name="skewness">beta-parameter</param>
        public static
        bool
        IsValidParameterSet(
            double location,
            double scale,
            double exponent,
            double skewness
            )
        {
            return scale >= 0
                && skewness <= 1.0
                && skewness >= -1.0
                && exponent > 0
                && exponent <= 2;
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
        /// Throws <see cref="NotSupportedException"/> if <see cref="Exponent"/> &lt;= 1.0,
        /// since the value is not defined in this case.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Mean
        {
            get
            {
                if(_exponent <= 1.0)
                {
                    throw new NotSupportedException();
                }

                return _location;
            }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Median
        {
            get
            {
                if(!Number.AlmostZero(_skewness))
                {
                    throw new NotSupportedException(); // not supported yet
                }

                return _location;
            }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get
            {
                if(Number.AlmostEqual(_exponent, 2))
                {
                    return 2.0 * _scale * _scale;
                }

                return double.PositiveInfinity;
            }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public override double Skewness
        {
            get
            {
                if(!Number.AlmostEqual(_exponent, 2))
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
            throw new NotSupportedException(); // not supported yet
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
            throw new NotSupportedException(); // not supported yet
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns an alpha-stable distributed floating point random number.
        /// </summary>
        /// <returns>An alpha-stable distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double randTheta = _uniformDistribution.NextDouble();
            double randW = _exponentialDistribution.NextDouble();

            if(!Number.AlmostEqual(_exponent, 1))
            {
                double angle = _exponent * (randTheta + _theta);
                double factor1 = Math.Sin(angle) / Math.Pow(Math.Cos(randTheta), (1.0 / _exponent));
                double factor2 = Math.Pow(Math.Cos(randTheta - angle) / randW, (1 - _exponent) / _exponent);
                return _factor * factor1 * factor2;
            }
            else
            {
                double part1 = Constants.Pi_2 + _skewness * randTheta;
                double summand = part1 * Math.Tan(randTheta);
                double subtrahend = _skewness * Math.Log(Constants.Pi_2 * randW * Math.Cos(randTheta) / part1);
                return (2.0 / Math.PI) * (summand - subtrahend);
            }
        }
        #endregion
    }
}
