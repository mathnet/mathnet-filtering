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
    /// Provides generation of geometric distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The geometric distribution generates only discrete numbers.<br />
    /// The implementation bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Geometric_distribution">Wikipedia - Geometric distribution</a>
    ///   and the implementation in the <a href="http://www.lkn.ei.tum.de/lehre/scn/cncl/doc/html/cncl_toc.html">
    ///   Communication Networks Class Library</a>.
    /// </remarks>
    public sealed class GeometricDistribution : DiscreteDistribution
    {
        double _p;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        GeometricDistribution()
            : base()
        {
            SetDistributionParameters(0.5);
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
        GeometricDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.5);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        GeometricDistribution(
            double probabilityOfSuccess
            )
            : base()
        {
            SetDistributionParameters(probabilityOfSuccess);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the success probability parameter.
        /// </summary>
        public double ProbabilityOfSuccess
        {
            get { return _p; }
            set { SetDistributionParameters(value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double probabilityOfSuccess
            )
        {
            if(!IsValidParameterSet(probabilityOfSuccess))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid, "probabilityOfSuccess");
            }

            _p = probabilityOfSuccess;
        }

        /// <summary>
        /// Determines whether the specified parameters are valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if value is greater than or equal to 0.0, and less than or equal to 1.0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double probabilityOfSuccess
            )
        {
            return probabilityOfSuccess >= 0.0 && probabilityOfSuccess <= 1.0;
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return 1; }
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
            get { return 1.0 / _p; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override int Median
        {
            get { return (int)Math.Ceiling(-Constants.Ln2 / Math.Log(1.0 - _p)); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return (1.0 - _p) / (_p * _p); }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return (2.0 - _p) / Math.Sqrt(1.0 - _p); }
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
            return _p * Math.Pow(1.0 - _p, x - 1);
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
            return 1.0 - Math.Pow(1.0 - _p, x);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a geometric distributed floating point random number.
        /// </summary>
        /// <returns>A geometric distributed double-precision floating point number.</returns>
        public override
        int
        NextInt32()
        {
            // TODO: Implement direct transformation instead of simulation
            int samples;

            for(samples = 1; this.RandomSource.NextDouble() >= _p; samples++)
            {
            }

            return samples;
        }
        #endregion
    }
}
