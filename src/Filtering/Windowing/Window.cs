// <copyright file="Window.cs" company="Math.NET">
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
using MathNet.Filtering.Properties;

namespace MathNet.Filtering.Windowing
{
    /// <summary>
    /// A windowing/apodization function.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Window width, number of samples. Typically an integer power of 2.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Window coefficients
        /// </summary>
        /// <param name="sampleIndex">window sample index, between 0 and Width-1 (inclusive).</param>
        double this[int sampleIndex] { get; }

        /// <summary>
        /// Copy the window coefficients to a double array.
        /// </summary>
        double[] CopyToArray();

        /// <summary>
        /// Compute the window for the current configuration.
        /// Skipped if the window has already been computed.
        /// </summary>
        void Precompute();
    }

    /// <summary>
    /// A windowing/apodization function.
    /// </summary>
    public abstract class Window :
        IWindow
    {
        int _width = 1; // N
        double[] _coefficients;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected Window()
        {
        }

        /// <summary>
        /// Windowing function generator implementation.
        /// </summary>
        /// <param name="width">window size, guaranteed to be greater than or equal to 2.</param>
        /// <returns>Window coefficients, array with length of 'width'</returns>
        protected abstract double[] ComputeWindowCore(int width);

        /// <summary>
        /// Reset the coefficients, triggering Precompute the next time a coefficient is requested.
        /// </summary>
        protected void Reset()
        {
            _coefficients = null;
        }

        /// <summary>
        /// Compute the window for the current configuration.
        /// Skipped if the window has already been computed.
        /// </summary>
        public void Precompute()
        {
            if (null != _coefficients)
            {
                // coefficients already available, skip
                return;
            }

            if (_width == 1)
            {
                // trivial window
                _coefficients = new double[1];
                _coefficients[0] = 1.0;
                return;
            }

            _coefficients = ComputeWindowCore(_width);

            if (null == _coefficients || _width != _coefficients.Length)
            {
                throw new ApplicationException(Resources.InvalidWindowFunctionException);
            }
        }

        /// <summary>
        /// Copy the window coefficients to a double array.
        /// </summary>
        public double[] CopyToArray()
        {
            Precompute();
            double[] data = new double[_width];
            _coefficients.CopyTo(data, 0);
            return data;
        }

        /// <summary>
        /// Window width, number of samples. Typically an integer power of 2, must be greater than 0.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int Width
        {
            get { return _width; }
            set
            {
                if (0 >= value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _width = value;
                Reset();
            }
        }

        /// <summary>
        /// Window coefficients
        /// </summary>
        /// <param name="sampleIndex">window sample index, between 0 and Width-1 (inclusive).</param>
        public double this[int sampleIndex]
        {
            get
            {
                if (0 > sampleIndex || _width <= sampleIndex)
                {
                    throw new ArgumentOutOfRangeException("sampleIndex");
                }

                if (null == _coefficients)
                {
                    Precompute();
                }

                return _coefficients[sampleIndex];
            }
        }
    }
}
