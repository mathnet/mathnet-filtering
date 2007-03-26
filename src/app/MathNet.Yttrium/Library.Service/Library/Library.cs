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
using MathNet.Symbolics.Conversion;

namespace MathNet.Symbolics.Library
{
    public class Library : ILibrary
    {
        private EntityCollection _entities;
        private ArchitectureFactoryCollection _architectures;
        private TheoremProviderCollection _theorems;
        private CustomDataCollection _customData;

        public Library()
        {
            _entities = new EntityCollection();
            _architectures = new ArchitectureFactoryCollection();
            _theorems = new TheoremProviderCollection();
            _customData = new CustomDataCollection();
        }

        public void AddEntity(IEntity entity)
        {
            if(entity == null) throw new ArgumentNullException("entity");
            _entities.Add(entity);
        }

        public void AddArchitecture(IArchitectureFactory factory)
        {
            if(factory == null) throw new ArgumentNullException("factory");
            _architectures.Add(factory);
        }

        public void AddTheoremType(ITheoremProvider provider)
        {
            if(provider == null) throw new ArgumentNullException("provider");
            _theorems.Add(provider);
        }

        public void AddCustomDataType(ICustomDataRef data)
        {
            if(data == null) throw new ArgumentNullException("data");
            _customData.Add(data);
        }

        public void AddCustomDataType<TCustomType>()
             where TCustomType : ICustomData
        {
            _customData.Add(new CustomDataRef(typeof(TCustomType), ValueConverter<TCustomType>.Router));
        }

        public void AddCustomDataType<TCustomType>(TCustomType optionalSingletonInstance)
             where TCustomType : ICustomData
        {
            _customData.Add(new CustomDataRef(typeof(TCustomType), ValueConverter<TCustomType>.Router, optionalSingletonInstance));
        }

        #region Entity Library
        public bool ContainsEntity(MathIdentifier entityId)
        {
            return _entities.Contains(entityId);
        }
        public bool TryLookupEntity(MathIdentifier entityId, out IEntity value)
        {
            return _entities.TryGetValue(entityId, out value);
        }
        public IEntity LookupEntity(MathIdentifier entityId)
        {
            return _entities[entityId];
        }


