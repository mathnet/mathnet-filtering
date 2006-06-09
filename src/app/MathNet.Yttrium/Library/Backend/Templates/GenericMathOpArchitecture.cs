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
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.StdPackage.Algebra;

namespace MathNet.Symbolics.Backend.Templates
{
    public abstract class GenericMathOpArchitecture : Architecture, IArchitectureFactory
    {
        /// <summary>Architecture Builder Constructor</summary>
        protected GenericMathOpArchitecture(MathIdentifier entityId) : base(entityId, entityId, true) { }
        /// <summary>Architecture Builder Constructor</summary>
        protected GenericMathOpArchitecture(MathIdentifier id, MathIdentifier entityId) : base(id, entityId, true) { }

        /// <summary>Port Instance Constructor</summary>
        protected GenericMathOpArchitecture(MathIdentifier entityId, Port port) : this(entityId, entityId, port) { }
        /// <summary>Port Instance Constructor</summary>
        protected GenericMathOpArchitecture(MathIdentifier id, MathIdentifier entityId, Port port)
            : base(id, entityId, true)
        {
            if(port == null) throw new ArgumentNullException("port");
            //System.Diagnostics.Debug.Assert(SupportsPort(port));
            SetPort(port);

            for(int i = 0; i < port.OutputSignalCount; i++)
                port.OutputSignals[i].PostNewValue(UndefinedSymbol.Instance);
        }

        public abstract Architecture InstantiateToPort(Port port);

        public override void UnregisterArchitecture() { }
        protected override void ReregisterArchitecture(Port oldPort, Port newPort) { }

        public static ITheorem[] BuildTheorems(Context context)
        {
            ITheorem[] theorems = new ITheorem[1];

            theorems[0] = new AutoSimplifyTransformation("GenericMathOpAutoSimplify", "Std",
                delegate(Port port)
                {
                    return port.HasArchitectureLink && port.CurrentArchitecture.IsMathematicalOperator;
                },
                delegate(Port port)
                {
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    if(hasManipulatedInputs)
                        port = port.Entity.InstantiatePort(context, manipulatedInputs);
                    if(port.HasArchitectureLink && port.CurrentArchitecture.IsMathematicalOperator)
                        return port.CurrentArchitecture.ExecuteMathematicalOperator();
                    else
                        return port.OutputSignals;
                });

            return theorems;
        }
    }
}
