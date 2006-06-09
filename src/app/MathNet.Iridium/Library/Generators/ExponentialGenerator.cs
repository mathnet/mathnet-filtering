#region MathNet Numerics, Copyright ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
//
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

using MathNet.Numerics;

namespace MathNet.Numerics.Generators
{
	// TODO: NUnit tests for ExponentialGenerator
	
	/// <summary>
	/// Pseudo-random generator of exponentially distributed deviates.
	/// </summary>
	/// <remarks>
	/// <p>See the <a href="http://en.wikipedia.org/wiki/Exponential_distribution">
	/// WikiPedia</a> for detail about the exponential distribution.</p>
	/// <p>See <a href="http://www.library.cornell.edu/nr/">Numerical recipees in C</a> 
	/// (chapter 7) for the detail of the algorithm.</p>
	/// </remarks>
	public class ExponentialGenerator : IRealGenerator
	{
		Random random;

		double halfPeriod;

		/// <summary>Pseudo-random exponential generator.</summary>
		/// <remark>The half period of the exponential distribution
		/// is set to <c>1</c> by default.</remark>
		public ExponentialGenerator()
		{
			this.halfPeriod = 1d;
            random = new Random();
		}

        public ExponentialGenerator(int seed)
        {
            this.halfPeriod = 1d;
            random = new Random(seed);
        }

		/// <summary>Pseudo-random exponential generator.</summary>
		/// <param name="halfPeriod">Mean of the exponential distribution.</param>
		public ExponentialGenerator(double halfPeriod)
		{
			if(halfPeriod <= 0.0) throw new ArgumentOutOfRangeException("halfPeriod", halfPeriod,
				"The half period of the exponential distribution should be positive.");

			this.halfPeriod = halfPeriod;
            random = new Random();
		}

        public ExponentialGenerator(double halfPeriod, Random random)
        {
            if (halfPeriod <= 0.0) throw new ArgumentOutOfRangeException("halfPeriod", halfPeriod,
                 "The half period of the exponential distribution should be positive.");

            this.halfPeriod = halfPeriod;
            this.random = random;
        }

		/// <summary>Half period of the exponential distribution.</summary>
		public double HalfPeriod
		{
			get { return halfPeriod; }
			set { halfPeriod = value; }
		}

		/// <summary>Returns an exponentially distributed positive random deviate.</summary>
		public double Next()
		{
			double nonZero = 0d;
			while(nonZero == 0d) nonZero = random.NextDouble();

			return - halfPeriod * Math.Log(nonZero);
		}
	}
}
