//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;

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
            Library library = project.Context.Library;

            Entity addEntity = library.LookupEntity(new MathIdentifier("Add", "Std"));
            Entity subtractEntity = library.LookupEntity("-", 2);

            Assert.AreEqual("+", addEntity.Symbol);
            Assert.AreEqual("Std.Subtract", subtractEntity.EntityId.ToString());
        }

        //[TestMethod]
        [Test]
        public void Basic_SignalTest()
        {
            Signal a = new Signal(project.Context);
            Signal b = new Signal(project.Context);

            Signal c = a + b;

            Assert.AreEqual("Std.Add", c.DrivenByPort.Entity.EntityId.ToString());
        }

        //[TestMethod]
        [Test]
        public void Basic_CycleTest()
        {
            Signal a = new Signal(project.Context); a.Label = "A";
            Signal b = new Signal(project.Context); b.Label = "B";

            Signal c = a + b; c.Label = "C";
            Signal d = a * c; d.Label = "D";

            Assert.AreEqual(0, a.Cycles, "Cycle A0");
            Assert.AreEqual(0, b.Cycles, "Cycle A1");
            Assert.AreEqual(0, c.Cycles, "Cycle A2");
            Assert.AreEqual(0, d.Cycles, "Cycle A3");

            project.Builder.MapSignals(c, b);

            Assert.AreEqual(0, a.Cycles, "Cycle B0");
            Assert.AreEqual(1, b.Cycles, "Cycle B1");
            Assert.AreEqual(1, c.Cycles, "Cycle B2");
            Assert.AreEqual(0, d.Cycles, "Cycle B3");

            project.Builder.MapSignals(d, a);

            Assert.AreEqual(2, a.Cycles, "Cycle C0");
            Assert.AreEqual(1, b.Cycles, "Cycle C1");
            Assert.AreEqual(2, c.Cycles, "Cycle C2");
            Assert.AreEqual(2, d.Cycles, "Cycle C3");

            List<List<Signal>> paths = Scanner.FindAllSignalPathsFrom(d, a, false);
            Assert.AreEqual(2, paths.Count,"Paths 0");
            Assert.AreEqual(a, paths[0][0], "Paths 1");
            Assert.AreEqual(d, paths[0][1], "Paths 2");
            Assert.AreEqual(a, paths[1][0], "Paths 3");
            Assert.AreEqual(c, paths[1][1], "Paths 4");
            Assert.AreEqual(d, paths[1][2], "Paths 5");

            Signal e = new Signal(project.Context); e.Label = "E";

            Signal f = Scanner.Substitute(d, c, e); //d.Substitute(c, e); f.Label = "F";

            Assert.AreEqual(f, f.DrivenByPort.InputSignals[0].DrivenByPort.InputSignals[0], "Paths2 X");

            List<List<Signal>> paths2 = Scanner.FindAllSignalPathsFrom(f, e, false);
            Assert.AreEqual(1, paths2.Count, "Paths2 0");
            Assert.AreEqual(e, paths2[0][0], "Paths2 1");
            Assert.AreEqual(f, paths2[0][1], "Paths2 2");

            Assert.AreEqual(0, Scanner.FindAllSignalPathsFrom(f, a, false).Count, "Paths3");
            Assert.AreEqual(0, Scanner.FindAllSignalPathsFrom(f, d, false).Count, "Paths4");

            paths = Scanner.FindAllSignalPathsFrom(d, a, false);
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
            Signal a = new Signal(project.Context); a.Label = "A";
            Signal b = new Signal(project.Context); b.Label = "B";
            Signal c = a + b; c.Label = "C";
            Signal d = a * c; d.Label = "D";

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

            project.Builder.MapSignals(c, b);

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

            Assert.IsTrue(Scanner.ExistsSignal(d, delegate(Signal s) { return s.Label == "A"; }, false));
            Assert.IsFalse(Scanner.ExistsSignal(d, delegate(Signal s) { return s.Label == "Z"; }, false));
            Assert.AreEqual("B", Scanner.FindSignal(d, delegate(Signal s) { return s.Label == "B"; }, false).Label);
        }
    }
}
