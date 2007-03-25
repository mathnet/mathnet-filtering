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

using MathNet.Symbolics.Events;

namespace MathNet.Symbolics.Simulation
{
    public class SimulationMediator : ISimulationMediator
    {
        private Dictionary<Guid, IScheduler> _schedulerStack;

        public SimulationMediator()
        {
            _schedulerStack = new Dictionary<Guid, IScheduler>();
            MathContext.ContextExpired += MathContext_ContextExpired;
        }

        public void ScheduleDeltaEvent(ISchedulable subject, IValueStructure value)
        {
            GetCurrentScheduler().ScheduleDeltaEvent(subject, value);
        }

        public void ScheduleDelayedEvent(ISchedulable subject, IValueStructure value, TimeSpan delay)
        {
            GetCurrentScheduler().ScheduleDelayedEvent(subject, value, delay);
        }

        public bool SimulateInstant()
        {
            return GetCurrentScheduler().SimulateInstant();
        }
        public TimeSpan SimulateFor(TimeSpan timespan)
        {
            return GetCurrentScheduler().SimulateFor(timespan);
        }
        public TimeSpan SimulateFor(int cycles)
        {
            return GetCurrentScheduler().SimulateFor(cycles);
        }

        public IScheduler GetCurrentScheduler()
        {
            MathContext ctx = MathContext.Current;
            Guid currentId = ctx.InstanceId;

            lock(_schedulerStack)
            {
                IScheduler scheduler;
                if(_schedulerStack.TryGetValue(ctx.InstanceId, out scheduler))
                    return scheduler;
                while(ctx.HasParent)
                {
                    ctx = ctx.ParentContext;
                    if(_schedulerStack.TryGetValue(ctx.InstanceId, out scheduler))
                        return scheduler;
                }
                scheduler = new ImmediateScheduler();
                _schedulerStack.Add(currentId, scheduler);
                return scheduler;
            }
        }

        public void CreateSimulationApartement(IScheduler scheduler)
        {
            MathContext ctx = MathContext.Current;
            lock(_schedulerStack) // don't interfer with GetCurrentScheduler
            {
                _schedulerStack.Add(ctx.InstanceId, scheduler);
            }
        }

        void MathContext_ContextExpired(object sender, MathContextEventArgs e)
        {
            if(_schedulerStack.ContainsKey(e.InstanceId))
                _schedulerStack.Remove(e.InstanceId);
        }

        
    }
}
