#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.Standard.Analysis;
using MathNet.Symbolics.Packages.Standard.Algebra;
using MathNet.Symbolics.Packages.Standard.Arithmetics;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Conversion;

namespace MathNet.Symbolics.Packages.Standard
{
    public static class Polynomial
    {
        #region Classification
        /// <summary>
        /// Checks whether a signal is a single variable monomial, e.g. '2*x^3'
        /// </summary>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified</remarks>
        public static bool IsMonomial(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(Std.IsAlwaysRational(signal))
                return true;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && Std.IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
                signal = signal.DrivenByPort.InputSignals[1];
            if(signal.Equals(variable))
                return true;
            if(signal.IsDrivenByPortEntity("Power", "Std"))
            {
                Signal b = signal.DrivenByPort.InputSignals[0];
                Signal e = signal.DrivenByPort.InputSignals[1];
                if(b.Equals(variable) && Std.IsAlwaysPositiveInteger(e))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether a signal is a single variable polynomial, e.g. '3*x^2+4*x-1'
        /// </summary>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static bool IsPolynomial(Signal signal, Signal variable)
        {
            if(signal.IsDrivenByPortEntity("Add", "Std"))
                return signal.DrivenByPort.InputSignals.TrueForAll(delegate(Signal s) { return IsMonomial(s, variable); });
            else
                return IsMonomial(signal, variable);
        }

        /// <summary>
        /// Checks whether a signal is a single variable rational, a fraction of polynomials.
        /// </summary>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static bool IsRational(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Divide", "Std"))
                return IsPolynomial(Std.Numerator(signal), variable) && IsPolynomial(Std.Denominator(signal), variable);
            else
                return IsPolynomial(signal, variable);
        }
        #endregion

        #region Degree
        /// <summary>
        /// Evaluates the degree of the single-variable monomial <see cref="signal"/>.
        /// </summary>
        /// <returns>
        /// <see cref="UndefinedSymbol"/> if <see cref="signal"/> is not a single-variable monomial.
        /// <see cref="NegativeInfinitySymbol"/> if <see cref="signal"/> is zero.
        /// Otherwise an <see cref="IntegerValue"/> representing the asked degree.
        /// </returns>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static IValueStructure MonomialDegree(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(Std.IsConstantAdditiveIdentity(signal))
                return NegativeInfinitySymbol.Instance;
            if(Std.IsAlwaysRational(signal))
                return IntegerValue.Zero;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && Std.IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
                signal = signal.DrivenByPort.InputSignals[1];
            if(signal.Equals(variable))
                return IntegerValue.One;
            if(signal.IsDrivenByPortEntity("Power", "Std"))
            {
                Signal b = signal.DrivenByPort.InputSignals[0];
                Signal e = signal.DrivenByPort.InputSignals[1];
                if(b.Equals(variable) && Std.IsAlwaysPositiveInteger(e))
                    return e.Value;
            }
            return UndefinedSymbol.Instance;
        }

