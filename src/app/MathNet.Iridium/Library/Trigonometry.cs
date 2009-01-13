//-----------------------------------------------------------------------
// <copyright file="Trigonometry.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
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

namespace MathNet.Numerics
{
    /// <summary>
    /// Double-precision trigonometry toolkit.
    /// </summary>
    public static class Trig
    {
        #region Angle Conversion

        /// <summary>
        /// Converts a degree (360-periodic) angle to a radian (2*Pi-periodic) angle.
        /// </summary>
        public static
        double
        DegreeToRadian(double degree)
        {
            return degree * Constants.Degree;
        }

        /// <summary>
        /// Converts a radian (2*Pi-periodic) angle to a degree (360-periodic) angle.
        /// </summary>
        public static
        double
        RadianToDegree(double radian)
        {
            return radian / Constants.Degree;
        }

        /// <summary>
        /// Converts a newgrad (400-periodic) angle to a radian (2*Pi-periodic) angle.
        /// </summary>
        public static
        double
        GradToRadian(double newgrad)
        {
            return newgrad * Constants.Grad;
        }

        /// <summary>
        /// Converts a radian (2*Pi-periodic) angle to a newgrad (400-periodic) angle.
        /// </summary>
        public static
        double
        RadianToGrad(double radian)
        {
            return radian / Constants.Grad;
        }

        /// <summary>
        /// Converts a degree (360-periodic) angle to a newgrad (400-periodic) angle.
        /// </summary>
        public static
        double
        DegreeToGrad(double degree)
        {
            return degree / 9 * 10;
        }

        /// <summary>
        /// Converts a newgrad (400-periodic) angle to a degree (360-periodic) angle.
        /// </summary>
        public static
        double
        GradToDegree(double newgrad)
        {
            return newgrad / 10 * 9;
        }

        #endregion

        #region Trigonometric Functions

        /// <summary>
        /// Trigonometric Sine (Sinus) of an angle in radian
        /// </summary>
        public static
        double
        Sine(double radian)
        {
            return Math.Sin(radian);
        }

        /// <summary>
        /// Trigonometric Cosine (Cosinus) of an angle in radian
        /// </summary>
        public static
        double
        Cosine(double radian)
        {
            return Math.Cos(radian);
        }

        /// <summary>
        /// Trigonometric Tangent (Tangens) of an angle in radian
        /// </summary>
        public static
        double
        Tangent(double radian)
        {
            return Math.Tan(radian);
        }

        /// <summary>
        /// Trigonometric Cotangent (Cotangens) of an angle in radian
        /// </summary>
        public static
        double
        Cotangent(double radian)
        {
            return 1 / Math.Tan(radian);
        }

        /// <summary>
        /// Trigonometric Secant (Sekans) of an angle in radian
        /// </summary>
        public static
        double
        Secant(double radian)
        {
            return 1 / Math.Cos(radian);
        }

        /// <summary>
        /// Trigonometric Cosecant (Cosekans) of an angle in radian
        /// </summary>
        public static
        double
        Cosecant(double radian)
        {
            return 1 / Math.Sin(radian);
        }

        #endregion

        #region Trigonometric Inverse Functions

        /// <summary>
        /// Trigonometric Arcus Sine (Arkussinus) in radian
        /// </summary>
        public static
        double
        InverseSine(double real)
        {
            return Math.Asin(real);
        }

        /// <summary>
        /// Trigonometric Arcus Cosine (Arkuscosinus) in radian
        /// </summary>
        public static
        double
        InverseCosine(double real)
        {
            return Math.Acos(real);
        }

        /// <summary>
        /// Trigonometric Arcus Tangent (Arkustangens) in radian
        /// </summary>
        public static
        double
        InverseTangent(double real)
        {
            return Math.Atan(real);
        }

        /// <summary>
        /// The principal argument (in radian) of the complex number x+I*y
        /// </summary>
        /// <param name="nominator">y</param>
        /// <param name="denominator">x</param>
        public static
        double
        InverseTangentFromRational(
            double nominator,
            double denominator)
        {
            return Math.Atan2(nominator, denominator);
        }

        /// <summary>
        /// Trigonometric Arcus Cotangent (Arkuscotangens) in radian
        /// </summary>
        public static
        double
        InverseCotangent(double real)
        {
            return Math.Atan(1 / real);
        }

        /// <summary>
        /// Trigonometric Arcus Secant (Arkussekans) in radian
        /// </summary>
        public static
        double
        InverseSecant(double real)
        {
            return Math.Acos(1 / real);
        }

        /// <summary>
        /// Trigonometric Arcus Cosecant (Arkuscosekans) in radian
        /// </summary>
        public static
        double
        InverseCosecant(double real)
        {
            return Math.Asin(1 / real);
        }

        #endregion

        #region Hyperbolic Functions

        /// <summary>
        /// Trigonometric Hyperbolic Sine (Sinus hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicSine(double radian)
        {
            // NOT SUPPORTED BY COMPACT FRAMEWORK!!???
            // return Math.Sinh(x)
            return (Math.Exp(radian) - Math.Exp(-radian)) / 2;
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosine (Cosinus hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicCosine(double radian)
        {
            // NOT SUPPORTED BY COMPACT FRAMEWORK!!???
            // return Math.Cosh(x);
            return (Math.Exp(radian) + Math.Exp(-radian)) / 2;
        }

        /// <summary>
        /// Trigonometric Hyperbolic Tangent (Tangens hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicTangent(double radian)
        {
            // NOT SUPPORTED BY COMPACT FRAMEWORK!!???
            // return Math.Tanh(x);
            double e1 = Math.Exp(radian);
            double e2 = Math.Exp(-radian);
            return (e1 - e2) / (e1 + e2);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cotangent (Cotangens hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicCotangent(double radian)
        {
            // NOT SUPPORTED BY COMPACT FRAMEWORK!!???
            // return 1/Math.Tanh(x);
            double e1 = Math.Exp(radian);
            double e2 = Math.Exp(-radian);
            return (e1 + e2) / (e1 - e2);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Secant (Sekans hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicSecant(double radian)
        {
            return 1 / HyperbolicCosine(radian);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosecant (Cosekans hyperbolicus)
        /// </summary>
        public static
        double
        HyperbolicCosecant(double radian)
        {
            return 1 / HyperbolicSine(radian);
        }

        #endregion

        #region Hyperbolic Area Functions

        /// <summary>
        /// Trigonometric Hyperbolic Area Sine (Areasinus hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicSine(double real)
        {
            return Math.Log(
                real + Math.Sqrt((real * real) + 1),
                Math.E);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosine (Areacosinus hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicCosine(double real)
        {
            return Math.Log(
                real + (Math.Sqrt(real - 1) * Math.Sqrt(real + 1)),
                Math.E);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Tangent (Areatangens hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicTangent(double real)
        {
            return 0.5 * Math.Log(
                (1 + real) / (1 - real),
                Math.E);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cotangent (Areacotangens hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicCotangent(double real)
        {
            return 0.5 * Math.Log(
                (real + 1) / (real - 1),
                Math.E);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Secant (Areasekans hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicSecant(double real)
        {
            return InverseHyperbolicCosine(1 / real);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosecant (Areacosekans hyperbolicus)
        /// </summary>
        public static
        double
        InverseHyperbolicCosecant(double real)
        {
            return InverseHyperbolicSine(1 / real);
        }

        #endregion
    }
}
