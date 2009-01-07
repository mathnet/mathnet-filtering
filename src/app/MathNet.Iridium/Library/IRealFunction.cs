//-----------------------------------------------------------------------
// <copyright file="IRealFunction.cs" company="Math.NET Project">
//    Copyright (c) 2004-2009, Joannes Vermorel.
//    All Right Reserved.
// </copyright>
// <author>
//    Joannes Vermorel, http://www.vermorel.com
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

namespace MathNet.Numerics
{
    /// <summary>
    /// Custom function double -> double, to be replaced later with the new generic Func delegate.
    /// </summary>
    public delegate double CustomFunction(double parameter);

    /// <summary>
    /// The interface <c>IRealFunction</c> defines an interface
    /// of real valued function with one real argument.
    /// </summary>
    /// <remarks>
    /// This interface will typically be implemented for
    /// distributions. See <see cref="MathNet.Numerics.Distributions"/>.
    /// </remarks>
    public interface IRealFunction
    {
        /// <summary>Gets the function value associated the provided
        /// <c>input</c> value.</summary>
        /// <remarks>The semantic associated to this interface is
        /// <i>deterministic function</i> (the same input should
        /// lead to the same returned value).</remarks>
        double ValueOf(double input);
    }
}
