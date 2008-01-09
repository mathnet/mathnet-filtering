using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    [TestFixture]
    public class BugRegression
    {
        [Test]
        public void IRID90_CholeskySolve()
        {
            Matrix i = Matrix.Identity(3, 3);

            double[][] pvals1 = { new double[] { 1.0, 1.0, 1.0 }, new double[] { 1.0, 2.0, 3.0 }, new double[] { 1.0, 3.0, 6.0 } };
            Matrix m1 = new Matrix(pvals1);
            CholeskyDecomposition cd1 = new CholeskyDecomposition(m1);

            Matrix inv1a = cd1.Solve(i);
            Matrix test1a = m1 * inv1a;
            NumericAssert.AreAlmostEqual(i, test1a, "1A");
            Matrix inv1b = m1.Inverse();
            NumericAssert.AreAlmostEqual(inv1a, inv1b, "1B");

            double[][] pvals2 = { new double[] { 25, -5, 10 }, new double[] { -5, 17, 10 }, new double[] { 10, 10, 62 } };
            Matrix m2 = new Matrix(pvals2);
            CholeskyDecomposition cd2 = new CholeskyDecomposition(m2);

            Matrix inv2a = cd2.Solve(i);
            Matrix test2a = m2 * inv2a;
            NumericAssert.AreAlmostEqual(i, test2a, "2A");
            Matrix inv2b = m2.Inverse();
            NumericAssert.AreAlmostEqual(inv2a, inv2b, "2B");
        }
    }
}
