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

namespace MathNet.Symbolics.Packages.Standard
{
    /// <summary>
    /// Standard Package Operations & Transformations.
    /// </summary>
    /// <remarks>
    /// Use <c>StdBuilder</c> instead if you only want to map operations to a set
    /// of signals but not actually execute them (now).
    /// </remarks>
    public static class Std
    {
        private static ITransformer _transformer = Service<ITransformer>.Instance;

        #region Simplification
        public static Signal AutoSimplify(Signal signal)
        {
            return _transformer.Transform(signal, AutoSimplifyTransformation.TransformationTypeIdentifier, false);
        }
        public static SignalSet AutoSimplify(IEnumerable<Signal> signals)
        {
            return _transformer.Transform(signals, AutoSimplifyTransformation.TransformationTypeIdentifier, false);
        }
        #endregion

        #region Algebra
        /// <summary>
        /// Separates factors in a product that depend on x from those that do not.
        /// </summary>
        /// <returns>
        /// A signal array [a,b], where a is the product of the factors not
        /// depending on x, and b the product of those depending on x.
        /// </returns>
        /// <remarks><see cref="product"/> is assumed to be automatic simplified</remarks>
        public static Signal[] SeparateFactors(Signal product, Signal x)
        {
            SignalSet freePart = new SignalSet();
            SignalSet dependentPart = new SignalSet();
            if(product.IsDrivenByPortEntity("Multiply", "Std"))
            {
                ISignalSet factors = product.DrivenByPort.InputSignals;
                foreach(Signal s in factors)
                {
                    if(s.DependsOn(x))
                        dependentPart.Add(s);
                    else
                        freePart.Add(s);
                }
            }
            else if(product.DependsOn(x))
                dependentPart.Add(product);
            else
                freePart.Add(product);
            Signal freeSignal = Multiply(freePart);
            Signal dependentSignal = Multiply(dependentPart);
            return new Signal[] { freeSignal, dependentSignal };
        }

        public static Signal Numerator(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Divide", "Std"))
                return signal.DrivenByPort.InputSignals[0];
            return signal;
        }

        public static Signal Denominator(Signal signal)
        {
            if(signal.IsDrivenByPortEntity("Divide", "Std") && signal.DrivenByPort.InputSignalCount > 1)
            {
                if(signal.DrivenByPort.InputSignals.Count == 2)
                    return signal.DrivenByPort.InputSignals[1];

                List<Signal> factors = new List<Signal>();
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                for(int i = 1; i < inputs.Count; i++) //all but first element
                    factors.Add(inputs[i]);
                return Multiply(factors);
            }
            return IntegerValue.ConstantOne;
        }
        #endregion

        #region Single Variable Polynomials
        /// <summary>
        /// Checks whether a signal is a single variable monomial, e.g. '2*x^3'
        /// </summary>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified</remarks>
        public static bool IsMonomial(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(IsAlwaysRational(signal))
                return true;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
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
        /// Checks whether a signal is a signle variable rational, a fraction of polynomials.
        /// </summary>
        /// <remarks><see cref="signal"/> is assumed to be automatic simplified.</remarks>
        public static bool IsRational(Signal signal, Signal variable)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Divide", "Std"))
                return IsPolynomial(Numerator(signal), variable) && IsPolynomial(Denominator(signal), variable);
            else
                return IsPolynomial(signal, variable);
        }

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

            if(IsConstantAdditiveIdentity(signal))
                return NegativeInfinitySymbol.Instance;
            if(IsAlwaysRational(signal))
                return IntegerValue.Zero;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
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
                    IValueStructure f = MonomialDegree(inputs[i],variable);
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

