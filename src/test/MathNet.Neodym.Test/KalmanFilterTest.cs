#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2008, Matthew Kitchin
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
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra;
using MathNet.SignalProcessing.Filter.Kalman;
using MathNet.Numerics.Distributions;
using Neodym.Test.Properties;

namespace Neodym.Test
{
	[TestFixture]
	public class KalmanFilterTest
	{
		// Array of measured ranges from cartesian center for a track (noisy - 0.5 units)
		double[] rM = {99.9901, 90.2995, 80.0453, 71.5741, 61.7019, 54.0062, 44.4181, 37.3228,
			30.8536, 26.2474, 25.0199, 29.1189, 34.3914, 42.5675, 50.7917, 60.1946 };
		// Array of measured bearings from cartesian center for a track (noisy - 2deg)
		double[] bM = { 0.7848, 0.7551, 0.7213, 0.6654, 0.6565, 0.4747, 0.4465, 0.3066,
			0.0720, -0.2540, -0.6531, -0.9779, -1.2390, -1.4442, -1.5699, -1.6535 };

		double re = 0.5;
		double the = 2d * Math.PI / 180d;
		double T = 10d;
		double q = 0.01;
		
		public static readonly double DefaultTolerance = 1e-8;
		
		[Test]
		public void TestDiscreteKalmanFilter()
		{
			// Read constants from resources
			double r = Double.Parse(KalmanTestResources.r);
			double T = Double.Parse(KalmanTestResources.T);
			// Read the measurements from resources
			Matrix[] zks = GetMatrices(KalmanTestResources.zks,1);
			// Generate the initial state matrix and covariance from two point differencing
			Matrix x0 = new Matrix(2,1);
			double z0 = zks[0][0,0]; double z1 = zks[1][0,0];
			x0[0,0] = z1;
			x0[1,0] = (z1 - z0)/T;
			Matrix P0 = new Matrix(2,2);
			P0[0,0] = r; P0[1,0] = r/T; P0[0,1] = r/T; P0[1,1] = 2 * r / (T * T);
			// Read all the matrices we test filter performance against from resources
			Matrix[] xhp = GetMatrices(KalmanTestResources.xhp,1); // state predicts
			Matrix[] xhu = GetMatrices(KalmanTestResources.xhu,1); // state updates
			Matrix[] Php = GetMatrices(KalmanTestResources.Php,2); // Covariance after predicts
			Matrix[] Phu = GetMatrices(KalmanTestResources.Phu,2); // Covariance after updates
			// Setup a DiscreteKalmanFilter to filter
			DiscreteKalmanFilter dkf = new DiscreteKalmanFilter(x0, P0);
			double[] aF = { 1d, 0d, T, 1 };
			double[] aG = { (T * T) / 2d, T };
			Matrix F = new Matrix(aF,2);   // State transition matrix
			Matrix G = new Matrix(aG,2);   // Plant noise matrix
			Matrix Q = new Matrix(1,1); Q[0,0] = Double.Parse(KalmanTestResources.Q); // Plant noise variance
			Matrix R = new Matrix(1,1); R[0,0] = r;  // Measurement variance matrix
			Matrix H = new Matrix(1,2); H[0,0] = 1d; H[0,1] = 0d;  // Measurement matrix
			
			// Test the performance of this filter against the stored data from a proven
			// Kalman Filter of a one dimenional tracker.
			for (int i = 2; i < zks.Length; i++)
			{
				// Perform the prediction
				dkf.Predict(F, G, Q);
				// Test against the prediction state/covariance
				Assert.IsTrue(Matrix.AlmostEqual(dkf.State, xhp[i-2], DefaultTolerance),"State Prediction (" + i + ")");
				Assert.IsTrue(Matrix.AlmostEqual(dkf.Cov, Php[i-2], DefaultTolerance), "Covariance Prediction (" + i + ")");
				// Perform the update
				dkf.Update(zks[i], H, R);
				// Test against the update state/covariance
				Assert.IsTrue(Matrix.AlmostEqual(dkf.State, xhu[i-2], DefaultTolerance),"State Update (" + i + ")");
				Assert.IsTrue(Matrix.AlmostEqual(dkf.Cov, Phu[i-2], DefaultTolerance), "Covariance Update (" + i + ")");
			}
		}
		
