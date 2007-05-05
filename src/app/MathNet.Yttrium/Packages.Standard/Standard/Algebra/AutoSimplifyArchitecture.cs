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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Manipulation;

namespace MathNet.Symbolics.Packages.Standard.Algebra
{
    /// <remarks>input 1..n: signals to derive; input n+1: variable; output 1..n: derived signals.</remarks>
    //[EntityImplementation("AutoSimplify", "Std")]
    public class AutoSimplifyArchitecture : GenericMathOpArchitecture
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("AutoSimplify", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public AutoSimplifyArchitecture() : base(_entityId) { }
        public AutoSimplifyArchitecture(Port port) : base(_entityId, port) { }

        public override ISignalSet ExecuteMathematicalOperator()
        {
            int cnt = Port.OutputSignalCount;
            SignalSet ret = new SignalSet();
            for(int i = 0; i < cnt; i++)
                ret.Add(Std.AutoSimplify(Port.InputSignals[i]));
            return ret;
        }

        public override bool SupportsPort(Port port)
        {
            return port != null && port.IsCompletelyConnected;
        }

        public override IArchitecture InstantiateToPort(Port port)
        {
            return new AutoSimplifyArchitecture(port);
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Algebra.AutoSimplifyTransformation.Provider.Add(
                new Algebra.AutoSimplifyTransformation(new MathIdentifier("GenericMathOpAutoSimplify", "Std"),
                delegate()
                {
                    return new Pattern(new ArchitectureCondition(delegate(IArchitecture a) { return a.IsMathematicalOperator; }));
                },
                delegate(Port port)
                {
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    if(hasManipulatedInputs)
                        port = port.Entity.InstantiatePort(manipulatedInputs);
                    if(port.HasArchitectureLink && port.CurrentArchitecture.IsMathematicalOperator)
                        return port.CurrentArchitecture.ExecuteMathematicalOperator();
                    else
                        return port.OutputSignals;
                }));
        }
    }
}