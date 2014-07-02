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

namespace MathNet.Filtering.Windowing
{
    /// <summary>
    /// Gauss window.
    /// </summary>
    public class GaussWindow : Window
    {
        double _sigma;

        /// <summary>
        /// Create a new gauss window.
        /// </summary>
        public
        GaussWindow(
            double sigma
            )
        {
            _sigma = sigma;
        }

        /// <summary>
        /// Sigma (standard distribution of the gaussian distribution).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public double Sigma
        {
            get { return _sigma; }
            set
            {
                if (0.0 > value || 0.5 < value)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _sigma = value;
                Reset();
            }
        }

        /// <summary>
        /// Windowing function generator implementation.
        /// </summary>
        protected override double[] ComputeWindowCore(int width)
        {
            return Numerics.Window.Gauss(width, _sigma);
        }
    }
}
