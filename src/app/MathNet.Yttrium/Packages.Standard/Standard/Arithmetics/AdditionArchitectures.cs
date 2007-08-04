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
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Conversion;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
{
    // TODO: Finally upgrade to the new generic design (maybe add to the documentation...)
    public class AdditionArchitectures : IArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Add", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public AdditionArchitectures()
        {
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public bool SupportsPort(Port port)
        {
            if(port == null || !port.Entity.EqualsById(_entityId))
                return false;

            if(IntegerValueCategory.IsIntegerValueMember(port))
                return true;

            if(RationalValueCategory.IsRationalValueMember(port))
                return true;

            if(RealValueCategory.IsRealValueMember(port))
                return true;

            if(ComplexValueCategory.IsComplexValueMember(port))
                return true;

            if(IsVector(port)) //DUMMY
                return true;

            return false;
        }

        private static bool IsVector(Port port)
        {
            return false; //DUMMY
        }

        public IArchitecture InstantiateToPort(Port port)
        {
            if(IntegerValueCategory.IsIntegerValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Integer"), EntityId, false, port, IntegerValueCategory.IsIntegerValueMember, 0, new ProcessBase[] { new IntegerValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(RationalValueCategory.IsRationalValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Rational"), EntityId, false, port, RationalValueCategory.IsRationalValueMember, 0, new ProcessBase[] { new RationalValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(RealValueCategory.IsRealValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Real"), EntityId, false, port, RealValueCategory.IsRealValueMember, 0, new ProcessBase[] { new RealValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(ComplexValueCategory.IsComplexValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Complex"), EntityId, false, port, RealValueCategory.IsRealValueMember, 0, new ProcessBase[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.Zero; }, ComplexValue.ConvertFrom, ComplexValue.Add, port.InputSignalCount) });

            if(IsVector(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Vector"), EntityId, false, port, IsVector, 0, new ProcessBase[] { }); //DUMMY

            throw new MathNet.Symbolics.Exceptions.ArchitectureNotAvailableException(port);
        }

        public static bool CollectSummands(ISignalSet signals)
        {
            bool changed = false;
            for(int i = 0; i < signals.Count; i++)
            {
                Signal s = signals[i];
                if(!s.BehavesAsBeingDriven(false))
                    continue;

                Port p = s.DrivenByPort;
                if(p.Entity.EntityId.Equals(AdditionArchitectures.EntityIdentifier))
                {
                    signals.RemoveAt(i);
                    ISignalSet inputs = p.InputSignals;
                    for(int j = 0; j < inputs.Count; j++)
                        signals.Insert(i + j, inputs[j]);
                    i--;
                    changed = true;
                    continue;
                }

                if(p.Entity.EntityId.Equals(SubtractionArchitectures.EntityIdentifier))
                {
                    ISignalSet inputs = p.InputSignals;
                    signals[i] = inputs[0];
                    i--;
                    for(int j = 1; j < inputs.Count; j++)
                        signals.Insert(i + j, Std.Negate(inputs[j]));
                    changed = true;
                    continue;
                }
            }
            return changed;
        }

        public static bool SimplifySummands(ISignalSet signals)
        {
            bool changed = CollectSummands(signals);
            if(signals.Count < 2)
                return changed;
            IAccumulator acc = null;
            for(int i = signals.Count - 1; i >= 0; i--)
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
                signals.Insert(0, Std.DefineConstant(acc.Value));
            }
            return changed;
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(Std.Add(manipulatedInputs));
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
                    if(SimplifySummands(manipulatedInputs) || hasManipulatedInputs)
                    {
                        if(manipulatedInputs.Count == 0)
                            return new SignalSet(IntegerValue.ConstantAdditiveIdentity);
                        if(manipulatedInputs.Count == 1)
                            return manipulatedInputs;
                        return new SignalSet(StdBuilder.Add(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                }));
        }
    }
}
