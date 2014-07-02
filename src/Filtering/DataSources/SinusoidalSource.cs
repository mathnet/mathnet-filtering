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
using System.Text;

using MathNet.SignalProcessing.Channel;

namespace MathNet.SignalProcessing.DataSources
{
    /// <summary>
    /// Sinus sample source.
    /// </summary>
    public class SinusoidalSource :
        IChannelSource
    {
        readonly int _delay;
        readonly double _amplitude;
        readonly double _mean;
        readonly double _phaseStep;
        double _nextPhase;
        const double _pi2 = 2 * Math.PI;

        /// <summary>
        /// Create a new on-demand sinus sample source with the given parameters.
        /// </summary>
        public
        SinusoidalSource(
            double samplingRate,
            double frequency,
            double amplitude,
            double phase,
            double mean,
            int delay
            )
        {
            _delay = delay;
            _mean = mean;
            _amplitude = amplitude;
            _phaseStep = frequency / samplingRate * _pi2;
            _nextPhase = phase - delay * _phaseStep;
        }

        /// <summary>
        /// Create a new on-demand sinus sample source with the given parameters an zero phase and mean.
        /// </summary>
        public
        SinusoidalSource(
            double samplingRate,
            double frequency,
            double amplitude
            )
            : this(samplingRate, frequency, amplitude, 0.0, 0.0, 0)
        {
        }

        /// <summary>
        /// Creates a pre-computed sinus sample source with the given parameters and zero mean.
        /// </summary>
        public static
        IChannelSource
        Precompute(
            int samplesPerPeriod,
            double amplitude,
            double phase
            )
        {
            double[] samples = SignalGenerator.Sine(
                samplesPerPeriod, // samplingRate
                1.0, // frequency
                phase,
                amplitude,
                samplesPerPeriod // length
                );

            return new ArbitraryPeriodicSource(samples);
        }

        /// <summary>
        /// Creates a pre-computed sinus sample source with the given parameters and zero phase and mean.
        /// </summary>
        public static
        IChannelSource
        Precompute(
            int samplesPerPeriod,
            double amplitude
            )
        {
            return Precompute(samplesPerPeriod, amplitude, 0.0);
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public
        double
        ReadNextSample()
        {
            double sample = _mean + _amplitude * Math.Sin(_nextPhase);
            _nextPhase += _phaseStep;
            if(_nextPhase > _pi2)
            {
                _nextPhase -= _pi2;
            }
            return sample;
        }

        /// <summary>
        /// Sample delay of this source in relation to the whole system.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
        }
    }
}
