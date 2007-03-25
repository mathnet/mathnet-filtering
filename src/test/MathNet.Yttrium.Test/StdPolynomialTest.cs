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

//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
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
    //[TestClass]
    [TestFixture]
    public class StdPolynomialTest
    {
        private Project project;

        //[TestInitialize]
        [SetUp]
        public void Initialize()
        {
            this.project = new Project();
        }

        //[TestCleanup]
        public void Cleanup()
        {
            this.project = null;
        }

        //[TestMethod]
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

            Assert.IsTrue(Std.IsMonomial(x, x), "x: is SVM(x)");
            Assert.IsTrue(Std.IsMonomial(a0, x), "0: is SVM(x)");
            Assert.IsTrue(Std.IsMonomial(a2, x), "1/2: is SVM(x)");
            Assert.IsFalse(Std.IsMonomial(badX, x), "sin(x): is not SVM(x)");
            Assert.IsFalse(Std.IsMonomial(badA2, x), "tan(1/2): is not SVM(x)");
            Assert.IsFalse(Std.IsMonomial(badA3, x), "pi is not SVM(x)");
            Assert.IsTrue(Std.IsMonomial(x3, x), "x^3: is SVM(x)");
            Assert.IsTrue(Std.IsMonomial(a2x2, x), "1/2*x^2: is SVM(x)");

            Assert.AreEqual("Std.Integer(1)", Std.MonomialDegree(x, x).ToString(), "x: SVM deg(x)=1");
            Assert.AreEqual("Std.Integer(0)", Std.MonomialDegree(a2, x).ToString(), "1/2: SVM deg(x)=0");
            Assert.AreEqual("Std.NegativeInfinity", Std.MonomialDegree(a0, x).ToString(), "0: SVM deg(x)=-inf");
            Assert.AreEqual("Std.Integer(3)", Std.MonomialDegree(x3, x).ToString(), "x^3: SVM deg(x)=3");
            Assert.AreEqual("Std.Integer(2)", Std.MonomialDegree(a2x2, x).ToString(), "1/2*x^2: SVM deg(x)=2");

            IValueStructure vs;
            Signal test = Std.MonomialCoefficient(x, x, out vs);
            Assert.AreEqual("Std.Integer(1)", vs.ToString(), "x: SVM coeff deg=1 ");
            Assert.IsTrue(test.Equals(a1), "x: SVM coeff = 1");

            test = Std.MonomialCoefficient(a2, x, out vs);
            Assert.AreEqual("Std.Integer(0)", vs.ToString(), "1/2: SVM coeff deg=0 ");
            Assert.IsTrue(test.Equals(a2), "1/2: SVM coeff = 1/2");

            test = Std.MonomialCoefficient(a3x3, x, out vs);
            Assert.AreEqual("Std.Integer(3)", vs.ToString(), "2*x^3: SVM coeff deg=3 ");
            Assert.IsTrue(test.Equals(a3), "2*x^3: SVM coeff = 2");

            Signal a3x3_a2x2_a4 = StdBuilder.Add(a3x3, a2x2, a4);

            Assert.IsFalse(Std.IsMonomial(a3x3_a2x2_a4, x), "2*x^3+1/2*x^2+3: is not SVM(x)");
            Assert.IsTrue(Std.IsPolynomial(a3x3_a2x2_a4, x), "2*x^3+1/2*x^2+3: is SVP(x)");
            Assert.AreEqual("Std.Integer(3)", Std.PolynomialDegree(a3x3_a2x2_a4, x).ToString(), "2*x^3+1/2*x^2+3: SVP deg(x)=3");

            test = Std.PolynomialCoefficient(a3x3_a2x2_a4, x, 1);
            Assert.IsTrue(test.Equals(a0), "2*x^3+1/2*x^2+3: SVP coeff(1) = 0");

            test = Std.PolynomialCoefficient(a3x3_a2x2_a4, x, 2);
            Assert.IsTrue(test.Equals(a2), "2*x^3+1/2*x^2+3: SVP coeff(2) = 1/2");
        }
    }
}
