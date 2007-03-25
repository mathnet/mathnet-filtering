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
    public class EntityCondition : Condition
    {
        private MathIdentifier _entityId;

        public EntityCondition(MathIdentifier entityId)
        {
            _entityId = entityId;
        }
        [Obsolete("Use MathIdentifiers directly",false)]
        public EntityCondition(string entityLabel, string entityDomain)
        {
            _entityId = new MathIdentifier(entityLabel, entityDomain);
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            if(port == null)
                return false;
            return _entityId.Equals(port.Entity.EntityId);
        }

        public override bool Equals(Condition other)
        {
            EntityCondition ot = other as EntityCondition;
            if(ot == null)
                return false;
            return _entityId.Equals(ot._entityId);
        }
    }
}
