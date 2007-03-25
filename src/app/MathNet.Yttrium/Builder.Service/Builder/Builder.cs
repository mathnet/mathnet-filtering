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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Builder
{
    public class Builder : IBuilder
    {
        private readonly ILibrary _library;

        internal Builder()
        {
            _library = Service<ILibrary>.Instance;
        }

        #region Building Single-Value Functions
        public Signal Function(IEntity entity, Signal argument)
        {
            Port port = entity.InstantiatePort(argument);

            if(port.InputSignalCount != 1 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("1 input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");

            return port[0];
        }
        public Signal Function(IEntity entity, Signal argument1, Signal argument2)
        {
            Port port = entity.InstantiatePort(argument1, argument2);

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("2 input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");

            return port[0];
        }
        public Signal Function(IEntity entity, params Signal[] arguments)
        {
            Port port = entity.InstantiatePort(arguments);

            if(port.InputSignalCount != arguments.Length || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException(arguments.Length.ToString(Config.InternalNumberFormat) + " input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");

            return port[0];
        }
        public Signal Function(IEntity entity, IList<Signal> arguments)
        {
            Port port = entity.InstantiatePort(arguments);

            if(port.InputSignalCount != arguments.Count || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException(arguments.Count.ToString(Config.InternalNumberFormat) + " input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");

            return port[0];
        }

        public Signal Function(string symbol, Signal argument)
        {
            return Function(_library.LookupEntity(symbol, 1, 1, 0), argument);
        }
        public Signal Function(string symbol, Signal argument1, Signal argument2)
        {
            return Function(_library.LookupEntity(symbol, 2, 1, 0), argument1, argument2);
        }
        public Signal Function(string symbol, params Signal[] arguments)
        {
            if(arguments == null)
                throw new ArgumentNullException("arguments");

            return Function(_library.LookupEntity(symbol, arguments.Length, 1, 0), arguments);
        }
        public Signal Function(string symbol, IList<Signal> arguments)
        {
            if(arguments == null)
                throw new ArgumentNullException("arguments");

            return Function(_library.LookupEntity(symbol, arguments.Count, 1, 0), arguments);
        }

        public Signal Function(string symbol, InfixNotation notation, Signal argument)
        {
            return Function(_library.LookupEntity(symbol, notation, 1, 1, 0), argument);
        }
        public Signal Function(string symbol, InfixNotation notation, Signal argument1, Signal argument2)
        {
            return Function(_library.LookupEntity(symbol, notation, 2, 1, 0), argument1, argument2);
        }
        public Signal Function(string symbol, InfixNotation notation, params Signal[] arguments)
        {
            if(arguments == null)
                throw new ArgumentNullException("arguments");

            return Function(_library.LookupEntity(symbol, notation, arguments.Length, 1, 0), arguments);
        }
        public Signal Function(string symbol, InfixNotation notation, IList<Signal> arguments)
        {
            if(arguments == null)
                throw new ArgumentNullException("arguments");

            return Function(_library.LookupEntity(symbol, notation, arguments.Count, 1, 0), arguments);
        }

        public Signal Function(MathIdentifier entityId, Signal argument)
        {
            return Function(_library.LookupEntity(entityId), argument);
        }
        public Signal Function(MathIdentifier entityId, Signal argument1, Signal argument2)
        {
            return Function(_library.LookupEntity(entityId), argument1, argument2);
        }
        public Signal Function(MathIdentifier entityId, params Signal[] arguments)
        {
            return Function(_library.LookupEntity(entityId), arguments);
        }
        public Signal Function(MathIdentifier entityId, IList<Signal> arguments)
        {
            return Function(_library.LookupEntity(entityId), arguments);
        }
        #endregion

        #region Building Multiple-Value Functions
        public ReadOnlySignalSet Functions(IEntity entity, Signal argument)
        {
            Port port = entity.InstantiatePort(argument);

            if(port.InputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("1 input", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(IEntity entity, Signal argument1, Signal argument2)
        {
            Port port = entity.InstantiatePort(argument1, argument2);

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("2 input", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(IEntity entity, params Signal[] arguments)
        {
            Port port = entity.InstantiatePort(arguments);

            if(port.InputSignalCount != arguments.Length || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException(arguments.Length.ToString(Config.InternalNumberFormat) + " input", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input");

            return port.OutputSignals;
        }
        public ReadOnlySignalSet Functions(IEntity entity, IList<Signal> arguments)
        {
            Port port = entity.InstantiatePort(arguments);

            if(port.InputSignalCount != arguments.Count || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException(arguments.Count.ToString(Config.InternalNumberFormat) + " input", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input");

            return port.OutputSignals;
        }

        public ReadOnlySignalSet Functions(string symbol, Signal argument)
        {
            return Functions(_library.LookupEntity(symbol, 1, 1, 0), argument);
        }
        public ReadOnlySignalSet Functions(string symbol, Signal argument1, Signal argument2)
        {
            return Functions(_library.LookupEntity(symbol, 2, 1, 0), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(string symbol, params Signal[] arguments)
        {
            return Functions(_library.LookupEntity(symbol, arguments.Length, 1, 0), arguments);
        }
        public ReadOnlySignalSet Functions(string symbol, IList<Signal> arguments)
        {
            return Functions(_library.LookupEntity(symbol, arguments.Count, 1, 0), arguments);
        }

        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument)
        {
            return Functions(_library.LookupEntity(symbol, notation, 1, 1, 0), argument);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument1, Signal argument2)
        {
            return Functions(_library.LookupEntity(symbol, notation, 2, 1, 0), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, params Signal[] arguments)
        {
            return Functions(_library.LookupEntity(symbol, notation, arguments.Length, 1, 0), arguments);
        }
        public ReadOnlySignalSet Functions(string symbol, InfixNotation notation, IList<Signal> arguments)
        {
            return Functions(_library.LookupEntity(symbol, notation, arguments.Count, 1, 0), arguments);
        }

        public ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument)
        {
            return Functions(_library.LookupEntity(entityId), argument);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument1, Signal argument2)
        {
            return Functions(_library.LookupEntity(entityId), argument1, argument2);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, params Signal[] arguments)
        {
            return Functions(_library.LookupEntity(entityId), arguments);
        }
        public ReadOnlySignalSet Functions(MathIdentifier entityId, IList<Signal> arguments)
        {
            return Functions(_library.LookupEntity(entityId), arguments);
        }
        #endregion

        #region Basic Encapsulation Multiplexing
        public Signal EncapsulateAsVector(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsVector(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsList(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsList(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsSet(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsSet(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsScalar(params Signal[] inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        public Signal EncapsulateAsScalar(IList<Signal> inner)
        {
            throw new NotImplementedException(MathNet.Symbolics.Properties.Resources.ex_NotImplementedYet);
        }
        #endregion

        #region Basic Signal Manipulation
        public Port MapSignals(Signal source, Signal target)
        {
            Port port = _library.LookupEntity(new MathIdentifier("Transport", "Std")).InstantiatePort(new Signal[] { source }, new Signal[] { target });

            if(port.InputSignalCount != 1 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("1 input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");
            return port;
        }
        public Port MapSignalsSynchronized(Signal source, Signal target, Signal clock)
        {
            Port port = _library.LookupEntity(new MathIdentifier("Sync", "Std")).InstantiatePort(new Signal[] { source, clock }, new Signal[] { target });

            if(port.InputSignalCount != 2 || port.OutputSignalCount != 1)
                throw new MathNet.Symbolics.Exceptions.EntitySignalCountUnexpectedException("2 input and 1 output", port.InputSignalCount.ToString(Config.InternalNumberFormat) + " input and " + port.OutputSignalCount.ToString(Config.InternalNumberFormat) + " output");

            return port;
        }
        public Signal Synchronize(Signal signal, Signal clock)
        {
            return Function(new MathIdentifier("Sync", "Std"), signal, clock);
        }
        #endregion
    }
}
