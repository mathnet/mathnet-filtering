//-----------------------------------------------------------------------
// <copyright file="IVector.cs" company="Math.NET Project">
//    Copyright (c) 2004-2008, Christoph Rüegg.
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

namespace MathNet.Numerics.LinearAlgebra
{
    /// <summary>
    /// Generic Vector
    /// </summary>
    public interface IVector<T>
    {
        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        int Length
        {
            get;
        }

        /// <summary>
        /// Gets or set the element indexed by <c>i</c>
        /// in the <c>Vector</c>.
        /// </summary>
        /// <param name="i">Dimension index.</param>
        T this[int i]
        {
            get;
            set;
        }

        /// <summary>
        /// Copy all elements of this vector to an array.
        /// </summary>
        T[] CopyToArray();
    }
}
