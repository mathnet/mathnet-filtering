#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2007, Christoph Rüegg,  http://christoph.ruegg.name
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
using System.Globalization;

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.Transformations;

namespace Iridium.Test
{
    [TestFixture]
    public class FftTest
    {
        private ComplexFourierTransformation cft;
        private RealFourierTransformation rft;

        #region Compare Helpers
        public static void RealTestTimeEven(double[] samples)
        {
            int len = samples.Length;
            for(int i = 1; i < samples.Length; i++)
                Assert.AreEqual(samples[i], samples[len - i], 0.00000001, "Real Even in Time Space");
        }

        public static void RealTestTimeOdd(double[] samples)
        {
            int len = samples.Length;
            for(int i = 1; i < samples.Length; i++)
                Assert.AreEqual(samples[i], -samples[len - i], 0.00000001, "Real Odd in Time Space");
            Assert.AreEqual(0.0, samples[0], 0.00000001, "Real Odd in Time Space: Periodic Continuation");
        }

        public static void ComplexTestTimeEven(double[] samples)
        {
            int len = samples.Length;
            for(int i = 2; i < samples.Length / 2; i += 2)
            {
                Assert.AreEqual(samples[i], samples[len - i], 0.00000001, "Complex Even in Time Space: Real Part");
                Assert.AreEqual(samples[i + 1], samples[len + 1 - i], 0.00000001, "Complex Even in Time Space: Imaginary Part");
            }
        }

        public static void ComplexTestTimeOdd(double[] samples)
        {
            int len = samples.Length;
            for(int i = 2; i < samples.Length / 2; i += 2)
            {
                Assert.AreEqual(samples[i], -samples[len - i], 0.00000001, "Complex Odd in Time Space: Real Part");
                Assert.AreEqual(samples[i + 1], -samples[len + 1 - i], 0.00000001, "Complex Odd in Time Space: Imaginary Part");
            }
            Assert.AreEqual(0.0, samples[0], 0.00000001, "Complex Odd in Time Space: Real Part: Periodic Continuation");
            Assert.AreEqual(0.0, samples[1], 0.00000001, "Complex Odd in Time Space: Imaginary Part: Periodic Continuation");
        }

        public static void ComplexTestFreqEven(double[] samples)
        {
            int len = samples.Length;
            for(int i = 0; i < samples.Length / 2; i += 2)
            {
                Assert.AreEqual(samples[i + 2], samples[len - 2 - i], 0.00000001, "Complex Even in Frequency Space: Real Part");
                Assert.AreEqual(samples[i + 3], samples[len - 1 - i], 0.00000001, "Complex Even in Frequency Space: Imaginary Part");
            }
        }

        public static void ComplexTestFreqOdd(double[] samples)
        {
            int len = samples.Length;
            for(int i = 0; i < samples.Length / 2; i += 2)
            {
                Assert.AreEqual(samples[i + 2], -samples[len - 2 - i], 0.00000001, "Complex Odd in Frequency Space: Real Part");
                Assert.AreEqual(samples[i + 3], -samples[len - 1 - i], 0.00000001, "Complex Odd in Frequency Space: Imaginary Part");
            }
            Assert.AreEqual(0.0, samples[0], 0.00000001, "Complex Odd in Frequency Space: Real Part: Periodic Continuation (No DC)");
            Assert.AreEqual(0.0, samples[1], 0.00000001, "Complex Odd in Frequency Space: Imaginary Part: Periodic Continuation (No DC)");
        }

        public static void ComplexTestRealZero(double[] samples)
        {
            for(int i = 0; i < samples.Length; i += 2)
                Assert.AreEqual(0, samples[i], 0.00000001, "Complex: Zero Real Part"); ;
        }

        public static void ComplexTestImagZero(double[] samples)
        {
            for(int i = 1; i < samples.Length; i += 2)
                Assert.AreEqual(0, samples[i], 0.00000001, "Complex: Zero Imaginary Part");
        }
        #endregion

