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
using System.Xml;
using System.Text;
using System.Collections.Generic;

using MathNet.Numerics;
using MathNet.Symbolics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;

using MathNet.Symbolics.Backend.Persistence;
using MathNet.Symbolics.Packages.Standard.Structures;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class MicrokernelTest
    {
        [Test]
        public void SerializeSingleCustomData()
        {
            Project p = new Project();
            Dictionary<Guid, Guid> emptyMappings = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Signal> signals = new Dictionary<Guid, Signal>();
            Dictionary<Guid, Bus> buses = new Dictionary<Guid, Bus>();

            ComplexValue cv = new ComplexValue(Constants.Pi, Constants.E);

            string xml = Serializer.SerializeToString(cv, emptyMappings, emptyMappings);

            ComplexValue ret = Serializer.DeserializeFromString<ComplexValue>(xml, signals, buses);

            Assert.AreEqual(cv.TypeId, ret.TypeId, "B");
            Assert.AreEqual(cv.ToString(), ret.ToString(), "C");
        }

        [Test]
        public void SerializeEmptyCustomDataList()
        {
            Project p = new Project();
            Dictionary<Guid, Guid> emptyMappings = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Signal> signals = new Dictionary<Guid, Signal>();
            Dictionary<Guid, Bus> buses = new Dictionary<Guid, Bus>();

            List<ICustomData> values = new List<ICustomData>();

            string xml = Serializer.SerializeListToString(values, emptyMappings, emptyMappings, "Structures");

            List<ICustomData> ret = Serializer.DeserializeListFromString<ICustomData>(xml, signals, buses, "Structures");

            Assert.AreEqual(0, ret.Count, "A");
        }

        [Test]
        public void SerializeFilledCustomDataList()
        {
            Project p = new Project();
            Dictionary<Guid, Guid> emptyMappings = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Signal> signals = new Dictionary<Guid, Signal>();
            Dictionary<Guid, Bus> buses = new Dictionary<Guid, Bus>();

            LiteralValue lv = new LiteralValue("test");
            IntegerValue iv = new IntegerValue(42);
            ComplexValue cv = new ComplexValue(Constants.Pi, Constants.E);
            RealValue rv = new RealValue(Constants.TwoInvSqrtPi);

            List<IValueStructure> values = new List<IValueStructure>();
            values.Add(lv);
            values.Add(iv);
            values.Add(cv);
            values.Add(rv);

            string xml = Serializer.SerializeListToString(values, emptyMappings, emptyMappings, "Structures");

            List<IValueStructure> ret = Serializer.DeserializeListFromString<IValueStructure>(xml, signals, buses, "Structures");

            Assert.AreEqual(4, ret.Count, "A");
            Assert.AreEqual(values[0].TypeId, ret[0].TypeId, "B1");
            Assert.AreEqual(values[1].TypeId, ret[1].TypeId, "B2");
            Assert.AreEqual(values[2].TypeId, ret[2].TypeId, "B3");
            Assert.AreEqual(values[3].TypeId, ret[3].TypeId, "B4");
            Assert.AreEqual(values[0].ToString(), ret[0].ToString(), "C1");
            Assert.AreEqual(values[1].ToString(), ret[1].ToString(), "C2");
            Assert.AreEqual(values[2].ToString(), ret[2].ToString(), "C3");
            Assert.AreEqual(values[3].ToString(), ret[3].ToString(), "C4");
        }

        [Test]
        public void GetInstanceOfCoreObjects()
        {
            Project p = new Project();
            IEntity ety = new MathNet.Symbolics.Core.Entity("%", new MathIdentifier("Percent", "Test"), InfixNotation.LeftAssociativeInnerOperator, 100, false);

            Signal signal = Binder.CreateSignal();
            Assert.IsNotNull(signal, "A0");
            Assert.AreEqual("MathNet.Symbolics.Core.Signal", signal.GetType().FullName, "A1");
            signal = Binder.GetSpecificInstance<Signal>(new MathIdentifier("Signal", "Core"));
            Assert.IsNotNull(signal, "A2");
            Assert.AreEqual("MathNet.Symbolics.Core.Signal", signal.GetType().FullName, "A3");

            Bus bus = Binder.CreateBus();
            Assert.IsNotNull(bus, "B0");
            Assert.AreEqual("MathNet.Symbolics.Core.Bus", bus.GetType().FullName, "B1");
            bus = Binder.GetSpecificInstance<Bus>(new MathIdentifier("Bus", "Core"));
            Assert.IsNotNull(bus, "B2");
            Assert.AreEqual("MathNet.Symbolics.Core.Bus", bus.GetType().FullName, "B3");

            Port port = Binder.CreatePort(ety);
            Assert.IsNotNull(port, "C0");
            Assert.AreEqual("MathNet.Symbolics.Core.Port", port.GetType().FullName, "C1");
            port = Binder.GetSpecificInstance<Port, IEntity>(new MathIdentifier("Port", "Core"), ety);
            Assert.IsNotNull(port, "C2");
            Assert.AreEqual("MathNet.Symbolics.Core.Port", port.GetType().FullName, "C3");
        }

        [Test]
        public void MathContextStacking()
        {
            MathContext c1 = MathContext.Current;
            Assert.IsNotNull(c1, "01");
            Assert.AreNotEqual(Guid.Empty, c1.InstanceId, "02");
            Guid id1 = c1.InstanceId;
            Assert.AreEqual(id1, MathContext.Current.InstanceId, "03");
            Assert.AreEqual(false, c1.HasParent, "04");
            Assert.IsNull(c1.ParentContext, "05");

            using(MathContext c2 = MathContext.Create())
            {
                Assert.IsNotNull(c2, "06");
                Assert.AreNotEqual(Guid.Empty, c2.InstanceId, "07");
                Assert.AreNotEqual(c1.InstanceId, c2.InstanceId, "08");
                Assert.AreEqual(true, c2.HasParent, "09");
                Assert.AreEqual(c1.InstanceId, c2.ParentContext.InstanceId, "10");
                Assert.AreEqual(id1, c1.InstanceId, "11");
                Assert.AreEqual(c2.InstanceId, MathContext.Current.InstanceId, "12");
            }

            Assert.AreEqual(id1, MathContext.Current.InstanceId, "13");
            Assert.AreEqual(id1, c1.InstanceId, "14");

        }
    }
}
