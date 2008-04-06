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
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace Iridium.Test
{
    [TestFixture]
    public class InterpolationTest
    {
        [Test]
        public void TestPolynomialInterpolation_PoleFree()
        {
            double[] t = new double[] { 0.0, 1.0, 3.0, 4.0 };
            double[] x = new double[] { 0.0, 3.0, 1.0, 3.0 };

            InterpolationSingleDimension interp =
                new InterpolationSingleDimension(t, x, InterpolationMode.ExpectNoPoles);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.AreEqual(x[i], interp.Evaluate(t[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "PolynomialInterpolation([[0,0],[1,3],[3,1],[4,3]], x);"
            NumericAssert.AreAlmostEqual(.5722500000, interp.Evaluate(0.1), 1e-8, "A 0.1");
            NumericAssert.AreAlmostEqual(1.884000000, interp.Evaluate(0.4), 1e-8, "A 0.4");
            NumericAssert.AreAlmostEqual(3.031416666, interp.Evaluate(1.1), 1e-8, "A 1.1");
            NumericAssert.AreAlmostEqual(1.034666672, interp.Evaluate(3.2), 1e-8, "A 3.2");
            NumericAssert.AreAlmostEqual(6.281250003, interp.Evaluate(4.5), 1e-8, "A 4.5");
            NumericAssert.AreAlmostEqual(277.5000000, interp.Evaluate(10.0), 1e-8, "A 10.0");
            NumericAssert.AreAlmostEqual(-1010.833333, interp.Evaluate(-10.0), 1e-8, "A -10.0");
        }


        [Test]
        public void TestRationalInterpolation_WithPoles()
        {
            double[] t = new double[] { 0, 1, 3,    4,     5 };
            double[] x = new double[] { 0, 3, 1000, -1000, 3 };

            InterpolationSingleDimension interp =
                new InterpolationSingleDimension(t, x, InterpolationMode.ExpectPoles);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.AreEqual(x[i], interp.Evaluate(t[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "RationalInterpolation([[0,0],[1,3],[3,1000],[4,-1000], [5,3]], x);"
            NumericAssert.AreAlmostEqual(.1938920338, interp.Evaluate(0.1), 1e-8, "A 0.1");
            NumericAssert.AreAlmostEqual(.8813290070, interp.Evaluate(0.4), 1e-8, "A 0.4");
            NumericAssert.AreAlmostEqual(3.505766568, interp.Evaluate(1.1), 1e-8, "A 1.1");
            NumericAssert.AreAlmostEqual(1548.766817, interp.Evaluate(3.01), 1e-7, "A 3.01");
            NumericAssert.AreAlmostEqual(3362.256182, interp.Evaluate(3.02), 1e-7, "A 3.02");
            NumericAssert.AreAlmostEqual(-22332.59394, interp.Evaluate(3.03), 1e-6, "A 3.03");
            NumericAssert.AreAlmostEqual(-440.3032377, interp.Evaluate(3.1), 1e-8, "A 3.1");
            NumericAssert.AreAlmostEqual(-202.4242120, interp.Evaluate(3.2), 1e-8, "A 3.2");
            NumericAssert.AreAlmostEqual(21.20824963, interp.Evaluate(4.5), 1e-8, "A 4.5");
            NumericAssert.AreAlmostEqual(-4.893698696, interp.Evaluate(10.0), 1e-8, "A 10.0");
            NumericAssert.AreAlmostEqual(-3.601758431, interp.Evaluate(-10.0), 1e-8, "A -10.0");
        }
    }
}
