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

        [Test]
        public void TestSpecialFunctions_Erf()
        {
            // Compare Erf with Maple: "evalf(erf(x),10);"
            Assert.AreEqual(.0, Fn.Erf(0.0), 0.000001, "A1");
            Assert.AreEqual(.1124629160, Fn.Erf(0.1), 0.000001, "A2");
            Assert.AreEqual(.2227025892, Fn.Erf(0.2), 0.000001, "A3");
            Assert.AreEqual(.3286267595, Fn.Erf(0.3), 0.000001, "A4");
            Assert.AreEqual(.4283923550, Fn.Erf(0.4), 0.000001, "A5");
            Assert.AreEqual(.5204998778, Fn.Erf(0.5), 0.000001, "A6");
            Assert.AreEqual(.6038560908, Fn.Erf(0.6), 0.000001, "A7");
            Assert.AreEqual(.6778011938, Fn.Erf(0.7), 0.000001, "A8");
            Assert.AreEqual(.7421009647, Fn.Erf(0.8), 0.000001, "A9");
            Assert.AreEqual(.7969082124, Fn.Erf(0.9), 0.000001, "A10");
            Assert.AreEqual(.8427007929, Fn.Erf(1.0), 0.000001, "A11");
            Assert.AreEqual(.8802050696, Fn.Erf(1.1), 0.000001, "A12");
            Assert.AreEqual(.9103139782, Fn.Erf(1.2), 0.000001, "A13");
            Assert.AreEqual(.9999779095, Fn.Erf(3.0), 0.000001, "A14");
            Assert.AreEqual(1.0, Fn.Erf(9.0), 0.000001, "A15");
            Assert.AreEqual(1.0, Fn.Erf(100), 0.000001, "A16");
            Assert.AreEqual(-.3286267595, Fn.Erf(-0.3), 0.000001, "A17");
            Assert.AreEqual(-.7421009647, Fn.Erf(-0.8), 0.000001, "A18");

            // Compare ErvInverse with Maple: "erfinv := y -> RootOf(-erf(_Z)+y); evalf(erfinv(x),10);"
            Assert.AreEqual(.0, Fn.ErfInverse(0.0), 0.000001, "B1");
            Assert.AreEqual(.8885599049e-1, Fn.ErfInverse(0.1), 0.000001, "B2");
            Assert.AreEqual(.1791434546, Fn.ErfInverse(0.2), 0.000001, "B3");
            Assert.AreEqual(.2724627147, Fn.ErfInverse(0.3), 0.000001, "B4");
            Assert.AreEqual(.3708071586, Fn.ErfInverse(0.4), 0.000001, "B5");
            Assert.AreEqual(.4769362762, Fn.ErfInverse(0.5), 0.000001, "B6");
            Assert.AreEqual(.5951160814, Fn.ErfInverse(0.6), 0.000001, "B7");
            Assert.AreEqual(.7328690780, Fn.ErfInverse(0.7), 0.000001, "B8");
            Assert.AreEqual(.9061938024, Fn.ErfInverse(0.8), 0.000001, "B9");
            Assert.AreEqual(1.163087154, Fn.ErfInverse(0.9), 0.000001, "B10");
            Assert.AreEqual(2.751063906, Fn.ErfInverse(0.9999), 0.000001, "B11");
            Assert.AreEqual(3.766562582, Fn.ErfInverse(0.9999999), 0.000001, "B12");
            Assert.AreEqual(-.2724627147, Fn.ErfInverse(-0.3), 0.000001, "B13");
            Assert.AreEqual(-.9061938024, Fn.ErfInverse(-0.8), 0.000001, "B14");
            Assert.AreEqual(.8862271575e-3, Fn.ErfInverse(0.001), 0.000001, "B15");
            Assert.AreEqual(.4431163629e-2, Fn.ErfInverse(0.005), 0.000001, "B16");
        }
    }
}
