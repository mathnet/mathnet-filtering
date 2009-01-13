//-----------------------------------------------------------------------
// <copyright file="ScalarIterator.cs" company="Math.NET Project">
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
    ////public class ScalarIterator
    ////{
    ////    IRealFunction f, fderiv;

    ////    public ScalarIterator(IRealFunction f)
    ////    {
    ////        this.f = f;
    ////    }
    ////    public ScalarIterator(IRealFunction f, IRealFunction derivative)
    ////    {
    ////        this.f = f;
    ////        this.fderiv = derivative;
    ////    }

    ////    /// <summary>Finds a solution of the equation f(x)=x near a given estimation.</summary>
    ////    /// <param name="estimation">Start value.</param>
    ////    /// <param name="relativeTolerance">Normalized tolerance, usually between 10^(-3) and 10^(-9).</param>
    ////    /// <param name="absoluteTolerance">Absolute tolerance, usually a few times of <see cref="System.Double.Epsilon"/>.</param>
    ////    /// <returns>z, one of the roots of f-x so that f(z)-z=0, or f(z)=z</returns>
    ////    public double FindFixpoint(double estimation, double relativeTolerance, double absoluteTolerance)
    ////    {
    ////        throw new NotImplementedException();
    ////    }

    ////    /// <summary>Finds a solution of the equation f(x)=0 near a given estimation.</summary>
    ////    /// <param name="firstEstimation">First start value.</param>
    ////    /// <param name="secondEstimation">Second start value.</param>
    ////    /// <param name="relativeTolerance">Normalized tolerance, usually between 10^(-3) and 10^(-9).</param>
    ////    /// <param name="absoluteTolerance">Absolute tolerance, usually a few times of <see cref="System.Double.Epsilon"/>.</param>
    ////    /// <returns>z, one of the roots of f so that f(z)=0</returns>
    ////    /// <remarks>This method uses the Quasi-Newton-Raphson method in the secant-modification if no derivative is provided (that's why two start values are required), or the classic Newton-Raphson method if a derivative is provided (although usually not recommended).</remarks>
    ////    public double FindRoot(double firstEstimation, double secondEstimation, double relativeTolerance, double absoluteTolerance)
    ////    {
    ////        if(fderiv == null) //Quasi-Newton-Raphson using Secant Method
    ////        {
    ////            throw new NotImplementedException();
    ////        }
    ////        else //Newton-Raphson
    ////        {
    ////            throw new NotImplementedException();
    ////        }
    ////    }
    ////}
}
