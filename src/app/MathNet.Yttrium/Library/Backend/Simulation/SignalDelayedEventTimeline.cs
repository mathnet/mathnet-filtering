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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Events;

namespace MathNet.Symbolics.Backend.Simulation
{
    public class SignalDelayedEventTimeline
    {
        private LinkedList<TimedSignalEventItem> _timeline;
        private Scheduler _scheduler;

        public SignalDelayedEventTimeline(Scheduler scheduler)
        {
            _timeline = new LinkedList<TimedSignalEventItem>();
            _scheduler = scheduler;
            _scheduler.SimulationTimeProgress += scheduler_SimulationTimeProgress;
        }

        /*private void RemoveAllAfter(LinkedListNode<TimedSignalEventItem> node)
        {
            while(node.Next != null)
                timeline.Remove(node.Next);
        }*/
        private void RemoveAllAfterIncluding(LinkedListNode<TimedSignalEventItem> node)
        {
            while(node.Next != null)
                _timeline.Remove(node.Next);
            _timeline.Remove(node);
        }

        public void InsertDelayedEvent(TimedSignalEventItem item)
        {
            if(_timeline.Count == 0)
            {
                _timeline.AddFirst(item);
                return;
            }

            TimeSpan delay = item.TimeSpan;
            TimeSpan d = TimeSpan.Zero;
            LinkedListNode<TimedSignalEventItem> node = _timeline.First;

            while(true)
            {
                if(d + node.Value.TimeSpan == delay) //already there...
                {
                    item.TimeSpan = node.Value.TimeSpan;
                    RemoveAllAfterIncluding(node);
                    _timeline.AddLast(item);
                    return;
                }

                if(d + node.Value.TimeSpan > delay) //later events available
                {
                    item.TimeSpan = delay - d;
                    RemoveAllAfterIncluding(node);
                    _timeline.AddLast(item);
                    return;
                }

                if(node.Next == null) //last
                {
                    item.TimeSpan = delay - d - node.Value.TimeSpan;
                    _timeline.AddLast(item);
                    return;
                }

                d += node.Value.TimeSpan;
                node = node.Next;
            }
        }

        private void scheduler_SimulationTimeProgress(object sender, SimulationTimeEventArgs e)
        {
            TimeSpan timespan = e.TimeSpan;
            while(_timeline.First != null && timespan >= TimeSpan.Zero)
            {
                TimedSignalEventItem item = _timeline.First.Value;
                item.TimeSpan -= timespan;
                timespan = -item.TimeSpan;
                if(item.TimeSpan <= TimeSpan.Zero)
                {
                    _scheduler.ScheduleDeltaEvent(item.Item);
                    _timeline.RemoveFirst();
                }
            }
        }
    }
}