        [TestFixtureSetUp]
        public void SetUp()
        {
            cft = new ComplexFourierTransformation();
            rft = new RealFourierTransformation();
        }

        #region Complex FFT
        [Test]
        public void Complex_Symmetry_RealEven_RealEven()
        {
            // h(t) real-valued even <=> H(f) real-valued even-with-offset

            int numSamples = 32;
            int length = 2 * numSamples;

            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = 1.0 / (z * z + 1.0);
                data[i + 1] = 0.0;
            }

            ComplexTestTimeEven(data);

            cft.Convention = TransformationConvention.Matlab; // so we can check MATLAB consistency
            cft.TransformForward(data);

            ComplexTestImagZero(data);
            ComplexTestFreqEven(data);

            /* Compare With MATLAB:
            samples_t = 1.0 ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(25.128, data[0 * 2], 0.001, "MATLAB 1");
            Assert.AreEqual(-3.623, data[1 * 2], 0.001, "MATLAB 2");
            Assert.AreEqual(-0.31055, data[2 * 2], 0.00001, "MATLAB 3");

            Assert.AreEqual(-0.050611, data[6 * 2], 0.000001, "MATLAB 7");
            Assert.AreEqual(-0.03882, data[7 * 2], 0.00001, "MATLAB 8");
            Assert.AreEqual(-0.031248, data[8 * 2], 0.000001, "MATLAB 9");

            Assert.AreEqual(-0.017063, data[13 * 2], 0.000001, "MATLAB 14");
            Assert.AreEqual(-0.016243, data[14 * 2], 0.000001, "MATLAB 15");
            Assert.AreEqual(-0.015777, data[15 * 2], 0.000001, "MATLAB 16");
        }

        [Test]
        public void Complex_Symmetry_ImaginaryEven_ImaginaryEven()
        {
            // h(t) imaginary-valued even <=> H(f) imaginary-valued even-with-offset

            int numSamples = 32;
            int length = 2 * numSamples;

            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = 0.0;
                data[i + 1] = 1.0 / (z * z + 1.0);
            }

            ComplexTestTimeEven(data);

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data);

            ComplexTestRealZero(data);
            ComplexTestFreqEven(data);

            /* Compare With MATLAB:
            samples_t = 1.0i ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(25.128, data[0 * 2 + 1], 0.001, "MATLAB 1");
            Assert.AreEqual(-3.623, data[1 * 2 + 1], 0.001, "MATLAB 2");
            Assert.AreEqual(-0.31055, data[2 * 2 + 1], 0.00001, "MATLAB 3");

            Assert.AreEqual(-0.050611, data[6 * 2 + 1], 0.000001, "MATLAB 7");
            Assert.AreEqual(-0.03882, data[7 * 2 + 1], 0.00001, "MATLAB 8");
            Assert.AreEqual(-0.031248, data[8 * 2 + 1], 0.000001, "MATLAB 9");

            Assert.AreEqual(-0.017063, data[13 * 2 + 1], 0.000001, "MATLAB 14");
            Assert.AreEqual(-0.016243, data[14 * 2 + 1], 0.000001, "MATLAB 15");
            Assert.AreEqual(-0.015777, data[15 * 2 + 1], 0.000001, "MATLAB 16");
        }

        [Test]
        public void Complex_Symmetry_RealOdd_ImaginaryEven()
        {
            // h(t) real-valued odd <=> H(f) imaginary-valued odd-with-offset

            int numSamples = 32;
            int length = 2 * numSamples;

            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = z / (z * z + 1.0);
                data[i + 1] = 0.0;
            }
            data[0] = 0.0; // peridoic continuation; force odd

            ComplexTestTimeOdd(data);

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data);

            ComplexTestRealZero(data);
            ComplexTestFreqOdd(data);

            /* Compare With MATLAB:
            samples_t = ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = 0
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(0, data[0 * 2 + 1], 0.001, "MATLAB 1");
            Assert.AreEqual(7.4953, data[1 * 2 + 1], 0.0001, "MATLAB 2");
            Assert.AreEqual(2.4733, data[2 * 2 + 1], 0.0001, "MATLAB 3");

            Assert.AreEqual(0.75063, data[6 * 2 + 1], 0.00001, "MATLAB 7");
            Assert.AreEqual(0.61071, data[7 * 2 + 1], 0.00001, "MATLAB 8");
            Assert.AreEqual(0.50097, data[8 * 2 + 1], 0.00001, "MATLAB 9");

            Assert.AreEqual(0.15183, data[13 * 2 + 1], 0.00001, "MATLAB 14");
            Assert.AreEqual(0.099557, data[14 * 2 + 1], 0.000001, "MATLAB 15");
            Assert.AreEqual(0.049294, data[15 * 2 + 1], 0.000001, "MATLAB 16");
        }

        [Test]
        public void Complex_Symmetry_ImaginaryOdd_RealEven()
        {
            // h(t) imaginary-valued odd <=> H(f) real-valued odd-with-offset

            int numSamples = 32;
            int length = 2 * numSamples;

            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = 0.0;
                data[i + 1] = z / (z * z + 1.0);
            }
            data[1] = 0.0; // peridoic continuation; force odd

            ComplexTestTimeOdd(data);

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data);

            ComplexTestImagZero(data);
            ComplexTestFreqOdd(data);

            /* Compare With MATLAB:
            samples_t = 1.0i * ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = 0
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(0, data[0 * 2], 0.001, "MATLAB 1");
            Assert.AreEqual(-7.4953, data[1 * 2], 0.0001, "MATLAB 2");
            Assert.AreEqual(-2.4733, data[2 * 2], 0.0001, "MATLAB 3");

            Assert.AreEqual(-0.75063, data[6 * 2], 0.00001, "MATLAB 7");
            Assert.AreEqual(-0.61071, data[7 * 2], 0.00001, "MATLAB 8");
            Assert.AreEqual(-0.50097, data[8 * 2], 0.00001, "MATLAB 9");

            Assert.AreEqual(-0.15183, data[13 * 2], 0.00001, "MATLAB 14");
            Assert.AreEqual(-0.099557, data[14 * 2], 0.000001, "MATLAB 15");
            Assert.AreEqual(-0.049294, data[15 * 2], 0.000001, "MATLAB 16");
        }

        [Test]
        public void Complex_Inverse_Mix()
        {
            int numSamples = 32;
            int length = 2 * numSamples;

            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = 1.0 / (z * z + 1.0);
                data[i + 1] = z / (z * z + 1.0);
            }
            data[1] = 0.0; // peridoic continuation; force odd

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data);

            ComplexTestImagZero(data);

            /* Compare With MATLAB:
            samples_t = 1.0i * ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
                                         + 1.0 ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = real(samples_t(1))
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(25.128, data[0 * 2], 0.001, "MATLAB 1");
            Assert.AreEqual(-11.118, data[1 * 2], 0.001, "MATLAB 2");
            Assert.AreEqual(-2.7838, data[2 * 2], 0.0001, "MATLAB 3");

            Assert.AreEqual(-0.80124, data[6 * 2], 0.00001, "MATLAB 7");
            Assert.AreEqual(-0.64953, data[7 * 2], 0.00001, "MATLAB 8");
            Assert.AreEqual(-0.53221, data[8 * 2], 0.00001, "MATLAB 9");

            Assert.AreEqual(-0.1689, data[13 * 2], 0.0001, "MATLAB 14");
            Assert.AreEqual(-0.1158, data[14 * 2], 0.0001, "MATLAB 15");
            Assert.AreEqual(-0.065071, data[15 * 2], 0.000001, "MATLAB 16");

            Assert.AreEqual(0.18904, data[20 * 2], 0.00001, "MATLAB 21");
            Assert.AreEqual(0.2475, data[21 * 2], 0.0001, "MATLAB 22");
            Assert.AreEqual(0.31196, data[22 * 2], 0.00001, "MATLAB 23");

            Assert.AreEqual(1.4812, data[29 * 2], 0.0001, "MATLAB 30");
            Assert.AreEqual(2.1627, data[30 * 2], 0.0001, "MATLAB 31");
            Assert.AreEqual(3.8723, data[31 * 2], 0.0001, "MATLAB 32");

            cft.TransformBackward(data);

            // Compare with original samples
            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                Assert.AreEqual(1.0 / (z * z + 1.0), data[i], 0.00001, "Inv: Real: " + i.ToString());
                Assert.AreEqual(i == 0 ? 0.0 : z / (z * z + 1.0), data[i+1], 0.00001, "Inv: Imag: " + i.ToString());
            }
        }
        #endregion

        #region Real FFT
        [Test]
        public void Real_TwoReal_EvenOdd()
        {
            int numSamples = 32;
            int half = numSamples >> 1;

            double[] dataEven = new double[numSamples];
            double[] dataOdd = new double[numSamples];

            for(int i = 0; i < numSamples; i++)
            {
                double z = (double)(i - half) / half;
                dataEven[i] = 1.0 / (z * z + 1.0);
                dataOdd[i] = z / (z * z + 1.0);
            }
            dataOdd[0] = 0.0; // peridoic continuation; force odd

            RealTestTimeEven(dataEven);
            RealTestTimeOdd(dataOdd);

            double[] evenReal, evenImag, oddReal, oddImag;

            rft.Convention = TransformationConvention.Matlab;
            rft.TransformForward(dataEven, dataOdd,
                out evenReal, out evenImag, out oddReal, out oddImag);

            /* Compare EVEN With MATLAB:
            samples_t = 1.0 ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(25.128, evenReal[0], 0.001, "MATLAB 1 (even)");
            Assert.AreEqual(-3.623, evenReal[1], 0.001, "MATLAB 2 (even)");
            Assert.AreEqual(-0.31055, evenReal[2], 0.00001, "MATLAB 3 (even)");

            Assert.AreEqual(-0.050611, evenReal[6], 0.000001, "MATLAB 7 (even)");
            Assert.AreEqual(-0.03882, evenReal[7], 0.00001, "MATLAB 8 (even)");
            Assert.AreEqual(-0.031248, evenReal[8], 0.000001, "MATLAB 9 (even)");

            Assert.AreEqual(-0.017063, evenReal[13], 0.000001, "MATLAB 14 (even)");
            Assert.AreEqual(-0.016243, evenReal[14], 0.000001, "MATLAB 15 (even)");
            Assert.AreEqual(-0.015777, evenReal[15], 0.000001, "MATLAB 16 (even)");

            Assert.AreEqual(0, evenImag[1], 0.001, "MATLAB 2i (even)");
            Assert.AreEqual(0, evenImag[7], 0.001, "MATLAB 8i (even)");
            Assert.AreEqual(0, evenImag[14], 0.001, "MATLAB 15i (even)");

            /* Compare ODD With MATLAB:
            samples_t = ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = 0
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(0, oddImag[0], 0.001, "MATLAB 1 (odd)");
            Assert.AreEqual(7.4953, oddImag[1], 0.0001, "MATLAB 2 (odd)");
            Assert.AreEqual(2.4733, oddImag[2], 0.0001, "MATLAB 3 (odd)");

            Assert.AreEqual(0.75063, oddImag[6], 0.00001, "MATLAB 7 (odd)");
            Assert.AreEqual(0.61071, oddImag[7], 0.00001, "MATLAB 8 (odd)");
            Assert.AreEqual(0.50097, oddImag[8], 0.00001, "MATLAB 9 (odd)");

            Assert.AreEqual(0.15183, oddImag[13], 0.00001, "MATLAB 14 (odd)");
            Assert.AreEqual(0.099557, oddImag[14], 0.000001, "MATLAB 15 (odd)");
            Assert.AreEqual(0.049294, oddImag[15], 0.000001, "MATLAB 16 (odd)");

            Assert.AreEqual(0, oddReal[1], 0.001, "MATLAB 2r (odd)");
            Assert.AreEqual(0, oddReal[7], 0.001, "MATLAB 8r (odd)");
            Assert.AreEqual(0, oddReal[14], 0.001, "MATLAB 15r (odd)");
        }

        [Test]
        public void Real_TwoReal_Inverse()
        {
            int numSamples = 32;
            int half = numSamples >> 1;

            double[] dataEven = new double[numSamples];
            double[] dataOdd = new double[numSamples];

            for(int i = 0; i < numSamples; i++)
            {
                double z = (double)(i - half) / half;
                dataEven[i] = 1.0 / (z * z + 1.0);
                dataOdd[i] = z / (z * z + 1.0);
            }
            dataOdd[0] = 0.0; // peridoic continuation; force odd

            RealTestTimeEven(dataEven);
            RealTestTimeOdd(dataOdd);

            double[] evenReal, evenImag, oddReal, oddImag;

            // Forward Transform
            rft.Convention = TransformationConvention.Default;
            rft.TransformForward(dataEven, dataOdd,
                out evenReal, out evenImag, out oddReal, out oddImag);

            // Backward Transform
            double[] dataEven2, dataOdd2;
            rft.TransformBackward(evenReal, evenImag, oddReal, oddImag,
                out dataEven2, out dataOdd2);

            // Compare with original samples
            for(int i = 0; i < numSamples; i += 2)
            {
                Assert.AreEqual(dataEven[i], dataEven2[i], 0.00001, "Inv: Even: " + i.ToString());
                Assert.AreEqual(dataOdd[i], dataOdd2[i], 0.00001, "Inv: Odd: " + i.ToString());
            }
        }

        [Test]
        public void Real_SingleReal_EvenOdd()
        {
            int numSamples = 32;
            int half = numSamples >> 1;

            double[] dataEven = new double[numSamples];
            double[] dataOdd = new double[numSamples];

            for(int i = 0; i < numSamples; i++)
            {
                double z = (double)(i - half) / half;
                dataEven[i] = 1.0 / (z * z + 1.0);
                dataOdd[i] = z / (z * z + 1.0);
            }
            dataOdd[0] = 0.0; // peridoic continuation; force odd

            RealTestTimeEven(dataEven);
            RealTestTimeOdd(dataOdd);

            double[] evenReal, evenImag, oddReal, oddImag;

            rft.Convention = TransformationConvention.Matlab;
            rft.TransformForward(dataEven, out evenReal, out evenImag);
            rft.TransformForward(dataOdd, out oddReal, out oddImag);

            /* Compare EVEN With MATLAB:
            samples_t = 1.0 ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(25.128, evenReal[0], 0.001, "MATLAB 1 (even)");
            Assert.AreEqual(-3.623, evenReal[1], 0.001, "MATLAB 2 (even)");
            Assert.AreEqual(-0.31055, evenReal[2], 0.00001, "MATLAB 3 (even)");

            Assert.AreEqual(-0.050611, evenReal[6], 0.000001, "MATLAB 7 (even)");
            Assert.AreEqual(-0.03882, evenReal[7], 0.00001, "MATLAB 8 (even)");
            Assert.AreEqual(-0.031248, evenReal[8], 0.000001, "MATLAB 9 (even)");

            Assert.AreEqual(-0.017063, evenReal[13], 0.000001, "MATLAB 14 (even)");
            Assert.AreEqual(-0.016243, evenReal[14], 0.000001, "MATLAB 15 (even)");
            Assert.AreEqual(-0.015777, evenReal[15], 0.000001, "MATLAB 16 (even)");

            Assert.AreEqual(0, evenImag[1], 0.001, "MATLAB 2i (even)");
            Assert.AreEqual(0, evenImag[7], 0.001, "MATLAB 8i (even)");
            Assert.AreEqual(0, evenImag[14], 0.001, "MATLAB 15i (even)");

            /* Compare ODD With MATLAB:
            samples_t = ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = 0
            samples_f = fft(samples_t)
            */

