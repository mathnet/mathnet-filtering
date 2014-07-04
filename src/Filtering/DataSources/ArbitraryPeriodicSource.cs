// <copyright file="ArbitraryPeriodicSource.cs" company="Math.NET">
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

namespace MathNet.Filtering.DataSources
{
    /// <summary>
    /// Precomputed periodic sample source
    /// </summary>
    public class ArbitraryPeriodicSource : IChannelSource
    {
        readonly double[] _samples;
        readonly int _sampleCount;
        readonly int _delay;
        int _nextIndex;

        /// <summary>
        /// Create a new precomputed periodic sample source
        /// </summary>
        public ArbitraryPeriodicSource(double[] samples, int indexOffset, int delay)
        {
            if (null == samples)
            {
                throw new ArgumentNullException("samples");
            }

            if (0 == samples.Length)
            {
                throw new ArgumentOutOfRangeException("samples");
            }

            if (indexOffset < 0 || indexOffset >= samples.Length)
            {
                throw new ArgumentOutOfRangeException("indexOffset");
            }

            _samples = samples;
            _sampleCount = samples.Length;
            _delay = delay;

            int effectiveDelay = delay%_sampleCount;
            _nextIndex = (indexOffset - effectiveDelay + _sampleCount)%_sampleCount;
        }

        /// <summary>
        /// Create a new precomputed periodic sample source
        /// </summary>
        public ArbitraryPeriodicSource(double[] samples)
            : this(samples, 0, 0)
        {
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public double ReadNextSample()
        {
            double sample = _samples[_nextIndex];
            _nextIndex = (_nextIndex + 1)%_sampleCount;
            return sample;
        }

        /// <summary>
        /// Sample delay of this source in relation to the whole system.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
        }
    }
}
