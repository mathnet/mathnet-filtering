#region Math.NET Iridium (LGPL) by Ruegg, Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//                          Joannes Vermorel, http://www.vermorel.com
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
using System.Globalization;  
using System.Collections;

using MathNet.Numerics.Properties;
using MathNet.Numerics.Distributions;

namespace MathNet.Numerics
{
    /// <summary>
    /// Complex numbers class.
    /// </summary>
    /// <remarks>
    /// <p>The class <c>Complex</c> provides all elementary operations
    /// on complex numbers. All the operators <c>+</c>, <c>-</c>,
    /// <c>*</c>, <c>/</c>, <c>==</c>, <c>!=</c> are defined in the
    /// canonical way. Additional complex trigonometric functions such 
    /// as <see cref="Complex.Cosine"/>, ... 
    /// are also provided. Note that the <c>Complex</c> structures 
    /// has two special constant values <see cref="Complex.NaN"/> and 
    /// <see cref="Complex.Infinity"/>.</p>
    /// 
    /// <p>In order to avoid possible ambiguities resulting from a 
    /// <c>Complex(double, double)</c> constructor, the static methods 
    /// <see cref="Complex.FromRealImaginary"/> and <see cref="Complex.FromModulusArgument"/>
    /// are provided instead.</p>
    /// 
    /// <code>
    /// Complex x = Complex.FromRealImaginary(1d, 2d);
    /// Complex y = Complex.FromModulusArgument(1d, Math.Pi);
    /// Complex z = (x + y) / (x - y);
    /// </code>
    /// 
    /// <p>Since there is no canonical order among the complex numbers,
    /// <c>Complex</c> does not implement <c>IComparable</c> but several
    /// lexicographic <c>IComparer</c> implementations are provided, see 
    /// <see cref="Complex.RealImaginaryComparer"/>,
    /// <see cref="Complex.ModulusArgumentComparer"/> and
    /// <see cref="Complex.ArgumentModulusComparer"/>.</p>
    /// 
    /// <p>For mathematical details about complex numbers, please
    /// have a look at the <a href="http://en.wikipedia.org/wiki/Complex_number">
    /// Wikipedia</a></p>
    /// </remarks>
    public struct Complex : IEquatable<Complex>, IComparable<Complex>
    {
        #region Complex comparers

        private sealed class RealImaginaryLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ModulusArgumentLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ArgumentModulusLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private static IComparer realImaginaryComparer;
        private static IComparer modulusArgumentComparer;
        private static IComparer argumentModulusComparer;

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(real, imaginary)</c>. 
        /// </summary>
        public static IComparer RealImaginaryComparer
        {
            get
            {
                if(realImaginaryComparer == null)
                {
                    realImaginaryComparer = new RealImaginaryLexComparer();
                }

                return realImaginaryComparer;
            }
        }

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(modulus, argument)</c>.
        /// </summary>
        public static IComparer ModulusArgumentComparer
        {
            get
            {
                if(modulusArgumentComparer == null)
                {
                    modulusArgumentComparer = new ModulusArgumentLexComparer();
                }

                return modulusArgumentComparer;
            }
        }

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(argument, modulus)</c>.
        /// </summary>
        public static IComparer ArgumentModulusComparer
        {
            get
            {
                if(argumentModulusComparer == null)
                {
                    argumentModulusComparer = new ArgumentModulusLexComparer();
                }

                return argumentModulusComparer;
            }
        }

        #endregion

        double real;
        double imag;

        #region Constructors and Constants

        /// <summary>
        /// Constructs a <c>Complex</c> from its real
        /// and imaginary parts.
        /// </summary>
        public
        Complex(
            double real,
            double imag
            )
        {
            this.real = real;
            this.imag = imag;
        }

        /// <summary>
        /// Constructs a <c>Complex</c> from its real
        /// and imaginary parts.
        /// </summary>
        public static
        Complex
        FromRealImaginary(
            double real,
            double imag
            )
        {
            return new Complex(real, imag);
        }

        /// <summary>
        /// Constructs a <c>Complex</c> from its modulus and
        /// argument.
        /// </summary>
        /// <param name="modulus">Must be non-negative.</param>
        /// <param name="argument">Real number.</param>
        public static
        Complex
        FromModulusArgument(
            double modulus,
            double argument
            )
        {
            if(modulus < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    "modulus",
                    modulus,
                    Resources.ArgumentNotNegative
                    );
            }

            return new Complex(
                modulus * Math.Cos(argument),
                modulus * Math.Sin(argument)
                );
        }

