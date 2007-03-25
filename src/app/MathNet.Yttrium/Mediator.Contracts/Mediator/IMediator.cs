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
    public interface IMediator
    {
        void AttachObserver(IObserver observer);
        void DetachObserver(IObserver observer);

        void NotifyNewSignalCreated(Signal signal);
        event EventHandler<SignalEventArgs> NewSignalCreated;
        void NotifyNewPortCreated(Port port);
        event EventHandler<PortEventArgs> NewPortCreated;
        void NotifyNewBusCreated(Bus bus);
        event EventHandler<BusEventArgs> NewBusCreated;

        void NotifyPortDrivesSignal(Signal signal, Port port, int outputIndex);
        event EventHandler<SignalPortIndexEventArgs> PortDrivesSignal;
        void NotifyPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex);
        event EventHandler<SignalPortIndexEventArgs> PortDrivesSignalNoLonger;
        void NotifySignalDrivesPort(Signal signal, Port port, int inputIndex);
        event EventHandler<SignalPortIndexEventArgs> SignalDrivesPort;
        void NotifySignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex);
        event EventHandler<SignalPortIndexEventArgs> SignalDrivesPortNoLonger;
        void NotifyBusAttachedToPort(Bus bus, Port port, int busIndex);
        event EventHandler<BusPortIndexEventArgs> BusAttachedToPort;
        void NotifyBusDetachedFromPort(Bus bus, Port port, int busIndex);
        event EventHandler<BusPortIndexEventArgs> BusDetachedFromPort;

        void NotifySignalValueChanged(Signal signal);
        event EventHandler<SignalEventArgs> SignalValueChanged;
        void NotifyBusValueChanged(Bus bus);
        event EventHandler<BusEventArgs> BusValueChanged;
    }
}