            Assert.AreEqual(0, oddImag[0], 0.001, "MATLAB 1 (odd)");
            Assert.AreEqual(7.4953, oddImag[1], 0.0001, "MATLAB 2 (odd)");
            Assert.AreEqual(2.4733, oddImag[2], 0.0001, "MATLAB 3 (odd)");

            Assert.AreEqual(0.75063, oddImag[6], 0.00001, "MATLAB 7 (odd)");
            Assert.AreEqual(0.61071, oddImag[7], 0.00001, "MATLAB 8 (odd)");
            Assert.AreEqual(0.50097, oddImag[8], 0.00001, "MATLAB 9 (odd)");

            Assert.AreEqual(0.15183, oddImag[13], 0.00001, "MATLAB 14 (odd)");
            Assert.AreEqual(0.099557, oddImag[14], 0.000001, "MATLAB 15 (odd)");
            Assert.AreEqual(0.049294, oddImag[15], 0.000001, "MATLAB 16 (odd)");

            Assert.AreEqual(0, oddReal[1], 0.001, "MATLAB 2r (odd)");
            Assert.AreEqual(0, oddReal[7], 0.001, "MATLAB 8r (odd)");
            Assert.AreEqual(0, oddReal[14], 0.001, "MATLAB 15r (odd)");
        }

