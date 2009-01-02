//-----------------------------------------------------------------------
// <copyright file="BugRegression.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;

namespace Iridium.Test
{
    [TestFixture]
    public class BugRegression
    {
        [Test]
        public void IRID77_NegativeComplexLogarithm()
        {
            Complex minusOne = -Complex.One;
            Complex piI = minusOne.NaturalLogarithm();

            Assert.AreEqual(0.0, piI.Real, 1e-8, "Re{ln(-1)} = 0");
            Assert.AreEqual(Constants.Pi, piI.Imag, 1e-8, "Im{ln(-1)} = Pi");

            Complex zero = Complex.Zero;
            Complex lnZero = zero.NaturalLogarithm();

            Assert.AreEqual(double.NegativeInfinity, lnZero.Real, "Re{ln(0)} = -infinity");
            Assert.AreEqual(0, lnZero.Imag, "Im{ln(0)} = 0");
        }

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

        [Test]
        public void IRID107_ComplexPowerAtZero()
        {
            Complex zeroPowTwo = Complex.Zero.Power(2);
            Assert.AreEqual(0d, zeroPowTwo.Real, "Re{(0)^(2)} = 0");
            Assert.AreEqual(0d, zeroPowTwo.Imag, "Im{(0)^(2)} = 0");
        }

        [Test]
        [Obsolete("The tested algorithms is obsolete, so is this test.")]
        public void IRID119_PolynomialExtrapolatePositiveDirection()
        {
            double[] x = new double[] { -6.060771484, -5.855378418, -1.794238281, -1.229428711, 0.89935791, 2.912121582, 4.699230957, 4.788347168, 7.728830566, 11.70989502 };
            double[] y = new double[] { 0.959422052, 0.959447861, 0.959958017, 0.960028946, 0.960323274, 0.960636258, 0.960914195, 0.960928023, 0.96138531, 0.962004483 };

            PolynomialInterpolationAlgorithm pia = new PolynomialInterpolationAlgorithm(10);
            SampleList sl = new SampleList(10);

            for(int i = 0; i < 10; i++)
            {
                sl.Add(x[i], y[i]);
            }
           
            pia.Prepare(sl);
            NumericAssert.AreAlmostEqual(0.9622, pia.Extrapolate(12), 1e-3, "extrapolate(12)");
        }

        [Test]
        public void IRID178_ComplexNumbersHashCode()
        {
            Assert.AreNotEqual(Complex.One.GetHashCode(), Complex.I.GetHashCode(), "A");
            Assert.AreNotEqual(Complex.One.GetHashCode(), (-Complex.I).GetHashCode(), "B");
            Assert.AreNotEqual((-Complex.One).GetHashCode(), Complex.I.GetHashCode(), "C");
            Assert.AreNotEqual((-Complex.One).GetHashCode(), (-Complex.I).GetHashCode(), "D");
        }

        [Test]
        public void IRID177_MatrixPseudoInverse()
        {
            Matrix a = new Matrix(new double[][] {
                new double[] { 15, 23, 44, 54 },
                new double[] { 1, 5, 9, 4 },
                new double[] { 8, 11, 4, 2 }
                });

            Matrix aInverse = new Matrix(new double[][] {
                new double[] { 0.00729481932863557, -0.0906433578450537, 0.0629567950756452 },
                new double[] { -0.00695248549232449, 0.0302767536403138, 0.0601374162387492 },
                new double[] { -0.00876996343998189, 0.155054444209528, -0.033311997806593 },
                new double[] { 0.0265993197732062, -0.114057602060568, -0.0159589740025151 }
                });

            NumericAssert.AreAlmostEqual(aInverse, a.Inverse(), "A");
            NumericAssert.AreAlmostEqual(Matrix.Transpose(aInverse), Matrix.Transpose(a).Inverse(), "B");
        }

