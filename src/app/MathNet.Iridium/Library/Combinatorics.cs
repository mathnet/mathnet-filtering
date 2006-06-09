#region MathNet Numerics, Copyright ©2006 Christoph Ruegg

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004-2006,	Christoph Ruegg, http://www.cdrnet.net
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
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// Static DoublePrecision Combinatorics Helper Class
    /// </summary>
    public static class Combinatorics
    {
        /// <summary>
        /// Computes the number of variations without repetition. When the order matters and each object can be chosen only once.
        /// </summary>
        public static double Variations(int n, int k)
        {
            if(k < 0 || k > n)
                return 0;
            return Math.Floor(0.5 + System.Math.Exp(Fn.FactorialLn(n) - Fn.FactorialLn(n - k)));
        }

        /// <summary>
        /// Computes the number of variations with repetition. When the order matters and an object can be chosen more than once.
        /// </summary>
        public static double VariationsWithRepetition(int n, int k)
        {
            return Math.Pow(n, k);
        }

        /// <summary>
        /// Computes the number of combinations without repetition. When the order does not matter and each object can be chosen only once.
        /// </summary>
        public static double Combinations(int n, int k)
        {
            return Fn.BinomialCoefficient(n, k);
        }

        /// <summary>
        /// Computes the number of combinations with repetition. When the order does not matter and an object can be chosen more than once.
        /// </summary>
        public static double CombinationsWithRepetition(int n, int k)
        {
            if(k < 0)
                return 0;
            return Math.Floor(0.5 + System.Math.Exp(Fn.FactorialLn(n + k - 1) - Fn.FactorialLn(k) - Fn.FactorialLn(n - 1)));
        }

        /// <summary>
        /// Computes the number of permutations without repetition. 
        /// </summary>
        public static double Permutations(int n)
        {
            return Fn.Factorial(n);
        }
    }
}
