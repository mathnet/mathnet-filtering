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
        public void TestNaturalLogarithmOfNegativeReal()
        {
            // Regression Test for Bug IRID-77

            Complex minusOne = -Complex.One;
            Complex piI = minusOne.NaturalLogarithm();

            Assert.AreEqual(0.0, piI.Real, 1e-8, "Re{ln(-1)} = 0");
            Assert.AreEqual(Constants.Pi, piI.Imag, 1e-8, "Im{ln(-1)} = Pi");

        }
    }
}
