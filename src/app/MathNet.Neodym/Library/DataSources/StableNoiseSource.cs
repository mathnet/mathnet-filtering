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
using MathNet.Numerics.Distributions;
using MathNet.Numerics.RandomSources;

namespace MathNet.SignalProcessing.DataSources
{
    /// <summary>
    /// Sample source with skew alpha stable distributed samples.
    /// </summary>
    public class StableNoiseSource :
        IChannelSource
    {
        IContinuousGenerator _distribution;

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="uniformWhiteRandomSource">Uniform white random source.</param>
        /// <param name="location">mu-parameter of the stable distribution</param>
        /// <param name="scale">c-parameter of the stable distribution</param>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public
        StableNoiseSource(
            RandomSource uniformWhiteRandomSource,
            double location,
            double scale,
            double exponent,
            double skewness
            )
        {
            StableDistribution stable = new StableDistribution(uniformWhiteRandomSource);
            stable.SetDistributionParameters(location, scale, exponent, skewness);
            _distribution = stable;
        }

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="location">mu-parameter of the stable distribution</param>
        /// <param name="scale">c-parameter of the stable distribution</param>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public
        StableNoiseSource(
            double location,
            double scale,
            double exponent,
            double skewness
            )
        {
            _distribution = new StableDistribution(location, scale, exponent, skewness);
        }

        /// <summary>
        /// Create a skew alpha stable noise source.
        /// </summary>
        /// <param name="exponent">alpha-parameter of the stable distribution</param>
        /// <param name="skewness">beta-parameter of the stable distribution</param>
        public
        StableNoiseSource(
            double exponent,
            double skewness
            )
        {
            _distribution = new StableDistribution(0.0, 1.0, exponent, skewness);
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public
        double
        ReadNextSample()
        {
            return _distribution.NextDouble();
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
