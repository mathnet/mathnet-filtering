#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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
using System.Runtime.InteropServices;

namespace MathNet.Numerics
{
    /// <summary>
    /// Scientific Unit prefix factors.
    /// </summary>
    /// <seealso cref="SiConstants"/>
    /// <seealso cref="Constants"/>
    [ComVisible(true)]
    public static class SiPrefixes
    {
        /// <summary>1 000 000 000 000 000 000 000 000</summary>
        public const double Yotta = 1e24;
        /// <summary>1 000 000 000 000 000 000 000</summary>
        public const double Zetta = 1e21;
        /// <summary>1 000 000 000 000 000 000</summary>
        public const double Exa = 1e18;
        /// <summary>1 000 000 000 000 000</summary>
        public const double Peta = 1e15;
        /// <summary>1 000 000 000 000</summary>
        public const double Tera = 1e12;
        /// <summary>1 000 000 000</summary>
        public const double Giga = 1e9;
        /// <summary>1 000 000</summary>
        public const double Mega = 1e6;
        /// <summary>1 000</summary>
        public const double Kilo = 1e3;
        /// <summary>100</summary>
        public const double Hecto = 1e2;
        /// <summary>10</summary>
        public const double Deca = 1e1;
        /// <summary>0.1</summary>
        public const double Deci = 1e-1;
        /// <summary>0.01</summary>
        public const double Centi = 1e-2;
        /// <summary>0.001</summary>
        public const double Milli = 1e-3;
        /// <summary>0.000 001</summary>
        public const double Micro = 1e-6;
        /// <summary>0.000 000 001</summary>
        public const double Nano = 1e-9;
        /// <summary>0.000 000 000 001</summary>
        public const double Pico = 1e-12;
        /// <summary>0.000 000 000 000 001</summary>
        public const double Femto = 1e-15;
        /// <summary>0.000 000 000 000 000 001</summary>
        public const double Atto = 1e-18;
        /// <summary>0.000 000 000 000 000 000 001</summary>
        public const double Zepto = 1e-21;
        /// <summary>0.000 000 000 000 000 000 000 001</summary>
        public const double Yocto = 1e-24;
    }
}
