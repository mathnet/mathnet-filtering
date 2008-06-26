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
    /// <para>The <c>ComplexFourierTransformation</c> provides algorithms
    /// for one, two and three dimensional fast fourier transformations
    /// (FFT) on complex vectors.</para>
    /// <para>This class caches precomputations locally, thus consider reusing/caching it.</para>
    /// </summary>
    public class ComplexFourierTransformation
    {
        InternalFFT _fft;
        TransformationConvention _convention;

        /// <summary>
        /// Construct a complex fourier transformation instance.
        /// </summary>
        public
        ComplexFourierTransformation()
        {
            _fft = new InternalFFT();
            _convention = TransformationConvention.Default;
        }

        /// <summary>
        /// Construct a complex fourier transformation instance with a given convention.
        /// </summary>
        /// <param name="convention">Fourier Transformation Convention</param>
        public
        ComplexFourierTransformation(
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
        /// <param name="numberOfSamplePairs">The real &amp; complex numbers count together as only one sample.</param>
        public
        double[]
        GenerateTimeScale(
            double sampleRate,
            int numberOfSamplePairs
            )
        {
            double[] scale = new double[numberOfSamplePairs];
            double t = 0, step = 1.0 / sampleRate;
            for(int i = 0; i < numberOfSamplePairs; i++)
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
        /// <param name="numberOfSamplePairs">The real &amp; complex numbers count together as only one sample.</param>
        public
        double[]
        GenerateFrequencyScale(
            double sampleRate,
            int numberOfSamplePairs
            )
        {
            double[] scale = new double[numberOfSamplePairs];
            double f = 0, step = sampleRate / numberOfSamplePairs;
            int secondHalf = (numberOfSamplePairs >> 1) + 1;
            for(int i = 0; i < secondHalf; i++)
            {
                scale[i] = f;
                f += step;
            }

            f = -step * (secondHalf - 2);
            for(int i = secondHalf; i < numberOfSamplePairs; i++)
            {
                scale[i] = f;
                f += step;
            }

            return scale;
        }
        #endregion

        #region Dimensions: 1
        /// <summary>
        /// Inplace Forward Transformation in one dimension. Size must be Power of Two.
        /// </summary>
        /// <param name="samplePairs">Complex samples (even = real, odd = imaginary). Length must be a power of two.</param>
        public
        void
        TransformForward(
            double[] samplePairs
            )
        {
            if(Fn.CeilingToPowerOf2(samplePairs.Length) != samplePairs.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samplePairs");
            }

            _fft.DiscreteFourierTransform(samplePairs, true, _convention);
        }

        /// <summary>
        /// Inplace Forward Transformation in one dimension. Size must be Power of Two.
        /// </summary>
        /// <param name="samples">Complex samples. Length must be a power of two.</param>
        /// <remarks>
        /// This method provides a simple shortcut if your data is already available as
        /// <see cref="Complex"/> type instances. However, if not, consider using the
        /// overloaded method with double pairs instead, it requires less internal copying.
        /// </remarks>
        public
        void
        TransformForward(
            Complex[] samples
            )
        {
            if(Fn.CeilingToPowerOf2(samples.Length) != samples.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samples");
            }

            double[] samplePairs = new double[samples.Length << 1];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samplePairs[j] = samples[i].Real;
                samplePairs[j + 1] = samples[i].Imag;
            }

            _fft.DiscreteFourierTransform(samplePairs, true, _convention);

            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samples[i].Real = samplePairs[j];
                samples[i].Imag = samplePairs[j + 1];
            }
        }

        /// <summary>
        /// Inplace Backward Transformation in one dimension. Size must be Power of Two.
        /// </summary>
        /// <param name="samplePairs">Complex samples (even = real, odd = imaginary). Length must be a power of two.</param>
        public
        void
        TransformBackward(
            double[] samplePairs
            )
        {
            if(Fn.CeilingToPowerOf2(samplePairs.Length) != samplePairs.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samplePairs");
            }

            _fft.DiscreteFourierTransform(samplePairs, false, _convention);
        }

        /// <summary>
        /// Inplace Backward Transformation in one dimension. Size must be Power of Two.
        /// </summary>
        /// <param name="samples">Complex samples. Length must be a power of two.</param>
        /// <remarks>
        /// This method provides a simple shortcut if your data is already available as
        /// <see cref="Complex"/> type instances. However, if not, consider using the
        /// overloaded method with double pairs instead, it requires less internal copying.
        /// </remarks>
        public
        void
        TransformBackward(
            Complex[] samples
            )
        {
            if(Fn.CeilingToPowerOf2(samples.Length) != samples.Length)
            {
                throw new ArgumentException(Resources.ArgumentPowerOfTwo, "samplePairs");
            }

            double[] samplePairs = new double[samples.Length << 1];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samplePairs[j] = samples[i].Real;
                samplePairs[j + 1] = samples[i].Imag;
            }

            _fft.DiscreteFourierTransform(samplePairs, false, _convention);
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samples[i].Real = samplePairs[j];
                samples[i].Imag = samplePairs[j + 1];
            }
        }
        #endregion

        #region Dimensions: n
        /// <summary>
        /// Inplace Forward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        /// <param name="samplePairs">Complex samples (even = real, odd = imaginary). Length must be a power of two in each dimension.</param>
        /// <param name="dimensionLengths">Sizes, must be Power of Two in each dimension</param>
        public
        void
        TransformForward(
            double[] samplePairs,
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

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, true, _convention);
        }

        /// <summary>
        /// Inplace Forward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        /// <param name="samples">Complex samples. Length must be a power of two in each dimension.</param>
        /// <remarks>
        /// This method provides a simple shortcut if your data is already available as
        /// <see cref="Complex"/> type instances. However, if not, consider using the
        /// overloaded method with double pairs instead, it requires less internal copying.
        /// </remarks>
        /// <param name="dimensionLengths">Sizes, must be Power of Two in each dimension</param>
        public
        void
        TransformForward(
            Complex[] samples,
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

            double[] samplePairs = new double[samples.Length << 1];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samplePairs[j] = samples[i].Real;
                samplePairs[j + 1] = samples[i].Imag;
            }

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, true, _convention);

            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samples[i].Real = samplePairs[j];
                samples[i].Imag = samplePairs[j + 1];
            }
        }

        /// <summary>
        /// Inplace Backward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        /// <param name="samplePairs">Complex samples (even = real, odd = imaginary). Length must be a power of two in each dimension.</param>
        /// <param name="dimensionLengths">Sizes, must be Power of Two in each dimension</param>
        public
        void
        TransformBackward(
            double[] samplePairs,
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

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, false, _convention);
        }

        /// <summary>
        /// Inplace Backward Transformation in multiple dimensions.
        /// Size must be Power of Two in each dimension.
        /// The Data is expected to be ordered such that the last index changes most rapidly (in 2D this means row-by-row when indexing as [y,x]).
        /// </summary>
        /// <param name="samples">Complex samples. Length must be a power of two.</param>
        /// <remarks>
        /// This method provides a simple shortcut if your data is already available as
        /// <see cref="Complex"/> type instances. However, if not, consider using the
        /// overloaded method with double pairs instead, it requires less internal copying.
        /// </remarks>
        /// <param name="dimensionLengths">Sizes, must be Power of Two in each dimension</param>
        public
        void
        TransformBackward(
            Complex[] samples,
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

            double[] samplePairs = new double[samples.Length << 1];
            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samplePairs[j] = samples[i].Real;
                samplePairs[j + 1] = samples[i].Imag;
            }

            _fft.DiscreteFourierTransformMultiDim(samplePairs, dimensionLengths, false, _convention);

            for(int i = 0, j = 0; i < samples.Length; i++, j += 2)
            {
                samples[i].Real = samplePairs[j];
                samples[i].Imag = samplePairs[j + 1];
            }
        }
        #endregion
    }
}
