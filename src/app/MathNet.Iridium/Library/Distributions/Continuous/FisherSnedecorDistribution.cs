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
    /// Provides generation of F-distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="FisherSnedecorDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/F-distribution">Wikipedia - F-distribution</a>.
    /// </remarks>
    public sealed class FisherSnedecorDistribution :
        ContinuousDistribution
    {
        int _alpha;
        int _beta;
        double _alphabeta;
        double _pdfScaleLn;
        double _pdfExponent1;
        double _pdfExponent2;

        ChiSquareDistribution _chiSquaredAlpha;
        ChiSquareDistribution _chiSquaredBeta;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        FisherSnedecorDistribution()
            : base()
        {
            _chiSquaredAlpha = new ChiSquareDistribution(this.RandomSource);
            _chiSquaredBeta = new ChiSquareDistribution(this.RandomSource);
            SetDistributionParameters(1, 1);
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
        FisherSnedecorDistribution(
            RandomSource random
            )
            : base(random)
        {
            _chiSquaredAlpha = new ChiSquareDistribution(random);
            _chiSquaredBeta = new ChiSquareDistribution(random);
            SetDistributionParameters(1, 1);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        FisherSnedecorDistribution(
            int alpha,
            int beta
            )
            : base()
        {
            _chiSquaredAlpha = new ChiSquareDistribution(this.RandomSource);
            _chiSquaredBeta = new ChiSquareDistribution(this.RandomSource);
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
                _chiSquaredAlpha.RandomSource = value;
                _chiSquaredBeta.RandomSource = value;
            }
        }

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the alpha parameter.
        /// </summary>
        public int Alpha
        {
            get { return _alpha; }
            set { SetDistributionParameters(value, _beta); }
        }

        /// <summary>
        /// Gets or sets the beta parameter.
        /// </summary>
        public int Beta
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
            int alpha,
            int beta
            )
        {
            if(!IsValidParameterSet(alpha, beta))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _alpha = alpha;
            _beta = beta;
            _chiSquaredAlpha.SetDistributionParameters(alpha);
            _chiSquaredBeta.SetDistributionParameters(beta);

            double alphaHalf = 0.5 * alpha;
            double betaHalf = 0.5 * beta;

            _alphabeta = (double)beta / (double)alpha;

            _pdfScaleLn =
                alphaHalf * Math.Log(alpha)
                + betaHalf * Math.Log(beta)
                - Fn.BetaLn(alphaHalf, betaHalf);
            _pdfExponent1 = alphaHalf - 1.0;
            _pdfExponent2 = -alphaHalf - betaHalf;
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
            int alpha,
            int beta
            )
        {
            return alpha > 0 && beta > 0;
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
        /// <exception cref="NotSupportedException" />
        public override double Mean
        {
            get
            {
                if(_beta <= 2)
                {
                    return double.NaN;
                }

                return (double)_beta / ((double)_beta - 2.0d);
            }
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
                if(_beta <= 4)
                {
                    return double.NaN;
                }

                int m = 2 * (_beta * _beta) * (_alpha + _beta - 2);
                int betam2 = _beta - 2;
                int n = _alpha * betam2 * betam2 * (_beta - 4);
                return (double)m / (double)n;
            }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get
            {
                int betam2 = _beta - 2;
                double m = (2 * _alpha + betam2) * Math.Sqrt(8 * (_beta - 4));
                double n = (_beta - 6) * Math.Sqrt(_alpha * (_alpha + betam2));
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
            if(Number.AlmostEqual(0.0, x))
            {
                if(1 == _alpha)
                {
                    return double.PositiveInfinity;
                }

                if(2 == _alpha)
                {
                    return 1.0;
                }

                return 0.0;
            }

            double ln1 = Math.Log(x);
            double ln2 = Math.Log(_alpha * x + _beta);

            return Math.Exp(
                _pdfScaleLn
                + _pdfExponent1 * ln1
                + _pdfExponent2 * ln2
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
            double xa = _alpha * x;
            double m = xa / (xa + _beta);

            return Fn.BetaRegularized(0.5 * _alpha, 0.5 * _beta, m);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a beta distributed floating point random number.
        /// </summary>
        /// <returns>An F-distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            return _alphabeta * _chiSquaredAlpha.NextDouble() / _chiSquaredBeta.NextDouble();
        }
        #endregion
    }
}
