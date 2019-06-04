// <copyright file="BilinearTransform.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2019 Math.NET
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
using System.Linq;
using System.Numerics;

namespace MathNet.Filtering.TransferFunctions
{
    /// <summary>
    /// Transform the list of zeros and poles (adjusting the gain accordingly) of a transfer function through the bilinear transform.
    /// </summary>
    public static class BilinearTransform
    {
        /// <summary>
        /// Applies the bilinear transform starting from the set of zeros and poles.
        /// </summary>
        /// <param name="gain">Transfer function gain.</param>
        /// <param name="zeros">List of zeros.</param>
        /// <param name="poles">List of poles.</param>
        /// <param name="samplingTime">Sampling time, computed as the inverse of sampling frequency.</param>
        /// <returns>Recomputed gain, list of zeros and list of poles.</returns>
        /// <exception cref="ArgumentException">The gain must be a finite value.</exception>
        /// <exception cref="ArgumentException">The sampling time must be a finite value.</exception>
        /// <exception cref="ArgumentNullException">The list of zeros must be not null.</exception>
        /// <exception cref="ArgumentNullException">The list of poles must be not null.</exception>
        /// <exception cref="ArgumentException">The number of poles must at least be equal to the number of zeros.</exception>
        /// <remarks>
        /// For performance reasons, the method edits the input list of zeros and poles in-place when feasible,
        /// so the input and output arrays might be the same object.
        /// </remarks>
        public static (double gain, Complex[] zeros, Complex[] poles) Apply(double gain, Complex[] zeros, Complex[] poles, double samplingTime)
        {
            Helpers.Validators.CheckDouble(gain, nameof(gain));
            Helpers.Validators.CheckDouble(samplingTime, nameof(samplingTime));
            Helpers.Validators.CheckNull(zeros, nameof(zeros));
            Helpers.Validators.CheckNull(poles, nameof(poles));

            var z = zeros.Length;
            var p = poles.Length;

            if (z > p || p == 0)
            {
                throw new ArgumentException("The number of poles must at least be equal to the number of zeros.", nameof(poles));
            }

            var nProd = zeros.Aggregate(Complex.One, (acc, zero) => acc *= (2 - (zero * samplingTime)) / samplingTime);
            var dProd = poles.Aggregate(Complex.One, (acc, pole) => acc *= (2 - (pole * samplingTime)) / samplingTime);

            gain = (gain * nProd / dProd).Real;

            for (int i = p - 1; i >= 0; i--)
            {
                poles[i] = (2 + (poles[i] * samplingTime)) / (samplingTime - (poles[i] * samplingTime));
            }

            if (z == 0)
            {
                zeros = Enumerable.Repeat(-Complex.One, p).ToArray();
            }
            else
            {
                var tmp = zeros;
                zeros = new Complex[p];

                for (int i = z - 1; i >= 0; i--)
                {
                    zeros[i] = (2 + (tmp[i] * samplingTime)) / (samplingTime - (tmp[i] * samplingTime));
                }

                Array.Copy(Enumerable.Repeat(-Complex.One, p - z).ToArray(), 0, zeros, z, p - z);
            }

            return (gain, zeros, poles);
        }
    }
}
