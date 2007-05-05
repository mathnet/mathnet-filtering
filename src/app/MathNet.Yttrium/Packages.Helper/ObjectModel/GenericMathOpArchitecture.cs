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
using System.Text;

using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Packages.ObjectModel
{
    public abstract class GenericMathOpArchitecture : ArchitectureBase, IArchitectureFactory
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

            //for(int i = 0; i < port.OutputSignalCount; i++)
            //    port.OutputSignals[i].PostNewValue(UndefinedSymbol.Instance);
        }

        public abstract IArchitecture InstantiateToPort(Port port);

        public override void UnregisterArchitecture() { }
        protected override void ReregisterArchitecture(Port oldPort, Port newPort) { }
    }
}