        [Test]
        public void Real_SingleReal_Inverse()
        {
            int numSamples = 32;
            int half = numSamples >> 1;

            double[] dataEven = new double[numSamples];
            double[] dataOdd = new double[numSamples];

            for(int i = 0; i < numSamples; i++)
            {
                double z = (double)(i - half) / half;
                dataEven[i] = 1.0 / (z * z + 1.0);
                dataOdd[i] = z / (z * z + 1.0);
            }
            dataOdd[0] = 0.0; // peridoic continuation; force odd

            RealTestTimeEven(dataEven);
            RealTestTimeOdd(dataOdd);

            double[] evenReal, evenImag, oddReal, oddImag;

            // Forward Transform
            rft.Convention = TransformationConvention.Default;
            rft.TransformForward(dataEven, out evenReal, out evenImag);
            rft.Convention = TransformationConvention.NumericalRecipes; // to also check this one once...
            rft.TransformForward(dataOdd, out oddReal, out oddImag);

            // Backward Transform
            double[] dataEven2, dataOdd2;
            rft.Convention = TransformationConvention.Default;
            rft.TransformBackward(evenReal, evenImag, out dataEven2);
            rft.Convention = TransformationConvention.NumericalRecipes;
            rft.TransformBackward(oddReal, oddImag, out dataOdd2);

            // Compare with original samples
            for(int i = 0; i < numSamples; i += 2)
            {
                Assert.AreEqual(dataEven[i], dataEven2[i], 0.00001, "Inv: Even: " + i.ToString());

                // Note: Numerical Recipes applies no scaling, 
                // so we have to compensate this here by scaling with N/2
                Assert.AreEqual(dataOdd[i] * half, dataOdd2[i], 0.00001, "Inv: Odd: " + i.ToString());
            }
        }
        #endregion

