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

        #region Signal Properties
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

            return signal.AskForFlag(StdAspect.ConstantFlag) == FlagState.Enabled;
            //return signal.AskForProperty("Constant", "Std");
        }

        public static bool IsUndefined(ValueNode signal)
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
        public static bool IsAdditiveIdentity(ValueNode signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            IAlgebraicAdditiveIdentityElement monoid = signal.Value as IAlgebraicAdditiveIdentityElement;
            return monoid != null && monoid.IsAdditiveIdentity;
        }
        /// <summary>Evaluates whether the signal is always zero (0).</summary>
        public static bool IsConstantAdditiveIdentity(Signal signal)
        {
            return IsConstant(signal) && IsAdditiveIdentity(signal);
        }

        /// <summary>Evaluates whether the signal is one (1).</summary>
        public static bool IsMultiplicativeIdentity(ValueNode signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            IAlgebraicMultiplicativeIdentityElement ring = signal.Value as IAlgebraicMultiplicativeIdentityElement;
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
        /// Whether the signal is restricted to be > 0
        /// </summary>
        public static bool IsAlwaysPositive(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.IsFlagEnabled(StdAspect.PositiveWithoutZeroConstraingFlag)
                || IsConstant(signal) && RealValue.CanConvertLosslessFrom(signal.Value) && RealValue.ConvertFrom(signal.Value).Value > 0d;
        }
        public static void ConstrainAlwaysPositive(Signal signal)
        {
            signal.EnableFlag(StdAspect.PositiveWithoutZeroConstraingFlag);
        }

        /// <summary>
        /// Whether the signal is restricted to be >= 0
        /// </summary>
        public static bool IsAlwaysNonnegative(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.IsFlagEnabled(StdAspect.PositiveOrZeroConstraintFlag)
                || IsConstant(signal) && RealValue.CanConvertLosslessFrom(signal.Value) && RealValue.ConvertFrom(signal.Value).Value >= 0d;
        }
        public static void ConstrainAlwaysNonnegative(Signal signal)
        {
            signal.EnableFlag(StdAspect.PositiveOrZeroConstraintFlag);
        }

        /// <summary>
        /// Whether the signal is restricted to be an integer, that is one of ...,-3,-2,-1,0,1,2,3,...
        /// </summary>
        public static bool IsAlwaysInteger(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.IsFlagEnabled(StdAspect.IntegerConstraintFlag)
                || IsConstant(signal) && IntegerValue.CanConvertLosslessFrom(signal.Value);
        }
        public static void ConstrainAlwaysInteger(Signal signal)
        {
            signal.EnableFlag(StdAspect.IntegerConstraintFlag);
        }

        /// <summary>
        /// Whether the signal is restricted to be a rational.
        /// </summary>
        public static bool IsAlwaysRational(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.IsFlagEnabled(StdAspect.RationalConstraintFlag)
                || IsConstant(signal) && RationalValue.CanConvertLosslessFrom(signal.Value);
        }
        public static void ConstrainAlwaysRational(Signal signal)
        {
            signal.EnableFlag(StdAspect.RationalConstraintFlag);
        }

        /// <summary>
        /// Whether the signal is restricted to be a real.
        /// </summary>
        public static bool IsAlwaysReal(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            return signal.IsFlagEnabled(StdAspect.RealConstraintFlag)
                || IsConstant(signal) && RealValue.CanConvertLosslessFrom(signal.Value);
        }
        public static void ConstrainAlwaysReal(Signal signal)
        {
            signal.EnableFlag(StdAspect.RealConstraintFlag);
        }

        /// <summary>
        /// Whether the signal is restricted to be an integer > 0, that is one of 1,2,3,...
        /// </summary>
        public static bool IsAlwaysPositiveInteger(Signal signal)
        {
            return IsAlwaysInteger(signal) && IsAlwaysPositive(signal);
        }
        public static void ConstrainAlwaysPositiveInteger(Signal signal)
        {
            ConstrainAlwaysInteger(signal);
            ConstrainAlwaysPositive(signal);
        }

        /// <summary>
        /// Whether the signal is restricted to be an integer >= 0, that is one of 0,1,2,3,...
        /// </summary>
        public static bool IsAlwaysNonnegativeInteger(Signal signal)
        {
            return IsAlwaysInteger(signal) && IsAlwaysNonnegative(signal);
        }
        public static void ConstrainAlwaysNonnegativeInteger(Signal signal)
        {
            ConstrainAlwaysInteger(signal);
            ConstrainAlwaysNonnegative(signal);
        }
        #endregion
    }
}
