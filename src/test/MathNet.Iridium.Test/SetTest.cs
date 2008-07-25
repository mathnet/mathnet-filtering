#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;

namespace Iridium.Test
{
    [TestFixture]
    public class SetTest
    {
        [Test]
        public void IntersectionTest()
        {
            Set<int> a = new Set<int>();
            for(int i = 0; i < 16; i += 2)
                a.Add(i);

            Set<int> b = new Set<int>();
            for(int i = 0; i < 16; i += 3)
                b.Add(i);

            Set<int> inter = a.Intersect(b);

            Assert.AreEqual(3, inter.Count, "A01");
            Assert.AreEqual(0, inter[0], "A02");
            Assert.AreEqual(6, inter[1], "A03");
            Assert.AreEqual(12, inter[2], "A04");
        }
    }
}
