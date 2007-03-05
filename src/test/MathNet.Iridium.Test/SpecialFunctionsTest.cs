using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using NUnit.Framework;

using MathNet.Numerics;

namespace Iridium.Test
{
    [TestFixture]
    public class SpecialFunctionsTest
    {
        [Test]
        public void TestSpecialFunctions_IntLog2()
        {
            Assert.AreEqual(0, Fn.IntLog2(1), "A");
            Assert.AreEqual(1, Fn.IntLog2(2), "B");
            Assert.AreEqual(2, Fn.IntLog2(3), "C");
            Assert.AreEqual(2, Fn.IntLog2(4), "D");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.AreEqual(i, Fn.IntLog2(pow), pow.ToString());
                Assert.AreEqual(i, Fn.IntLog2(pow - 1), pow.ToString() + "-1");
                Assert.AreEqual(i + 1, Fn.IntLog2(pow + 1), pow.ToString() + "+1");
            }

            Assert.AreEqual(31, Fn.IntLog2(int.MaxValue), "Y");
            Assert.AreEqual(31, Fn.IntLog2(int.MaxValue-1), "Z");
        }

        [Test]
        public void TestSpecialFunctions_FloorToPowerOf2()
        {
            Assert.AreEqual(0, Fn.FloorToPowerOf2(0), "A");
            Assert.AreEqual(1, Fn.FloorToPowerOf2(1), "B");
            Assert.AreEqual(2, Fn.FloorToPowerOf2(2), "C");
            Assert.AreEqual(2, Fn.FloorToPowerOf2(3), "D");
            Assert.AreEqual(4, Fn.FloorToPowerOf2(4), "E");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.AreEqual(pow, Fn.FloorToPowerOf2(pow), pow.ToString());
                Assert.AreEqual(pow >> 1, Fn.FloorToPowerOf2(pow - 1), pow.ToString() + "-1");
            }

            Assert.AreEqual((int.MaxValue >> 1) + 1, Fn.FloorToPowerOf2(int.MaxValue), "Z");
        }

        [Test]
        public void TestSpecialFunctions_CeilingToPowerOf2()
        {
            Assert.AreEqual(0, Fn.CeilingToPowerOf2(0), "A");
            Assert.AreEqual(1, Fn.CeilingToPowerOf2(1), "B");
            Assert.AreEqual(2, Fn.CeilingToPowerOf2(2), "C");
            Assert.AreEqual(4, Fn.CeilingToPowerOf2(3), "D");
            Assert.AreEqual(4, Fn.CeilingToPowerOf2(4), "E");

            for(int i = 2; i < 31; i++)
            {
                int pow = (int)Math.Pow(2.0, i);
                Assert.AreEqual(pow, Fn.CeilingToPowerOf2(pow), pow.ToString());
                Assert.AreEqual(pow, Fn.CeilingToPowerOf2(pow - 1), pow.ToString() + "-1");
            }

            Assert.AreEqual((int.MaxValue >> 1) + 1, Fn.CeilingToPowerOf2((int.MaxValue >> 1) + 1), "Y");
            Assert.AreEqual((int.MaxValue >> 1) + 1, Fn.CeilingToPowerOf2((int.MaxValue >> 1)), "Z");
        }
    }
}
