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
    public class ImmediateScheduler : IScheduler
    {
        private List<ISchedulable> _schedulablesWithEvent;
        private Stack<SchedulerEventItem> _deltaEvents;
        private Timeline _timeline;
        private ScheduleStore _schedule;
        private SchedulerPhase _phase = SchedulerPhase.Idle;
        private TimeSpan _simulationTime;

        public event EventHandler<SimulationTimeEventArgs> SimulationTimeProgress;

        public ImmediateScheduler()
        {
            _schedulablesWithEvent = new List<ISchedulable>(128);
            _deltaEvents = new Stack<SchedulerEventItem>(128);
            _timeline = new Timeline();
            _schedule = new ScheduleStore();
            _simulationTime = TimeSpan.Zero;
        }
        
        public TimeSpan SimulationTime
        {
            get { return _simulationTime; }
        }

        public void ResetSimulationTime()
        {
            _simulationTime = TimeSpan.Zero;
        }

        public bool SimulateInstant()
        {
            TimeSpan instant = TimeSpan.FromTicks(1);
            return instant <= SimulateFor(instant);
        }

        public TimeSpan SimulateFor(TimeSpan timespan)
        {
            TimeSpan simulatedTimeTotal = TimeSpan.Zero;
            TimeSpan simulatedTimePhase = TimeSpan.Zero;

            while(simulatedTimeTotal < timespan)
            {
                simulatedTimePhase = RunSignalAssignmentPhase(true);

                if(simulatedTimePhase == TimeSpan.MinValue)
                    return simulatedTimeTotal;

                RunProcessExecutionPhase();
                simulatedTimeTotal += simulatedTimePhase;
            }
            while(_deltaEvents.Count > 0)
            {
                RunSignalAssignmentPhase(false);
                RunProcessExecutionPhase();
            }

            return simulatedTimeTotal;
        }

        public TimeSpan SimulateFor(int cycles)
        {
            TimeSpan simulatedTimeTotal = TimeSpan.Zero;
            TimeSpan simulatedTimePhase = TimeSpan.Zero;

            for(int i = 0; i < cycles; i++)
            {
                simulatedTimePhase = RunSignalAssignmentPhase(true);

                if(simulatedTimePhase == TimeSpan.MinValue)
                    return simulatedTimeTotal;

                RunProcessExecutionPhase();
                simulatedTimeTotal += simulatedTimePhase;
            }

            return simulatedTimeTotal;
        }

        private void RunProcessExecutionPhase()
        {
            _phase = SchedulerPhase.ProcessExecution;
            foreach(ISchedulable item in _schedulablesWithEvent)
                item.HasEvent = true;
            foreach(ISchedulable item in _schedulablesWithEvent)
                item.NotifyOutputsNewValue();
            foreach(ISchedulable item in _schedulablesWithEvent)
                item.HasEvent = false;
            _schedulablesWithEvent.Clear();
            _phase = SchedulerPhase.Idle;
        }

        /// <returns>
        /// The simulated time.
        /// TimeSpan.Zero if no time progress was achieved due to delta events.
        /// TimeSpan.MinValue if no events were available.
        /// </returns>
        private TimeSpan RunSignalAssignmentPhase(bool progressTime)
        {
            _phase = SchedulerPhase.SignalAssignment;
            TimeSpan simulatedTime = TimeSpan.Zero;

            while(progressTime && _deltaEvents.Count == 0)
            {
                TimeSpan timespan;
                if(_timeline.TryNextEventTime(out timespan))
                {
                    _schedule.ProgressTime(timespan, this);
                    OnSimulationTimeProgress(timespan);
                    simulatedTime += timespan;
                }
                else
                {
                    _phase = SchedulerPhase.Idle;
                    return TimeSpan.MinValue;
                    // TODO: check, why MinValue?
                }
            }

            while(_deltaEvents.Count > 0)
            {
                SchedulerEventItem item = _deltaEvents.Pop();
                ISchedulable subject = item.Subject;
                if(subject.CurrentValue == null || !subject.CurrentValue.Equals(item.Value))
                {
                    item.Subject.CurrentValue = item.Value;
                    _schedulablesWithEvent.Add(item.Subject);
                }
            }

            _phase = SchedulerPhase.Idle;
            return simulatedTime;
        }


        public void ScheduleDeltaEvent(ISchedulable subject, IValueStructure value)
        {
            _deltaEvents.Push(new SchedulerEventItem(subject, value, TimeSpan.Zero));
        }

        public void ScheduleDelayedEvent(ISchedulable subject, IValueStructure value, TimeSpan delay)
        {
            if(delay < TimeSpan.Zero)
                return;
            if(delay == TimeSpan.Zero)
                ScheduleDeltaEvent(subject, value);
            else
            {
                _schedule.ScheduleDelayedEvent(subject, value, delay);
                _timeline.InsertTime(delay);
            }
        }

        private void OnSimulationTimeProgress(TimeSpan timespan)
        {
            EventHandler<SimulationTimeEventArgs> handler = SimulationTimeProgress;
            if(handler != null)
                handler(this, new SimulationTimeEventArgs(timespan));
        }
    }
}
