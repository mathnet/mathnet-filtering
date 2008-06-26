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
    /// Provides generation of erlang distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The implementation of the <see cref="ErlangDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Erlang_distribution">Wikipedia - Erlang distribution</a> and
    ///   <a href="http://www.xycoon.com/erlang_random.htm">Xycoon - Erlang Distribution</a>.
    /// </remarks>
    public sealed class ErlangDistribution : ContinuousDistribution
    {
        int _shape;
        double _rate;
        double _helper1;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ErlangDistribution()
            : base()
        {
            SetDistributionParameters(1, 1.0);
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
        ErlangDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(1, 1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        ErlangDistribution(
            int shape,
            double rate
            )
            : base()
        {
            SetDistributionParameters(shape, rate);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the shape k parameter.
        /// </summary>
        public int Shape
        {
            get { return _shape; }
            set { SetDistributionParameters(value, _rate); }
        }

        /// <summary>
        /// Gets or sets the rate lambda parameter.
        /// </summary>
        public double Rate
        {
            get { return _rate; }
            set { SetDistributionParameters(_shape, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            int shape,
            double rate
            )
        {
            if(!IsValidParameterSet(shape, rate))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _shape = shape;
            _rate = rate;
            _helper1 = -1.0 / rate;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if both shape and rate are greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            int shape,
            double rate
            )
        {
            return shape > 0 && rate > 0.0;
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
            get { return _shape / _rate; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// Throws <see cref="NotSupportedException"/> since
        /// the value is not defined for this distribution.
        /// </summary>
        /// <exception cref="NotSupportedException">Always.</exception>
        public override double Median
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _shape / (_rate * _rate); }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return 2.0 / Math.Sqrt(_shape); }
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
            return Math.Exp(_shape * Math.Log(_rate) + (_shape - 1) * Math.Log(x) - _rate * x - Fn.FactorialLn(_shape - 1));
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
            return Fn.GammaRegularized(_shape, _rate * x);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a erlang distributed floating point random number.
        /// </summary>
        /// <returns>A erlang distributed double-precision floating point number.</returns>
        public override
        double
        NextDouble()
        {
            double product = 1.0;
            for(int i = 0; i < _shape; i++)
            {
                // Subtract random number from 1.0 to avoid Math.Log(0.0)
                product *= (1.0 - this.RandomSource.NextDouble());
            }

            return _helper1 * Math.Log(product);
        }
        #endregion
    }
}
