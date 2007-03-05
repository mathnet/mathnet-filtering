#region Math.NET Iridium (LGPL) by Ruegg + Houston
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2007, Christoph Rüegg,  http://christoph.ruegg.name
// Based on Exocortex.DSP, Ben Houston, http://www.exocortex.org
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

namespace MathNet.Numerics.Transformations
{
    /// <summary>
    /// Internal FFT Implementation and Helper Function. This class caches precomputations locally, thus consider reusing it.
    /// </summary>
    internal class InternalFFT
    {
        private const int maxLength = 4096;
        private const int minLength = 1;
        private const int maxBits = 12;
        private const int minBits = 0;

        public void DiscreteFourierTransform(double[] samples, bool forward, TransformationConvention convention)
        {
            ReorderSamples(samples);
            DanielsonLanczosTransform(samples, forward, convention);
            Rescale(samples, forward, convention);
        }

        public void DiscreteFourierTransformMultiDim(double[] samples, int[] dimensions, bool forward, TransformationConvention convention)
        {
            int rank = dimensions.Length;
            int n, nprev = 1, step, stride;

            for(int idim = rank - 1; idim >= 0; idim--)
            {
                n = dimensions[idim];
                step = nprev << 1; // complex numbers as pairs
                stride = n * step;

                ReorderSamplesMultiDim(samples, stride, step);
                DanielsonLanczosTransformMultiDim(samples, stride, step, forward, convention);

                nprev *= n;
            }

            Rescale(samples, forward, convention);
        }

        #region Sample Reordering (Step 1)
        /// <param name="samples">Complex samples (even = real, odd = imaginary). Length must be a power of two.</param>
        public void ReorderSamples(double[] samples)
        {
            int numSamplePairs = samples.Length >> 1;
            int[] reversedBits = ReverseBits(Fn.IntLog2(numSamplePairs));
            for(int i = 0; i < numSamplePairs; i++)
            {
                int swap = reversedBits[i];
                if(swap > i)
                {
                    int a = 2 * i, b = 2 * swap;

                    double tmp = samples[a];
                    samples[a] = samples[b];
                    samples[b] = tmp;

                    tmp = samples[a + 1];
                    samples[a + 1] = samples[b + 1];
                    samples[b + 1] = tmp;
                }

            }
        }

        /// <param name="samples">Complex samples (even = real, odd = imaginary). Length must be a power of two in every dimension.</param>
        /// <param name="stride">Current dimension lengths * steps (steps: next method parameter).</param>
        /// <param name="step">2 * Product of all previous dimension lengths. (times 2 because of the complex sample pairs)</param>
        public void ReorderSamplesMultiDim(double[] samples, int stride, int step)
        {
            int numSamplePairs = stride / step;
            int[] reversedBits = ReverseBits(Fn.IntLog2(numSamplePairs));
            for(int i2 = 0, i=0; i2 < stride; i2 += step, i++)
            {
                int swap = reversedBits[i] * step;
                if(swap > i2)
                {
                    for(int i1 = i2; i1 < i2 + step; i1 += 2)
                        for(int i3 = i1; i3 < samples.Length; i3 += stride)
                        {
                            int swap3 = swap + i3 - i2;

                            double tmp = samples[swap3];
                            samples[swap3] = samples[i3];
                            samples[i3] = tmp;

                            tmp = samples[swap3 + 1];
                            samples[swap3 + 1] = samples[i3 + 1];
                            samples[i3 + 1] = tmp;
                        }
                }
            }
        }
        #endregion

