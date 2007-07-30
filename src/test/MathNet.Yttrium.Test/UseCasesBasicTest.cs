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
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Properties;
using MathNet.Symbolics.Packages.Standard.Structures;

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

            Signal x = Binder.CreateSignal();
            Std.ConstrainAlwaysReal(x);
            Signal x2 = StdBuilder.Square(x);
            Signal sinx2 = StdBuilder.Sine(x2);

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

            Signal x = Binder.CreateSignal(); x.Label = "x";
            Signal x2 = StdBuilder.Square(x); x2.Label = "x2";
            Signal secx2 = StdBuilder.Secant(x2); secx2.Label = "secx2";
            Signal diff = Std.Derive(secx2, x); diff.Label = "diff1";

            Assert.AreEqual(0, s.InputCount, "Input Signal Count A");
            Assert.AreEqual(0, s.OutputCount, "Output Signal Count A");
            Assert.AreEqual(0, s.BusCount, "Bus Count A");
            Assert.AreEqual(7, s.SignalCount, "Signal Count A");
            Assert.AreEqual(5, s.PortCount, "Port Count A");

            Signal diff2 = Std.AutoSimplify(diff);
            s.PromoteAsInput(x);
            s.PromoteAsOutput(diff2);
            s.RemoveUnusedObjects();

            Assert.AreEqual(1, s.InputCount, "Input Signal Count B");
            Assert.AreEqual(1, s.OutputCount, "Output Signal Count B");
            Assert.AreEqual(0, s.BusCount, "Bus Count B");
            Assert.AreEqual(7, s.SignalCount, "Signal Count B");
            Assert.AreEqual(5, s.PortCount, "Port Count B");

            IValueStructure vs = s.Evaluate(ComplexValue.I)[0];
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

            Signal x = Binder.CreateSignal();
            Std.ConstrainAlwaysReal(x);
            Signal x2 = StdBuilder.Square(x);
            Signal sinx2 = StdBuilder.Sine(x2);

            s.AddSignalTree(sinx2, true, true);
            s.RemoveUnusedObjects();

            s.PublishToLibrary("SineOfSquaredX", "sinx2");

            Signal y = Binder.CreateSignal();
            y.PostNewValue(RealValue.E);
            Signal z = Service<IBuilder>.Instance.Function("sinx2", y);
            p.SimulateInstant();  //.SimulateFor(new TimeSpan(1)); 
            IValueStructure res = z.Value;
            RealValue resReal = RealValue.ConvertFrom(res);
            Assert.AreEqual(0.8939, Math.Round(resReal.Value, 4));
        }
    }
}
