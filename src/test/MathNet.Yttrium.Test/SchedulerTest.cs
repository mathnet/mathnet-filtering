//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.StdPackage.Structures;

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class SchedulerTest
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
        public void Scheduler_SignalTransportTest()
        {
            Signal a = new Signal(project.Context);
            Assert.IsNull(a.Value);

            a.PostNewValue(new IntegerValue(1));
            Assert.IsNull(a.Value);

            Assert.AreEqual(TimeSpan.Zero, project.SimulateFor(100));
            Assert.IsInstanceOfType(typeof(IntegerValue), a.Value);
            Assert.AreEqual(1, ((IntegerValue)a.Value).Value);

            a.PostNewValue(new IntegerValue(2),new TimeSpan(0,1,0));
            Assert.AreEqual(1, ((IntegerValue)a.Value).Value);
            Assert.AreEqual(new TimeSpan(0, 1, 0), project.SimulateFor(new TimeSpan(0, 0, 30)));
            Assert.AreEqual(2, ((IntegerValue)a.Value).Value);
        }

        //[TestMethod]
        [Test]
        public void Scheduler_PortTransportTest()
        {
            Signal a = new Signal(project.Context);
            Signal b = new Signal(project.Context);
            Signal c = a / b;

            Signal d = new Signal(project.Context);
            Signal e = new Signal(project.Context);
            Signal f = project.Builder.Function("xor", d, e);

            a.PostNewValue(new RationalValue(1,1));
            b.PostNewValue(new IntegerValue(2));
            d.PostNewValue(LogicValue.True);
            e.PostNewValue(LogicValue.False);

            Assert.IsNull(a.Value);
            Assert.IsNull(b.Value);
            Assert.IsNull(c.Value);
            Assert.IsNull(d.Value);
            Assert.IsNull(e.Value);
            Assert.IsNull(f.Value);

            Assert.AreEqual(TimeSpan.Zero, project.SimulateFor(1));
            Assert.IsNotNull(a.Value);
            Assert.IsNotNull(b.Value);
            Assert.IsNull(c.Value);
            Assert.IsNotNull(d.Value);
            Assert.IsNotNull(e.Value);
            Assert.IsNull(f.Value);

            Assert.AreEqual(TimeSpan.Zero, project.SimulateFor(1));
            Assert.IsNotNull(c.Value);
            Assert.IsInstanceOfType(typeof(RationalValue), c.Value);
            Assert.IsNotNull(f.Value);
            Assert.IsInstanceOfType(typeof(LogicValue), f.Value);

            Assert.AreEqual(1, ((RationalValue)c.Value).NumeratorValue);
            Assert.AreEqual(2, ((RationalValue)c.Value).DenominatorValue);
            Assert.AreEqual(ELogicX01.True, ((LogicValue)f.Value).Value);

            a.PostNewValue(new IntegerValue(3), new TimeSpan(0, 1, 0));
            Assert.AreEqual(1, ((RationalValue)c.Value).NumeratorValue);
            Assert.AreEqual(2, ((RationalValue)c.Value).DenominatorValue);
            Assert.AreEqual(new TimeSpan(0, 1, 0), project.SimulateFor(new TimeSpan(0, 2, 0)));
            Assert.IsInstanceOfType(typeof(RationalValue), c.Value);
            Assert.AreEqual(3, ((RationalValue)c.Value).NumeratorValue);
            Assert.AreEqual(2, ((RationalValue)c.Value).DenominatorValue);
        }

        //[TestMethod]
        [Test]
        public void Scheduler_DelayedSchedulerTest()
        {
            Signal a = new Signal(project.Context);
            Signal b = new Signal(project.Context);
            Signal c = a * b;

            a.PostNewValue(new IntegerValue(5));

            for(int i=1;i<=1000;i++)
                b.PostNewValue(new IntegerValue(i),new TimeSpan(0,0,i));

            Assert.AreEqual(new TimeSpan(0, 1, 0), project.SimulateFor(new TimeSpan(0, 1, 0)));
            Assert.AreEqual(300, ((IntegerValue)c.Value).Value);

            Assert.AreEqual(new TimeSpan(0, 0, 40), project.SimulateFor(new TimeSpan(0, 0, 40)));
            Assert.AreEqual(500, ((IntegerValue)c.Value).Value);

            Assert.AreEqual(new TimeSpan(0, 15, 0), project.SimulateFor(new TimeSpan(0, 0, 900)));
            Assert.AreEqual(5000, ((IntegerValue)c.Value).Value);
        }
    }
}
