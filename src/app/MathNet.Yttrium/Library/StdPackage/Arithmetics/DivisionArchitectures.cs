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
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Arithmetics
{
    public class DivisionArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Divide", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public DivisionArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Integer"),
                IntegerValueCategory.IsIntegerValueMember,
                delegate(Port port) { return new Process[] { new IntegerValue.DivideProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Rational"),
                RationalValueCategory.IsRationalValueMember,
                delegate(Port port) { return new Process[] { new RationalValue.DivideProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new Process[] { new RealValue.DivideProcess(0, port.InputSignalCount - 1, 0) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new Process[] { new GenericStdFunctionProcess<ComplexValue>(delegate() { return ComplexValue.One; }, ComplexValue.ConvertFrom, ComplexValue.Divide, port.InputSignalCount) }; });
        }

        public static bool SimplifyFactors(SignalSet signals)
        {
            if(signals == null) throw new ArgumentNullException("signals");
            if(Std.IsConstantAdditiveIdentity(signals[0]))
            {
                signals.Clear();
                signals.Add(signals[0]);
                return true;
            }
            bool altered = false;
            for(int i = signals.Count - 1; i > 0; i--) //don't touch first item!
                if(Std.IsConstantMultiplicativeIdentity(signals[i]))
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
                    int cnt = manipulatedInputs.Count - 1;
                    Builder b = context.Builder;

                    Signal[] multiplySignals = new Signal[cnt];
                    Signal[] addSignals = new Signal[cnt];

                    for(int i = 0; i < cnt; i++)
                        multiplySignals[i] = port.InputSignals[i + 1];
                    Signal left = b.DivideSimplified(manipulatedInputs[0], multiplySignals);

                    for(int i = 0; i < cnt; i++)
                    {
                        for(int j = 0; j < cnt; j++)
                            multiplySignals[j] = i == j ? b.Square(port.InputSignals[j + 1]) : port.InputSignals[j + 1];
                        Signal num = b.MultiplySimplified(port.InputSignals[0], manipulatedInputs[i + 1]);
                        addSignals[i] = b.DivideSimplified(num, multiplySignals);
                    }

                    return new SignalSet(b.SubtractSimplified(left, addSignals));
                });

            theorems[1] = new Algebra.AutoSimplifyTransformation(context.Library.LookupEntity(_entityId),
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
                            return new SignalSet(StdPackage.Structures.IntegerValue.ConstantMultiplicativeIdentity(context));
                        if(manipulatedInputs.Count == 1)
                            return manipulatedInputs;
                        return new SignalSet(context.Builder.Divide(manipulatedInputs));
                    }
                    else
                        return port.OutputSignals;
                });

            return theorems;
        }
    }
}
