using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    public static class NumericAssert
    {
        public static void AreAlmostEqual(Matrix expected, Matrix actual, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected.Norm1(), actual.Norm1(), (expected-actual).Norm1(), 10 * Number.DefaultRelativeAccuracy, message));
        }

        public static void AreAlmostEqual(double expected, double actual, double relativeAccuracy, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected, actual, relativeAccuracy, message));
        }

        public static void AreAlmostEqual(double expected, double actual, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected, actual, message));
        }
    }

    public class AlmostEqualAsserter : ComparisonAsserter
    {
        double _relativeAccuracy;
        double _difference;

        public AlmostEqualAsserter(double expected, double actual, double difference, double relativeAccuracy, string message, params object[] args)
            : base(expected, actual, message, args)
        {
            _relativeAccuracy = relativeAccuracy;
            _difference = difference;
        }

        public AlmostEqualAsserter(double expected, double actual, double relativeAccuracy, string message, params object[] args)
            : this(expected, actual, expected - actual, relativeAccuracy, message, args)
        {
        }

        public AlmostEqualAsserter(double expected, double actual, string message, params object[] args)
            : this(expected, actual, expected - actual, Number.DefaultRelativeAccuracy, message, args)
        {
        }

        public override bool Test()
        {
            double actual = (double)base.actual;
            double expected = (double)base.expected;

            if(!Number.AlmostEqualNorm(actual, expected, _difference, _relativeAccuracy))
            {
                FailureMessage.DisplayDifferences(expected, actual, false);
                return false;
            }

            return true;
        }
    }
}
