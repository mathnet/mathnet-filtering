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

namespace MathNet.Filtering.Filter.Utils
{
    /// <summary>
    /// ShiftBuffer is a circular buffer behaving like a shift register (el. engineering)
    /// </summary>
    public class ShiftBuffer
    {
        int _offset = 0;
        readonly int _size;
        double[] _buffer;

        /// <summary>
        /// Shift Buffer for discrete convolutions.
        /// </summary>
        public
        ShiftBuffer(
            int size
            )
        {
            _size = size;
            _buffer = new double[size];
        }
        /// <summary>
        /// Shift Buffer for discrete convolutions.
        /// </summary>
        public
        ShiftBuffer(
            double[] buffer
            )
        {
            if(null == buffer)
                throw new ArgumentNullException("buffer");

            _size = buffer.Length;
            _buffer = buffer;
        }

        /// <summary>
        /// Add a new sample on top of the buffer and discard the oldest entry (tail).
        /// </summary>
        public
        void
        ShiftAddHead(
            double sample
            )
        {
            _offset = (_offset != 0) ? _offset - 1 : _size - 1;
            _buffer[_offset] = sample;
        }

        /// <summary>
        /// Buffer Indexer. The newest (top) item has index 0.
        /// </summary>
        public double this[int index]
        {
            get { return _buffer[(_offset + index) % _size]; }
            set { _buffer[(_offset + index) % _size] = value; }
        }

        /// <summary>
        /// Discrete Convolution. Evaluates the classic MAC operation to another buffer/vector (looped).
        /// </summary>
        /// <returns>The sum of the memberwiese sample products, sum(a[i]*b[i],i=0..size)</returns>
        public
        double
        MultiplyAccumulate(
            double[] samples
            )
        {
            if(null == samples)
                throw new ArgumentNullException("samples");

            int len = Math.Min(samples.Length, _size);
            double sum = 0;
            int j = _offset;
            for(int i = 0; i < len; i++)
            {
                sum += samples[i] * _buffer[j];
                j = (j + 1) % _size;
            }
            return sum;
        }

        /// <summary>
        /// Discrete Convolution. Evaluates the classic MAC operation to another buffer/vector (looped).
        /// </summary>
        /// <returns>The sum of the memberwiese sample products, sum(a[i]*b[i],i=0..size)</returns>
        public
        double
        MultiplyAccumulate(
            double[] samples,
            int sampleOffset,
            int count
            )
        {
            if(null == samples)
                throw new ArgumentNullException("samples");

            int end = Math.Min(
                Math.Min(samples.Length, _size + sampleOffset),
                count + sampleOffset
                );

            double sum = 0;
            int j = _offset;
            for(int i = sampleOffset; i < end; i++)
            {
                sum += samples[i] * _buffer[j];
                j = (j + 1) % _size;
            }
            return sum;
        }

        /// <summary>
        /// Sets all buffer items to zero.
        /// </summary>
        public
        void
        Reset()
        {
            for(int i = 0; i < _size; i++)
            {
                _buffer[i] = 0;
            }
        }
    }
}
