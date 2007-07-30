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
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

using MathNet.Numerics;
using MathNet.Symbolics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Properties;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.SystemBuilder.Toolkit;

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

            // BUILD SYSTEM 1: sin(x^2)
            Signal x = Binder.CreateSignal(); x.Label = "x";
            Std.ConstrainAlwaysReal(x);
            Signal x2 = StdBuilder.Square(x); x2.Label = "x2";
            Signal sinx2 = StdBuilder.Sine(x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo;
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
            SystemWriter writer = new SystemWriter();
            SystemReader reader = new SystemReader(writer);
            reader.ReadSystem(s1);
            IMathSystem s2 = writer.WrittenSystems.Dequeue();

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

            // BUILD SYSTEM 1: sin(x^2)*2
            Signal x = Binder.CreateSignal(); x.Label = "x";
            Std.ConstrainAlwaysReal(x);
            Signal x2 = StdBuilder.Square(x); x2.Label = "x2";
            Signal sinx2 = StdBuilder.Sine(x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo;
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
                settings.Encoding = Config.InternalEncoding;
                XmlWriter xwriter = XmlWriter.Create(sb, settings);
                xwriter.WriteStartElement("Systems");
                XmlSystemWriter writer = new XmlSystemWriter(xwriter);
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
            IMathSystem s2;
            {
                StringReader sr = new StringReader(s2xml);
                XmlReader xreader = XmlReader.Create(sr);
                xreader.ReadToFollowing("Systems");
                xreader.Read();
                SystemWriter writer = new SystemWriter();
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

            // BUILD SYSTEM 1: sin(x^2)*2
            Signal x = Binder.CreateSignal(); x.Label = "x";
            Std.ConstrainAlwaysReal(x);
            Signal x2 = StdBuilder.Square(x); x2.Label = "x2";
            Signal sinx2 = StdBuilder.Sine(x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo;
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
