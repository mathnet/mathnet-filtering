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
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// Rational represents a finite order rational with positive
    /// powers and constant real coefficients for both numerator and denominator.
    /// </summary>
    public class Rational : IComparable, ICloneable
    {
        Polynomial numerator;
        Polynomial denominator;

        // TODO: Implement polynomial factorization to normalize rationals

        /// <summary>
        /// Create a new rational by polynomials (directly linked, clone them manually before applying if needed)
        /// </summary>
        public
        Rational(
            Polynomial numerator,
            Polynomial denominator
            )
        {
            this.numerator = numerator.Clone();
            this.denominator = denominator.Clone();
        }

        /// <summary>
        /// Create a new rational by orders
        /// </summary>
        public
        Rational(
            int numeratorOrder,
            int denominatorOrder
            )
        {
            numerator = new Polynomial(numeratorOrder);
            denominator = new Polynomial(denominatorOrder);
        }

        /// <summary>
        /// Create a new rational by coefficients
        /// </summary>
        public
        Rational(
            double[] numeratorCoefficients,
            double[] denominatorCoefficients
            )
        {
            numerator = new Polynomial(numeratorCoefficients);
            denominator = new Polynomial(denominatorCoefficients);
        }

        /// <summary>
        /// Create a new rational by copy
        /// </summary>
        /// <param name="copy">A rational to copy from.</param>
        public
        Rational(
            Rational copy
            )
        {
            numerator = new Polynomial(copy.numerator);
            denominator = new Polynomial(copy.denominator);
        }

        #region Accessors

        /// <summary>
        /// Normalize both numerator and denominator polynomials.
        /// </summary>
        public
        void
        Normalize()
        {
            numerator.Normalize();
            denominator.Normalize();
        }

        /// <summary>
        /// The numerator polynomial.
        /// </summary>
        public Polynomial Numerator
        {
            get { return numerator; }
            set { numerator = value; }
        }

        /// <summary>
        /// The denominator polynomial.
        /// </summary>
        public Polynomial Denominator
        {
            get { return denominator; }
            set { denominator = value; }
        }

        /// <summary>
        /// The order of the numerator polynomial.
        /// </summary>
        public int NumeratorOrder
        {
            get { return numerator.Order; }
        }

        /// <summary>
        /// The order of the denominator polynomial.
        /// </summary>
        public int DenominatorOrder
        {
            get { return denominator.Order; }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Check whether two rationals have the same coefficients.
        /// </summary>
        public static
        bool
        operator ==(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Equals(rational2);
        }

        /// <summary>
        /// Check whether two rationals have different coefficients.
        /// </summary>
        public static
        bool
        operator !=(
            Rational rational1,
            Rational rational2
            )
        {
            return !rational1.Equals(rational2);
        }


        /// <summary>
        /// Check wether a rational is bigger than another rational.
        /// </summary>
        public static
        bool
        operator >(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.CompareTo(rational2) == 1;
        }

        /// <summary>
        /// Check wether a rational is smaller than another rational.
        /// </summary>
        public static
        bool
        operator <(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.CompareTo(rational2) == -1;
        }

        /// <summary>
        /// Check wether a rational is bigger than or equal to another rational.
        /// </summary>
        public static
        bool
        operator >=(
            Rational rational1,
            Rational rational2
            )
        {
            int res = rational1.CompareTo(rational2);
            return res == 1 || res == 0;
        }

        /// <summary>
        /// Check wether a rational is smaller than or equal to another rational.
        /// </summary>
        public static
        bool
        operator <=(
            Rational rational1,
            Rational rational2
            )
        {
            int res = rational1.CompareTo(rational2);
            return res == -1 || res == 0;
        }


        /// <summary>
        /// Add a rational to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Add(rational2);
        }

        /// <summary>
        /// Add a polynomial to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational,
            Polynomial polynomial
            )
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a rational to a polynomial.
        /// </summary>
        public static
        Rational
        operator +(
            Polynomial polynomial,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a real number to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational,
            double n
            )
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a rational to a real number.
        /// </summary>
        public static
        Rational
        operator +(
            double n,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational
            )
        {
            return rational;
        }


        /// <summary>
        /// Subtract a rational from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Subtract(rational2);
        }

        /// <summary>
        /// Subtract a polynomial from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational,
            Polynomial polynomial
            )
        {
            Rational ret = new Rational(rational);
            ret.SubtractInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a polynomial.
        /// </summary>
        public static
        Rational
        operator -(
            Polynomial polynomial,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a real number from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational,
            double n
            )
        {
            Rational ret = new Rational(rational);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a real number.
        /// </summary>
        public static
        Rational
        operator -(
            double n,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Negate a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            return ret;
        }


        /// <summary>
        /// Multiply/Convolute two rationals.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Multiply(rational2);
        }

        /// <summary>
        /// Multiply/Convolute a rationals with a polynomial.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational,
            Polynomial polynomial
            )
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Multiply/Convolute a polynomial with a rational.
        /// </summary>
        public static
        Rational
        operator *(
            Polynomial polynomial,
            Rational rational
            )
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Stretch a rational with a real number factor.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational,
            double n
            )
        {
            Rational ret = new Rational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        Rational
        operator *(
            double n,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }


        /// <summary>
        /// Divide two rationals.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Divide(rational2);
        }

        /// <summary>
        /// Divide a rational to a polynomial.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational,
            Polynomial polynomial
            )
        {
            return rational.Divide(polynomial);
        }

        /// <summary>
        /// Divide a polynomial to a rational.
        /// </summary>
        public static
        Rational
        operator /(
            Polynomial polynomial,
            Rational rational
            )
        {
            Rational ret = rational.Divide(polynomial);
            ret.InvertInplace();
            return ret;
        }

        /// <summary>
        /// Stretch a rational with a real number quotient.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational,
            double n
            )
        {
            Rational ret = new Rational(rational);
            ret.DivideInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a the inverse of a rational with a real number quotient.
        /// </summary>
        public static
        Rational
        operator /(
            double n,
            Rational rational
            )
        {
            Rational ret = new Rational(rational);
            ret.InvertInplace();
            ret.MultiplyInplace(n);
            return ret;
        }

        #endregion

        #region Inplace Arithmetic Methods

        /// <summary>
        /// Create a new rational as the result of adding a rational to this rational.
        /// </summary>
        /// <param name="rational">The rational to add</param>
        public
        Rational
        Add(
            Rational rational
            )
        {
            if(denominator.Equals(rational.denominator))
            {
                return new Rational(
                    numerator + rational.numerator,
                    denominator.Clone()
                    );
            }

            Polynomial num = numerator * rational.denominator + rational.numerator * denominator;
            Polynomial denom = denominator * rational.denominator;
            return new Rational(num, denom);
        }

        /// <summary>
        /// Add a polynomial directly inplace to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to add.</param>
        public
        void
        AddInplace(
            Polynomial polynomial
            )
        {
            numerator.AddInplace(denominator * polynomial);
        }

        /// <summary>
        /// Add a floating point number directly inplace to this rational.
        /// </summary>
        /// <param name="n">The floating point number to add.</param>
        public
        void
        AddInplace(
            double n
            )
        {
            numerator.AddInplace(denominator * n);
        }


        /// <summary>
        /// Create a new rational as the result of subtracting a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to subtract.</param>
        public
        Rational
        Subtract(
            Rational rational
            )
        {
            if(denominator.Equals(rational.denominator))
            {
                return new Rational(
                    numerator - rational.numerator,
                    denominator.Clone()
                    );
            }

            Polynomial num = numerator * rational.denominator - rational.numerator * denominator;
            Polynomial denom = denominator * rational.denominator;
            return new Rational(num, denom);
        }

        /// <summary>
        /// Subtract a polynomial directly inplace from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract.</param>
        public
        void
        SubtractInplace(
            Polynomial polynomial
            )
        {
            numerator.SubtractInplace(denominator * polynomial);
        }

        /// <summary>
        /// Subtract a floating point number directly inplace from this rational.
        /// </summary>
        /// <param name="n">The floating point number to subtract.</param>
        public
        void
        SubtractInplace(
            double n
            )
        {
            numerator.SubtractInplace(denominator * n);
        }

        /// <summary>
        /// Negate this rational directly inplace.
        /// </summary>
        public
        void
        NegateInplace()
        {
            numerator.NegateInplace();
        }


        /// <summary>
        /// Create a new rational as the result of multiplying a rational to this rational.
        /// </summary>
        /// <param name="rational">The rational to multiply with.</param>
        public
        Rational
        Multiply(
            Rational rational
            )
        {
            return new Rational(
                numerator * rational.numerator,
                denominator * rational.denominator
                );
        }

        /// <summary>
        /// Create a new rational as the result of multiplying a polynomial to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to multiply with.</param>
        public
        Rational
        Multiply(
            Polynomial polynomial
            )
        {
            return new Rational(
                numerator * polynomial,
                denominator.Clone()
                );
        }

        /// <summary>
        /// Multiply a floating point number directly inplace to this rational.
        /// </summary>
        /// <param name="n">The floating point number to multiply with.</param>
        public
        void
        MultiplyInplace(
            double n
            )
        {
            numerator.MultiplyInplace(n);
        }


        /// <summary>
        /// Create a new rational as the result of dividing a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to divide with.</param>
        public
        Rational
        Divide(
            Rational rational
            )
        {
            return new Rational(
                numerator * rational.denominator,
                denominator * rational.numerator
                );
        }

        /// <summary>
        /// Create a new rational as the result of dividing a polynomial from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to divide with.</param>
        public
        Rational
        Divide(
            Polynomial polynomial
            )
        {
            return new Rational(
                numerator.Clone(),
                denominator * polynomial
                );
        }

        /// <summary>
        /// Divide a floating point number directly inplace from this rational.
        /// </summary>
        /// <param name="n">The floating point number to divide with.</param>
        public
        void
        DivideInplace(
            double n
            )
        {
            denominator.MultiplyInplace(n);
        }

        /// <summary>
        /// Invert this rational directly inplace.
        /// </summary>
        public
        void
        InvertInplace()
        {
            Polynomial temp = denominator;
            denominator = numerator;
            numerator = temp;
        }

        #endregion

        #region Evaluation

        /// <summary>
        /// Evaluate the value of this rational at a given point.
        /// </summary>
        /// <param name="value">The point where to evaluate the rational</param>
        public
        double
        Evaluate(
            double value
            )
        {
            // TODO: correct Rational.Evaluate implementation
            // the formula here below is accurate iff the Rational cannot be 'simplified'

            return numerator.Evaluate(value) / denominator.Evaluate(value);
        }

        #endregion

        #region String Formatting and Parsing

        /// <summary>
        /// Format a human-readable string of this rational with the given string as base variable (e.g. "x").
        /// </summary>
        public
        string
        ToString(
            string baseVariable
            )
        {
            return "(" + numerator.ToString(baseVariable) + ")/(" + denominator.ToString(baseVariable) + ")";
        }

        /// <summary>
        /// Format a human-readable string of this rational with "x" as base variable.
        /// </summary>
        public override
        string
        ToString()
        {
            return ToString("x");
        }

        #endregion

        #region .NET Integration: Hashing, Equality, Ordering, Cloning

        /// <summary>
        /// Serves as a hash function for rationals.
        /// </summary>
        public override
        int
        GetHashCode()
        {
            return numerator.GetHashCode() ^ denominator.GetHashCode();
        }

        /// <summary>
        /// Check whether this rational is equal to another rational.
        /// </summary>
        public override
        bool
        Equals(
            object obj
            )
        {
            if(obj == null || !(obj is Rational))
            {
                return false;
            }

            return Equals((Rational)obj);
        }

        /// <summary>
        /// Check whether this rational is equal to another rational.
        /// </summary>
        public
        bool
        Equals(
            Rational rational
                )
        {
            return numerator.Equals(rational.numerator)
                && denominator.Equals(rational.denominator);
        }

        /// <summary>
        /// Check whether two rationals are equal.
        /// </summary>
        public static
        bool
        Equals(
            Rational rational1,
            Rational rational2
            )
        {
            return rational1.Equals(rational2);
        }

        /// <summary>
        /// Compare this rational to another rational.
        /// </summary>
        int
        IComparable.CompareTo(
            object obj
            )
        {
            if(obj == null)
            {
                return 1;
            }

            if(!(obj is Rational))
            {
                throw new ArgumentException(Resources.ArgumentTypeMismatch, "obj");
            }

            return CompareTo((Rational)obj);
        }

        /// <summary>
        /// Compare this rational to another rational.
        /// </summary>
        public
        int
        CompareTo(
            Rational rational
            )
        {
            int n = numerator.CompareTo(rational.numerator);
            if(n == 0)
            {
                n = denominator.CompareTo(rational.denominator);
            }

            return n;
        }

        /// <summary>
        /// Create a copy of this rational.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a copy of this rational.
        /// </summary>
        public
        Rational
        Clone()
        {
            return new Rational(this);
        }

        #endregion
    }
}
