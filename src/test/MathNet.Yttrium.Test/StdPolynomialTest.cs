#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Symbolics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.Standard;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class StdPolynomialTest
    {
        private Project project;

        [SetUp]
        public void Initialize()
        {
            this.project = new Project();
        }

        [TearDown]
        public void Cleanup()
        {
            this.project = null;
        }

        [Test]
        public void StdPolynomial_SingleVariable()
        {
            Signal x = Binder.CreateSignal();

            Signal a0 = IntegerValue.ConstantZero;
            Signal a1 = IntegerValue.ConstantOne;
            Signal a2 = RationalValue.ConstantHalf;
            Signal a3 = IntegerValue.Constant(2);
            Signal a4 = IntegerValue.Constant(3);

            Signal badX = StdBuilder.Sine(x);
            Signal badA2 = StdBuilder.Tangent(a2);
            Signal badA3 = RealValue.ConstantPI;

            Signal x2 = StdBuilder.Power(x, a3);
            Signal x3 = StdBuilder.Power(x, a4);

            Signal a3x3 = a3 * x3;
            Signal a2x2 = a2 * x2;

            Assert.IsTrue(Polynomial.IsMonomial(x, x), "x: is SVM(x)");
            Assert.IsTrue(Polynomial.IsMonomial(a0, x), "0: is SVM(x)");
            Assert.IsTrue(Polynomial.IsMonomial(a2, x), "1/2: is SVM(x)");
            Assert.IsFalse(Polynomial.IsMonomial(badX, x), "sin(x): is not SVM(x)");
            Assert.IsFalse(Polynomial.IsMonomial(badA2, x), "tan(1/2): is not SVM(x)");
            Assert.IsFalse(Polynomial.IsMonomial(badA3, x), "pi is not SVM(x)");
            Assert.IsTrue(Polynomial.IsMonomial(x3, x), "x^3: is SVM(x)");
            Assert.IsTrue(Polynomial.IsMonomial(a2x2, x), "1/2*x^2: is SVM(x)");

            Assert.AreEqual("Std.Integer(1)", Polynomial.MonomialDegree(x, x).ToString(), "x: SVM deg(x)=1");
            Assert.AreEqual("Std.Integer(0)", Polynomial.MonomialDegree(a2, x).ToString(), "1/2: SVM deg(x)=0");
            Assert.AreEqual("Std.NegativeInfinity", Polynomial.MonomialDegree(a0, x).ToString(), "0: SVM deg(x)=-inf");
            Assert.AreEqual("Std.Integer(3)", Polynomial.MonomialDegree(x3, x).ToString(), "x^3: SVM deg(x)=3");
            Assert.AreEqual("Std.Integer(2)", Polynomial.MonomialDegree(a2x2, x).ToString(), "1/2*x^2: SVM deg(x)=2");

            IValueStructure vs;
            Signal test = Polynomial.MonomialCoefficient(x, x, out vs);
            Assert.AreEqual("Std.Integer(1)", vs.ToString(), "x: SVM coeff deg=1 ");
            Assert.IsTrue(test.Value.Equals(a1.Value), "x: SVM coeff = 1");

            test = Polynomial.MonomialCoefficient(a2, x, out vs);
            Assert.AreEqual("Std.Integer(0)", vs.ToString(), "1/2: SVM coeff deg=0 ");
            Assert.IsTrue(test.Value.Equals(a2.Value), "1/2: SVM coeff = 1/2");

            test = Polynomial.MonomialCoefficient(a3x3, x, out vs);
            Assert.AreEqual("Std.Integer(3)", vs.ToString(), "2*x^3: SVM coeff deg=3 ");
            Assert.IsTrue(test.Value.Equals(a3.Value), "2*x^3: SVM coeff = 2");

            Signal a3x3_a2x2_a4 = StdBuilder.Add(a3x3, a2x2, a4);

            Assert.IsFalse(Polynomial.IsMonomial(a3x3_a2x2_a4, x), "2*x^3+1/2*x^2+3: is not SVM(x)");
            Assert.IsTrue(Polynomial.IsPolynomial(a3x3_a2x2_a4, x), "2*x^3+1/2*x^2+3: is SVP(x)");
            Assert.AreEqual("Std.Integer(3)", Polynomial.PolynomialDegree(a3x3_a2x2_a4, x).ToString(), "2*x^3+1/2*x^2+3: SVP deg(x)=3");

            test = Polynomial.PolynomialCoefficient(a3x3_a2x2_a4, x, 1);
            Assert.IsTrue(test.Value.Equals(a0.Value), "2*x^3+1/2*x^2+3: SVP coeff(1) = 0");

            test = Polynomial.PolynomialCoefficient(a3x3_a2x2_a4, x, 2);
            Assert.IsTrue(test.Value.Equals(a2.Value), "2*x^3+1/2*x^2+3: SVP coeff(2) = 1/2");

            Signal[] coefficients = Polynomial.PolynomialCoefficients(a3x3_a2x2_a4, x);
            Assert.AreEqual(4, coefficients.Length, "2*x^3+1/2*x^2+3: SVP coeffs: len(coeffs) = 4 (-> deg=3)");
            Assert.IsTrue(coefficients[0].Value.Equals(a4.Value), "2*x^3+1/2*x^2+3: SVP coeffs: coeffs[0] = 3");
            Assert.IsTrue(coefficients[1].Value.Equals(a0.Value), "2*x^3+1/2*x^2+3: SVP coeffs: coeffs[1] = 0");
            Assert.IsTrue(coefficients[2].Value.Equals(a2.Value), "2*x^3+1/2*x^2+3: SVP coeffs: coeffs[2] = 1/2");
            Assert.IsTrue(coefficients[3].Value.Equals(a3.Value), "2*x^3+1/2*x^2+3: SVP coeffs: coeffs[3] = 2");
        }

        [Test]
        public void PolynomialDivisionTest()
        {
            Signal x = Binder.CreateSignal();

            Signal c0 = IntegerValue.ConstantZero;
            Signal c1 = IntegerValue.ConstantOne;
            Signal c3 = IntegerValue.Constant(3);
            Signal c3n = IntegerValue.Constant(-3);
            Signal c5 = IntegerValue.Constant(5);

            Signal a = Polynomial.ConstructPolynomial(x, c5, c1, c1, c3);
            Signal b = Polynomial.ConstructPolynomial(x, c1, c3n, c5);

            Assert.AreEqual(c5.Value, Polynomial.PolynomialCoefficient(a, x, 0).Value, "A01");
            Assert.AreEqual(c1.Value, Polynomial.PolynomialCoefficient(a, x, 1).Value, "A02");
            Assert.AreEqual(c1.Value, Polynomial.PolynomialCoefficient(a, x, 2).Value, "A03");
            Assert.AreEqual(c3.Value, Polynomial.PolynomialCoefficient(a, x, 3).Value, "A04");
            Assert.AreEqual(c1.Value, Polynomial.PolynomialCoefficient(b, x, 0).Value, "A05");
            Assert.AreEqual(c3n.Value, Polynomial.PolynomialCoefficient(b, x, 1).Value, "A06");
            Assert.AreEqual(c5.Value, Polynomial.PolynomialCoefficient(b, x, 2).Value, "A07");

            Signal remainder;
            Signal quotient = Polynomial.PolynomialDivision(a, b, x, out remainder);

            Signal[] quotientCoeff = Polynomial.PolynomialCoefficients(quotient, x);
            Assert.AreEqual(2, quotientCoeff.Length, "B01");
            Assert.AreEqual(new RationalValue(14, 25), quotientCoeff[0].Value, "B02");
            Assert.AreEqual(new RationalValue(3, 5), quotientCoeff[1].Value, "B03");

            Signal[] remainderCoeff = Polynomial.PolynomialCoefficients(remainder, x);
            Assert.AreEqual(2, remainderCoeff.Length, "C01");
            Assert.AreEqual(new RationalValue(111, 25), remainderCoeff[0].Value, "C02");
            Assert.AreEqual(new RationalValue(52, 25), remainderCoeff[1].Value, "C03");

        }
    }
}
