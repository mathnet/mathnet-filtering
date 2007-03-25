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
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Library;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class ConversionTest
    {
        [SetUp]
        public void Initialize()
        {
            Service<IPackageLoader>.Instance.LoadStdPackage();
        }

        [Test]
        public void DirectLosslessConversionsTest()
        {
            IntegerValue int1 = new IntegerValue(35);

            RationalValue rat1 = ValueConverter<RationalValue>.ConvertFrom(int1);
            Assert.AreEqual(35, rat1.NumeratorValue, "01");
            Assert.AreEqual(1, rat1.DenominatorValue, "02");

            RealValue real1 = ValueConverter<RealValue>.ConvertFrom(rat1);
            Assert.AreEqual(35d, real1.Value, 0.0001d, "03");
        }

        [Test]
        public void IndirectLosslessConversionsTest()
        {
            ToggleValue toggle1 = ToggleValue.InitialToggle;
            Assert.AreEqual("Std.Toggle(A)", toggle1.ToString(), "01");

            RationalValue rat1 = ValueConverter<RationalValue>.ConvertFrom(toggle1);
            Assert.AreEqual(1, rat1.NumeratorValue, "02");
            Assert.AreEqual(1, rat1.DenominatorValue, "03");

            RationalValue rat2 = ValueConverter<RationalValue>.ConvertFrom(toggle1.Toggle());
            Assert.AreEqual(0, rat2.NumeratorValue, "04");
            Assert.AreEqual(1, rat2.DenominatorValue, "05");

            RealValue real1 = ValueConverter<RealValue>.ConvertFrom(toggle1);
            Assert.AreEqual(1d, real1.Value, 0.0001d, "06");
        }
    }
}
