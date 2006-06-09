#region MathNet Numerics, Copyright ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
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

namespace MathNet.Numerics.Generators
{
	/// <summary>
	/// Non-uniform discrete random generator.
	/// </summary>
	/// <remarks>
	/// <p>The class <c>DiscreteGenerator</c> provides integers 
	/// deviates for any arbitrary (finite) distribution.</p>
	/// 
	/// <code>
	/// double[] distribution = {0.25, 0.25, 0.5};
	/// DiscreteGenerator gen = new DiscreteGenerator(distribution);
	/// 
	/// // Pr(x = 0) = 0.25, Pr(x = 1) = 0.25, Pr(x = 2) = 0.5
	/// int x = gen.Next();
	/// 
	/// // Changing the distribution
	/// gen[2] = 0.0;
	/// 
	/// // Pr(y = 0) = 0.5, Pr(y = 1) = 0.5, Pr(x = 2) = 0.0
	/// int y = gen.Next();
	/// </code>
	/// 
	/// <p>The probability <c>Pr(x)</c> for any integer <c>x</c>
	/// is proportional to <c>DiscreteGenerator[x]</c>.</p>
	/// 
	/// <p>See the <a href="http://cgm.cs.mcgill.ca/~luc/chapter_three.pdf">
	/// chapter three</a> of the book <i>Non-uniform variate Generation</i>
	/// from Luc Devroye.</p>
	/// </remarks>
	public class DiscreteGenerator
	{
		// TODO: better implementation for this class
		// TODO: provides the unit testing suite

		Random random;

		double[] weights;

		/// <summary>Uniform distribution for the integers <c>[0, count)</c>.</summary>
		/// <param name="count">Integer deviate set size.</param>
		/// <param name="uniformWeight">Weight assigned to each integer deviate.</param>
		/// <remarks>All weights are initialized to <c>1.0</c>.</remarks>
		public DiscreteGenerator(int count, double uniformWeight)
		{
			weights = new double[count];
			for(int i = 0; i < count; i++) weights[i] = uniformWeight;
            random = new Random();
		}

        public DiscreteGenerator(int count, double uniformWeight, int seed)
        {
            weights = new double[count];
            for (int i = 0; i < count; i++) weights[i] = uniformWeight;
            random = new Random(seed);
        }

		/// <summary>Non-uniform distribution for the integers 
		/// <c>[0, distribution.Length)</c>.</summary>
		/// <param name="weights">Weights associated to each
		/// integer deviate.</param>
		public DiscreteGenerator(double[] weights)
		{
			this.weights = weights;
            random = new Random();
		}

        public DiscreteGenerator(double[] weights, Random random)
        {
            this.weights = weights;
            this.random = random;
        }

		/// <summary>Gets or sets the weights for any integer deviate.</summary>
		/// <remarks>
		/// The complexity of setting an element is <c>O(log(this.Count))</c>.
		/// </remarks>
		public double this [int index]
		{
			get { return weights[index]; }
			set { weights[index] = value; }
		}

		/// <summary>Gets the number of elements in the distribution.</summary>
		public int Count
		{
			get { return weights.Length; }
		}

		/// <summary>Gets the next integer deviate.</summary>
		/// <returns>An integer within <c>[0, Count)</c>.</returns>
		/// <remarks>The complexity of a call to this method is <c>O(1)</c>.</remarks>
		public int Next()
		{
			/* this implementation requires a linear time,
			 * must upgrade to a logarithmic complexity. */

			double[] weightSums = new double[this.Count];
			weightSums[0] = weights[0];
			for(int i = 1; i < this.Count; i++)
				weightSums[i] = weightSums[i - 1] + weights[i];

			double threshold = weightSums[this.Count - 1] * random.NextDouble();

			int index = 0;
			while(weightSums[index] < threshold) index++;
			return index;
		}
	}
}
