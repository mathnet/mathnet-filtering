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
using MathNet.SignalProcessing.Channel;
using MathNet.Numerics.Distributions;

namespace MathNet.SignalProcessing.DataSources
{
    /// <summary>
    /// Sample source with independent amplitudes of normal distribution and a flat spectral density.
    /// </summary>
    public class WhiteGaussianNoiseSource :
        IChannelSource
    {
        readonly IContinuousDistribution _distribution;

        /// <summary>
        /// Create a gaussian noise source with normally distributed amplitudes.
        /// </summary>
        /// <param name="uniformWhiteRandomSource">Uniform white random source.</param>
        /// <param name="mean">mu-parameter of the normal distribution</param>
        /// <param name="standardDeviation">sigma-parameter of the normal distribution</param>
        public
        WhiteGaussianNoiseSource(
            Random uniformWhiteRandomSource,
            double mean,
            double standardDeviation
            )
        {
            _distribution = new Normal(mean, standardDeviation, uniformWhiteRandomSource);
        }

        /// <summary>
        /// Create a gaussian noise source with normally distributed amplites.
        /// </summary>
        /// <param name="mean">mu-parameter of the normal distribution</param>
        /// <param name="standardDeviation">sigma-parameter of the normal distribution</param>
        public
        WhiteGaussianNoiseSource(
            double mean,
            double standardDeviation
            )
        {
            // assuming the default random source is white
            _distribution = new Normal(mean, standardDeviation);
        }

        /// <summary>
        /// Create a gaussian noise source with standard distributed amplitudes.
        /// </summary>
        public
        WhiteGaussianNoiseSource()
        {
            // assuming the default random source is white
            _distribution = new Normal();
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public
        double
        ReadNextSample()
        {
            return _distribution.Sample();
        }

        /// <summary>
        /// Sample delay of this source in relation to the whole system. Constant Zero.
        /// </summary>
        public int Delay
        {
            get { return 0; }
        }
    }
}
