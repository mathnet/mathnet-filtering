//-----------------------------------------------------------------------
// <copyright file="Solver.cs" company="Math.NET Project">
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

namespace MathNet.Numerics.Equations
{
    using MathNet.Numerics.LinearAlgebra;

    ////public enum SolveMode
    ////{
    ////    /// <summary>Exact solving (up to rounding errors). Probably inefficient for huge data sets.</summary>
    ////    Direct,
    ////    /// <summary>Iterative approximation with choosable tolerance to control performance for large data sets.</summary>
    ////    Iterative
    ////}

    ////public class Solver
    ////{
    ////    private Solver();

    ////    /// <summary>Solves   Exact solving (up to rounding errors). Probably inefficient for huge data sets.</summary>
    ////    /// <param name="A"></param>
    ////    /// <param name="B"></param>
    ////    /// <returns></returns>
    ////    public static Matrix Solve(Matrix A, Matrix B)
    ////    {
    ////        return A.Solve(B);
    ////    }

    ////    /// <summary></summary>
    ////    /// <param name="A"></param>
    ////    /// <param name="B"></param>
    ////    /// <param name="estimation"></param>
    ////    /// <returns></returns>
    ////    public static Matrix Solve(Matrix A, Matrix B, Matrix estimation)
    ////    {
    ////    }

    ////    /// <summary>Solves the equation f(x)=0 to x.</summary>
    ////    /// <param name="f">The function to find the roots of.</param>
    ////    /// <param name="tolerance">normalized (relative) tolerance, usually between 10^(-3) and 10^(-9)</param>
    ////    /// <param name="estimation">Start value.</param>
    ////    /// <returns>z, one of the roots of f so that f(z)=0</returns>
    ////    /// <remarks>This method usually uses <see cref="ScalarIterator.FindRoot"/> but could also (automatically) decide to use other algorithms if appropriate.</remarks>
    ////    public static double Solve(IRealFunction f, double estimation, double tolerance)
    ////    {
    ////        ScalarIterator iterator = new ScalarIterator(f);
    ////        return iterator.FindRoot(estimation,estimation*0.95d,tolerance,100*double.Epsilon);
    ////    }
    ////}
}
