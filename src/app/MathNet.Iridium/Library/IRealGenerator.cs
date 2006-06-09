#region MathNet Numerics, Copyright ©2004 Joannes Vermorel

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

using System;

namespace MathNet.Numerics
{
	/// <summary>
	/// The <c>IRealGenerator</c> interface defines a generator
	/// of real (here <c>double</c>) values.
	/// </summary>
	/// <remarks>
	/// The implementations of this class will typically
	/// be random generators. See <see cref="MathNet.Numerics.Generators"/>.
	/// </remarks>
	public interface IRealGenerator
	{
		/// <summary>Returns the next value of the generator.</summary>
		double Next();
	}
}
