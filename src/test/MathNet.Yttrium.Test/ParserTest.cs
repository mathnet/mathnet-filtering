//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Parsing;
using MathNet.Symbolics.Backend.Channels;
using MathNet.Symbolics.StdPackage;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.StdPackage.Properties;
using MathNet.Numerics;

namespace Yttrium.UnitTests
{
    //[TestClass]
    [TestFixture]
    public class ParserTest
    {
        //[TestMethod]
        [Test]
        public void Parser_SimpleExpressions()
        {
            Project p = new Project();
            LogObserver lo = new LogObserver(new TextLogWriter(Console.Out));
            p.AttachLocalObserver(lo);
            MathSystem s = p.CurrentSystem;

            Assert.AreEqual(0, s.SignalCount, "0");

            p.Interpret("signal x;");
            Assert.AreEqual(1, s.SignalCount, "A1");
            Assert.IsTrue(s.ContainsNamedSignal("x"), "A2");

            p.Interpret("x");
            Assert.AreEqual(1, s.SignalCount, "B1");

            p.Interpret("y <- x^2");
            Assert.AreEqual(3, s.SignalCount, "C1");
            Assert.AreEqual("Std.Power", s.LookupNamedSignal("y").DrivenByPort.Entity.EntityId.ToString(), "C2");
            Assert.AreEqual("x", s.LookupNamedSignal("y").DrivenByPort.InputSignals[0].Label, "C3");
            Assert.AreEqual("Std.Integer(2)", s.LookupNamedSignal("y").DrivenByPort.InputSignals[1].Value.ToString(), "C4");

            p.Interpret("instantiate + in a,b out c;");
            Assert.AreEqual(6, s.SignalCount, "D1");
            Assert.AreEqual("Std.Add", s.LookupNamedSignal("c").DrivenByPort.Entity.EntityId.ToString(), "D2");
            Assert.AreEqual("a", s.LookupNamedSignal("c").DrivenByPort.InputSignals[0].Label, "D3");
            Assert.AreEqual("b", s.LookupNamedSignal("c").DrivenByPort.InputSignals[1].Label, "D4");

            p.Interpret("d <- diff(a*b,a);");
            Assert.AreEqual(8, s.SignalCount, "E1");
            Assert.AreEqual("Std.Derive", s.LookupNamedSignal("d").DrivenByPort.Entity.EntityId.ToString(), "E2");
            Assert.AreEqual("Std.Multiply", s.LookupNamedSignal("d").DrivenByPort.InputSignals[0].DrivenByPort.Entity.EntityId.ToString(), "E3");
            Assert.AreEqual("a", s.LookupNamedSignal("d").DrivenByPort.InputSignals[1].Label, "E4");
        }

        //[TestMethod]
        [Test]
        public void Parser_StructuralExpressions()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            s.AddNamedSignal("w", new RealValue(0.1));
            p.SimulateInstant();

            p.Interpret("define entity Test \"test\" function in x,c out y;");
            Assert.IsTrue(p.Context.Library.ContainsEntity(new MathIdentifier("Test", "Work")),"01");
            Assert.AreEqual("Work.Test", p.Context.Library.LookupEntity("test", 2).EntityId.ToString());

            p.Interpret("instantiate Work.Test in x->a,c->a*b out res1;\nres2 <- test(a*b,b);");
            Assert.AreEqual("Work.Test", s.LookupNamedSignal("res1").DrivenByPort.Entity.EntityId.ToString(),"02");
            Assert.AreEqual("Work.Test", s.LookupNamedSignal("res2").DrivenByPort.Entity.EntityId.ToString(),"03");
            Assert.AreEqual("Std.Multiply", s.LookupNamedSignal("res1").DrivenByPort.InputSignals[1].DrivenByPort.Entity.EntityId.ToString(),"04");
            Assert.AreEqual("Std.Multiply", s.LookupNamedSignal("res2").DrivenByPort.InputSignals[0].DrivenByPort.Entity.EntityId.ToString(),"05");

            p.Interpret("define architecture TestArch Test { y <- x * sin(c) +w; };");
            Assert.IsTrue(p.Context.Library.Architectures.ContainsEntity(new MathIdentifier("Test", "Work")),"06");

            s.LookupNamedSignal("a").PostNewValue(new RealValue(0.25));
            s.LookupNamedSignal("b").PostNewValue(new RealValue(0.75));
            p.SimulateInstant();
            Signal res1 = s.LookupNamedSignal("res1");
            Signal res2 = s.LookupNamedSignal("res2");
            Assert.AreEqual("Work.TestArch", res1.DrivenByPort.CurrentArchitecture.Identifier.ToString());
            Assert.AreEqual("Work.TestArch", res2.DrivenByPort.CurrentArchitecture.Identifier.ToString());
            Assert.AreEqual(0.1466, Math.Round(RealValue.ConvertFrom(res1.Value).Value, 4));
            Assert.AreEqual(0.2278, Math.Round(RealValue.ConvertFrom(res2.Value).Value, 4));
        }
    }
}