            if(IsConstantAdditiveIdentity(signal))
            {
                degree = NegativeInfinitySymbol.Instance;
                return signal;
            }
            if(IsAlwaysRational(signal))
            {
                degree = IntegerValue.Zero;
                return signal;
            }
            Signal coeff = IntegerValue.ConstantOne;
            if(signal.IsDrivenByPortEntity("Multiply", "Std") && signal.DrivenByPort.InputSignalCount == 2 && IsAlwaysRational(signal.DrivenByPort.InputSignals[0]))
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
                return Add(coeffs);
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
                            polynom[iv.Value] = Add(polynom[iv.Value], c);
                }
            }
            if(polynom.Keys.Count == 0)
                return new Signal[] { };
            long deg = polynom.Keys[polynom.Keys.Count - 1];
            Signal[] ret = new Signal[deg];
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

        #region General Polynomials
        private static bool IsMonomialFactor(Signal signal, Signal[] generalizedVariables)
        {
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return true;
            if(!Array.Exists<Signal>(generalizedVariables, signal.DependsOn))
                return true;
            if(signal.IsDrivenByPortEntity("Power", "Std"))
            {
                Signal b = signal.DrivenByPort.InputSignals[0];
                Signal e = signal.DrivenByPort.InputSignals[1];
                if(Array.Exists<Signal>(generalizedVariables, b.Equals) && Std.IsAlwaysPositiveInteger(e))
                    return true;
            }
            return false;
        }
        public static bool IsMonomial(Signal signal, Signal[] generalizedVariables)
        {
            if(signal.IsDrivenByPortEntity("Multiply", "Std"))
                return signal.DrivenByPort.InputSignals.TrueForAll(delegate(Signal s) {return IsMonomialFactor(s, generalizedVariables);});
            else
                return IsMonomialFactor(signal, generalizedVariables);
        }
        public static bool IsPolynomial(Signal signal, Signal[] generalizedVariables)
        {
            if(!signal.IsDrivenByPortEntity("Add", "Std"))
                return IsMonomial(signal, generalizedVariables);
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return true;
            return signal.DrivenByPort.InputSignals.TrueForAll(delegate(Signal s) { return IsMonomial(s, generalizedVariables); });
        }

        public static Signal[] PolynomialVariables(Signal signal)
        {
            List<Signal> variables = new List<Signal>();
            ISignalSet monomials;
            if(signal.IsDrivenByPortEntity("Add", "Std"))
                monomials = signal.DrivenByPort.InputSignals;
            else
                monomials = new SignalSet(signal);
            for(int i = 0; i < monomials.Count; i++)
            {
                ISignalSet factors;
                if(monomials[i].IsDrivenByPortEntity("Multiply", "Std"))
                    factors = monomials[i].DrivenByPort.InputSignals;
                else
                    factors = new SignalSet(monomials[i]);
                for(int j = 0; j < factors.Count; j++)
                {
                    if(IsAlwaysRational(factors[j]))
                        continue;
                    if(factors[j].IsDrivenByPortEntity("Power", "Std"))
                    {
                        Signal b = factors[j].DrivenByPort.InputSignals[0];
                        if(IsAlwaysPositiveInteger(factors[j].DrivenByPort.InputSignals[1]))
                        {
                            if(!variables.Contains(b))
                                variables.Add(b);
                        }
                        else
                        {
                            if(!variables.Contains(signal))
                                variables.Add(signal);
                        }
                    }
                    else
                        if(!variables.Contains(factors[j]))
                            variables.Add(factors[j]);
                }
            }
            return variables.ToArray();
        }

        public static IValueStructure MonomialDegree(Signal signal, Signal[] generalizedVariables)
        {
            if(IsConstantAdditiveIdentity(signal))
                return NegativeInfinitySymbol.Instance;
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return IntegerValue.One;
            if(!Array.Exists<Signal>(generalizedVariables, signal.DependsOn))
                return IntegerValue.Zero;
            ISignalSet factors;
            long deg = 0;
            if(signal.IsDrivenByPortEntity("Multiply", "Std"))
                factors = signal.DrivenByPort.InputSignals;
            else
                factors = new SignalSet(signal);
            for(int i = 0; i < factors.Count; i++)
            {
                if(Array.Exists<Signal>(generalizedVariables, factors[i].Equals))
                    deg++;
                else if(factors[i].IsDrivenByPortEntity("Power", "Std"))
                {
                    Signal b = signal.DrivenByPort.InputSignals[0];
                    Signal e = signal.DrivenByPort.InputSignals[1];
                    IntegerValue v;
                    if(Array.Exists<Signal>(generalizedVariables, b.Equals) && (v = e.Value as IntegerValue) != null && v.Value > 0)
                        deg += v.Value;
                }
            }
            return new IntegerValue(deg);
        }
        public static IValueStructure PolynomialDegree(Signal signal, Signal[] generalizedVariables)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Add", "Std"))
            {
                IntegerValue d = IntegerValue.Zero;
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    IValueStructure f = MonomialDegree(inputs[i], generalizedVariables);
                    if(f is UndefinedSymbol)
                        return f;
                    else if(!(f is NegativeInfinitySymbol))
                        d = d.Max((IntegerValue)f);
                }
                return d;
            }
            else return MonomialDegree(signal, generalizedVariables);
        }
        public static IValueStructure PolynomialTotalDegree(Signal signal)
        {
            Signal[] variables = PolynomialVariables(signal);
            if(variables.Length == 0)
                return UndefinedSymbol.Instance;
            return PolynomialDegree(signal, variables);
        }



        #endregion

        #region Trigonometry
        /// <summary>
        /// Substitutes all instances of Tangent, Cotangent, Secant and Cosecant
        /// by their representation using Sine and Cosine.
        /// </summary>
        public static Signal TrigonometricSubstitute(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return _transformer.Transform(signal, new MathIdentifier("TrigonometricSubstitute", "Std"), false);
        }
        public static SignalSet TrigonometricSubstitute(IEnumerable<Signal> signals)
        {
            return _transformer.Transform(signals, new MathIdentifier("TrigonometricSubstitute", "Std"), false);
        }

        //public static Signal TrigonometricSimplify(IContext context, Signal signal)
        //{
        //    Signal v = TrigonometricSubstitute(context, signal);
        //    Signal w = Rationalize_expression(context, v);
        //    Signal n = Contract_trig(Expand_trig(context, Numerator(w)));
        //    Signal d = Contract_trig(Expand_trig(context, Denominator(w)));
        //    if(d = 0)
        //        return undefined;
        //    else
        //        return n / d;
        //}
        #endregion

        #region Analysis
        public static Signal Derive(Signal signal, Signal variable)
        {
            return _transformer.Transform(signal, DerivativeTransformation.TransformationTypeIdentifier, delegate(ITransformationTheorem theorem)
            {
                DerivativeTransformation dt = (DerivativeTransformation)theorem;
                dt.Variable = variable;
            }, false);
        }
        public static SignalSet Derive(IEnumerable<Signal> signals, Signal variable)
        {
            return _transformer.Transform(signals, DerivativeTransformation.TransformationTypeIdentifier, delegate(ITransformationTheorem theorem)
            {
                DerivativeTransformation dt = (DerivativeTransformation)theorem;
                dt.Variable = variable;
            }, false);
        }
        #endregion

        #region Arithmetics
        /// <summary>
        /// Adds a set of signals and tries to automatically simplify the term.
        /// WARNING: This method may change the contents of the <c>signals</c> paramerer.
        /// Pass <c>signals.AsReadOnly</c> instead of <c>signals</c> if you pass
        /// a writeable signal set that must not be changed.
        /// </summary>
        public static Signal Add(ISignalSet signals)
        {
            if(signals.IsReadOnly)
                signals = new SignalSet(signals);
            AdditionArchitectures.SimplifySummands(signals);
            if(signals.Count == 0)
                return IntegerValue.ConstantAdditiveIdentity;
            if(signals.Count == 1)
                return signals[0];
            return StdBuilder.Add(signals);
        }
        public static Signal Add(params Signal[] signals)
        {
            return Add(new SignalSet(signals));
        }
        public static Signal Add(IEnumerable<Signal> signals)
        {
            return Add(new SignalSet(signals));
        }
        public static Signal Add(Signal signal, IEnumerable<Signal> signals)
        {
            SignalSet set = new SignalSet(signal);
            set.AddRange(signals);
            return Add(set);
        }

        /// <summary>
        /// Substracts a set of signals (from the first signal) and tries to automatically simplify the term.
        /// WARNING: This method may change the contents of the <c>signals</c> paramerer.
        /// Pass <c>signals.AsReadOnly</c> instead of <c>signals</c> if you pass
        /// a writeable signal set that must not be changed.
        /// </summary>
        public static Signal Subtract(ISignalSet signals)
        {
            if(signals.IsReadOnly)
                signals = new SignalSet(signals);
            SubtractionArchitectures.SimplifySummands(signals);
            if(signals.Count == 0)
                return IntegerValue.ConstantAdditiveIdentity;
            if(signals.Count == 1)
                return signals[0];
            return StdBuilder.Subtract(signals);
        }
        public static Signal Subtract(params Signal[] signals)
        {
            return Subtract(new SignalSet(signals));
        }
        public static Signal Subtract(IEnumerable<Signal> signals)
        {
            return Subtract(new SignalSet(signals));
        }
        public static Signal Subtract(Signal signal, IEnumerable<Signal> signals)
        {
            SignalSet set = new SignalSet(signal);
            set.AddRange(signals);
            return Subtract(set);
        }

        /// <summary>
        /// Multiplies a set of signals and tries to automatically simplify the term.
        /// WARNING: This method may change the contents of the <c>signals</c> paramerer.
        /// Pass <c>signals.AsReadOnly</c> instead of <c>signals</c> if you pass
        /// a writeable signal set that must not be changed.
        /// </summary>
        public static Signal Multiply(ISignalSet signals)
        {
            if(signals.IsReadOnly)
                signals = new SignalSet(signals);
            MultiplicationArchitectures.SimplifyFactors(signals);
            if(signals.Count == 0)
                return IntegerValue.ConstantMultiplicativeIdentity;
            if(signals.Count == 1)
                return signals[0];
            return StdBuilder.Multiply(signals);
        }
        public static Signal Multiply(params Signal[] signals)
        {
            return Multiply(new SignalSet(signals));
        }
        public static Signal Multiply(IEnumerable<Signal> signals)
        {
            return Multiply(new SignalSet(signals));
        }
        public static Signal Multiply(Signal signal, IEnumerable<Signal> signals)
        {
            SignalSet set = new SignalSet(signal);
            set.AddRange(signals);
            return Multiply(set);
        }

        /// <summary>
        /// Divides a set of signals (from the first signal) and tries to automatically simplify the term.
        /// WARNING: This method may change the contents of the <c>signals</c> paramerer.
        /// Pass <c>signals.AsReadOnly</c> instead of <c>signals</c> if you pass
        /// a writeable signal set that must not be changed.
        /// </summary>
        public static Signal Divide(ISignalSet signals)
        {
            if(signals.IsReadOnly)
                signals = new SignalSet(signals);
            DivisionArchitectures.SimplifyFactors(signals);
            if(signals.Count == 0)
                return IntegerValue.ConstantMultiplicativeIdentity;
            if(signals.Count == 1)
                return signals[0];
            return StdBuilder.Divide(signals);
        }
        public static Signal Divide(params Signal[] signals)
        {
            return Divide(new SignalSet(signals));
        }
        public static Signal Divide(IEnumerable<Signal> signals)
        {
            return Divide(new SignalSet(signals));
        }
        public static Signal Divide(Signal signal, IEnumerable<Signal> signals)
        {
            SignalSet set = new SignalSet(signal);
            set.AddRange(signals);
            return Divide(set);
        }
        #endregion

        #region Signals
        //public static Signal Constant(IContext context, ValueStructure value)
        //{
        //    Signal s = new Signal(context, value);
        //    s.Label = "Constant_" + value.ToString();
        //    s.AddConstraint(Properties.ConstantSignalProperty.Instance);
        //    return s;
        //}

        public static bool IsConstant(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.AskForProperty("Constant", "Std");
        }

        public static bool IsUndefined(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.Value is MathNet.Symbolics.Packages.Standard.Structures.UndefinedSymbol;
        }
        public static bool IsConstantUndefined(Signal signal)
        {
            return IsConstant(signal) && IsUndefined(signal);
        }

        /// <summary>Evaluates whether the signal is zero (0).</summary>
        public static bool IsAdditiveIdentity(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            IAlgebraicMonoid monoid = signal.Value as IAlgebraicMonoid;
            return monoid != null && monoid.IsAdditiveIdentity;
        }
        /// <summary>Evaluates whether the signal is always zero (0).</summary>
        public static bool IsConstantAdditiveIdentity(Signal signal)
        {
            return IsConstant(signal) && IsAdditiveIdentity(signal);
        }

        /// <summary>Evaluates whether the signal is one (1).</summary>
        public static bool IsMultiplicativeIdentity(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            IAlgebraicRingWithUnity ring = signal.Value as IAlgebraicRingWithUnity;
            return ring != null && ring.IsMultiplicativeIdentity;
        }
        /// <summary>Evaluates whether the signal is always one (1).</summary>
        public static bool IsConstantMultiplicativeIdentity(Signal signal)
        {
            return IsConstant(signal) && IsMultiplicativeIdentity(signal);
        }

        //public static bool IsConstantZero(Signal signal)
        //{
        //    return IsConstant(signal) && (signal.Value.ToString() == "0");
        //}
        //public static bool IsConstantOne(Signal signal)
        //{
        //    return IsConstant(signal) && (signal.Value.ToString() == "1");
        //}

        /// <summary>
        /// Whether the signal is restricted to be an integer > 0, that is one of 1,2,3,...
        /// </summary>
        public static bool IsAlwaysPositiveInteger(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.AskForProperty("PositiveIntegerSet", "Std")
                || IsConstant(signal) && IntegerValue.CanConvertLosslessFrom(signal.Value) && IntegerValue.ConvertFrom(signal.Value).Value > 0;
        }
        /// <summary>
        /// Whether the signal is restricted to be an integer >= 0, that is one of 0,1,2,3,...
        /// </summary>
        public static bool IsAlwaysNonnegativeInteger(Signal signal)
        {
            return IsAlwaysPositiveInteger(signal) || IsConstantAdditiveIdentity(signal);
        }
        /// <summary>
        /// Whether the signal is restricted to be an integer, that is one of ...,-3,-2,-1,0,1,2,3,...
        /// </summary>
        public static bool IsAlwaysInteger(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.AskForProperty("IntegerSet", "Std")
                || IsConstant(signal) && IntegerValue.CanConvertLosslessFrom(signal.Value)
                || IsAlwaysNonnegativeInteger(signal);
        }
        public static bool IsAlwaysRational(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.AskForProperty("RationalSet", "Std")
                || IsConstant(signal) && RationalValue.CanConvertLosslessFrom(signal.Value)
                || IsAlwaysInteger(signal);
        }
        public static bool IsAlwaysReal(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.AskForProperty("RealSet", "Std")
                || IsConstant(signal) && RealValue.CanConvertLosslessFrom(signal.Value)
                || IsAlwaysRational(signal);
        }
        #endregion
    }
}
