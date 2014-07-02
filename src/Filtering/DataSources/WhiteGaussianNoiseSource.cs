// <copyright file="WhiteGaussianNoiseSource.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using MathNet.Filtering.Channel;
using MathNet.Numerics.Distributions;

namespace MathNet.Filtering.DataSources
{
    /// <summary>
    /// Sample source with independent amplitudes of normal distribution and a flat spectral density.
    /// </summary>
    public class WhiteGaussianNoiseSource : IChannelSource
    {
        readonly IContinuousDistribution _distribution;

        /// <summary>
        /// Create a gaussian noise source with normally distributed amplitudes.
        /// </summary>
        /// <param name="uniformWhiteRandomSource">Uniform white random source.</param>
        /// <param name="mean">mu-parameter of the normal distribution</param>
        /// <param name="standardDeviation">sigma-parameter of the normal distribution</param>
        public WhiteGaussianNoiseSource(Random uniformWhiteRandomSource, double mean, double standardDeviation)
        {
            _distribution = new Normal(mean, standardDeviation, uniformWhiteRandomSource);
        }

        /// <summary>
        /// Create a gaussian noise source with normally distributed amplites.
        /// </summary>
        /// <param name="mean">mu-parameter of the normal distribution</param>
        /// <param name="standardDeviation">sigma-parameter of the normal distribution</param>
        public WhiteGaussianNoiseSource(double mean, double standardDeviation)
        {
            // assuming the default random source is white
            _distribution = new Normal(mean, standardDeviation);
        }

        /// <summary>
        /// Create a gaussian noise source with standard distributed amplitudes.
        /// </summary>
        public WhiteGaussianNoiseSource()
        {
            // assuming the default random source is white
            _distribution = new Normal();
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public double ReadNextSample()
        {
            return _distribution.Sample();
        }

        /// <summary>
        /// Sample delay of this source in relation to the whole system. Constant Zero.
        /// </summary>
        public int Delay
        {
            get { return 0; }
        }
    }
}
