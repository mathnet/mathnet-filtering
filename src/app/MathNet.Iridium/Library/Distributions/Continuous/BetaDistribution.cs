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
    /// Provides generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="BetaDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta distribution</a> and
    ///   <a href="http://www.xycoon.com/beta_randomnumbers.htm">Xycoon - Beta Distribution</a>.
    /// </remarks>
    public sealed class BetaDistribution : ContinuousDistribution
    {
        double _alpha;
        double _beta;
        double _lnbetaAlphaBeta;

        GammaDistribution _gammaAlpha;
        GammaDistribution _gammaBeta;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BetaDistribution()
            : base()
        {
            _gammaAlpha = new GammaDistribution(this.RandomSource);
            _gammaBeta = new GammaDistribution(this.RandomSource);
            SetDistributionParameters(1.0, 1.0);
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
        BetaDistribution(
            RandomSource random
            )
            : base(random)
        {
            _gammaAlpha = new GammaDistribution(random);
            _gammaBeta = new GammaDistribution(random);
            SetDistributionParameters(1.0, 1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BetaDistribution(
            double alpha,
            double beta
            )
            : base()
        {
            _gammaAlpha = new GammaDistribution(this.RandomSource);
            _gammaBeta = new GammaDistribution(this.RandomSource);
            SetDistributionParameters(alpha, beta);
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
                _gammaAlpha.RandomSource = value;
                _gammaBeta.RandomSource = value;
            }
        }

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the alpha parameter.
        /// </summary>
        public double Alpha
        {
            get { return _alpha; }
            set { SetDistributionParameters(value, _beta); }
        }

        /// <summary>
        /// Gets or sets the beta parameter.
        /// </summary>
        public double Beta
        {
            get { return _beta; }
            set { SetDistributionParameters(_alpha, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double alpha,
            double beta
            )
        {
            if(!IsValidParameterSet(alpha, beta))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _alpha = alpha;
            _beta = beta;
            _gammaAlpha.SetDistributionParameters(alpha, 1.0);
            _gammaBeta.SetDistributionParameters(beta, 1.0);
            _lnbetaAlphaBeta = Fn.BetaLn(alpha, beta);
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if both alpha and beta are greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double alpha,
            double beta
            )
        {
            return alpha > 0.0 && beta > 0.0;
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
            get { return 1.0; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return _alpha / (_alpha + _beta); }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override double Median
        {
            get { return double.NaN; }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get
            {
                double ab = _alpha + _beta;
                return (_alpha * _beta) / (ab * ab * (ab + 1.0));
            }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get
            {
                double m = 2 * (_beta - _alpha) * Math.Sqrt(_alpha + _beta + 1.0);
                double n = (_alpha + _beta + 2.0) * Math.Sqrt(_alpha * _beta);
                return m / n;
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
            return Math.Exp(
                (_alpha - 1.0) * Math.Log(x)
                + (_beta - 1.0) * Math.Log(1 - x)
                - _lnbetaAlphaBeta
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
            return Fn.BetaRegularized(_alpha, _beta, x);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a beta distributed floating point random number.
        /// </summary>
        /// <returns>A beta distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double x = _gammaAlpha.NextDouble();

            return x / (x + _gammaBeta.NextDouble());
        }
        #endregion
    }
}
