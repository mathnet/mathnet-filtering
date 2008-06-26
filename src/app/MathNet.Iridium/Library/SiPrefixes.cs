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

namespace MathNet.Numerics
{
    /// <summary>
    /// Scientific Unit prefix factors.
    /// </summary>
    /// <seealso cref="SiConstants"/>
    /// <seealso cref="Constants"/>
    public static
    class SiPrefixes
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

        /////// <summary>
        /////// Convert an SI magnitude reference to the common character.
        /////// </summary>
        ////static
        ////char
        ////PrefixToCharacter(
        ////    SiPrefix prefix
        ////    )
        ////{
        ////    switch(prefix)
        ////    {
        ////        case SiPrefix.Yotta:
        ////            return 'Y';
        ////        case SiPrefix.Zetta:
        ////            return 'Z';
        ////        case SiPrefix.Exa:
        ////            return 'E';
        ////        case SiPrefix.Peta:
        ////            return 'P';
        ////        case SiPrefix.Tera:
        ////            return 'T';
        ////        case SiPrefix.Giga:
        ////            return 'G';
        ////        case SiPrefix.Mega:
        ////            return 'M';
        ////        case SiPrefix.Kilo:
        ////            return 'k';
        ////        case SiPrefix.Hecto:
        ////            return 'H';
        ////        case SiPrefix.Deca:
        ////            return 'D';
        ////        case SiPrefix.Base:
        ////            return ' ';
        ////        case SiPrefix.Deci:
        ////            return 'd';
        ////        case SiPrefix.Centi:
        ////            return 'c';
        ////        case SiPrefix.Milli:
        ////            return 'm';
        ////        case SiPrefix.Micro:
        ////            return 'u';
        ////        case SiPrefix.Nano:
        ////            return 'n';
        ////        case SiPrefix.Pico:
        ////            return 'p';
        ////        case SiPrefix.Femto:
        ////            return 'f';
        ////        case SiPrefix.Atto:
        ////            return 'a';
        ////        case SiPrefix.Zepto:
        ////            return 'z';
        ////        case SiPrefix.Yocto:
        ////            return 'y';
        ////        default:
        ////            throw new ArgumentOutOfRangeException("prefix");
        ////    }
        ////}
    }

    /////// <summary>
    /////// SI Magnitude Reference.
    /////// </summary>
    ////public
    ////enum SiPrefix
    ////    : int
    ////{
    ////    /// <summary>10^24</summary>
    ////    Yotta = 24,
    ////    /// <summary>10^21</summary>
    ////    Zetta = 21,
    ////    /// <summary>10^18</summary>
    ////    Exa = 18,
    ////    /// <summary>10^15</summary>
    ////    Peta = 15,
    ////    /// <summary>10^12</summary>
    ////    Tera = 12,
    ////    /// <summary>10^9</summary>
    ////    Giga = 9,
    ////    /// <summary>10^6</summary>
    ////    Mega = 6,
    ////    /// <summary>10^3</summary>
    ////    Kilo = 3,
    ////    /// <summary>10^2</summary>
    ////    Hecto = 2,
    ////    /// <summary>10^1</summary>
    ////    Deca = 1,
    ////    /// <summary>10^0</summary>
    ////    Base = 0,
    ////    /// <summary>10^(-1)</summary>
    ////    Deci = -1,
    ////    /// <summary>10^(-2)</summary>
    ////    Centi = -2,
    ////    /// <summary>10^(-3)</summary>
    ////    Milli = -3,
    ////    /// <summary>10^(-6)</summary>
    ////    Micro = -6,
    ////    /// <summary>10^(-9)</summary>
    ////    Nano = -9,
    ////    /// <summary>10^(-12)</summary>
    ////    Pico = -12,
    ////    /// <summary>10^(-15)</summary>
    ////    Femto = -15,
    ////    /// <summary>10^(-18)</summary>
    ////    Atto = -18,
    ////    /// <summary>10^(-21)</summary>
    ////    Zepto = -21,
    ////    /// <summary>10^(-24)</summary>
    ////    Yocto = -24
    ////}
}
