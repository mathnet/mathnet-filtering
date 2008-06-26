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
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Transformations
{
    /// <summary>
    /// <para>The <c>RealFourierTransformation</c> provides algorithms
    /// for one, two and three dimensional fast fourier transformations
    /// (FFT) on real vectors.</para>
    /// <para>This class caches precomputations locally, thus consider reusing/caching it.</para>
    /// </summary>
    public class RealFourierTransformation
    {
        InternalFFT _fft;
        TransformationConvention _convention;

        /// <summary>
        /// Construct a real fourier transformation instance.
        /// </summary>
        public
        RealFourierTransformation()
        {
            _fft = new InternalFFT();
            _convention = TransformationConvention.Default;
        }

        /// <summary>
        /// Construct a real fourier transformation instance with a given convention.
        /// </summary>
        /// <param name="convention">Fourier Transformation convention</param>
        public
        RealFourierTransformation(
            TransformationConvention convention
            )
        {
            _fft = new InternalFFT();
            _convention = convention;
        }

        /// <summary>
        /// Fourier Transformation Convention
        /// </summary>
        public TransformationConvention Convention
        {
            get { return _convention; }
            set { _convention = value; }
        }

        #region Scales
        /// <summary>
        /// Generate the expected sample points in time space.
        /// </summary>
        /// <param name="sampleRate">The sampling rate of the time-space data.</param>
        /// <param name="numberOfSamples">Number of data samples.</param>
        public
        double[]
        GenerateTimeScale(
            double sampleRate,
            int numberOfSamples
            )
        {
            double[] scale = new double[numberOfSamples];
            double t = 0, step = 1.0 / sampleRate;
            for(int i = 0; i < numberOfSamples; i++)
            {
                scale[i] = t;
                t += step;
            }

            return scale;
        }

        /// <summary>
        /// Generate the expected sample points in frequency space.
        /// </summary>
        /// <param name="sampleRate">The sampling rate of the time-space data.</param>
        /// <param name="numberOfSamples">Number of data samples.</param>
        public
        double[]
        GenerateFrequencyScale(
            double sampleRate,
            int numberOfSamples
            )
        {
            double[] scale = new double[numberOfSamples];
            double f = 0, step = sampleRate / numberOfSamples;
            int secondHalf = (numberOfSamples >> 1) + 1;
            for(int i = 0; i < secondHalf; i++)
            {
                scale[i] = f;
                f += step;
            }

            f = -step * (secondHalf - 2);
            for(int i = secondHalf; i < numberOfSamples; i++)
            {
                scale[i] = f;
                f += step;
            }

            return scale;
        }
        #endregion

        #region Dimensions: 1 - Two Vector
        /// <summary>
        /// Outplace Forward Transformation in one dimension for two vectors with the same length.
        /// Size must be Power of Two.
        /// </summary>
        /// <param name="samples1">Real samples. Length must be a power of two.</param>
        /// <param name="samples2">Real samples. Length must be a power of two.</param>
        /// <param name="fftReal1">Output for the first sample set, real part.</param>
        /// <param name="fftImag1">Output for the first sample set, imaginary part</param>
        /// <param name="fftReal2">Output for the second sample set, real part.</param>
        /// <param name="fftImag2">Output for the second sample set, imaginary part.</param>
        public
        void
        TransformForward(
            double[] samples1,
            double[] samples2,
            out double[] fftReal1,
            out double[] fftImag1,
            out double[] fftReal2,
            out double[] fftImag2
            )
        {
            if(samples1.Length != samples2.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths, "samples2");
            }

            if(Fn.CeilingToPowerOf2(samples1.Length) != samples1.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samples1");
            }

            int numSamples = samples1.Length;
            int length = numSamples << 1;

            // Pack together to one complex vector

            double[] complex = new double[length];
            for(int i = 0, j = 0; i < numSamples; i++, j += 2)
            {
                complex[j] = samples1[i];
                complex[j + 1] = samples2[i];
            }

            // Transform complex vector

            _fft.DiscreteFourierTransform(complex, true, _convention);

            // Reconstruct data for the two vectors by using symmetries

            fftReal1 = new double[numSamples];
            fftImag1 = new double[numSamples];
            fftReal2 = new double[numSamples];
            fftImag2 = new double[numSamples];

            double h1r, h2i, h2r, h1i;

            fftReal1[0] = complex[0];
            fftReal2[0] = complex[1];
            fftImag1[0] = fftImag2[0] = 0d;
            for(int i = 1, j = 2; j <= numSamples; i++, j += 2)
            {
                h1r = 0.5 * (complex[j] + complex[length - j]);
                h1i = 0.5 * (complex[j + 1] - complex[length + 1 - j]);
                h2r = 0.5 * (complex[j + 1] + complex[length + 1 - j]);
                h2i = -0.5 * (complex[j] - complex[length - j]);

                fftReal1[i] = h1r;
                fftImag1[i] = h1i;
                fftReal1[numSamples - i] = h1r;
                fftImag1[numSamples - i] = -h1i;

                fftReal2[i] = h2r;
                fftImag2[i] = h2i;
                fftReal2[numSamples - i] = h2r;
                fftImag2[numSamples - i] = -h2i;
            }
        }

        /// <summary>
        /// Outplace Backward Transformation in one dimension for two vectors with the same length.
        /// Size must be Power of Two.
        /// </summary>
        /// <param name="fftReal1">Real part of the first vector in frequency space. Length must be a power of two.</param>
        /// <param name="fftImag1">Imaginary part of the first vector in frequency space. Length must be a power of two.</param>
        /// <param name="fftReal2">Real part of the second vector in frequency space. Length must be a power of two.</param>
        /// <param name="fftImag2">Imaginary part of the second vector in frequency space. Length must be a power of two.</param>
        /// <param name="samples1">Output samples for the first vector.</param>
        /// <param name="samples2">Output samples for te second vector.</param>
        public
        void
        TransformBackward(
            double[] fftReal1,
            double[] fftImag1,
            double[] fftReal2,
            double[] fftImag2,
            out double[] samples1,
            out double[] samples2
            )
        {
            if(fftReal1.Length != fftImag1.Length || fftReal2.Length != fftImag2.Length || fftReal1.Length != fftReal2.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths);
            }

            if(Fn.CeilingToPowerOf2(fftReal1.Length) != fftReal1.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "fftReal1");
            }

            int numSamples = fftReal1.Length;
            int length = numSamples << 1;

            // Pack together to one complex vector

            double[] complex = new double[length];
            for(int i = 0, j = 0; i < numSamples; i++, j += 2)
            {
                complex[j] = fftReal1[i] - fftImag2[i];
                complex[j + 1] = fftImag1[i] + fftReal2[i];
            }

            // Transform complex vector

            _fft.DiscreteFourierTransform(complex, false, _convention);

            // Reconstruct data for the two vectors

            samples1 = new double[numSamples];
            samples2 = new double[numSamples];

            for(int i = 0, j = 0; i < numSamples; i++, j += 2)
            {
                samples1[i] = complex[j];
                samples2[i] = complex[j + 1];
            }
        }
        #endregion

        #region Dimensions: 1 - Single Vector
        /// <summary>
        /// Outplace Forward Transformation in one dimension.
        /// Size must be Power of Two.
        /// </summary>
        /// <param name="samples">Real samples. Length must be a power of two.</param>
        /// <param name="fftReal">Output of the sample set, real part.</param>
        /// <param name="fftImag">Output of the sample set, imaginary part.</param>
        public
        void
        TransformForward(
            double[] samples,
            out double[] fftReal,
            out double[] fftImag
            )
        {
            if(Fn.CeilingToPowerOf2(samples.Length) != samples.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samples");
            }

            int length = samples.Length;
            int numSamples = length >> 1;
            double expSignConvention = (_convention & TransformationConvention.InverseExponent) > 0 ? -1d : 1d;

            // Transform odd and even vectors (packed as one complex vector)

            // We work on a copy so the original array is not changed.
            double[] complex = new double[length];
            for(int i = 0; i < complex.Length; i++)
            {
                complex[i] = samples[i];
            }

            _fft.DiscreteFourierTransform(complex, true, _convention);

            // Reconstruct data for the two vectors by using symmetries

            double theta = Constants.Pi / numSamples;
            double wtemp = Trig.Sine(0.5 * theta);
            double wpr = -2.0 * wtemp * wtemp;
            double wpi = expSignConvention * Trig.Sine(theta);
            double wr = 1.0 + wpr;
            double wi = wpi;

            fftReal = new double[length];
            fftImag = new double[length];

            double h1r, h2i, h2r, h1i;

            fftImag[0] = fftImag[numSamples] = 0d;
            fftReal[0] = complex[0] + complex[1];
            fftReal[numSamples] = complex[0] - complex[1];
            for(int i = 1, j = 2; j <= numSamples; i++, j += 2)
            {
                h1r = 0.5 * (complex[j] + complex[length - j]);
                h1i = 0.5 * (complex[j + 1] - complex[length + 1 - j]);
                h2r = 0.5 * (complex[j + 1] + complex[length + 1 - j]);
                h2i = -0.5 * (complex[j] - complex[length - j]);

                fftReal[i] = h1r + wr * h2r + wi * h2i;
                fftImag[i] = h1i + wr * h2i - wi * h2r;
                fftReal[numSamples - i] = h1r - wr * h2r - wi * h2i;
                fftImag[numSamples - i] = -h1i + wr * h2i - wi * h2r;

                // For consistency and completeness we also provide the
                // negative spectrum, even though it's redundant in the real case.
                fftReal[numSamples + i] = fftReal[numSamples - i];
                fftImag[numSamples + i] = -fftImag[numSamples - i];
                fftReal[length - i] = fftReal[i];
                fftImag[length - i] = -fftImag[i];

                wr = (wtemp = wr) * wpr - wi * wpi + wr;
                wi = wi * wpr + wtemp * wpi + wi;
            }
        }

        /// <summary>
        /// Outplace Backward Transformation in one dimension.
        /// Size must be Power of Two.
        /// </summary>
        public
        void
        TransformBackward(
            double[] fftReal,
            double[] fftImag,
            out double[] samples
            )
        {
            if(fftReal.Length != fftImag.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths);
            }

            if(Fn.CeilingToPowerOf2(fftReal.Length) != fftReal.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samples");
            }

            int length = fftReal.Length;
            int numSamples = length >> 1;
            double expSignConvention = (_convention & TransformationConvention.InverseExponent) > 0 ? -1d : 1d;

            double theta = -Constants.Pi / numSamples;
            double wtemp = Trig.Sine(0.5 * theta);
            double wpr = -2.0 * wtemp * wtemp;
            double wpi = expSignConvention * Trig.Sine(theta);
            double wr = 1.0 + wpr;
            double wi = wpi;

            samples = new double[length];

            double h1r, h2i, h2r, h1i;

            // TODO: may be 1 <--> 0 swapped?
            samples[1] = 0.5 * (fftReal[0] - fftReal[numSamples]);
            samples[0] = 0.5 * (fftReal[0] + fftReal[numSamples]);

            for(int i = 1, j = 2; j <= numSamples; i++, j += 2)
            {
                h1r = 0.5 * (fftReal[i] + fftReal[numSamples - i]);
                h1i = 0.5 * (fftImag[i] - fftImag[numSamples - i]);
                h2r = -0.5 * (fftImag[i] + fftImag[numSamples - i]);
                h2i = 0.5 * (fftReal[i] - fftReal[numSamples - i]);

                samples[j] = h1r + wr * h2r + wi * h2i;
                samples[j + 1] = h1i + wr * h2i - wi * h2r;
                samples[length - j] = h1r - wr * h2r - wi * h2i;
                samples[length + 1 - j] = -h1i + wr * h2i - wi * h2r;

                wr = (wtemp = wr) * wpr - wi * wpi + wr;
                wi = wi * wpr + wtemp * wpi + wi;
            }

            // Transform odd and even vectors (packed as one complex vector)

            _fft.DiscreteFourierTransform(samples, false, _convention);
        }
        #endregion

        #region Dimensions: n
        /// <summary>
        /// Outplace Forward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        /// <param name="samples">Real samples. Length must be a power of two in each dimension.</param>
        /// <param name="fftReal">Output of the sample set, real part.</param>
        /// <param name="fftImag">Output of the sample set, imaginary part.</param>
        /// <param name="dimensionLengths">Sizes, must be Power of Two in each dimension</param>
        public
        void
        TransformForward(
            double[] samples,
            out double[] fftReal,
            out double[] fftImag,
            params int[] dimensionLengths
            )
        {
            for(int i = 0; i < dimensionLengths.Length; i++)
            {
                if(Fn.CeilingToPowerOf2(dimensionLengths[i]) != dimensionLengths[i])
                {
                    throw new ArgumentException(Resources.ArgumentPowerOfTwoEveryDimension, "dimensionLengths");
                }
            }

            // TODO: Implement real version (at the moment this is just a wrapper to the complex version)!

            double[] samplePairs = new double[samples.Length << 1];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samplePairs[j] = samples[i];
            }

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, true, _convention);

            fftReal = new double[samples.Length];
            fftImag = new double[samples.Length];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                fftReal[i] = samplePairs[j];
                fftImag[i] = samplePairs[j + 1];
            }
        }

        /// <summary>
        /// Outplace Backward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        public
        void
        TransformBackward(
            double[] fftReal,
            double[] fftImag,
            out double[] samples,
            params int[] dimensionLengths
            )
        {
            if(fftReal.Length != fftImag.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths);
            }

            for(int i = 0; i < dimensionLengths.Length; i++)
            {
                if(Fn.CeilingToPowerOf2(dimensionLengths[i]) != dimensionLengths[i])
                {
                    throw new ArgumentException(Resources.ArgumentPowerOfTwoEveryDimension, "dimensionLengths");
                }
            }

            // TODO: Implement real version (at the moment this is just a wrapper to the complex version)!

            double[] samplePairs = new double[fftReal.Length << 1];
            for(int i = 0, j = 0; i < fftReal.Length; i++, j += 2)
            {
                samplePairs[j] = fftReal[i];
                samplePairs[j + 1] = fftImag[i];
            }

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, false, _convention);

            samples = new double[fftReal.Length];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samples[i] = samplePairs[j];
            }
        }
        #endregion
    }
}
