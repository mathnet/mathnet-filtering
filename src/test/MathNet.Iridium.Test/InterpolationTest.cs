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
using MathNet.Numerics.Interpolation.Algorithms;

namespace Iridium.Test
{
    [TestFixture]
    public class InterpolationTest
    {
        [Test]
        public void TestInterpolationMethod_NevillePolynomial()
        {
            double[] t = new double[] { 0.0, 1.0, 3.0, 4.0 };
            double[] x = new double[] { 0.0, 3.0, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreatePolynomial(t, x);
            Assert.IsInstanceOfType(typeof(PolynomialInterpolation), method, "Type");

            double dx, d2x;

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
                Assert.AreEqual(x[i], method.Differentiate(t[i], out dx, out d2x), "B Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[3,1],[4,3]], x)),20);"
            NumericAssert.AreAlmostEqual(.57225000000000000000, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(.57225000000000000000, method.Differentiate(0.1, out dx, out d2x), 1e-15, "B 0.1");
            NumericAssert.AreAlmostEqual(1.8840000000000000000, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(1.8840000000000000000, method.Differentiate(0.4, out dx, out d2x), 1e-15, "B 0.4");
            NumericAssert.AreAlmostEqual(3.0314166666666666667, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(3.0314166666666666667, method.Differentiate(1.1, out dx, out d2x), 1e-15, "B 1.1");
            NumericAssert.AreAlmostEqual(1.034666666666666667, method.Interpolate(3.2), 1e-15, "A 3.2");
            NumericAssert.AreAlmostEqual(1.034666666666666667, method.Differentiate(3.2, out dx, out d2x), 1e-15, "B 3.2");
            NumericAssert.AreAlmostEqual(6.281250000000000000, method.Interpolate(4.5), 1e-15, "A 4.5");
            NumericAssert.AreAlmostEqual(6.281250000000000000, method.Differentiate(4.5, out dx, out d2x), 1e-15, "B 4.5");
            NumericAssert.AreAlmostEqual(277.50000000000000000, method.Interpolate(10.0), 1e-15, "A 10.0");
            NumericAssert.AreAlmostEqual(277.50000000000000000, method.Differentiate(10.0, out dx, out d2x), 1e-15, "B 10.0");
            NumericAssert.AreAlmostEqual(-1010.8333333333333333, method.Interpolate(-10.0), 1e-15, "A -10.0");
            NumericAssert.AreAlmostEqual(-1010.8333333333333333, method.Differentiate(-10.0, out dx, out d2x), 1e-15, "B -10.0");
        }

        [Test]
        public void TestInterpolationMethod_EquidistantBarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnEquidistantPoints(0.0, 4.0, x);
            Assert.IsInstanceOfType(typeof(EquidistantPolynomialInterpolation), method, "Type");

            for(int i = 0; i < 4; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(i), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[2,2.5],[3,1],[4,3]], x)),20);"
            NumericAssert.AreAlmostEqual(.48742500000000000000, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(1.6968000000000000000, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(3.0819250000000000000, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(.940800000000000001, method.Interpolate(3.2), 1e-15, "A 3.2");
            NumericAssert.AreAlmostEqual(7.265625000000000001, method.Interpolate(4.5), 1e-15, "A 4.5");
            NumericAssert.AreAlmostEqual(592.50000000000000000, method.Interpolate(10.0), 1e-13, "A 10.0");
            NumericAssert.AreAlmostEqual(657.50000000000000000, method.Interpolate(-10.0), 1e-12, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_Chebyshev1BarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnChebyshevFirstKindPoints(0.0, 4.0, x);
            Assert.IsInstanceOfType(typeof(ChebyshevFirstKindPolynomialInterpolation), method, "Type");

            double[] t = Interpolation.GenerateChebyshevFirstKindSamplePoints(0.0, 4.0, 5);
            for(int i = 0; i < 4; i++)
            {
                // verify the generated chebyshev1 points
                double tt = 2.0 + 2.0 * Math.Cos(Math.PI * 0.1 * (2 * i + 1)); 
                NumericAssert.AreAlmostEqual(t[i], tt, "Point " + i.ToString());
                // verify the interpolated values exactly at the sample points.
                NumericAssert.AreAlmostEqual(x[i], method.Interpolate(tt), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation(evalf([[2*cos(Pi/10)+2,0],[2*cos(3*Pi/10)+2,3],[2*cos(5*Pi/10)+2,2.5],[2*cos(7*Pi/10)+2,1],[2*cos(9*Pi/10)+2,3]]), x)),20);"
            NumericAssert.AreAlmostEqual(2.9882560375702001608, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(1.7097090371118968872, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(1.0462830804302586508, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(2.951922899377369724, method.Interpolate(3.2), 1e-15, "A 3.2");
            NumericAssert.AreAlmostEqual(-5.394317844683536750, method.Interpolate(4.5), 1e-15, "A 4.5");
            NumericAssert.AreAlmostEqual(-228.01438153088988107, method.Interpolate(10.0), 1e-13, "A 10.0");
            NumericAssert.AreAlmostEqual(1979.2646653044133954, method.Interpolate(-10.0), 1e-12, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_Chebyshev2BarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnChebyshevSecondKindPoints(0.0, 4.0, x);
            Assert.IsInstanceOfType(typeof(ChebyshevSecondKindPolynomialInterpolation), method, "Type");

            double[] t = Interpolation.GenerateChebyshevSecondKindSamplePoints(0.0, 4.0, 5);
            for(int i = 0; i < 4; i++)
            {
                // verify the generated chebyshev2 points
                double tt = 2.0 + 2.0 * Math.Cos(Math.PI * i * 0.25); 
                NumericAssert.AreAlmostEqual(t[i], tt, "Point " + i.ToString());
                // verify the interpolated values exactly at the sample points.
                NumericAssert.AreAlmostEqual(x[i], method.Interpolate(tt), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation(evalf([[2*cos(0*Pi/4)+2,0],[2*cos(1*Pi/4)+2,3],[2*cos(2*Pi/4)+2,2.5],[2*cos(3*Pi/4)+2,1],[2*cos(4*Pi/4)+2,3]]), x)),20);"
            NumericAssert.AreAlmostEqual(2.4826419375703841423, method.Interpolate(0.1), 1e-14, "A 0.1");
            NumericAssert.AreAlmostEqual(1.3814129880730972522, method.Interpolate(0.4), 1e-14, "A 0.4");
            NumericAssert.AreAlmostEqual(.8808232156067110292, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(3.478116015902536997, method.Interpolate(3.2), 1e-15, "A 3.2");
            NumericAssert.AreAlmostEqual(-5.035612822087164912, method.Interpolate(4.5), 1e-15, "A 4.5");
            NumericAssert.AreAlmostEqual(-369.20562748477140583, method.Interpolate(10.0), 1e-13, "A 10.0");
            NumericAssert.AreAlmostEqual(1199.4696961966999204, method.Interpolate(-10.0), 1e-12, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_RationalPoleFreeBarycentric()
        {
            // *************************************************************************************************
            // 1st: polynomial case (equidistant polynomial generates the same values; rational would have pole)
            // *************************************************************************************************

            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.Create(t, x);
            Assert.IsInstanceOfType(typeof(RationalPoleFreeInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "PolynomialInterpolation([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x);"
            NumericAssert.AreAlmostEqual(-4.5968, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.65395, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(0.21875, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-0.84205, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-1.10805, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-1.1248, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(0.5392, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(-4431, method.Interpolate(10.0), 1e-12, "A 10.0");
            NumericAssert.AreAlmostEqual(-5071, method.Interpolate(-10.0), 1e-12, "A -10.0");

            // *****************************************************************************
            // 2nd: x(t) = 1/(1+t^2), t=-5..5 (polynomial can' t interpolate that function!)
            // *****************************************************************************

            t = new double[40];
            x = new double[40];

            double step = 10.0 / 39.0;
            for(int i = 0; i < t.Length; i++)
            {
                double tt = -5 + i * step;
                t[i] = tt;
                x[i] = 1.0 / (1.0 + tt * tt);
            }

            RationalPoleFreeInterpolation methodTyped = (RationalPoleFreeInterpolation)method;
            methodTyped.Init(t, x); // re-initialize for another set of points/values.

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "B Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "tt := [seq(-5+(i-1)*10/39,i=1..40)]: xx := [seq(1/(1+tt[i]*tt[i]),i=1..40)]:"
            // Maple: "RationalInterpolation(tt, xx, x);"
        }

        [Test]
        public void TestInterpolationMethod_RationalWithPoles()
        {
            double[] t = new double[] { 0, 1, 3, 4, 5 };
            double[] x = new double[] { 0, 3, 1000, -1000, 3 };

            RationalInterpolation method = new RationalInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},RationalInterpolation([[0,0],[1,3],[3,1000],[4,-1000], [5,3]], x)),20);"
            NumericAssert.AreAlmostEqual(.19389203383553566255, method.Interpolate(0.1), 1e-14, "A 0.1");
            NumericAssert.AreAlmostEqual(.88132900698869875369, method.Interpolate(0.4), 1e-14, "A 0.4");
            NumericAssert.AreAlmostEqual(3.5057665681580626913, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(1548.7666642693586902, method.Interpolate(3.01), 1e-13, "A 3.01");
            NumericAssert.AreAlmostEqual(3362.2564334253633516, method.Interpolate(3.02), 1e-13, "A 3.02");
            NumericAssert.AreAlmostEqual(-22332.603641443806014, method.Interpolate(3.03), 1e-12, "A 3.03");
            NumericAssert.AreAlmostEqual(-440.30323769822443789, method.Interpolate(3.1), 1e-14, "A 3.1");
            NumericAssert.AreAlmostEqual(-202.42421196280566349, method.Interpolate(3.2), 1e-14, "A 3.2");
            NumericAssert.AreAlmostEqual(21.208249625210155439, method.Interpolate(4.5), 1e-14, "A 4.5");
            NumericAssert.AreAlmostEqual(-4.8936986959784751517, method.Interpolate(10.0), 1e-13, "A 10.0");
            NumericAssert.AreAlmostEqual(-3.6017584308603731307, method.Interpolate(-10.0), 1e-13, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_LimitedOrderPolynomial()
        {
            double[] t = new double[] { 0.0, 1.0, 3.0, 4.0 };
            double[] x = new double[] { 0.0, 3.0, 1.0, 3.0 };

            LimitedOrderPolynomialInterpolation method = new LimitedOrderPolynomialInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[3,1],[4,3]], x)),20);"
            NumericAssert.AreAlmostEqual(.57225000000000000000, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(1.8840000000000000000, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(3.0314166666666666667, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(1.034666666666666667, method.Interpolate(3.2), 1e-15, "A 3.2");
            NumericAssert.AreAlmostEqual(6.281250000000000000, method.Interpolate(4.5), 1e-15, "A 4.5");
            NumericAssert.AreAlmostEqual(277.50000000000000000, method.Interpolate(10.0), 1e-15, "A 10.0");
            NumericAssert.AreAlmostEqual(-1010.8333333333333333, method.Interpolate(-10.0), 1e-15, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_LimitedOrderRationalWithPoles()
        {
            double[] t = new double[] { 0, 1, 3,    4,     5 };
            double[] x = new double[] { 0, 3, 1000, -1000, 3 };

            LimitedOrderRationalInterpolation method = new LimitedOrderRationalInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},RationalInterpolation([[0,0],[1,3],[3,1000],[4,-1000], [5,3]], x)),20);"
            NumericAssert.AreAlmostEqual(.19389203383553566255, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(.88132900698869875369, method.Interpolate(0.4), 1e-14, "A 0.4");
            NumericAssert.AreAlmostEqual(3.5057665681580626913, method.Interpolate(1.1), 1e-15, "A 1.1");
            NumericAssert.AreAlmostEqual(1548.7666642693586902, method.Interpolate(3.01), 1e-13, "A 3.01");
            NumericAssert.AreAlmostEqual(3362.2564334253633516, method.Interpolate(3.02), 1e-13, "A 3.02");
            NumericAssert.AreAlmostEqual(-22332.603641443806014, method.Interpolate(3.03), 1e-12, "A 3.03");
            NumericAssert.AreAlmostEqual(-440.30323769822443789, method.Interpolate(3.1), 1e-14, "A 3.1");
            NumericAssert.AreAlmostEqual(-202.42421196280566349, method.Interpolate(3.2), 1e-14, "A 3.2");
            NumericAssert.AreAlmostEqual(21.208249625210155439, method.Interpolate(4.5), 1e-14, "A 4.5");
            NumericAssert.AreAlmostEqual(-4.8936986959784751517, method.Interpolate(10.0), 1e-13, "A 10.0");
            NumericAssert.AreAlmostEqual(-3.6017584308603731307, method.Interpolate(-10.0), 1e-13, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_LinearSpline()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateLinearSpline(t, x);
            Assert.IsInstanceOfType(typeof(LinearSplineInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "f := x -> piecewise(x<-1,3+x,x<0,-1-3*x,x<1,-1+x,-1+x);"
            // Maple: "f(x)"
            NumericAssert.AreAlmostEqual(.6, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.7, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(.5, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-.7, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-.9, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-.6, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(.2, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(9.0, method.Interpolate(10.0), 1e-15, "A 10.0");
            NumericAssert.AreAlmostEqual(-7.0, method.Interpolate(-10.0), 1e-15, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundaryNatural()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateNaturalCubicSpline(t, x);
            Assert.IsInstanceOfType(typeof(CubicSplineInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints='natural')),20);"
            NumericAssert.AreAlmostEqual(.144000000000000000, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.7906428571428571429, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(.47321428571428571431, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-.80992857142857142857, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-1.1089285714285714286, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-1.0285714285714285714, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(.30285714285714285716, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(189, method.Interpolate(10.0), 1e-15, "A 10.0");
            NumericAssert.AreAlmostEqual(677, method.Interpolate(-10.0), 1e-15, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundaryFirstDerivativeFixed()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateCubicSpline(t, x, SplineBoundaryCondition.FirstDerivative, 1.0, SplineBoundaryCondition.FirstDerivative, -1.0);
            Assert.IsInstanceOfType(typeof(CubicSplineInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints=[1,-1])),20);"
            NumericAssert.AreAlmostEqual(1.120000000000000001, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.8243928571428571428, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(.54910714285714285715, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-.78903571428571428572, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-1.1304642857142857143, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-1.1040000000000000000, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(.4148571428571428571, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(-608.14285714285714286, method.Interpolate(10.0), 1e-15, "A 10.0");
            NumericAssert.AreAlmostEqual(1330.1428571428571429, method.Interpolate(-10.0), 1e-15, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundarySecondDerivativeFixed()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateCubicSpline(t, x, SplineBoundaryCondition.SecondDerivative, -5.0, SplineBoundaryCondition.SecondDerivative, -1.0);
            Assert.IsInstanceOfType(typeof(CubicSplineInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints=Matrix(2,13,{(1,3)=1,(1,13)=-5,(2,10)=1,(2,13)=-1}))),20);"
            NumericAssert.AreAlmostEqual(-.8999999999999999993, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.7590357142857142857, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(.41517857142857142854, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-.82010714285714285714, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-1.1026071428571428572, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-1.0211428571428571429, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(.31771428571428571421, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(39, method.Interpolate(10.0), 1e-14, "A 10.0");
            NumericAssert.AreAlmostEqual(-37, method.Interpolate(-10.0), 1e-14, "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_AkimaSpline()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateAkimaCubicSpline(t, x);
            Assert.IsInstanceOfType(typeof(AkimaSplineInterpolation), method, "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.AreEqual(x[i], method.Interpolate(t[i]), "A Exact Point " + i.ToString());
            }

            // TODO: Verify the expected values (that they are really the expected ones)
            NumericAssert.AreAlmostEqual(-0.52, method.Interpolate(-2.4), 1e-15, "A -2.4");
            NumericAssert.AreAlmostEqual(1.826, method.Interpolate(-0.9), 1e-15, "A -0.9");
            NumericAssert.AreAlmostEqual(0.25, method.Interpolate(-0.5), 1e-15, "A -0.5");
            NumericAssert.AreAlmostEqual(-1.006, method.Interpolate(-0.1), 1e-15, "A -0.1");
            NumericAssert.AreAlmostEqual(-0.9, method.Interpolate(0.1), 1e-15, "A 0.1");
            NumericAssert.AreAlmostEqual(-0.6, method.Interpolate(0.4), 1e-15, "A 0.4");
            NumericAssert.AreAlmostEqual(0.2, method.Interpolate(1.2), 1e-15, "A 1.2");
            NumericAssert.AreAlmostEqual(9, method.Interpolate(10.0), 1e-14, "A 10.0");
            NumericAssert.AreAlmostEqual(-151, method.Interpolate(-10.0), 1e-14, "A -10.0");
        }
    }
}
