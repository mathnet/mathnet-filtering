#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Workplace
{
    public class Builder : IContextSensitive
    {
        private readonly Context context;

        internal Builder(Context context)
        {
            this.context = context;
        }

        public Context Context
        {
            get { return context; }
        }

        #region Building Single-Value Functions
        public Signal Function(Entity entity, Signal argument)
        {
            Port port = entity.InstantiatePort(context, argument);

            if(port.InputSignalCount != 1 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("1 input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");

            return port[0]; //.AutomaticSimplify();
        }
        public Signal Function(Entity entity, Signal argument1, Signal argument2)
        {
            Port port = entity.InstantiatePort(context, argument1, argument2);

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("2 input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");

            return port[0]; //.AutomaticSimplify();
        }
        public Signal Function(Entity entity, params Signal[] arguments)
        {
            Port port = entity.InstantiatePort(context, arguments);

            if(port.InputSignalCount != arguments.Length || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException(arguments.Length.ToString(Context.NumberFormat) + " input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");

            return port[0]; //.AutomaticSimplify();
        }
        public Signal Function(Entity entity, IList<Signal> arguments)
        {
            Port port = entity.InstantiatePort(context, arguments);

            if(port.InputSignalCount != arguments.Count || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException(arguments.Count.ToString(Context.NumberFormat) + " input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");

            return port[0]; //.AutomaticSimplify();
        }

        public Signal Function(string symbol, Signal argument)
        {
            return Function(context.Library.LookupEntity(symbol, 1, 1, 0), argument);
        }
        public Signal Function(string symbol, Signal argument1, Signal argument2)
        {
            return Function(context.Library.LookupEntity(symbol, 2, 1, 0), argument1, argument2);
        }
        public Signal Function(string symbol, params Signal[] arguments)
        {
            return Function(context.Library.LookupEntity(symbol, arguments.Length, 1, 0), arguments);
        }
        public Signal Function(string symbol, IList<Signal> arguments)
        {
            return Function(context.Library.LookupEntity(symbol, arguments.Count, 1, 0), arguments);
        }

        public Signal Function(string symbol, InfixNotation notation, Signal argument)
        {
            return Function(context.Library.LookupEntity(symbol, notation, 1, 1, 0), argument);
        }
        public Signal Function(string symbol, InfixNotation notation, Signal argument1, Signal argument2)
        {
            return Function(context.Library.LookupEntity(symbol, notation, 2, 1, 0), argument1, argument2);
        }
        public Signal Function(string symbol, InfixNotation notation, params Signal[] arguments)
        {
            return Function(context.Library.LookupEntity(symbol, notation, arguments.Length, 1, 0), arguments);
        }
        public Signal Function(string symbol, InfixNotation notation, IList<Signal> arguments)
        {
            return Function(context.Library.LookupEntity(symbol, notation, arguments.Count, 1, 0), arguments);
        }

        public Signal Function(MathIdentifier entityId, Signal argument)
        {
            return Function(context.Library.LookupEntity(entityId), argument);
        }
        public Signal Function(MathIdentifier entityId, Signal argument1, Signal argument2)
        {
            return Function(context.Library.LookupEntity(entityId), argument1, argument2);
        }
        public Signal Function(MathIdentifier entityId, params Signal[] arguments)
        {
            return Function(context.Library.LookupEntity(entityId), arguments);
        }
        public Signal Function(MathIdentifier entityId, IList<Signal> arguments)
        {
            return Function(context.Library.LookupEntity(entityId), arguments);
        }
        #endregion

        #region Building Multiple-Value Functions
        public ReadOnlySignalSet Functions(Entity entity, Signal argument)
        {
            Port port = entity.InstantiatePort(context, argument);

            if(port.InputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("1 input", port.InputSignalCount.ToString(Context.NumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(Entity entity, Signal argument1, Signal argument2)
        {
            Port port = entity.InstantiatePort(context, argument1, argument2);

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("2 input", port.InputSignalCount.ToString(Context.NumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(Entity entity, params Signal[] arguments)
        {
            Port port = entity.InstantiatePort(context, arguments);

            if(port.InputSignalCount != arguments.Length || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException(arguments.Length.ToString(Context.NumberFormat) + " input", port.InputSignalCount.ToString(Context.NumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(Entity entity, IList<Signal> arguments)
        {
            Port port = entity.InstantiatePort(context, arguments);

            if(port.InputSignalCount != arguments.Count || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException(arguments.Count.ToString(Context.NumberFormat) + " input", port.InputSignalCount.ToString(Context.NumberFormat) + " input");

            return port.OutputSignals;
        }

        public ReadOnlySignalSet Functions(string symbol, Signal argument)
        {
            return Functions(context.Library.LookupEntity(symbol, 1, 1, 0), argument);
        }
        public ReadOnlySignalSet Functions(string symbol, Signal argument1, Signal argument2)
        {
            return Functions(context.Library.LookupEntity(symbol, 2, 1, 0), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(string symbol, params Signal[] arguments)
        {
            return Functions(context.Library.LookupEntity(symbol, arguments.Length, 1, 0), arguments);
        }
        public ReadOnlySignalSet Functions(string symbol, IList<Signal> arguments)
        {
            return Functions(context.Library.LookupEntity(symbol, arguments.Count, 1, 0), arguments);
        }

        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument)
        {
            return Functions(context.Library.LookupEntity(symbol, notation, 1, 1, 0), argument);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument1, Signal argument2)
        {
            return Functions(context.Library.LookupEntity(symbol, notation, 2, 1, 0), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, params Signal[] arguments)
        {
            return Functions(context.Library.LookupEntity(symbol, notation, arguments.Length, 1, 0), arguments);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, IList<Signal> arguments)
        {
            return Functions(context.Library.LookupEntity(symbol, notation, arguments.Count, 1, 0), arguments);
        }

        public ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument)
        {
            return Functions(context.Library.LookupEntity(entityId), argument);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument1, Signal argument2)
        {
            return Functions(context.Library.LookupEntity(entityId), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, params Signal[] arguments)
        {
            return Functions(context.Library.LookupEntity(entityId), arguments);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, IList<Signal> arguments)
        {
            return Functions(context.Library.LookupEntity(entityId), arguments);
        }
        #endregion

        #region Basic Operation Templates (part of Std Package) eg for operator overloadings
        public Signal Add(Signal summand1, Signal summand2)
        {
            return Function("+", InfixNotation.LeftAssociativeInnerOperator, summand1, summand2);
        }
        public Signal Add(params Signal[] signals)
        {
            return Function("+", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal Add(IList<Signal> signals)
        {
            return Function("+", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal AddSimplified(params Signal[] signals)
        {
            return AddSimplified(new SignalSet(signals));
        }
        public Signal AddSimplified(IEnumerable<Signal> signals)
        {
            return AddSimplified(new SignalSet(signals));
        }
        public Signal AddSimplified(Signal signal, Signal[] signals)
        {
            SignalSet list = new SignalSet(); //signals.Length+1);
            list.Add(signal);
            list.AddRange(signals);
            return AddSimplified(list);
        }
        public Signal AddSimplified(ReadOnlySignalSet signals)
        {
            return AddSimplified(new SignalSet(signals));
        }
        /// <remarks>The signal set may be changed in this call! Use the read only version if this is not applicable.</remarks>
        public Signal AddSimplified(SignalSet signals)
        {
            StdPackage.Arithmetics.AdditionArchitectures.SimplifySummands(signals);
            if(signals.Count == 0)
                return StdPackage.Structures.IntegerValue.ConstantAdditiveIdentity(context);
            if(signals.Count == 1)
                return signals[0];
            return Function(StdPackage.Arithmetics.AdditionArchitectures.EntityIdentifier, signals);
        }

        public Signal Negate(Signal subtrahend)
        {
            return Function("-", InfixNotation.PreOperator, subtrahend);
        }
        public ReadOnlySignalSet Negate(params Signal[] signals)
        {
            return Functions("-", InfixNotation.PreOperator, signals);
        }
        public ReadOnlySignalSet Negate(IList<Signal> signals)
        {
            return Functions("-", InfixNotation.PreOperator, signals);
        }
        public Signal Subtract(Signal minuend, Signal subtrahend)
        {
            return Function("-", InfixNotation.LeftAssociativeInnerOperator, minuend, subtrahend);
        }
        public Signal Subtract(params Signal[] signals)
        {
            return Function("-", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal Subtract(IList<Signal> signals)
        {
            return Function("-", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal SubtractSimplified(params Signal[] signals)
        {
            return SubtractSimplified(new SignalSet(signals));
        }
        public Signal SubtractSimplified(IEnumerable<Signal> signals)
        {
            return SubtractSimplified(new SignalSet(signals));
        }
        public Signal SubtractSimplified(Signal signal, Signal[] signals)
        {
            SignalSet list = new SignalSet(); //signals.Length + 1);
            list.Add(signal);
            list.AddRange(signals);
            return SubtractSimplified(list);
        }
        public Signal SubtractSimplified(ReadOnlySignalSet signals)
        {
            return SubtractSimplified(new SignalSet(signals));
        }
        /// <remarks>The signal set may be changed in this call! Use the read only version if this is not applicable.</remarks>
        public Signal SubtractSimplified(SignalSet signals)
        {
            StdPackage.Arithmetics.SubtractionArchitectures.SimplifySummands(signals);
            if(signals.Count == 0)
                return StdPackage.Structures.IntegerValue.ConstantAdditiveIdentity(context);
            if(signals.Count == 1)
                return signals[0];
            return Function(StdPackage.Arithmetics.SubtractionArchitectures.EntityIdentifier, signals);
        }

        public Signal Multiply(Signal multiplicand, Signal multiplier)
        {
            return Function("*", InfixNotation.LeftAssociativeInnerOperator, multiplicand, multiplier);
        }
        public Signal Multiply(params Signal[] signals)
        {
            return Function("*", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal Multiply(IList<Signal> signals)
        {
            return Function("*", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal MultiplySimplified(params Signal[] signals)
        {
            return MultiplySimplified(new SignalSet(signals));
        }
        public Signal MultiplySimplified(IEnumerable<Signal> signals)
        {
            return MultiplySimplified(new SignalSet(signals));
        }
        public Signal MultiplySimplified(Signal signal, Signal[] signals)
        {
            SignalSet list = new SignalSet(); //signals.Length + 1);
            list.Add(signal);
            list.AddRange(signals);
            return MultiplySimplified(list);
        }
        public Signal MultiplySimplified(ReadOnlySignalSet signals)
        {
            return MultiplySimplified(new SignalSet(signals));
        }
        /// <remarks>The signal set may be changed in this call! Use the read only version if this is not applicable.</remarks>
        public Signal MultiplySimplified(SignalSet signals)
        {
            StdPackage.Arithmetics.MultiplicationArchitectures.SimplifyFactors(signals);
            if(signals.Count == 0)
                return StdPackage.Structures.IntegerValue.ConstantMultiplicativeIdentity(context);
            if(signals.Count == 1)
                return signals[0];
            return Function(StdPackage.Arithmetics.MultiplicationArchitectures.EntityIdentifier, signals);
        }

        public Signal Invert(Signal divisor)
        {
            return Function("inv", divisor);
        }
        public ReadOnlySignalSet Invert(params Signal[] signals)
        {
            return Functions("inv", signals);
        }
        public ReadOnlySignalSet Invert(IList<Signal> signals)
        {
            return Functions("inv", signals);
        }
        public Signal Divide(Signal dividend, Signal divisor)
        {
            return Function("/", InfixNotation.LeftAssociativeInnerOperator, dividend, divisor);
        }
        public Signal Divide(params Signal[] signals)
        {
            return Function("/", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal Divide(IList<Signal> signals)
        {
            return Function("/", InfixNotation.LeftAssociativeInnerOperator, signals);
        }
        public Signal DivideSimplified(params Signal[] signals)
        {
            return DivideSimplified(new SignalSet(signals));
        }
        public Signal DivideSimplified(IEnumerable<Signal> signals)
        {
            return DivideSimplified(new SignalSet(signals));
        }
        public Signal DivideSimplified(Signal signal, Signal[] signals)
        {
            SignalSet list = new SignalSet(); //signals.Length + 1);
            list.Add(signal);
            list.AddRange(signals);
            return DivideSimplified(list);
        }
        public Signal DivideSimplified(ReadOnlySignalSet signals)
        {
            return DivideSimplified(new SignalSet(signals));
        }
        /// <remarks>The signal set may be changed in this call! Use the read only version if this is not applicable.</remarks>
        public Signal DivideSimplified(SignalSet signals)
        {
            StdPackage.Arithmetics.DivisionArchitectures.SimplifyFactors(signals);
            if(signals.Count == 0)
                return StdPackage.Structures.IntegerValue.ConstantMultiplicativeIdentity(context);
            if(signals.Count == 1)
                return signals[0];
            return Function(StdPackage.Arithmetics.DivisionArchitectures.EntityIdentifier, signals);
        }

        public Signal Exponential(Signal exponent)
        {
            return Function("exp", exponent);
        }
        public Signal NaturalLogarithm(Signal exponent)
        {
            return Function("ln", exponent);
        }

        public Signal Power(Signal radix, Signal exponent)
        {
            return Function("^", InfixNotation.RightAssociativeInnerOperator, radix, exponent);
        }
        public Signal Power(params Signal[] signals)
        {
            return Function("^", InfixNotation.RightAssociativeInnerOperator, signals);
        }
        public Signal Power(IList<Signal> signals)
        {
            return Function("^", InfixNotation.RightAssociativeInnerOperator, signals);
        }

        public Signal Square(Signal radix)
        {
            return Function("sqr", radix);
        }
        public ReadOnlySignalSet Square(params Signal[] signals)
        {
            return Functions("sqr", signals);
        }
        public ReadOnlySignalSet Square(IList<Signal> signals)
        {
            return Functions("sqr", signals);
        }

        public Signal Absolute(Signal signal)
        {
            return Function("abs", signal);
        }
        public ReadOnlySignalSet Absolute(params Signal[] signals)
        {
            return Functions("abs", signals);
        }
        public ReadOnlySignalSet Absolute(IList<Signal> signals)
        {
            return Functions("abs", signals);
        }

        public Signal Factorial(Signal signal)
        {
            return Function("!", InfixNotation.PostOperator, signal);
        }

        public Signal Derive(Signal signal, Signal variable)
        {
            return Function("diff", signal, variable);
        }
        public Signal Derive(params Signal[] signals)
        {
            return Function("diff", signals);
        }
        public Signal Derive(IList<Signal> signals)
        {
            return Function("diff", signals);
        }
        #endregion

        #region Basic Encapsulation Multiplexing
        public Signal EncapsulateAsVector(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsVector(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsList(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsList(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsSet(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsSet(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsScalar(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsScalar(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        #endregion

        #region Basic Signal Manipulation
        public Port MapSignals(Signal source, Signal target)
        {
            Port port = context.Library.LookupEntity(new MathIdentifier("Transport", "Std")).InstantiatePort(context, new Signal[] { source }, new Signal[] { target });

            if(port.InputSignalCount != 1 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("1 input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");
            return port;
        }
        public Port MapSignalsSynchronized(Signal source, Signal target, Signal clock)
        {
            Port port = context.Library.LookupEntity(new MathIdentifier("Sync", "Std")).InstantiatePort(context, new Signal[] { source, clock }, new Signal[] { target });

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalCountUnexpectedException("2 input and 1 output", port.InputSignalCount.ToString(Context.NumberFormat) + " input and " + port.OutputSignalCount.ToString(Context.NumberFormat) + " output");

            return port;
        }
        public Signal Synchronize(Signal signal, Signal clock)
        {
            return Function(new MathIdentifier("Sync", "Std"), signal, clock);
        }
        #endregion
    }
}
