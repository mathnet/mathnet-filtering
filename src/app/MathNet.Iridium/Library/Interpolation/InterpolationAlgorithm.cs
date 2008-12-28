//-----------------------------------------------------------------------
// <copyright file="InterpolationAlgorithm.cs" company="Math.NET Project">
//    Copyright (c) 2002-2008, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
//-----------------------------------------------------------------------

using System;
using MathNet.Numerics;

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Interpolation algorithm
    /// </summary>
    [Obsolete("Please use IInterpolationMethod instead. This interface is obsolete and will be removed in future versions.")]
    public interface IInterpolationAlgorithm
    {
        /// <summary>
        /// Maximum interpolation order.
        /// </summary>
        /// <seealso cref="EffectiveOrder"/>
        int MaximumOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Effective interpolation order.
        /// </summary>
        /// <seealso cref="MaximumOrder"/>
        int EffectiveOrder
        {
            get;
        }

        /// <summary>
        /// Precompute/optimize the algoritm for the given sample set.
        /// </summary>
        void Prepare(SampleList samples);

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        double Interpolate(double t);

        /// <summary>
        /// Extrapolate at point t.
        /// </summary>
        double Extrapolate(double t);

        /// <summary>
        /// True if the alorithm supports error estimation.
        /// </summary>
        bool SupportErrorEstimation
        {
            get;
        }

        /// <summary>
        /// Interpolate at point t and return the estimated error as error-parameter.
        /// </summary>
        double Interpolate(double t, out double error);
    }
}
