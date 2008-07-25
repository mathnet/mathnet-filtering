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
using System.Globalization;

using NUnit.Framework;

using MathNet.Numerics;

namespace Iridium.Test
{
    [TestFixture]
    public class ComplexTest
    {
        [Test]
        public void TestParser_Invariant()
        {
            // Invariant Culture
            Complex c1 = Complex.Parse("1231.2", NumberFormatInfo.InvariantInfo);
            Assert.AreEqual(1231.2d, c1.Real, "A1");
            Assert.AreEqual(0.0d, c1.Imag, "A2");

            Complex c2 = Complex.Parse("1.5-I*34.56", NumberFormatInfo.InvariantInfo);
            Assert.AreEqual(1.5d, c2.Real, 0.01, "B1");
            Assert.AreEqual(-34.56, c2.Imag, 0.001, "B2");

            Complex c3 = Complex.Parse("-1.5 + I * -34.56", NumberFormatInfo.InvariantInfo);
            Assert.AreEqual(-1.5d, c3.Real, 0.01, "C1");
            Assert.AreEqual(-34.56, c3.Imag, 0.001, "C2");

            Complex c4 = Complex.Parse("-34.56 * I", NumberFormatInfo.InvariantInfo);
            Assert.AreEqual(0.0d, c4.Real, 0.01, "D1");
            Assert.AreEqual(-34.56, c4.Imag, 0.001, "D2");
        }

        [Test]
        public void TestParser_DE_CH()
        {
            // Swiss (German) Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("de-CH").NumberFormat;
            Complex c1 = Complex.Parse("1'231.2", format);
            Assert.AreEqual(1231.2d, c1.Real, "A1");
            Assert.AreEqual(0.0d, c1.Imag, "A2");
        }

        [Test]
        public void TestParser_DE_DE()
        {
            // German Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("de-DE").NumberFormat;
            Complex c1 = Complex.Parse("1.231,2", format);
            Assert.AreEqual(1231.2d, c1.Real, "A1");
            Assert.AreEqual(0.0d, c1.Imag, "A2");
        }

        [Test]
        public void TestParser_TH_TH() 
        {
            // Thai Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("th-TH").NumberFormat;
            //format.DigitSubstitution = DigitShapes.NativeNational;
            string number = (1231.2).ToString(format);
            Complex c1 = Complex.Parse(number, format);
            Assert.AreEqual(1231.2d, c1.Real, "A1");
            Assert.AreEqual(0.0d, c1.Imag, "A2");
        }

        [Test]
        public void TestNaturalLogarithm()
        {
            // ln(0) = -infty
            Complex zero = Complex.Zero;
            Complex lnZero = zero.NaturalLogarithm();
            Assert.AreEqual(double.NegativeInfinity, lnZero.Real, "Re{ln(0)} = -infinity");
            Assert.AreEqual(0d, lnZero.Imag, "Im{ln(0)} = 0");

            // ln(1) = 0
            Complex one = Complex.One;
            Complex lnOne = one.NaturalLogarithm();
            Assert.AreEqual(0d, lnOne.Real, "Re{ln(1)} = 0");
            Assert.AreEqual(0d, lnOne.Imag, "Im{ln(1)} = 0");

            // ln(i) = Pi/2 * i
            Complex I = Complex.I;
            Complex lnI = I.NaturalLogarithm();
            Assert.AreEqual(0d, lnI.Real, "Re{ln(i)} = 0");
            Assert.AreEqual(Constants.Pi_2, lnI.Imag, "Im{ln(i)} = Pi/2");

            // ln(-1) = Pi * i
            Complex mOne = -Complex.One;
            Complex lnMOne = mOne.NaturalLogarithm();
            Assert.AreEqual(0d, lnMOne.Real, "Re{ln(-1)} = 0");
            Assert.AreEqual(Constants.Pi, lnMOne.Imag, "Im{ln(-1)} = Pi");

            // ln(-i) = -Pi/2 * i
            Complex mI = -Complex.I;
            Complex lnMI = mI.NaturalLogarithm();
            Assert.AreEqual(0d, lnMI.Real, "Re{ln(-i)} = 0");
            Assert.AreEqual(-Constants.Pi_2, lnMI.Imag, "Im{ln(-i)} = -Pi/2");

            // ln(i+1) = ln(2)/2 + Pi/4 * i
            Complex onePlusI = Complex.One + Complex.I;
            Complex lnOnePlusI = onePlusI.NaturalLogarithm();
            Assert.AreEqual(Constants.Ln2 * 0.5, lnOnePlusI.Real, "Re{ln(i+1)} = ln(2)/2");
            Assert.AreEqual(Constants.Pi_4, lnOnePlusI.Imag, "Im{ln(i+1)} = Pi/4");
        }

