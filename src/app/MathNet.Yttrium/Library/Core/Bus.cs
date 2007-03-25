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
using System.Xml;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Simulation;
using MathNet.Symbolics.Mediator;

namespace MathNet.Symbolics.Core
{
    public class Bus : MathNet.Symbolics.Bus, ISchedulable, IBus_BuilderAdapter
    {
        internal Bus()
            : base()
        {
            Service<IMediator>.Instance.NotifyNewBusCreated(this);
        }

        internal Bus(IValueStructure value)
            : base(value)
        {
            Service<IMediator>.Instance.NotifyNewBusCreated(this);
        }

        #region Value & Scheduling
        /// <summary>
        /// Request the scheduler to set a new value to this signal in the next delta-timestep.
        /// </summary>
        /// <remarks>The value is not set immediately. To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least one cycle or a time instant, by calling
        /// <see cref="Scheduler.SimulateInstant"/> or <see cref="Scheduler.SimulateFor"/>.</remarks>
        public override void PostNewValue(IValueStructure newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Request the scheduler to set a new value to this signal with a specified simulation-time delay.
        /// </summary>
        /// <remarks>The value is not set immediately, To propagate it to the <see cref="Value"/> property
        /// you need to simulate the model for at least at least the specified delay, by calling
        /// <see cref="Scheduler.SimulateFor"/>.</remarks>
        /// <param name="delay">The simulation-time delay.</param>
        public override void PostNewValue(IValueStructure newValue, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        // TODO: Implement Value Handling
        #endregion

        #region System Builder
        Guid IBus_BuilderAdapter.AcceptSystemBuilderBefore(ISystemBuilder builder)
        {
            return builder.BuildBus(Label);
        }
        void IBus_BuilderAdapter.AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
        }
        #endregion

        //#region Instance Equality
        ///// <remarks>Two buses are equal only if they are the same instance.</remarks>
        //public bool Equals(Bus other)
        //{
        //    return other != null && _iid.Equals(other.InstanceId);
        //}
        ///// <remarks>Two buses are equal only if they are the same instance.</remarks>
        //public override bool Equals(object obj)
        //{
        //    Bus other = obj as Bus;
        //    if(other == null)
        //        return false;
        //    else
        //        return _iid.Equals(other._iid);
        //}
        //public override int GetHashCode()
        //{
        //    return _iid.GetHashCode();
        //}
        //#endregion

        #region ISchedulable Members

        bool ISchedulable.HasEvent
        {
            get { return HasEvent; }
            set { base.SetHasEvent(value); }
        }

        IValueStructure ISchedulable.CurrentValue
        {
            get { return Value; }
            set { base.SetPresentValue(value); }
        }

        void ISchedulable.NotifyOutputsNewValue()
        {
            OnValueChanged();
        }
        #endregion
    }
}