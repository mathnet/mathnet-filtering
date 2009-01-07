//-----------------------------------------------------------------------
// <copyright file="TrapeziumRule.cs" company="Math.NET Project">
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
    /// <summary>
    /// Approximation algorithm for definite integrals by the Trapezium rule of the Newton-Cotes family.
    /// </summary>
    /// <remarks>
    /// <a href="http://en.wikipedia.org/wiki/Trapezium_rule">Wikipedia - Trapezium Rule</a>
    /// </remarks>
    public class TrapeziumRule
    {
        /// <summary>
        /// Direct 2-point approximation of the definite integral in the provided interval by the trapezium rule.
        /// </summary>
        public double IntegrateTwoPoint(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd)
        {
            return (intervalEnd - intervalBegin) / 2 * (f(intervalBegin) + f(intervalEnd));
        }

        /// <summary>
        /// Composite N-point approximation of the definite integral in the provided interval by the trapezium rule.
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

            double step = (intervalEnd - intervalBegin) / numberOfPartitions;

            double offset = step;
            double sum = 0.5 * (f(intervalBegin) + f(intervalEnd));
            for(int i = 0; i < numberOfPartitions - 1; i++)
            {
                // NOTE (ruegg, 2009-01-07): Do not combine intervalBegin and offset (numerical stability!)
                sum += f(intervalBegin + offset);
                offset += step;
            }

            return step * sum;
        }

        /// <summary>
        /// Adaptive approximation of the definite integral in the provided interval by the trapezium rule.
        /// </summary>
        public double IntegrateAdaptive(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd,
            double targetRelativeError)
        {
            int numberOfPartitions = 1;
            double step = intervalEnd - intervalBegin;
            double sum = 0.5 * step * (f(intervalBegin) + f(intervalEnd));
            for(int k = 0; k < 20; k++)
            {
                double midpointsum = 0;
                for(int i = 0; i < numberOfPartitions; i++)
                {
                    midpointsum += f(intervalBegin + ((i + 0.5) * step));
                }

                midpointsum *= step;
                sum = 0.5 * (sum + midpointsum);
                step *= 0.5;
                numberOfPartitions *= 2;

                if(Number.AlmostEqual(sum, midpointsum, targetRelativeError))
                {
                    break;
                }
            }

            return sum;
        }

        /// <summary>
        /// Adaptive approximation of the definite integral by the trapezium rule.
        /// </summary>
        public double IntegrateAdaptiveTransformedOdd(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd,
            IEnumerable<double[]> levelAbcissas,
            IEnumerable<double[]> levelWeights,
            double levelOneStep,
            double targetRelativeError)
        {
            double linearSlope = 0.5 * (intervalEnd - intervalBegin);
            double linearOffset = 0.5 * (intervalEnd + intervalBegin);
            targetRelativeError /= (5 * linearSlope);

            IEnumerator<double[]> abcissasIterator = levelAbcissas.GetEnumerator();
            IEnumerator<double[]> weightsIterator = levelWeights.GetEnumerator();

            double step = levelOneStep;
            
            // First Level
            abcissasIterator.MoveNext();
            weightsIterator.MoveNext();
            double[] abcissasL1 = abcissasIterator.Current;
            double[] weightsL1 = weightsIterator.Current;

            double sum = f(linearOffset) * weightsL1[0];
            for(int i = 1; i < abcissasL1.Length; i++)
            {
                sum += weightsL1[i] * (f((linearSlope * abcissasL1[i]) + linearOffset) + f(-(linearSlope * abcissasL1[i]) + linearOffset));
            }

            sum *= step;

            // Additional Levels
            double previousDelta = double.MaxValue;
            for(int level = 1; abcissasIterator.MoveNext() && weightsIterator.MoveNext(); level++)
            {
                double[] abcissas = abcissasIterator.Current;
                double[] weights = weightsIterator.Current;

                double midpointsum = 0;
                for(int i = 0; i < abcissas.Length; i++)
                {
                    midpointsum += weights[i] * (f((linearSlope * abcissas[i]) + linearOffset) + f(-(linearSlope * abcissas[i]) + linearOffset));
                }

                midpointsum *= step;
                sum = 0.5 * (sum + midpointsum);
                step *= 0.5;

                double delta = Math.Abs(sum - midpointsum);

                if(level == 1)
                {
                    previousDelta = delta;
                    continue;
                }

                double r = Math.Log(delta) / Math.Log(previousDelta);
                previousDelta = delta;

                if(r > 1.9 && r < 2.1)
                {
                    // convergence region
                    delta = Math.Sqrt(delta);
                }

                if(Number.AlmostEqualNorm(sum, midpointsum, delta, targetRelativeError))
                {
                    break;
                }
            }

            return sum * linearSlope;
        }
    }
}
