#region MathNet Numerics, Copyright ©2004 Thaddaeus Parker

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Thaddaeus Parker
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

namespace MathNet.Numerics.Generators
{
    /// <summary>
    /// BinomialGenerator is a binomial deviate random number generator using the System.Math.Random as a seed generator.
    /// </summary>
    /// <remarks><i>Taken from Numerical Recipes in C</i>, William H. Press, et al; <b>Chap 7.3 pg 295 equation 7.3.7</b></remarks>
    public class BinomialGenerator : IRealGenerator
    {
        Random random;

        // TODO: refactor field names to replace '_foo' by 'foo'.

        private double _probability; //internal holder of probability factor
        private int _numberTrials; //number of trials that are needed to provide a rejection paradigm
        private static int oldNumber = -1;
        private static double oldProbability = -1.0;
        private double pc,  //a probability count
            probabilityLog,  //the logarithm of the probability
            pclog, //temp log value
            en,  //a rejection value
            oldGamma;  //the old gamma value

        /// <summary>Creates a new <see cref="BinomialGenerator"/> instance.</summary>
        /// <param name="probability">Probability.</param>
        /// <param name="numberTrials">Number trials to test the probability.</param>
        public BinomialGenerator(double probability, int numberTrials)
        {
            _probability = probability;
            _numberTrials = numberTrials;
            random = new Random();
        }

        public BinomialGenerator(double probability, int numberTrials, Random random)
        {
            _probability = probability;
            _numberTrials = numberTrials;
            this.random = random;
        }

        /// <summary>
        /// Returns the next binomial value based on the instantiated BinomialGenerator
        /// <seealso cref="BinomialGenerator.Next(double,int)"/>
        /// </summary>
        /// <returns>A number from an integer value that is a random deviate drawn from a binomial
        /// distribution of x trials each of probability xx</returns>
        public double Next()
        {
            // TODO: check that the call here below is correct
            // Check is good--TP
            return Next(this._probability, this._numberTrials);
        }

        /// <summary>
        /// Returns a number from an integer value that is a random deviate drawn from a binomial distribution of
        /// numTrials each of probability "probability", using the System.Random as a source of uniform random deviates.
        /// </summary>
        /// <param name="probability">The probability of each trial</param>
        /// <param name="numberTrials">The number of trials to perform</param>
        /// <returns>A binomial deviated floating point number</returns>
        public double Next(double probability, int numberTrials)
        {
            double inProbability; //used for the next function

            double amount, //the mean deviate to produce
                em,
                angle,  //an angle that is temporarily calculated for each iteration
                binomial; //the binomial return value
            int count = 0;
            double tmp; //used for gathering temporary deviations
            double yangle; //used for finding tmp value
            double root; //finds the root of amount of trials for the probability cause
            //reset the probability and number of trials but keep the same seed.
            _probability = (probability <= 0.5d) ? probability : 1.0 - probability;
            amount = numberTrials * _probability;  //mean deviate produced
            _numberTrials = numberTrials;
            inProbability = probability;
            if(_numberTrials < 25)
            {  //use the direct method while numberTrials is not too large
                binomial = 0.0;
                for(count = 1; count <= _numberTrials; count++)
                {
                    if(random.NextDouble() < _probability)
                        ++binomial;
                }
            }
            else if(amount < 1.0)
            {  //if fewer than one event is expected our of 25+ trials, then the distribution
                //is quite accurately Poisson.  Use the direct Poisson method
                double g = System.Math.Exp(-amount);
                double t = 1.0;
                for(count = 0; count <= _numberTrials; count++)
                {
                    t *= random.NextDouble();
                    if(t < g)
                        break;
                }
                binomial = ((count <= _numberTrials) ? count : _numberTrials);
            }
            else
            {
                //use rejection method
                if(_numberTrials != oldNumber)
                {  //if numberTrials has changed then compute useful quantities
                    en = numberTrials;
                    oldGamma = Fn.GammaLn(en + 1.0);
                    oldNumber = numberTrials;
                }
                if(_probability != oldProbability)
                {  //if _probability has changed, then compute useful quantities
                    pc = 1.0 - _probability;
                    probabilityLog = System.Math.Log(_probability);
                    pclog = System.Math.Log(pc);
                    oldProbability = _probability;
                }
                root = System.Math.Sqrt(2.0 * amount * pc);
                do
                {     // The following code is rejection method with a lorentzian comparison function
                    do
                    {
                        angle = System.Math.PI * random.NextDouble();
                        yangle = System.Math.Tan(angle);
                        em = root * yangle + amount; //trick for integer distribution
                    } while(em < 0.0 || em >= (en + 1.0)); //reject
                    em = System.Math.Floor(em);
                    tmp = 1.2 * root * (1.0 + yangle * yangle) * System.Math.Exp(oldGamma - Fn.GammaLn(em + 1.0) - Fn.GammaLn(en - em + 1.0) + em * probabilityLog + (en - em) * pclog);
                } while(random.NextDouble() > tmp);  //reject; this happens about 1.5 times per deviate, on avg
                binomial = em;
            }
            if(_probability != probability)
                binomial = numberTrials - binomial;  // remember to undo the symmetry transformation
            return binomial;
        }

    }//end of class BinomialGenerator

//    #region Testing Area
//#if DEBUG
//    /// <summary>
//    /// Testing suite for the <see cref="BinomialGenerator"/> class.
//    /// </summary>
//    [TestFixture]
//    public class BinomialTestingSuite
//    {
//        private BinomialGenerator bg;

//        /// <summary>
//        /// Initializes each test 
//        /// </summary>
//        [SetUp]
//        public void Init(){
//            bg = new BinomialGenerator(0.65,50);
//        }
//        /// <summary>
//        /// Tears down the  test.
//        /// </summary>
//        [TearDown]
//        public void TearDownTest(){
//            bg = null;
//        }
//        /// <summary>
//        /// Testing the <see cref="BinomialGenerator.Next"/>
//        /// </summary>
//        [Test]
//        public void Next() {
//            System.Console.WriteLine(bg.Next());
//        }
//        /// <summary>
//        /// Testing the <see cref="BinomialGenerator.Next"/>
//        /// </summary>
//        [Test]
//        public void NextWithValues() {
//            for(int i = 0; i < 10; i++){
//                System.Console.WriteLine(bg.Next(new Random().NextDouble(),new Random().Next(150)));
//            }
//        }
//    }
//#endif
//    #endregion
}
