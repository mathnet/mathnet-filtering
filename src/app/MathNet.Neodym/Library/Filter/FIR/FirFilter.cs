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
using System.Collections.Generic;

namespace MathNet.SignalProcessing.Filter.FIR
{
	/// <summary>
	/// Finite Impulse Response (FIR) Filters are based on
	/// fourier series and implemented using a discrete
	/// convolution equation. FIR Filters are always
	/// online, stable and causal. 
	/// </summary>
    /// <remarks>
    /// System Descripton: H(z) = a0 + a1*z^-1 + a2*z^-2 + ...
    /// </remarks>
	public class OnlineFirFilter : OnlineFilter
	{
		private double[] _coefficients;
        private double[] _buffer;
        private int _offset;
        private readonly int _size;

		/// <summary>
		/// Finite Impulse Response (FIR) Filter.
		/// </summary>
		public OnlineFirFilter(IList<double> coefficients)
		{
            _size = coefficients.Count;
            _offset = 0;
            _coefficients = new double[_size << 1];
            for(int i = 0; i < _size; i++)
                _coefficients[i] = _coefficients[_size + i] = coefficients[i];
            _buffer = new double[_size];
		}

		/// <summary>
		/// Process a single sample.
		/// </summary>
		public override double ProcessSample(double sample)
		{
            _offset = (_offset != 0) ? _offset - 1 : _size - 1;
            _buffer[_offset] = sample;

            double acc = 0;
            for(int i = 0, j = _size - _offset; i < _size; i++, j++)
                acc += _buffer[i] * _coefficients[j];

            return acc;
		}

		/// <summary>
		/// Reset internal state (not coefficients!).
		/// </summary>
		public override void Reset()
		{
            for(int i = 0; i < _buffer.Length; i++)
                _buffer[i] = 0d;
		}
    }
}
