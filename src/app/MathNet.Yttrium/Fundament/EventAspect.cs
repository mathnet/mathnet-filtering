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

namespace MathNet.Symbolics
{
    public interface IEventAspectHost<TFriendlyIdentifier, TEvent>
        where TFriendlyIdentifier : IEquatable<TFriendlyIdentifier>, IComparable<TFriendlyIdentifier>
        where TEvent : EventAspect<TFriendlyIdentifier, TEvent>
    {
        void RaiseEvent<TEventArgs>(TEvent eventId, TEventArgs e)
            where TEventArgs : EventArgs;
    }

    public abstract class EventAspect<TFriendlyIdentifier, TEvent>
        : Identifier<TFriendlyIdentifier>
        where TFriendlyIdentifier : IEquatable<TFriendlyIdentifier>, IComparable<TFriendlyIdentifier>
        where TEvent : EventAspect<TFriendlyIdentifier, TEvent>
    {
        private Type _handlerType;
        private Type _ownerType;

        public EventAspect(TFriendlyIdentifier friendly, Type handlerType, Type ownerType, IdentifierService<TFriendlyIdentifier> service)
            : base(friendly, service)
        {
            _handlerType = handlerType;
            _ownerType = ownerType;
        }

        public Type HandlerType
        {
            get { return _handlerType; }
        }

        public Type OwnerType
        {
            get { return _ownerType; }
        }

        public bool IsValidHandlerType(Type type)
        {
            return _handlerType.IsAssignableFrom(type);
        }
        public bool IsValidHandler(Delegate value)
        {
            return _handlerType.IsInstanceOfType(value);
            //return _handlerType.IsAssignableFrom(value.GetType());
        }
    }
}
