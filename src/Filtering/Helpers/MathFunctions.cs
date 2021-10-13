// <copyright file="MathFunctions.cs" company="Math.NET">
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
using System.Numerics;
using MathNet.Numerics;

namespace MathNet.Filtering.Helpers
{
    /// <summary>
    /// Collection of mathematical functions used across the project.
    /// </summary>
    internal static class MathFunctions
    {
        /// <summary>
        /// Computes the warped frequency.
        /// </summary>
        /// <param name="cutoffFrequency">Digital cutoff frequency.</param>
        /// <param name="samplingTime">Sampling time (computed as the reverse of the sampling frequency).</param>
        /// <returns>Warped frequency.</returns>
        internal static double WarpFrequency(double cutoffFrequency, double samplingTime)
        {
            return Math.Tan(Math.PI * cutoffFrequency / samplingTime);
        }

        /// <summary>
        /// Computes the coefficients of the polynomial whose solutions are roots are given as input parameter.
        /// </summary>
        /// <param name="roots">Roots of the polynomial.</param>
        /// <returns>Polynomial coefficients.</returns>
        internal static Complex[] PolynomialCoefficients(Complex[] roots)
        {
            Complex[] y = Generate.Repeat(roots.Length + 1, Complex.Zero);
            y[0] = Complex.One;

            for (int i = 0; i < roots.Length; i++)
            {
                for (int j = i; j >= 0; j--)
                {
                    y[j + 1] -= roots[i] * y[j];
                }
            }

            return y;
        }
    }
}
