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
    public class MultiplicationArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Multiply", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public MultiplicationArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Integer"),
                IntegerValueCategory.IsIntegerValueMember,
                delegate(Port port) { return new ProcessBase[] { new IntegerValue.MultiplyProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Rational"),
                RationalValueCategory.IsRationalValueMember,
                delegate(Port port) { return new ProcessBase[] { new RationalValue.MultiplyProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new ProcessBase[] { new RealValue.MultiplyProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.One; }, ComplexValue.ConvertFrom, ComplexValue.Multiply, port.InputSignalCount) }; });
        }

        public static bool SimplifyFactors(ISignalSet signals)
        {
            Signal zero;
            if(signals.Exists(Std.IsConstantAdditiveIdentity, out zero))
            {
                signals.Clear();
                signals.Add(zero);
                return true;
            }
            return 0 < signals.RemoveAll(Std.IsConstantMultiplicativeIdentity);
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    int cnt = manipulatedInputs.Count;
                    Signal[] addSignals = new Signal[cnt];
                    Signal[] multiplySignals = new Signal[cnt];
                    for(int i = 0; i < cnt; i++)
                    {
                        for(int j = 0; j < cnt; j++)
                            multiplySignals[j] = i == j ? manipulatedInputs[j] : port.InputSignals[j];
                        addSignals[i] = Std.Multiply(multiplySignals);
                    }
                    return new SignalSet(Std.Add(addSignals));
                }));

            Algebra.AutoSimplifyTransformation.Provider.Add(
                new Algebra.AutoSimplifyTransformation(_entityId,
                delegate(Port port)
                {
                    // TODO
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    if(SimplifyFactors(manipulatedInputs) || hasManipulatedInputs)
                    {
                        if(manipulatedInputs.Count == 0)
                            return new SignalSet(IntegerValue.ConstantMultiplicativeIdentity);
                        if(manipulatedInputs.Count == 1)
                            return manipulatedInputs;
                        return new SignalSet(Std.Multiply(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                }));
        }
    }
}
