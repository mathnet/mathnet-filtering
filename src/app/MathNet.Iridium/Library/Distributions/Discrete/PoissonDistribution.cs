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
#region Derived From: Copyright 2002 Aachen University of Technology
// ****************************************************************************
//
//   |_|_|_  |_|_    |_    |_|_|_  |_             C O M M U N I C A T I O N
// |_        |_  |_  |_  |_        |_                       N E T W O R K S
// |_        |_  |_  |_  |_        |_                             C L A S S
//   |_|_|_  |_    |_|_    |_|_|_  |_|_|_|_                   L I B R A R Y
//
// $Id: Geometric.c,v 1.2 2002/01/14 11:37:33 spee Exp $
//
// CNClass: CNGeometric --- CNGeometric distributed random numbers
//
// ****************************************************************************
// Copyright (C) 1992-1996   Communication Networks
//                           Aachen University of Technology
//                           D-52056 Aachen
//                           Germany
//                           Email: cncl-adm@comnets.rwth-aachen.de
// ****************************************************************************
// This file is part of the CN class library. All files marked with
// this header are free software; you can redistribute it and/or modify
// it under the terms of the GNU Library General Public License as
// published by the Free Software Foundation; either version 2 of the
// License, or (at your option) any later version.  This library is
// distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Library General Public
// License for more details.  You should have received a copy of the GNU
// Library General Public License along with this library; if not, write
// to the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139,
// USA.
// ****************************************************************************
// original Copyright:
// -------------------
// Copyright (C) 1988 Free Software Foundation
//    written by Dirk Grunwald (grunwald@cs.uiuc.edu)
// 
// This file is part of the GNU C++ Library.  This library is free
// software; you can redistribute it and/or modify it under the terms of
// the GNU Library General Public License as published by the Free
// Software Foundation; either version 2 of the License, or (at your
// option) any later version.  This library is distributed in the hope
// that it will be useful, but WITHOUT ANY WARRANTY; without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE.  See the GNU Library General Public License for more details.
// You should have received a copy of the GNU Library General Public
// License along with this library; if not, write to the Free Software
// Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
// ****************************************************************************
#endregion

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Pseudo-random generation of poisson distributed deviates.
    /// </summary>
    /// 
    /// <remarks> 
    /// <para>For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Poisson_distribution">
    /// Wikipedia - Poisson distribution</a>.</para>
    /// 
    /// <para>For details of the algorithm, see
    /// <a href="http://www.lkn.ei.tum.de/lehre/scn/cncl/doc/html/cncl_toc.html">
    /// Communication Networks Class Library (TU München)</a></para>
    ///
    /// <para>pdf: f(x) = exp(-l)*l^x/x!; l = lambda</para>
    /// </remarks>
    public sealed class PoissonDistribution : DiscreteDistribution
    {
        double _lambda;
        double _helper1;

        #region construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        PoissonDistribution()
            : base()
        {
            SetDistributionParameters(1.0);
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
        PoissonDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(1.0);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        PoissonDistribution(
            double lambda
            )
            : base()
        {
            SetDistributionParameters(lambda);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the lambda parameter.
        /// </summary>
        public double Lambda
        {
            get { return _lambda; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double lambda
            )
        {
            if(!IsValidParameterSet(lambda))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid, "lambda");
            }

            _lambda = lambda;
            _helper1 = Math.Exp(-_lambda);
        }

        /// <summary>
        /// Determines whether the specified parameters are valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double lambda
            )
        {
            return lambda > 0.0;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override int Maximum
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers. 
        /// </summary>
        public override double Mean
        {
            get { return _lambda; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override int Median
        { // approximation, see Wikipedia
            get { return (int)Math.Floor(_lambda + 1.0 / 3.0 - 0.2 / _lambda); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _lambda; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return Math.Sqrt(_lambda); }
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
            return Math.Exp(
                -_lambda
                + x * Math.Log(_lambda)
                - Fn.FactorialLn(x)
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
            return 1.0 - Fn.GammaRegularized(x + 1, _lambda);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a poisson distributed random number.
        /// </summary>
        /// <returns>A poisson distributed 32-bit signed integer.</returns>
        public override
        int
        NextInt32()
        {
            int count = 0;
            for(double product = this.RandomSource.NextDouble(); product >= _helper1; product *= this.RandomSource.NextDouble())
            {
                count++;
            }

            return count;
        }
        #endregion
    }
}
