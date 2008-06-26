#region Math.NET Iridium (LGPL) by Ruegg, Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//                          Joannes Vermorel, http://www.vermorel.com
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
    /// Non-uniform discrete random distribution.
    /// </summary>
    /// <remarks>
    /// <p>The class <c>ArbitraryDistribution</c> provides integers 
    /// deviates for any arbitrary (finite) distribution.</p>
    /// 
    /// <code>
    /// double[] distribution = {0.25, 0.25, 0.5};
    /// ArbitraryDistribution gen = new ArbitraryDistribution(0, distribution);
    /// 
    /// // Pr(x = 0) = 0.25, Pr(x = 1) = 0.25, Pr(x = 2) = 0.5
    /// int x = gen.NextInt32();
    /// </code>
    /// 
    /// <p>The probability <c>Pr(x)</c> for any integer <c>x</c>
    /// is proportional to <c>ArbitraryDistribution.ProbabilityMass(x)</c>.</p>
    /// 
    /// <p>See the <a href="http://cgm.cs.mcgill.ca/~luc/chapter_three.pdf">
    /// chapter three</a> of the book <i>Non-uniform variate Generation</i>
    /// from Luc Devroye.</p>
    /// </remarks>
    public sealed class ArbitraryDistribution : DiscreteDistribution
    {
        int _first;
        double[] _pmf;
        double[] _cdf;
        int _last, _n;
        double _mean, _variance;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ArbitraryDistribution()
            : base()
        {
            SetDistributionParameters(0, 1.0);
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
        ArbitraryDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0, 1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ArbitraryDistribution(
            int offset,
            params double[] probabilityMass
            )
            : base()
        {
            SetDistributionParameters(offset, probabilityMass);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets the index of the first item.
        /// </summary>
        public int FirstIndex
        {
            get { return _first; }
        }

        /// <summary>
        /// Gets the index of the last item.
        /// </summary>
        public int LastIndex
        {
            get { return _last; }
        }

        /// <summary>
        /// Gets the number of item.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            int offset,
            params double[] probabilityMass
            )
        {
            if(!IsValidParameterSet(offset, probabilityMass))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _first = offset;
            _n = probabilityMass.Length;
            _last = offset + _n - 1;

            if(_pmf == null || _pmf.Length < _n || _pmf.Length >= 4 * _n)
            {
                _pmf = new double[_n];
                _cdf = new double[_n];
            }

            // prepare probability mass function
            Array.Copy(probabilityMass, _pmf, _n);

            // precompute cumulated distribution function
            _cdf[0] = _pmf[0];
            for(int i = 1; i < _n; i++)
            {
                _cdf[i] = _cdf[i - 1] + _pmf[i];
            }

            // precompute mean/variance
            _mean = 0;
            _variance = 0;
            for(int i = 0, x = _first; i < _n; i++, x++)
            {
                _mean += x * _pmf[i];
                _variance += x * x * _pmf[i];
            }

            _variance -= _mean * _mean;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the probabilities sum up to 1; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            int offset,
            params double[] probabilityMass
            )
        {
            if(null == probabilityMass)
            {
                throw new ArgumentNullException("probabilityMass");
            }

            double sum = 0.0;
            for(int i = 0; i < probabilityMass.Length; i++)
            {
                sum += probabilityMass[i];
            }

            return Number.AlmostEqual(1.0, sum, 10 + 2 * probabilityMass.Length);
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return _first; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override int Maximum
        {
            get { return _last; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return _mean; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// Throws <see cref="NotSupportedException"/> since
        /// the value is not defined for this distribution.
        /// </summary>
        /// <exception cref="NotSupportedException">Always.</exception>
        public override int Median
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _variance; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Discrete probability mass function (pmf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityMass(
            int x
            )
        {
            if(x < _first || x > _last)
            {
                return 0.0;
            }

            return _pmf[x - _first];
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
            if(x < _first)
            {
                return 0.0;
            }

            int xx = (int)Math.Floor(x);
            if(xx <= _last)
            {
                return _cdf[xx - _first];
            }

            return 1.0;
        }

        #endregion

        #region Generator
        /// <summary>
        /// Generate a new random number according to this distribution.
        /// </summary>
        public override
        int
        NextInt32()
        {
            double rnd = this.RandomSource.NextDouble();
            if(rnd >= _cdf[_n - 1])
            {
                return _last;
            }

            // TODO: consider using binary search instead
            int index = 0;
            while(_cdf[index] < rnd)
            {
                index++;
            }

            return index + _first;
        }
        #endregion
    }
}
