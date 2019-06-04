// <copyright file="Validators.cs" company="Math.NET">
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

namespace MathNet.Filtering.Helpers
{
    /// <summary>
    /// Exposes some common validation rules. Even though those check can be implemented in-place,
    /// it was deemed useful to keep them in a separate file to let the utilizers focus only on their goal.
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Checks that a double is a finite number.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="parameterName">Parameter name for presentation purposes.</param>
        /// <exception cref="ArgumentException">If the parameter is NaN or Infinity.</exception>
        internal static void CheckDouble(double value, string parameterName)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentException($"The {parameterName} must be a finite value.", parameterName);
            }
        }

        /// <summary>
        /// Checks that a frequency is normalized to the Nyquist rate, meaning that must be in [0;1] range.
        /// </summary>
        /// <param name="frequency">Frequency to check.</param>
        /// <param name="parameterName">Parameter name for presentation purposes.</param>
        /// <exception cref="ArgumentException">If the parameter is not in [0;1] range.</exception>
        /// <seealso cref="CheckDouble(double, string)"/>
        internal static void CheckFrequency(double frequency, string parameterName)
        {
            CheckDouble(frequency, parameterName);
            if (frequency < 0 || frequency > 1)
            {
                throw new ArgumentException($"The {parameterName} frequency must be normalized to Nyquist rate.", parameterName);
            }
        }

        /// <summary>
        /// Explicitly checks that an object is not null
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <param name="parameterName">Parameter name for presentation purposes.</param>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        internal static void CheckNull(object obj, string parameterName)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(parameterName, $"{parameterName} cannot be null.");
            }
        }
    }
}
