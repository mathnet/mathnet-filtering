/*
 * Provided by Kevin Whitefoot <kwhitefoot@hotmail.com>,
 * thank you very much!
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using NUnit.Framework;

using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    [TestFixture]
    public class MatrixTest
    {


        [Test]
        public void TestMatrix_Create()
        {
            double[,] a = { { 1, 2 }, { 2, 3 } };
            Matrix ma = Matrix.Create(a);
            double[,] b = { { 1.0, 2.0 }, { 2.0, 3.0 } };
            Matrix mb = Matrix.Create(a);
            Assert.IsTrue(ma.Equals(ma), "Matrices should be equal");
        }

        [Test]
        public void TestMatrix_Add()
        {
            double[,] a = { { 1, 2 }, { 2, 3 } };
            Matrix ma = Matrix.Create(a);
            double[,] b = { { 1.0, 2.0 }, { 3.0, 4.0 } };
            Matrix mb = Matrix.Create(b);
            double[,] r = { { 2.0, 4.0 }, { 5.0, 7.0 } };
            Matrix mr = Matrix.Create(r);
            //Console.WriteLine("a");
            //Console.WriteLine(ma.ToString());
            //Console.WriteLine("b");
            //Console.WriteLine(mb.ToString());
            Matrix sum = ma + mb;
            //Console.WriteLine("sum");
            //Console.WriteLine(sum.ToString());
            //Console.WriteLine("expected sum");
            //Console.WriteLine(mr.ToString());

            Assert.AreEqual(sum.ToString(), mr.ToString(), "Matrices should be equal");
        }


        [Test]
        public void TestMatrix_Multiply()
        {
            double[,] a = { { 1, 2 }, { 3, 5 } };
            Matrix ma = Matrix.Create(a);
            double[,] b = { { 7, 11 }, { 13, 17 } };
            Matrix mb = Matrix.Create(b);
            double[,] r = { { 33, 45 }, { 86, 118 } };
            Matrix mr = Matrix.Create(r);
            //Console.WriteLine("a");
            //Console.WriteLine(ma.ToString());
            //Console.WriteLine("b");
            //Console.WriteLine(mb.ToString());
            Matrix mp = ma * mb;
            //Console.WriteLine("product");
            //Console.WriteLine(mp.ToString());
            //Console.WriteLine("expected product");
            //Console.WriteLine(mr.ToString());

            Assert.AreEqual(mp.ToString(), mr.ToString(), "Matrices should be equal");
        }


        [Test]
        public void TestMatrix_Solve()
        {
            double[,] a = { { 1, 2 }, { 3, 5 } };
            Matrix ma = Matrix.Create(a);
            double[,] b = { { 29.0 }, { 76.0 } };
            Matrix mb = Matrix.Create(b);
            double[,] r = { { 7 }, { 11.0 } };
            Matrix mr = Matrix.Create(r);
            //Console.WriteLine("a");
            //Console.WriteLine(ma.ToString());
            //Console.WriteLine("b");
            //Console.WriteLine(mb.ToString());
            Matrix mx = null;
            MyStopwatch.MethodToTime m = delegate
            {
                mx = ma.Solve(mb);
            };
            Console.Write("Solve Time (ms): ");
            MyStopwatch.Time(m);

            //Console.WriteLine("solution");
            //Console.WriteLine(mx.ToString());
            //Console.WriteLine("expected solution");
            //Console.WriteLine(mr.ToString());

            Assert.AreEqual(mx.ToString(), mr.ToString(), "Matrices should be equal");

            //Check by multiplying a by x
            Matrix mc = ma * mx;
            Assert.AreEqual(mc.ToString(), mb.ToString(), "Matrices should be equal");
        }

        [Test]
        public void TestMatrix_SolveA()
        {
            TestMatrix(
              new double[,] { { 1, 2 }, 
                        { 3, 5 } },
              new double[,] { { 7 }, 
                        { 11.0 } },
              1e-13, false);
        }

        [Test]
        public void TestMatrix_SolveB()
        {
            TestMatrix(
              new double[,] { { 1,  2,  3}, 
                        { 5,  7, 11 },
                        {13, 17, 19}},
              new double[,] { { 23 }, 
                        { 29 }, 
                        { 31 }},
              1e-13, false);
        }


        [Test]
        public void TestMatrix_Solve010()
        {
            TestMatrix_NxN(10, 1e-12, false);
        }

        [Test]
        public void TestMatrix_Solve020()
        {
            TestMatrix_NxN(20, 1e-12, false);
        }
        [Test]
        public void TestMatrix_Solve040()
        {
            TestMatrix_NxN(40, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve060()
        {
            TestMatrix_NxN(60, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve080()
        {
             TestMatrix_NxN(80, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve100()
        {
            TestMatrix_NxN(100, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve110()
        {
            TestMatrix_NxN(110, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve120()
        {
            TestMatrix_NxN(120, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve150()
        {
            TestMatrix_NxN(150, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve200()
        {
            TestMatrix_NxN(200, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve330()
        {
            TestMatrix_NxN(330, 1e-9, false);
        }

        //[Test]
        //public void TestMatrix_Solve1000()
        //{
        //    TestMatrix_NxN(1000, 1e-9, false);
        //}

        private void TestMatrix_NxN(int n, double epsilon, bool show)
        {
            Random r = new Random();
            double[,] a = new double[n, n];
            double[,] x = new double[n, 1];
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(j == 0)
                    {
                        x[i, j] = r.NextDouble();
                    }
                    a[i, j] = r.NextDouble();
                }
            }
            TestMatrix(a, x, epsilon, show);
        }


        //calculate right hand side and then solve for x, show all matrices on console out.
        private void TestMatrix(double[,] a, double[,] x, double epsilon, bool show)
        {
            Matrix ma = Matrix.Create(a);
            Matrix mx = Matrix.Create(x);
            if(show)
            {
                Console.WriteLine("a");
                Console.WriteLine(ma.ToString());
                Console.WriteLine("x");
                Console.WriteLine(mx.ToString());
            }
            Matrix ms = TestMatrix_Solutions(ma, mx, epsilon, show);
            if(show)
            {
                Console.WriteLine("solution");
                Console.WriteLine(ms.ToString());
                Console.WriteLine("expected solution");
                Console.WriteLine(mx.ToString());
            }
        }

        /*Test a given solution by calculating b and then solving for x.
        Shows only the elapsed time on console out so that we can use 
        matrices too large to print.*/
        private Matrix TestMatrix_Solutions(Matrix ma, Matrix mx, double epsilon, bool showB)
        {
            Matrix mb = ma * mx;
            if(showB)
            {
                Console.WriteLine("b");
                Console.WriteLine(mb.ToString());
            }
            Matrix ms = null;
            MyStopwatch.MethodToTime m = delegate
            {
                ms = ma.Solve(mb);
            };
            Console.Write("Solve Time (ms) for " + ma.ColumnCount + ": ");
            MyStopwatch.Time(m);

            Assert.IsTrue(CompareMatrices(ms, mx, epsilon), "Matrices should be equal");
            //Assert.AreEqual(ms.ToString(), mx.ToString(), "Matrices should be equal");

            return ms;
        }

        private bool CompareMatrices(Matrix a, Matrix b, double epsilon)
        {
            Matrix c = a - b;
            for(int i = 0; i < c.RowCount; i++)
            {
                for(int j = 0; j < c.ColumnCount; j++)
                {
                    if(epsilon < Math.Abs(c[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }



    }
}
