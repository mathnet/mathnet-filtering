using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace MathNet.Filtering.Kalman.UnitTests
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
			// Test constants
			double r = 30.0;  // Measurement covariance
			double T = 20.0;  // Time interval between measurements
			double q = 0.1;   // Plant noise constant
			double tol = 0.0001;  // Accuracy tolerance

			// Reference values to test against (generated from a known filter)
			// Reference Measurements
			double[] zs = { 290.16851039,654.55633793,968.97141280,1325.09197161,1636.35947675,1974.39053148,2260.80770553,2574.36119750,2901.32285462,3259.14709098};
			// Expected position estimates (predictions)
			double[] posp = {1018.94416547,1237.00029618,1754.97092716,1855.62596430,2400.27521403,2446.47067625,2978.94381631,3173.63724675};
			// Expected velocity estimates (predictions)
			double[] velp = { 18.21939138,13.38351136,21.52280841,10.92729947,21.32868461,9.24370334,20.26482836,13.59419761 };
			// Expected position estimates (after measurement update)
			double[] posu = { 969.33006892,1324.51475894,1637.07997492,1973.70152187,2261.59660945,2573.64724909,2901.75329465,3258.67447647 };
			// Expected velocity estimates (after measurement update)
			double[] velu = { 13.38351136,21.52280841,10.92729947,21.32868461,9.24370334,20.26482836,13.59419761,20.93270702 };

			// Initial estimate based on two point differencing
			double z0 = zs[0];
			double z1 = zs[1];
		    Matrix<double> x0 = Matrix<double>.Build.Dense(2, 1, new[] { z1, (z1 - z0)/T });
		    Matrix<double> P0 = Matrix<double>.Build.Dense(2, 2, new[] { r, r/T, r/T, 2*r/(T*T) });
			// Setup a DiscreteKalmanFilter to filter
			DiscreteKalmanFilter dkf = new DiscreteKalmanFilter(x0, P0);
            Matrix<double> F = Matrix<double>.Build.Dense(2, 2, new[] { 1d, 0d, T, 1 });   // State transition matrix
            Matrix<double> G = Matrix<double>.Build.Dense(2, 1, new[] { (T * T) / 2d, T });   // Plant noise matrix
            Matrix<double> Q = Matrix<double>.Build.Dense(1, 1, new[] { q }); // Plant noise variance
            Matrix<double> R = Matrix<double>.Build.Dense(1, 1, new[] { r }); // Measurement variance matrix
            Matrix<double> H = Matrix<double>.Build.Dense(1, 2, new[] { 1d, 0d }); // Measurement matrix

			// Test the performance of this filter against the stored data from a proven
			// Kalman Filter of a one dimenional tracker.
			for (int i = 2; i < zs.Length; i++)
			{
				// Perform the prediction
				dkf.Predict(F, G, Q);
				// Test against the prediction state/covariance
				Assert.IsTrue(dkf.State[0,0].AlmostEqual(posp[i-2], tol), "State Prediction (" + i + ")");
                Assert.IsTrue(dkf.State[1, 0].AlmostEqual(velp[i-2], tol), "Covariance Prediction (" + i + ")");
				// Perform the update
			    Matrix<double> z = Matrix<double>.Build.Dense(1, 1, new[] { zs[i] });
				dkf.Update(z, H, R);
				// Test against the update state/covariance
				// Test against the prediction state/covariance
                Assert.IsTrue(dkf.State[0, 0].AlmostEqual(posu[i-2], tol), "State Prediction (" + i + ")");
                Assert.IsTrue(dkf.State[1, 0].AlmostEqual(velu[i-2], tol), "Covariance Prediction (" + i + ")");
			}
		}

		[Test]
		public void TestInformationFilter()
		{
			System.Console.WriteLine("Filter 1 - DiscreteKalmanFilter, Filter 2 - InformationFilter");
			Matrix<double> x0 = RangeBearingTracker.TwoPointDifferenceState(rM[0], rM[1], bM[0], bM[1], T);
            Matrix<double> P0 = RangeBearingTracker.TwoPointDifferenceCovariance(rM[0], rM[1], bM[0], bM[1], re, the, T);
			DiscreteKalmanFilter dkf = new DiscreteKalmanFilter(x0, P0);
			InformationFilter inf = new InformationFilter(x0, P0);
			Assert.IsTrue(RunTest(dkf, inf, DefaultTolerance));
		}

		[Test]
		public void TestSquareRootFilter()
		{
			System.Console.WriteLine("Filter 1 - DiscreteKalmanFilter, Filter 2 - SquareRootFilter");
            Matrix<double> x0 = RangeBearingTracker.TwoPointDifferenceState(rM[0], rM[1], bM[0], bM[1], T);
            Matrix<double> P0 = RangeBearingTracker.TwoPointDifferenceCovariance(rM[0], rM[1], bM[0], bM[1], re, the, T);
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
            Matrix<double> ZeroCov = Matrix<double>.Build.Dense(filter1.Cov.RowCount, filter1.Cov.RowCount);
            Matrix<double> ZeroState = Matrix<double>.Build.Dense(filter1.State.RowCount, 1);

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

				if (!ZeroCov.AlmostEqual((rbt2.Cov - rbt1.Cov), tolerance))
					withinTolerance = false;
                else if (!ZeroState.AlmostEqual((rbt2.State - rbt1.State), tolerance))
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

		private static Matrix<double>[] GetMatrices(byte[] input, int cols)
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
					double tmp = Double.Parse(allVals[i], System.Globalization.NumberFormatInfo.InvariantInfo);
					dblVals.Add(tmp);
				}
				dblLines.Add(dblVals.ToArray());
			}

            Matrix<double> bigMat = Matrix<double>.Build.DenseOfRowArrays(dblLines);
			int num_matrices = dblLines[0].Length / cols;
			Matrix<double>[] outMatrices = new Matrix<double>[num_matrices];

			for (int i=0; i<num_matrices; i++)
			{
			    outMatrices[i] = bigMat.SubMatrix(0, dblLines.Count, i*cols, cols);

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

        private Matrix<double> F;

        private Matrix<double> Q;

        private Matrix<double> G;

        private Matrix<double> H;

        private Matrix<double> R;

		public OneDimensionTracker(IKalmanFilter filter)
		{
			this.filter = filter;
		    this.F = Matrix<double>.Build.Dense(2, 2, new[] { 1d, 0d, -1d, 1d });
            this.Q = Matrix<double>.Build.Dense(1, 1);
            this.G = Matrix<double>.Build.Dense(2, 1);
		    this.H = Matrix<double>.Build.Dense(1, 2, new[] { 1d, 0d });
            this.R = Matrix<double>.Build.Dense(1, 1);
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
            Matrix<double> z = Matrix<double>.Build.Dense(1, 1, new[] { x });
			this.R[0,0] = r;

			filter.Update(z, this.H, this.R);
		}


	}


	internal class RangeBearingTracker
	{
		private IKalmanFilter kf;

        public Matrix<double> State
		{
			get { return kf.State; }
		}

        public Matrix<double> Cov
		{
			get { return kf.Cov; }
		}

		public RangeBearingTracker(IKalmanFilter kf)
		{
			this.kf = kf;
		}

		public void Predict(double T, double q)
		{
            Matrix<double> F = GenerateTransition(T);
            Matrix<double> G = GenerateNoiseCoupling(T);
		    Matrix<double> Q = Matrix<double>.Build.Dense(1, 1, new[] { q });

			//System.Console.WriteLine("GQG' " + G * Q * Matrix.Transpose(G));

			//kf.Predict(F);
			//kf.Predict(F, G * Q * Matrix.Transpose(G));
			kf.Predict(F, G, Q);
		}

		public void Update(double range, double bearing, double range_error, double bearing_error)
		{
		    Matrix<double> Z = Matrix<double>.Build.Dense(2, 1, new[] { range*Math.Cos(bearing), range*Math.Sin(bearing) });

            Matrix<double> H = Matrix<double>.Build.Dense(2, 4);
			H[0,0] = 1.0d;
			H[1,2] = 1.0d;

            Matrix<double> R = GenerateCartesianCovariance(range, bearing, range_error, bearing_error);
			this.kf.Update(Z, H, R);
		}

		public static Matrix<double> TwoPointDifferenceCovariance(double r1, double r2, double th1, double th2, double rs, double thetas, double T)
		{
            Matrix<double> R1 = GenerateCartesianCovariance(r1, th1, rs, thetas);
            Matrix<double> R2 = GenerateCartesianCovariance(r2, th2, rs, thetas);
			double x1 = r1 * Math.Cos(th1);
			double y1 = r1 * Math.Sin(th1);
			double x2 = r2 * Math.Cos(th2);
			double y2 = r2 * Math.Sin(th2);
			double rx = R2[0,0];
			double ry = R2[1,1];
			double rxy = R2[1,0];

		    Matrix<double> xc = Matrix<double>.Build.Dense(2, 2, new[] { rx, rx/T, rx/T, (2*rx)/(T*T) });
		    Matrix<double> yc = Matrix<double>.Build.Dense(2, 2, new[] { ry, ry/T, ry/T, (2*ry)/(T*T) });
		    Matrix<double> xyc = Matrix<double>.Build.Dense(2, 2, new[] { rxy, rxy/T, rxy/T, (2*rxy)/(T*T) });
		    return Matrix<double>.Build.DenseOfMatrixArray(new[,] { { xc, xyc }, { xyc, yc } });

		}

        public static Matrix<double> TwoPointDifferenceState(double r1, double r2, double th1, double th2, double T)
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

            return Matrix<double>.Build.Dense(4, 1, x0);
		}

        public static Matrix<double> GenerateCartesianCovariance(double r, double theta, double rs, double thetas)
        {

			double sinSqth = Math.Sin(theta) * Math.Sin(theta);
			double cosSqth = Math.Cos(theta) * Math.Cos(theta);

            double[] R = new double[4];
			R[0] = ((r * r) * thetas * sinSqth) + (rs * cosSqth);
			R[3] = ((r * r) * thetas * cosSqth) + (rs * sinSqth);
            R[1] = R[2] = (rs - (r*r*thetas))*Math.Sin(theta)*Math.Cos(theta);

            return Matrix<double>.Build.Dense(2, 2, R);
        }

        private static Matrix<double> GenerateTransition(double T)
		{
			double[,] Fp = new double[4,4];
			Fp[0,0] = 1; Fp[0,1] = T; Fp[1,1] = 1;
			Fp[2,2] = 1; Fp[2,3] = T; Fp[3,3] = 1;

            return Matrix<double>.Build.DenseOfArray(Fp);
		}

        private static Matrix<double> GenerateNoiseCoupling(double T)
        {
            double[] G = new double[4];
			G[0] = T * T / 2d;
			G[1] = T;
			G[2] = T * T/ 2d;
			G[3] = T;

            return Matrix<double>.Build.Dense(4, 1, G);
        }
	}
}
