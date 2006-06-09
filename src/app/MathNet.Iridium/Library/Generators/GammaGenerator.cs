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
	// TODO: NUnit tests for GammaGenerator

	/// <summary>
	/// Pseudo-random generator of gamma distributed deviates.
	/// </summary>
	/// <remarks>
	/// <p>The returned deviate of <see cref="Next"/> could be interpreted as the
	/// waiting time for the <see cref="Order"/>th event in a Poisson process of unit mean
	/// (see the <a href="http://en.wikipedia.org/wiki/Poisson_distribution">WikiPedia</a> 
	/// for details about Poisson distribution).</p>
	/// <p>See <a href="http://www.library.cornell.edu/nr/">Numerical recipees in C</a> 
	/// (chapter 7) for the detail of the algorithm.</p>
	/// </remarks>
	public class GammaGenerator : IRealGenerator
	{
		Random random;

		private int order;

		/// <summary>
		/// Gamma pseudo-random generator of order equal to <c>order</c>.
		/// </summary>
		public GammaGenerator(int order)
		{
			if(order < 1) throw new ArgumentOutOfRangeException( "order", order,
				"The integer order of the Gamma distribution should be positive.");

			this.order = order;
            random = new Random();
		}

        public GammaGenerator(int order, Random random)
        {
            if (order < 1) throw new ArgumentOutOfRangeException("order", order,
                 "The integer order of the Gamma distribution should be positive.");

            this.order = order;
            this.random = random;
        }

		/// <summary>Gets or sets the order of the gamma distribution.</summary>
		public int Order
		{
			get { return order; }
			set { order = value; }
		}

		/// <summary>
		/// Returns the next pseudo random Gamma distributed deviate.
		/// </summary>
		public double Next()
		{
			// Using direct method, adding waiting times
			if(order < 6)
			{
				double x = 1.0;
				for(int j = 0; j < order; j++)
				{
					double nonZero = 0d;
					while(nonZero == 0d) nonZero = random.NextDouble();

					x *= nonZero;
				}
				return - Math.Log(x);
			}
				// Use rejection method
			else
			{
				double x = 0.0, y, s, e;
				do 
				{
					do 
					{
						y = Math.Tan(Math.PI * random.NextDouble());
						s = Math.Sqrt(2.0*(order - 1) + 1.0);
						x = s * y + (order - 1);
					} while(x <= 0.0);

					e  = (1.0 + y * y) * Math.Exp((order - 1) * Math.Log(x / (order - 1)) - s * y);
				} while(random.NextDouble() > e);

				return x;
			}
		}
	}
}
