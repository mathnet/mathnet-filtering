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

        private void TestContinuousDistributionShapeMatchesCumulativeDensity(
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
        public void TestContinuousDistributions_Uniform()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ContinuousUniformDistribution(0.0, 1.0),
                0.0, 1.0, 5, 100000, 0.01, "ContinuousUniform(0.0,1.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ContinuousUniformDistribution(-2.0, 2.0),
                -2.0, 2.0, 10, 100000, 0.01, "ContinuousUniform(-2.0,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_Triangular()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new TriangularDistribution(2.0, 4.0, 2.5),
                2.0, 4.0, 10, 100000, 0.01, "TriangularDistribution(2.0,4.0,2.5)");
        }

        [Test]
        public void TestContinuousDistributions_Gaussian()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new StandardDistribution(),
                -2.0, 2.0, 10, 100000, 0.01, "StandardDistribution()");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new NormalDistribution(-5.0, 2.0),
                -9.0, -1.0, 10, 100000, 0.01, "NormalDistribution(-5.0,2.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new LognormalDistribution(1.0, 0.5),
                0.0, 8.0, 10, 100000, 0.01, "LognormalDistribution(1.0,0.5)");
        }

        [Test]
        public void TestContinuousDistributions_Exponential()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ExponentialDistribution(0.75),
                0.0, 7.0, 10, 100000, 0.01, "ExponentialDistribution(0.75)");
        }

        [Test]
        public void TestContinuousDistributions_GammaBeta()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new GammaDistribution(2.0, 2.0),
                0.0, 12.0, 10, 100000, 0.01, "GammaDistribution(2.0,2.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new BetaDistribution(2.0, 5.0),
                0.0, 1.0, 10, 100000, 0.01, "BetaDistribution(2.0,5.0)");
        }

        [Test]
        public void TestContinuousDistributions_Laplace()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new LaplaceDistribution(4.0, 1.5),
                0.0, 8.0, 10, 100000, 0.01, "LaplaceDistribution(4.0,1.5)");
        }

        [Test]
        public void TestContinuousDistributions_Chi()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ChiDistribution(3),
                0.0, 8.0, 10, 100000, 0.01, "ChiDistribution(3)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ChiSquareDistribution(2),
                0.0, 8.0, 10, 100000, 0.01, "ChiSquareDistribution(2)");
        }

        [Test]
        public void TestContinuousDistributions_Pareto()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ParetoDistribution(1.0, 2.0),
                1.0, 4.0, 10, 100000, 0.01, "ParetoDistribution(1.0,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_Cauchy()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new CauchyLorentzDistribution(1.0, 0.5),
                0.0, 3.0, 10, 100000, 0.01, "CauchyLorentzDistribution(1.0,0.5)");
        }

        [Test]
        public void TestContinuousDistributions_Erlang()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ErlangDistribution(2, 2.0),
                0.0, 10.0, 10, 100000, 0.01, "ErlangDistribution(2,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_FisherSnedecor()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new FisherSnedecorDistribution(10, 5),
                0.0, 3.0, 10, 100000, 0.01, "FisherSnedecorDistribution(10,5)");

            FisherSnedecorDistribution d = new FisherSnedecorDistribution(100, 100);

            // PDF - Evaluated in Maple with "stats[statevalf,pdf,fratio[100,100]](x);"
            NumericAssert.AreAlmostEqual(0.0, d.ProbabilityDensity(0.00), "pdf(0.00)");
            NumericAssert.AreAlmostEqual(0.1102042201e-1, d.ProbabilityDensity(0.50), 1e-9, "pdf(0.50)");
            NumericAssert.AreAlmostEqual(0.9462174566, d.ProbabilityDensity(0.75), 1e-9, "pdf(0.75)");
            NumericAssert.AreAlmostEqual(1.989730935, d.ProbabilityDensity(1.00), 1e-9, "pdf(1.00)");
            NumericAssert.AreAlmostEqual(0.8553281091, d.ProbabilityDensity(1.25), 1e-9, "pdf(1.25)");
            NumericAssert.AreAlmostEqual(0.1722918542, d.ProbabilityDensity(1.50), 1e-9, "pdf(1.50)");
            NumericAssert.AreAlmostEqual(0.2755105502e-2, d.ProbabilityDensity(2.00), 1e-9, "pdf(2.00)");
            NumericAssert.AreAlmostEqual(0.6858025452e-13, d.ProbabilityDensity(5.00), 1e-9, "pdf(5.00)");

            // CDF - Evaluated in Maple with "stats[statevalf,cdr,fratio[100,100]](x);"
            NumericAssert.AreAlmostEqual(0.0, d.CumulativeDistribution(0.00), "cdf(0.00)");
            NumericAssert.AreAlmostEqual(0.309136863e-3, d.CumulativeDistribution(0.50), 1e-8, "cdf(0.50)");
            NumericAssert.AreAlmostEqual(0.7602003436e-1, d.CumulativeDistribution(0.75), 1e-9, "cdf(0.75)");
            NumericAssert.AreAlmostEqual(0.5000000000, d.CumulativeDistribution(1.00), 1e-9, "cdf(1.00)");
            NumericAssert.AreAlmostEqual(0.8668843977, d.CumulativeDistribution(1.25), 1e-9, "cdf(1.25)");
            NumericAssert.AreAlmostEqual(0.9780695579, d.CumulativeDistribution(1.50), 1e-9, "cdf(1.50)");
            NumericAssert.AreAlmostEqual(0.9996908631, d.CumulativeDistribution(2.00), 1e-9, "cdf(2.00)");
            NumericAssert.AreAlmostEqual(1.0, d.CumulativeDistribution(5.00), 1e-9, "cdf(5.00)");

            FisherSnedecorDistribution dOne = new FisherSnedecorDistribution(1, 5);
            Assert.IsTrue(double.IsPositiveInfinity(dOne.ProbabilityDensity(0.00)), "pdf[1,5](0.00)");
            NumericAssert.AreAlmostEqual(0.0, dOne.CumulativeDistribution(0.00), "cdf[1,5](0.00)");

            FisherSnedecorDistribution dTwo = new FisherSnedecorDistribution(2, 5);
            NumericAssert.AreAlmostEqual(1.0, dTwo.ProbabilityDensity(0.00), "pdf[2,5](0.00)");
            NumericAssert.AreAlmostEqual(0.0, dTwo.CumulativeDistribution(0.00), "cdf[2,5](0.00)");
        }

        [Test]
        public void TestContinuousDistributions_StudensT()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new StudentsTDistribution(2),
                -2.0, 5.0, 10, 100000, 0.01, "StudentsTDistribution(2)");

            StudentsTDistribution d = new StudentsTDistribution(2);

            // PDF - Evaluated in Maple with "stats[statevalf,pdf,studentst[2]](x);"
            NumericAssert.AreAlmostEqual(0.3535533906, d.ProbabilityDensity(0.0), 1e-9, "pdf(0)");
            NumericAssert.AreAlmostEqual(0.1924500897, d.ProbabilityDensity(1.0), 1e-9, "pdf(1)");
            NumericAssert.AreAlmostEqual(0.06804138174, d.ProbabilityDensity(2.0), 1e-9, "pdf(2)");
            NumericAssert.AreAlmostEqual(0.02741012223, d.ProbabilityDensity(3.0), 1e-9, "pdf(3)");
            NumericAssert.AreAlmostEqual(0.01309457002, d.ProbabilityDensity(4.0), 1e-9, "pdf(4)");
            NumericAssert.AreAlmostEqual(0.1924500897, d.ProbabilityDensity(-1.0), 1e-9, "pdf(-1)");
            NumericAssert.AreAlmostEqual(0.06804138174, d.ProbabilityDensity(-2.0), 1e-9, "pdf(-2)");
            NumericAssert.AreAlmostEqual(0.02741012223, d.ProbabilityDensity(-3.0), 1e-9, "pdf(-3)");
            NumericAssert.AreAlmostEqual(0.01309457002, d.ProbabilityDensity(-4.0), 1e-9, "pdf(-4)");

            // CDF - Evaluated in Maple with "stats[statevalf,cdf,studentst[2]](x);"
            NumericAssert.AreAlmostEqual(0.5000000000, d.CumulativeDistribution(0.0), "cdf(0)");
            NumericAssert.AreAlmostEqual(0.7886751346, d.CumulativeDistribution(1.0), 1e-9, "cdf(1)");
            NumericAssert.AreAlmostEqual(0.9082482905, d.CumulativeDistribution(2.0), 1e-9, "cdf(2)");
            NumericAssert.AreAlmostEqual(0.9522670169, d.CumulativeDistribution(3.0), 1e-9, "cdf(3)");
            NumericAssert.AreAlmostEqual(0.9714045208, d.CumulativeDistribution(4.0), 1e-9, "cdf(4)");
            NumericAssert.AreAlmostEqual(0.2113248654, d.CumulativeDistribution(-1.0), 1e-9, "cdf(-1)");
            NumericAssert.AreAlmostEqual(0.09175170954, d.CumulativeDistribution(-2.0), 1e-9, "cdf(-2)");
            NumericAssert.AreAlmostEqual(0.04773298313, d.CumulativeDistribution(-3.0), 1e-9, "cdf(-3)");
            NumericAssert.AreAlmostEqual(0.02859547921, d.CumulativeDistribution(-4.0), 1e-9, "cdf(-4)");
        }
    }
}