        [Test]
        public void IRID182_Eigenvalues()
        {
            Matrix m = Matrix.Create(new double[,] {
                {
                    0.885544230294749, -0.580336000562429, 0.400869970588928,
                    0.165954532231597, 0.690977298196212, -0.521887360236719,
                    -0.0814483582258942, -0.939682389503636, -0.210173828668251,
                    0.564932147694539, -1.0154353322131, 0.640685090404004
                },
                {
                    -0.580336000562429, 0.492486112353507, -0.262914069278714,
                    -0.0584647849641528, -0.537766775087812, 0.294726342538363,
                    0.0927869109175177, 0.726403468926861, 0.0862640603521092,
                    -0.489794367061056, 0.744944058771799, -0.508334956905994
                },
                {
                    0.400869970588928, -0.262914069278714, 0.244830980759568,
                    0.11785975727827, 0.32222855875326, -0.204193677592214,
                    -0.214481531745175, -0.442608559845998, 0.026150314424896,
                    0.227433408251588, -0.513686177880137, 0.298511026285728
                },
                {
                    0.165954532231597, -0.0584647849641528, 0.11785975727827,
                    0.140069862075066, 0.0897123706302303, -0.0758472091590544,
                    -0.0932037350339198, -0.165672162129375, -0.00707090905640249,
                    0.026167220128558, -0.193113416527151, 0.0536084745263342
                },
                {
                    0.690977298196212, -0.537766775087812, 0.32222855875326,
                    0.0897123706302303, 0.657562200713567, -0.401936443046788,
                    -0.0234089455262449, -0.862071919853614, -0.216526083877413,
                    0.571650829476108, -0.854054719662501, 0.563633629284995
                },
                {
                    -0.521887360236719, 0.294726342538363, -0.204193677592214,
                    -0.0758472091590544, -0.401936443046788, 0.37498137218359,
                    -0.129585782998913, 0.517649461236531, 0.273871905878737,
                    -0.322714035020032, 0.536190051081469, -0.34125462486497
                },
                {
                    -0.0814483582258942, 0.0927869109175177, -0.214481531745175,
                    -0.0932037350339198, -0.0234089455262449, -0.129585782998913,
                    0.910194610272133, 0.0543719437593689, -0.59175909723612,
                    0.011081021028939, 0.235777494730137, -0.170324529941829
                },
                {
                    -0.939682389503636, 0.726403468926861, -0.442608559845998, 
                    -0.165672162129375, -0.862071919853614, 0.517649461236531, 
                    0.0543719437593689, 1.18519903822716, 0.226643546080826, 
                    -0.742715732562644, 1.20491236837527, -0.762429062710757
                },
                {
                    -0.210173828668251, 0.0862640603521092, 0.026150314424896,
                    -0.00707090905640249, -0.216526083877413, 0.273871905878737, 
                    -0.59175909723612, 0.226643546080826, 0.621099014778964,
                    -0.217571234379085, 0.0386828111635692, -0.0296104994618287,
                },
                {
                    0.564932147694539, -0.489794367061056, 0.227433408251588, 
                    0.026167220128558, 0.571650829476108, -0.322714035020032, 
                    0.011081021028939, -0.742715732562644, -0.217571234379085, 
                    0.557123237502864, -0.694564753107317, 0.508972258047537
                },
                {
                    -1.0154353322131, 0.744944058771799, -0.513686177880137, 
                    -0.193113416527151, -0.854054719662501, 0.536190051081469,
                    0.235777494730137, 1.20491236837527, 0.0386828111635692,
                    -0.694564753107317, 1.347356880385, -0.837009265117046
                },
                {
                    0.640685090404004, -0.508334956905994, 0.298511026285728,
                    0.0536084745263342, 0.563633629284995, -0.34125462486497,
                    -0.170324529941829, -0.762429062710757, -0.0296104994618287,
                    0.508972258047537, -0.837009265117046, 0.583552460453826
                }
            });

            // Expected data evaluated with MATLAB "[V,D] = eig(A)"
            ComplexVector expectedEigenValues = ComplexVector.Create(new double[] {
                0d, 0d, 0d, 0d,
                0.00262233860281324,
                0.0210323773292213,
                0.0355556528747382,
                0.127624731796062,
                0.179718416331653,
                0.310576269081767,
                1.52253860917203,
                5.80033160481171
            });

            Matrix expectedEigenVectors = new Matrix(new double[][] {
                new double[] { 0.106498497282379, 0.419691143613967, -0.0557817580954078, -0.00690851159493115, 0.371136426079307, 0.223461682210106, -0.468606464889867, 0.343527867020022, 0.0756466774996531, -0.369826279577119, -0.0284941597416966, 0.375195260395109 },
                new double[] { 0.106498497282348, 0.419691143613984, -0.0557817580953903, -0.00690851159494274, 0.347453164986349, -0.567931379282469, 0.448676542846663, 0.0426012577167882, -0.00926728212925367, -0.290807685971263, -0.0241538261982648, -0.278790999189846 },
                new double[] { 0.141668469389994, 0.156579178192424, -0.546459905897362, -0.320022417617439, -0.370241497490735, 0.332713332831733, 0.399336647042652, -0.104765030616846, 0.0739529635375817, -0.283780350824145, 0.138326048745256, 0.176134628893008 },
                new double[] { 0.456535390675942, -0.214226432376964, 0.109822773758153, 0.133529424410943, -0.191583966179205, -0.266148198349251, -0.313325326798921, -0.506526681171299, 0.0272792573144116, -0.495783211258065, 0.0584677852966494, 0.059551430007216 },
                new double[] { 0.364647620387872, 0.0977045432532739, 0.436554618830564, 0.335448611182954, 0.163995962609024, 0.328414432657394, 0.477700760063001, -0.152793609708388, 0.192188934685942, 0.137049561691855, -0.0685542301292428, 0.328126132911985 },
                new double[] { -0.0118021512337526, 0.687830921400826, 0.0253622787075734, 0.0340672396524277, -0.128932474003535, 0.111659773051072, -0.253460159931006, -0.480758764565753, -0.0957818149202032, 0.338676131657749, 0.180081795108562, -0.210185274196965 },
                new double[] { 0.283462964841237, 0.0345316526707189, -0.136594149073437, -0.0420356194913279, -0.0133246227195541, 0.102617378729154, -0.00943588067870301, -0.0313070656278569, -0.574003455674121, 0.0681122379738542, -0.741712896390675, -0.0446883956970386 },
                new double[] { 0.244849290172628, 0.0918403159398603, -0.0949837111837109, 0.577123713866291, -0.38448721213687, 0.141537415024233, -0.0419331454701546, 0.452402089212107, 0.0853436565432261, -0.0866385554860394, 0.0434070293735178, -0.445597633077243 },
                new double[] { 0.400790602926352, -0.199507773642233, -0.135930857423972, -0.0303383327890531, 0.343450721385221, 0.108021571075484, 0.0137877512860494, 0.0892884039126865, -0.512143067951694, 0.158808597386327, 0.590928375174204, -0.0752948350722554 },
                new double[] { 0.221355443237417, -0.0542519363253111, -0.585993504810665, 0.258161164492889, 0.123510354803434, -0.32794171148589, -0.105403788999763, -0.0520352776917921, 0.325720237275597, 0.455414054946441, -0.0958514753059141, 0.280573659051638 },
                new double[] { 0.407153336780693, -0.096005525911419, 0.0677148671312564, -0.494402873973341, 0.192742899138734, 0.18832136952349, -0.123734336631771, 0.0245713241631777, 0.481424400615441, 0.12926200568014, -0.142716239407588, -0.467562669903876 },
                new double[] { 0.316238576923814, 0.181825098245019, 0.312639383751487, -0.345156725151147, -0.45371975647163, -0.374725665985126, -0.0236025978381509, 0.375795487357127, -0.0703605067966124, 0.23951349378026, 0.0902717934751914, 0.30253869587827 }
                });

            // Verify the eigen values
            ComplexVector eigenValues = m.EigenValues;
            Assert.AreEqual(12, eigenValues.Length, "Eigenvalue Length");
            NumericAssert.AreAlmostEqual(expectedEigenValues, eigenValues, "Eigenvalue Values");

            // verify the eigen vectors, except the first 4 (since their eigen values are 0)
            Matrix eigenVectors = m.EigenVectors;
            Assert.AreEqual(12, eigenVectors.RowCount, "Eigenvector Rows");
            Assert.AreEqual(12, eigenVectors.ColumnCount, "Eigenvector Columns");
            for(int i = 4; i < 12; i++)
            {
                Vector a = expectedEigenVectors.GetColumnVector(i);
                Vector b = eigenVectors.GetColumnVector(i);
             
                // Normalize sign
                if(a[0] < 0)
                {
                    a.NegateInplace();
                }

                if(b[0] < 0)
                {
                    b.NegateInplace();
                }

                // Compare
                NumericAssert.AreAlmostEqual(a, b, 1e-13, "Eigenvector Values: " + i.ToString());
            }
        }
    }
}