		[Test]
		public void TestInformationFilter()
		{
			System.Console.WriteLine("Filter 1 - DiscreteKalmanFilter, Filter 2 - InformationFilter");
			Matrix x0 = RangeBearingTracker.TwoPointDifferenceState(rM[0], rM[1], bM[0], bM[1], T);
			Matrix P0 = RangeBearingTracker.TwoPointDifferenceCovariance(rM[0], rM[1], bM[0], bM[1], re, the, T);
			DiscreteKalmanFilter dkf = new DiscreteKalmanFilter(x0, P0);
			InformationFilter inf = new InformationFilter(x0, P0);
			Assert.IsTrue(RunTest(dkf, inf, DefaultTolerance));
		}
		
		[Test]
		public void TestSquareRootFilter()
		{
			System.Console.WriteLine("Filter 1 - DiscreteKalmanFilter, Filter 2 - SquareRootFilter");
			Matrix x0 = RangeBearingTracker.TwoPointDifferenceState(rM[0], rM[1], bM[0], bM[1], T);
			Matrix P0 = RangeBearingTracker.TwoPointDifferenceCovariance(rM[0], rM[1], bM[0], bM[1], re, the, T);
			DiscreteKalmanFilter dkf = new DiscreteKalmanFilter(x0, P0);
			SquareRootFilter sqrf = new SquareRootFilter(x0, P0);
			Assert.IsTrue(RunTest(dkf, sqrf, DefaultTolerance));
		}
		
		
		private bool RunTest(IKalmanFilter filter1, IKalmanFilter filter2, double tolerance)
		{
			List<double> xf1 = new List<double>();
			List<double> yf1 = new List<double>();
			List<double> xf2 = new List<double>();
			List<double> yf2 = new List<double>();
			Matrix ZeroCov = new Matrix(filter1.Cov.RowCount, filter1.Cov.RowCount);
			Matrix ZeroState = new Matrix(filter1.State.RowCount,1);
			
			RangeBearingTracker rbt1 = new RangeBearingTracker(filter1);
			RangeBearingTracker rbt2 = new RangeBearingTracker(filter2);
			bool withinTolerance = true;
			
			// Predict the filters
			rbt1.Predict(this.T, this.q);
			rbt2.Predict(this.T, this.q);
			
			for (int i = 2; i < this.bM.Length; i++)
			{
				rbt1.Update(this.rM[i], this.bM[i], this.re, this.the);
				rbt2.Update(this.rM[i], this.bM[i], this.re, this.the);
				
				xf1.Add(rbt1.State[0,0]);
				yf1.Add(rbt1.State[2,0]);
				xf2.Add(rbt2.State[0,0]);
				yf2.Add(rbt2.State[2,0]);
				
				if (!Matrix.AlmostEqual(ZeroCov, (rbt2.Cov - rbt1.Cov), tolerance))
					withinTolerance = false;
				else if (!Matrix.AlmostEqual(ZeroState, (rbt2.State - rbt1.State), tolerance))
					withinTolerance = false;
				
				rbt1.Predict(this.T, this.q);
				rbt2.Predict(this.T, this.q);
			}
			
			// Create strings
			string x1s = ""; string y1s = ""; string x2s = ""; string y2s = "";
			for (int i=0; i < xf1.Count; i++)
			{
				x1s += xf1[i].ToString("#00.00") + "\t";
				y1s += yf1[i].ToString("#00.00") + "\t";
				x2s += xf2[i].ToString("#00.00") + "\t";
				y2s += yf2[i].ToString("#00.00") + "\t";
			}
			
			System.Console.WriteLine("Filter 1 Estimates");
			System.Console.WriteLine(x1s);
			System.Console.WriteLine(y1s);
			System.Console.WriteLine("Filter 2 Estimates");
			System.Console.WriteLine(x2s);
			System.Console.WriteLine(y2s);
			
			
			return withinTolerance;
		}
		