        #region Complex Multi Dimensional FFT

        [Test]
        public void Complex_MultiDim_1D_Inverse_Mix()
        {
            int numSamples = 32;
            int length = 2 * numSamples;

            int[] dims = new int[] { numSamples };
            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                data[i] = 1.0 / (z * z + 1.0);
                data[i + 1] = z / (z * z + 1.0);
            }
            data[1] = 0.0; // peridoic continuation; force odd

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data, dims);

            ComplexTestImagZero(data);

            /* Compare With MATLAB:
            samples_t = 1.0i * ([-16:1:15] ./ 16) ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
                                         + 1.0 ./ (([-16:1:15] ./ 16) .^ 2 + 1.0)
            samples_t(1) = real(samples_t(1))
            samples_f = fftn(samples_t)
            */

            Assert.AreEqual(25.128, data[0 * 2], 0.001, "MATLAB 1");
            Assert.AreEqual(-11.118, data[1 * 2], 0.001, "MATLAB 2");
            Assert.AreEqual(-2.7838, data[2 * 2], 0.0001, "MATLAB 3");

            Assert.AreEqual(-0.80124, data[6 * 2], 0.00001, "MATLAB 7");
            Assert.AreEqual(-0.64953, data[7 * 2], 0.00001, "MATLAB 8");
            Assert.AreEqual(-0.53221, data[8 * 2], 0.00001, "MATLAB 9");

            Assert.AreEqual(-0.1689, data[13 * 2], 0.0001, "MATLAB 14");
            Assert.AreEqual(-0.1158, data[14 * 2], 0.0001, "MATLAB 15");
            Assert.AreEqual(-0.065071, data[15 * 2], 0.000001, "MATLAB 16");

            Assert.AreEqual(0.18904, data[20 * 2], 0.00001, "MATLAB 21");
            Assert.AreEqual(0.2475, data[21 * 2], 0.0001, "MATLAB 22");
            Assert.AreEqual(0.31196, data[22 * 2], 0.00001, "MATLAB 23");

            Assert.AreEqual(1.4812, data[29 * 2], 0.0001, "MATLAB 30");
            Assert.AreEqual(2.1627, data[30 * 2], 0.0001, "MATLAB 31");
            Assert.AreEqual(3.8723, data[31 * 2], 0.0001, "MATLAB 32");

            cft.TransformBackward(data, dims);

            // Compare with original samples
            for(int i = 0; i < length; i += 2)
            {
                double z = (double)(i - numSamples) / numSamples;
                Assert.AreEqual(1.0 / (z * z + 1.0), data[i], 0.00001, "Inv: Real: " + i.ToString());
                Assert.AreEqual(i == 0 ? 0.0 : z / (z * z + 1.0), data[i + 1], 0.00001, "Inv: Imag: " + i.ToString());
            }
        }

        [Test]
        public void Complex_MultiDim_2D_Inverse_Mix()
        {
            int numSamples = 4;
            int length = 2 * numSamples;

            int[] dims = new int[] { 2, 2 };
            double[] data = new double[length];

            for(int i = 0; i < length; i += 2)
            {
                data[i] = i;
                data[i + 1] = numSamples - i;
            }
            data[1] = 0.0; // peridoic continuation; force odd

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data, dims);

            /* Compare With MATLAB:
            samples_t = [0, 2+2i;4,6-2i]
            samples_f = fftn(samples_t)
            */

            Assert.AreEqual(12, data[0 * 2], 0.000001, "MATLAB 1");
            Assert.AreEqual(0, data[0 * 2 + 1], 0.000001, "MATLAB 1b");
            Assert.AreEqual(-4, data[1 * 2], 0.000001, "MATLAB 2");
            Assert.AreEqual(0, data[1 * 2 + 1], 0.000001, "MATLAB 2b");
            Assert.AreEqual(-8, data[2 * 2], 0.000001, "MATLAB 3");
            Assert.AreEqual(4, data[2 * 2 + 1], 0.000001, "MATLAB 3b");
            Assert.AreEqual(0, data[3 * 2], 0.000001, "MATLAB 4");
            Assert.AreEqual(-4, data[3 * 2 + 1], 0.000001, "MATLAB 4b");

            cft.TransformBackward(data, dims);

            // Compare with original samples
            for(int i = 0; i < length; i += 2)
            {
                Assert.AreEqual(i, data[i], 0.000001, "Inv: Real: " + i.ToString());
                Assert.AreEqual(i == 0 ? 0.0 : numSamples - i, data[i + 1], 0.000001, "Inv: Imag: " + i.ToString());
            }
        }

        [Test]
        public void Complex_MultiDim_3D_Inverse_Mix()
        {
            int[] dims = new int[] { 2, 4, 8 };
            int ntot = 2 * 4 * 8;
            int len = 2 * ntot;

            double[] data = new double[len];
            for(int i = 0; i < len; i += 2)
            {
                data[i] = (double)i;
                data[i + 1] = 0.0;
            }

            cft.Convention = TransformationConvention.Matlab;
            cft.TransformForward(data, dims);

            /* Compare With MATLAB:
            M = zeros(2,4,8);
            for x = 0:1
                for y = 0:3
                    for z = 0:7
                        M(x+1,y+1,z+1) = 2*(4*8*x+8*y+z);
                    end
                end
            end
            H = fftn(M);

            M1 = reshape(M(1,:,:),[4,8])
            M2 = reshape(M(2,:,:),[4,8])
            H1 = reshape(H(1,:,:),[4,8])
            H2 = reshape(H(2,:,:),[4,8])
            */

            Assert.AreEqual(4032, data[0 * 2], 1, "MATLAB 1");
            Assert.AreEqual(0, data[0 * 2 + 1], 1, "MATLAB 2b");

            Assert.AreEqual(-64, data[1 * 2], 1, "MATLAB 2");
            Assert.AreEqual(154.51, data[1 * 2 + 1], 1, "MATLAB 2b");
            Assert.AreEqual(-64, data[2 * 2], 1, "MATLAB 3");
            Assert.AreEqual(64, data[2 * 2 + 1], 1, "MATLAB 3b");
            Assert.AreEqual(-64, data[6 * 2], 1, "MATLAB 7");
            Assert.AreEqual(-64, data[6 * 2 + 1], 1, "MATLAB 7b");
            Assert.AreEqual(-64, data[7 * 2], 1, "MATLAB 8");
            Assert.AreEqual(-154.51, data[7 * 2 + 1], 1, "MATLAB 8b");

            Assert.AreEqual(-512, data[8 * 2], 1, "MATLAB 9");
            Assert.AreEqual(512, data[8 * 2 + 1], 1, "MATLAB 9b");

            Assert.AreEqual(0, data[9 * 2], 1, "MATLAB 10");
            Assert.AreEqual(0, data[9 * 2 + 1], 1, "MATLAB 10b");
            Assert.AreEqual(0, data[15 * 2], 1, "MATLAB 16");
            Assert.AreEqual(0, data[15 * 2 + 1], 1, "MATLAB 16b");

            Assert.AreEqual(-512, data[16 * 2], 1, "MATLAB 17");
            Assert.AreEqual(0, data[16 * 2 + 1], 1, "MATLAB 17b");

            Assert.AreEqual(-512, data[24 * 2], 1, "MATLAB 25");
            Assert.AreEqual(-512, data[24 * 2 + 1], 1, "MATLAB 25b");

            Assert.AreEqual(-2048, data[32 * 2], 1, "MATLAB 33");
            Assert.AreEqual(0, data[32 * 2 + 1], 1, "MATLAB 33b");

            Assert.AreEqual(0, data[33 * 2], 1, "MATLAB 34");
            Assert.AreEqual(0, data[33 * 2 + 1], 1, "MATLAB 34b");
            Assert.AreEqual(0, data[39 * 2], 1, "MATLAB 40");
            Assert.AreEqual(0, data[39 * 2 + 1], 1, "MATLAB 40b");

            Assert.AreEqual(0, data[56 * 2], 1, "MATLAB 57");
            Assert.AreEqual(0, data[56 * 2 + 1], 1, "MATLAB 57b");

            cft.TransformBackward(data, dims);

            // Compare with original samples
            for(int i = 0; i < len; i += 2)
            {
                Assert.AreEqual((double)i, data[i], 0.00001, "Inv: Real: " + i.ToString());
                Assert.AreEqual(0d, data[i + 1], 0.00001, "Inv: Imag: " + i.ToString());
            }
        }
        #endregion

    }
}
