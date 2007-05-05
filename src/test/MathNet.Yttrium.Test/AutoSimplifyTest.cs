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

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class AutoSimplifyTest
    {
        [Test]
        public void AutoSimplify_MathOp_AlgebraDerive()
        {
            Project p = new Project();
            IMathSystem s = p.CurrentSystem;

            p.Interpret(@"Signal x;");
            p.Interpret(@"Signal fn;");
            p.Interpret(@"Signal fdiv;");
            p.Interpret(@"Signal fdiff;");

            p.Interpret(@"fn <- 3*exp(-3*x)-x;");
            p.Interpret(@"fdiff <- diff(3*exp(-3*x)-x, x);");
            p.Interpret(@"fdiv <- fn / fdiff;");

            Signal x = s.LookupNamedSignal("x");
            x.AddConstraint(RealSetProperty.Instance);

            Signal fdiv = s.LookupNamedSignal("fdiv");
            Assert.AreEqual(new MathIdentifier("Divide", "Std"), fdiv.DrivenByPort.Entity.EntityId, "A");
            
            Signal fdiv_n = fdiv.DrivenByPort.InputSignals[0];
            Signal fdiv_d = fdiv.DrivenByPort.InputSignals[1];
            Assert.AreEqual(new MathIdentifier("Subtract", "Std"), fdiv_n.DrivenByPort.Entity.EntityId, "B");
            Assert.AreEqual(new MathIdentifier("Derive", "Std"), fdiv_d.DrivenByPort.Entity.EntityId, "C");

            // Execute MathOp Std.Derive
            Signal simplified = Std.AutoSimplify(fdiv);
            Assert.AreEqual(new MathIdentifier("Divide", "Std"), simplified.DrivenByPort.Entity.EntityId, "D");

            Signal simplified_n = simplified.DrivenByPort.InputSignals[0];
            Signal simplified_d = simplified.DrivenByPort.InputSignals[1];
            Assert.AreEqual(new MathIdentifier("Subtract", "Std"), simplified_n.DrivenByPort.Entity.EntityId, "E");
            Assert.AreEqual(new MathIdentifier("Subtract", "Std"), simplified_d.DrivenByPort.Entity.EntityId, "F");

            s.PromoteAsInput(x);
            s.AddSignalTree(simplified, true, false);
            //s.PromoteAsOutput(simplified);
            s.RemoveUnusedObjects();

            // The Derive Mapping should be removed from the system
            Assert.IsFalse(s.GetAllPorts().Exists(delegate(Port port) { return port.Entity.EqualsById(new MathIdentifier("Derive", "Std")); }), "G");

            Assert.AreEqual(-0.3, s.Evaluate(0.0)[0], 1e-8, "x=0.0");
            Assert.AreEqual(.5874238104, s.Evaluate(1.0)[0], 1e-8, "x=1.0");
            Assert.AreEqual(3.139070661, s.Evaluate(Constants.Pi)[0], 1e-8, "x=Pi");
            Assert.AreEqual(-.3334664750, s.Evaluate(-2.5)[0], 1e-8, "x=-2.5");
        }
    }
}
