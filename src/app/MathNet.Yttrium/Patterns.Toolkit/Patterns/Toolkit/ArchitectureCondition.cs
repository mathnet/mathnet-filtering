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

namespace MathNet.Symbolics.Patterns.Toolkit
{
    public class ArchitectureCondition : Condition
    {
        private Predicate<IArchitecture> _match;

        public ArchitectureCondition(Predicate<IArchitecture> match)
        {
            _match = match;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            if(port == null)
                return false;

            return port.HasArchitectureLink && _match(port.CurrentArchitecture);
        }

        public override bool Equals(Condition other)
        {
            ArchitectureCondition ot = other as ArchitectureCondition;
            if(ot == null)
                return false;
            return _match.Equals(ot._match);
        }
    }
}