        /// <summary>
        /// Constructs a complex number with random real and imaginary value.
        /// </summary>
        /// <param name="realRandomDistribution">Continuous random distribution or source for the real part.</param>
        /// <param name="imagRandomDistribution">Continuous random distribution or source for the imaginary part.</param>
        public static
        Complex
        Random(
            IContinuousGenerator realRandomDistribution,
            IContinuousGenerator imagRandomDistribution
            )
        {
            return new Complex(
                realRandomDistribution.NextDouble(),
                imagRandomDistribution.NextDouble()
                );
        }

        /// <summary>
        /// Constructs a complex number with random real and imaginary value.
        /// </summary>
        /// <param name="randomDistribution">Continuous random distribution or source for the real and imaginary parts.</param>
        public static
        Complex
        Random(
            IContinuousGenerator randomDistribution
            )
        {
            return new Complex(
                randomDistribution.NextDouble(),
                randomDistribution.NextDouble()
                );
        }

        /// <summary>
        /// Constructs a complex number with random modulus and argument.
        /// </summary>
        /// <param name="modulusRandomDistribution">Continuous random distribution or source for the modulus.</param>
        /// <param name="argumentRandomDistribution">Continuous random distribution or source for the argument.</param>
        public static
        Complex
        RandomPolar(
            IContinuousGenerator modulusRandomDistribution,
            IContinuousGenerator argumentRandomDistribution
            )
        {
            return FromModulusArgument(
                modulusRandomDistribution.NextDouble(),
                argumentRandomDistribution.NextDouble()
                );
        }

        /// <summary>
        /// Constructs a complex number on the unit circle with random argument.
        /// </summary>
        /// <param name="argumentRandomDistribution">Continuous random distribution or source for the argument.</param>
        public static
        Complex
        RandomUnitCircle(
            IContinuousGenerator argumentRandomDistribution
            )
        {
            return FromModulusArgument(
                1d,
                argumentRandomDistribution.NextDouble()
                );
        }


