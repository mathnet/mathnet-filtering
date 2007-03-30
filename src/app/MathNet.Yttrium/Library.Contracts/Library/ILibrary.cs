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
    public interface ILibrary
    {
        //MathNet.Symbolics.Backend.Containers.EntityTable Entities { get; }
        //MathNet.Symbolics.Backend.Containers.ArchitectureTable Architectures { get; }
        //MathNet.Symbolics.Backend.Containers.TheoremTable Theorems { get; }
        //MathNet.Symbolics.Backend.ValueConversion.StructureTable Structures { get; }

        void AddEntity(IEntity entity);
        void AddArchitecture(IArchitectureFactory factory);
        void AddTheoremType(ITheoremProvider provider);
        void AddCustomDataType(ICustomDataRef data);
        void AddCustomDataType<TCustomType>() where TCustomType : ICustomData;
        void AddCustomDataType<TCustomType>(TCustomType optionalSingletonInstance) where TCustomType : ICustomData;
        void AddArbitraryType(Type type);

        #region Entity Library
        bool ContainsEntity(MathIdentifier entityId);
        bool TryLookupEntity(MathIdentifier entityId, out IEntity value);
        IEntity LookupEntity(MathIdentifier entityId);

        // TODO: Which ones of all those overloads are actually used in real-world code? find out! (and next time use TDD...)

        bool ContainsEntity(string symbol);
        bool ContainsEntity(string symbol, InfixNotation notation);
        bool ContainsEntity(string symbol, int inputSignals, int outputSignals, int buses);
        bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses);
        bool ContainsEntity(string symbol, int inputSignals);
        bool ContainsEntity(string symbol, InfixNotation notation, int inputSignals);
        bool ContainsEntity(string symbol, Predicate<IEntity> match);

        IList<IEntity> LookupEntities(string symbol);
        IList<IEntity> LookupEntities(string symbol, InfixNotation notation);
        IList<IEntity> LookupEntities(string symbol, int inputSignals);
        IList<IEntity> LookupEntities(string symbol, InfixNotation notation, int inputSignals);
        IList<IEntity> LookupEntities(string symbol, int inputSignals, int outputSignals, int buses);
        IList<IEntity> LookupEntities(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses);
        IList<IEntity> LookupEntities(string symbol, Predicate<IEntity> match);

        IEntity LookupEntity(string symbol);
        IEntity LookupEntity(string symbol, InfixNotation notation);
        IEntity LookupEntity(string symbol, int inputSignals);
        IEntity LookupEntity(string symbol, InfixNotation notation, int inputSignals);
        IEntity LookupEntity(string symbol, int inputSignals, int outputSignals, int buses);
        IEntity LookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses);
        IEntity LookupEntity(string symbol, Predicate<IEntity> match);

        bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, out IEntity entity);
        bool TryLookupEntity(string symbol, InfixNotation notation, int inputSignals, int outputSignals, int buses, out IEntity entity);
        bool TryLookupEntity(string symbol, Predicate<IEntity> match, out IEntity entity);

        MathIdentifier FindEntityByLabel(string label);
        bool TryFindEntityByLabel(string label, out MathIdentifier id);

        ReadOnlyCollection<IEntity> GetAllEntities();
        #endregion

        #region Architecture Library
        bool ContainsArchitecture(Port port);
        IArchitectureFactory LookupArchitecture(Port port);
        bool TryLookupArchitecture(Port port, out IArchitectureFactory architecture);
        #endregion

        #region Theorem Library
        bool ContainsTheoremType(MathIdentifier theoremTypeId);
        ITheoremProvider LookupTheoremType(MathIdentifier theoremTypeId);
        bool TryLookupTheoremType(MathIdentifier theoremTypeId, out ITheoremProvider provider);
        #endregion

        #region CustomData Library
        bool ContainsCustomDataType(MathIdentifier typeId);
        ICustomDataRef LookupCustomDataType(MathIdentifier typeId);
        bool TryLookupCustomDataType(MathIdentifier typeId, out ICustomDataRef data);
        #endregion

        #region Arbitrary Type Library
        bool ContainsArbitraryType(MathIdentifier id);
        bool ContainsArbitraryType(Type type);
        Type LookupArbitraryType(MathIdentifier id);
        MathIdentifier LookupArbitraryType(Type type);
        bool TryLookupArbitraryType(MathIdentifier id, out Type type);
        #endregion

        #region Theorem Lookup
        //bool ContainsTheorem(MathIdentifier theoremId);
        //bool TryLookupTheorem(MathIdentifier theoremId, out MathNet.Symbolics.Backend.Theorems.ITheorem theorem);
        //bool TryLookupTheorem<T>(MathIdentifier theoremId, out T theorem) where T : MathNet.Symbolics.Backend.Theorems.ITheorem;
        //MathNet.Symbolics.Backend.Theorems.ITheorem LookupTheorem(MathIdentifier theoremId);
        //bool TryLookupTransformationTheoremType(MathIdentifier transformationTypeId, out MathNet.Symbolics.Backend.Containers.TransformationTheoremTypeTable value);
        //MathNet.Symbolics.Backend.Containers.TransformationTheoremTypeTable LookupTransformationTheoremType(MathIdentifier transformationTypeId);
        #endregion

        #region Assembly Analysis
        //void LoadAssembly(System.Reflection.Assembly assembly);
        //void LoadAssemblyManual(System.Reflection.Assembly assembly);
        //void LoadPackageManager(MathNet.Symbolics.Backend.Discovery.IPackageManager packageManager);
        //void LoadArchitectureServer(MathNet.Symbolics.Backend.Discovery.IArchitectureServer architectureServer);
        //void LoadEntityServer(MathNet.Symbolics.Backend.Discovery.IEntityServer entityServer);
        //void LoadTheoremServer(MathNet.Symbolics.Backend.Discovery.ITheoremServer theoremServer);
        //void LoadStructureServer(MathNet.Symbolics.Backend.Discovery.IStructureServer structureServer);
        #endregion
    }
}
