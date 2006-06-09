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
using System.Collections.Generic;

namespace MathNet.Symbolics.Backend.Simulation
{
    public class EventTimeline
    {
        private LinkedList<TimeSpan> _timeline;

        public EventTimeline()
        {
            _timeline = new LinkedList<TimeSpan>();
        }

        public void InsertTime(TimeSpan delay)
        {
            if(_timeline.Count == 0)
            {
                _timeline.AddFirst(delay);
                return;
            }

            TimeSpan d = TimeSpan.Zero;
            LinkedListNode<TimeSpan> node = _timeline.First;

            while(true)
            {
                if(d + node.Value == delay) //already there...
                    return;

                if(d + node.Value > delay) //later events available
                {
                    _timeline.AddAfter(node, d + node.Value - delay);
                    node.Value = delay - d;
                    return;
                }

                if(node.Next == null) //last
                {
                    _timeline.AddAfter(node, delay - d - node.Value);
                    return;
                }

                d += node.Value;
                node = node.Next;
            }
        }

        /// <returns>True if there were future event available and the simulation time progressed.</returns>
        public bool TryNextEventTime(out TimeSpan timespan)
        {
            if(_timeline.Count == 0)
            {
                timespan = TimeSpan.Zero;
                return false;
            }
            else
            {
                timespan = _timeline.First.Value;
                _timeline.RemoveFirst();
                return true;
            }
        }
    }
}
