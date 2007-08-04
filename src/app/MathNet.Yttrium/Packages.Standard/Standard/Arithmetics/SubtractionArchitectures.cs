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
using MathNet.Symbolics.Conversion;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
{
    public class SubtractionArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Subtract", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public SubtractionArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Integer"),
                IntegerValueCategory.IsIntegerValueMember,
                delegate(Port port) { return new ProcessBase[] { new IntegerValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Rational"),
                RationalValueCategory.IsRationalValueMember,
                delegate(Port port) { return new ProcessBase[] { new RationalValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new ProcessBase[] { new RealValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.Zero; }, ComplexValue.ConvertFrom, ComplexValue.Subtract, port.InputSignalCount) }; });
        }

        public static bool SimplifySummandsForceAddition(ISignalSet signals)
        {
            if(signals.Count < 2)
                return false;

            for(int i = 1; i < signals.Count; i++)
                signals[i] = Std.Negate(signals[i]);
            Signal sum = Std.Add(signals);

            signals.Clear();
            signals.Add(sum);
            return true;
        }

        public static bool SimplifySummands(ISignalSet signals)
        {
            if(signals.Count < 2)
                return false;
            bool changed = false;
            IAccumulator acc = null; 
            for(int i = signals.Count - 1; i > 0; i--)  //don't touch first item!
            {
                Signal s = signals[i];
                if(Std.IsConstantComplex(s))
                {
                    if(acc == null)
                        acc = Accumulator<IntegerValue, RationalValue>.Create(IntegerValue.AdditiveIdentity);
                    signals.RemoveAt(i);
                    changed = true;
                    acc = acc.Add(s.Value);
                }
            }
            if(acc != null && !acc.Value.Equals(IntegerValue.AdditiveIdentity))
            {
                Signal first = signals[0];
                if(Std.IsConstantComplex(first))
                {
                    acc = acc.Subtract(first.Value).Negate();
                    signals[0] = Std.DefineConstant(acc.Value);
                    changed = true;
                }
                else
                {
                    signals.Insert(1, Std.DefineConstant(acc.Value));
                }
            }
            return changed;
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(Std.Subtract(manipulatedInputs));
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
                    if(SimplifySummandsForceAddition(manipulatedInputs) || hasManipulatedInputs)
                    {
                        if(manipulatedInputs.Count == 0)
                            return new SignalSet(IntegerValue.ConstantAdditiveIdentity);
                        if(manipulatedInputs.Count == 1)
                            return manipulatedInputs;
                        return new SignalSet(StdBuilder.Subtract(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                }));
        }
    }
}
