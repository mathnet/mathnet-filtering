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
using System.Runtime.Serialization;
using System.Security.Permissions;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Exceptions
{
    [Serializable]
    public class EntityNotAvailableException : MathNetSymbolicsException
    {
        Entity entity;

        public EntityNotAvailableException(Entity entity)
            : base(string.Format(MathNet.Symbolics.Properties.Resources.ex_NotAvailable_Entity, (entity == null ? "N/A" : entity.ToString())))
        {
            this.entity = entity;
        }

        protected EntityNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            entity = (Entity)info.GetValue("entity", typeof(Entity));
        }

        public Entity Entity
        {
            get { return entity; }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("entity", entity, typeof(Entity));
            base.GetObjectData(info, context);
        }
    }
}
