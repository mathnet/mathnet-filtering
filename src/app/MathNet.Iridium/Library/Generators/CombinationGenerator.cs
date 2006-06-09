using System;
using System.Collections;

namespace MathNet.Numerics.Generators
{
	/// <summary>
	/// The class <c>CombinationGenerator</c> used to generates combination.
	/// </summary>
	/// <remarks>
	/// <p>See the <a href="http://en.wikipedia.org/wiki/Combination">WikiPedia</a>
	/// for more information about the combinatorics properties of combinations.</p>
	/// </remarks>
	public class CombinationGenerator
	{
		private CombinationGenerator() {}

		/// <summary>
		/// Gets an array of <c>k</c> indices picked uniformly drawn from the
		/// interval <c>[0, n-1]</c>.
		/// </summary>
		public static int[] Next(int n, int k)
		{
			if(n < 0 || k < 0) throw new ArgumentException(
				"#E00 n and k must be non negative integer.");

			if(k > n) throw new ArgumentException(
				"#E01 n must be greater or equal to k.");

			/* Current implementation relies of the permutation generator and
			 * therefore requires a O(n*log(n)) computation time to run. 
			 * 
			 * This complexity could be significantly improved, especially for
			 * small k. */

            int[] permutation = PermutationGenerator.Next(n);
			int[] combination = new int[k];
			for(int i = 0; i < k; i++)
				combination[i] = permutation[i];

			return combination;
		}

		/// <summary>
		/// Gets an array of <c>k</c> indices picked from the interval <c>[0, weights.Length-1]</c> 
		/// with probabilities proportionnal to the <c>weights</c>.
		/// </summary>
		public static int[] Next(double[] weights, int k)
		{
			if(k < 0) throw new ArgumentException(
				"#E00 k must be non negative.");

			if(k > weights.Length) throw new ArgumentException(
				"#E01 k must smaller or equal to the collection size.");

			// TODO: implement the method

			throw new NotImplementedException();
		}
	}
}
