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
using System.Diagnostics;

namespace MathNet.Numerics.Distributions
{

	// TODO: clean the comments of the private methods

	/// <summary>Cumulative normal distribution.</summary>
	/// <remarks>
	/// <p>Returns the value (Maple)
	/// <c>int(1 / (sigma * sqrt(2 * Pi)) * exp(-(x - mu)^2/ (2 * sigma ^ 2)), x = -infinity..t)</c>
	/// for the specified <c>t</c>, <c>mu</c> and <c>sigma</c>.</p>
	/// <p>The <i>CND</i> is related to the <i>erf</i> with the formula
	/// <i>CND(x)=(erf(x/sqrt(2))+1)/2</i>.</p>
	/// <p>For more details about the cumulative normal distribution, look at
	/// the <a href="http://en.wikipedia.org/wiki/Erf">WikiPedia</a>.</p>
	/// <p>See <a href="http://www.library.cornell.edu/nr/">Numerical recipees in C</a> 
	/// (chapter 6) for the detail of the algorithm.</p>
	/// </remarks>
	public class CumulativeNormalDistribution : IRealFunction
	{
		// A small number close to the smallest representable floating point number
		private const double FPMIN = 1e-300;

		//  Lanczos Gamma Function approximation - N (number of coefficients -1)
		private static int lgfN = 6;
		
		//  Lanczos Gamma Function approximation - Coefficients
		private static double[] lgfCoeff = {1.000000000190015, 76.18009172947146, -86.50532032941677, 24.01409824083091, -1.231739572450155, 0.1208650973866179E-2, -0.5395239384953E-5};
		
		//  Lanczos Gamma Function approximation - small gamma
		private static double lgfGamma = 5.0;
		
		//  Maximum number of iterations allowed in Incomplete Gamma Function calculations
		private static int igfiter = 1000;
		
		//  Tolerance used in terminating series in Incomplete Gamma Function calculations
		private static double igfeps = 1e-8;

		private double mean;
		private double sigma;

		/// <summary>
		/// Distribution corresponding to the standard normal distribution
		/// (mean equal to <c>0.0</c> and sigma equal to <c>1.0</c>).
		/// </summary>
		public CumulativeNormalDistribution()
		{
			this.mean = 0.0;
			this.sigma = 1.0;
		}

		/// <summary>
		/// Distribution corresponding to the standard normal distribution
		/// of given <c>mean</c> and <c>sigma</c>.
		/// </summary>
		public CumulativeNormalDistribution(double mean, double sigma)
		{
			this.mean = mean;
			this.sigma = sigma;
		}


		/// <summary>
		/// Gets the cumulative normal distribution function.
		/// </summary>
		public double ValueOf(double x)
		{
			x = (x - mean) / (Math.Sqrt(2) * sigma);

			double erf = 0.0;

			if(x != 0.0)
			{
				if(double.IsPositiveInfinity(x))
				{
					erf = 1.0;
				}
				else if(double.IsNegativeInfinity(x))
				{
					erf = -1.0;
				}
				else
				{
					if(x >= 0.0)
					{
						erf = IncompleteGamma(0.5, x*x);
					}
					else
					{
						erf = - IncompleteGamma(0.5, x*x);
					}
				}
			}

			return (erf + 1.0) / 2.0;
		}

		// Incomplete Gamma Function P(a,x) = integral from zero to x of (exp(-t)t^(a-1))dt
		private static double IncompleteGamma(double a, double x)
		{
			Debug.Assert(a >= 0.0 && x >= 0.0);

			double igf = 0.0;

			if(x < a + 1.0)
			{
				// Series representation
				igf = IncompleteGammaSer(a, x);
			}
			else
			{
				// Continued fraction representation
				igf = IncompleteGammaFract(a, x);
			}

			return igf;
		}

		// Incomplete Gamma Function P(a,x) = integral from zero to x of (exp(-t)t^(a-1))dt
		// Series representation of the function - valid for x < a + 1
		private static double IncompleteGammaSer(double a, double x)
		{
			Debug.Assert(a >= 0.0 && x >= 0.0);
			Debug.Assert(x < a + 1.0, "#E00 Continued Fraction Representation.");

			int i = 0;
			double igf = 0.0D;
			bool check = true;

			double acopy = a;
			double sum = 1.0 / a;
			double incr = sum;
			double loggamma = LogGamma(a);

			while(check)
			{
				++i;
				++a;
				incr *= x/a;
				sum += incr;
				if(Math.Abs(incr) < Math.Abs(sum) * igfeps)
				{
					igf = sum * Math.Exp(-x + acopy * Math.Log(x)- loggamma);
					check = false;
				}

				if(i >= igfiter)
				{
					check = false;
					igf = sum * Math.Exp(-x + acopy * Math.Log(x)- loggamma);
				}
			}

			return igf;
		}

		// Incomplete Gamma Function P(a,x) = integral from zero to x of (exp(-t)t^(a-1))dt
		// Continued Fraction representation of the function - valid for x >= a + 1
		// This method follows the general procedure used in Numerical Recipes for C,
		// The Art of Scientific Computing
		// by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
		// Cambridge University Press,   http://www.nr.com/
		private static double IncompleteGammaFract(double a, double x)
		{
			Debug.Assert(a >= 0.0 && x >= 0.0);
			Debug.Assert(x >= a + 1, "#E00 Use Series Representation.");

			int i = 0;
			double ii = 0;
			double igf = 0.0;
			bool check = true;

			double loggamma = LogGamma(a);
			double numer = 0.0;
			double incr = 0.0;
			double denom = x - a + 1.0D;
			double first = 1.0 / denom;
			double term = 1.0 / FPMIN;
			double prod = first;

			while(check)
			{
				++i;
				ii = (double) i;
				numer = -ii*(ii - a);
				denom += 2.0D;
				first = numer*first + denom;
				if(Math.Abs(first) < FPMIN)
				{
					first = FPMIN;
				}
				term = denom + numer/term;
				if(Math.Abs(term) < FPMIN)
				{
					term = FPMIN;
				}
				first = 1.0 / first;
				incr = first*term;
				prod *= incr;
				if(Math.Abs(incr - 1.0) < igfeps) check = false;
				if(i >= igfiter)
				{
					check = false;
				}
			}
			igf = 1.0 - Math.Exp(-x + a * Math.Log(x) - loggamma) * prod;
			return igf;
		}

