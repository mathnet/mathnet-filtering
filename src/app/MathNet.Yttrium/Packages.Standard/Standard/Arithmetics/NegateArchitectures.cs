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
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
{
    public class NegateArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Negate", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public NegateArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Integer"),
                IntegerValueCategory.IsIntegerValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<IntegerValue,IntegerValue>(delegate(IntegerValue value) { return value.Negate(); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Rational"),
                RationalValueCategory.IsRationalValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<RationalValue, RationalValue>(delegate(RationalValue value) { return value.Negate(); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<RealValue, RealValue>(delegate(RealValue value) { return value.Negate(); }, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<ComplexValue, ComplexValue>(ComplexValue.Negate, ComplexValue.ConvertFrom, port.InputSignalCount) }; });
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return StdBuilder.Negate(manipulatedInputs);
                }));
        }
    }
}
