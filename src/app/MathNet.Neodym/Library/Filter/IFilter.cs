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
	/// An online filter that allows processing samples just in time.
	/// Online Filters are always causal.
	/// </summary>
	public interface IOnlineFilter
	{
		/// <summary>Process a single sample.</summary>
		double ProcessSample(double sample);
		/// <summary>Process a whole set of samples at once.</summary>
		double[] ProcessSamples(double[] samples);
		/// <summary>Reset internal state (not coefficients!).</summary>
		void Reset();
	}

	/*
	public interface IOfflineFilter
	{
		void Process();
	}
	*/
}
