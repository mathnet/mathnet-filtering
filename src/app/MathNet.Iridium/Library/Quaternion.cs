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
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>Quaternion Number.</summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Quaternion
    /// http://de.wikipedia.org/wiki/Quaternion
    /// </remarks>
    public struct Quaternion :
        IComparable,
        ICloneable
    {

        // TODO: testing suite for Quaternion

        readonly double qw; // real part
        readonly double qx, qy, qz; // imaginary part
        readonly double qabs, qnorm; // norm
        readonly double qarg; // polar notation

        /// <summary>
        /// Create a quarternion.
        /// </summary>
        public
        Quaternion(
            double real,
            double imagX,
            double imagY,
            double imagZ
            )
        {
            qx = imagX;
            qy = imagY;
            qz = imagZ;
            qw = real;
            qnorm = ToNorm(real, imagX, imagY, imagZ);
            qabs = Math.Sqrt(qnorm);
            qarg = Math.Acos(real / qabs);
        }

        /// <summary>
        /// Create a quarternion.
        /// </summary>
        internal
        Quaternion(
            double real,
            double imagX,
            double imagY,
            double imagZ,
            double abs,
            double norm,
            double arg
            )
        {
            qx = imagX;
            qy = imagY;
            qz = imagZ;
            qw = real;
            qnorm = norm;
            qabs = abs;
            qarg = arg;
        }

        static
        double
        ToNorm(
            double real,
            double imagX,
            double imagY,
            double imagZ
            )
        {
            return imagX * imagX
                + imagY * imagY
                + imagZ * imagZ
                + real * real;
        }
        
        static
        Quaternion
        ToUnitQuaternion(
            double real,
            double imagX,
            double imagY, 
            double imagZ
            )
        {
            double abs = Math.Sqrt(ToNorm(real, imagX, imagY, imagZ));
            return new Quaternion(
                real / abs,
                imagX / abs,
                imagY / abs,
                imagZ / abs,
                1, // abs
                1, // norm
                Math.Acos(real / abs) // arg
                );
        }

        #region Accessors

        /// <summary>Gets the real part of the quaternion.</summary>
        public double Real
        {
            get { return qw; }
        }

        /// <summary>Gets the imaginary X part (coefficient of complex I) of the quaternion.</summary>
        public double ImagX
        {
            get { return qx; }
        }

        /// <summary>Gets the imaginary Y part (coefficient of complex J) of the quaternion.</summary>
        public double ImagY
        {
            get { return qy; }
        }

        /// <summary>Gets the imaginary Z part (coefficient of complex K) of the quaternion.</summary>
        public double ImagZ
        {
            get { return qz; }
        }

        /// <summary>G
        /// ets the standard euclidean length |q| = sqrt(||q||) of the quaternion q: the square root of the sum of the squares of the four components.
        /// Q may then be represented as q = r*(cos(phi) + u * sin(phi)) = r*exp(phi*u) where u is the unit vector and phi the argument of q.
        /// </summary>
        public double Abs
        {
            get { return qabs; }
        }

        /// <summary>Gets the norm ||q|| = |q|^2 of the quaternion q: the sum of the squares of the four components.</summary>
        public double Norm
        {
            get { return qnorm; }
        }

        /// <summary>Gets the argument phi = arg(q) of the quaternion q, such that q = r*(cos(phi) + u * sin(phi)) = r*exp(phi*u) where r is the absolute and u the unit vector of q.</summary>
        public double Arg
        {
            get { return qarg; }
        }

        /// <summary>True if the quaternion q is of lenght |q| = 1.</summary>
        /// <remarks>To normalize a quaternion to a length of 1, use the <see cref="Sign"/> method. All unit quaternions form a 3-sphere.</remarks>
        public bool IsUnitQuaternion
        {
            get { return Number.AlmostEqual(qabs, 1); }
        }

        /// <summary>
        /// Returns a new Quaternion q with the Scalar part only.
        /// If you need a Double, use the Real-Field instead.
        /// </summary>
        public
        Quaternion
        Scalar()
        {
            return new Quaternion(qw, 0, 0, 0);
        }

        /// <summary>
        /// Returns a new Quaternion q with the Vectorpart only.
        /// </summary>
        public
        Quaternion
        Vector()
        {
            return new Quaternion(0, qx, qy, qz);
        }

        /// <summary>
        /// Returns a new normalized Quaternion u with the Vectorpart only, such that ||u|| = 1.
        /// Q may then be represented as q = r*(cos(phi) + u * sin(phi)) = r*exp(phi*u) where r is the absolute and phi the argument of q.
        /// </summary>
        public
        Quaternion
        UnitVector()
        {
            return ToUnitQuaternion(0, qx, qy, qz);
        }

        /// <summary>
        /// Returns a new normalized Quaternion q with the direction of this quaternion.
        /// </summary>
        public
        Quaternion
        Sign()
        {
            return ToUnitQuaternion(qw, qx, qy, qz);
        }

        /////// <summary>
        /////// Returns a new Quaternion q with the Sign of the components.
        /////// </summary>
        /////// <returns>
        /////// <list type="bullet">
        /////// <item>1 if Positive</item>
        /////// <item>0 if Neutral</item>
        /////// <item>-1 if Negative</item>
        /////// </list>
        /////// </returns>
        ////public Quaternion ComponentSigns()
        ////{
        ////    return new Quaternion(
        ////        Math.Sign(qx),
        ////        Math.Sign(qy),
        ////        Math.Sign(qz),
        ////        Math.Sign(qw));
        ////}

        #endregion

        #region Operators

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        Quaternion
        operator +(
            Quaternion q
            )
        {
            return q;
        }

        /// <summary>
        /// Negate a quaternion.
        /// </summary>
        public static
        Quaternion
        operator -(
            Quaternion q
            )
        {
            return q.Negate();
        }

        /// <summary>
        /// Add a quaternion to a quaternion.
        /// </summary>
        public static
        Quaternion
        operator +(
            Quaternion q1,
            Quaternion q2
            )
        {
            return q1.Add(q2);
        }

        /// <summary>
        /// Add a floating point number to a quaternion.
        /// </summary>
        public static
        Quaternion
        operator +(
            Quaternion q1,
            double d
            )
        {
            return q1.Add(d);
        }

        /// <summary>
        /// Subtract a quaternion from a quaternion.
        /// </summary>
        public static
        Quaternion
        operator -(
            Quaternion q1,
            Quaternion q2
            )
        {
            return q1.Subtract(q2);
        }

        /// <summary>
        /// Subtract a floating point number from a quaternion.
        /// </summary>
        public static
        Quaternion
        operator -(
            Quaternion q1,
            double d
            )
        {
            return q1.Subtract(d);
        }

        /// <summary>
        /// Multiplay a quaternion with a quaternion.
        /// </summary>
        public static
        Quaternion
        operator *(
            Quaternion q1,
            Quaternion q2
            )
        {
            return q1.Multiply(q2);
        }

        /// <summary>
        /// Multiplay a floating point number with a quaternion.
        /// </summary>
        public static
        Quaternion
        operator *(
            Quaternion q1,
            double d
            )
        {
            return q1.Multiply(d);
        }

        /// <summary>
        /// Divide a quaternion by a quaternion.
        /// </summary>
        public static
        Quaternion
        operator /(
            Quaternion q1,
            Quaternion q2
            )
        {
            return q1.Divide(q2);
        }

        /// <summary>
        /// Divide a quaternion by a floating point number.
        /// </summary>
        public static
        Quaternion
        operator /(
            Quaternion q1,
            double d
            )
        {
            return q1.Divide(d);
        }

        /// <summary>
        /// Raise a quaternion to a quaternion.
        /// </summary>
        public static
        Quaternion
        operator ^(
            Quaternion q1,
            Quaternion q2
            )
        {
            return q1.Pow(q2);
        }

        /// <summary>
        /// Raise a quaternion to a floating point number.
        /// </summary>
        public static
        Quaternion
        operator ^(
            Quaternion q1,
            double d
            )
        {
            return q1.Pow(d);
        }

        /// <summary>
        /// Convert a floating point number to a quaternion.
        /// </summary>
        public static implicit
        operator Quaternion(
            double d
            )
        {
            return new Quaternion(d, 0, 0, 0);
        }

        #endregion

        #region Arithmetic Methods
        
        /// <summary>
        /// Add a quaternion to this quaternion.
        /// </summary>
        public
        Quaternion
        Add(
            Quaternion q
            )
        {
            return new Quaternion(
                qw + q.qw,
                qx + q.qx,
                qy + q.qy,
                qz + q.qz
                );
        }

        /// <summary>
        /// Add a floating point number to this quaternion.
        /// </summary>
        public
        Quaternion
        Add(
            double r
            )
        {
            return new Quaternion(
                qw + r,
                qx,
                qy,
                qz
                );
        }

        /// <summary>
        /// SUbtract a quaternion from this quaternion.
        /// </summary>
        public
        Quaternion
        Subtract(
            Quaternion q
            )
        {
            return new Quaternion(
                qw - q.qw,
                qx - q.qx,
                qy - q.qy,
                qz - q.qz
                );
        }

        /// <summary>
        /// Subtract a floating point number from this quaternion.
        /// </summary>
        public
        Quaternion
        Subtract(
            double r
            )
        {
            return new Quaternion(
                qw - r,
                qx,
                qy,
                qz
                );
        }

        /// <summary>
        /// Negate this quaternion.
        /// </summary>
        public
        Quaternion
        Negate()
        {
            return new Quaternion(
                -qw,
                -qx,
                -qy,
                -qz,
                qabs, // abs
                qnorm, // norm
                Math.PI - qarg // arg
                );
        }


        /// <summary>
        /// Multiply a quaternion with this quaternion.
        /// </summary>
        public
        Quaternion
        Multiply(
            Quaternion q
            )
        {
            double ci = +qx * q.qw + qy * q.qz - qz * q.qy + qw * q.qx;
            double cj = -qx * q.qz + qy * q.qw + qz * q.qx + qw * q.qy;
            double ck = +qx * q.qy - qy * q.qx + qz * q.qw + qw * q.qz;
            double cr = -qx * q.qx - qy * q.qy - qz * q.qz + qw * q.qw;
            return new Quaternion(cr, ci, cj, ck);
        }

        /// <summary>
        /// Multiply a floating point number to this quaternion.
        /// </summary>
        public
        Quaternion
        Multiply(
            double d
            )
        {
            return new Quaternion(
                d * qw,
                d * qx,
                d * qy,
                d * qz
                );
        }

        /// <summary>
        /// Multiplies a Quaternion with the inverse of another
        /// Quaternion (q*q<sup>-1</sup>). Note that for Quaternions
        /// q*q<sup>-1</sup> is not the same then q<sup>-1</sup>*q,
        /// because this will lead to a rotation in the other direction.
        /// </summary>
        public
        Quaternion
        Divide(
            Quaternion q
            )
        {
            return Multiply(q.Inverse());
        }

        /// <summary>
        /// Multiplies a Quaterion with the inverse of a real number.
        /// </summary>
        /// <remarks>
        /// Its also Possible to cast a double to a Quaternion
        /// and make the division afterwards. But this is less
        /// performant.
        /// </remarks>
        public
        Quaternion
        Divide(
            double d
            )
        {
            return new Quaternion(
                qw / d,
                qx / d,
                qy / d,
                qz / d
                );
        }

        /// <summary>
        /// Inverts this quaternion.
        /// </summary>
        public
        Quaternion
        Inverse()
        {
            if(Number.AlmostEqual(qabs, 1))
            {
                return new Quaternion(
                    qw,
                    -qx,
                    -qy,
                    -qz
                    );
            }
            
            return new Quaternion(
                qw / qnorm,
                -qx / qnorm,
                -qy / qnorm,
                -qz / qnorm
                );
        }

        /// <summary>
        /// Returns the distance |a-b| of two quaternions, forming a metric space.
        /// </summary>
        public static
        double
        Distance(
            Quaternion a,
            Quaternion b
            )
        {
            return a.Subtract(b).Abs;
        }

        /// <summary>
        /// Conjugate this quaternion.
        /// </summary>
        public
        Quaternion
        Conjugate()
        {
            return new Quaternion(
                qw,
                -qx,
                -qy,
                -qz,
                qabs,
                qnorm,
                qarg
                );
        }
        
        #endregion

        #region Exponential

        /// <summary>
        /// Logarithm to a given base.
        /// </summary>
        public
        Quaternion
        Log(
            double lbase
            )
        {
            return Ln().Divide(Math.Log(lbase));
        }

        /// <summary>
        /// Natural Logrithm to base E.
        /// </summary>
        public
        Quaternion
        Ln()
        {
            return UnitVector().Multiply(qarg).Add(Math.Log(qabs));
        }

        /// <summary>
        /// Common Logarithm to base 10.
        /// </summary>
        public
        Quaternion
        Lg()
        {
            return Ln().Divide(Math.Log(10));
        }

        /// <summary>
        /// Exponential Function.
        /// </summary>
        /// <returns></returns>
        public
        Quaternion
        Exp()
        {
            double vabs = Math.Sqrt(ToNorm(0, qx, qy, qz));
            return UnitVector().Multiply(Math.Sin(vabs)).Add(Math.Cos(vabs)).Multiply(Math.Exp(qw));
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public
        Quaternion
        Pow(
            double power
            )
        {
            double arg = power * qarg;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(Math.Pow(qw, power));
        }

        /// <summary>
        /// Raise the quaternion to a given power.
        /// </summary>
        public
        Quaternion
        Pow(
            Quaternion power
            )
        {
            return power.Multiply(Ln()).Exp();
        }

        /// <summary>
        /// Square of the Quaternion q: q^2.
        /// </summary>
        public
        Quaternion
        Sqr()
        {
            double arg = qarg * 2;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(qw * qw);
        }

        /// <summary>
        /// Square root of the Quaternion: q^(1/2).
        /// </summary>
        public
        Quaternion
        Sqrt()
        {
            double arg = qarg * 0.5;
            return UnitVector().Multiply(Math.Sin(arg)).Add(Math.Cos(arg)).Multiply(Math.Sqrt(qw));
        }

        #endregion

        #region Trigonometric

        // TODO: Implement

        #endregion

        #region String Formatting and Parsing

        // TODO: Implement

        #endregion

        #region .NET Integration: Hashing, Equality, Ordering, Cloning

        /// <summary>
        /// Compares this quaternion with another quaternion.
        /// </summary>
        public
        int
        CompareTo(
            object obj
            )
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a copy of this quaternion.
        /// </summary>
        public
        object
        Clone()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        #endregion
    }
}
