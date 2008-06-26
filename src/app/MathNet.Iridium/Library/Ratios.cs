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
    /// Neper/Decibel ratio expression toolkit.
    /// </summary>
    /// <remarks>
    /// <p>See <a href="http://en.wikipedia.org/wiki/Neper">Wikipedia - Neper</a>
    /// and <a href="http://en.wikipedia.org/wiki/Decibel">Wikipedia - Decibel</a></p>
    /// </remarks>
    public static class Ratios
    {
        #region Neper (Np)

        /// <summary>
        /// Given a ratio, express it in Neper (Np).
        /// </summary>
        public static
        double
        RatioToNeper(
            double ratio
            )
        {
            return Math.Log(ratio);
        }

        /// <summary>
        /// Given an effective value and a fixed base value, express the ratio in Neper (Np).
        /// </summary>
        public static
        double
        RatioToNeper(
            double value,
            double baseValue
            )
        {
            return Math.Log(value) - Math.Log(baseValue);
        }

        /// <summary>
        /// Given a ratio in Neper (Np), evaluate the effective ratio.
        /// </summary>
        public static
        double
        NeperToRatio(
            double neper
            )
        {
            return Math.Exp(neper);
        }

        /// <summary>
        /// Given a ratio in Neper (Np) and the fixed base value, evaluate the effective value.
        /// </summary>
        public static
        double
        NeperToValue(
            double neper,
            double baseValue
            )
        {
            return Math.Exp(neper) * baseValue;
        }

        #endregion

        #region Neutral Decibel (dB)

        /// <summary>
        /// Given a ratio, express it in Decibel (dB).
        /// </summary>
        public static
        double
        RatioToDecibel(
            double ratio
            )
        {
            return 10 * Math.Log10(ratio);
        }

        /// <summary>
        /// Given an effective value and a fixed base value, express the ratio in Decibel (dB).
        /// </summary>
        public static
        double
        RatioToDecibel(
            double value,
            double baseValue
            )
        {
            return 10 * (Math.Log10(value) - Math.Log10(baseValue));
        }

        /// <summary>
        /// Given a ratio in Decibel (dB), evaluate the effective ratio.
        /// </summary>
        public static
        double
        DecibelToRatio(
            double decibel
            )
        {
            return Math.Pow(10, 0.1 * decibel);
        }

        /// <summary>
        /// Given a ratio in Decibel (dB) and the fixed base value, evaluate the effective value.
        /// </summary>
        public static
        double
        DecibelToValue(
            double decibel,
            double baseValue
            )
        {
            return Math.Pow(10, 0.1 * decibel) * baseValue;
        }

        #endregion

        #region Power Decibel (dB)

        /// <summary>
        /// Given a ratio, express it in Decibel (dB, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts).
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        RatioToPowerDecibel(
            double ratio
            )
        {
            return 20 * Math.Log10(ratio);
        }

        /// <summary>
        /// Given an effective value and a fixed base value, express the ratio in Decibel (dB, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts).
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        RatioToPowerDecibel(
            double value,
            double baseValue
            )
        {
            return 20 * (Math.Log10(value) - Math.Log10(baseValue));
        }

        /// <summary>
        /// Given a ratio in Decibel (dB, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts), evaluate the effective ratio.
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        PowerDecibelToRatio(
            double decibel
            )
        {
            return Math.Pow(10, 0.05 * decibel);
        }

        /// <summary>
        /// Given a ratio in Decibel (dB, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts) and the fixed base value, evaluate the effective value.
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        PowerDecibelToValue(
            double decibel,
            double baseValue
            )
        {
            return Math.Pow(10, 0.05 * decibel) * baseValue;
        }

        #endregion

        #region Neper <-> Decibel Conversion

        /// <summary>
        /// Convert a ratio in Decibel (dB, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts) to Neper (Np).
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        PowerDecibelToNeper(
            double decibel
            )
        {
            return decibel * Constants.PowerDecibel;
        }

        /// <summary>
        /// Convert a ratio in Neper (Np, representing a power gain, while the compared values are not powers, e.g. amplitude, ampère, volts) to Decibel (dB).
        /// </summary>
        /// <remarks>
        /// Power Decibel indicates that the method takes care of squaring the compared values to get a power gain mesure.
        /// </remarks>
        public static
        double
        NeperToPowerDecibel(
            double neper
            )
        {
            return neper / Constants.PowerDecibel;
        }

        /// <summary>
        /// Converts a ratio in Decibel (dB, not representing a power gain, or both compared values are already powers) to Neper (Np).
        /// </summary>
        public static
        double
        DecibelToNeper(
            double decibel
            )
        {
            return decibel * Constants.NeutralDecibel;
        }

        /// <summary>
        /// Converts a ration in Neper (Np, not representing a power gain, or both compared values are already powers) to Decibel (dB).
        /// </summary>
        public static
        double
        NeperToDecibel(
            double neper
            )
        {
            return neper / Constants.NeutralDecibel;
        }

        #endregion
    }
}
