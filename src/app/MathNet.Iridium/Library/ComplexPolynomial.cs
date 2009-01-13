//-----------------------------------------------------------------------
// <copyright file="ComplexPolynomial.cs" company="Math.NET Project">
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
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// Polynomial represents a finite order polynomial
    /// with positive powers and constant complex coefficients.
    /// </summary>
    /// <remarks>The polynomial coefficients are ordered such that
    /// c[0] is the constant term and c[n] is the coefficient of z^n,
    /// that is y = c[0]*x^0+c[1]*x^1+c[2]*x^2+...</remarks>
    public class ComplexPolynomial :
        IComparable,
        ICloneable
    {
        Complex[] coefficients;
        int order;

        /// <summary>
        /// Initializes a new instance of the ComplexPolynomial class
        /// of the provided order.
        /// </summary>
        /// <param name="order">The highest power. Example: 2*x^3+x-3 has order 3.</param>
        public
        ComplexPolynomial(int order)
        {
            this.order = order;
            this.coefficients = new Complex[SizeOfOrder(order)];
        }

        /// <summary>
        /// Initializes a new instance of the ComplexPolynomial class
        /// with the provided coefficients.
        /// </summary>
        /// <param name="coefficients">The complex coefficients vector. The coefficient index denotes the related power (c[0]*x^0+c[1]*x^1+..)</param>
        public
        ComplexPolynomial(Complex[] coefficients)
        {
            this.order = FindOrder(coefficients);
            this.coefficients = new Complex[SizeOfOrder(order)];
            Array.Copy(
                coefficients,
                this.coefficients,
                Math.Min(this.coefficients.Length, coefficients.Length));
        }

        /// <summary>
        /// Initializes a new instance of the ComplexPolynomial class
        /// by deep copy from an existing polynomial.
        /// </summary>
        /// <param name="copy">A complex polynomial to copy from.</param>
        public
        ComplexPolynomial(ComplexPolynomial copy)
        {
            this.order = copy.order;
            this.coefficients = new Complex[copy.coefficients.Length];
            Array.Copy(
                copy.coefficients,
                this.coefficients,
                Math.Min(this.coefficients.Length, copy.coefficients.Length));
        }

        /// <summary>
        /// Initializes a new instance of the ComplexPolynomial class
        /// by deep copy from an existing real polynomial.
        /// </summary>
        /// <param name="copy">A real polynomial to copy from.</param>
        public
        ComplexPolynomial(Polynomial copy)
        {
            int newOrder = copy.Order;

            Complex[] newCoeff = new Complex[SizeOfOrder(newOrder)];
            for(int i = 0; i < newCoeff.Length; i++)
            {
                newCoeff[i].Real = copy[i];
            }

            this.order = newOrder;
            this.coefficients = newCoeff;
        }

        #region Size

        static
        int
        SizeOfOrder(int order)
        {
            return 1 << (int)Math.Ceiling(Math.Log(order + 1, 2));
        }

        void
        ResizeOptimalForOrder(int order)
        {
            int bestSize = SizeOfOrder(order);
            if(bestSize == coefficients.Length)
            {
                return;
            }

            Complex[] newCoeffs = new Complex[bestSize];
            Array.Copy(
                this.coefficients,
                newCoeffs,
                Math.Min(this.coefficients.Length, newCoeffs.Length));
            coefficients = newCoeffs;
        }
        
        void
        EnsureSupportForOrder(int order)
        {
            if(coefficients.Length <= order)
            {
                ResizeOptimalForOrder(order);
            }
        }

        /// <summary>
        /// Normalizes the polynomial's order and resizes its data structure to that order.
        /// </summary>
        public
        void
        Normalize()
        {
            NormalizeOrder();
            ResizeOptimalForOrder(this.order);
        }

        void
        NormalizeOrder()
        {
            while(coefficients[order].IsZero && order > 0)
            {
                order--;
            }
        }

        int
        FindOrder(Complex[] coeff)
        {
            int o = coeff.Length - 1;
            while(coeff[o].IsZero && order > 0)
            {
                o--;
            }

            return o;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// The size of the internal coefficients data structure.
        /// </summary>
        public int Size
        {
            get { return coefficients.Length; }
        }

        /// <summary>
        /// The order of this polynomial.
        /// </summary>
        public int Order
        {
            get { return order; }
        }

        /// <summary>
        /// Get/set the coefficient for the given power.
        /// </summary>
        public Complex this[int power]
        {
            get
            {
                if(power < 0)
                {
                    throw new ArgumentOutOfRangeException("power", power, Properties.LocalStrings.ArgumentNotNegative);
                }

                if(power > order)
                {
                    return 0d;
                }

                return coefficients[power];
            }

            set
            {
                if(power < 0)
                {
                    throw new ArgumentOutOfRangeException("power", power, Properties.LocalStrings.ArgumentNotNegative);
                }

                if(power > order)
                {
                    EnsureSupportForOrder(power);
                }

                coefficients[power] = value;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Check whether two polynomials have the same coefficients.
        /// </summary>
        public static
        bool
        operator ==(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.Equals(polynomial2);
        }

        /////// <summary>
        /////// Check whether two polynomials have the same coefficients.
        /////// </summary>
        ////public static
        ////bool
        ////operator ==(
        ////    ComplexPolynomial polynomial1,
        ////    Polynomial polynomial2
        ////    )
        ////{
        ////    return polynomial1.Equals(polynomial2);
        ////}

        /////// <summary>
        /////// Check whether two polynomials have the same coefficients.
        /////// </summary>
        ////public static
        ////bool
        ////operator ==(
        ////    Polynomial polynomial1,
        ////    ComplexPolynomial polynomial2
        ////    )
        ////{
        ////    return polynomial2.Equals(polynomial1);
        ////}

        /// <summary>
        /// Check whether two polynomials have different coefficients.
        /// </summary>
        public static
        bool
        operator !=(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return !polynomial1.Equals(polynomial2);
        }

        /////// <summary>
        /////// Check whether two polynomials have different coefficients.
        /////// </summary>
        ////public static
        ////bool
        ////operator !=(
        ////    ComplexPolynomial polynomial1,
        ////    Polynomial polynomial2
        ////    )
        ////{
        ////    return !polynomial1.Equals(polynomial2);
        ////}

        /////// <summary>
        /////// Check whether two polynomials have different coefficients.
        /////// </summary>
        ////public static
        ////bool
        ////operator !=(
        ////    Polynomial polynomial1,
        ////    ComplexPolynomial polynomial2
        ////    )
        ////{
        ////    return !polynomial2.Equals(polynomial1);
        ////}

        /// <summary>
        /// Check wether a polynomial is bigger than another polynomial.
        /// </summary>
        public static
        bool
        operator >(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) > 0;
        }
        
        /// <summary>
        /// Check wether a polynomial is bigger than another polynomial.
        /// </summary>
        public static
        bool
        operator >(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) > 0;
        }

        /// <summary>
        /// Check wether a polynomial is bigger than another polynomial.
        /// </summary>
        public static
        bool
        operator >(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial2.CompareTo(polynomial1) < 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than another polynomial.
        /// </summary>
        public static
        bool
        operator <(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) < 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than another polynomial.
        /// </summary>
        public static
        bool
        operator <(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) < 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than another polynomial.
        /// </summary>
        public static
        bool
        operator <(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial2.CompareTo(polynomial1) > 0;
        }

        /// <summary>
        /// Check wether a polynomial is bigger than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator >=(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) >= 0;
        }

        /// <summary>
        /// Check wether a polynomial is bigger than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator >=(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) >= 0;
        }

        /// <summary>
        /// Check wether a polynomial is bigger than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator >=(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial2.CompareTo(polynomial1) <= 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator <=(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) <= 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator <=(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            return polynomial1.CompareTo(polynomial2) <= 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator <=(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial2.CompareTo(polynomial1) >= 0;
        }

        /// <summary>
        /// Add a polynomial to a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial1);
            ret.AddInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Add a polynomial to a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial1);
            ret.AddInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Add a polynomial to a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial2);
            ret.AddInplace(polynomial1);
            return ret;
        }

        /// <summary>
        /// Add a complex number to a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            ComplexPolynomial polynomial,
            Complex n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a polynomial to a complex number.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            Complex n,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a real number to a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            ComplexPolynomial polynomial,
            double n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a polynomial to a real number.
        /// </summary>
        public static
        ComplexPolynomial
        operator +(
            double n,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        ComplexPolynomial
        operator +(ComplexPolynomial polynomial)
        {
            return polynomial;
        }

        /// <summary>
        /// Subtract a polynomial from another polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial1);
            ret.SubtractInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Subtract a polynomial from another polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial1);
            ret.SubtractInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Subtract a polynomial from another polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial2);
            ret.NegateInplace();
            ret.AddInplace(polynomial1);
            return ret;
        }

        /// <summary>
        /// Subtract a complex number from a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            ComplexPolynomial polynomial,
            Complex n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a polynomial from a complex number.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            Complex n,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a real number from a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            ComplexPolynomial polynomial,
            double n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a polynomial from a real number.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(
            double n,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Negate a polynomial.
        /// </summary>
        public static
        ComplexPolynomial
        operator -(ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.NegateInplace();
            return ret;
        }

        /// <summary>
        /// Multiply/Convolute two polynomials.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial1.Multiply(polynomial2);
        }

        /// <summary>
        /// Multiply/Convolute two polynomials.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            ComplexPolynomial polynomial1,
            Polynomial polynomial2)
        {
            return polynomial1.Multiply(polynomial2);
        }

        /// <summary>
        /// Multiply/Convolute two polynomials.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            Polynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            return polynomial2.Multiply(polynomial1);
        }

        /// <summary>
        /// Stretch a polynomial with a complex number factor.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            ComplexPolynomial polynomial,
            Complex s)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.MultiplyInplace(s);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a complex number factor.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            Complex s,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.MultiplyInplace(s);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            ComplexPolynomial polynomial,
            double s)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.MultiplyInplace(s);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        ComplexPolynomial
        operator *(
            double s,
            ComplexPolynomial polynomial)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.MultiplyInplace(s);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a complex number quotient.
        /// </summary>
        /// <remarks>
        /// The quotient must not be null.
        /// </remarks>
        /// <exception cref="System.DivideByZeroException" />
        public static
        ComplexPolynomial
        operator /(
            ComplexPolynomial polynomial,
            Complex n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.DivideInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number quotient.
        /// </summary>
        /// <remarks>
        /// The quotient must not be null.
        /// </remarks>
        /// <exception cref="System.DivideByZeroException" />
        public static
        ComplexPolynomial
        operator /(
            ComplexPolynomial polynomial,
            double n)
        {
            ComplexPolynomial ret = new ComplexPolynomial(polynomial);
            ret.DivideInplace(n);
            return ret;
        }

        #endregion

        #region Inplace Arithmetic Methods

        /// <summary>Add another complex polynomial inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(ComplexPolynomial polynomial)
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] += polynomial.coefficients[i];
            }
        }

        /// <summary>Add a real polynomial inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(Polynomial polynomial)
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.Order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] += polynomial[i];
            }
        }

        /// <summary>Add a complex number inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(Complex s)
        {
            this[0] += s;
        }

        /// <summary>Add a real number inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(double s)
        {
            this[0] += s;
        }

        /// <summary>Subtract anoter complex polynomial inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(ComplexPolynomial polynomial)
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] -= polynomial.coefficients[i];
            }
        }

        /// <summary>Subtract a real polynomial inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(Polynomial polynomial)
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.Order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] -= polynomial[i];
            }
        }

        /// <summary>Subtract a complex number inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(Complex s)
        {
            this[0] -= s;
        }

        /// <summary>Subtract a real number inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(double s)
        {
            this[0] -= s;
        }

        /// <summary>Negate this polynomial inplace.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        NegateInplace()
        {
            for(int i = 0; i <= order; i++)
            {
                coefficients[i] = -coefficients[i];
            }
        }

        /// <summary>
        /// Multiplies this polynomial with a complex number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyInplace(Complex c0)
        {
            for(int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] = c0 * coefficients[i];
            }
        }

        /// <summary>
        /// Multiplies this polynomial with a real number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyInplace(double c0)
        {
            for(int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] = c0 * coefficients[i];
            }
        }

        /// <summary>
        /// Multiplies this polynomial with its base x^n, n>0, resulting in a coefficient shift.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyShiftInplace(int n)
        {
            if(n <= 0)
            {
                throw new ArgumentOutOfRangeException("n", n, Properties.LocalStrings.ArgumentPositive);
            }

            EnsureSupportForOrder(order + n);
            order += n;

            for(int i = order; i >= n; i--)
            {
                coefficients[i] = coefficients[i - n];
            }

            for(int i = 0; i < n; i++)
            {
                coefficients[i] = 0;
            }
        }

        /// <summary>
        /// Multiplies this polynomial with x-c0
        /// where x is its base and c0 a constant.
        /// This process is the counterpart to synthetic division.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplySyntheticInplace(Complex c0)
        {
            EnsureSupportForOrder(order + 1);
            order++;
            coefficients[order] = coefficients[order - 1];

            for(int j = order - 1; j >= 1; j--)
            {
                coefficients[j] = coefficients[j - 1] - (c0 * coefficients[j]);
            }

            coefficients[0] *= -c0;
        }

        /// <summary>
        /// Multiplies this polynomial with x-c0
        /// where x is its base and c0 a constant.
        /// This process is the counterpart to synthetic division.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplySyntheticInplace(double c0)
        {
            EnsureSupportForOrder(order + 1);
            order++;
            coefficients[order] = coefficients[order - 1];

            for(int j = order - 1; j >= 1; j--)
            {
                coefficients[j] = coefficients[j - 1] - (c0 * coefficients[j]);
            }

            coefficients[0] *= -c0;
        }

        /// <summary>
        /// Multiplies this polynomial with a linear factor c1*x+c0.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyLinearInplace(
            Complex c0,
            Complex c1)
        {
            if(c1.IsZero)
            {
                MultiplyInplace(c0);
                return;
            }

            Complex a = -c0 / c1;
            MultiplySyntheticInplace(a);
            MultiplyInplace(c1);
        }

        /// <summary>
        /// Multiplies this polynomial with a linear factor c1*x+c0.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyLinearInplace(
            double c0,
            double c1)
        {
            if(Number.AlmostZero(c1))
            {
                MultiplyInplace(c0);
                return;
            }

            double a = -c0 / c1;
            MultiplySyntheticInplace(a);
            MultiplyInplace(c1);
        }

        /// <summary>
        /// Divides this polynomial with a real number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideInplace(Complex c0)
        {
            if(c0.IsZero)
            {
                throw new DivideByZeroException();
            }

            Complex factor = 1 / c0;
            for(int i = 0; i <= order; i++)
            {
                coefficients[i] = coefficients[i] * factor;
            }
        }

        /// <summary>
        /// Divides this polynomial with a real number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideInplace(double c0)
        {
            if(Number.AlmostZero(c0))
            {
                throw new DivideByZeroException();
            }

            double factor = 1 / c0;
            for(int i = 0; i <= order; i++)
            {
                coefficients[i] = coefficients[i] * factor;
            }
        }

        /// <summary>
        /// Divides this polynomial with its base x^n, n>0, resulting in a coefficient shift.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideShiftInplace(
            int n,
            out Complex[] remainder)
        {
            if(n <= 0)
            {
                throw new ArgumentOutOfRangeException("n", n, Properties.LocalStrings.ArgumentPositive);
            }

            remainder = new Complex[n];
            for(int i = 0; i < n; i++)
            {
                remainder[i] = coefficients[i];
            }

            order -= n;
            for(int i = 0; i < order; i++)
            {
                coefficients[i] = coefficients[i + n];
            }

            for(int i = order; i < order + n; i++)
            {
                coefficients[i] = Complex.Zero;
            }

            if((n << 2) > order)
            {
                Normalize();
            }
        }

        /// <summary>
        /// Divides this polynomial with x-c0
        /// where x is its base and c0 a constant.
        /// This process is often called synthetic division.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideSyntheticInplace(
            Complex c0,
            out Complex remainder)
        {
            Complex swap;
            remainder = coefficients[order];
            coefficients[order--] = Complex.Zero;
            for(int i = order; i >= 0; i--)
            {
                swap = coefficients[i];
                coefficients[i] = remainder;
                remainder = swap + (c0 * remainder);
            }

            NormalizeOrder();
        }

        /// <summary>
        /// Divides this polynomial with a linear factor c1*x+c0.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideLinearInplace(
            Complex c0,
            Complex c1,
            out Complex remainder)
        {
            if(c1.IsZero)
            {
                DivideInplace(c0);
                remainder = Complex.Zero;
                return;
            }

            Complex a = -c0 / c1;
            DivideInplace(c1);
            DivideSyntheticInplace(a, out remainder);
        }

        #endregion

        #region Multiplicative Methods

        /// <summary>
        /// Multiply two polynomials.
        /// </summary>
        /// <remarks>
        /// If both polynomials have an order > 3, the faster karatsuba algorithm is used.
        /// </remarks>
        public
        ComplexPolynomial
        Multiply(ComplexPolynomial polynomial)
        {
            int orderMin = Math.Min(Order, polynomial.Order);

            // TODO: Measure the smallest order where karatsuba is really faster.
            // (the current ">3" is currently not based on hard measured numbers!)
            if(orderMin > 3)
            {
                int orderMax = Math.Max(Order, polynomial.Order);
                this.EnsureSupportForOrder(orderMax);
                polynomial.EnsureSupportForOrder(orderMax);

                return MultiplyKaratsuba(
                    this.coefficients,
                    polynomial.coefficients,
                    this.order,
                    polynomial.order,
                    SizeOfOrder(orderMax),
                    0);
            }

            // Direct multipliction (slow for large orders but faster for smaller ones).
            Complex[] coeff = new Complex[1 + Order + polynomial.Order];
            for(int i = 0; i <= order; i++)
            {
                for(int j = 0; j <= polynomial.order; j++)
                {
                    coeff[i + j] += coefficients[i] * polynomial.coefficients[j];
                }
            }

            return new ComplexPolynomial(coeff);
        }

        /// <summary>
        /// Multiply two polynomials.
        /// </summary>
        /// <remarks>
        /// If both polynomials have an order > 3, the faster karatsuba algorithm is used.
        /// </remarks>
        public
        ComplexPolynomial
        Multiply(Polynomial polynomial)
        {
            return Multiply(new ComplexPolynomial(polynomial));
        }

        ComplexPolynomial
        MultiplyKaratsuba(
            Complex[] leftCoefficients,
            Complex[] rightCoefficients,
            int leftOrder,
            int rightOrder,
            int n,
            int offset)
        {
            if(n == 1)
            {
                return new ComplexPolynomial(new Complex[] { leftCoefficients[offset] * rightCoefficients[offset] });
            }

            if(n == 2)
            {
                return new ComplexPolynomial(
                    new Complex[] {
                        leftCoefficients[offset] * rightCoefficients[offset],
                        (leftCoefficients[offset] * rightCoefficients[offset + 1]) + (leftCoefficients[offset + 1] * rightCoefficients[offset]),
                        leftCoefficients[offset + 1] * rightCoefficients[offset + 1]
                    });
            }

            n >>= 1;
            Complex[] FF = new Complex[n], GG = new Complex[n];
            for(int i = offset; i < n + offset; i++)
            {
                FF[i - offset] = leftCoefficients[i] + leftCoefficients[n + i];
                GG[i - offset] = rightCoefficients[i] + rightCoefficients[n + i];
            }

            ComplexPolynomial FG0 = MultiplyKaratsuba(
                leftCoefficients,
                rightCoefficients,
                n - 1,
                n - 1,
                n,
                offset);

            ComplexPolynomial FG1 = MultiplyKaratsuba(
                leftCoefficients,
                rightCoefficients,
                Math.Max(leftOrder - n, 0),
                Math.Max(rightOrder - n, 0),
                n,
                offset + n);

            ComplexPolynomial FFGG = MultiplyKaratsuba(
                FF,
                GG,
                n - 1,
                n - 1,
                n,
                0);

            FFGG.SubtractInplace(FG0);
            FFGG.SubtractInplace(FG1);
            FFGG.MultiplyShiftInplace(n);

            FG1.MultiplyShiftInplace(n + n);
            FG1.AddInplace(FFGG);
            FG1.AddInplace(FG0);

            return FG1;
        }

        /// <summary>
        /// Divides this polynomial with another polynomial.
        /// </summary>
        public
        ComplexRational
        Divide(ComplexPolynomial polynomial)
        {
            return new ComplexRational(Clone(), polynomial.Clone());
        }

        /// <summary>
        /// Divides this polynomial with another polynomial.
        /// </summary>
        public
        ComplexRational
        Divide(Polynomial polynomial)
        {
            return new ComplexRational(Clone(), new ComplexPolynomial(polynomial));
        }

        #endregion

        #region Evaluation

        /// <summary>
        /// Evaluates the real result of the polynomial to the given value. 
        /// </summary>
        /// <param name="value">The polynomial base, x.</param>
        /// <returns>The real result.</returns>
        public
        Complex
        Evaluate(Complex value)
        {
            Complex ret = coefficients[order];
            for(int j = order - 1; j >= 0; j--)
            {
                ret = (ret * value) + coefficients[j];
            }

            return ret;
        }

        /// <summary>
        /// Evaluates the real result of the polynomial and its first
        /// derivative to the given value. 
        /// </summary>
        /// <param name="value">The polynomial base, x.</param>
        /// <param name="derivative">The real result of the derivative.</param>
        /// <returns>The real result of the original polynomial.</returns>
        public
        Complex
        Evaluate(
            Complex value,
            out Complex derivative)
        {
            Complex ret = coefficients[order];
            derivative = 0d;
            for(int j = order - 1; j >= 0; j--)
            {
                derivative = (derivative * value) + ret;
                ret = (ret * value) + coefficients[j];
            }

            return ret;
        }

        /// <summary>
        /// Evaluates the real result of the polynomial and its first
        /// few derivatives to the given value. 
        /// </summary>
        /// <param name="value">The polynomial base, x.</param>
        /// <param name="derivativeOrderMax">The highest derivative order. Example: '2' evaluates the first and the second derivatives.</param>
        /// <returns>A real array with the result of the i-th derivate in cell c[i], thus the result of the original polynomial in c[0].</returns>
        public
        Complex[]
        Evaluate(
            Complex value,
            int derivativeOrderMax)
        {
            Complex[] ret = new Complex[derivativeOrderMax + 1];
            ret[0] = coefficients[order];

            int len;
            for(int i = order - 1; i >= 0; i--)
            {
                len = Math.Min(derivativeOrderMax, coefficients.Length - 1 - i);
                for(int j = len; j >= 1; j--)
                {
                    ret[j] = (ret[j] * value) + ret[j - 1];
                }

                ret[0] = (ret[0] * value) + coefficients[i];
            }

            double factorial = 1.0;
            for(int i = 2; i < ret.Length; i++)
            {
                factorial *= i;
                ret[i] *= factorial;
            }

            return ret;
        }

        #endregion

        #region String Formatting and Parsing

        /// <summary>
        /// Format a human-readable string of this polynomial with the given string as base variable (e.g. "x").
        /// </summary>
        public
        string
        ToString(string baseVariable)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = Order; i >= 0; i--)
            {
                Complex coeff = coefficients[i];

                if(coeff.IsZero)
                {
                    continue;
                }

                if(!coeff.IsReal)
                {
                    builder.Append(builder.Length > 0 ? " + (" : "(");
                    builder.Append(coeff.ToString());
                    builder.Append(')');
                }
                else
                {
                    double realCoeff = coeff.Real;
                    if(builder.Length > 0)
                    {
                        builder.Append(realCoeff > 0d ? " + " : " - ");
                    }
                    else
                    {
                        if(realCoeff < 0d)
                        {
                            builder.Append('-');
                        }
                    }

                    if(i == 0 || (!Number.AlmostEqual(realCoeff, 1) && !Number.AlmostEqual(realCoeff, -1)))
                    {
                        builder.Append(Math.Abs(realCoeff));
                    }
                }

                if(i > 0)
                {
                    builder.Append(" " + baseVariable);
                }

                if(i > 1)
                {
                    builder.Append('^');
                    builder.Append(i);
                }
            }

            if(builder.Length == 0)
            {
                builder.Append('0');
            }

            return builder.ToString();
        }

        /// <summary>
        /// Format a human-readable string of this polynomial with "x" as base variable.
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
        /// Serves as a hash function for polynomials.
        /// </summary>
        public override
        int
        GetHashCode()
        {
            return coefficients.GetHashCode();
        }

        /// <summary>
        /// Check whether this polynomial is equal to another polynomial.
        /// </summary>
        public override
        bool
        Equals(object obj)
        {
            ComplexPolynomial p = obj as ComplexPolynomial;
            return p == null ? false : Equals(p);
        }

        /// <summary>
        /// Check whether this polynomial is equal to another polynomial.
        /// </summary>
        public
        bool
        Equals(ComplexPolynomial polynomial)
        {
            return CompareTo(polynomial) == 0;
        }

        /// <summary>
        /// Check whether two polynomials are equal.
        /// </summary>
        public static
        bool
        Equals(
            ComplexPolynomial polynomial1,
            ComplexPolynomial polynomial2)
        {
            if(polynomial1 == null)
            {
                return polynomial2 == null;
            }

            return polynomial1.Equals(polynomial2);
        }

        /// <summary>
        /// Compare this polynomial to another polynomial.
        /// </summary>
        public
        int
        CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            if(!(obj is ComplexPolynomial))
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentTypeMismatch, "obj");
            }

            return CompareTo((ComplexPolynomial)obj);
        }

        /// <summary>
        /// Compare this polynomial to another polynomial.
        /// </summary>
        public
        int
        CompareTo(ComplexPolynomial polynomial)
        {
            int i = this.Order;
            int j = polynomial.Order;

            if(i > j)
            {
                return 1;
            }

            if(j > i)
            {
                return -1;
            }

            while(i >= 0)
            {
                if(this.coefficients[i] > polynomial.coefficients[i])
                {
                    return 1;
                }

                if(this.coefficients[i] < polynomial.coefficients[i])
                {
                    return -1;
                }

                i--;
            }

            return 0;
        }

        /// <summary>
        /// Create a copy of this polynomial.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a copy of this polynomial.
        /// </summary>
        public
        ComplexPolynomial
        Clone()
        {
            return new ComplexPolynomial(this);
        }

        #endregion
    }
}
