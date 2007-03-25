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

namespace MathNet.Symbolics.Simulation
{
    public class ScheduleStore
    {
        private Dictionary<Guid, LinkedList<SchedulerEventItem>> _delayedEvents;

        public ScheduleStore()
        {
            _delayedEvents = new Dictionary<Guid, LinkedList<SchedulerEventItem>>();
        }

        private void RemoveAllAfterIncluding(LinkedList<SchedulerEventItem> timeline, LinkedListNode<SchedulerEventItem> node)
        {
            while(node.Next != null)
                timeline.Remove(node.Next);
            timeline.Remove(node);
        }

        public void ScheduleDelayedEvent(ISchedulable subject, IValueStructure value, TimeSpan delay)
        {
            LinkedList<SchedulerEventItem> timeline;
            if(!_delayedEvents.TryGetValue(subject.InstanceId, out timeline))
            {
                timeline = new LinkedList<SchedulerEventItem>();
                _delayedEvents.Add(subject.InstanceId, timeline);
            }

            if(timeline.Count == 0)
            {
                timeline.AddFirst(new SchedulerEventItem(subject, value, delay));
                return;
            }

            TimeSpan d = TimeSpan.Zero;
            LinkedListNode<SchedulerEventItem> node = timeline.First;

            while(true)
            {
                if(d + node.Value.TimeSpan == delay) //already there...
                {
                    TimeSpan relativeDelay = node.Value.TimeSpan;
                    RemoveAllAfterIncluding(timeline, node);
                    timeline.AddLast(new SchedulerEventItem(subject, value, relativeDelay));
                    return;
                }

                if(d + node.Value.TimeSpan > delay) //later events available
                {
                    TimeSpan relativeDelay = delay - d;
                    RemoveAllAfterIncluding(timeline, node);
                    timeline.AddLast(new SchedulerEventItem(subject, value, relativeDelay));
                    return;
                }

                if(node.Next == null) //last
                {
                    TimeSpan relativeDelay = delay - d - node.Value.TimeSpan;
                    timeline.AddLast(new SchedulerEventItem(subject, value, relativeDelay));
                    return;
                }

                d += node.Value.TimeSpan;
                node = node.Next;
            }
        }

        public void ProgressTime(TimeSpan timespan, IScheduler scheduler)
        {
            foreach(KeyValuePair<Guid, LinkedList<SchedulerEventItem>> schedulable in _delayedEvents)
            {
                TimeSpan span = timespan;
                LinkedList<SchedulerEventItem> timeline = schedulable.Value;
                while(timeline.First != null && span >= TimeSpan.Zero)
                {
                    SchedulerEventItem item = timeline.First.Value;
                    item.TimeSpan -= span;
                    span = -item.TimeSpan;
                    if(item.TimeSpan <= TimeSpan.Zero)
                    {
                        scheduler.ScheduleDeltaEvent(item.Subject, item.Value);
                        timeline.RemoveFirst();
                    }
                }
            }
        }
    }
}
