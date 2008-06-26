#region Math.NET Iridium (LGPL) by Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2008, Joannes Vermorel, http://www.vermorel.com
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Statistics
{
    /// <summary>
    /// The <c>Accumulator</c> provides online algorithms to computes the first
    /// statistical moments and their derivatives.
    /// </summary>
    /// <remarks>
    /// <p>The <c>Accumulator</c> provides memory efficient online algorithms
    /// to compute the first statistical moments (mean, variance) and their
    /// derivatives (sigma, error estimate).</p>
    /// <p>The memory required by the accumulator is <c>O(1)</c> independent
    /// from the distribution size. All methods are executed in a <c>O(1)</c>
    /// computational time.
    /// </p>
    /// <p>The <c>Accumulator</c> is not thread safe.</p>
    /// </remarks>
    public class Accumulator
    {
        /* Design note (joannes):
         * The Min/Max have not been included on purpose. It usually clearer
         * (because being trivial) to manage explicitly in the client the Min/Max 
         * than using a library to do so.
         * 
         * The skewness and kurtosis have not been included because I never heard of
         * anyone using those indicator in practice.
         * */

        /// <summary>
        /// Sum of the values added to the accumulator.
        /// </summary>
        private double sum;

        /// <summary>
        /// Sum of the square of the values added to the accumulator.
        /// </summary>
        private double squaredSum;

        /// <summary>
        /// Number of values added to the accumulator.
        /// </summary>
        private int count;

        /// <summary>
        /// Creates an empty <c>Accumulator</c>.
        /// </summary>
        public Accumulator()
        {
            this.Clear();
        }

        /// <summary>
        /// Creates an <c>Accumulator</c> that contains the provided values.
        /// </summary>
        public Accumulator(double[] values)
        {
            this.Clear();
            this.AddRange(values);
        }

        /// <summary>
        /// Creates an <c>Accumulator</c> that contains the provided values.
        /// </summary>
        [Obsolete("This method is obsolete, please use the generic version instead: Accumulator(IEnumerable<double>)", false)]
        public Accumulator(ICollection values)
        {
            this.Clear();
            this.AddRange(values);
        }

        /// <summary>
        /// Creates an <c>Accumulator</c> that contains the provided values.
        /// </summary>
        public Accumulator(IEnumerable<double> values)
        {
            this.Clear();
            this.AddRange(values);
        }

        #region Add/Remove Samples
        /// <summary>
        /// Adds a real value to the accumulator.
        /// </summary>
        public void Add(double value)
        {
            sum += value;
            squaredSum += value * value;
            count++;
        }

        /// <summary>
        /// Adds a range of values to the accumulator.
        /// </summary>
        public void AddRange(double[] values)
        {
            for(int i = 0; i < values.Length; i++)
            {
                this.Add(values[i]);
            }
        }

        /// <summary>
        /// Adds a range of values to the accumulator.
        /// </summary>
        [Obsolete("This method is obsolete, please use the generic version instead: AddRange(IEnumerable<double>)", false)]
        public void AddRange(ICollection values)
        {
            foreach(object obj in values)
            {
                if(!(obj is double))
                {
                    throw new ArgumentException(Resources.ArgumentTypeMismatch);
                }

                this.Add((double)obj);
            }
        }

        /// <summary>
        /// Adds a range of values to the accumulator.
        /// </summary>
        public void AddRange(IEnumerable<double> values)
        {
            foreach(double v in values)
            {
                Add(v);
            }
        }

        /// <summary>
        /// Clears (re-initialize) the accumulator.
        /// </summary>
        public void Clear()
        {
            this.sum = 0d;
            this.squaredSum = 0d;
            this.count = 0;
        }

        /// <summary>
        /// Removes a value from the accumulator.
        /// </summary>
        /// <remarks>
        /// <p>Caution: the <c>Accumulator</c> does not explicitly records the
        /// added values. Therefore, no exception will be thrown if an attempt
        /// is made to remove a value that have not been previously added to
        /// the accumulator.</p>
        /// </remarks>
        public void Remove(double value)
        {
            if(count <= 0)
            {
                throw new InvalidOperationException(Resources.InvalidOperationAccumulatorEmpty);
            }

            sum -= value;
            squaredSum -= value * value;
            count--;
        }

        /// <summary>
        /// Removes a range of values from the accumulator.
        /// </summary>
        public void RemoveRange(double[] values)
        {
            for(int i = 0; i < values.Length; i++)
            {
                this.Remove(values[i]);
            }
        }

        /// <summary>
        /// Removes a range of values from the accumulator.
        /// </summary>
        [Obsolete("This method is obsolete, please use the generic version instead: RemoveRange(IEnumerable<double>)", false)]
        public void RemoveRange(ICollection values)
        {
            foreach(object obj in values)
            {
                if(!(obj is double))
                {
                    throw new ArgumentException(Resources.ArgumentTypeMismatch);
                }

                this.Remove((double)obj);
            }
        }

        /// <summary>
        /// Removes a range of values from the accumulator.
        /// </summary>
        public void RemoveRange(IEnumerable<double> values)
        {
            foreach(double v in values)
            {
                Remove(v);
            }
        }
        #endregion

        /// <summary>
        /// Gets the number of values added to the accumulator.
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets the sum of the values added to the accumulator.
        /// </summary>
        public double Sum
        {
            get { return sum; }
        }

        /// <summary>
        /// Gets the sum of the squared values added to the accumulator.
        /// </summary>
        public double SquaredSum
        {
            get { return squaredSum; }
        }

        /// <summary>
        /// Gets the arithmetic mean of the values added to the accumulator.
        /// </summary>
        public double Mean
        {
            get
            {
                if(count <= 0)
                {
                    throw new InvalidOperationException(Resources.InvalidOperationAccumulatorEmpty);
                }

                return (sum / count);
            }
        }

        /// <summary>
        /// Gets the arithmetic mean of the squared values added to the accumulator. Note that this is not equal to the squared mean of the values.
        /// </summary>
        public double MeanSquared
        {
            get
            {
                if(count <= 0)
                {
                    throw new InvalidOperationException(Resources.InvalidOperationAccumulatorEmpty);
                }

                return (squaredSum / count);
            }
        }

        /// <summary>
        /// Gets the variance of the values added to the accumulator.
        /// </summary>
        public double Variance
        {
            get
            {
                if(count <= 0)
                {
                    throw new InvalidOperationException(Resources.InvalidOperationAccumulatorEmpty);
                }

                double mean = this.Mean;
                return (squaredSum - mean * mean * count) / (count - 1);
            }
        }

        /// <summary>
        /// Gets the standard deviation of the values added to the accumulator.
        /// </summary>
        public double Sigma
        {
            get
            {
                return Math.Sqrt(this.Variance);
            }
        }

        /// <summary>
        /// Gets the mean error estimate defined as the square root of the ratio of 
        /// the standard deviation to the number of values added to the accumulator.
        /// </summary>
        public double ErrorEstimate
        {
            get
            {
                if(count <= 0)
                {
                    throw new InvalidOperationException(Resources.InvalidOperationAccumulatorEmpty);
                }

                return Sigma / Math.Sqrt(count);
            }
        }
    }
}
