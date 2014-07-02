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

namespace MathNet.SignalProcessing.Filter
{
    /// <summary>
    /// Specifies how a filter will respond to an impulse input.
    /// </summary>
    public enum ImpulseResponse
    {
        /// <summary>
        /// Impulse response always has a finite length of time and are stable, but usually have a long delay.
        /// </summary>
        Finite,
        /// <summary>
        /// Impulse response may have an infinite length of time and may be unstable, but usually have only a short delay.
        /// </summary>
        Infinite
    }
}
