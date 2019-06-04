// <copyright file="TransferFunctionTransformer.cs" company="Math.NET">
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
    /// Transforms the zeros, the poles and the gain of a low-pass or band-pass prototype to the required filter type.
    /// </summary>
    internal static class TransferFunctionTransformer
    {
        /// <summary>
        /// Recomputes the gain and the list of zeros and poles for a low-pass filter.
        /// </summary>
        /// <param name="gain">Initial gain.</param>
        /// <param name="zeros">List of zeros.</param>
        /// <param name="poles">List of poles.</param>
        /// <param name="wc">Cutoff frequency.</param>
        /// <returns>Recomputed gain, list of zeros and list of poles.</returns>
        /// <remarks>
        /// For performance reasons, the method edits the input list of zeros and poles in place, so the input and output
        /// arrays are actually the same object.
        /// </remarks>
        internal static (double gain, Complex[] zeros, Complex[] poles) LowPass(double gain, Complex[] zeros, Complex[] poles, double wc)
        {
            var z = zeros.Length;
            var p = poles.Length;

            gain *= Math.Pow(1 / wc, z - p);

            for (int i = z - 1; i >= 0; i--)
            {
                zeros[i] *= wc;
            }

            for (int i = p - 1; i >= 0; i--)
            {
                poles[i] *= wc;
            }

            return (gain, zeros, poles);
        }

        /// <summary>
        /// Recomputes the gain and the list of zeros and poles for a high-pass filter.
        /// </summary>
        /// <param name="gain">Initial gain.</param>
        /// <param name="zeros">List of zeros.</param>
        /// <param name="poles">List of poles.</param>
        /// <param name="wc">Cutoff frequency.</param>
        /// <returns>Recomputed gain, list of zeros and list of poles.</returns>
        /// <remarks>
        /// For performance reasons, the method edits the input list of zeros and poles in place, so the input and output
        /// arrays are actually the same object.
        /// </remarks>
        internal static (double gain, Complex[] zeros, Complex[] poles) HighPass(double gain, Complex[] zeros, Complex[] poles, double wc)
        {
            var z = zeros.Length;
            var p = poles.Length;

            var pProd = poles.Aggregate(Complex.One, (acc, pole) => acc *= -pole);

            if (z == 0)
            {
                gain *= (Complex.One / pProd).Real;
                zeros = Enumerable.Repeat(Complex.Zero, p).ToArray();
            }
            else
            {
                var zProd = zeros.Aggregate(Complex.One, (acc, zero) => acc *= -zero);

                gain *= (zProd / pProd).Real;

                for (int i = z - 1; i >= 0; i--)
                {
                    zeros[i] = wc / zeros[i];
                }

                if (p > z)
                {
                    var tmp = zeros;
                    zeros = new Complex[p];
                    Array.Copy(tmp, zeros, z);
                }
            }

            for (int i = p - 1; i >= 0; i--)
            {
                poles[i] = wc / poles[i];
            }

            return (gain, zeros, poles);
        }

        /// <summary>
        /// Recomputes the gain and the list of zeros and poles for a band-pass filter.
        /// </summary>
        /// <param name="gain">Initial gain.</param>
        /// <param name="zeros">List of zeros.</param>
        /// <param name="poles">List of poles.</param>
        /// <param name="wc1">Lower cutoff frequency.</param>
        /// <param name="wc2">Higher cutoff frequency.</param>
        /// <returns>Recomputed gain, list of zeros and list of poles.</returns>
        /// <remarks>
        /// For performance reasons, the method edits the input list of zeros and poles in place, so the input and output
        /// arrays are actually the same object.
        /// </remarks>
        internal static (double gain, Complex[] zeros, Complex[] poles) BandPass(double gain, Complex[] zeros, Complex[] poles, double wc1, double wc2)
        {
            var z = zeros.Length;
            var p = poles.Length;

            gain *= Math.Pow(1 / (wc2 - wc1), z - p);

            { // scopes b
                var b = poles.Select(pole => pole * ((wc2 - wc1) / 2)).ToArray();

                poles = new Complex[2 * p];

                for (int i = p - 1; i >= 0; i--)
                {
                    var sqrt = Complex.Sqrt((b[i] * b[i]) - (wc2 * wc1));
                    poles[i] = b[i] + sqrt;
                    poles[i + p] = b[i] - sqrt;
                }
            }

            if (z == 0)
            {
                zeros = Enumerable.Repeat(Complex.Zero, p).ToArray();
            }
            else
            {
                var a = zeros.Select(zero => zero * ((wc2 - wc1) / 2)).ToArray();

                zeros = new Complex[2 * z];

                for (int i = z - 1; i >= 0; i--)
                {
                    var sqrt = Complex.Sqrt((a[i] * a[i]) - (wc2 * wc1));
                    zeros[i] = a[i] + sqrt;
                    zeros[i + z] = a[i] - sqrt;
                }

                if (poles.Length > zeros.Length)
                {
                    var tmp = zeros;
                    zeros = new Complex[poles.Length];
                    Array.Copy(tmp, zeros, zeros.Length);
                }
            }

            return (gain, zeros, poles);
        }

        /// <summary>
        /// Recomputes the gain and the list of zeros and poles for a band-stop filter.
        /// </summary>
        /// <param name="gain">Initial gain.</param>
        /// <param name="zeros">List of zeros.</param>
        /// <param name="poles">List of poles.</param>
        /// <param name="wc1">Lower cutoff frequency.</param>
        /// <param name="wc2">Higher cutoff frequency.</param>
        /// <returns>Recomputed gain, list of zeros and list of poles.</returns>
        /// <remarks>
        /// For performance reasons, the method edits the input list of zeros and poles in place, so the input and output
        /// arrays are actually the same object.
        /// </remarks>
        internal static (double gain, Complex[] zeros, Complex[] poles) BandStop(double gain, Complex[] zeros, Complex[] poles, double wc1, double wc2)
        {
            var z = zeros.Length;
            var p = poles.Length;

            if (z == 0)
            {
                gain *= (1 / poles.Aggregate(Complex.One, (aggr, pole) => aggr *= -pole)).Real;
            }
            else
            {
                gain *= (zeros.Aggregate(Complex.One, (aggr, zero) => aggr *= -zero) / poles.Aggregate(Complex.One, (aggr, pole) => aggr *= -pole)).Real;
            }

            { // scopes b
                var b = poles.Select(pole => (wc2 - wc1) / 2 / pole).ToArray();

                poles = new Complex[2 * p];

                for (int i = p - 1; i >= 0; i--)
                {
                    poles[i] = b[i] + Complex.Sqrt((b[i] * b[i]) - (wc2 * wc1));
                    poles[i + p] = b[i] - Complex.Sqrt((b[i] * b[i]) - (wc2 * wc1));
                }
            }

            if (z == 0)
            {
                var sqrt = new Complex(0, Complex.Sqrt(-(wc1 * wc2)).Imaginary);
                zeros = new Complex[2 * p];
                for (int i = (2 * p) - 1; i >= 0; i -= 2)
                {
                    zeros[i] = -sqrt;
                    zeros[i - 1] = sqrt;
                }
            }
            else
            {
                var a = zeros.Select(zero => (wc2 - wc1) / 2 / zero).ToArray();

                zeros = new Complex[2 * z];
                for (int i = z - 1; i >= 0; i--)
                {
                    var sqrt = Complex.Sqrt((a[i] * a[i]) - (wc1 * wc2));
                    zeros[i] = a[i] + sqrt;
                    zeros[i + z] = a[i] - sqrt;
                }

                if (p > z)
                {
                    var sqrt = Complex.Sqrt(-(wc1 * wc2));
                    var tmp = zeros;
                    zeros = new Complex[zeros.Length + (2 * (p - z))];
                    Array.Copy(tmp, zeros, tmp.Length);
                    for (int i = tmp.Length; i < zeros.Length; i += 2)
                    {
                        zeros[i] = -sqrt;
                        zeros[i + 1] = sqrt;
                    }
                }
            }

            return (gain, zeros, poles);
        }
    }
}
