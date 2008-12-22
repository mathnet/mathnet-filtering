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
    /// <summary>
    /// Rational represents a finite order rational with positive
    /// powers and constant complex coefficients for both numerator and denominator.
    /// </summary>
    public class ComplexRational : IComparable, ICloneable
    {
        ComplexPolynomial numerator;
        ComplexPolynomial denominator;

        // TODO: Implement polynomial factorization to normalize rationals

        /// <summary>
        /// Initializes a new instance of the ComplexRational class,
        /// by directly referencing the two provided polynomials (no deep copy).
        /// </summary>
        public
        ComplexRational(
            ComplexPolynomial numerator,
            ComplexPolynomial denominator
            )
        {
            this.numerator = numerator.Clone();
            this.denominator = denominator.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the ComplexRational class,
        /// with the provided numerator and denominator order.
        /// </summary>
        public
        ComplexRational(
            int numeratorOrder,
            int denominatorOrder
            )
        {
            numerator = new ComplexPolynomial(numeratorOrder);
            denominator = new ComplexPolynomial(denominatorOrder);
        }

        /// <summary>
        /// Initializes a new instance of the ComplexRational class,
        /// by directly referencing the two provided polynomial coefficients.
        /// </summary>
        public
        ComplexRational(
            Complex[] numeratorCoefficients,
            Complex[] denominatorCoefficients
            )
        {
            numerator = new ComplexPolynomial(numeratorCoefficients);
            denominator = new ComplexPolynomial(denominatorCoefficients);
        }

        /// <summary>
        /// Initializes a new instance of the ComplexRational class,
        /// by deep-copy from an existing complex rational.
        /// </summary>
        /// <param name="copy">A rational to copy from.</param>
        public
        ComplexRational(
            ComplexRational copy
            )
        {
            numerator = new ComplexPolynomial(copy.numerator);
            denominator = new ComplexPolynomial(copy.denominator);
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
        public ComplexPolynomial Numerator
        {
            get { return numerator; }
            set { numerator = value; }
        }

        /// <summary>
        /// The denominator polynomial.
        /// </summary>
        public ComplexPolynomial Denominator
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
            ComplexRational rational1,
            ComplexRational rational2
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
            ComplexRational rational1,
            ComplexRational rational2
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
            ComplexRational rational1,
            ComplexRational rational2
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
            ComplexRational rational1,
            ComplexRational rational2
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
            ComplexRational rational1,
            ComplexRational rational2
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
            ComplexRational rational1,
            ComplexRational rational2
            )
        {
            int res = rational1.CompareTo(rational2);
            return res == -1 || res == 0;
        }


        /// <summary>
        /// Add a rational to a rational.
        /// </summary>
        public static
        ComplexRational
        operator +(
            ComplexRational rational1,
            ComplexRational rational2
            )
        {
            return rational1.Add(rational2);
        }

        /// <summary>
        /// Add a polynomial to a rational.
        /// </summary>
        public static
        ComplexRational
        operator +(
            ComplexRational rational,
            ComplexPolynomial polynomial
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a rational to a polynomial.
        /// </summary>
        public static
        ComplexRational
        operator +(
            ComplexPolynomial polynomial,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a complex number to a rational.
        /// </summary>
        public static
        ComplexRational
        operator +(
            ComplexRational rational,
            Complex n
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a rational to a real number.
        /// </summary>
        public static
        ComplexRational
        operator +(
            Complex n,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        ComplexRational
        operator +(
            ComplexRational rational
            )
        {
            return rational;
        }


        /// <summary>
        /// Subtract a rational from a rational.
        /// </summary>
        public static
        ComplexRational
        operator -(
            ComplexRational rational1,
            ComplexRational rational2
            )
        {
            return rational1.Subtract(rational2);
        }

        /// <summary>
        /// Subtract a polynomial from a rational.
        /// </summary>
        public static
        ComplexRational
        operator -(
            ComplexRational rational,
            ComplexPolynomial polynomial
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.SubtractInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a polynomial.
        /// </summary>
        public static
        ComplexRational
        operator -(
            ComplexPolynomial polynomial,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.NegateInplace();
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a real number from a rational.
        /// </summary>
        public static
        ComplexRational
        operator -(
            ComplexRational rational,
            double n
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a real number.
        /// </summary>
        public static
        ComplexRational
        operator -(
            Complex n,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Negate a rational.
        /// </summary>
        public static
        ComplexRational
        operator -(
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.NegateInplace();
            return ret;
        }


        /// <summary>
        /// Multiply/Convolute two rationals.
        /// </summary>
        public static
        ComplexRational
        operator *(
            ComplexRational rational1,
            ComplexRational rational2
            )
        {
            return rational1.Multiply(rational2);
        }

        /// <summary>
        /// Multiply/Convolute a rationals with a polynomial.
        /// </summary>
        public static
        ComplexRational
        operator *(
            ComplexRational rational,
            ComplexPolynomial polynomial
            )
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Multiply/Convolute a polynomial with a rational.
        /// </summary>
        public static
        ComplexRational
        operator *(
            ComplexPolynomial polynomial,
            ComplexRational rational
            )
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Stretch a rational with a real number factor.
        /// </summary>
        public static
        ComplexRational
        operator *(
            ComplexRational rational,
            Complex n
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        ComplexRational
        operator *(
            Complex n,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }


        /// <summary>
        /// Divide two rationals.
        /// </summary>
        public static
        ComplexRational
        operator /(
            ComplexRational rational1,
            ComplexRational rational2
            )
        {
            return rational1.Divide(rational2);
        }

        /// <summary>
        /// Divide a rational to a polynomial.
        /// </summary>
        public static
        ComplexRational
        operator /(
            ComplexRational rational,
            ComplexPolynomial polynomial
            )
        {
            return rational.Divide(polynomial);
        }

        /// <summary>
        /// Divide a polynomial to a rational.
        /// </summary>
        public static
        ComplexRational
        operator /(
            ComplexPolynomial polynomial,
            ComplexRational rational
            )
        {
            ComplexRational ret = rational.Divide(polynomial);
            ret.InvertInplace();
            return ret;
        }

        /// <summary>
        /// Stretch a rational with a real number quotient.
        /// </summary>
        public static
        ComplexRational
        operator /(
            ComplexRational rational,
            double n
            )
        {
            ComplexRational ret = new ComplexRational(rational);
            ret.DivideInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a the inverse of a rational with a real number quotient.
        /// </summary>
        public static
        ComplexRational
        operator /(
            double n,
            ComplexRational rational
            )
        {
            ComplexRational ret = new ComplexRational(rational);
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
        ComplexRational
        Add(
            ComplexRational rational
            )
        {
            if(denominator.Equals(rational.denominator))
            {
                return new ComplexRational(
                    numerator + rational.numerator,
                    denominator.Clone()
                    );
            }

            ComplexPolynomial num = numerator * rational.denominator + rational.numerator * denominator;
            ComplexPolynomial denom = denominator * rational.denominator;
            return new ComplexRational(num, denom);
        }

        /// <summary>
        /// Add a polynomial directly inplace to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to add.</param>
        public
        void
        AddInplace(
            ComplexPolynomial polynomial
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
            Complex n
            )
        {
            numerator.AddInplace(denominator * n);
        }


        /// <summary>
        /// Create a new rational as the result of subtracting a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to subtract.</param>
        public
        ComplexRational
        Subtract(
            ComplexRational rational
            )
        {
            if(denominator.Equals(rational.denominator))
            {
                return new ComplexRational(
                    numerator - rational.numerator,
                    denominator.Clone()
                    );
            }

            ComplexPolynomial num = numerator * rational.denominator - rational.numerator * denominator;
            ComplexPolynomial denom = denominator * rational.denominator;
            return new ComplexRational(num, denom);
        }

        /// <summary>
        /// Subtract a polynomial directly inplace from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract.</param>
        public
        void
        SubtractInplace(
            ComplexPolynomial polynomial
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
            Complex n
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
        ComplexRational
        Multiply(
            ComplexRational rational
            )
        {
            return new ComplexRational(
                numerator * rational.numerator,
                denominator * rational.denominator
                );
        }

        /// <summary>
        /// Create a new rational as the result of multiplying a polynomial to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to multiply with.</param>
        public
        ComplexRational
        Multiply(
            ComplexPolynomial polynomial
            )
        {
            return new ComplexRational(
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
            Complex n
            )
        {
            numerator.MultiplyInplace(n);
        }


        /// <summary>
        /// Create a new rational as the result of dividing a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to divide with.</param>
        public
        ComplexRational
        Divide(
            ComplexRational rational
            )
        {
            return new ComplexRational(
                numerator * rational.denominator,
                denominator * rational.numerator
                );
        }

        /// <summary>
        /// Create a new rational as the result of dividing a polynomial from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to divide with.</param>
        public
        ComplexRational
        Divide(
            ComplexPolynomial polynomial
            )
        {
            return new ComplexRational(
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
            Complex n
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
            ComplexPolynomial temp = denominator;
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
        Complex
        Evaluate(
            Complex value
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

            return Equals((ComplexRational)obj);
        }

        /// <summary>
        /// Check whether this rational is equal to another rational.
        /// </summary>
        public
        bool
        Equals(
            ComplexRational rational
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
            ComplexRational rational1,
            ComplexRational rational2
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

            if(!(obj is ComplexRational))
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentTypeMismatch, "obj");
            }

            return CompareTo((ComplexRational)obj);
        }

        /// <summary>
        /// Compare this rational to another rational.
        /// </summary>
        public
        int
        CompareTo(
            ComplexRational rational
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
        ComplexRational
        Clone()
        {
            return new ComplexRational(this);
        }

        #endregion
    }
}
