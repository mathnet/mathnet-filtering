// <copyright file="SignalGenerator.cs" company="Math.NET">
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

namespace MathNet.Filtering.DataSources
{
    /// <summary>
    /// Generators for sinusoidal and theoretical signal vectors.
    /// </summary>
    public static class SignalGenerator
    {
        /// <summary>
        /// Create a Sine Signal Sample Vector.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="frequency">Frequency in samples per unit.</param>
        /// <param name="phase">Optional phase offset.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Sine(double samplingRate, double frequency, double phase, double amplitude, int length)
        {
            double[] data = new double[length];
            double step = frequency/samplingRate*2*Math.PI;

            for (int i = 0; i < length; i++)
            {
                data[i] = amplitude*Math.Sin(phase + i*step);
            }

            return data;
        }

        /// <summary>
        /// Create a Heaviside Step Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Step(int offset, double amplitude, int length)
        {
            var data = new double[length];
            int cursor;

            for (cursor = 0; cursor < offset && cursor < length; cursor++)
            {
                data[cursor] = 0d;
            }

            for (; cursor < length; cursor++)
            {
                data[cursor] = amplitude;
            }

            return data;
        }

        /// <summary>
        /// Create a Dirac Delta Impulse Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="frequency">impulse sequence frequency. -1 for single impulse only.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Impulse(int offset, int frequency, double amplitude, int length)
        {
            var data = new double[length];

            if (frequency <= 0)
            {
                data[offset] = amplitude;
            }
            else
            {
                while (offset < length)
                {
                    data[offset] = amplitude;
                    offset += frequency;
                }
            }

            return data;
        }
    }
}
