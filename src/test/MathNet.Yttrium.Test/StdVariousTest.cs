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

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Symbolics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Structures;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class StdVariousTest
    {
        private Project _project;

        [SetUp]
        public void Initialize()
        {
            _project = new Project();
        }

        [TearDown]
        public void Cleanup()
        {
            _project = null;
        }

        [Test]
        public void ConstraintsTest()
        {
            Signal signal = IntegerValue.ConstantOne;
            Assert.IsTrue(Std.IsConstant(signal), "A01");
            Assert.IsTrue(Std.IsAlwaysInteger(signal), "A02");
            Assert.IsTrue(Std.IsAlwaysRational(signal), "A03");
            Assert.IsTrue(Std.IsAlwaysReal(signal), "A04");
            Assert.IsTrue(Std.IsAlwaysPositive(signal), "A05");
            Assert.IsTrue(Std.IsAlwaysNonnegative(signal), "A06");
            Assert.IsTrue(Std.IsAlwaysPositiveInteger(signal), "A07");
            Assert.IsTrue(Std.IsAlwaysNonnegativeInteger(signal), "A08");

            signal = IntegerValue.ConstantZero;
            Assert.IsTrue(Std.IsConstant(signal), "B01");
            Assert.IsTrue(Std.IsAlwaysInteger(signal), "B02");
            Assert.IsTrue(Std.IsAlwaysRational(signal), "B03");
            Assert.IsTrue(Std.IsAlwaysReal(signal), "B04");
            Assert.IsFalse(Std.IsAlwaysPositive(signal), "B05");
            Assert.IsTrue(Std.IsAlwaysNonnegative(signal), "B06");
            Assert.IsFalse(Std.IsAlwaysPositiveInteger(signal), "B07");
            Assert.IsTrue(Std.IsAlwaysNonnegativeInteger(signal), "B08");

            signal = IntegerValue.ConstantMinusOne;
            Assert.IsTrue(Std.IsConstant(signal), "C01");
            Assert.IsTrue(Std.IsAlwaysInteger(signal), "C02");
            Assert.IsTrue(Std.IsAlwaysRational(signal), "C03");
            Assert.IsTrue(Std.IsAlwaysReal(signal), "C04");
            Assert.IsFalse(Std.IsAlwaysPositive(signal), "C05");
            Assert.IsFalse(Std.IsAlwaysNonnegative(signal), "C06");
            Assert.IsFalse(Std.IsAlwaysPositiveInteger(signal), "C07");
            Assert.IsFalse(Std.IsAlwaysNonnegativeInteger(signal), "C08");
        }
    }
}
