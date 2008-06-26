#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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
#region Derived From: Copyright 2006 Troschütz
/* 
 * Derived from the Troschuetz.Random Class Library,
 * Copyright © 2006 Stefan Troschütz (stefan@troschuetz.de)
 * 
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
 */
#endregion

using System;
using MathNet.Numerics.RandomSources;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Provides generation of binomial distributed random numbers.
    /// </summary>
    /// <remarks>
    /// The binomial distribution generates only discrete numbers.<br />
    /// The implementation bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/binomial_distribution">Wikipedia - Binomial distribution</a>.
    /// </remarks>
    public sealed class BinomialDistribution : DiscreteDistribution
    {
        double _p;
        int _n;

        #region Construction
        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BinomialDistribution()
            : base()
        {
            SetDistributionParameters(0.5, 1);
        }

        /// <summary>
        /// Initializes a new instance, using the specified <see cref="RandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        /// <param name="random">A <see cref="RandomSource"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="random"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public
        BinomialDistribution(
            RandomSource random
            )
            : base(random)
        {
            SetDistributionParameters(0.5, 1);
        }

        /// <summary>
        /// Initializes a new instance, using a <see cref="SystemRandomSource"/>
        /// as underlying random number generator.
        /// </summary>
        public
        BinomialDistribution(
            double probabilityOfSuccess,
            int numberOfTrials
            )
            : base()
        {
            SetDistributionParameters(probabilityOfSuccess, numberOfTrials);
        }
        #endregion

        #region Distribution Parameters
        /// <summary>
        /// Gets or sets the success probability parameter.
        /// </summary>
        public double ProbabilityOfSuccess
        {
            get { return _p; }
            set { SetDistributionParameters(value, _n); }
        }

        /// <summary>
        /// Gets or sets the number of trials parameter.
        /// </summary>
        /// <remarks>Call <see cref="IsValidParameterSet"/> to determine whether a value is valid and therefore assignable.</remarks>
        public int NumberOfTrials
        {
            get { return _n; }
            set { SetDistributionParameters(_p, value); }
        }

        /// <summary>
        /// Configure all distribution parameters.
        /// </summary>
        public
        void
        SetDistributionParameters(
            double probabilityOfSuccess,
            int numberOfTrials
            )
        {
            if(!IsValidParameterSet(probabilityOfSuccess, numberOfTrials))
            {
                throw new ArgumentException(Properties.Resources.ArgumentParameterSetInvalid);
            }

            _p = probabilityOfSuccess;
            _n = numberOfTrials;
        }

        /// <summary>
        /// Determines whether the specified parameters is valid.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if probabilityOfSuccess is greater than or equal to 0.0, and less than or equal to 1.0,
        /// and numberOfTrials is greater than or equal to 0; otherwise, <see langword="false"/>.
        /// </returns>
        public static
        bool
        IsValidParameterSet(
            double probabilityOfSuccess,
            int numberOfTrials
            )
        {
            return (probabilityOfSuccess >= 0.0 && probabilityOfSuccess <= 1.0)
                && (numberOfTrials >= 0);
        }
        #endregion

        #region Distribution Properties
        /// <summary>
        /// Gets the minimum possible value of generated random numbers.
        /// </summary>
        public override int Minimum
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the maximum possible value of generated random numbers.
        /// </summary>
        public override int Maximum
        {
            get { return _n; }
        }

        /// <summary>
        /// Gets the mean value of generated random numbers.
        /// </summary>
        public override double Mean
        {
            get { return _p * _n; }
        }

        /// <summary>
        /// Gets the median of generated random numbers.
        /// </summary>
        public override int Median
        {
            get { return (int)Math.Floor(_p * _n); }
        }

        /// <summary>
        /// Gets the variance of generated random numbers.
        /// </summary>
        public override double Variance
        {
            get { return _p * (1.0 - _p) * _n; }
        }

        /// <summary>
        /// Gets the skewness of generated random numbers.
        /// </summary>
        public override double Skewness
        {
            get { return (1.0 - 2.0 * _p) / Math.Sqrt(_p * (1.0 - _p) * _n); }
        }

        /// <summary>
        /// Discrete probability mass function (pmf) of this probability distribution.
        /// </summary>
        public override
        double
        ProbabilityMass(
            int x
            )
        {
            return Fn.BinomialCoefficient(_n, x) * Math.Pow(_p, x) * Math.Pow(1 - _p, _n - x);
        }

        /// <summary>
        /// Continuous cumulative distribution function (cdf) of this probability distribution.
        /// </summary>
        public override
        double
        CumulativeDistribution(
            double x
            )
        {
            return Fn.BetaRegularized(_n - x, x + 1, 1 - _p);
        }
        #endregion

        #region Generator
        /// <summary>
        /// Returns a bernoulli distributed random number.
        /// </summary>
        /// <returns>A bernoulli distributed 32-bit signed integer.</returns>
        public override
        int
        NextInt32()
        {
            // TODO: Implement direct transformation instead of simulation
            int successes = 0;
            for(int i = 0; i < _n; i++)
            {
                if(this.RandomSource.NextDouble() < _p)
                {
                    successes++;
                }
            }

            return successes;



            // ALTERNATIVE IMPLEMENTATION (by Thaddaeus Parker/NR), CONSIDER USE THIS INSTEAD:

            /*
             * 
               
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
            
           
             * 
             */
        }
        #endregion
    }
}
