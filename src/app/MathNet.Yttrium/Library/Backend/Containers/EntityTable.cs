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
    public class EntityTable : IdentifierDictionary<Entity>
    {
        private readonly Dictionary<string, List<Entity>> _symbolTable;

        public EntityTable()
            : base(4, 64)
        {
            _symbolTable = new Dictionary<string, List<Entity>>(128, Context.IdentifierComparer);
        }

        public bool ContainsSymbol(string symbol)
        {
            return _symbolTable.ContainsKey(symbol);
        }
        public bool ContainsSymbol(string symbol, InfixNotation notation)
        {
            List<Entity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(Entity e) { return e.Notation == notation; });
            return false;
        }

        public ReadOnlyCollection<Entity> LookupSymbol(string symbol)
        {
            return _symbolTable[symbol].AsReadOnly();
        }
        public ReadOnlyCollection<Entity> LookupSymbol(string symbol, InfixNotation notation)
        {
            List<Entity> filteredEntities = _symbolTable[symbol].FindAll(delegate(Entity e) { return e.Notation == notation; });
            return filteredEntities.AsReadOnly();
        }
        public bool TryLookupSymbol(string symbol, InfixNotation notation, out ReadOnlyCollection<Entity> entities)
        {
            entities = null;
            List<Entity> list;
            if(!_symbolTable.TryGetValue(symbol, out list))
                return false;
            list = list.FindAll(delegate(Entity e) { return e.Notation == notation; });
            if(list.Count > 0)
            {
                entities = list.AsReadOnly();
                return true;
            }
            return false;
        }

        private List<Entity> AddSymbol(string symbol)
        {
            if(_symbolTable.ContainsKey(symbol))
                return _symbolTable[symbol];
            List<Entity> entry = new List<Entity>(16);
            _symbolTable.Add(symbol, entry);
            return entry;
        }

        public void Add(Entity value)
        {
            Add(value.EntityId, value);
        }
        public override void Add(MathIdentifier id, Entity value)
        {
            base.Add(id, value);
            AddSymbol(value.Symbol).Add(value);
        }

        public void Remove(Entity entity)
        {
            Remove(entity.EntityId);
        }
        public override void Remove(MathIdentifier id)
        {
            Entity entity;
            if(TryGetValue(id, out entity))
            {
                List<Entity> item;
                if(_symbolTable.TryGetValue(entity.Symbol, out item))
                    item.Remove(entity);
            }
            base.Remove(id);
            
        }
    }
}