		private static Matrix[] GetMatrices(byte[] input, int cols)
		{
			// Create a test reader for the given byte array
			MemoryStream stream = new MemoryStream(input);
			StreamReader reader = new StreamReader(stream);
			
			// Each item in the dblLines should be a line of doubles read from the bytes
			List<double[]> dblLines = new List<double[]>();
			while (!reader.EndOfStream)
			{
				// 
				List<double> dblVals = new List<double>();
				string thisLine = reader.ReadLine();
				string[] allVals = thisLine.Split("\t".ToCharArray());
				for (int i=0; i<allVals.Length; i++)
				{
					double tmp = Double.Parse(allVals[i]);
					dblVals.Add(tmp);
				}
				dblLines.Add(dblVals.ToArray());
			}
			
			double[][] arr = dblLines.ToArray();
			Matrix bigMat = new Matrix(arr);
			int num_matrices = dblLines[0].Length / cols;
			Matrix[] outMatrices = new Matrix[num_matrices];
			
			for (int i=0; i<num_matrices; i++)
			{
				outMatrices[i] = bigMat.GetMatrix(0,dblLines.Count-1,i*cols,(i*cols)+cols-1);
				
			}
			
			return outMatrices;
		}
		
	}
	
	
	internal class OneDimensionTracker
	{
		public IKalmanFilter Filter
		{
			get { return this.filter; }
		}
		
		IKalmanFilter filter;
		
		private Matrix F;
		
		private Matrix Q;
		
		private Matrix G;
		
		private Matrix H;
		
		private Matrix R;
		
		public OneDimensionTracker(IKalmanFilter filter)
		{
			this.filter = filter;
			this.F = new Matrix(2,2);
			this.F[0,0] = 1d;
			this.F[1,0] = 0d;
			this.F[0,1] = -1d;
			this.F[1,1] = 1d;
			this.Q = new Matrix(1,1);
			this.G = new Matrix(2,1);
			this.H = new Matrix(1,2);
			this.H[0,0] = 1d;
			this.H[0,1] = 0d;
			this.R = new Matrix(1,1);
		}
		
		public void Predict(double dT, double q)
		{
			this.F[0,1] = dT;
			this.G[0,0] = 0.5 * (dT * dT);
			this.G[1,0] = dT;
			this.Q[0,0] = q;
			
			filter.Predict(this.F, this.G, this.Q);
		}
		
		public void Update(double x, double r)
		{
			Matrix z = new Matrix(1,1);
			z[0,0] = x;
			this.R[0,0] = r;
			
			filter.Update(z, this.H, this.R);
		}
		
		
	}
	
	
	internal class RangeBearingTracker
	{
		private IKalmanFilter kf;
		
		public Matrix State
		{
			get { return kf.State; }
		}
		
		public Matrix Cov
		{
			get { return kf.Cov; }
		}
		
		public RangeBearingTracker(IKalmanFilter kf)
		{
			this.kf = kf;
		}
		
		public void Predict(double T, double q)
		{
			Matrix F = GenerateTransition(T);
			Matrix G = GenerateNoiseCoupling(T);
			Matrix Q = new Matrix(1,1);
			Q[0,0] = q;
			
			//System.Console.WriteLine("GQG' " + G * Q * Matrix.Transpose(G));
			
			//kf.Predict(F);
			//kf.Predict(F, G * Q * Matrix.Transpose(G));
			kf.Predict(F, G, Q);
		}
		
