//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.StdPackage;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.StdPackage.Properties;
using MathNet.Numerics;

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class UseCasesBasicTest
    {
        //[TestMethod]
        [Test]
        public void UseCase_SimpleSystemAsFunction()
        {
            // TODO: Replace with new (easier to use) MathFunction class instead of MathSystem

            Project p = new Project();
            p.KeepTrack = true;
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            Signal x = new Signal(c);
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x);
            Signal sinx2 = Std.Sine(c,x2);

            Assert.AreEqual(0, s.InputCount, "Input Signal Count A");
            Assert.AreEqual(0, s.OutputCount, "Output Signal Count A");
            Assert.AreEqual(0, s.BusCount, "Bus Count A");
            Assert.AreEqual(3, s.SignalCount, "Signal Count A");
            Assert.AreEqual(2, s.PortCount, "Port Count A");

            s.PromoteAsInput(x);
            s.PromoteAsOutput(sinx2);
            s.RemoveUnusedObjects();

            Assert.AreEqual(1, s.InputCount, "Input Signal Count B");
            Assert.AreEqual(1, s.OutputCount, "Output Signal Count B");
            Assert.AreEqual(0, s.BusCount, "Bus Count B");
            Assert.AreEqual(3, s.SignalCount, "Signal Count B");
            Assert.AreEqual(2, s.PortCount, "Port Count B");

            double ret = Math.Round(s.Evaluate(Math.PI)[0],4);

            Assert.AreEqual(-0.4303, ret, "Result");
        }

        //[TestMethod]
        [Test]
        public void UseCase_ComplexDerivedSystemAsFunction()
        {
            // TODO: Replace with new (easier to use) MathFunction class instead of MathSystem

            Project p = new Project();
            p.KeepTrack = true;
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            Signal x = new Signal(c);
            Signal x2 = b.Square(x);
            Signal secx2 = Std.Secant(c, x2);
            Signal diff = Std.Derive(c, secx2, x);

            Assert.AreEqual(0, s.InputCount, "Input Signal Count A");
            Assert.AreEqual(0, s.OutputCount, "Output Signal Count A");
            Assert.AreEqual(0, s.BusCount, "Bus Count A");
            Assert.AreEqual(8, s.SignalCount, "Signal Count A");
            Assert.AreEqual(5, s.PortCount, "Port Count A");

            Signal diff2 = Std.AutoSimplify(c, diff);
            s.PromoteAsInput(x);
            s.PromoteAsOutput(diff2);
            s.RemoveUnusedObjects();

            Assert.AreEqual(1, s.InputCount, "Input Signal Count B");
            Assert.AreEqual(1, s.OutputCount, "Output Signal Count B");
            Assert.AreEqual(0, s.BusCount, "Bus Count B");
            Assert.AreEqual(7, s.SignalCount, "Signal Count B");
            Assert.AreEqual(5, s.PortCount, "Port Count B");

            ValueStructure vs = s.Evaluate(ComplexValue.I)[0];
            Assert.IsInstanceOfType(typeof(ComplexValue), vs, "Result is complex.");
            ComplexValue cv = (ComplexValue)vs;

            Assert.AreEqual(0, Math.Round(cv.RealValue, 4), "Real Result");
            Assert.AreEqual(-5.7649, Math.Round(cv.ImaginaryValue, 4), "Imag Result");
        }

        //[TestMethod]
        [Test]
        public void UseCase_SystemAsCompoundArchitecture()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            Signal x = new Signal(c);
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x);
            Signal sinx2 = Std.Sine(c, x2);

            s.AddSignalTree(sinx2, true, true);
            s.RemoveUnusedObjects();

            s.PublishToLibrary("SineOfSquaredX", "sinx2");

            Signal y = new Signal(c);
            y.PostNewValue(RealValue.E);
            Signal z = p.Builder.Function("sinx2", y);
            p.SimulateInstant();  //.SimulateFor(new TimeSpan(1)); 
            ValueStructure res = z.Value;
            RealValue resReal = RealValue.ConvertFrom(res);
            Assert.AreEqual(0.8939, Math.Round(resReal.Value, 4));
        }
    }
}
