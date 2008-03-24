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
using MathNet.SignalProcessing.Properties;

namespace MathNet.SignalProcessing.Windowing
{
    /// <summary>
    /// A windowing/apodization function.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Window width, number of samples. Typically an integer power of 2.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Window coefficients
        /// </summary>
        /// <param name="sampleIndex">window sample index, between 0 and Width-1 (inclusive).</param>
        double this[int sampleIndex] { get; }

        /// <summary>
        /// Copy the window coefficients to a double array.
        /// </summary>
        double[] CopyToArray();

        /// <summary>
        /// Compute the window for the current configuration.
        /// Skipped if the window has already been computed.
        /// </summary>
        void Precompute();
    }

    /// <summary>
    /// A windowing/apodization function.
    /// </summary>
    public abstract class Window :
        IWindow
    {
        int _width = 1; // N
        double[] _coefficients;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected
        Window()
        {
        }

        /// <summary>
        /// Windowing function generator implementation.
        /// </summary>
        /// <param name="width">window size, guaranteed to be greater than or equal to 2.</param>
        /// <returns>Window coefficients, array with length of 'width'</returns>
        protected abstract
        double[]
        ComputeWindowCore(
            int width
            );

        /// <summary>
        /// Reset the coefficients, triggering Precompute the next time a coefficient is requested.
        /// </summary>
        protected
        void
        Reset()
        {
            _coefficients = null;
        }

        /// <summary>
        /// Compute the window for the current configuration.
        /// Skipped if the window has already been computed.
        /// </summary>
        public
        void
        Precompute()
        {
            if (null != _coefficients)
            {
                // coefficients already available, skip
                return;
            }

            if (_width == 1)
            {
                // trivial window
                _coefficients = new double[1];
                _coefficients[0] = 1.0;
                return;
            }

            _coefficients = ComputeWindowCore(_width);

            if (null == _coefficients || _width != _coefficients.Length)
            {
                throw new ApplicationException(Resources.InvalidWindowFunctionException);
            }
        }

        /// <summary>
        /// Copy the window coefficients to a double array.
        /// </summary>
        public
        double[]
        CopyToArray()
        {
            Precompute();
            double[] data = new double[_width];
            _coefficients.CopyTo(data, 0);
            return data;
        }

        /// <summary>
        /// Window width, number of samples. Typically an integer power of 2, must be greater than 0.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int Width
        {
            get { return _width; }
            set
            {
                if (0 >= value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _width = value;
                Reset();
            }
        }

        /// <summary>
        /// Window coefficients
        /// </summary>
        /// <param name="sampleIndex">window sample index, between 0 and Width-1 (inclusive).</param>
        public double this[int sampleIndex]
        {
            get
            {
                if (0 > sampleIndex || _width <= sampleIndex)
                {
                    throw new ArgumentOutOfRangeException("sampleIndex");
                }

                if (null == _coefficients)
                {
                    Precompute();
                }

                return _coefficients[sampleIndex];
            }
        }
    }
}
