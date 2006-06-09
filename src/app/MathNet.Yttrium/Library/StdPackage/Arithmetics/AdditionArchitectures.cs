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

namespace MathNet.Symbolics.StdPackage.Arithmetics
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

        public Architecture InstantiateToPort(Port port)
        {
            if(IntegerValueCategory.IsIntegerValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Integer"), EntityId, false, port, IntegerValueCategory.IsIntegerValueMember, 0, new Process[] { new IntegerValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(RationalValueCategory.IsRationalValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Rational"), EntityId, false, port, RationalValueCategory.IsRationalValueMember, 0, new Process[] { new RationalValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(RealValueCategory.IsRealValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Real"), EntityId, false, port, RealValueCategory.IsRealValueMember, 0, new Process[] { new RealValue.AddProcess(0, port.InputSignalCount - 1, 0) });

            if(ComplexValueCategory.IsComplexValueMember(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Complex"), EntityId, false, port, RealValueCategory.IsRealValueMember, 0, new Process[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.Zero; }, ComplexValue.ConvertFrom, ComplexValue.Add, port.InputSignalCount) });

            if(IsVector(port))
                return new GenericArchitecture(EntityId.DerivePrefix("Vector"), EntityId, false, port, IsVector, 0, new Process[] { }); //DUMMY

            throw new MathNet.Symbolics.Backend.Exceptions.ArchitectureNotAvailableException(port);
        }

        public static bool SimplifySummands(SignalSet signals)
        {
            return 0 < signals.RemoveAll(Std.IsConstantAdditiveIdentity);
        }

        public static ITheorem[] BuildTheorems(Context context)
        {
            ITheorem[] theorems = new ITheorem[2];

            theorems[0] = new Analysis.DerivativeTransformation(context.Library.LookupEntity(_entityId),
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(context.Builder.AddSimplified(manipulatedInputs));
                });

            theorems[1] = new Algebra.AutoSimplifyTransformation(context.Library.LookupEntity(_entityId),
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
                            return new SignalSet(StdPackage.Structures.IntegerValue.ConstantAdditiveIdentity(context));
                        if(manipulatedInputs.Count == 1)
                            return manipulatedInputs;
                        return new SignalSet(context.Builder.Add(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                });

            return theorems;
        }
    }
}
