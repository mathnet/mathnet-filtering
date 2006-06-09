#region Copyright ©2000 The MathWorks and NIST, ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
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

namespace MathNet.Numerics.LinearAlgebra
{
	/// <summary>
	/// <p>This namespace is a port of the 
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA library</a>.</p>
	/// 
	/// <p>The <c>MathNet.Numerics.LinearAlgebra</c> provides the fundamental operations 
	/// of numerical linear algebra.  Various constructors create Matrices from two 
	/// dimensional arrays of double precision floating point numbers.  Various "gets" 
	/// and "sets" provide access to submatrices and matrix elements.  Several methods 
	/// implement basic matrix arithmetic, including matrix addition and
	/// multiplication, matrix norms, and element-by-element array operations.
	/// Methods for reading and printing matrices are also included.  All the
	/// operations in this version of the Matrix Class involve real matrices.
	/// Complex matrices may be handled in a future version.</p>
	/// 
	/// <p>Five fundamental matrix decompositions, which consist of pairs or triples
	/// of matrices, permutation vectors, and the like, produce results in five
	/// decomposition classes.  These decompositions are accessed by the Matrix
	/// class to compute solutions of simultaneous linear equations, determinants,
	/// inverses and other matrix functions.</p>
	/// 
	/// The five decompositions are:<br/>
	/// <UL>
	/// <LI>Cholesky Decomposition of symmetric, positive definite matrices.</LI>
	/// <LI>LU Decomposition of rectangular matrices.</LI>
	/// <LI>QR Decomposition of rectangular matrices.</LI>
	/// <LI>Singular Value Decomposition of rectangular matrices.</LI>
	/// <LI>Eigenvalue Decomposition of both symmetric and nonsymmetric square matrices.</LI>
	/// </UL>
	/// 
	/// <p><b>Example of use:</b> Solve a linear system <c>A x = b</c> and compute the residual norm, 
	/// <c>||b - A x||</c>.</p>
	/// <code>
	/// double[,] vals = {{1.,2.,3.},{4.,5.,6.},{7.,8.,10.}};
	/// Matrix a = new Matrix(vals);
	/// Matrix b = Matrix.Random(3,1);
	/// Matrix x = a.Solve(b);
	/// Matrix r = a * x - b;
	/// double rnorm = r.NormInf();
	/// </code>
	/// 
	/// <p>Author: The MathWorks, Inc. and the National Institute of Standards and Technology (5 August 1998).</p>
	/// <p>Port: Joannes Vermorel (2004).</p>
	/// </summary>
	public class NamespaceDoc
	{
		private NamespaceDoc() {}
		// nothing, documentation only.
	}
}
