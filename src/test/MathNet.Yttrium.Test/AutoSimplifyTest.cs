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

using MathNet.Numerics;
using MathNet.Symbolics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Interpreter;
using MathNet.Symbolics.Backend.Channels;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.Standard.Properties;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Formatter;

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class AutoSimplifyTest
    {
        private Project _p;
        private IFormatter _f;

        [SetUp]
        public void Initialize()
        {
            _p = new Project();
            _f = Service<IFormatter>.Instance;
        }

        [TearDown]
        public void Cleanup()
        {
            _p = null;
            _f = null;
        }

        [Test]
        public void AutoSimplify_RationalNumberExpression()
        {
            // A
            Signal a =
                StdBuilder.Add(
                    StdBuilder.Divide(IntegerValue.Constant(2), IntegerValue.Constant(3)),
                    RationalValue.Constant(3, 4));
            Assert.AreEqual("2/3+3/4", _f.Format(a, FormattingOptions.Compact), "A1");

            Signal aS = Std.AutoSimplify(a);
            Assert.AreEqual("17/12", _f.Format(aS, FormattingOptions.Compact), "A2");

            // B
            Signal b =
                StdBuilder.Power(
                    StdBuilder.Divide(IntegerValue.Constant(4), IntegerValue.Constant(2)),
                    IntegerValue.Constant(3));
            Assert.AreEqual("(4/2)^3", _f.Format(b, FormattingOptions.Compact), "B1");

            Signal bS = Std.AutoSimplify(b);
            Assert.AreEqual("8", _f.Format(bS, FormattingOptions.Compact), "B2");

            // C
            Signal c =
                StdBuilder.Divide(
                    IntegerValue.ConstantOne,
                    StdBuilder.Subtract(
                        RationalValue.Constant(2, 4),
                        RationalValue.ConstantHalf));
            Assert.AreEqual("1/(1/2-1/2)", _f.Format(c, FormattingOptions.Compact), "C1");

            Signal cS = Std.AutoSimplify(c);
            Assert.AreEqual("Std.Undefined", _f.Format(cS, FormattingOptions.Compact), "C2");

            // D
            Signal d =
                StdBuilder.Power(
                    StdBuilder.Power(
                        IntegerValue.Constant(5),
                        IntegerValue.Constant(2)),
                    StdBuilder.Power(
                        IntegerValue.Constant(3),
                        IntegerValue.Constant(1)));
            Assert.AreEqual("(5^2)^(3^1)", _f.Format(d, FormattingOptions.Compact), "D1");

            Signal dS = Std.AutoSimplify(d);
            Assert.AreEqual("15625", _f.Format(dS, FormattingOptions.Compact), "D2");
        }

        [Test]
        public void AutoSimplify_MathOp_AlgebraDerive()
        {
            IMathSystem s = _p.CurrentSystem;

            _p.Interpret(@"Signal x;");
            _p.Interpret(@"Signal fn;");
            _p.Interpret(@"Signal fdiv;");
            _p.Interpret(@"Signal fdiff;");

            _p.Interpret(@"fn <- 3*exp(-3*x)-x;");
            _p.Interpret(@"fdiff <- diff(3*exp(-3*x)-x, x);");
            _p.Interpret(@"fdiv <- fn / fdiff;");

            Signal x = s.LookupNamedSignal("x");
            Std.ConstrainAlwaysReal(x);

            Signal fdiv = s.LookupNamedSignal("fdiv");
            Assert.AreEqual(new MathIdentifier("Divide", "Std"), fdiv.DrivenByPort.Entity.EntityId, "A");
            
            Signal fdiv_n = fdiv.DrivenByPort.InputSignals[0];
            Signal fdiv_d = fdiv.DrivenByPort.InputSignals[1];
            Assert.AreEqual(new MathIdentifier("Subtract", "Std"), fdiv_n.DrivenByPort.Entity.EntityId, "B");
            Assert.AreEqual(new MathIdentifier("Derive", "Std"), fdiv_d.DrivenByPort.Entity.EntityId, "C");

            // Execute MathOp Std.Derive
            Signal simplified = Std.AutoSimplify(fdiv);
            Assert.AreEqual("(3*exp(-3*x)+-1*x)*(-1+-9*exp(-3*x))^(-1)", _f.Format(simplified, FormattingOptions.Compact), "D");
            Assert.AreEqual(new MathIdentifier("Multiply", "Std"), simplified.DrivenByPort.Entity.EntityId, "E");

            Signal simplified_n = simplified.DrivenByPort.InputSignals[0];
            Signal simplified_d = simplified.DrivenByPort.InputSignals[1];
            Assert.AreEqual(new MathIdentifier("Add", "Std"), simplified_n.DrivenByPort.Entity.EntityId, "F");
            Assert.AreEqual(new MathIdentifier("Power", "Std"), simplified_d.DrivenByPort.Entity.EntityId, "G");

            s.PromoteAsInput(x);
            s.AddSignalTree(simplified, true, false);
            //s.PromoteAsOutput(simplified);
            s.RemoveUnusedObjects();

            // The Derive Mapping should be removed from the system
            Assert.IsFalse(s.GetAllPorts().Exists(delegate(Port port) { return port.Entity.EqualsById(new MathIdentifier("Derive", "Std")); }), "H");

            Assert.AreEqual(-0.3, s.Evaluate(0.0)[0], 1e-8, "x=0.0");
            Assert.AreEqual(.5874238104, s.Evaluate(1.0)[0], 1e-8, "x=1.0");
            Assert.AreEqual(3.139070661, s.Evaluate(Constants.Pi)[0], 1e-8, "x=Pi");
            Assert.AreEqual(-.3334664750, s.Evaluate(-2.5)[0], 1e-8, "x=-2.5");
        }
    }
}