        [Test]
        public void TestExponential()
        {
            // exp(0) = 1
            Complex zero = Complex.Zero;
            Complex expZero = zero.Exponential();
            Assert.AreEqual(1d, expZero.Real, "Re{exp(0)} = 1");
            Assert.AreEqual(0d, expZero.Imag, "Im{exp(0)} = 0");

            // exp(1) = e
            Complex one = Complex.One;
            Complex expOne = one.Exponential();
            Assert.AreEqual(Constants.E, expOne.Real, "Re{exp(1)} = e");
            Assert.AreEqual(0d, expOne.Imag, "Im{exp(1)} = 0");

            // exp(i) = cos(1) + sin(1) * i
            Complex I = Complex.I;
            Complex expI = I.Exponential();
            Assert.AreEqual(Trig.Cosine(1d), expI.Real, "Re{exp(i)} = cos(1)");
            Assert.AreEqual(Trig.Sine(1d), expI.Imag, "Im{exp(i)} = sin(1)");

            // exp(-1) = 1/e
            Complex mOne = -Complex.One;
            Complex expMOne = mOne.Exponential();
            Assert.AreEqual(1.0 / Constants.E, expMOne.Real, "Re{exp(-1)} = 1/e");
            Assert.AreEqual(0d, expMOne.Imag, "Im{exp(-1)} = 0");

            // exp(-i) = cos(1) - sin(1) * i
            Complex mI = -Complex.I;
            Complex expMI = mI.Exponential();
            Assert.AreEqual(Trig.Cosine(1d), expMI.Real, "Re{exp(-i)} = cos(1)");
            Assert.AreEqual(-Trig.Sine(1d), expMI.Imag, "Im{exp(-i)} = -sin(1)");

            // exp(i+1) = e * cos(1) + e * sin(1) * i
            Complex onePlusI = Complex.One + Complex.I;
            Complex expOnePlusI = onePlusI.Exponential();
            Assert.AreEqual(Constants.E * Trig.Cosine(1d), expOnePlusI.Real, "Re{exp(i+1)} = e * cos(1)");
            Assert.AreEqual(Constants.E * Trig.Sine(1d), expOnePlusI.Imag, "Im{exp(i+1)} = e * sin(1)");
        }

