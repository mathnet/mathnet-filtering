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
using MathNet.Numerics.RandomSources;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Distributions
{
    // TODO: NUnit test for ZipfGenerator
    /* Note: ZipfGenerator seems to returns value that are really
     * big, must check the validity of the formula in use. */

    // TODO: Integrate into iridium's distribution pattern/approach
    // (e.g. includes providing pmf & cdf, inheriting from superclass etc.)

    /// <summary>
    /// Pseudo-random generator of Zipf distributed deviates.
    /// </summary>
    /// <remarks>
    /// <p>The density of the continuous Zipf distribution is defined
    /// on <c>[1, +infinity)</c>, the density is proportional to
    /// <c>x^s</c> where <c>s &gt; 1</c> is the <i>skew</i>.</p>
    /// 
    /// <p>See the <a href="http://en.wikipedia.org/wiki/Zipfs_law">Wikipedia</a>
    /// for more information about Zipf distribution.</p>
    /// </remarks>
    public sealed class ZipfDistribution
    {
        RandomSource random;

        private double skew;

        /// <summary>
        /// Zipfian generator with a default <c>skew</c> equal to <c>2</c>.
        /// </summary>
        public ZipfDistribution()
        {
            this.skew = 2d;
            random = new SystemRandomSource();
        }

        /// <summary>
        /// Zipfian generator with a default <c>skew</c> equal to <c>2</c>.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        public ZipfDistribution(RandomSource random)
        {
            this.skew = 2d;
            this.random = random;
        }

        /// <summary>
        /// Create a zipf distribution with the provided skew.
        /// </summary>
        public ZipfDistribution(double skew)
        {
            if(skew <= 1d)
            {
                throw new ArgumentOutOfRangeException("skew", skew, string.Format(Resources.ArgumentOutOfRangeGreater, "skew", "1.0"));
            }

            this.skew = skew;
            random = new SystemRandomSource();
        }

        /// <summary>
        /// Create a zipf distribution with the provided skew and seed.
        /// </summary>
        public ZipfDistribution(double skew, int seed)
        {
            if(skew <= 1d)
            {
                throw new ArgumentOutOfRangeException("skew", skew, string.Format(Resources.ArgumentOutOfRangeGreater, "skew", "1.0"));
            }

            this.skew = skew;
            random = new SystemRandomSource(seed);
        }

        /// <summary>
        /// Gets or sets the skew of the zipfian distribution.
        /// </summary>
        public double Skew
        {
            get
            {
                return skew;
            }

            set
            {
                if(value <= 1d)
                {
                    throw new ArgumentOutOfRangeException("value", value, string.Format(Resources.ArgumentOutOfRangeGreater, "skew", "1.0"));
                }

                skew = value;
            }
        }

        /// <summary>
        /// Returns the next zipfian deviate.
        /// </summary>
        public double Next()
        {
            /* A transformation method (similar to the exponential
             * generator) is used here to generate a zipfian deviate. */

            double p = random.NextDouble();
            return Math.Pow(1d - p, 1d / (1d - skew));
        }
    }
}
