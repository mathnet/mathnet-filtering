#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2007, Christoph Rüegg, http://christoph.ruegg.name
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
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace Iridium.Test
{
    public sealed class DistributionShape
    {
        readonly double _scale, _offset;
        readonly int _buckets;
        int[] _shape;
        int _underflow, _overflow;

        private DistributionShape(int buckets, double scale, double offset)
        {
            _scale = scale;
            _offset = offset;
            _buckets = buckets;
            _shape = new int[buckets];
        }

        /// <param name="buckets">number of buckets.</param>
        /// <param name="min">inclusive minimum.</param>
        /// <param name="max">exclusive maximum.</param>
        public static DistributionShape CreateMinMax(int buckets, double min, double max)
        {
            return new DistributionShape(buckets, buckets / (max - min), -min);
        }

        public void Push(double value)
        {
            double bucket = (value + _offset) * _scale;
            if(bucket < 0)
                _underflow++;
            else if(bucket >= _buckets)
                _overflow++;
            else
                _shape[(int)bucket]++;
        }

        public int Underflow
        {
            get { return _underflow; }
        }
        public int Overflow
        {
            get { return _overflow; }
        }
        public int this[int bucket]
        {
            get { return _shape[bucket]; }
        }
        public int BucketCount
        {
            get { return _buckets; }
        }
    }

    [TestFixture]
    public class DistributionTest
    {
        [Test]
        public void TestDistributionShapeTestHelper()
        {
            DistributionShape shape = DistributionShape.CreateMinMax(2, -1.0, +1.0);
            shape.Push(-1.5); // underflow
            shape.Push(-1.0); // 0
            shape.Push(-0.5); // 0
            shape.Push(0.0); // 1
            shape.Push(0.5); // 1
            shape.Push(1.0); // overflow
            shape.Push(1.5); // overflow

            Assert.AreEqual(1, shape.Underflow, "underflow");
            Assert.AreEqual(2, shape.Overflow, "overflow");
            Assert.AreEqual(2, shape[0], "0");
            Assert.AreEqual(2, shape[1], "1");
        }

        private void TestContinuousDistributionShape(
            ContinuousDistribution distribution,
            double min, double max,
            double[] expectedShape, double expectedUnderflow, double expectedOverflow,
            int avgSamplesPerBucket, double absoluteAccuracy, string message)
        {
            DistributionShape shape = DistributionShape.CreateMinMax(expectedShape.Length, min, max);
            int sampleCount = expectedShape.Length * avgSamplesPerBucket;
            for(int i = 0; i < sampleCount; i++)
            {
                shape.Push(distribution.NextDouble());
            }
            double scale = 1.0 / (avgSamplesPerBucket * expectedShape.Length);
            Assert.AreEqual(expectedUnderflow, shape.Underflow * scale, absoluteAccuracy, message + " Underflow");
            Assert.AreEqual(expectedOverflow, shape.Overflow * scale, absoluteAccuracy, message + " Overflow");
            for(int i = 0; i < expectedShape.Length; i++)
            {
                Assert.AreEqual(expectedShape[i], shape[i] * scale, absoluteAccuracy, message + " Bucket " + i.ToString());
            }
        }

        private void TestContinuousDistributionShapeMatchesCummulativeDensity(
            ContinuousDistribution distribution,
            double min, double max,
            int numberOfBuckets, int avgSamplesPerBucket,
            double absoluteAccuracy, string message)
        {
            double[] shape = new double[numberOfBuckets];
            double bucketWidth = (max - min) / numberOfBuckets;
            double previous = distribution.CumulativeDistribution(min);
            double underflow = previous;
            double position = min;
            for(int i = 0; i < numberOfBuckets; i++)
            {
                position += bucketWidth;
                double current = distribution.CumulativeDistribution(position);
                shape[i] = current - previous;
                previous = current;
            }
            double overflow = 1 - previous;
            
            TestContinuousDistributionShape(distribution, min, max,
                shape, underflow, overflow,
                avgSamplesPerBucket, absoluteAccuracy, message);
        }

        [Test]
        public void TestContinuousDistributions_ShapeMatchesCumulativeDensity()
        {
            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ContinuousUniformDistribution(0.0, 1.0),
                0.0, 1.0, 5, 100000, 0.01, "ContinuousUniform(0.0,1.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ContinuousUniformDistribution(-2.0, 2.0),
                -2.0, 2.0, 10, 100000, 0.01, "ContinuousUniform(-2.0,2.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new TriangularDistribution(2.0, 4.0, 2.5),
                2.0, 4.0, 10, 100000, 0.01, "TriangularDistribution(2.0,4.0,2.5)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new StandardDistribution(),
                -2.0, 2.0, 10, 100000, 0.01, "StandardDistribution()");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new NormalDistribution(-5.0, 2.0),
                -9.0, -1.0, 10, 100000, 0.01, "NormalDistribution(-5.0,2.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new LognormalDistribution(1.0, 0.5),
                0.0, 8.0, 10, 100000, 0.01, "LognormalDistribution(1.0,0.5)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ExponentialDistribution(0.75),
                0.0, 7.0, 10, 100000, 0.01, "ExponentialDistribution(0.75)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new GammaDistribution(2.0, 2.0),
                0.0, 12.0, 10, 100000, 0.01, "GammaDistribution(2.0,2.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new BetaDistribution(2.0, 5.0),
                0.0, 1.0, 10, 100000, 0.01, "BetaDistribution(2.0,5.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new LaplaceDistribution(4.0, 1.5),
                0.0, 8.0, 10, 100000, 0.01, "LaplaceDistribution(4.0,1.5)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ChiDistribution(3),
                0.0, 8.0, 10, 100000, 0.01, "ChiDistribution(3)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ChiSquareDistribution(2),
                0.0, 8.0, 10, 100000, 0.01, "ChiSquareDistribution(2)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ParetoDistribution(1.0, 2.0),
                1.0, 4.0, 10, 100000, 0.01, "ParetoDistribution(1.0,2.0)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new CauchyLorentzDistribution(1.0, 0.5),
                0.0, 3.0, 10, 100000, 0.01, "CauchyLorentzDistribution(1.0,0.5)");

            TestContinuousDistributionShapeMatchesCummulativeDensity(
                new ErlangDistribution(2, 2.0),
                0.0, 10.0, 10, 100000, 0.01, "ErlangDistribution(2,2.0)");
        }
    }
}
