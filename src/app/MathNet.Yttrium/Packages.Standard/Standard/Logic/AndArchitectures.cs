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

using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Logic
{
    public class AndArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("And", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public AndArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Logic"),
                LogicValueCategory.IsLogicValueMember,
                delegate(Port port) { return new ProcessBase[] { LogicValue.CreateAndProcess(0, port.InputSignalCount - 1, 0) }; });
        }
    }
}
