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

using MathNet.Symbolics;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Utils;

namespace MathNet.Symbolics.Library
{
    internal sealed class ArchitectureFactoryCollection : KeyedCollection<MathIdentifier, EntityArchitecturesRelation>
    {
        public ArchitectureFactoryCollection()
        {
        }

        public bool TryGetValue(MathIdentifier entityId, out EntityArchitecturesRelation architectures)
        {
            architectures = null;
            if(!Contains(entityId))
                return false;
            architectures = base[entityId];
            return true;
        }

        protected override MathIdentifier GetKeyForItem(EntityArchitecturesRelation item)
        {
            return item.EntityId;
        }

        public void Add(IArchitectureFactory architecture)
        {
            EntityArchitecturesRelation relation;
            if(!TryGetValue(architecture.EntityId, out relation))
            {
                relation = new EntityArchitecturesRelation(architecture.EntityId);
                Add(relation);
            }
            if(!relation.Architectures.Contains(architecture))
                relation.Architectures.Add(architecture);
        }

        public void Remove(IArchitectureFactory architecture)
        {
            EntityArchitecturesRelation relation;
            if(!TryGetValue(architecture.EntityId, out relation))
                return;
            int idx = relation.Architectures.IndexOf(architecture);

            // Remove Factory-entry in the table of this entity
            if(idx >= 0)
                relation.Architectures.RemoveAt(idx);

            // Remove Entity if no more Architecture-entry
            if(relation.Architectures.Count == 0)
                Remove(relation.EntityId);
        }


        public bool Contains(IEntity entity)
        {
            return Contains(entity.EntityId);
        }
    }
}
