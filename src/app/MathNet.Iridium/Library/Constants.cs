#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2007, Christoph Rüegg,  http://christoph.ruegg.name
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
    /// Mathematical Constants
    /// </summary>
    /// <seealso cref="SiConstants"/>
    /// <seealso cref="SiPrefixes"/>
    public static class Constants
    {
        /// <summary>e</summary>
        public const double E = 2.7182818284590452353602874713526624977572470937000d;
        /// <summary>log[2](e)</summary>
        public const double Log2E = 1.4426950408889634073599246810018921374266459541530d;
        /// <summary>log[10](e)</summary>
        public const double Log10E = 0.43429448190325182765112891891660508229439700580366d;
        /// <summary>log[e](2)</summary>
        public const double Ln2 = 0.69314718055994530941723212145817656807550013436026d;
        /// <summary>log[e](10)</summary>
        public const double Ln10 = 2.3025850929940456840179914546843642076011014886288d;
        /// <summary>log[e](pi)</summary>
        public const double LnPi = 1.1447298858494001741434273513530587116472948129153d;

        /// <summary>1/e</summary>
        public const double InvE = 0.36787944117144232159552377016146086744581113103176d;
        /// <summary>sqrt(e)</summary>
        public const double SqrtE = 1.6487212707001281468486507878141635716537761007101d;

        /// <summary>sqrt(2)</summary>
        public const double Sqrt2 = 1.4142135623730950488016887242096980785696718753769d;
        /// <summary>sqrt(1/2) = 1/sqrt(2) = sqrt(2)/2</summary>
        public const double Sqrt1_2 = 0.70710678118654752440084436210484903928483593768845d;
        /// <summary>sqrt(3)/2</summary>
        public const double HalfSqrt3 = 0.86602540378443864676372317075293618347140262690520d;

        /// <summary>pi</summary>
        public const double Pi = 3.1415926535897932384626433832795028841971693993751d;
        /// <summary>pi/2</summary>
        public const double Pi_2 = 1.5707963267948966192313216916397514420985846996876d;
        /// <summary>pi/4</summary>
        public const double Pi_4 = 0.78539816339744830961566084581987572104929234984378d;
        /// <summary>sqrt(pi)</summary>
        public const double SqrtPi = 1.7724538509055160272981674833411451827975494561224d;
        /// <summary>sqrt(2pi)</summary>
        public const double Sqrt2Pi = 2.5066282746310005024157652848110452530069867406099d;

        /// <summary>1/pi</summary>
        public const double InvPi = 0.31830988618379067153776752674502872406891929148091d;
        /// <summary>2/pi</summary>
        public const double TwoInvPi = 0.63661977236758134307553505349005744813783858296182d;
        /// <summary>1/sqrt(pi)</summary>
        public const double InvSqrtPi = 0.56418958354775628694807945156077258584405062932899d;
        /// <summary>1/sqrt(2pi)</summary>
        public const double InvSqrt2Pi = 0.39894228040143267793994605993438186847585863116492d;
        /// <summary>2/sqrt(pi)</summary>
        public const double TwoInvSqrtPi = 1.1283791670955125738961589031215451716881012586580d;

        /// <summary>(pi)/180</summary>
        public const double Degree = 0.0174532925199432957692369076848861271344287188854172545609719144017100911460344944368d;
        /// <summary>(pi)/200</summary>
        public const double NewDegree = Pi / 200d;

        /// <summary>Catalan constant</summary>
        /// <remarks>Sum(k=0 -> inf){ (-1)^k/(2*k + 1)2 }</remarks>
        public const double Catalan = 0.9159655941772190150546035149323841107741493742816721342664981196217630197762547694794d;
        /// <summary>The Euler-Mascheroni constant</summary>
        /// <remarks>lim(n -> inf){ Sum(k=1 -> n) { 1/k - log(n) } }</remarks>
        public const double EulerGamma = 0.5772156649015328606065120900824024310421593359399235988057672348849d;
        /// <summary>(1+sqrt(5))/2</summary>
        public const double GoldenRatio = 1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072d;
        /// <summary>Glaisher Constant</summary>
        /// <remarks>e^(1/12 - Zeta(-1))</remarks>
        public const double Glaisher = 1.2824271291006226368753425688697917277676889273250011920637400217404063088588264611297d;
        /// <summary>Khinchin constant</summary>
        /// <remarks>prod(k=1 -> inf){1+1/(k*(k+2))^log(k,2)}</remarks>
        public const double Khinchin = 2.6854520010653064453097148354817956938203822939944629530511523455572188595371520028011d;
    }
}
