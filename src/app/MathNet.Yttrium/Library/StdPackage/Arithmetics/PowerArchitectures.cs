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
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.StdPackage.Arithmetics
{
    public class PowerArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Power", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public PowerArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Integer"),
                IntegerValueCategory.IsIntegerValueMember,
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<IntegerValue>(delegate() {return IntegerValue.One;}, IntegerValue.ConvertFrom, delegate(IntegerValue acc, IntegerValue item) { return acc.Power(item); }, port.InputSignalCount) }; });

            AddArchitecture(new MathIdentifier("RationalPowerIntegerRadix", "Std"),
                delegate(Port port) { return RationalValue.Converter.CanConvertLosslessFrom(port.InputSignals[0].Value) && IntegerValue.Converter.CanConvertLosslessFrom(port.InputSignals[1].Value); },
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<RationalValue>(delegate() { return IntegerValue.One; }, RationalValue.ConvertFrom, delegate(RationalValue acc, RationalValue item) { return acc.Power(item.Numerator); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<RealValue>(delegate() { return IntegerValue.One; }, RealValue.ConvertFrom, delegate(RealValue acc, RealValue item) { return acc.Power(item); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.One; }, ComplexValue.ConvertFrom, ComplexValue.Power, port.InputSignalCount) }; });
        }

        //public static Signal[] SimplifyOperands(List<Signal> signals)
        //{
        //    if(signals.Count>0 && Std.IsConstantOne(signals[0]))
        //        return new Signal[] { signals[0] };
        //    for(int i = signals.Count - 1; i > 0; i--) //don't touch first item
        //    {
        //        if(Std.IsConstantZero(signals[i]))
        //            return new Signal[] { IntegerValue.ConstantOne(signals[i].Context) };
        //        if(Std.IsConstantOne(signals[i]))
        //            signals.RemoveAt(i);
        //    }
        //    return signals.ToArray();
        //}

        public static ITheorem[] BuildTheorems(Context context)
        {
            ITheorem[] theorems = new ITheorem[1];

            //theorems[0] = new Analysis.DerivativeTransformation("PowerDerivative", "Std", context.Library.LookupEntity("Power", "Std"),
            //    delegate(Port port, Signal[] derivedInputSignals)
            //    {
            //        Signal innerA = derivedInputSignals[1] * context.Builder.NaturalLogarithm(port.InputSignals[0]);
            //        Signal innerB = (port.InputSignals[1] * derivedInputSignals[0]) / port.InputSignals[0];
            //        return port.OutputSignals[0] * (innerA + innerB);
            //    });

            theorems[0] = new Algebra.AutoSimplifyTransformation(context.Library.LookupEntity(_entityId),
                delegate(Port port)
                {
                    // TODO
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    if(Std.IsConstantMultiplicativeIdentity(manipulatedInputs[0]))
                        return new SignalSet(manipulatedInputs[0]);
                    if(Std.IsConstantMultiplicativeIdentity(manipulatedInputs[1]))
                        return new SignalSet(manipulatedInputs[0]);
                    if(Std.IsConstantAdditiveIdentity(manipulatedInputs[1]))
                        return new SignalSet(IntegerValue.ConstantOne(port.Context));
                    if(hasManipulatedInputs)
                        return port.Entity.InstantiatePort(port.Context, manipulatedInputs).OutputSignals;
                    else
                        return port.OutputSignals;
                });

            return theorems;
        }
    }
}
