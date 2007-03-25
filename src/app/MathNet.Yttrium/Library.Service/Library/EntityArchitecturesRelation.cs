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
using System.Collections.ObjectModel;
using System.Text;

namespace MathNet.Symbolics.Library
{
    internal class EntityArchitecturesRelation
    {
        private readonly MathIdentifier _entityId;
        private readonly List<IArchitectureFactory> _architectures;

        public EntityArchitecturesRelation(MathIdentifier entityId)
        {
            _entityId = entityId;
            _architectures = new List<IArchitectureFactory>();
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public List<IArchitectureFactory> Architectures
        {
            get { return _architectures; }
        }

        public bool ContainsArchitectureFor(Port port)
        {
            return _architectures.Exists(delegate(IArchitectureFactory factory) { return factory.SupportsPort(port); });
        }

        public IList<IArchitectureFactory> LookupArchitecturesFor(Port port)
        {
            return _architectures.FindAll(delegate(IArchitectureFactory factory) { return factory.SupportsPort(port); });
        }

        public IArchitectureFactory LookupArchitectureFor(Port port)
        {
            return _architectures.Find(delegate(IArchitectureFactory factory) { return factory.SupportsPort(port); });
        }

        public bool TryLookupArchitectureFor(Port port, out IArchitectureFactory architecture)
        {
            architecture = _architectures.Find(delegate(IArchitectureFactory factory) { return factory.SupportsPort(port); });
            return architecture != null;
        }

        
    }
}
