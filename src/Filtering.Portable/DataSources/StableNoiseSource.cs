// <copyright file="StableNoiseSource.cs" company="Math.NET">
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
using MathNet.Filtering.Portable.Channel;
using MathNet.Numerics.Distributions;

namespace MathNet.Filtering.Portable.DataSources
{
    /// <summary>
    /// Sample source with skew alpha stable distributed samples.
    /// </summary>
    public class StableNoiseSource : IChannelSource
    {
        readonly IContinuousDistribution _distribution;

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="uniformWhiteRandomSource">Uniform white random source.</param>
        /// <param name="location">mu-parameter of the stable distribution</param>
        /// <param name="scale">c-parameter of the stable distribution</param>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public StableNoiseSource(Random uniformWhiteRandomSource, double location, double scale, double exponent, double skewness)
        {
            _distribution = new Stable(exponent, skewness, scale, location, uniformWhiteRandomSource);
        }

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="location">mu-parameter of the stable distribution</param>
        /// <param name="scale">c-parameter of the stable distribution</param>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public StableNoiseSource(double location, double scale, double exponent, double skewness)
        {
            _distribution = new Stable(exponent, skewness, scale, location);
        }

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public StableNoiseSource(double exponent, double skewness)
        {
            _distribution = new Stable(exponent, skewness, 1.0, 0.0);
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
