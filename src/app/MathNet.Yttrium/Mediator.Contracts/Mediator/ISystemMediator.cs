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

namespace MathNet.Symbolics.Mediator
{
    public interface ISystemMediator
    {
        void AttachObserver(ISystemObserver observer);
        void DetachObserver(ISystemObserver observer);

        void SubscribeSystem(IMathSystem system);
        void UnsubscribeSystem(IMathSystem system);

        void NotifySignalAdded(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> SignalAdded;
        void NotifySignalRemoved(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> SignalRemoved;
        void NotifySignalMoved(Signal signal, int indexBefore, int indexAfter);
        event EventHandler<SignalIndexChangedEventArgs> SignalMoved;

        void NotifyBusAdded(Bus bus, int index);
        event EventHandler<BusIndexEventArgs> BusAdded;
        void NotifyBusRemoved(Bus bus, int index);
        event EventHandler<BusIndexEventArgs> BusRemoved;
        void NotifyBusMoved(Bus bus, int indexBefore, int indexAfter);
        event EventHandler<BusIndexChangedEventArgs> BusMoved;

        void NotifyPortAdded(Port port, int index);
        event EventHandler<PortIndexEventArgs> PortAdded;
        void NotifyPortRemoved(Port port, int index);
        event EventHandler<PortIndexEventArgs> PortRemoved;
        void NotifyPortMoved(Port port, int indexBefore, int indexAfter);
        event EventHandler<PortIndexChangedEventArgs> PortMoved;

        void NotifyInputAdded(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> InputAdded;
        void NotifyInputRemoved(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> InputRemoved;
        void NotifyInputMoved(Signal signal, int indexBefore, int indexAfter);
        event EventHandler<SignalIndexChangedEventArgs> InputMoved;

        void NotifyOutputAdded(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> OutputAdded;
        void NotifyOutputRemoved(Signal signal, int index);
        event EventHandler<SignalIndexEventArgs> OutputRemoved;
        void NotifyOutputMoved(Signal signal, int indexBefore, int indexAfter);
        event EventHandler<SignalIndexChangedEventArgs> OutputMoved;

        //void NotifyPortDrivesSignal(Signal signal, Port port, int outputIndex);
        event EventHandler<SignalPortIndexEventArgs> PortDrivesSignal;
        //void NotifyPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex);
        event EventHandler<SignalPortIndexEventArgs> PortDrivesSignalNoLonger;
        //void NotifySignalDrivesPort(Signal signal, Port port, int inputIndex);
        event EventHandler<SignalPortIndexEventArgs> SignalDrivesPort;
        //void NotifySignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex);
        event EventHandler<SignalPortIndexEventArgs> SignalDrivesPortNoLonger;
        //void NotifyBusAttachedToPort(Bus bus, Port port, int busIndex);
        event EventHandler<BusPortIndexEventArgs> BusAttachedToPort;
        //void NotifyBusDetachedFromPort(Bus bus, Port port, int busIndex);
        event EventHandler<BusPortIndexEventArgs> BusDetachedFromPort;

        //void NotifySignalValueChanged(Signal signal);
        event EventHandler<SignalEventArgs> SignalValueChanged;
        //void NotifyBusValueChanged(Bus bus);
        event EventHandler<BusEventArgs> BusValueChanged;
    }
}
