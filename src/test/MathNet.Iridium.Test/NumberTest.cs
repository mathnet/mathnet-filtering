using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;

namespace Iridium.Test
{
    [TestFixture]
    public class NumberTest
    {
        [Test]
        public void TestIncrementDecrementAtZero()
        {
            double x = 2 * double.Epsilon;
            Assert.AreEqual(2 * double.Epsilon, x, "A");
            x = Number.Decrement(x);
            x = Number.Decrement(x);
            Assert.AreEqual(0, x, "B");
            x = Number.Decrement(x);
            x = Number.Decrement(x);
            Assert.AreEqual(-2 * double.Epsilon, x, "C");
            x = Number.Increment(x);
            x = Number.Increment(x);
            Assert.AreEqual(0, x, "D");
            x = Number.Increment(x);
            x = Number.Increment(x);
            Assert.AreEqual(2 * double.Epsilon, x, "E");
        }

        [Test]
        public void TestIncrementDecrementAtMinMax()
        {
            double x = double.MaxValue;
            Assert.AreEqual(double.MaxValue, x, "A");
            x = Number.Decrement(x);
            Assert.IsTrue(x < double.MaxValue, "B");
            x = Number.Increment(x);
            Assert.AreEqual(double.MaxValue, x, "C");
            x = Number.Increment(x);
            Assert.IsTrue(double.IsPositiveInfinity(x), "D");

            x = double.MinValue;
            Assert.AreEqual(double.MinValue, x, "E");
            x = Number.Increment(x);
            Assert.IsTrue(x > double.MinValue, "F");
            x = Number.Decrement(x);
            Assert.AreEqual(double.MinValue, x, "G");
            x = Number.Decrement(x);
            Assert.IsTrue(double.IsNegativeInfinity(x), "H");
        }

        [Test]
        public void TestIncrementDecrementStep()
        {
            double x0 = 1e-100;
            double x1 = 1e+0;
            double x2 = 1e+100;
            double x3 = 1e+200;
            double x4 = -1e+100;

            double y0 = Number.Increment(x0) - x0;
            double y1 = Number.Increment(x1) - x1;
            double y2 = Number.Increment(x2) - x2;
            double y3 = Number.Increment(x3) - x3;
            double y4 = Number.Increment(x4) - x4;

            Assert.IsTrue(y0 < y1, "A");
            Assert.IsTrue(y1 < y2, "B");
            Assert.IsTrue(y2 < y3, "C");
            Assert.IsTrue(y2 == y4, "D");
         }

        [Test]
        public void TestEpsilonOf()
        {
            IFormatProvider format = System.Globalization.CultureInfo.InvariantCulture;
            Assert.AreEqual("1.11022302462516E-16", Number.EpsilonOf(1.0).ToString(format), "A");
            Assert.AreEqual("1.11022302462516E-16", Number.EpsilonOf(-1.0).ToString(format), "B");
            Assert.AreEqual("4.94065645841247E-324", Number.EpsilonOf(0.0).ToString(format), "C");
            Assert.AreEqual("1.94266889222573E+84", Number.EpsilonOf(1.0e+100).ToString(format), "D");
            Assert.AreEqual("1.94266889222573E+84", Number.EpsilonOf(-1.0e+100).ToString(format), "E");
            Assert.AreEqual("1.26897091865782E-116", Number.EpsilonOf(1.0e-100).ToString(format), "F");
            Assert.AreEqual("1.26897091865782E-116", Number.EpsilonOf(-1.0e-100).ToString(format), "G");
            Assert.AreEqual("1.99584030953472E+292", Number.EpsilonOf(double.MaxValue).ToString(format), "H");
            Assert.AreEqual("1.99584030953472E+292", Number.EpsilonOf(double.MinValue).ToString(format), "I");
            Assert.AreEqual("4.94065645841247E-324", Number.EpsilonOf(double.Epsilon).ToString(format), "J");
            Assert.AreEqual("4.94065645841247E-324", Number.EpsilonOf(-double.Epsilon).ToString(format), "K");
            Assert.IsTrue(double.IsNaN(Number.EpsilonOf(double.NaN)), "L");
            Assert.IsTrue(double.IsNaN(Number.EpsilonOf(double.PositiveInfinity)), "M");
            Assert.IsTrue(double.IsNaN(Number.EpsilonOf(double.NegativeInfinity)), "N");
        }

        [Test]
        public void TestLexicographicalOrder()
        {
            Assert.AreEqual(2, Number.ToLexicographicalOrderedUInt64(2 * double.Epsilon), "A");
            Assert.AreEqual(1, Number.ToLexicographicalOrderedUInt64(1 * double.Epsilon), "B");
            Assert.AreEqual(0, Number.ToLexicographicalOrderedUInt64(0.0), "C");
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, Number.ToLexicographicalOrderedUInt64(-1 * double.Epsilon), "D");
            Assert.AreEqual(0xFFFFFFFFFFFFFFFE, Number.ToLexicographicalOrderedUInt64(-2 * double.Epsilon), "E");

            Assert.AreEqual(2, Number.ToLexicographicalOrderedInt64(2 * double.Epsilon), "N");
            Assert.AreEqual(1, Number.ToLexicographicalOrderedInt64(1 * double.Epsilon), "O");
            Assert.AreEqual(0, Number.ToLexicographicalOrderedInt64(0.0), "P");
            Assert.AreEqual(-1, Number.ToLexicographicalOrderedInt64(-1 * double.Epsilon), "Q");
            Assert.AreEqual(-2, Number.ToLexicographicalOrderedInt64(-2 * double.Epsilon), "R");
        }

