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
using MathNet.Symbolics.Interpreter;
using MathNet.Symbolics.Formatter;
using MathNet.Symbolics.Backend.Channels;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Library;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class FormatterTest
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
        public void FormatPolynomialTest()
        {
            IFormatter f = Service<IFormatter>.Instance;

            Signal x = Binder.CreateSignal();
            x.Label = "x";

            Signal p = Polynomial.ConstructPolynomial(x,
                new RationalValue(1, 3),
                new RationalValue(4, 5),
                RationalValue.Zero,
                new RationalValue(12, 23));

            Assert.AreEqual("1/3+(4/5)*x+(12/23)*x^3",
                f.Format(p, FormattingOptions.Compact), "A01");
        }

        [Test]
        public void FormatAssociativeTest()
        {
            IFormatter f = Service<IFormatter>.Instance;
            MathSystem s = project.CurrentSystem;
            s.RemoveUnusedObjects();

            project.Interpret("v1 <- (a+b)+(c+d);");
            Signal v1 = s.LookupNamedSignal("v1");
            Assert.AreEqual("(a+b)+(c+d)", f.Format(v1, FormattingOptions.Compact), "V1A");
            Signal v1S = Std.AutoSimplify(v1);
            Assert.AreEqual("a+b+c+d", f.Format(v1S, FormattingOptions.Compact), "V1B");

            project.Interpret("v2 <- (a-b)+(c-d);");
            Signal v2 = s.LookupNamedSignal("v2");
            Assert.AreEqual("(a-b)+(c-d)", f.Format(v2, FormattingOptions.Compact), "V2A");
            Signal v2S = Std.AutoSimplify(v2);
            Assert.AreEqual("a+-1*b+c+-1*d", f.Format(v2S, FormattingOptions.Compact), "V2B");

            project.Interpret("w1 <- (a-b)-(c-d);");
            Signal w1 = s.LookupNamedSignal("w1");
            Assert.AreEqual("(a-b)-(c-d)", f.Format(w1, FormattingOptions.Compact), "W1A");
            Signal w1S = Std.AutoSimplify(w1);
            Assert.AreEqual("a+-1*b+-1*c+d", f.Format(w1S, FormattingOptions.Compact), "W1B");

            project.Interpret("w2 <- (a+b)-(c+d);");
            Signal w2 = s.LookupNamedSignal("w2");
            Assert.AreEqual("(a+b)-(c+d)", f.Format(w2, FormattingOptions.Compact), "W2A");
            Signal w2S = Std.AutoSimplify(w2);
            Assert.AreEqual("a+b+-1*c+-1*d", f.Format(w2S, FormattingOptions.Compact), "W2B");

            project.Interpret("x1 <- (a*b)*(c*d);");
            Signal x1 = s.LookupNamedSignal("x1");
            Assert.AreEqual("(a*b)*(c*d)", f.Format(x1, FormattingOptions.Compact), "X1A");
            Signal x1S = Std.AutoSimplify(x1);
            Assert.AreEqual("a*b*c*d", f.Format(x1S, FormattingOptions.Compact), "X1B");

            project.Interpret("x2 <- (a/b)*(c/d);");
            Signal x2 = s.LookupNamedSignal("x2");
            Assert.AreEqual("(a/b)*(c/d)", f.Format(x2, FormattingOptions.Compact), "X2A");
            Signal x2S = Std.AutoSimplify(x2);
            Assert.AreEqual("a*b^(-1)*c*d^(-1)", f.Format(x2S, FormattingOptions.Compact), "X2B");

            project.Interpret("y1 <- (a/b)/(c/d);");
            Signal y1 = s.LookupNamedSignal("y1");
            Assert.AreEqual("(a/b)/(c/d)", f.Format(y1, FormattingOptions.Compact), "Y1A");
            Signal y1S = Std.AutoSimplify(y1);
            Assert.AreEqual("a*b^(-1)*c^(-1)*d", f.Format(y1S, FormattingOptions.Compact), "Y1B");

            project.Interpret("y2 <- (a*b)/(c*d);");
            Signal y2 = s.LookupNamedSignal("y2");
            Assert.AreEqual("(a*b)/(c*d)", f.Format(y2, FormattingOptions.Compact), "Y2A");
            Signal y2S = Std.AutoSimplify(y2);
            Assert.AreEqual("a*b*c^(-1)*d^(-1)", f.Format(y2S, FormattingOptions.Compact), "Y2B");
        }

        [Test]
        public void FormatPrecedenceTest()
        {
            MathSystem s = project.CurrentSystem;
            s.RemoveUnusedObjects();

            project.Interpret("x <- a+3+b*(c+d)/(1/2+3/4)^(5/6);");
            Signal x = s.LookupNamedSignal("x");

            Assert.AreEqual("(a+3)+(b*(c+d))/(1/2+3/4)^(5/6)",
                Service<IFormatter>.Instance.Format(x, FormattingOptions.Compact), "A01");

            Signal x2 = Std.AutoSimplify(x);

            Assert.AreEqual("3+a+b*(c+d)*((5/4)^(5/6))^(-1)",
                Service<IFormatter>.Instance.Format(x2, FormattingOptions.Compact), "B01");
        }
    }
}
