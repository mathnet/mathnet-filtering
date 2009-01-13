//-----------------------------------------------------------------------
// <copyright file="SimpsonRule.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
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
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics.Integration.Algorithms
{
    using MathNet.Numerics.NumberTheory;

    /// <summary>
    /// Approximation algorithm for definite integrals by Simpson's rule.
    /// </summary>
    public class SimpsonRule
    {
        /// <summary>
        /// Direct 3-point approximation of the definite integral in the provided interval by Simpson's rule.
        /// </summary>
        public double IntegrateThreePoint(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd)
        {
            double midpoint = (intervalEnd + intervalBegin) / 2;
            return (intervalEnd - intervalBegin) / 6 * (f(intervalBegin) + f(intervalEnd) + (4 * f(midpoint)));
        }

        /// <summary>
        /// Composite N-point approximation of the definite integral in the provided interval by Simpson's rule.
        /// </summary>
        public double IntegrateComposite(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd,
            int numberOfPartitions)
        {
            if(numberOfPartitions <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfPartitions", Properties.LocalStrings.ArgumentPositive);
            }

            if(IntegerTheory.IsOdd(numberOfPartitions))
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentEven, "numberOfPartitions");
            }

            double step = (intervalEnd - intervalBegin) / numberOfPartitions;
            double factor = step / 3;

            double offset = step;
            int m = 4;
            double sum = f(intervalBegin) + f(intervalEnd);
            for(int i = 0; i < numberOfPartitions - 1; i++)
            {
                // NOTE (ruegg, 2009-01-07): Do not combine intervalBegin and offset (numerical stability!)
                sum += m * f(intervalBegin + offset);
                m = 6 - m;
                offset += step;
            }

            return factor * sum;
        }
    }
}
