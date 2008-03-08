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
    /// Precomputed periodic sample source
    /// </summary>
    public class ArbitraryPeriodicSource :
        IChannelSource
    {
        double[] _samples;
        readonly int _sampleCount;
        readonly int _delay;
        int _nextIndex;

        /// <summary>
        /// Create a new precomputed periodic sample source
        /// </summary>
        public
        ArbitraryPeriodicSource(
            double[] samples,
            int indexOffset,
            int delay
            )
        {
            if(null == samples)
                throw new ArgumentNullException("samples");
            if(0 == samples.Length)
                throw new ArgumentOutOfRangeException("samples");
            if(indexOffset < 0 || indexOffset >= samples.Length)
                throw new ArgumentOutOfRangeException("indexOffset");

            _samples = samples;
            _sampleCount = samples.Length;
            _delay = delay;

            int effectiveDelay = delay % _sampleCount;
            _nextIndex = (indexOffset - effectiveDelay + _sampleCount) % _sampleCount;
        }

        /// <summary>
        /// Create a new precomputed periodic sample source
        /// </summary>
        public
        ArbitraryPeriodicSource(
            double[] samples
            )
            : this(samples, 0, 0)
        {
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public
        double
        ReadNextSample()
        {
            double sample = _samples[_nextIndex];
            _nextIndex = (_nextIndex + 1) % _sampleCount;
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
