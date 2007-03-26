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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.PetriNet
{
    public class PetriNetPackageManager : IPackageManager
    {
        public PetriNetPackageManager()
        {
        }

        public string Domain
        {
            get { return "PetriNet"; }
        }

        public void Register(ILibrary library)
        {
            RegisterEntities(library);
            RegisterArchitectures(library);
            RegisterTheorems(library);
            RegisterStructures(library);
        }

        public void RegisterEntities(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            library.AddEntity(new ArbitraryGenericEntity("|", "Transition", "PetriNet"));
        }

        public void RegisterArchitectures(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            library.AddArchitecture(new TransitionArchitectures());
        }

        public void RegisterTheorems(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            TransitionArchitectures.RegisterTheorems(library);
        }

        public void RegisterStructures(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            TokenValue.Register(library);
        }
    }
}
