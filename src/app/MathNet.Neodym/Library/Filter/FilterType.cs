#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2008, Christoph Rüegg,  http://christoph.ruegg.name
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

namespace MathNet.SignalProcessing.Filter
{
	/// <summary>
	/// Frequency Filter Type
	/// </summary>
	public enum FilterType
	{
		/// <summary>LowPass, lets only low frequencies pass.</summary>
		LowPass,
		/// <summary>HighPass, lets only high frequencies pass.</summary>
		HighPass,
		/// <summary>BandPass, lets only frequencies pass that are inside of a band.</summary>
		BandPass,
		/// <summary>BandStop, lets only frequencies pass that are outside of a band.</summary>
		BandStop,
		/// <summary>Other behavior.</summary>
		Other
	}
}
