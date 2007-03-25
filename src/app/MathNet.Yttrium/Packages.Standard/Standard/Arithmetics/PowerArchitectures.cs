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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
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
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<IntegerValue>(delegate() { return IntegerValue.One; }, IntegerValue.ConvertFrom, delegate(IntegerValue acc, IntegerValue item) { return acc.Power(item); }, port.InputSignalCount) }; });

            AddArchitecture(new MathIdentifier("RationalPowerIntegerRadix", "Std"),
                delegate(Port port) { return RationalValue.CanConvertLosslessFrom(port.InputSignals[0].Value) && IntegerValue.CanConvertLosslessFrom(port.InputSignals[1].Value); },
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<RationalValue>(delegate() { return IntegerValue.One; }, RationalValue.ConvertFrom, delegate(RationalValue acc, RationalValue item) { return acc.Power(item.Numerator); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<RealValue>(delegate() { return IntegerValue.One; }, RealValue.ConvertFrom, delegate(RealValue acc, RealValue item) { return acc.Power(item); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.One; }, ComplexValue.ConvertFrom, ComplexValue.Power, port.InputSignalCount) }; });
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

        public static void RegisterTheorems(ILibrary library)
        {
            //Analysis.DerivativeTransformation.Provider.Add(
            //    new Analysis.DerivativeTransformation(_entityId,
            //    delegate(Port port, Signal[] derivedInputSignals)
            //    {
            //        Signal innerA = derivedInputSignals[1] * StdBuilder.NaturalLogarithm(port.InputSignals[0]);
            //        Signal innerB = (port.InputSignals[1] * derivedInputSignals[0]) / port.InputSignals[0];
            //        return port.OutputSignals[0] * (innerA + innerB);
            //    }));

            Algebra.AutoSimplifyTransformation.Provider.Add(
                new Algebra.AutoSimplifyTransformation(_entityId,
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
                        return new SignalSet(IntegerValue.ConstantOne);
                    if(hasManipulatedInputs)
                        return port.Entity.InstantiatePort(manipulatedInputs).OutputSignals;
                    else
                        return port.OutputSignals;
                }));
        }
    }
}
