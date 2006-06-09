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
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Events;

namespace MathNet.Symbolics.Backend.Simulation
{
    public enum SchedulerPhase
    {
        Idle,
        ProcessExecution,
        SignalAssignment
    }

    public class Scheduler : IContextSensitive
    {
        private List<SignalEventItem> _eventSignals;
        private Stack<SignalEventItem> _deltaEvents;
        private EventTimeline _timeQueue;
        private SchedulerPhase _phase = SchedulerPhase.Idle;
        private readonly Context _context;
        private TimeSpan _simulationTime;

        public event EventHandler<SimulationTimeEventArgs> SimulationTimeProgress;

        public Scheduler(Context context)
        {
            _context = context;
            _eventSignals = new List<SignalEventItem>(128);
            _deltaEvents = new Stack<SignalEventItem>(128);
            _timeQueue = new EventTimeline();
            _simulationTime = TimeSpan.Zero;
        }

        public SchedulerPhase CurrentPhase
        {
            get { return _phase; }
        }

        public Context Context
        {
            get { return _context; }
        }

        public TimeSpan SimulationTime
        {
            get { return _simulationTime; }
        }

        public void ResetSimulationTime()
        {
            _simulationTime = TimeSpan.Zero;
        }

        /// <returns>True if there were any events available.</returns>
        public bool SimulateInstant()
        {
            TimeSpan instant = TimeSpan.FromTicks(1);
            return instant <= SimulateFor(instant);
        }
        /// <param name="timespan">The time (in simulation time space) to simulate.</param>
        /// <returns>
        /// Actually simulated time.
        /// If this time is smaller than <see cref="timespan"/>, there were no more events available.
        /// </returns>
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
        /// <param name="cycles">The number of cycles to simulate.</param>
        /// <returns>Simulated time.</returns>
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
            foreach(SignalEventItem item in _eventSignals)
                item.SetEventFlag(true);
            foreach(SignalEventItem item in _eventSignals)
                item.NotifyNewValue();
            foreach(SignalEventItem item in _eventSignals)
                item.SetEventFlag(false);
            _eventSignals.Clear();
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
                if(_timeQueue.TryNextEventTime(out timespan))
                {
                    OnSimulationTimeProgress(timespan);
                    simulatedTime += timespan;
                }
                else
                {
                    _phase = SchedulerPhase.Idle;
                    return TimeSpan.MinValue;
                }
            }

            while(_deltaEvents.Count > 0)
            {
                SignalEventItem item = _deltaEvents.Pop();
                if(item.SetValue(item.NewValue))
                    _eventSignals.Add(item);
            }

            _phase = SchedulerPhase.Idle;
            return simulatedTime;
        }

        public void ScheduleDeltaEvent(SignalEventItem item)
        {
            _deltaEvents.Push(item);
        }

        public void ScheduleDelayedEvent(TimedSignalEventItem item)
        {
            if(item.TimeSpan < TimeSpan.Zero)
                return;
            if(item.TimeSpan == TimeSpan.Zero)
                ScheduleDeltaEvent(item.Item);
            else
            {
                item.Item.Signal.EventQueue.InsertDelayedEvent(item);
                _timeQueue.InsertTime(item.TimeSpan);
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
