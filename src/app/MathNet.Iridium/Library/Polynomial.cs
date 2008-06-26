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
    /// Polynomial represents a finite order polynomial
    /// with positive powers and constant real coefficients.
    /// </summary>
    /// <remarks>The polynomial coefficients are ordered such that
    /// c[0] is the constant term and c[n] is the coefficient of z^n,
    /// that is y = c[0]*x^0+c[1]*x^1+c[2]*x^2+...</remarks>
    public class Polynomial :
        IComparable,
        ICloneable
    {
        double[] coefficients;
        int order;

        /// <summary>
        /// Create a new polynomial by order
        /// </summary>
        /// <param name="order">The highest power. Example: 2*x^3+x-3 has order 3.</param>
        public
        Polynomial(
            int order
            )
        {
            this.order = order;
            this.coefficients = new double[SizeOfOrder(order)];
        }

        /// <summary>
        /// Create a new polynomial by coefficients
        /// </summary>
        /// <param name="coefficients">The coefficients vector. The coefficient index denotes the related power (c[0]*x^0+c[1]*x^1+..)</param>
        public
        Polynomial(
            double[] coefficients
            )
        {
            this.order = FindOrder(coefficients);
            this.coefficients = new double[SizeOfOrder(order)];
            Array.Copy(
                coefficients,
                this.coefficients,
                Math.Min(this.coefficients.Length, coefficients.Length)
                );
        }

        /// <summary>
        /// Create a new polynomial by copy
        /// </summary>
        /// <param name="copy">A polynomial to copy from.</param>
        public
        Polynomial(
            Polynomial copy
            )
        {
            this.order = copy.order;
            this.coefficients = new double[copy.coefficients.Length];
            Array.Copy(
                copy.coefficients,
                this.coefficients,
                Math.Min(this.coefficients.Length, copy.coefficients.Length)
                );
        }

        #region Size

        static
        int
        SizeOfOrder(
            int order
            )
        {
            return 1 << (int)Math.Ceiling(Math.Log(order + 1, 2));
        }
        
        // TODO: Never called, remove?
        ////void
        ////ResizeDouble()
        ////{
        ////    double[] newCoeffs = new double[coefficients.Length + coefficients.Length];
        ////    coefficients.CopyTo(newCoeffs, 0);
        ////    coefficients = newCoeffs;
        ////}

        void
        ResizeOptimalForOrder(
            int order
            )
        {
            int bestSize = SizeOfOrder(order);
            if(bestSize == coefficients.Length)
            {
                return;
            }

            double[] newCoeffs = new double[bestSize];
            Array.Copy(
                this.coefficients,
                newCoeffs,
                Math.Min(this.coefficients.Length, newCoeffs.Length)
                );
            coefficients = newCoeffs;
        }
        
        void
        EnsureSupportForOrder(
            int order
            )
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
            while(coefficients[order] == 0d && order > 0)
            {
                order--;
            }
        }

        int
        FindOrder(
            double[] coeff
            )
        {
            int o = coeff.Length - 1;
            while(coeff[o] == 0d && order > 0)
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
        public double this[int power]
        {
            get
            {
                if(power < 0)
                {
                    throw new ArgumentOutOfRangeException("power", power, Resources.ArgumentNotNegative);
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
                    throw new ArgumentOutOfRangeException("power", power, Resources.ArgumentNotNegative);
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
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            return polynomial1.Equals(polynomial2);
        }

        /// <summary>
        /// Check whether two polynomials have different coefficients.
        /// </summary>
        public static
        bool
        operator !=(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            return !polynomial1.Equals(polynomial2);
        }

        /// <summary>
        /// Check wether a polynomial is bigger than another polynomial.
        /// </summary>
        public static
        bool
        operator >(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            return polynomial1.CompareTo(polynomial2) == 1;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than another polynomial.
        /// </summary>
        public static
        bool
        operator <(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            return polynomial1.CompareTo(polynomial2) == -1;
        }

        /// <summary>
        /// Check wether a polynomial is bigger than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator >=(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            int res = polynomial1.CompareTo(polynomial2);
            return res == 1 || res == 0;
        }

        /// <summary>
        /// Check wether a polynomial is smaller than or equal to another polynomial.
        /// </summary>
        public static
        bool
        operator <=(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            int res = polynomial1.CompareTo(polynomial2);
            return res == -1 || res == 0;
        }


        /// <summary>
        /// Add a polynomials to a polynomial.
        /// </summary>
        public static
        Polynomial
        operator +(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            Polynomial ret = new Polynomial(polynomial1);
            ret.AddInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Add a real number to a polynomial.
        /// </summary>
        public static
        Polynomial
        operator +(
            Polynomial polynomial,
            double n
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a polynomial to a real number.
        /// </summary>
        public static
        Polynomial
        operator +(
            double n,
            Polynomial polynomial
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        Polynomial
        operator +(
            Polynomial polynomial
            )
        {
            return polynomial;
        }


        /// <summary>
        /// Subtract a polynomial from another polynomial.
        /// </summary>
        public static
        Polynomial
        operator -(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            Polynomial ret = new Polynomial(polynomial1);
            ret.SubtractInplace(polynomial2);
            return ret;
        }

        /// <summary>
        /// Subtract a real number from a polynomial.
        /// </summary>
        public static
        Polynomial
        operator -(
            Polynomial polynomial,
            double n
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a polynomial from a real number.
        /// </summary>
        public static
        Polynomial
        operator -(
            double n,
            Polynomial polynomial
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Negate a polynomial.
        /// </summary>
        public static
        Polynomial
        operator -(
            Polynomial polynomial
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.NegateInplace();
            return ret;
        }


        /// <summary>
        /// Multiply/Convolute two polynomials.
        /// </summary>
        public static
        Polynomial
        operator *(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
        {
            return polynomial1.Multiply(polynomial2);
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        Polynomial
        operator *(
            Polynomial polynomial,
            double n
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.MultiplyInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        Polynomial
        operator *(
            double n,
            Polynomial polynomial
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.MultiplyInplace(n);
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
        Polynomial
        operator /(
            Polynomial polynomial,
            double n
            )
        {
            Polynomial ret = new Polynomial(polynomial);
            ret.DivideInplace(n);
            return ret;
        }

        #endregion

        #region Inplace Arithmetic Methods

        /// <summary>Add anoter polynomial inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(
            Polynomial polynomial
            )
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] += polynomial.coefficients[i];
            }
        }

        /// <summary>Add a real number inplace to this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        AddInplace(
            double n
            )
        {
            this[0] += n;
        }

        /// <summary>Subtract anoter polynomial inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(
            Polynomial polynomial
            )
        {
            EnsureSupportForOrder(polynomial.Order);
            int len = Math.Min(order, polynomial.order) + 1;
            for(int i = 0; i < len; i++)
            {
                coefficients[i] -= polynomial.coefficients[i];
            }
        }

        /// <summary>Subtract a real number inplace from this polynomial.</summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        SubtractInplace(
            double n
            )
        {
            this[0] -= n;
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
        /// Multiply two small polynomials.
        /// </summary>
        public
        Polynomial
        MultiplySlow(
            Polynomial polynomial
            )
        {
            double[] coeff = new double[1 + Order + polynomial.Order];
            for(int i = 0; i <= order; i++)
            {
                for(int j = 0; j <= polynomial.order; j++)
                {
                    coeff[i + j] += coefficients[i] * polynomial.coefficients[j];
                }
            }

            return new Polynomial(coeff);
        }

        /// <summary>
        /// Multiply two polynomials.
        /// </summary>
        /// <remarks>
        /// If both polynomials have an order > 3, the faster karatsua algorithm is used.
        /// </remarks>
        public
        Polynomial
        Multiply(
            Polynomial polynomial
            )
        {
            int orderMin = Math.Min(Order, polynomial.Order);
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
                    0
                    );
            }
            else
            {
                double[] coeff = new double[1 + Order + polynomial.Order];
                for(int i = 0; i <= order; i++)
                {
                    for(int j = 0; j <= polynomial.order; j++)
                    {
                        coeff[i + j] += coefficients[i] * polynomial.coefficients[j];
                    }
                }

                return new Polynomial(coeff);
            }
        }

        Polynomial
        MultiplyKaratsuba(
            double[] leftCoefficients,
            double[] rightCoefficients,
            int leftOrder,
            int rightOrder,
            int n,
            int offset
            )
        {
            if(n == 1)
            {
                return new Polynomial(new double[] { leftCoefficients[offset] * rightCoefficients[offset] });
            }

            if(n == 2)
            {
                return new Polynomial(
                    new double[] {
                        leftCoefficients[offset] * rightCoefficients[offset],
                        leftCoefficients[offset] * rightCoefficients[offset + 1] + leftCoefficients[offset + 1] * rightCoefficients[offset],
                        leftCoefficients[offset + 1] * rightCoefficients[offset + 1]
                    });
            }

            n >>= 1;
            double[] FF = new double[n], GG = new double[n];
            for(int i = offset; i < n + offset; i++)
            {
                FF[i - offset] = leftCoefficients[i] + leftCoefficients[n + i];
                GG[i - offset] = rightCoefficients[i] + rightCoefficients[n + i];
            }

            Polynomial FG0 = MultiplyKaratsuba(
                leftCoefficients,
                rightCoefficients,
                n - 1,
                n - 1,
                n,
                offset
                );

            Polynomial FG1 = MultiplyKaratsuba(
                leftCoefficients,
                rightCoefficients,
                Math.Max(leftOrder - n, 0),
                Math.Max(rightOrder - n, 0),
                n,
                offset + n
                );

            Polynomial FFGG = MultiplyKaratsuba(
                FF,
                GG,
                n - 1,
                n - 1,
                n,
                0
                );

            FFGG.SubtractInplace(FG0);
            FFGG.SubtractInplace(FG1);
            FFGG.MultiplyShiftInplace(n);

            FG1.MultiplyShiftInplace(n + n);
            FG1.AddInplace(FFGG);
            FG1.AddInplace(FG0);

            return FG1;
        }

        /// <summary>
        /// Multiplies this polynomial with a real number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyInplace(
            double c0
            )
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
        MultiplyShiftInplace(
            int n
            )
        {
            if(n <= 0)
            {
                throw new ArgumentOutOfRangeException("n", n, Resources.ArgumentPositive);
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
        /// Multiplies this polynomial with x-a
        /// where x is its base and c0 a constant.
        /// This process is the counterpart to synthetic division.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplySyntheticInplace(
            double a
            )
        {
            EnsureSupportForOrder(order + 1);
            order++;
            coefficients[order] = coefficients[order - 1];

            for(int j = order - 1; j >= 1; j--)
            {
                coefficients[j] = coefficients[j - 1] - a * coefficients[j];
            }

            coefficients[0] *= -a;
        }

        /// <summary>
        /// Multiplies this polynomial with a linear factor c1*x+c0.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        MultiplyLinearInplace(
            double c0,
            double c1
            )
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
        /// Divides this polynomial with anoter polynomial.
        /// </summary>
        public
        Rational
        Divide(
            Polynomial polynomial
            )
        {
            return new Rational(Clone(), polynomial.Clone());
        }

        /// <summary>
        /// Divides this polynomial with a real number.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideInplace(
            double c0
            )
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
            out double[] reminder
            )
        {
            if(n <= 0)
            {
                throw new ArgumentOutOfRangeException("n", n, Resources.ArgumentPositive);
            }

            reminder = new double[n];
            for(int i = 0; i < n; i++)
            {
                reminder[i] = coefficients[i];
            }

            order -= n;
            for(int i = 0; i < order; i++)
            {
                coefficients[i] = coefficients[i + n];
            }

            for(int i = order; i < order + n; i++)
            {
                coefficients[i] = 0;
            }

            if((n << 2) > order)
            {
                Normalize();
            }
        }

        /// <summary>
        /// Divides this polynomial with x-a
        /// where x is its base and c0 a constant.
        /// This process is often called synthetic division.
        /// </summary>
        /// <remarks>This method operates inplace and thus alters this instance.</remarks>
        public
        void
        DivideSyntheticInplace(
            double a,
            out double reminder
            )
        {
            double swap;
            reminder = coefficients[order];
            coefficients[order--] = 0;
            for(int i = order; i >= 0; i--)
            {
                swap = coefficients[i];
                coefficients[i] = reminder;
                reminder = swap + a * reminder;
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
            double c0,
            double c1,
            out double reminder
            )
        {
            if(Number.AlmostZero(c1))
            {
                DivideInplace(c0);
                reminder = 0d;
                return;
            }

            double a = -c0 / c1;
            DivideInplace(c1);
            DivideSyntheticInplace(a, out reminder);
        }
        #endregion

        #region Evaluation

        /// <summary>
        /// Evaluates the real result of the polynomial to the given value. 
        /// </summary>
        /// <param name="value">The polynomial base, x.</param>
        /// <returns>The real result.</returns>
        public
        double
        Evaluate(
            double value
            )
        {
            double ret = coefficients[order];
            for(int j = order - 1; j >= 0; j--)
            {
                ret = ret * value + coefficients[j];
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
        double
        Evaluate(
            double value,
            out double derivative
            )
        {
            double ret = coefficients[order];
            derivative = 0d;
            for(int j = order - 1; j >= 0; j--)
            {
                derivative = derivative * value + ret;
                ret = ret * value + coefficients[j];
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
        double[]
        Evaluate(
            double value,
            int derivativeOrderMax
            )
        {
            double[] ret = new double[derivativeOrderMax + 1];
            ret[0] = coefficients[order];

            int len;
            for(int i = order - 1; i >= 0; i--)
            {
                len = Math.Min(derivativeOrderMax, coefficients.Length - 1 - i);
                for(int j = len; j >= 1; j--)
                {
                    ret[j] = ret[j] * value + ret[j - 1];
                }

                ret[0] = ret[0] * value + coefficients[i];
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
        ToString(
            string baseVariable
            )
        {
            StringBuilder builder = new StringBuilder();
            for(int i = Order; i >= 0; i--)
            {
                double coeff = coefficients[i];

                if(Number.AlmostZero(coeff))
                {
                    continue;
                }

                if(builder.Length > 0)
                {
                    builder.Append(coeff > 0d ? " + " : " - ");
                }
                else
                {
                    if(coeff < 0d)
                    {
                        builder.Append('-');
                    }
                }

                if(!Number.AlmostEqual(coeff, 1) && !Number.AlmostEqual(coeff, -1) || i == 0)
                {
                    builder.Append(Math.Abs(coeff));
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
        Equals(
            object obj
            )
        {
            Polynomial p = obj as Polynomial;
            return p == null ? false : Equals(p);
        }

        /// <summary>
        /// Check whether this polynomial is equal to another polynomial.
        /// </summary>
        public
        bool
        Equals(
            Polynomial polynomial
            )
        {
            return CompareTo(polynomial) == 0;
        }

        /// <summary>
        /// Check whether two polynomials are equal.
        /// </summary>
        public static
        bool
        Equals(
            Polynomial polynomial1,
            Polynomial polynomial2
            )
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
        CompareTo(
            object obj
            )
        {
            if(obj == null)
            {
                return 1;
            }

            if(!(obj is Polynomial))
            {
                throw new ArgumentException(Resources.ArgumentTypeMismatch, "obj");
            }

            return CompareTo((Polynomial)obj);
        }

        /// <summary>
        /// Compare this polynomial to another polynomial.
        /// </summary>
        public
        int
        CompareTo(
            Polynomial polynomial
            )
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
        Polynomial
        Clone()
        {
            return new Polynomial(this);
        }

        #endregion
    }
}