		private static double LogGamma(double x)
		{
			double xcopy = x;
			double fg = 0.0;
			double first = x + lgfGamma + 0.5;
			double second = lgfCoeff[0];

			if(x >= 0.0)
			{
				if(x >= 1.0 && x - (int) x == 0.0)
				{
					fg = LogFactorial(x) - Math.Log(x);
				}
				else
				{
					first -= (x + 0.5) * Math.Log(first);
					for(int i=1; i <= lgfN; i++) second += lgfCoeff[i] / ++xcopy;
					fg = Math.Log(Math.Sqrt(2.0 * Math.PI)*second / x) - first;
				}
			}
			else
			{
				fg = Math.PI / (Gamma(1.0 - x) * Math.Sin(Math.PI * x));

				if(!double.IsInfinity(fg))
				{
					if(fg < 0)
					{
						throw new ArgumentException("The gamma function is negative.");
					}
					else
					{
						fg = Math.Log(fg);
					}
				}
			}

			return fg;
		}

		// Gamma function
		// Lanczos approximation (6 terms)
		private static double Gamma(double x)
		{
			double xcopy = x;
			double first = x + lgfGamma + 0.5;
			double second = lgfCoeff[0];
			double fg = 0.0D;

			if(x >= 0.0)
			{
				if(x >= 1.0 && x -(int) x == 0.0)
				{
					fg = Factorial(x) / x;
				}
				else
				{
					first = Math.Pow(first, x + 0.5) * Math.Exp(-first);
					for(int i=1; i <= lgfN; i++) second += lgfCoeff[i] / ++xcopy;
					fg = first * Math.Sqrt(2.0 * Math.PI) * second / x;
				}
			}
			else
			{
				fg = - Math.PI / (x * Gamma(-x) * Math.Sin(Math.PI * x));
			}
			return fg;
		}

		// factorial of n
		// Argument is of type double but must be, numerically, an integer
		// factorial returned as double but is, numerically, should be an integer
		// numerical rounding may makes this an approximation after n = 21
		private static double Factorial(double n)
		{
			if(n < 0 || (n-(int)n) != 0)
				throw new ArgumentOutOfRangeException("n must be a positive integer.");
			
			double f = 1.0D;
			int nn = (int) n;
			for(int i = 1; i <= nn; i++) f *= i;
			return f;
		}

		// log to base e of the factorial of n
		// Argument is of type double but must be, numerically, an integer
		// log[e](factorial) returned as double
		// numerical rounding may makes this an approximation
		private static double LogFactorial(double n)
		{
			if(n < 0 || (n-(int)n) != 0)
				throw new ArgumentOutOfRangeException("n must be a positive integer.");
			
			double f = 0.0;
			int nn = (int)n;
			for(int i = 2; i <= nn; i++) f += Math.Log(i);
			return f;
		}

//        #region NUnit testing suite
//#if DEBUG
//        /// <summary>
//        /// Testing the class <see cref="CumulativeNormalDistribution"/>.
//        /// </summary>
//        [TestFixture] public class TestingSuite
//        {
//            /// <summary>
//            /// Testing <see cref="CumulativeNormalDistribution.ValueOf"/> on few values.
//            /// </summary>
//            [Test] public void SmallSampleCheck()
//            {
//                CumulativeNormalDistribution cnd = new CumulativeNormalDistribution();

//                Assertion.AssertEquals("#A00", 0.0, cnd.ValueOf(0), 0.5);
//                Assertion.AssertEquals("#A01", 1.0, cnd.ValueOf(double.PositiveInfinity));
//                Assertion.AssertEquals("#A02", 0.0, cnd.ValueOf(double.NegativeInfinity));

//                // values obtained with Maple 9.01
//                Assertion.AssertEquals("#A03", .6914624613, cnd.ValueOf(0.5), 1e-6);
//                Assertion.AssertEquals("#A04", .3085375387, cnd.ValueOf(-0.5), 1e-6);
//                Assertion.AssertEquals("#A05", .9331927987, cnd.ValueOf(1.5), 1e-6);
//                Assertion.AssertEquals("#A06", .06680720127, cnd.ValueOf(-1.5), 1e-6);

//                cnd = new CumulativeNormalDistribution(1.0, 2.0);
//                Assertion.AssertEquals("#A07", 0.0, cnd.ValueOf(1.0), 0.5);
//                Assertion.AssertEquals("#A08", 1.0, cnd.ValueOf(double.PositiveInfinity));
//                Assertion.AssertEquals("#A09", 0.0, cnd.ValueOf(double.NegativeInfinity));

//                // values obtained with Maple 9.01
//                Assertion.AssertEquals("#A10", .4012936743, cnd.ValueOf(0.5), 1e-6);
//                Assertion.AssertEquals("#A11", .2266273524, cnd.ValueOf(-0.5), 1e-6);
//                Assertion.AssertEquals("#A12", .5987063257, cnd.ValueOf(1.5), 1e-6);
//                Assertion.AssertEquals("#A13", .1056497737, cnd.ValueOf(-1.5), 1e-6);
//            }
//        }
//#endif
//        #endregion
	}
}
