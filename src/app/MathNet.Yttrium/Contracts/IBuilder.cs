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

namespace MathNet.Symbolics
{
    public interface IBuilder
    {
        #region Building Single-Value Functions
        Signal Function(string symbol, InfixNotation notation, params Signal[] arguments);
        Signal Function(IEntity entity, Signal argument);
        Signal Function(IEntity entity, Signal argument1, Signal argument2);
        Signal Function(MathIdentifier entityId, IList<Signal> arguments);
        Signal Function(string symbol, InfixNotation notation, IList<Signal> arguments);
        Signal Function(MathIdentifier entityId, Signal argument);
        Signal Function(MathIdentifier entityId, params Signal[] arguments);
        Signal Function(MathIdentifier entityId, Signal argument1, Signal argument2);
        Signal Function(string symbol, IList<Signal> arguments);
        Signal Function(string symbol, InfixNotation notation, Signal argument);
        Signal Function(string symbol, Signal argument1, Signal argument2);
        Signal Function(string symbol, params Signal[] arguments);
        Signal Function(string symbol, InfixNotation notation, Signal argument1, Signal argument2);
        Signal Function(IEntity entity, params Signal[] arguments);
        Signal Function(IEntity entity, IList<Signal> arguments);
        Signal Function(string symbol, Signal argument);
        #endregion

        #region Building Multiple-Value Functions
        ReadOnlySignalSet Functions(IEntity entity, IList<Signal> arguments);
        ReadOnlySignalSet Functions(string symbol, Signal argument);
        ReadOnlySignalSet Functions(string symbol, IList<Signal> arguments);
        ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument);
        ReadOnlySignalSet Functions(string symbol, Signal argument1, Signal argument2);
        ReadOnlySignalSet Functions(string symbol, params Signal[] arguments);
        ReadOnlySignalSet Functions(IEntity entity, params Signal[] arguments);
        ReadOnlySignalSet Functions(MathIdentifier entityId, params Signal[] arguments);
        ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument1, Signal argument2);
        ReadOnlySignalSet Functions(MathIdentifier entityId, Signal argument);
        ReadOnlySignalSet Functions(IEntity entity, Signal argument1, Signal argument2);
        ReadOnlySignalSet Functions(IEntity entity, Signal argument);
        ReadOnlySignalSet Functions(MathIdentifier entityId, IList<Signal> arguments);
        ReadOnlySignalSet Functions(string symbol, InfixNotation notation, Signal argument1, Signal argument2);
        ReadOnlySignalSet Functions(string symbol, InfixNotation notation, params Signal[] arguments);
        ReadOnlySignalSet Functions(string symbol, InfixNotation notation, IList<Signal> arguments);
        #endregion

        #region Basic Encapsulation Multiplex
        Signal EncapsulateAsList(IList<Signal> inner);
        Signal EncapsulateAsList(params Signal[] inner);
        Signal EncapsulateAsScalar(params Signal[] inner);
        Signal EncapsulateAsScalar(IList<Signal> inner);
        Signal EncapsulateAsSet(params Signal[] inner);
        Signal EncapsulateAsSet(IList<Signal> inner);
        Signal EncapsulateAsVector(IList<Signal> inner);
        Signal EncapsulateAsVector(params Signal[] inner);
        #endregion

        #region Basic Signal Manipulation
        Port MapSignals(Signal source, Signal target);
        Port MapSignalsSynchronized(Signal source, Signal target, Signal clock);
        Signal Synchronize(Signal signal, Signal clock);
        #endregion
    }
}
