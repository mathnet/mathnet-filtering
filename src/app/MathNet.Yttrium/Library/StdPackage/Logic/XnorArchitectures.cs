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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Logic
{
    public class XnorArchitectures : GenericArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Xnor", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public XnorArchitectures()
            : base(_entityId)
        {
            AddArchitecture(EntityId.DerivePrefix("Logic"),
                LogicValueCategory.IsLogicValueMember,
                delegate(Port port) { return new Process[] { LogicValue.CreateXnorProcess(0, port.InputSignalCount - 1, 0) }; });
        }
    }
}
