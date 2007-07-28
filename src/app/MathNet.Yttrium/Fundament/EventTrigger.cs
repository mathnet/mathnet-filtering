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
    public enum EventTriggerAction
    {
        /// <summary>Do nothing</summary>
        Nothing = 0,
        /// <summary>Clear the property or flag</summary>
        Clear,
        /// <summary>Disable the flag</summary>
        Disable,
        /// <summary>Enable the flag</summary>
        Enable,
        /// <summary>Mark the property or flag as dirty</summary>
        Dirty,
        /// <summary>Reevaluate the property or flag</summary>
        Reevaluate
    }

    public class EventTrigger<TIdentifier, TEvent>
        where TIdentifier : IEquatable<TIdentifier>, IComparable<TIdentifier>
        where TEvent : EventAspect<TIdentifier, TEvent>
    {
        private TEvent[] _events;
        private EventTriggerAction _action;

        public EventTrigger(EventTriggerAction action, params TEvent[] events)
        {
            _action = action;
            _events = events;
        }

        public EventTriggerAction Action
        {
            get { return _action; }
        }

        public IEnumerable<TEvent> Events
        {
            get { return _events; }
        }
    }
}