		public void Update(double range, double bearing, double range_error, double bearing_error)
		{
			Matrix Z = new Matrix(2,1);
			Z[0,0] = range * Math.Cos(bearing);
			Z[1,0] = range * Math.Sin(bearing);
			
			Matrix H = new Matrix(2,4);
			H[0,0] = 1.0d;
			H[1,2] = 1.0d;
			
			Matrix R = GenerateCartesianCovariance(range, bearing, range_error, bearing_error);
			this.kf.Update(Z, H, R);
		}
		
		public static Matrix TwoPointDifferenceCovariance(double r1, double r2, double th1, double th2, double rs, double thetas, double T)
		{
			Matrix R1 = GenerateCartesianCovariance(r1, th1, rs, thetas);
			Matrix R2 = GenerateCartesianCovariance(r2, th2, rs, thetas);
			double x1 = r1 * Math.Cos(th1);
			double y1 = r1 * Math.Sin(th1);
			double x2 = r2 * Math.Cos(th2);
			double y2 = r2 * Math.Sin(th2);
			double rx = R2[0,0];
			double ry = R2[1,1];
			double rxy = R2[1,0];
			
			Matrix xc = new Matrix(2,2);
			xc[0,0] = rx; xc[0,1] = rx / T; xc[1,0] = rx / T; xc[1,1] = (2 * rx) / (T * T);
			Matrix yc = new Matrix(2,2);
			yc[0,0] = ry; yc[0,1] = ry / T; yc[1,0] = ry / T; yc[1,1] = (2 * ry) / (T * T);
			Matrix xyc = new Matrix(2,2);
			xyc[0,0] = rxy; xyc[0,1] = rxy / T; xyc[1,0] = rxy / T; xyc[1,1] = (2 * rxy) / (T * T);
			
			Matrix P = new Matrix(4,4);
			P.SetMatrix(0,1,0,1,xc);
			P.SetMatrix(2,3,2,3,yc);
			P.SetMatrix(0,1,2,3,xyc);
			P.SetMatrix(2,3,0,1,xyc);
			return P;

		}
		
		public static Matrix TwoPointDifferenceState(double r1, double r2, double th1, double th2, double T)
		{
			double x1 = r1 * Math.Cos(th1);
			double y1 = r1 * Math.Sin(th1);
			double x2 = r2 * Math.Cos(th2);
			double y2 = r2 * Math.Sin(th2);
			
			double[] x0 = new double[4];
			x0[0] = x2;
			x0[1] = (x2 - x1) / T;
			x0[2] = y2;
			x0[3] = (y2 - y1) / T;
			
			return new Matrix(x0,4);
		}
		
		public static Matrix GenerateCartesianCovariance(double r, double theta, double rs, double thetas)
		{
			double[][] R = Matrix.CreateMatrixData(2,2);
			
			double sinSqth = Math.Sin(theta) * Math.Sin(theta);
			double cosSqth = Math.Cos(theta) * Math.Cos(theta);
			
			R[0][0] = ((r * r) * thetas * sinSqth) + (rs * cosSqth);
			R[1][1] = ((r * r) * thetas * cosSqth) + (rs * sinSqth);
			R[0][1] = (rs - (r * r * thetas)) * Math.Sin(theta) * Math.Cos(theta);
			R[1][0] = R[0][1];
			
			return new Matrix(R);
		}
		
		private static Matrix GenerateTransition(double T)
		{
			Matrix Fp = new Matrix(4,4);
			Fp[0,0] = 1; Fp[0,1] = T; Fp[1,1] = 1;
			Fp[2,2] = 1; Fp[2,3] = T; Fp[3,3] = 1;
			return Fp;
		}
		
		private static Matrix GenerateNoiseCoupling(double T)
		{
			Matrix G = new Matrix(4,1);
			G[0,0] = T * T / 2d;
			G[1,0] = T;
			G[2,0] = T * T/ 2d;
			G[3,0] = T;
			
			return G;
		}
	}
}