        [Test]
        public void TestSignedMagnitudeToTwosComplement()
        {
            Assert.AreEqual(2, Number.SignedMagnitudeToTwosComplementUInt64(2), "A");
            Assert.AreEqual(1, Number.SignedMagnitudeToTwosComplementUInt64(1), "B");
            Assert.AreEqual(0, Number.SignedMagnitudeToTwosComplementUInt64(0), "C");
            Assert.AreEqual(0, Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808), "D");
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808 + 1), "E");
            Assert.AreEqual(0xFFFFFFFFFFFFFFFE, Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808 + 2), "F");

            Assert.AreEqual(2, Number.SignedMagnitudeToTwosComplementInt64(2), "M");
            Assert.AreEqual(1, Number.SignedMagnitudeToTwosComplementInt64(1), "O");
            Assert.AreEqual(0, Number.SignedMagnitudeToTwosComplementInt64(0), "P");
            Assert.AreEqual(0, Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808), "Q");
            Assert.AreEqual(-1, Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808 + 1), "R");
            Assert.AreEqual(-2, Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808 + 2), "S");
        }

        [Test]
        public void TestNumbersBetween()
        {
            Assert.AreEqual(0, Number.NumbersBetween(1.0, 1.0), "A");
            Assert.AreEqual(0, Number.NumbersBetween(0, 0), "B");
            Assert.AreEqual(0, Number.NumbersBetween(-1.0, -1.0), "C");
            Assert.AreEqual(1, Number.NumbersBetween(0, double.Epsilon), "D");
            Assert.AreEqual(1, Number.NumbersBetween(0, -double.Epsilon), "E");
            Assert.AreEqual(1, Number.NumbersBetween(double.Epsilon, 0), "D2");
            Assert.AreEqual(1, Number.NumbersBetween(-double.Epsilon, 0), "E2");
            Assert.AreEqual(2, Number.NumbersBetween(0, 2*double.Epsilon), "F");
            Assert.AreEqual(2, Number.NumbersBetween(0, -2 * double.Epsilon), "G");
            Assert.AreEqual(3, Number.NumbersBetween(-double.Epsilon, 2 * double.Epsilon), "H");
            Assert.AreEqual(3, Number.NumbersBetween(double.Epsilon, -2 * double.Epsilon), "I");

            double test = Math.PI * 1e+150;
            Assert.AreEqual(10, Number.NumbersBetween(test, test + 10 * Number.EpsilonOf(test)), "J");
            Assert.AreEqual(10, Number.NumbersBetween(test, test - 10 * Number.EpsilonOf(test)), "K");

            Assert.AreEqual(450359962737, Number.NumbersBetween(1.0001, 1.0002), "L");
            Assert.AreEqual(54975582, Number.NumbersBetween(10000.0001, 10000.0002), "M");
            Assert.AreEqual(53687, Number.NumbersBetween(10000000.0001, 10000000.0002), "N");
            Assert.AreEqual(53, Number.NumbersBetween(10000000000.0001, 10000000000.0002), "O");

            Assert.AreEqual(0xFFDFFFFFFFFFFFFE, Number.NumbersBetween(double.MinValue, double.MaxValue), "R");
        }

        [Test]
        public void TestAlmostEqual()
        {
            IFormatProvider format = System.Globalization.CultureInfo.InvariantCulture;

            double max = double.MaxValue;
            double min = double.MinValue;

            Assert.IsTrue(Number.AlmostEqual(0.0, 0.0, 0), "A");
            Assert.IsTrue(Number.AlmostEqual(0.0, 0.0, 50), "B");
            Assert.IsTrue(Number.AlmostEqual(max, max, 0), "C");
            Assert.IsTrue(Number.AlmostEqual(min, min, 0), "D");

            Assert.IsFalse(Number.AlmostEqual(0.0, 0.0 + double.Epsilon, 0), "E");
            Assert.IsTrue(Number.AlmostEqual(0.0, 0.0 + double.Epsilon, 1), "F");

            Assert.IsFalse(Number.AlmostEqual(max, max - 2 * Number.EpsilonOf(max), 0), "G");
            Assert.IsFalse(Number.AlmostEqual(max, max - 2 * Number.EpsilonOf(max), 1), "H");
            Assert.IsTrue(Number.AlmostEqual(max, max - 2 * Number.EpsilonOf(max), 2), "I");

            Assert.IsTrue(Convert.ToDouble("3.170404", format) == 3.170404, "J");
            Assert.IsFalse(Convert.ToDouble("4.170404", format) == 4.170404, "K");

            Assert.IsTrue(Number.AlmostEqual(Convert.ToDouble("3.170404", format), 3.170404, 0), "L");
            Assert.IsFalse(Number.AlmostEqual(Convert.ToDouble("4.170404", format), 4.170404, 0), "M");
            Assert.IsTrue(Number.AlmostEqual(Convert.ToDouble("4.170404", format), 4.170404, 1), "N");

            Assert.IsFalse(Number.AlmostEqual(double.NaN, double.NaN, 25), "O");
            Assert.IsFalse(Number.AlmostEqual(double.PositiveInfinity, double.NegativeInfinity, 25), "P");
            Assert.IsTrue(Number.AlmostEqual(double.PositiveInfinity, double.PositiveInfinity, 25), "Q");
        }
    }
}
