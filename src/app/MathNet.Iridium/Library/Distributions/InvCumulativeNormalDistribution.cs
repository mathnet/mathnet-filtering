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

namespace MathNet.Numerics.Distributions
{
	// TODO: missing NUnit tests for InvCumulativeNormalDistribution

	/// <summary>
	/// Inverse cumulative normal distribution class.
	/// </summary>
	/// <remarks>
	/// <p>This function returns an approximation of the inverse cumulative
	/// normal distribution function. I.e., given P, it returns
	/// an approximation to the X satisfying P = Pr(Z &lt;= X) where Z is a
	/// random variable from the standard normal distribution.</p>
	/// 
	/// <p>The algorithm uses a minimax approximation by rational functions
	/// and the result has a relative error whose absolute value is less
	/// than 1.15e-9.</p>
	/// 
	/// <p>See the page <see href="http://home.online.no/~pjacklam/notes/invnorm/"/>
	/// for more details.</p>
	/// </remarks>
	public class InvCumulativeNormalDistribution : IRealFunction
	{
		#region Coefficients in rational approximations

		private static double[] a = {-3.969683028665376e+01, 2.209460984245205e+02,
						 -2.759285104469687e+02, 1.383577518672690e+02,
						 -3.066479806614716e+01, 2.506628277459239e+00};

		private static double[] b = {-5.447609879822406e+01, 1.615858368580409e+02,
						 -1.556989798598866e+02, 6.680131188771972e+01, -1.328068155288572e+01};

		private static double[] c = {-7.784894002430293e-03, -3.223964580411365e-01,
						 -2.400758277161838e+00, -2.549732539343734e+00,
						 4.374664141464968e+00, 2.938163982698783e+00};

		private static double[] d = {7.784695709041462e-03, 3.224671290700398e-01,
						 2.445134137142996e+00, 3.754408661907416e+00};

		#endregion

		private double mean;
		private double sigma;

		/// <summary>Inverse cumulative standard normal distribution.</summary>
		/// <remarks>The <b>standard</b> normal distribution has a
		/// mean equal to zero and a standart deviation equal to one.</remarks>
		public InvCumulativeNormalDistribution()
		{
			this.mean = 0d;
			this.sigma = 1d;
		}

		/// <summary>Inverse cumulative normal distribution.</summary>
		/// <remarks>The normal distribution has a mean equal to <c>mean</c> 
		/// and a standart deviation equal to <c>sigma</c>.</remarks>
		public InvCumulativeNormalDistribution(double mean, double sigma)
		{
			this.mean = mean;
			this.sigma = sigma;
		}

		/// <summary>Gets or sets the mean of the normal distribution.</summary>
		public double Mean
		{
			get { return mean; }
			set { mean = value; }
		}

		/// <summary>
		/// Gets or sets the standard deviation of the normal distribution.
		/// </summary>
		public double Sigma
		{
			get { return sigma; }
			set { sigma = value; }
		}

		/// <summary>
		/// Gets the inverse cumulative normal distribution function.
		/// </summary>
		/// <param name="p">A <c>double</c> in <c>[0,1]</c> expected.</param>
		public double ValueOf(double p)
		{
			return mean + sigma * StandardValueOf(p);
		}

		/// <summary>Returns the inverse cumulative <b>standard</b> normal 
		/// distribution for the probability <c>p</c>.</summary>
		private double StandardValueOf(double p)
		{
			if(p < 0.0 || p > 1.0) throw new ArgumentOutOfRangeException(
				"p", p, "The probability must be comprised in [0, 1].");

			// Define break-points.
			double plow = 0.02425;
			double phigh = 1 - plow;

			double q;

			// Rational approximation for lower region:
			if ( p < plow ) 
			{
				q = Math.Sqrt(-2*Math.Log(p));
				return (((((c[0]*q+c[1])*q+c[2])*q+c[3])*q+c[4])*q+c[5]) /
					((((d[0]*q+d[1])*q+d[2])*q+d[3])*q+1);
			}

			// Rational approximation for upper region:
			if ( phigh < p ) 
			{
				q = Math.Sqrt(-2*Math.Log(1-p));
				return -(((((c[0]*q+c[1])*q+c[2])*q+c[3])*q+c[4])*q+c[5]) /
					((((d[0]*q+d[1])*q+d[2])*q+d[3])*q+1);
			}

			// Rational approximation for central region:
			q = p - 0.5;
			double r = q*q;
			return (((((a[0]*r+a[1])*r+a[2])*r+a[3])*r+a[4])*r+a[5])*q /
				(((((b[0]*r+b[1])*r+b[2])*r+b[3])*r+b[4])*r+1);
		}

//        #region NUnit testing suite
//#if DEBUG
//        /// <summary>
//        /// Testing the class <see cref="InvCumulativeNormalDistribution"/>.
//        /// </summary>
//        [TestFixture]
//        public class TestingSuite
//        {
//            /// <summary>
//            /// Testing the inverse cumulative normal distribution on few values.
//            /// </summary>
//            [Test] public void SmallSampleCheck()
//            {
//                InvCumulativeNormalDistribution invCnd = new InvCumulativeNormalDistribution();

//                Assertion.AssertEquals("#A00", 0.0, invCnd.ValueOf(0.5), 1e-6);

//                // values obtained with Maple 9.01
//                Assertion.AssertEquals("#A01", 0.5, invCnd.ValueOf(.6914624613), 1e-6);
//                Assertion.AssertEquals("#A02", -0.5, invCnd.ValueOf(0.3085375387), 1e-6);
//                Assertion.AssertEquals("#A03", 1.5, invCnd.ValueOf(.9331927987), 1e-6);
//                Assertion.AssertEquals("#A04", -1.5, invCnd.ValueOf(0.06680720127), 1e-6);


//                invCnd = new InvCumulativeNormalDistribution(1.0, 2.0);

//                Assertion.AssertEquals("#A05", 1.0, invCnd.ValueOf(0.5), 1e-6);

//                // values obtained with Maple 9.01
//                Assertion.AssertEquals("#A06", 0.5, invCnd.ValueOf(.4012936743), 1e-6);
//                Assertion.AssertEquals("#A07", -0.5, invCnd.ValueOf(.2266273524), 1e-6);
//                Assertion.AssertEquals("#A08", 1.5, invCnd.ValueOf(.5987063257), 1e-6);
//                Assertion.AssertEquals("#A09", -1.5, invCnd.ValueOf(.1056497737), 1e-6);

//            }
//        }
//#endif
//        #endregion
	}
}
