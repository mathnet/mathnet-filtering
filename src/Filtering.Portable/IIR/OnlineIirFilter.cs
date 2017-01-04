// <copyright file="OnlineIirFilter.cs" company="Math.NET">
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
using MathNet.Filtering.Portable.Properties;

namespace MathNet.Filtering.Portable.IIR
{
    /// <summary>
    /// Infinite Impulse Response (FIR) Filters need much
    /// less coefficients (and are thus much faster) than
    /// comparable FIR Filters, but are potentially unstable.
    /// IIR Filters are always online and causal. This IIR
    /// Filter implements the canonic Direct Form II structure.
    /// </summary>
    /// <remarks>
    /// System Description: H(z) = (b0 + b1*z^-1 + b2*z^-2) / (1 + a1*z^-1 + a2*z^-2)
    /// </remarks>
    public class OnlineIirFilter : OnlineFilter
    {
        readonly double[] _b;
        readonly double[] _a;
        readonly double[] _bufferX;
        readonly double[] _bufferY;
        readonly int _halfSize;
        int _offset;

        /// <summary>
        /// Infinite Impulse Response (IIR) Filter.
        /// </summary>
        public OnlineIirFilter(double[] coefficients)
        {
            if (null == coefficients)
            {
                throw new ArgumentNullException("coefficients");
            }

            if ((coefficients.Length & 1) != 0)
            {
                throw new ArgumentException(Resources.ArgumentEvenNumberOfCoefficients, "coefficients");
            }

            int size = coefficients.Length;
            _halfSize = size >> 1;
            _b = new double[size];
            _a = new double[size];

            for (int i = 0; i < _halfSize; i++)
            {
                _b[i] = _b[_halfSize + i] = coefficients[i];
                _a[i] = _a[_halfSize + i] = coefficients[_halfSize + i];
            }

            _bufferX = new double[size];
            _bufferY = new double[size];
        }

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public override double ProcessSample(double sample)
        {
            _offset = (_offset != 0) ? _offset - 1 : _halfSize - 1;
            _bufferX[_offset] = sample;
            _bufferY[_offset] = 0d;
            double yn = 0d;

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; i++, j++)
            {
                yn += _bufferX[i]*_b[j];
            }

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; i++, j++)
            {
                yn -= _bufferY[i]*_a[j];
            }

            _bufferY[_offset] = yn;
            return yn;
        }

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < _bufferX.Length; i++)
            {
                _bufferX[i] = 0d;
                _bufferY[i] = 0d;
            }
        }
    }
}
