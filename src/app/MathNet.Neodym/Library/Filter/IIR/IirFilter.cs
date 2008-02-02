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

namespace MathNet.SignalProcessing.Filter.IIR
{
	/// <summary>
	/// Infinite Impulse Response (FIR) Filters need much
	/// less coefficients (and are thus much faster) than
	/// comparable FIR Filters, but are potentially unstable.
	/// IIR Filters are always online and causal. This IIR
	/// Filter implements the canonic Direct Form II structure.
    /// </summary>
    /// <remarks>
    /// System Descripton: H(z) = (a0 + a1*z^-1 + a2*z^-2) / (1 + b1*z^-1 + b2*z^-2)
    /// </remarks>
	public class OnlineIirFilter : OnlineFilter
	{
		private double[] _leftCoefficients, _rightCoefficients;
		private double[] _buffer;
		private readonly int _size, _halfSize;
        private int _offset;

		/// <summary>
		/// Infinite Impulse Response (IIR) Filter.
		/// </summary>
        public OnlineIirFilter(double[] coefficients)
        {
            if((coefficients.Length & 1) != 0)
                throw new ArgumentException("Even number of coefficients required", "coefficients");

            _size = coefficients.Length;
            _halfSize = _size >> 1;
            _offset = 0;
            _leftCoefficients = new double[_size];
            _rightCoefficients = new double[_size];
            for(int i = 0; i < _halfSize; i++)
            {
                _leftCoefficients[i] = _leftCoefficients[_halfSize + i] = coefficients[i];
                _rightCoefficients[i] = _rightCoefficients[_halfSize + i] = coefficients[_halfSize + i];
            }
            _buffer = new double[_size];
        }
		
		/// <summary>
		/// Process a single sample.
		/// </summary>
		public override double ProcessSample(double sample)
		{
            double un = _leftCoefficients[0] * sample;
            for(int i = 0, j = _halfSize - _offset + 1; i < _halfSize - 1; i++, j++)
                un = _buffer[i] * _leftCoefficients[j];

            _offset = (_offset != 0) ? _offset - 1 : _halfSize - 1;
            _buffer[_offset] = un - _buffer[_offset] * _leftCoefficients[1];

            double yn = 0d;
            for(int i = 0, j = _halfSize - _offset; i < _halfSize; i++, j++)
                yn += _buffer[i] * _rightCoefficients[j];

            return yn;
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
