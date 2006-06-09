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
using System.Reflection;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.Backend
{
    public class Library : IContextSensitive
    {
        private EntityTable entityTable;
        private ArchitectureTable archTable;
        private TheoremTable theoremTable;
        private StructureTable structureTable;
        private readonly Context context;

        public Library(Context context)
        {
            this.context = context;
            this.entityTable = new EntityTable();
            this.archTable = new ArchitectureTable();
            this.theoremTable = new TheoremTable();
            this.structureTable = new StructureTable();
        }

        public Context Context
        {
            get { return context; }
        }

        public EntityTable Entities
        {
            get { return entityTable; }
        }

        public ArchitectureTable Architectures
        {
            get { return archTable; }
        }

        public TheoremTable Theorems
        {
            get { return theoremTable; }
        }

        public StructureTable Structures
        {
            get { return structureTable; }
        }

        #region Entity Lookup
        public bool ContainsEntity(MathIdentifier entityId)
        {
            return entityTable.ContainsId(entityId);
        }
        public bool TryLookupEntity(MathIdentifier entityId, out Entity value)
        {
            return entityTable.TryGetValue(entityId, out value);
        }
        public Entity LookupEntity(MathIdentifier entityId)
        {
            return entityTable.GetValue(entityId);
        }
        #region Symbol Based Lookup
        public bool ContainsEntity(string symbol, int inputSignals, int outputSignals, int buses)
        {
            if(!entityTable.ContainsSymbol(symbol))
                return false;
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if((entity.InputSignals.Length == inputSignals
                        && entity.OutputSignals.Length == outputSignals
                        && entity.Buses.Length == buses) || entity.IsGeneric && entity.Buses.Length == buses)
                    return true;
            }
            return false;
        }
        public bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            if(!entityTable.ContainsSymbol(symbol))
                return false;
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol, notation);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if((entity.InputSignals.Length == inputSignals
                        && entity.OutputSignals.Length == outputSignals
                        && entity.Buses.Length == buses) || entity.IsGeneric && entity.Buses.Length == buses)
                    return true;
            }
            return false;
        }
        public bool ContainsEntity(string symbol, int inputSignals)
        {
            if(!entityTable.ContainsSymbol(symbol))
                return false;
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if(entity.InputSignals.Length == inputSignals || entity.IsGeneric)
                    return true;
            }
            return false;
        }
        public bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals)
        {
            if(!entityTable.ContainsSymbol(symbol))
                return false;
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol, notation);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if(entity.InputSignals.Length == inputSignals || entity.IsGeneric)
                    return true;
            }
            return false;
        }
        public Entity LookupEntity(string symbol, int inputSignals, int outputSignals, int buses)
        {
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if((entity.InputSignals.Length == inputSignals
                    && entity.OutputSignals.Length == outputSignals
                    && entity.Buses.Length == buses) || entity.IsGeneric && entity.Buses.Length == buses)
                    return entity;
            }
            throw new MathNet.Symbolics.Backend.Exceptions.SymbolNotAvailableException(symbol);
        }
        public Entity LookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses)
        {
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol, notation);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if((entity.InputSignals.Length == inputSignals
                    && entity.OutputSignals.Length == outputSignals
                    && entity.Buses.Length == buses) || entity.IsGeneric && entity.Buses.Length == buses)
                    return entity;
            }
            throw new MathNet.Symbolics.Backend.Exceptions.SymbolNotAvailableException(symbol);
        }
        public Entity LookupEntity(string symbol, int inputSignals)
        {
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if(entity.InputSignals.Length == inputSignals || entity.IsGeneric)
                    return entity;
            }
            throw new MathNet.Symbolics.Backend.Exceptions.SymbolNotAvailableException(symbol);
        }
        public Entity LookupEntity(string symbol, InfixNotation notation, int inputSignals)
        {
            ReadOnlyCollection<Entity> entry = entityTable.LookupSymbol(symbol, notation);
            for(int i = 0; i < entry.Count; i++)
            {
                Entity entity = entry[i];
                if(entity.InputSignals.Length == inputSignals || entity.IsGeneric)
                    return entity;
            }
            throw new MathNet.Symbolics.Backend.Exceptions.SymbolNotAvailableException(symbol);
        }
        public bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses, out Entity entity)
        {
            entity = null;
            ReadOnlyCollection<Entity> entry;
            if(entityTable.TryLookupSymbol(symbol, notation, out entry))
            {
                for(int i = 0; i < entry.Count; i++)
                {
                    entity = entry[i];
                    if((entity.InputSignals.Length == inputSignals
                        && entity.OutputSignals.Length == outputSignals
                        && entity.Buses.Length == buses) || entity.IsGeneric && entity.Buses.Length == buses)
                        return true;
                }
            }
            return false;
        }
        public bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, out Entity entity)
        {
            entity = null;
            ReadOnlyCollection<Entity> entry;
            if(entityTable.TryLookupSymbol(symbol, notation, out entry))
            {
                for(int i = 0; i < entry.Count; i++)
                {
                    entity = entry[i];
                    if(entity.InputSignals.Length == inputSignals || entity.IsGeneric)
                        return true;
                }
            }
            return false;
        }
        #endregion
        #endregion

        #region Architecture Lookup
        public bool ContainsArchitecture(Port port)
        {
            if(!archTable.ContainsEntity(port.Entity))
                return false;

            ReadOnlyCollection<IArchitectureFactory> entry = archTable.LookupEntity(port.Entity);
            for(int i = 0; i < entry.Count; i++)
            {
                IArchitectureFactory factory = entry[i];
                if(factory.SupportsPort(port))
                    return true;
            }

            return false;
        }
        public IArchitectureFactory LookupArchitecture(Port port)
        {
            ReadOnlyCollection<IArchitectureFactory> entry = archTable.LookupEntity(port.Entity);
            for(int i = 0; i < entry.Count; i++)
            {
                IArchitectureFactory factory = entry[i];
                if(factory.SupportsPort(port))
                    return factory;
            }
            throw new MathNet.Symbolics.Backend.Exceptions.ArchitectureNotAvailableException(port);
        }
        #endregion

        #region Theorem Lookup
        public bool ContainsTheorem(MathIdentifier theoremId)
        {
            return theoremTable.ContainsTheorem(theoremId);
        }
        public bool TryLookupTheorem(MathIdentifier theoremId, out ITheorem theorem)
        {
            return theoremTable.TryLookupTheorem(theoremId, out theorem);
        }
        public bool TryLookupTheorem<T>(MathIdentifier theoremId, out T theorem) where T : ITheorem
        {
            return theoremTable.TryLookupTheorem<T>(theoremId, out theorem);
        }
        public ITheorem LookupTheorem(MathIdentifier theoremId)
        {
            return theoremTable.LookupTheorem(theoremId);
        }

        public bool TryLookupTransformationTheoremType(MathIdentifier transformationTypeId, out TransformationTypeTable value)
        {
            return theoremTable.TryLookupTransformationType(transformationTypeId, out value);
        }
        public TransformationTypeTable LookupTransformationTheoremType(MathIdentifier transformationTypeId)
        {
            return theoremTable.LookupTransformationType(transformationTypeId);
        }
        #endregion

        #region Assembly Analysis
        /// <summary>
        /// Looks for package manager or servers to load and analyze the assembly.
        /// If none is available, a manual analysis is tried instead.
        /// </summary>
        public void LoadAssembly(Assembly assembly)
        {
            Type packageManagerType = typeof(PackageManagerAttribute);
            Type architectureServerType = typeof(ArchitectureServerAttribute);
            Type entityServerType = typeof(EntityServerAttribute);
            Type theoremServerType = typeof(TheoremServerAttribute);
            Type structreServerType = typeof(StructureServerAttribute);

            bool foundPackageManager = false;
            bool foundArchitectureServer = false;
            bool foundEntityServer = false;
            bool foundTheoremServer = false;
            bool foundStructureServer = false;

            foreach(Type t in assembly.GetTypes())
            {
                object instance = null;

                object[] pmas = t.GetCustomAttributes(packageManagerType, false);
                if(pmas.Length > 0)
                {
                    foundPackageManager = true;
                    if(instance == null)
                        instance = Activator.CreateInstance(t, new object[] { context });
                    IPackageManager packageManager = instance as IPackageManager;
                    if(packageManager != null)
                        LoadPackageManager(packageManager);
                }

                object[] asas = t.GetCustomAttributes(architectureServerType, false);
                if(asas.Length > 0)
                {
                    foundArchitectureServer = true;
                    if(instance == null)
                        instance = Activator.CreateInstance(t, new object[] { context });
                    IArchitectureServer architectureServer = instance as IArchitectureServer;
                    if(architectureServer != null)
                        LoadArchitectureServer(architectureServer);
                }

                object[] esas = t.GetCustomAttributes(entityServerType, false);
                if(esas.Length > 0)
                {
                    foundEntityServer = true;
                    if(instance == null)
                        instance = Activator.CreateInstance(t, new object[] { context });
                    IEntityServer entityServer = instance as IEntityServer;
                    if(entityServer != null)
                        LoadEntityServer(entityServer);
                }

                object[] tsas = t.GetCustomAttributes(theoremServerType, false);
                if(tsas.Length > 0)
                {
                    foundTheoremServer = true;
                    if(instance == null)
                        instance = Activator.CreateInstance(t, new object[] { context });
                    ITheoremServer theoremServer = instance as ITheoremServer;
                    if(theoremServer != null)
                        LoadTheoremServer(theoremServer);
                }

                object[] ssas = t.GetCustomAttributes(structreServerType, false);
                if(ssas.Length > 0)
                {
                    foundStructureServer = true;
                    if(instance == null)
                        instance = Activator.CreateInstance(t, new object[] { context });
                    IStructureServer structureServer = instance as IStructureServer;
                    if(structureServer != null)
                        LoadStructureServer(structureServer);
                }
            }

            if(!foundPackageManager && !foundArchitectureServer && !foundEntityServer && !foundTheoremServer && !foundStructureServer)
                LoadAssemblyManual(assembly);
        }

        /// <summary>
        /// Loads and analyzes an assembly without the help of any package manager or server.
        /// </summary>
        public void LoadAssemblyManual(Assembly assembly)
        {
            Type entityImplementationType = typeof(EntityImplementationAttribute);

            foreach(Type t in assembly.GetTypes())
            {
                object instance = null;

                object[] eias = t.GetCustomAttributes(entityImplementationType, false);
                if(eias.Length > 0)
                {
                    if(instance == null)
                        instance = Activator.CreateInstance(t);
                    IArchitectureFactory builder = instance as IArchitectureFactory;
                    if(builder != null)
                        foreach(EntityImplementationAttribute attr in eias)
                        {
                            Entity e;
                            if(entityTable.TryGetValue(attr.EntityId, out e))
                                archTable.AddArchitectureBuilder(e, builder);
                        }
                }
            }
        }

        public void LoadPackageManager(IPackageManager packageManager) //, PackageManagerAttribute attribute)
        {
            if(packageManager == null) throw new ArgumentNullException("packageManager");
            packageManager.Entities.AppendEntities(entityTable);
            packageManager.Architectures.AppendArchitectures(archTable);
            packageManager.Theorems.AppendTheorems(theoremTable);
            packageManager.Structures.AppendStructures(structureTable);
        }

        public void LoadArchitectureServer(IArchitectureServer architectureServer) //, ArchitectureServerAttribute attribute)
        {
            if(architectureServer == null) throw new ArgumentNullException("architectureServer");
            architectureServer.AppendArchitectures(archTable);
        }

        public void LoadEntityServer(IEntityServer entityServer) //, EntityServerAttribute attribute)
        {
            if(entityServer == null) throw new ArgumentNullException("entityServer");
            entityServer.AppendEntities(entityTable);
        }

        public void LoadTheoremServer(ITheoremServer theoremServer)
        {
            if(theoremServer == null) throw new ArgumentNullException("theoremServer");
            theoremServer.AppendTheorems(theoremTable);
        }

        public void LoadStructureServer(IStructureServer structureServer)
        {
            if(structureServer == null) throw new ArgumentNullException("structureServer");
            structureServer.AppendStructures(structureTable);
        }
        #endregion
    }
}
