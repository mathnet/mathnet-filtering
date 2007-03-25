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
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Trigonometry
{
    public class SecantArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Secant", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public SecantArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Real"),
                RealValueCategory.IsRealValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<RealValue, RealValue>(delegate(RealValue value) { return value.Secant(); }, RealValue.ConvertFrom, port.InputSignalCount) }; });

            AddArchitecture(EntityId.DerivePrefix("Complex"),
                ComplexValueCategory.IsComplexValueMember,
                delegate(Port port) { return new ProcessBase[] { new GenericStdParallelProcess<ComplexValue, ComplexValue>(delegate(ComplexValue value) { return value.Secant(); }, ComplexValue.ConvertFrom, port.InputSignalCount) }; });
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    ReadOnlySignalSet tangents = StdBuilder.Tangent(port.InputSignals);
                    SignalSet outputs = new SignalSet();
                    for(int i = 0; i < manipulatedInputs.Count; i++)
                        outputs.Add(Std.Multiply(port.OutputSignals[i], tangents[i], manipulatedInputs[i]));
                    return outputs;
                }));

            MathIdentifier typeId = new MathIdentifier("TrigonometricSubstitute", "Std");
            ITheoremProvider basicProvider;
            if(!library.TryLookupTheoremType(typeId, out basicProvider))
            {
                basicProvider = Binder.GetInstance<ITransformationTheoremProvider, MathIdentifier>(typeId);
                library.AddTheoremType(basicProvider);
            }
            ((ITransformationTheoremProvider)basicProvider).Add(
                new BasicTransformation(_entityId.DerivePostfix("TrigonometricSubstitute"), typeId,
                delegate() { return new Pattern(new EntityCondition(_entityId)); },
                delegate(Port port) { return ManipulationPlan.DoAlter; },
                delegate(Port port, SignalSet transformedInputs, bool hasTransformedInputs)
                {
                    return StdBuilder.Invert(StdBuilder.Cosine(transformedInputs));
                    //Signal[] ret = new Signal[transformedInputs.Length];
                    //for(int i = 0; i < ret.Length; i++)
                    //    ret[i] = StdBuilder.Invert(StdBuilder.Cosine(port.Context, transformedInputs[i]));
                    //return ret;
                }));
        }
    }
}
