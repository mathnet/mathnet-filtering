using System;

using MathNet.Numerics;

namespace MathNet.Numerics.Generators
{
	// TODO: implement PoissonGenerator

	/// <summary>
	/// Pseudo random generator of Poisson distributed deviates.
	/// </summary>
	/// <remarks>
	/// <p>See the <a href="http://en.wikipedia.org/wiki/Poisson_distribution">
	/// WikiPedia</a> for details about the Poisson distribution.</p>
	/// </remarks>
	public class PoissonGenerator : IRealGenerator
	{
		private double lambda;

		/// <summary>
		/// Standard Poisson generator (lambda equal to <c>1.0</c>).
		/// </summary>
		public PoissonGenerator()
		{
			this.lambda = 1.0;	
		}

		/// <summary>
		/// Poisson generator with the provided <c>lambda</c>.
		/// </summary>
		/// <param name="lambda">Positive <c>double</c> value.</param>
		public PoissonGenerator(double lambda)
		{
			if(lambda <= 0.0) throw new ArgumentOutOfRangeException(
				"lambda", lambda, "The lambda parameter must be positive.");

			this.lambda = lambda;
		}

		/// <summary>
		/// Gets or sets the lambda parameter of the Poisson distribution.
		/// </summary>
		/// <remarks>The lambda parameter is interpreted as the number 
		/// of occurences per unit of time. The value of the property
		/// is always positive.</remarks>
		public double Lambda
		{
			get { return lambda; }
			set 
			{
				if(value <= 0.0) throw new ArgumentOutOfRangeException(
					"value", value, "The lambda parameter must be positive.");

				lambda = value; 
			}
		}

		/// <summary>Returns the next pseudo random Poisson distributed deviate.</summary>
		public double Next()
		{
			throw new NotImplementedException();
		}
	}
}