        /// <summary>
        /// Represents the zero value. This field is constant.
        /// </summary>
        public static Complex Zero
        {
            get { return new Complex(0d, 0d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is zero.
        /// </summary>
        public bool IsZero
        {
            get { return Number.AlmostZero(real) && Number.AlmostZero(imag); }
        }

        /// <summary>
        /// Represents the <c>1</c> value. This field is constant.
        /// </summary>
        public static Complex One
        {
            get { return new Complex(1d, 0d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is one.
        /// </summary>
        public bool IsOne
        {
            get { return Number.AlmostEqual(real, 1) && Number.AlmostZero(imag); }
        }

        /// <summary>
        /// Represents the imaginary unit number. This field is constant.
        /// </summary>
        public static Complex I
        {
            get { return new Complex(0d, 1d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is the imaginary unit.
        /// </summary>
        public bool IsI
        {
            get { return Number.AlmostZero(real) && Number.AlmostEqual(imag, 1); }
        }

        /// <summary>
        /// Represents a value that is not a number. This field is constant.
        /// </summary>
        public static Complex NaN
        {
            get { return new Complex(double.NaN, double.NaN); }
        }

        /// <summary>
        /// Indicates whether the provided <c>Complex</c> evaluates to a
        /// value that is not a number.
        /// </summary>
        public bool IsNaN
        {
            get { return double.IsNaN(real) || double.IsNaN(imag); }
        }

        /// <summary>
        /// Represents the infinity value. This field is constant.
        /// </summary>
        /// <remarks>
        /// The semantic associated to this value is a <c>Complex</c> of 
        /// infinite real and imaginary part. If you need more formal complex
        /// number handling (according to the Riemann Sphere and the extended
        /// complex plane C*, or using directed infinity) please check out the
        /// alternative MathNet.PreciseNumerics and MathNet.Symbolics packages
        /// instead.
        /// </remarks>
        public static Complex Infinity
        {
            get { return new Complex(double.PositiveInfinity, double.PositiveInfinity); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> evaluates to an
        /// infinite value.
        /// </summary>
        /// <remarks>
        /// True if it either evaluates to a complex infinity
        /// or to a directed infinity.
        /// </remarks>
        public bool IsInfinity
        {
            get { return double.IsInfinity(real) || double.IsInfinity(imag); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is real.
        /// </summary>
        public bool IsReal
        {
            get { return Number.AlmostZero(imag); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is real and not negative, that is >= 0.
        /// </summary>
        public bool IsRealNonNegative
        {
            get { return Number.AlmostZero(imag) && real >= 0; }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is imaginary.
        /// </summary>
        public bool IsImaginary
        {
            get { return Number.AlmostZero(real); }
        }

        #endregion

        #region Cartesian and Polar Components

        /// <summary>
        /// Gets or sets the real part of this <c>Complex</c>.
        /// </summary>
        /// <seealso cref="Imag"/>
        public double Real
        {
            get { return real; }
            set { real = value; }
        }

        /// <summary>
        /// Gets or sets the imaginary part of this <c>Complex</c>.
        /// </summary>
        /// <seealso cref="Real"/>
        public double Imag
        {
            get { return imag; }
            set { imag = value; }
        }

        /// <summary>
        /// Gets or sets the modulus of this <c>Complex</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set a negative modulus.
        /// </exception>
        /// <remarks>
        /// If this <c>Complex</c> is zero when the modulus is set,
        /// the Complex is assumed to be positive real with an argument of zero.
        /// </remarks>
        /// <seealso cref="Argument"/>
        public double Modulus
        {
            get
            {
                return Math.Sqrt(real * real + imag * imag);
            }

            set
            {
                if(value < 0d)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        Resources.ArgumentNotNegative
                        );
                }

                if(double.IsInfinity(value))
                {
                    real = value;
                    imag = value;
                }
                else
                {
                    if(IsZero)
                    {
                        real = value;
                        imag = 0;
                    }
                    else
                    {
                        double factor = value / Math.Sqrt(real * real + imag * imag);
                        real *= factor;
                        imag *= factor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the squared modulus of this <c>Complex</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set a negative modulus.
        /// </exception>
        /// <remarks>
        /// If this <c>Complex</c> is zero when the modulus is set,
        /// the Complex is assumed to be positive real with an argument of zero.
        /// </remarks>
        /// <seealso cref="Argument"/>
        public double ModulusSquared
        {
            get
            {
                return real * real + imag * imag;
            }

            set
            {
                if(value < 0d)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        Resources.ArgumentNotNegative
                        );
                }

                if(double.IsInfinity(value))
                {
                    real = value;
                    imag = value;
                }
                else
                {
                    if(IsZero)
                    {
                        real = Math.Sqrt(value);
                        imag = 0;
                    }
                    else
                    {
                        double factor = value / (real * real + imag * imag);
                        real *= factor;
                        imag *= factor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the argument of this <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// Argument always returns a value bigger than negative Pi and
        /// smaller or equal to Pi. If this <c>Complex</c> is zero, the Complex
        /// is assumed to be positive real with an argument of zero.
        /// </remarks>
        public double Argument
        {
            get
            {
                if(IsReal && real < 0)
                {
                    return Math.PI;
                }

                if(IsRealNonNegative)
                {
                    return 0;
                }

                return Math.Atan2(imag, real);
            }

            set
            {
                double modulus = Modulus;
                real = Math.Cos(value) * modulus;
                imag = Math.Sin(value) * modulus;
            }
        }

        /// <summary>
        /// Gets the unity of this complex (same argument, but on the unit circle; exp(I*arg))
        /// </summary>
        public Complex Sign
        {
            get
            {
                if(double.IsPositiveInfinity(real) && double.IsPositiveInfinity(imag))
                {
                    return new Complex(Constants.Sqrt1_2, Constants.Sqrt1_2);
                }

                if(double.IsPositiveInfinity(real) && double.IsNegativeInfinity(imag))
                {
                    return new Complex(Constants.Sqrt1_2, -Constants.Sqrt1_2);
                }

                if(double.IsNegativeInfinity(real) && double.IsPositiveInfinity(imag))
                {
                    return new Complex(-Constants.Sqrt1_2, -Constants.Sqrt1_2);
                }

                if(double.IsNegativeInfinity(real) && double.IsNegativeInfinity(imag))
                {
                    return new Complex(-Constants.Sqrt1_2, Constants.Sqrt1_2);
                }

                // don't replace this with "Modulus"!
                double mod = Fn.Hypot(real, imag);
                if(mod == 0)
                {
                    return Complex.Zero;
                }

                return new Complex(real / mod, imag / mod);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the conjugate of this <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// The semantic of <i>setting the conjugate</i> is such that
        /// <code>
        /// // a, b of type Complex
        /// a.Conjugate = b;
        /// </code>
        /// is equivalent to
        /// <code>
        /// // a, b of type Complex
        /// a = b.Conjugate
        /// </code>
        /// </remarks>
        public Complex Conjugate
        {
            get { return new Complex(real, -imag); }
            set { this = value.Conjugate; }
        }

        #region Equality & Hashing

        /// <summary>
        /// Indicates whether <c>obj</c> is equal to this instance.
        /// </summary>
        public override
        bool
        Equals(
            object obj
            )
        {
            return (obj is Complex) && this.Equals((Complex)obj);
        }

        /// <summary>
        /// Indicates whether <c>z</c> is equal to this instance.
        /// </summary>
        public
        bool
        Equals(
            Complex other
            )
        {
            return !IsNaN
                && !other.IsNaN
                && (real == other.real)
                && (imag == other.imag);
        }

        /// <summary>
        /// Gets the hashcode of this <c>Complex</c>.
        /// </summary>
        public override
        int
        GetHashCode()
        {
            return real.GetHashCode() ^ (-imag.GetHashCode());
        }

        /// <summary>
        /// Compare this complex number with another complex number.
        /// </summary>
        /// <remarks>
        /// The complex number's modulus takes precedence over the argument.
        /// </remarks>
        /// <param name="other">The complex number to compare with.</param>
        public
        int
        CompareTo(
            Complex other
            )
        {
            int res = Modulus.CompareTo(other.Modulus);
            if(res != 0)
            {
                return res;
            }

            return Argument.CompareTo(other.Argument);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Equality test.
        /// </summary>
        public static bool operator ==(Complex complex1, Complex complex2)
        {
            return complex1.Equals(complex2);
        }

        /// <summary>
        /// Inequality test.
        /// </summary>
        public static bool operator !=(Complex complex1, Complex complex2)
        {
            return !complex1.Equals(complex2);
        }

        /// <summary>
        /// Unary addition.
        /// </summary>
        public static Complex operator +(Complex summand)
        {
            return summand;
        }

        /// <summary>
        /// Unary minus.
        /// </summary>
        public static Complex operator -(Complex subtrahend)
        {
            return new Complex(-subtrahend.real, -subtrahend.imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(Complex summand1, Complex summand2)
        {
            return new Complex(summand1.real + summand2.real, summand1.imag + summand2.imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(Complex minuend, Complex subtrahend)
        {
            return new Complex(minuend.real - subtrahend.real, minuend.imag - subtrahend.imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(Complex summand1, double summand2)
        {
            return new Complex(summand1.real + summand2, summand1.imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(Complex minuend, double subtrahend)
        {
            return new Complex(minuend.real - subtrahend, minuend.imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(double summand1, Complex summand2)
        {
            return new Complex(summand2.real + summand1, summand2.imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(double minuend, Complex subtrahend)
        {
            return new Complex(minuend - subtrahend.real, -subtrahend.imag);
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(Complex multiplicand, Complex multiplier)
        {
            return new Complex(multiplicand.real * multiplier.real - multiplicand.imag * multiplier.imag, multiplicand.real * multiplier.imag + multiplicand.imag * multiplier.real);
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(double multiplicand, Complex multiplier)
        {
            return new Complex(multiplier.real * multiplicand, multiplier.imag * multiplicand);
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(Complex multiplicand, double multiplier)
        {
            return new Complex(multiplicand.real * multiplier, multiplicand.imag * multiplier);
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(Complex dividend, Complex divisor)
        {
            if(divisor.IsZero)
            {
                return Complex.Infinity;
            }

            double z2mod = divisor.ModulusSquared;
            return new Complex((dividend.real * divisor.real + dividend.imag * divisor.imag) / z2mod, (dividend.imag * divisor.real - dividend.real * divisor.imag) / z2mod);
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(double dividend, Complex divisor)
        {
            if(divisor.IsZero)
            {
                return Complex.Infinity;
            }

            double zmod = divisor.ModulusSquared;
            return new Complex(dividend * divisor.real / zmod, -dividend * divisor.imag / zmod);
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(Complex dividend, double divisor)
        {
            if(Number.AlmostZero(divisor))
            {
                return Complex.Infinity;
            }

            return new Complex(dividend.real / divisor, dividend.imag / divisor);
        }

        /// <summary>
        /// Implicit conversion of a real double to a real <c>Complex</c>.
        /// </summary>
        public static implicit operator Complex(double number)
        {
            return new Complex(number, 0d);
        }

        #endregion


        #region Trigonometric Functions

        /// <summary>
        /// Trigonometric Sine (sin, Sinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Sine()
        {
            if(IsReal)
            {
                return new Complex(Trig.Sine(real), 0d);
            }

            return new Complex(
                Trig.Sine(real) * Trig.HyperbolicCosine(imag),
                Trig.Cosine(real) * Trig.HyperbolicSine(imag)
                );
        }

        /// <summary>
        /// Trigonometric Cosine (cos, Cosinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cosine()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cosine(real), 0d);
            }

            return new Complex(
                Trig.Cosine(real) * Trig.HyperbolicCosine(imag),
                -Trig.Sine(real) * Trig.HyperbolicSine(imag)
                );
        }

        /// <summary>
        /// Trigonometric Tangent (tan, Tangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Tangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.Tangent(real), 0d);
            }

            double cosr = Trig.Cosine(real);
            double sinhi = Trig.HyperbolicSine(imag);
            double denom = cosr * cosr + sinhi * sinhi;

            return new Complex(
                Trig.Sine(real) * cosr / denom,
                sinhi * Trig.HyperbolicCosine(imag) / denom
                );
        }

        /// <summary>
        /// Trigonometric Cotangent (cot, Cotangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cotangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cotangent(real), 0d);
            }

            double sinr = Trig.Sine(real);
            double sinhi = Trig.HyperbolicSine(imag);
            double denom = sinr * sinr + sinhi * sinhi;

            return new Complex(
                sinr * Trig.Cosine(real) / denom,
                -sinhi * Trig.HyperbolicCosine(imag) / denom
                );
        }

        /// <summary>
        /// Trigonometric Secant (sec, Sekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Secant()
        {
            if(IsReal)
            {
                return new Complex(Trig.Secant(real), 0d);
            }

            double cosr = Trig.Cosine(real);
            double sinhi = Trig.HyperbolicSine(imag);
            double denom = cosr * cosr + sinhi * sinhi;

            return new Complex(
                cosr * Trig.HyperbolicCosine(imag) / denom,
                Trig.Sine(real) * sinhi / denom
                );
        }

        /// <summary>
        /// Trigonometric Cosecant (csc, Cosekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cosecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cosecant(real), 0d);
            }

            double sinr = Trig.Sine(real);
            double sinhi = Trig.HyperbolicSine(imag);
            double denom = sinr * sinr + sinhi * sinhi;

            return new Complex(
                sinr * Trig.HyperbolicCosine(imag) / denom,
                -Trig.Cosine(real) * sinhi / denom
                );
        }

        #endregion

        #region Trigonometric Arcus Functions

        /// <summary>
        /// Trigonometric Arcus Sine (asin, Arkussinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseSine()
        {
            return -Complex.I * ((1 - this.Square()).SquareRoot() + Complex.I * this).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Cosine (acos, Arkuscosinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCosine()
        {
            return -Complex.I * (this + Complex.I * (1 - this.Square()).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Tangent (atan, Arkustangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseTangent()
        {
            Complex iz = new Complex(-imag, real); // I*this
            return new Complex(0, 0.5) * ((1 - iz).NaturalLogarithm() - (1 + iz).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Arcus Cotangent (acot, Arkuscotangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCotangent()
        {
            Complex iz = new Complex(-imag, real); // I*this
            return new Complex(0, 0.5) * ((1 + iz).NaturalLogarithm() - (1 - iz).NaturalLogarithm()) + Math.PI / 2;
        }

        /// <summary>
        /// Trigonometric Arcus Secant (asec, Arkussekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseSecant()
        {
            Complex inv = 1 / this;
            return -Complex.I * (inv + Complex.I * (1 - inv.Square()).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Cosecant (acsc, Arkuscosekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCosecant()
        {
            Complex inv = 1 / this;
            return -Complex.I * (Complex.I * inv + (1 - inv.Square()).SquareRoot()).NaturalLogarithm();
        }

        #endregion

        #region Trigonometric Hyperbolic Functions

        /// <summary>
        /// Trigonometric Hyperbolic Sine (sinh, Sinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicSine()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicSine(real), 0d);
            }

            return new Complex(
                Trig.HyperbolicSine(real) * Trig.Cosine(imag),
                Trig.HyperbolicCosine(real) * Trig.Sine(imag)
                );
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosine (cosh, Cosinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCosine()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCosine(real), 0d);
            }

            return new Complex(
                Trig.HyperbolicCosine(real) * Trig.Cosine(imag),
                Trig.HyperbolicSine(real) * Trig.Sine(imag)
                );
        }

        /// <summary>
        /// Trigonometric Hyperbolic Tangent (tanh, Tangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicTangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicTangent(real), 0d);
            }

            double cosi = Trig.Cosine(imag);
            double sinhr = Trig.HyperbolicSine(real);
            double denom = cosi * cosi + sinhr * sinhr;

            return new Complex(
                Trig.HyperbolicCosine(real) * sinhr / denom,
                cosi * Trig.Sine(imag) / denom
                );
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cotangent (coth, Cotangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCotangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCotangent(real), 0d);
            }

            double sini = Trig.Sine(imag);
            double sinhr = Trig.HyperbolicSine(real);
            double denom = sini * sini + sinhr * sinhr;

            return new Complex(
                sinhr * Trig.HyperbolicCosine(real) / denom,
                sini * Trig.Cosine(imag) / denom
                );
        }

        /// <summary>
        /// Trigonometric Hyperbolic Secant (sech, Secans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicSecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicSecant(real), 0d);
            }

            Complex exp = this.Exponential();
            return 2 * exp / (exp.Square() + 1);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosecant (csch, Cosecans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCosecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCosecant(real), 0d);
            }

            Complex exp = this.Exponential();
            return 2 * exp / (exp.Square() - 1);
        }

        #endregion

        #region Trigonometric Hyperbolic Area Functions

        /// <summary>
        /// Trigonometric Hyperbolic Area Sine (asinh, reasinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicSine()
        {
            return (this + (this.Square() + 1).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosine (acosh, Areacosinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCosine()
        {
            return (this + (this - 1).SquareRoot() * (this + 1).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Tangent (atanh, Areatangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicTangent()
        {
            return 0.5 * ((1 + this).NaturalLogarithm() - (1 - this).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cotangent (acoth, Areacotangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCotangent()
        {
            return 0.5 * ((this + 1).NaturalLogarithm() - (this - 1).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Secant (asech, Areasekans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicSecant()
        {
            Complex inv = 1 / this;
            return (inv + (inv - 1).SquareRoot() * (inv + 1).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosecant (acsch, Areacosekans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCosecant()
        {
            Complex inv = 1 / this;
            return (inv + (inv.Square() + 1).SquareRoot()).NaturalLogarithm();
        }

        #endregion


        #region Exponential Functions

        /// <summary>
        /// Exponential of this <c>Complex</c> (exp(x), E^x).
        /// </summary>
        public
        Complex
        Exponential()
        {
            double exp = Math.Exp(real);
            if(IsReal)
            {
                return new Complex(exp, 0d);
            }

            return new Complex(
                exp * Trig.Cosine(imag),
                exp * Trig.Sine(imag)
                );
        }

        /// <summary>
        /// Natural Logarithm of this <c>Complex</c> (Base E).
        /// </summary>
        public
        Complex
        NaturalLogarithm()
        {
            if(IsRealNonNegative)
            {
                return new Complex(Math.Log(real), 0d);
            }

            return new Complex(
                0.5d * Math.Log(ModulusSquared),
                Argument
                );
        }

        /// <summary>
        /// Raise this <c>Complex</c> to the given value.
        /// </summary>
        public
        Complex
        Power(
            Complex exponent
            )
        {
            if(IsZero)
            {
                if(exponent.IsZero)
                {
                    return Complex.One;
                }

                if(exponent.Real > 0)
                {
                    return Complex.Zero;
                }

                if(exponent.Real < 0)
                {
                    if(Number.AlmostEqual(0d, exponent.Imag))
                    {
                        return new Complex(double.PositiveInfinity, 0d);
                    }

                    return new Complex(double.PositiveInfinity, double.PositiveInfinity);
                }

                return Complex.NaN;
            }

            return (exponent * NaturalLogarithm()).Exponential();
        }

        /// <summary>
        /// Raise this <c>Complex</c> to the inverse of the given value.
        /// </summary>
        public
        Complex
        Root(
            Complex rootexponent
            )
        {
            return Power(1 / rootexponent);
        }

        /// <summary>
        /// The Square (power 2) of this <c>Complex</c>
        /// </summary>
        public
        Complex
        Square()
        {
            if(IsReal)
            {
                return new Complex(real * real, 0d);
            }

            return new Complex(
                real * real - imag * imag,
                2 * real * imag
                );
        }

        /// <summary>
        /// The Square Root (power 1/2) of this <c>Complex</c>
        /// </summary>
        public
        Complex
        SquareRoot()
        {
            if(IsRealNonNegative)
            {
                return new Complex(Math.Sqrt(real), 0d);
            }

            double mod = Modulus;

            if(imag > 0 || imag == 0 && real < 0)
            {
                return new Complex(
                    Constants.Sqrt1_2 * Math.Sqrt(mod + real),
                    Constants.Sqrt1_2 * Math.Sqrt(mod - real)
                    );
            }

            return new Complex(
                Constants.Sqrt1_2 * Math.Sqrt(mod + real),
                -Constants.Sqrt1_2 * Math.Sqrt(mod - real)
                );
        }

        #endregion


        #region ToString and Parse

        /// <summary>
        /// Parse a string into a <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// The adopted string representation for the complex numbers is 
        /// <i>UVW+I*XYZ</i> where <i>UVW</i> and <i>XYZ</i> are <c>double</c> 
        /// strings. Some alternative representations are <i>UVW+XYZi</i>,
        /// <i>UVW+iXYZ</i>, <i>UVW</i> and <i>iXYZ</i>. 
        /// Additionally the string <c>"NaN"</c> is mapped to 
        /// <c>Complex.NaN</c>, the string <c>"Infinity"</c> to 
        /// <c>Complex.ComplexInfinity</c>, <c>"PositiveInfinity"</c>
        /// to <c>Complex.DirectedInfinity(Complex.One)</c>,
        /// <c>"NegativeInfinity"</c> to <c>Complex.DirectedInfinity(-Complex.One)</c>
        /// and finally <c>"DirectedInfinity(WVW+I*XYZ)"</c> to <c>Complex.DirectedInfinity(WVW+I*XYZ)</c>.
        /// <code>
        /// Complex z = Complex.Parse("12.5+I*7");
        /// Complex nan = Complex.Parse("NaN");
        /// Complex infinity = Complex.Parse("Infinity");
        /// </code>
        /// This method is symmetric to <see cref="Complex.ToString()"/>.
        /// </remarks>
        public static Complex Parse(string complex)
        {
            return Parse(complex, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parse a string into a <c>Complex</c>.
        /// </summary>
        public static Complex Parse(string complex, NumberFormatInfo numberFormat)
        {
            ComplexParser parser = new ComplexParser(complex, numberFormat);
            return parser.Complex;
        }

        /// <summary>
        /// Formats this <c>Complex</c> into a <c>string</c>.
        /// </summary>
        public override string ToString()
        {
            return ToString(NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Formats this <c>Complex</c> into a <c>string</c>.
        /// </summary>
        public string ToString(NumberFormatInfo numberFormat)
        {
            if(IsInfinity)
            {
                return "Infinity";
            }

            if(IsNaN)
            {
                return numberFormat.NaNSymbol;
            }

            if(IsReal)
            {
                return real.ToString(numberFormat);
            }

            // note: there's a difference between the negative sign and the subtraction operator!
            if(IsImaginary)
            {
                if(Number.AlmostEqual(imag, 1))
                {
                    return "I";
                }

                if(Number.AlmostEqual(imag, -1))
                {
                    return numberFormat.NegativeSign + "I";
                }

                if(imag < 0)
                {
                    return numberFormat.NegativeSign + "I*" + (-imag).ToString(numberFormat);
                }

                return "I*" + imag.ToString(numberFormat);
            }
            else
            {
                if(Number.AlmostEqual(imag, 1))
                {
                    return real.ToString(numberFormat) + "+I";
                }

                if(Number.AlmostEqual(imag, -1))
                {
                    return real.ToString(numberFormat) + "-I";
                }

                if(imag < 0)
                {
                    return real.ToString(numberFormat) + "-I*"
                       + (-imag).ToString(numberFormat);
                }

                return real.ToString() + "+I*" + imag.ToString(numberFormat);
            }
        }

        private sealed class ComplexParser
        {
            Complex complex;
            int cursor; // = 0;
            string source;
            NumberFormatInfo numberFormat;

            public
            ComplexParser(
                string complex,
                NumberFormatInfo numberFormat
                )
            {
                this.numberFormat = numberFormat;
                this.source = complex.ToLower().Trim();
                this.complex = ScanComplex();
            }

            #region Infrastructure

            char
            Consume()
            {
                return source[cursor++];
            }

            char LookAheadCharacterOrNull
            {
                get
                {
                    if(cursor >= source.Length)
                    {
                        return '\0';
                    }
                    
                    return source[cursor];
                }
            }

            char LookAheadCharacter
            {
                get
                {
                    if(cursor >= source.Length)
                    {
                        throw new ArgumentException(Resources.ArgumentParseComplexNumber, "complex");
                    }
                    
                    return source[cursor];
                }
            }

            #endregion

            #region Scanners

            Complex
            ScanComplex()
            {
                if(source.Equals("i"))
                {
                    return Complex.I;
                }

                if(source.Equals(numberFormat.NaNSymbol.ToLower()))
                {
                    return Complex.NaN;
                }

                if(source.Equals("infinity") || source.Equals("infty"))
                {
                    return Complex.Infinity;
                }

                ScanSkipWhitespace();
                Complex complex = ScanSignedComplexNumberPart();
                ScanSkipWhitespace();

                if(IsSign(LookAheadCharacterOrNull))
                {
                    complex += ScanSignedComplexNumberPart();
                }

                return complex;
            }

            Complex
            ScanSignedComplexNumberPart()
            {
                bool negativeSign = false;

                if(IsSign(LookAheadCharacterOrNull))
                {
                    if(IsNegativeSign(LookAheadCharacter))
                    {
                        negativeSign = true;
                    }

                    Consume();
                    ScanSkipWhitespace();
                }

                if(negativeSign)
                {
                    return -ScanComplexNumberPart();
                }

                return ScanComplexNumberPart();
            }

            Complex
            ScanComplexNumberPart()
            {
                bool imaginary = false;

                if(IsI(LookAheadCharacter))
                {
                    Consume();
                    ScanSkipWhitespace();

                    if(IsMult(LookAheadCharacterOrNull))
                    {
                        Consume();
                    }

                    ScanSkipWhitespace();
                    imaginary = true;
                }

                if(!IsNumber(LookAheadCharacterOrNull) && !IsSign(LookAheadCharacterOrNull))
                {
                    return new Complex(0d, 1d);
                }

                double part = ScanNumber();
                ScanSkipWhitespace();

                if(IsMult(LookAheadCharacterOrNull))
                {
                    Consume();
                    ScanSkipWhitespace();
                }

                if(IsI(LookAheadCharacterOrNull))
                {
                    Consume();
                    ScanSkipWhitespace();
                    imaginary = true;
                }

                if(imaginary)
                {
                    return new Complex(0d, part);
                }
                else
                {
                    return new Complex(part, 0d);
                }
            }

            double
            ScanNumber()
            {
                StringBuilder sb = new StringBuilder();

                if(IsSign(LookAheadCharacter))
                {
                    sb.Append(Consume());
                }

                ScanSkipWhitespace();
                ScanInteger(sb);
                ScanSkipWhitespace();

                if(IsDecimal(LookAheadCharacterOrNull))
                {
                    Consume();
                    sb.Append(numberFormat.NumberDecimalSeparator);
                    ScanInteger(sb);
                }

                if(IsE(LookAheadCharacterOrNull))
                {
                    Consume();
                    sb.Append('e');

                    if(IsSign(LookAheadCharacter))
                    {
                        sb.Append(Consume());
                    }

                    ScanInteger(sb);
                }

                return double.Parse(
                    sb.ToString(),
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign,
                    numberFormat
                    );
            }

            void
            ScanInteger(
                StringBuilder sb
                )
            {
                sb.Append(Consume());
                while(IsNumber(LookAheadCharacterOrNull) || IsGroup(LookAheadCharacterOrNull))
                {
                    char c = Consume();
                    if(!IsGroup(c))
                    {
                        sb.Append(c);
                    }
                }
            }

            void
            ScanSkipWhitespace()
            {
                while(cursor < source.Length && !IsNotWhiteSpace(LookAheadCharacter))
                {
                    Consume();
                }
            }

            #endregion

            #region Indicators

            bool
            IsNotWhiteSpace(
                char c
                )
            {
                return IsNumber(c) || IsDecimal(c) || IsE(c) || IsI(c) || IsSign(c) || IsMult(c);
            }
            
            static
            bool
            IsNumber(
                char c
                )
            {
                // TODO: consider using numberFormat.NativeDigits
                return c >= '0' && c <= '9';
            }
            
            bool
            IsDecimal(
                char c
                )
            {
                return numberFormat.NumberDecimalSeparator.Equals(c.ToString());
            }

            bool
            IsGroup(
                char c
                )
            {
                return numberFormat.NumberGroupSeparator.Equals(c.ToString());
            }

            static
            bool
            IsE(
                char c
                )
            {
                return c == 'e';
            }

            static
            bool
            IsI(
                char c
                )
            {
                return c == 'i' || c == 'j';
            }

            bool
            IsSign(
                char c
                )
            {
                return numberFormat.PositiveSign.Equals(c.ToString()) || numberFormat.NegativeSign.Equals(c.ToString());
            }

            bool
            IsNegativeSign(
                char c
                )
            {
                return numberFormat.NegativeSign.Equals(c.ToString());
            }

            static
            bool
            IsMult(
                char c
                )
            {
                return c == '*';
            }

            #endregion

            public Complex Complex
            {
                get { return complex; }
            }

            public double Real
            {
                get { return complex.real; }
            }

            public double Imaginary
            {
                get { return complex.imag; }
            }
        }

        #endregion
    }
}
