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

using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Events;

namespace MathNet.Symbolics
{
    /// <summary>
    /// Represents an Yttrium Port. Ports connect signals by operations defined
    /// in interchangeable architectures. 
    /// </summary>
    public abstract class Port : Node, IEquatable<Port>
    {
        protected Port()
            : base()
        {
        }

        #region Event Aspects
        public static readonly NodeEvent ArchitectureChangedEvent
            = NodeEvent.Register(new MathIdentifier("ArchitectureChanged", "Core"),
                                 typeof(EventHandler<ValueChangedEventArgs<IArchitecture, Port>>),
                                 typeof(Port));
        public event EventHandler<ValueChangedEventArgs<IArchitecture, Port>> ArchitectureChanged
        {
            add { AddHandler(ArchitectureChangedEvent, value); }
            remove { RemoveHandler(ArchitectureChangedEvent, value); }
        }
        protected virtual void OnArchitectureChanged(IArchitecture oldArchitecture, IArchitecture newArchitecture)
        {
            RaiseEvent(ArchitectureChangedEvent, new ValueChangedEventArgs<IArchitecture, Port>(this, oldArchitecture, newArchitecture));
        }

        public static readonly NodeEvent PortInputTreeChangedEvent
            = NodeEvent.Register(new MathIdentifier("PortInputTreeChanged", "Core"),
                                 typeof(EventHandler<PortIndexEventArgs>),
                                 typeof(Port));
        public event EventHandler<PortIndexEventArgs> PortInputTreeChanged
        {
            add { AddHandler(PortInputTreeChangedEvent, value); }
            remove { RemoveHandler(PortInputTreeChangedEvent, value); }
        }
        protected virtual void OnInputTreeChanged(int index)
        {
            RaiseEvent(PortInputTreeChangedEvent, new PortIndexEventArgs(this, index));
        }

        public static readonly NodeEvent PortBusChangedEvent
            = NodeEvent.Register(new MathIdentifier("PortBusChanged", "Core"),
                                 typeof(EventHandler<PortIndexEventArgs>),
                                 typeof(Port));
        public event EventHandler<PortIndexEventArgs> PortBusChanged
        {
            add { AddHandler(PortBusChangedEvent, value); }
            remove { RemoveHandler(PortBusChangedEvent, value); }
        }
        protected virtual void OnBusChanged(int index)
        {
            RaiseEvent(PortBusChangedEvent, new PortIndexEventArgs(this, index));
        }
        #endregion

        #region Graph Structure
        public abstract int InputSignalCount { get; }
        public abstract int OutputSignalCount { get; }
        public abstract int BusCount { get; }
        public abstract ReadOnlySignalSet InputSignals { get; }
        public abstract ReadOnlySignalSet OutputSignals { get; }
        public abstract ReadOnlyBusSet Buses { get; }
        public abstract Signal this[int outputIndex] { get; }

        public abstract int IndexOfOutputSignal(Signal signal);

        public abstract bool DependsOn(Signal signal);
        public abstract bool DependsOn(Port port);
        public abstract bool DependsOn(MathIdentifier portEntity);
        #endregion

        /// <summary>
        /// The entity defining this port's interface and (indirectly) its operation.
        /// </summary>
        public abstract IEntity Entity { get; }

        /// <summary>
        /// The architecture currently attached to this port. Architectures are
        /// interchangeable as long as they implement this port's entity.
        /// </summary>
        public abstract IArchitecture CurrentArchitecture { get; }

        public abstract bool HasArchitectureLink { get; }

        /// <summary>
        /// Checks if the current architecture still matches the bound signals
        /// and tries to find a matching architecture if not.
        /// </summary>
        /// <returns>True if there's a matching architecture linked after the call.</returns>
        public abstract bool EnsureArchitectureLink();

        public abstract void RemoveLinkedArchitecture();

        public abstract bool IsCompletelyConnected { get; }

        public abstract void AddInputSignalBinding(int index, Signal signal);
        public abstract void RemoveInputSignalBinding(int index);
        public abstract void ReplaceInputSignalBinding(int index, Signal replacement);
        public abstract void AddOutputSignalBinding(int index, Signal signal);
        public abstract void RemoveOutputSignalBinding(int index);
        public abstract void ReplaceOutputSignalBinding(int index, Signal replacement);
        public abstract void AddBusBinding(int index, Bus bus);
        public abstract void RemoveBusBinding(int index);
        public abstract void ReplaceBusBinding(int index, Bus replacement);
        public abstract void RemoveAllBindings();
        public abstract void BindInputSignals(IEnumerable<Signal> inputSignals);
        public abstract void BindBuses(IEnumerable<Bus> buses);

        public abstract Port CloneWithNewInputs(IList<Signal> newInputs);

        public bool Equals(Port other)
        {
            return base.Equals((Node)other);
        }
    }
}
