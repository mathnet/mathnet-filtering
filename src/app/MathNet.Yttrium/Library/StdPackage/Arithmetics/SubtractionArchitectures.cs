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
                delegate(Port port) { return new Process[] { new IntegerValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Rational"),
                RationalValueCategory.IsRationalValueMember,
                delegate(Port port) { return new Process[] { new RationalValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new Process[] { new RealValue.SubtractProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.Zero; }, ComplexValue.ConvertFrom, ComplexValue.Subtract, port.InputSignalCount) }; });
        }

        public static bool SimplifySummands(SignalSet signals)
        {
            if(signals == null) throw new ArgumentNullException("signals");
            bool altered = false;
            for(int i = signals.Count - 1; i > 0; i--) //don't touch first item!
                if(Std.IsConstantAdditiveIdentity(signals[i]))
                {
                    altered = true;
                    signals.RemoveAt(i);
                }
            return altered;
        }

        public static ITheorem[] BuildTheorems(Context context)
        {
            ITheorem[] theorems = new ITheorem[2];

            theorems[0] = new Analysis.DerivativeTransformation(context.Library.LookupEntity(_entityId),
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(context.Builder.SubtractSimplified(manipulatedInputs));
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
                        return new SignalSet(context.Builder.Subtract(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                });

            return theorems;
        }
    }
}
