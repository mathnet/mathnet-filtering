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
    /// <summary>
    /// A system builder is a concrete builder (writer) conforming with the builder design pattern,
    /// building an arbitrary representation of a <see cref="MathSystem">Math System</see>.
    /// System builders are controlled by a director (reader), eg. <see cref="SystemReader"/>.
    /// </summary>
    public interface ISystemBuilder
    {
        void BeginBuildSystem(int inputSignalCount, int outputSignalCount, int busCount);

        Guid BuildSignal(string label, bool hold, bool isSource);

        Guid BuildBus(string label);

        Guid BuildPort(MathIdentifier entityId, InstanceIdSet inputSignals, InstanceIdSet outputSignals, InstanceIdSet buses);

        void AppendSignalValue(Guid iid, ICustomDataPack<IValueStructure> value);
        void AppendSignalProperty(Guid iid, ICustomDataPack<IProperty> property);
        void AppendSignalConstraint(Guid iid, ICustomDataPack<IProperty> constraint);

        void AppendSystemInputSignal(Guid iid);
        void AppendSystemOutputSignal(Guid iid);
        void AppendSystemNamedSignal(Guid iid, string name);
        void AppendSystemNamedBus(Guid iid, string name);

        void EndBuildSystem();
    }
}