        #region Danielson Lanczos Transform (Step 2)
        /// <param name="samples">Reordered complex samples (even = real, odd = imaginary). Length must be a power of two.</param>
        /// <param name="forward">true for forward transformation, false for (unscaled) backward/inverse transform.</param>
        /// <remarks>The returnd results in backward/inverse mode are not scaled yet; scale them using <see cref="RescaleAfterInverseTransformation"/> afterwards.</remarks>
        public void DanielsonLanczosTransform(double[] samples, bool forward, TransformationConvention convention)
        {
            int levels = Fn.IntLog2(samples.Length >> 1);

            // precompute coefficients if they're not already there.
            BuildCoefficientsForLevels(levels);
            double expSignConvention = (convention & TransformationConvention.InverseExponent) > 0 ? -1d : 1d;

            int N = 2;
            for(int level = 1; level <= levels; level++)
            {
                int M = N;
                N <<= 1;

                double[] realCosine = RealCosineCoefficients(level, forward);
                double[] imagSine = ImaginarySineCoefficients(level, forward);

                for(int j = 0, jj=0; jj < M; j++, jj+=2)
                {
                    double uR = realCosine[j];
                    double uI = expSignConvention * imagSine[j];

                    for(int even = jj; even < samples.Length; even += N)
                    {
                        int odd = even + M;

                        double re = samples[odd];
                        double im = samples[odd+1];

                        double tmpr = re * uR - im * uI;
                        double tmpi = re * uI + im * uR;

                        re = samples[even];
                        im = samples[even+1];

                        samples[even] = re + tmpr;
                        samples[even+1] = im + tmpi;

                        samples[odd] = re - tmpr;
                        samples[odd+1] = im - tmpi;
                    }
                }
            }
        }

        public void DanielsonLanczosTransformMultiDim(double[] samples, int stride, int step, bool forward, TransformationConvention convention)
        {
            int levels = Fn.IntLog2(stride / step);

            // precompute coefficients if they're not already there.
            BuildCoefficientsForLevels(levels);
            double expSignConvention = (convention & TransformationConvention.InverseExponent) > 0 ? -1d : 1d;

            int N = step;
            for(int level = 1; level <= levels; level++)
            {
                int M = N;
                N <<= 1;

                double[] realCosine = RealCosineCoefficients(level, forward);
                double[] imagSine = ImaginarySineCoefficients(level, forward);

                for(int j = 0, jj = 0; jj < M; j++, jj += step)
                {
                    double uR = realCosine[j];
                    double uI = expSignConvention * imagSine[j];

                    for(int i = jj; i < jj + step; i += 2)
                        for(int even = i; even < samples.Length; even += N)
                        {
                            int odd = even + M;

                            double re = samples[odd];
                            double im = samples[odd + 1];

                            double tmpr = re * uR - im * uI;
                            double tmpi = re * uI + im * uR;

                            re = samples[even];
                            im = samples[even + 1];

                            samples[even] = re + tmpr;
                            samples[even + 1] = im + tmpi;

                            samples[odd] = re - tmpr;
                            samples[odd + 1] = im - tmpi;
                        }
                }
            }
        }

        //public void DanielsonLanczosTransform(double[] samples, bool forward)
        //{
        //    int length = samples.Length;
        //    int levels = Fn.IntLog2(length) - 1;

        //    // precompute coefficients if they're not already there.
        //    BuildCoefficientsForLevels(levels);

        //    int N = 2;
        //    for(int level = 1; level <= levels; level++)
        //    {
        //        int M = N;
        //        N <<= 1;

        //        double[] realCosine = RealCosineCoefficients(level, forward);
        //        double[] imagSine = ImaginarySineCoefficients(level, forward);

        //        for(int j = 0; j < M; j += 2)
        //        {
        //            double uR = realCosine[j / 2];
        //            double uI = imagSine[j / 2];

        //            for(int even = j; even < length; even += N)
        //            {
        //                int odd = even + M;

        //                double r = samples[odd];
        //                double i = samples[odd + 1];

        //                double odduR = r * uR - i * uI;
        //                double odduI = r * uI + i * uR;

        //                r = samples[even];
        //                i = samples[even + 1];

        //                samples[even] = r + odduR;
        //                samples[even + 1] = i + odduI;

        //                samples[odd] = r - odduR;
        //                samples[odd + 1] = i - odduI;
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Rescaling (Step 3)
        public void Rescale(double[] samples, bool forward, TransformationConvention convention)
        {
            if((convention & TransformationConvention.NoScaling) > 0)
                return;
            bool asymmetric = (convention & TransformationConvention.AsymmetricScaling) > 0;
            if(forward && asymmetric)
                return;
            double factor = 2.0 / samples.Length;
            if(!asymmetric)
                factor = Math.Sqrt(factor);
            for(int i = 0; i < samples.Length; i++)
                samples[i] *= factor;
        }
        #endregion