        public bool ContainsEntity(string symbol)
        {
            return _entities.ContainsSymbol(symbol);
        }
        public bool ContainsEntity(string symbol, InfixNotation notation)
        {
            return _entities.ContainsSymbol(symbol, notation);
        }
        public bool ContainsEntity(string symbol, int inputSignals, int outputSignals, int buses)
        {
            return _entities.ContainsSymbol(symbol, inputSignals, outputSignals, buses);
        }
        public bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            return _entities.ContainsSymbol(symbol, notation, inputSignals, outputSignals, buses);
        }
        public bool ContainsEntity(string symbol, int inputSignals)
        {
            return _entities.ContainsSymbol(symbol, inputSignals);
        }
        public bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals)
        {
            return _entities.ContainsSymbol(symbol, notation, inputSignals);
        }
        public bool ContainsEntity(string symbol, Predicate<IEntity> match)
        {
            return _entities.ContainsSymbol(symbol, match);
        }


        public IList<IEntity> LookupEntities(string symbol)
        {
            return _entities.LookupSymbols(symbol);
        }
        public IList<IEntity> LookupEntities(string symbol, InfixNotation notation)
        {
            return _entities.LookupSymbols(symbol, notation);
        }
        public IList<IEntity> LookupEntities(string symbol, int inputSignals, int outputSignals, int buses)
        {
            return _entities.LookupSymbols(symbol, inputSignals, outputSignals, buses);
        }
        public IList<IEntity> LookupEntities(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            return _entities.LookupSymbols(symbol, notation, inputSignals, outputSignals, buses);
        }
        public IList<IEntity> LookupEntities(string symbol, int inputSignals)
        {
            return _entities.LookupSymbols(symbol, inputSignals);
        }
        public IList<IEntity> LookupEntities(string symbol, InfixNotation notation, int inputSignals)
        {
            return _entities.LookupSymbols(symbol, notation, inputSignals);
        }
        public IList<IEntity> LookupEntities(string symbol, Predicate<IEntity> match)
        {
            return _entities.LookupSymbols(symbol, match);
        }


        public IEntity LookupEntity(string symbol)
        {
            return _entities.LookupSymbol(symbol);
        }
        public IEntity LookupEntity(string symbol, InfixNotation notation)
        {
            return _entities.LookupSymbol(symbol, notation);
        }
        public IEntity LookupEntity(string symbol, int inputSignals, int outputSignals, int buses)
        {
            return _entities.LookupSymbol(symbol, inputSignals, outputSignals, buses);
        }
        public IEntity LookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            return _entities.LookupSymbol(symbol, notation, inputSignals, outputSignals, buses);
        }
        public IEntity LookupEntity(string symbol, int inputSignals)
        {
            return _entities.LookupSymbol(symbol, inputSignals);
        }
        public IEntity LookupEntity(string symbol, InfixNotation notation, int inputSignals)
        {
            return _entities.LookupSymbol(symbol, notation, inputSignals);
        }
        public IEntity LookupEntity(string symbol, Predicate<IEntity> match)
        {
            return _entities.LookupSymbol(symbol, match);
        }


        public bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, out IEntity entity)
        {
            return _entities.TryLookupSymbol(symbol, notation, inputSignals, out entity);
        }
        public bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses, out IEntity entity)
        {
            return _entities.TryLookupSymbol(symbol, notation, inputSignals, outputSignals, buses, out entity);
        }
        public bool TryLookupEntity(string symbol, Predicate<IEntity> match, out IEntity entity)
        {
            return _entities.TryLookupSymbol(symbol, match, out entity);
        }

        public MathIdentifier FindEntityByLabel(string label)
        {
            MathIdentifier id;
            if(!_entities.TryFindDomainOfLabel(label, out id))
                throw new Exceptions.NotFoundException();
            return id;
            // TODO: better exception
        }
        public bool TryFindEntityByLabel(string label, out MathIdentifier id)
        {
            return _entities.TryFindDomainOfLabel(label, out id);
        }
        #endregion

        #region Architecture Library
        public bool ContainsArchitecture(Port port)
        {
            EntityArchitecturesRelation relation;
            return _architectures.TryGetValue(port.Entity.EntityId, out relation) && relation.ContainsArchitectureFor(port);
        }
        public IArchitectureFactory LookupArchitecture(Port port)
        {
            return _architectures[port.Entity.EntityId].LookupArchitectureFor(port);
        }
        public bool TryLookupArchitecture(Port port, out IArchitectureFactory architecture)
        {
            architecture = null;
            EntityArchitecturesRelation relation;
            return _architectures.TryGetValue(port.Entity.EntityId, out relation) && relation.TryLookupArchitectureFor(port, out architecture);
        }
        #endregion

        #region Theorem Library
        public bool ContainsTheoremType(MathIdentifier theoremTypeId)
        {
            return _theorems.Contains(theoremTypeId);
        }
        public ITheoremProvider LookupTheoremType(MathIdentifier theoremTypeId)
        {
            return _theorems[theoremTypeId];
        }
        public bool TryLookupTheoremType(MathIdentifier theoremTypeId, out ITheoremProvider provider)
        {
            return _theorems.TryGetValue(theoremTypeId, out provider);
        }
        #endregion

        #region CustomData Library
        public bool ContainsCustomDataType(MathIdentifier typeId)
        {
            return _customData.Contains(typeId);
        }
        public ICustomDataRef LookupCustomDataType(MathIdentifier typeId)
        {
            return _customData[typeId];
        }
        public bool TryLookupCustomDataType(MathIdentifier typeId, out ICustomDataRef data)
        {
            return _customData.TryGetValue(typeId, out data);
        }
        #endregion
    }
}