        /// <summary>
        /// Evaluates the degree of the single-variable polynomial <see cref="signal"/>.
        /// </summary>
        /// <returns>
        /// <see cref="UndefinedSymbol"/> if <see cref="signal"/> is not a single-variable polynomial.
        /// <see cref="NegativeInfinitySymbol"/> if <see cref="signal"/> is zero.
        /// Otherwise an <see cref="IntegerValue"/> representing the asked degree.
        /// </returns>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static IValueStructure PolynomialDegree(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Add", "Std"))
            {
                IntegerValue d = IntegerValue.Zero;
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    IValueStructure f = MonomialDegree(inputs[i], variable);
                    if(f is UndefinedSymbol)
                        return f;
                    else if(!(f is NegativeInfinitySymbol))
                        d = d.Max((IntegerValue)f);
                }
                return d;
            }
            else
                return MonomialDegree(signal, variable);
        }
        #endregion

        #region Coefficients
        /// <summary>
        /// Returns the coefficient factor in the monomial <see cref="signal"/>
        /// </summary>
        /// <returns>
        /// Constant UndefinedSymbol if <see cref="signal"/> is not a single-variable monomial.
        /// Otherwise the coefficient factor of the term.
        /// </returns>
        /// </returns>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static Signal MonomialCoefficient(Signal signal, Signal variable, out IValueStructure degree)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(Std.IsConstantAdditiveIdentity(signal))
            {
                degree = NegativeInfinitySymbol.Instance;
                return signal;
            }
            if(Std.IsAlwaysRational(signal))
            {
                degree = IntegerValue.Zero;
                return signal;
            }
            Signal coeff = IntegerValue.ConstantOne;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && Std.IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
            {
                coeff = signal.DrivenByPort.InputSignals[0];
                signal = signal.DrivenByPort.InputSignals[1];
            }
            if(signal.Equals(variable))
            {
                degree = IntegerValue.One;
                return coeff;
            }
            if(signal.IsDrivenByPortEntity("Power", "Std"))
            {
                Signal b = signal.DrivenByPort.InputSignals[0];
                Signal e = signal.DrivenByPort.InputSignals[1];
                if(b.Equals(variable) && Std.IsAlwaysPositiveInteger(e))
                {
                    degree = e.Value;
                    return coeff;
                }
            }
            degree = UndefinedSymbol.Instance;
            return UndefinedSymbol.Constant;
        }

        /// <summary>
        /// Returns the coefficient u[exponent] of x^exponent in the polynomial <see cref="signal"/>
        /// </summary>
        /// <returns>
        /// Constant UndefinedSymbol if <see cref="signal"/> is not a single-variable polynomial.
        /// Constant <see cref="IntegerValue.Zero"/> if there's no summand with the given exponent.
        /// Otherwise the sum of all coefficient factors of the term with the given exponent.
        /// </returns>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static Signal PolynomialCoefficient(Signal signal, Signal variable, int exponent)
        {
            IValueStructure vs;
            Signal c = MonomialCoefficient(signal, variable, out vs);
            if(!(vs is UndefinedSymbol))
            {
                IntegerValue iv = vs as IntegerValue;
                if(iv == null || iv.Value == exponent)
                    return c;
                else
                    return IntegerValue.ConstantZero;
            }
            if(signal.IsDrivenByPortEntity("Add", "Std"))
            {
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                List<Signal> coeffs = new List<Signal>(4);
                for(int i = 0; i < inputs.Count; i++)
                {
                    c = MonomialCoefficient(inputs[i], variable, out vs);
                    IntegerValue iv = vs as IntegerValue;
                    if(iv != null && iv.Value == exponent)
                        coeffs.Add(c);
                }
                return Std.Add(coeffs);
            }
            return UndefinedSymbol.Constant;
        }

        public static Signal PolynomialLeadingCoefficient(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            IValueStructure degree = PolynomialDegree(signal, variable);
            IntegerValue iv = degree as IntegerValue;
            if(iv != null)
                return PolynomialCoefficient(signal, variable, (int)iv.Value);
            if(degree is NegativeInfinitySymbol)
                return IntegerValue.ConstantZero;
            return UndefinedSymbol.Constant;
        }

        /// <summary>
        /// Extracts all coefficients of the polynomial <see cref="signal"/>.
        /// </summary>
        /// <returns>A signal array [c0,c1,c2,..] where ci ist the coefficient of x^i.</returns>
        public static Signal[] PolynomialCoefficients(Signal signal, Signal variable)
        {
            IValueStructure vs;
            SortedList<long, Signal> polynom = new SortedList<long, Signal>();

            Signal c = MonomialCoefficient(signal, variable, out vs);
            if(!(vs is UndefinedSymbol))
            {
                IntegerValue iv = vs as IntegerValue;
                if(iv != null)
                    polynom.Add(iv.Value, c);
                else
                    return new Signal[] { IntegerValue.ConstantZero };
            }
            else if(signal.IsDrivenByPortEntity("Add", "Std"))
            {
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    c = MonomialCoefficient(inputs[i], variable, out vs);
                    IntegerValue iv = vs as IntegerValue;
                    if(iv != null)
                        if(!polynom.ContainsKey(iv.Value))
                            polynom.Add(iv.Value, c);
                        else
                            polynom[iv.Value] = Std.Add(polynom[iv.Value], c);
                }
            }
            if(polynom.Keys.Count == 0)
                return new Signal[] { };
            long deg = polynom.Keys[polynom.Keys.Count - 1];
            Signal[] ret = new Signal[deg + 1];
            Signal zero = IntegerValue.ConstantZero;
            for(int i = 0; i < ret.Length; i++)
                if(polynom.ContainsKey(i))
                    ret[i] = polynom[i];
                else
                    ret[i] = zero;
            return ret;
        }

        /// <summary>
        /// The height of a polynomial is the maxium of the absolute values of its coefficients.
        /// </summary>
        public static IValueStructure PolynomialHeight(Signal signal, Signal variable)
        {
            Signal[] coefficients = PolynomialCoefficients(signal, variable);
            if(coefficients.Length == 0)
                return UndefinedSymbol.Instance;
            RationalValue max = RationalValue.ConvertFrom(coefficients[0].Value).Absolute();
            for(int i = 1; i < coefficients.Length; i++)
            {
                RationalValue next = RationalValue.ConvertFrom(coefficients[i].Value).Absolute();
                if(next > max)
                    max = next;
            }
            return max;
        }
        #endregion

        #region Construction
        public static Signal ConstructPolynomial<TCoeff>(Signal variable, params TCoeff[] coefficients)
            where TCoeff : IAlgebraicRingWithUnity<TCoeff>, IValueStructure
        {
            Signal zero = null;
            Signal[] summands = new Signal[coefficients.Length];
            for(int i = 0; i < summands.Length; i++)
            {
                TCoeff c = coefficients[i];
                if(c.IsAdditiveIdentity)
                {
                    if(zero == null)
                        zero = Std.DefineConstant(c);
                    summands[i] = zero;
                    continue;
                }

                Signal coeff = Std.DefineConstant(c);

                if(i == 0)
                    summands[0] = coeff;
                else if(i == 1)
                {
                    if(c.IsMultiplicativeIdentity)
                        summands[1] = variable;
                    else
                        summands[1] = StdBuilder.Multiply(coeff, variable);
                }
                else
                {
                    if(c.IsMultiplicativeIdentity)
                        summands[i] = StdBuilder.Power(variable, IntegerValue.Constant(i));
                    else
                        summands[i] = StdBuilder.Multiply(coeff, StdBuilder.Power(variable, IntegerValue.Constant(i)));
                }
            }
            return Std.Add(summands);
        }

        public static Signal ConstructPolynomial(Signal variable, params Signal[] coefficients)
        {
            Signal[] summands = new Signal[coefficients.Length];
            for(int i = 0; i < summands.Length; i++)
            {
                Signal coeff = coefficients[i];
                if(Std.IsConstantAdditiveIdentity(coeff))
                {
                    summands[i] = coeff;
                    continue;
                }

                if(i == 0)
                    summands[0] = coeff;
                else if(i == 1)
                {
                    if(Std.IsConstantMultiplicativeIdentity(coeff))
                        summands[1] = variable;
                    else
                        summands[1] = StdBuilder.Multiply(coeff, variable);
                }
                else
                {
                    if(Std.IsConstantMultiplicativeIdentity(coeff))
                        summands[i] = StdBuilder.Power(variable, IntegerValue.Constant(i));
                    else
                        summands[i] = StdBuilder.Multiply(coeff, StdBuilder.Power(variable, IntegerValue.Constant(i)));
                }
            }
            return Std.Add(summands);
        }
        #endregion

        #region Polynomial Division
        /// <remarks>
        /// This method manipulates the nummerator array. If you need to keep this unchanged then
        /// copy it yourself and provide the copy to this method call.
        /// </remarks>
        public static TCoeff[] PolynomialDivision<TCoeff>(TCoeff[] numerator, TCoeff[] denominator, out TCoeff[] remainder)
            where TCoeff : IAlgebraicField<TCoeff>, IValueStructure
        {
            TCoeff[] B = denominator;
            TCoeff[] R = numerator;

            int degB = B.Length - 1;
            TCoeff lcB = B[degB];
            TCoeff[] Q = new TCoeff[R.Length - degB];

            for(int delta = R.Length - 1 - degB; delta >= 0; delta--)
            {
                int degR = degB + delta;
                if(R[degR].IsAdditiveIdentity)
                {
                    Q[delta] = R[degR];
                    continue;
                }
                TCoeff T = R[degR].Divide(lcB);
                Q[delta] = T;
                R[degB + delta] = (TCoeff)lcB.AdditiveIdentity;
                for(int i = delta; i < degB + delta; i++)
                    R[i] = R[i].Subtract(B[i - delta].Multiply(T));
            }

            remainder = new TCoeff[Q.Length];
            for(int i = 0; i < remainder.Length; i++)
                remainder[i] = R[i];
            return Q;
        }

        public static Signal PolynomialDivision(Signal numerator, Signal denominator, Signal variable, out Signal remainder)
        {
            Signal[] R = PolynomialCoefficients(numerator, variable);
            Signal[] B = PolynomialCoefficients(denominator, variable);

            int degB = B.Length - 1;
            Signal lcB = B[degB];
            Signal[] Q = new Signal[R.Length - degB];

            for(int delta = R.Length - 1 - degB; delta >= 0; delta--)
            {
                int degR = degB + delta;
                if(Std.IsConstantAdditiveIdentity(R[degR]))
                {
                    Q[delta] = R[degR];
                    continue;
                }
                Signal T = Std.Divide(R[degR], lcB);
                Q[delta] = T;
                R[degB + delta] = IntegerValue.ConstantZero;
                for(int i = delta; i < degB + delta; i++)
                    R[i] = Std.Subtract(R[i], Std.Multiply(B[i - delta], T));
            }

            remainder = ConstructPolynomial(variable, R);
            return ConstructPolynomial(variable, Q);
        }
        #endregion
    }
}
