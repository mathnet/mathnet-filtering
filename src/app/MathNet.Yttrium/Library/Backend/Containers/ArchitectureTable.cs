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
using System.Collections.ObjectModel;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;

namespace MathNet.Symbolics.Backend.Containers
{
    public sealed class ArchitectureTable
    {
        // TODO: consider a more usable/standardized interface. inherit?

        private IdentifierDictionary<List<IArchitectureFactory>> _table;

        public ArchitectureTable()
        {
            _table = new IdentifierDictionary<List<IArchitectureFactory>>(4, 16);
        }

        public bool ContainsEntity(Entity entity)
        {
            return ContainsEntity(entity.EntityId);
        }
        public bool ContainsEntity(MathIdentifier entityId)
        {
            return _table.ContainsId(entityId);
        }


        public ReadOnlyCollection<IArchitectureFactory> LookupEntity(Entity entity)
        {
            return LookupEntity(entity.EntityId);
        }
        public ReadOnlyCollection<IArchitectureFactory> LookupEntity(MathIdentifier entityId)
        {
            return _table.GetValue(entityId).AsReadOnly();
        }

        private List<IArchitectureFactory> AddEntity(MathIdentifier entityId)
        {
            List<IArchitectureFactory> value;
            if(!_table.TryGetValue(entityId, out value))
            {
                value = new List<IArchitectureFactory>();
                _table.Add(entityId, value);
            }
            return value;
        }

        public void AddArchitectureBuilder(IArchitectureFactory factory)
        {
            AddEntity(factory.EntityId).Add(factory);
        }
        public void AddArchitectureBuilder(Entity entity, IArchitectureFactory factory)
        {
            AddArchitectureBuilder(entity.EntityId, factory);
        }
        public void AddArchitectureBuilder(MathIdentifier entityId, IArchitectureFactory factory)
        {
            AddEntity(entityId).Add(factory);
        }
    }
}