        [Test]
        public void TestPower()
        {
            // (1)^(1) = 1
            Complex one = Complex.One;
            Complex onePowOne = one.Power(one);
            Assert.AreEqual(1d, onePowOne.Real, "Re{(1)^(1)} = 1");
            Assert.AreEqual(0d, onePowOne.Imag, "Im{(1)^(1)} = 0");

            // (i)^(1) = i
            Complex I = Complex.I;
            Complex IPowOne = I.Power(one);
            NumericAssert.AreAlmostEqual(0d, IPowOne.Real, "Re{(i)^(1)} = 0");
            Assert.AreEqual(1d, IPowOne.Imag, "Im{(i)^(1)} = 1");

            // (1)^(-1) = 1
            Complex mOne = -Complex.One;
            Complex onePowMOne = one.Power(mOne);
            Assert.AreEqual(1d, onePowMOne.Real, "Re{(1)^(-1)} = 1");
            Assert.AreEqual(0d, onePowMOne.Imag, "Im{(1)^(-1)} = 0");

            // (i)^(-1) = -i
            Complex IPowMOne = I.Power(mOne);
            NumericAssert.AreAlmostEqual(0d, IPowMOne.Real, "Re{(i)^(-1)} = 0");
            Assert.AreEqual(-1d, IPowMOne.Imag, "Im{(i)^(-1)} = -1");

            // (i)^(-i) = exp(Pi/2)
            Complex mI = -Complex.I;
            Complex IPowMI = I.Power(mI);
            Assert.AreEqual(Math.Exp(Constants.Pi_2), IPowMI.Real, "Re{(i)^(-i)} = exp(Pi/2)");
            Assert.AreEqual(0d, IPowMI.Imag, "Im{(i)^(-i)} = 0");

            // (0)^(0) = 1
            Assert.AreEqual(1d, Math.Pow(0d, 0d), "(0)^(0) = 1 (.Net Framework Sanity Check)");
            Complex zero = Complex.Zero;
            Complex zeroPowZero = zero.Power(zero);
            Assert.AreEqual(1d, zeroPowZero.Real, "Re{(0)^(0)} = 1");
            Assert.AreEqual(0d, zeroPowZero.Imag, "Im{(0)^(0)} = 0");

            // (0)^(2) = 0
            Assert.AreEqual(0d, Math.Pow(0d, 2d), "(0)^(2) = 0 (.Net Framework Sanity Check)");
            Complex two = new Complex(2d, 0d);
            Complex zeroPowTwo = zero.Power(two);
            Assert.AreEqual(0d, zeroPowTwo.Real, "Re{(0)^(2)} = 0");
            Assert.AreEqual(0d, zeroPowTwo.Imag, "Im{(0)^(2)} = 0");

            // (0)^(-2) = infty
            Assert.AreEqual(double.PositiveInfinity, Math.Pow(0d, -2d), "(0)^(-2) = infty (.Net Framework Sanity Check)");
            Complex mTwo = Complex.FromRealImaginary(-2d, 0d);
            Complex zeroPowMTwo = zero.Power(mTwo);
            Assert.AreEqual(double.PositiveInfinity, zeroPowMTwo.Real, "Re{(0)^(-2)} = infty");
            Assert.AreEqual(0d, zeroPowMTwo.Imag, "Im{(0)^(-2)} = 0");

            // (0)^(I) = NaN
            Complex zeroPowI = zero.Power(I);
            Assert.AreEqual(double.NaN, zeroPowI.Real, "Re{(0)^(I)} = NaN");
            Assert.AreEqual(double.NaN, zeroPowI.Imag, "Im{(0)^(I)} = NaN");

            // (0)^(-I) = NaN
            Complex zeroPowMI = zero.Power(mI);
            Assert.AreEqual(double.NaN, zeroPowMI.Real, "Re{(0)^(-I)} = NaN");
            Assert.AreEqual(double.NaN, zeroPowMI.Imag, "Im{(0)^(-I)} = NaN");

            // (0)^(1+I) = 0
            Complex onePlusI = Complex.One + Complex.I;
            Complex zeroPowOnePlusI = zero.Power(onePlusI);
            Assert.AreEqual(0d, zeroPowOnePlusI.Real, "Re{(0)^(1+I)} = 0");
            Assert.AreEqual(0d, zeroPowOnePlusI.Imag, "Im{(0)^(1+I)} = 0");

            // (0)^(1-I) = 0
            Complex oneMinusI = Complex.One - Complex.I;
            Complex zeroPowOneMinusI = zero.Power(oneMinusI);
            Assert.AreEqual(0d, zeroPowOneMinusI.Real, "Re{(0)^(1-I)} = 0");
            Assert.AreEqual(0d, zeroPowOneMinusI.Imag, "Im{(0)^(1-I)} = 0");

            // (0)^(-1+I) = infty + infty * i
            Complex minusOnePlusI = new Complex(-1d, 1d);
            Complex zeroPowMinusOnePlusI = zero.Power(minusOnePlusI);
            Assert.AreEqual(double.PositiveInfinity, zeroPowMinusOnePlusI.Real, "Re{(0)^(-1+I)} = infty");
            Assert.AreEqual(double.PositiveInfinity, zeroPowMinusOnePlusI.Imag, "Im{(0)^(-1+I)} = infty");
        }

        [Test]
        public void TestDivision()
        {
            // 0/1
            Complex zeroDivOne = Complex.Zero / Complex.One;
            Assert.AreEqual(0, zeroDivOne.Real, "Re{0/1} = 0");
            Assert.AreEqual(0, zeroDivOne.Imag, "Im{0/1} = 0");

            // 1/0
            // TODO: verify this is really what should happen
            Complex oneDivZero = Complex.One / Complex.Zero;
            Assert.AreEqual(double.PositiveInfinity, oneDivZero.Real, "Re{1/0} = infty");
            Assert.AreEqual(double.PositiveInfinity, oneDivZero.Imag, "Im{1/0} = infty");

            // (1+2I)/(3+4I)
            Complex onePlus2I = new Complex(1, 2);
            Complex threePlus4I = new Complex(3, 4);
            Complex onPlus2IDivthreePlus4I = onePlus2I / threePlus4I;
            Assert.AreEqual(11d / 25d, onPlus2IDivthreePlus4I.Real, "Re{(1+2I)/(3+4I)} = 11/25");
            Assert.AreEqual(2d / 25d, onPlus2IDivthreePlus4I.Imag, "Im{(1+2I)/(3+4I)} = 2/25");

            // (big+big*I)/(2*big+2*big*I)
            double big1 = double.MaxValue / 4;
            double big2 = double.MaxValue / 2;
            Complex big1PlusBig1I = new Complex(big1, big1);
            Complex big2PlusBig2I = new Complex(big2, big2);
            Complex big1PlusBig1IDivBig2PlusBig2I = big1PlusBig1I / big2PlusBig2I;
            Console.WriteLine(big1PlusBig1IDivBig2PlusBig2I.Real);
            Assert.AreEqual(0.5, big1PlusBig1IDivBig2PlusBig2I.Real, "Re{(big+big*I)/(2*big+2*big*I)} = 0.5");
            Assert.AreEqual(0, big1PlusBig1IDivBig2PlusBig2I.Imag, "Im{((big+big*I)/(2*big+2*big*I)} = 0");
        }
    }
}
