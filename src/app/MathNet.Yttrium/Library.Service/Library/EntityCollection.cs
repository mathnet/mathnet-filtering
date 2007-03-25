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
    internal sealed class EntityCollection : KeyedCollection<MathIdentifier, IEntity>
    {
        private readonly Dictionary<string, List<IEntity>> _symbolTable;

        public EntityCollection()
        {
            _symbolTable = new Dictionary<string, List<IEntity>>(128, Config.IdentifierComparer);
        }

        public bool TryGetValue(MathIdentifier entityId, out IEntity entity)
        {
            entity = null;
            if(!Contains(entityId))
                return false;
            entity = base[entityId];
            return true;
        }

        protected override MathIdentifier GetKeyForItem(IEntity item)
        {
            return item.EntityId;
        }

        protected override void InsertItem(int index, IEntity item)
        {
            base.InsertItem(index, item);
            AddSymbolEntry(item);
        }

        protected override void RemoveItem(int index)
        {
            RemoveSymbolEntry(base[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, IEntity item)
        {
            RemoveSymbolEntry(base[index]);
            base.SetItem(index, item);
            AddSymbolEntry(item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _symbolTable.Clear();
        }

        private void AddSymbolEntry(IEntity entity)
        {
            List<IEntity> item;
            if(!_symbolTable.TryGetValue(entity.Symbol, out item))
            {
                item = new List<IEntity>();
                _symbolTable.Add(entity.Symbol, item);
            }
            if(!item.Contains(entity))
                item.Add(entity);
        }

        private void RemoveSymbolEntry(IEntity entity)
        {
            List<IEntity> item;
            if(!_symbolTable.TryGetValue(entity.Symbol, out item))
                return;
            int idx = item.IndexOf(entity);

            // Remove Entity-entry in the table of this symbol
            if(idx >= 0) 
                item.RemoveAt(idx);

            // Remove Symbol if no more Entity-entry
            if(item.Count == 0) 
                _symbolTable.Remove(entity.Symbol);
        }

        public bool ContainsSymbol(string symbol)
        {
            return _symbolTable.ContainsKey(symbol);
        }
        public bool ContainsSymbol(string symbol, InfixNotation notation)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(IEntity e) { return e.Notation == notation; });
            return false;
        }
        public bool ContainsSymbol(string symbol, int inputSignals)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(IEntity e) { return e.InputSignals.Length == inputSignals || e.IsGeneric; });
            return false;
        }
        public bool ContainsSymbol(string symbol, InfixNotation notation, int inputSignals)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(IEntity e) { return e.Notation == notation && (e.InputSignals.Length == inputSignals || e.IsGeneric); });
            return false;
        }
        public bool ContainsSymbol(string symbol, int inputSignals, int outputSignals, int buses)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(IEntity e) { return (e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses); });
            return false;
        }
        public bool ContainsSymbol(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(delegate(IEntity e) { return e.Notation == notation && ((e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses)); });
            return false;
        }
        public bool ContainsSymbol(string symbol, Predicate<IEntity> match)
        {
            List<IEntity> entities;
            if(_symbolTable.TryGetValue(symbol, out entities))
                return entities.Exists(match);
            return false;
        }
        

        public IList<IEntity> LookupSymbols(string symbol)
        {
            return _symbolTable[symbol].AsReadOnly();
        }
        public IList<IEntity> LookupSymbols(string symbol, InfixNotation notation)
        {
            return _symbolTable[symbol].FindAll(delegate(IEntity e) { return e.Notation == notation; });
        }
        public IList<IEntity> LookupSymbols(string symbol, int inputSignals)
        {
            return _symbolTable[symbol].FindAll(delegate(IEntity e) { return e.InputSignals.Length == inputSignals || e.IsGeneric; });
        }
        public IList<IEntity> LookupSymbols(string symbol, InfixNotation notation, int inputSignals)
        {
            return _symbolTable[symbol].FindAll(delegate(IEntity e) { return e.Notation == notation && (e.InputSignals.Length == inputSignals || e.IsGeneric); });
        }
        public IList<IEntity> LookupSymbols(string symbol, int inputSignals, int outputSignals, int buses)
        {
            return _symbolTable[symbol].FindAll(delegate(IEntity e) { return (e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses); });
        }
        public IList<IEntity> LookupSymbols(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            return _symbolTable[symbol].FindAll(delegate(IEntity e) { return e.Notation == notation && ((e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses)); });
        }
        public IList<IEntity> LookupSymbols(string symbol, Predicate<IEntity> match)
        {
            return _symbolTable[symbol].FindAll(match);
        }


        public IEntity LookupSymbol(string symbol)
        {
            return _symbolTable[symbol][0];
        }
        public IEntity LookupSymbol(string symbol, InfixNotation notation)
        {
            return _symbolTable[symbol].Find(delegate(IEntity e) { return e.Notation == notation; });
        }
        public IEntity LookupSymbol(string symbol, int inputSignals)
        {
            return _symbolTable[symbol].Find(delegate(IEntity e) { return e.InputSignals.Length == inputSignals || e.IsGeneric; });
        }
        public IEntity LookupSymbol(string symbol, InfixNotation notation, int inputSignals)
        {
            return _symbolTable[symbol].Find(delegate(IEntity e) { return e.Notation == notation && (e.InputSignals.Length == inputSignals || e.IsGeneric); });
        }
        public IEntity LookupSymbol(string symbol, int inputSignals, int outputSignals, int buses)
        {
            return _symbolTable[symbol].Find(delegate(IEntity e) { return (e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses); });
        }
        public IEntity LookupSymbol(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            return _symbolTable[symbol].Find(delegate(IEntity e) { return e.Notation == notation && ((e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses)); });
        }
        public IEntity LookupSymbol(string symbol, Predicate<IEntity> match)
        {
            return _symbolTable[symbol].Find(match);
        }


        public bool TryLookupSymbol(string symbol, InfixNotation notation, int inputSignals, out IEntity entity)
        {
            entity = null;
            List<IEntity> list;
            if(!_symbolTable.TryGetValue(symbol, out list))
                return false;
            entity = list.Find(delegate(IEntity e) { return e.Notation == notation && (e.InputSignals.Length == inputSignals || e.IsGeneric); });
            return entity != null;
        }
        public bool TryLookupSymbol(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses, out IEntity entity)
        {
            entity = null;
            List<IEntity> list;
            if(!_symbolTable.TryGetValue(symbol, out list))
                return false;
            entity = list.Find(delegate(IEntity e) { return e.Notation == notation && ((e.InputSignals.Length == inputSignals && e.OutputSignals.Length == outputSignals && e.Buses.Length == buses) || (e.IsGeneric && e.Buses.Length == buses)); });
            return entity != null;
        }
        public bool TryLookupSymbol(string symbol, Predicate<IEntity> match, out IEntity entity)
        {
            entity = null;
            List<IEntity> list;
            if(!_symbolTable.TryGetValue(symbol, out list))
                return false;
            entity = list.Find(match);
            return entity != null;
        }


        public bool TryFindDomainOfLabel(string label, out MathIdentifier id)
        {
            foreach(MathIdentifier mid in Dictionary.Keys)
                if(mid.Label.Equals(label))
                {
                    id = mid;
                    return true;
                }
            return false;
        }
    }
}
