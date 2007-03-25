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

using MathNet.Symbolics.Backend.Containers;

using MathNet.Symbolics.Packages.Standard.Arithmetics;
using MathNet.Symbolics.Packages.Standard.Trigonometry;
using MathNet.Symbolics.Packages.Standard.Analysis;
using MathNet.Symbolics.Packages.Standard.Algebra;

namespace MathNet.Symbolics.Packages.Standard
{
    /// <summary>
    /// Standard Package Builder, Maps available operations to the given signals.
    /// </summary>
    /// <remarks>
    /// A Package Builder only maps operations on signals, but does not "execute" them.
    /// For example, if you have two signals a=<c>x</c> and b=<c>sin(x)</c> and apply
    /// them to c=<c>StdBuilder.Derive(b,a)</c>, you'll get a new signal c=<c>diff(sin(x),x)</c>
    /// and not <c>cos(x)</c> as you may expect. Use <c>Std</c> instead of <c>StdBuilder</c>
    /// if you want to actually transform the signals.
    /// </remarks>
    public static class StdBuilder
    {
        private static IBuilder _builder = Service<IBuilder>.Instance;

        #region Arithmetics
        public static Signal Add(Signal summand1, Signal summand2)
        {
            return _builder.Function(AdditionArchitectures.EntityIdentifier, summand1, summand2);
        }
        public static Signal Add(params Signal[] op)
        {
            return _builder.Function(AdditionArchitectures.EntityIdentifier, op);
        }
        public static Signal Add(IList<Signal> op)
        {
            return _builder.Function(AdditionArchitectures.EntityIdentifier, op);
        }

        public static Signal Negate(Signal subtrahend)
        {
            return _builder.Function(NegateArchitectures.EntityIdentifier, subtrahend);
        }
        public static ReadOnlySignalSet Negate(params Signal[] op)
        {
            return _builder.Functions(NegateArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Negate(IList<Signal> op)
        {
            return _builder.Functions(NegateArchitectures.EntityIdentifier, op);
        }

        public static Signal Subtract(Signal minuend, Signal subtrahend)
        {
            return _builder.Function(SubtractionArchitectures.EntityIdentifier, minuend, subtrahend);
        }
        public static Signal Subtract(params Signal[] op)
        {
            return _builder.Function(SubtractionArchitectures.EntityIdentifier, op);
        }
        public static Signal Subtract(IList<Signal> op)
        {
            return _builder.Function(SubtractionArchitectures.EntityIdentifier, op);
        }

        public static Signal Multiply(Signal multiplicand, Signal multiplier)
        {
            return _builder.Function(MultiplicationArchitectures.EntityIdentifier, multiplicand, multiplier);
        }
        public static Signal Multiply(params Signal[] op)
        {
            return _builder.Function(MultiplicationArchitectures.EntityIdentifier, op);
        }
        public static Signal Multiply(IList<Signal> op)
        {
            return _builder.Function(MultiplicationArchitectures.EntityIdentifier, op);
        }

        public static Signal Invert(Signal divisor)
        {
            return _builder.Function(InvertArchitectures.EntityIdentifier, divisor);
        }
        public static ReadOnlySignalSet Invert(params Signal[] op)
        {
            return _builder.Functions(InvertArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Invert(IList<Signal> op)
        {
            return _builder.Functions(InvertArchitectures.EntityIdentifier, op);
        }

        public static Signal Divide(Signal dividend, Signal divisor)
        {
            return _builder.Function(DivisionArchitectures.EntityIdentifier, dividend, divisor);
        }
        public static Signal Divide(params Signal[] op)
        {
            return _builder.Function(DivisionArchitectures.EntityIdentifier, op);
        }
        public static Signal Divide(IList<Signal> op)
        {
            return _builder.Function(DivisionArchitectures.EntityIdentifier, op);
        }

        public static Signal Exponential(Signal exponent)
        {
            return _builder.Function(ExponentialArchitectures.EntityIdentifier, exponent);
        }
        public static Signal NaturalLogarithm(Signal exponent)
        {
            return _builder.Function(NaturalLogarithmArchitectures.EntityIdentifier, exponent);
        }

        public static Signal Power(Signal radix, Signal exponent)
        {
            return _builder.Function(PowerArchitectures.EntityIdentifier, radix, exponent);
        }
        public static Signal Power(params Signal[] op)
        {
            return _builder.Function(PowerArchitectures.EntityIdentifier, op);
        }
        public static Signal Power(IList<Signal> op)
        {
            return _builder.Function(PowerArchitectures.EntityIdentifier, op);
        }

        public static Signal Square(Signal radix)
        {
            return _builder.Function(SquareArchitectures.EntityIdentifier, radix);
        }
        public static ReadOnlySignalSet Square(params Signal[] op)
        {
            return _builder.Functions(SquareArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Square(IList<Signal> op)
        {
            return _builder.Functions(SquareArchitectures.EntityIdentifier, op);
        }

        public static Signal Absolute(Signal signal)
        {
            return _builder.Function(AbsoluteArchitectures.EntityIdentifier, signal);
        }
        public static ReadOnlySignalSet Absolute(params Signal[] op)
        {
            return _builder.Functions(AbsoluteArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Absolute(IList<Signal> op)
        {
            return _builder.Functions(AbsoluteArchitectures.EntityIdentifier, op);
        }

        public static Signal Factorial(Signal signal)
        {
            return _builder.Function("!", InfixNotation.PostOperator, signal);
        }

        #endregion

        #region Trigonometry
        public static Signal Sine(Signal op)
        {
            return _builder.Function(SineArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Sine(IList<Signal> op)
        {
            return _builder.Functions(SineArchitectures.EntityIdentifier, op);
        }
        public static Signal Cosine(Signal op)
        {
            return _builder.Function(CosineArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Cosine(IList<Signal> op)
        {
            return _builder.Functions(CosineArchitectures.EntityIdentifier, op);
        }
        public static Signal Tangent(Signal op)
        {
            return _builder.Function(TangentArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Tangent(IList<Signal> op)
        {
            return _builder.Functions(TangentArchitectures.EntityIdentifier, op);
        }
        public static Signal Cotangent(Signal op)
        {
            return _builder.Function(CotangentArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Cotangent(IList<Signal> op)
        {
            return _builder.Functions(CotangentArchitectures.EntityIdentifier, op);
        }
        public static Signal Secant(Signal op)
        {
            return _builder.Function(SecantArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Secant(IList<Signal> op)
        {
            return _builder.Functions(SecantArchitectures.EntityIdentifier, op);
        }
        public static Signal Cosecant(Signal op)
        {
            return _builder.Function(CosecantArchitectures.EntityIdentifier, op);
        }
        public static ReadOnlySignalSet Cosecant(IList<Signal> op)
        {
            return _builder.Functions(CosecantArchitectures.EntityIdentifier, op);
        }
        #endregion

        #region Analysis
        public static Signal Derive(Signal signal, Signal variable)
        {
            return _builder.Function(AlgebraicDerivativeArchitecture.EntityIdentifier, signal, variable);
        }
        public static Signal Derive(params Signal[] signals)
        {
            return _builder.Function(AlgebraicDerivativeArchitecture.EntityIdentifier, signals);
        }
        public static Signal Derive(IList<Signal> signals)
        {
            return _builder.Function(AlgebraicDerivativeArchitecture.EntityIdentifier, signals);
        }
        #endregion

        #region Algebra
        #endregion
    }
}
