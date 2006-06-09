using NUnit.Framework;

using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.SystemBuilder;
using MathNet.Symbolics.StdPackage;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.StdPackage.Properties;
using MathNet.Numerics;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class SystemBuilderTest
    {
        [Test]
        public void SystemToSystemCloneTest()
        {
            Project p = new Project();
            MathSystem s1 = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // BUILD SYSTEM 1: sin(x^2)
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo(c);
            s1.AddSignalTree(sinx2t2, true, true);

            // EVALUATE SYSTEM 1 FOR x=1.5
            x.PostNewValue(new RealValue(1.5));
            p.SimulateInstant();
            Assert.AreEqual(0, s1.BusCount, "A0");
            Assert.AreEqual(5, s1.SignalCount, "A1");
            Assert.AreEqual(3, s1.PortCount, "A2");
            Assert.AreEqual("Std.Real(1.55614639377584)", sinx2t2.Value.ToString(), "A3");

            // CLONE SYSTEM 1 TO SYSTEM 2
            /* 
             * HINT: would be simpler to just call:
             * MathSystem s2 = s1.Clone();
             */
            SystemWriter writer = new SystemWriter(c);
            SystemReader reader = new SystemReader(writer);
            reader.ReadSystem(s1);
            MathSystem s2 = writer.WrittenSystems.Dequeue();

            Assert.AreEqual(0, s2.BusCount, "B0");
            Assert.AreEqual(5, s2.SignalCount, "B1");
            Assert.AreEqual(3, s2.PortCount, "B2");
            Assert.AreEqual("Std.Real(1.55614639377584)", s2.GetOutput(0).Value.ToString(), "B3");

            // EVALUATE SYSTEM 2 FOR x=2.5
            s2.GetInput(0).PostNewValue(new RealValue(2.5));
            p.SimulateInstant();
            Assert.AreEqual("Std.Real(-0.0663584330951136)", s2.GetOutput(0).Value.ToString(), "C0");

            // CHECK SYSTEM 1 STILL ON x=1.5
            Assert.AreEqual("Std.Real(1.5)", x.Value.ToString(), "D0");
            Assert.AreEqual("Std.Real(1.55614639377584)", sinx2t2.Value.ToString(), "D1");
        }

        [Test]
        public void SystemToXmlSerializeTest()
        {
            Project p = new Project();
            MathSystem s1 = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // BUILD SYSTEM 1: sin(x^2)*2
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo(c);
            s1.AddSignalTree(sinx2t2, true, true);

            // EVALUATE SYSTEM 1 FOR x=1.5
            x.PostNewValue(new RealValue(1.5));
            p.SimulateInstant();
            Assert.AreEqual(0, s1.BusCount, "A0");
            Assert.AreEqual(5, s1.SignalCount, "A1");
            Assert.AreEqual(3, s1.PortCount, "A2");
            Assert.AreEqual("Std.Real(1.55614639377584)", sinx2t2.Value.ToString(), "A3");

            // SERIALIZE SYSTEM 1 TO XML
            /* 
             * HINT: would be simpler to just call:
             * string s2xml = s1.WriteXml(false);
             */
            StringBuilder sb = new StringBuilder();
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;
                settings.OmitXmlDeclaration = false;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.Entitize;
                settings.Encoding = Context.DefaultEncoding;
                XmlWriter xwriter = XmlWriter.Create(sb, settings);
                xwriter.WriteStartElement("Systems");
                XmlSystemWriter writer = new XmlSystemWriter(c, xwriter);
                SystemReader reader = new SystemReader(writer);
                reader.ReadSystem(s1);
                xwriter.WriteEndElement();
                xwriter.Flush();
                xwriter.Close();
            }
            string s2xml = sb.ToString();
            Console.WriteLine(s2xml);

            // READER XML BACK TO SYSTEM 2
            /* 
             * HINT: would be simpler to just call:
             * MathSystem s2 = MathSystem.ReadXml(s2xml, c);
             */
            MathSystem s2;
            {
                StringReader sr = new StringReader(s2xml);
                XmlReader xreader = XmlReader.Create(sr);
                xreader.ReadToFollowing("Systems");
                xreader.Read();
                SystemWriter writer = new SystemWriter(c);
                XmlSystemReader reader = new XmlSystemReader(writer);
                reader.ReadSystems(xreader, false);
                xreader.ReadEndElement();
                s2 = writer.WrittenSystems.Dequeue();
            }

            Assert.AreEqual(0, s2.BusCount, "B0");
            Assert.AreEqual(5, s2.SignalCount, "B1");
            Assert.AreEqual(3, s2.PortCount, "B2");
            Assert.AreEqual("Std.Real(1.55614639377584)", s2.GetOutput(0).Value.ToString(), "B3");

            // EVALUATE SYSTEM 2 FOR x=2.5
            s2.GetInput(0).PostNewValue(new RealValue(2.5));
            p.SimulateInstant();
            Assert.AreEqual("Std.Real(-0.0663584330951136)", s2.GetOutput(0).Value.ToString(), "C0"); //-0.0331792165475568
        }

        [Test]
        public void SystemToExpressionTest()
        {
            Project p = new Project();
            MathSystem s1 = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // BUILD SYSTEM 1: sin(x^2)*2
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo(c);
            s1.AddSignalTree(sinx2t2, true, true);

            // EVALUATE SYSTEM 1 FOR x=1.5
            x.PostNewValue(new RealValue(1.5));
            p.SimulateInstant();
            Assert.AreEqual(0, s1.BusCount, "A0");
            Assert.AreEqual(5, s1.SignalCount, "A1");
            Assert.AreEqual(3, s1.PortCount, "A2");
            Assert.AreEqual("Std.Real(1.55614639377584)", sinx2t2.Value.ToString(), "A3");

            // SERIALIZE SYSTEM 1 TO EXPRESSION
            ExpressionWriter writer = new ExpressionWriter();
            SystemReader reader = new SystemReader(writer);
            reader.ReadSystem(s1);
            string expr = writer.WrittenExpressions.Dequeue();
            Console.WriteLine(expr);

            // ....
        }
    }
}
