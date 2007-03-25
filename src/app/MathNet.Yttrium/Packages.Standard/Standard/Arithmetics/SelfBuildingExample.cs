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

using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
{
    //TODO: remove this dummy (maybe add to documentation instead...)
    //[EntityImplementation("Example", "Std")]
    public class SelfBuildingExample : ArchitectureBase, IArchitectureFactory
    {
        public SelfBuildingExample() : base(new MathIdentifier("Example", "Std"), new MathIdentifier("Example", "Std"), false) { }
        private SelfBuildingExample(Port port)
            : base(new MathIdentifier("Example", "Std"), new MathIdentifier("Example", "Std"), false)
        {
            SetPort(port);
            // init processes here, or whatever you want to do ...
        }

        public override bool SupportsPort(Port port)
        {
            return false;
        }

        public IArchitecture InstantiateToPort(Port port)
        {
            return new SelfBuildingExample(port);
        }

        public override void UnregisterArchitecture()
        {
            // remove here your signal event handlers ...
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            // add here your signal event handlers again ...
        }
    }
}