        #region Precomputation: Reverse Bits
        private int[][] reversedBitsLookup = new int[maxBits][];
        
        /// <summary>
        /// Permutates <c>numberOfBits</c> in ascending order
        /// and reverses each element's bits afterwards.
        /// </summary>
        public int[] ReverseBits(int numberOfBits)
        {
            System.Diagnostics.Debug.Assert(numberOfBits >= minBits);
            System.Diagnostics.Debug.Assert(numberOfBits <= maxBits);
            if(reversedBitsLookup[numberOfBits - 1] == null)
            {
                int len = Fn.IntPow2(numberOfBits);
                int[] reversedBits = new int[len];
                for(int i = 0; i < len; i++)
                {
                    int oldBits = i;
                    int newBits = 0;
                    for(int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits = (oldBits >> 1);
                    }
                    reversedBits[i] = newBits;
                }
                reversedBitsLookup[numberOfBits - 1] = reversedBits;
            }
            return reversedBitsLookup[numberOfBits - 1];
        }
        #endregion

        #region Precomputation: Real/Imag Coefficients (Complex Rotation)
        private double[,][] realCoefficients = new double[maxBits + 1, 2][];
        private double[,][] imagCoefficients = new double[maxBits + 1, 2][];

        /// <summary>
        /// Evaluates complex rotation coefficients if not already available
        /// and returns the (real) cosine lookup table.
        /// </summary>
        public double[] RealCosineCoefficients(int level, bool forward)
        {
            if(realCoefficients[level, 0] == null)
                BuildCoefficientsForLevels(level);
            return realCoefficients[level, forward ? 0 : 1];
        }

        /// <summary>
        /// Evaluates complex rotation coefficients if not already available
        /// and returns the (imaginary) sine lookup table.
        /// </summary>
        public double[] ImaginarySineCoefficients(int level, bool forward)
        {
            if(imagCoefficients[level, 0] == null)
                BuildCoefficientsForLevels(level);
            return imagCoefficients[level, forward ? 0 : 1];
        }

        /// <summary>
        /// Evaluates complex rotation coefficients if not already available.
        /// </summary>
        private void BuildCoefficientsForLevels(int levels)
        {
            if(realCoefficients[levels, 0] != null)
                return;

            int M = 1;
            double uRealFw, uImagFw, uRealBw, uImagBw, angle, wRreal, wImag, uwI;
            for(int level = 1; level <= levels; level++, M <<= 1)
            {
                if(realCoefficients[level, 0] != null)
                    continue;

                uRealFw = uRealBw = 1;
                uImagFw = uImagBw = 0;

                angle = Constants.Pi / M;
                wRreal = Trig.Cosine(angle);
                wImag = Trig.Sine(angle);

                double[] realForward = new double[M];
                double[] imagForward = new double[M];
                double[] realBackward = new double[M];
                double[] imagBackward = new double[M];

                for(int i = 0; i < M; i++)
                {
                    realForward[i] = uRealFw;
                    imagForward[i] = uImagFw;
                    realBackward[i] = uRealBw;
                    imagBackward[i] = uImagBw;

                    uwI = uImagFw * wRreal - uRealFw * wImag;
                    uRealFw = uRealFw * wRreal + uImagFw * wImag;
                    uImagFw = uwI;

                    uwI = uImagBw * wRreal + uRealBw * wImag;
                    uRealBw = uRealBw * wRreal - uImagBw * wImag;
                    uImagBw = uwI;
                }

                realCoefficients[level, 0] = realForward;
                imagCoefficients[level, 0] = imagForward;
                realCoefficients[level, 1] = realBackward;
                imagCoefficients[level, 1] = imagBackward;
            }
        }
        #endregion

        public double[] ExtendToPowerOf2Length(double[] samples)
        {
            int newlen = Fn.CeilingToPowerOf2(samples.Length);
            double[] ret = new double[newlen];
            for(int i = 0; i < samples.Length; i++)
                ret[i] = samples[i];
            // rest is padded with zero.
            return ret;
        }
    }
}
