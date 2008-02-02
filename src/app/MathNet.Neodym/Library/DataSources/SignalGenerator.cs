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

namespace MathNet.SignalProcessing.DataSources
{
    /// <summary>
    /// Generators for sinusidual and theoretical signal vectors. 
    /// </summary>
    public class SignalGenerator
    {
        /// <summary>
        /// Create a Sine Signal Sample Vector.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="frequency">Frequency in samples per unit.</param>
        /// <param name="phase">Optional phase offset.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Sine(double samplingRate, double frequency, double phase, double amplitude, int length)
        {
            double[] data = new double[length];
            double step = frequency / samplingRate * 2 * Math.PI;
            for(int i = 0; i < length; i++)
                data[i] = amplitude * Math.Sin(phase + i * step);
            return data;
        }

        /// <summary>
        /// Create a Heaviside Step Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Step(int offset, double amplitude, int length)
        {
            double[] data = new double[length];
            int cursor;
            for(cursor = 0; cursor < offset && cursor < length; cursor++)
                data[cursor] = 0d;
            for(; cursor < length; cursor++)
                data[cursor] = amplitude;
            return data;
        }

        /// <summary>
        /// Create a Dirac Delta Impulse Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="frequency">impulse sequence frequency. -1 for single impulse only.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Impulse(int offset, int frequency, double amplitude, int length)
        {
            double[] data = new double[length];
            if(frequency <= 0)
                data[offset] = amplitude;
            else
                while(offset < length)
                {
                    data[offset] = amplitude;
                    offset += frequency;
                }
            return data;
        }
    }
}
