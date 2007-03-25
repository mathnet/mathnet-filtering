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
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Traversing;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Packages.Standard;

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class BasicTest
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
        public void Basic_EntityLookupTest()
        {
            ILibrary library = Service<ILibrary>.Instance;

            IEntity addEntity = library.LookupEntity(new MathIdentifier("Add", "Std"));
            IEntity subtractEntity = library.LookupEntity("-", 2);

            Assert.AreEqual("+", addEntity.Symbol);
            Assert.AreEqual("Std.Subtract", subtractEntity.EntityId.ToString());
        }

        //[TestMethod]
        [Test]
        public void Basic_SignalTest()
        {
            Signal a = Binder.CreateSignal();
            Signal b = Binder.CreateSignal();

            Signal c = a + b;

            Assert.AreEqual("Std.Add", c.DrivenByPort.Entity.EntityId.ToString());
        }

        //[TestMethod]
        [Test]
        public void Basic_CycleTest()
        {
            Signal a = Binder.CreateSignal(); a.Label = "A";
            Signal b = Binder.CreateSignal(); b.Label = "B";

            Signal c = a + b; c.Label = "C";
            Signal d = a * c; d.Label = "D"; 

            Assert.AreEqual(0, a.Cycles, "Cycle A0");
            Assert.AreEqual(0, b.Cycles, "Cycle A1");
            Assert.AreEqual(0, c.Cycles, "Cycle A2");
            Assert.AreEqual(0, d.Cycles, "Cycle A3");

            Service<IBuilder>.Instance.MapSignals(c, b);

            Assert.AreEqual(0, a.Cycles, "Cycle B0");
            Assert.AreEqual(1, b.Cycles, "Cycle B1");
            Assert.AreEqual(1, c.Cycles, "Cycle B2");
            Assert.AreEqual(0, d.Cycles, "Cycle B3");

            Service<IBuilder>.Instance.MapSignals(d, a);

            Assert.AreEqual(2, a.Cycles, "Cycle C0");
            Assert.AreEqual(1, b.Cycles, "Cycle C1");
            Assert.AreEqual(2, c.Cycles, "Cycle C2");
            Assert.AreEqual(2, d.Cycles, "Cycle C3");

            List<List<Signal>> paths = Service<IScanner>.Instance.FindAllSignalPathsFrom(d, a, false);
            Assert.AreEqual(2, paths.Count,"Paths 0");
            Assert.AreEqual(a, paths[0][0], "Paths 1");
            Assert.AreEqual(d, paths[0][1], "Paths 2");
            Assert.AreEqual(a, paths[1][0], "Paths 3");
            Assert.AreEqual(c, paths[1][1], "Paths 4");
            Assert.AreEqual(d, paths[1][2], "Paths 5");

            Signal e = Binder.CreateSignal(); e.Label = "E";
    
            Signal f =  Service<IManipulator>.Instance.Substitute(d, c, e); //d.Substitute(c, e); f.Label = "F";

            Assert.AreEqual(f, f.DrivenByPort.InputSignals[0].DrivenByPort.InputSignals[0], "Paths2 X");

            List<List<Signal>> paths2 = Service<IScanner>.Instance.FindAllSignalPathsFrom(f, e, false);
            Assert.AreEqual(1, paths2.Count, "Paths2 0");
            Assert.AreEqual(e, paths2[0][0], "Paths2 1");
            Assert.AreEqual(f, paths2[0][1], "Paths2 2");

            Assert.AreEqual(0, Service<IScanner>.Instance.FindAllSignalPathsFrom(f, a, false).Count, "Paths3");
            Assert.AreEqual(0, Service<IScanner>.Instance.FindAllSignalPathsFrom(f, d, false).Count, "Paths4");

            paths = Service<IScanner>.Instance.FindAllSignalPathsFrom(d, a, false);
            Assert.AreEqual(2, paths.Count, "Paths5 0");
            Assert.AreEqual(a, paths[0][0], "Paths5 1");
            Assert.AreEqual(d, paths[0][1], "Paths5 2");
            Assert.AreEqual(a, paths[1][0], "Paths5 3");
            Assert.AreEqual(c, paths[1][1], "Paths5 4");
            Assert.AreEqual(d, paths[1][2], "Paths5 5");
        }

        //[TestMethod]
        [Test]
        public void Basic_Traversing()
        {
            Signal a = Binder.CreateSignal(); a.Label = "A";
            Signal b = Binder.CreateSignal(); b.Label = "B";
            Signal c = StdBuilder.Add(a, b); c.Label = "C"; //a + b; 
            Signal d = StdBuilder.Multiply(a, c); d.Label = "D"; //a * c; 

            Assert.IsTrue(c.DependsOn(a), "1: a->c");
            Assert.IsTrue(c.DependsOn(b), "1: b->c");
            Assert.IsTrue(d.DependsOn(a), "1: a->d");
            Assert.IsTrue(d.DependsOn(b), "1: b->d");
            Assert.IsTrue(d.DependsOn(c), "1: c->d");
            Assert.IsFalse(a.DependsOn(b), "1: b!->a");
            Assert.IsFalse(b.DependsOn(a), "1: a!->b");
            Assert.IsFalse(a.DependsOn(d), "1: d!->a");
            Assert.IsFalse(b.DependsOn(d), "1: d!->b");
            Assert.IsFalse(c.DependsOn(d), "1: d!->c");
            Assert.IsFalse(a.DependsOn(c), "1: c!->a");
            Assert.IsFalse(b.DependsOn(c), "1: c!->b");

            Service<IBuilder>.Instance.MapSignals(c, b);

            Assert.IsTrue(c.DependsOn(a), "2: a->c");
            Assert.IsTrue(c.DependsOn(b), "2: b->c");
            Assert.IsTrue(d.DependsOn(a), "2: a->d");
            Assert.IsTrue(d.DependsOn(b), "2: b->d");
            Assert.IsTrue(d.DependsOn(c), "2: c->d");
            Assert.IsFalse(a.DependsOn(b), "2: b!->a");
            Assert.IsTrue(b.DependsOn(a), "2: a->b"); // NEW
            Assert.IsFalse(a.DependsOn(d), "2: d!->a");
            Assert.IsFalse(b.DependsOn(d), "2: d!->b");
            Assert.IsFalse(c.DependsOn(d), "2: d!->c");
            Assert.IsFalse(a.DependsOn(c), "2: c!->a");
            Assert.IsTrue(b.DependsOn(c), "2: c->b");  // NEW

            Assert.IsTrue(Service<IScanner>.Instance.ExistsSignal(d, delegate(Signal s) { return s.Label == "A"; }, false));
            Assert.IsFalse(Service<IScanner>.Instance.ExistsSignal(d, delegate(Signal s) { return s.Label == "Z"; }, false));
            Assert.AreEqual("B", Service<IScanner>.Instance.FindSignal(d, delegate(Signal s) { return s.Label == "B"; }, false).Label);
        }
    }
}
