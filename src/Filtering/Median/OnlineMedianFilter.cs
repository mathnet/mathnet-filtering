// <copyright file="MedianFilter.cs" company="Math.NET">
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

using System.Linq;
using MathNet.Numerics.Statistics;

namespace MathNet.Filtering.Median
{
    /// <summary>
    /// Median-Filters are non-linear filters, returning
    /// the median of a sample window as output. Median-Filters
    /// perform well for de-noise applications where it's
    /// important to not loose sharp steps/edges.
    /// </summary>
    public class OnlineMedianFilter : OnlineFilter
    {
        readonly double[] _buffer;
        int _offset;
        bool _bufferFull;

        /// <summary>
        /// Create a Median Filter
        /// </summary>
        public OnlineMedianFilter(int windowSize)
        {
            _buffer = new double[windowSize];
        }

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public override double ProcessSample(double sample)
        {
            _buffer[_offset = (_offset == 0) ? _buffer.Length - 1 : _offset - 1] = sample;
            _bufferFull |= _offset == 0;

            var data = _bufferFull ? _buffer : _buffer.Skip(_offset);
            return data.Median();
        }

        /// <summary>
        /// Reset internal state.
        /// </summary>
        public override void Reset()
        {
            _offset = 0;
            _bufferFull = false;
        }
    }
}
