//-----------------------------------------------------------------------
// <copyright file="IMatrix.cs" company="Math.NET Project">
//    Copyright (c) 2004-2009, Christoph Rüegg.
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
    /// Generic Matrix
    /// </summary>
    public interface IMatrix<T>
    {
        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        int RowCount
        {
            get;
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        int ColumnCount
        {
            get;
        }

        /// <summary>
        /// Gets or set the element indexed by <c>(i, j)</c>
        /// in the <c>Matrix</c>.
        /// </summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        T this[int i, int j]
        {
            get;
            set;
        }

        /// <summary>
        /// Copy all elements of this matrix to a rectangular 2D array.
        /// </summary>
        T[,] CopyToArray();

        /// <summary>
        /// Copy all elements of this matrix to a jagged array.
        /// </summary>
        T[][] CopyToJaggedArray();
    }
}
