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
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Mediator
{
    public class Mediator : IMediator
    {
        private static Mediator _instance = new Mediator();
        public static Mediator Instance
        {
            get { return _instance; }
        }

        private List<IObserver> _observers;

        public Mediator()
        {
            _observers = new List<IObserver>();
        }

        public void AttachObserver(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void DetachObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        #region Units Constructed & Deconstructed
        public event EventHandler<SignalEventArgs> NewSignalCreated;
        public void NotifyNewSignalCreated(Signal signal)
        {
            foreach(IObserver observer in _observers)
                observer.OnNewSignalCreated(signal);
            EventHandler<SignalEventArgs> handler = NewSignalCreated;
            if(handler != null)
                handler(this, new SignalEventArgs(signal));
        }

        public event EventHandler<PortEventArgs> NewPortCreated;
        public void NotifyNewPortCreated(Port port)
        {
            foreach(IObserver observer in _observers)
                observer.OnNewPortCreated(port);
            EventHandler<PortEventArgs> handler = NewPortCreated;
            if(handler != null)
                handler(this, new PortEventArgs(port));
        }

        public event EventHandler<BusEventArgs> NewBusCreated;
        public void NotifyNewBusCreated(Bus bus)
        {
            foreach(IObserver observer in _observers)
                observer.OnNewBusCreated(bus);
            EventHandler<BusEventArgs> handler = NewBusCreated;
            if(handler != null)
                handler(this, new BusEventArgs(bus));
        }
        #endregion

        #region Port <-> Signal|Bus Connections
        public event EventHandler<SignalPortIndexEventArgs> PortDrivesSignal;
        public void NotifyPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnPortDrivesSignal(signal, port, outputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = PortDrivesSignal;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> PortDrivesSignalNoLonger;
        public void NotifyPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnPortDrivesSignalNoLonger(signal, port, outputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = PortDrivesSignalNoLonger;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> SignalDrivesPort;
        public void NotifySignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnSignalDrivesPort(signal, port, inputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = SignalDrivesPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> SignalDrivesPortNoLonger;
        public void NotifySignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnSignalDrivesPortNoLonger(signal, port, inputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = SignalDrivesPortNoLonger;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> BusAttachedToPort;
        public void NotifyBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnBusAttachedToPort(bus, port, busIndex);
            EventHandler<BusPortIndexEventArgs> handler = BusAttachedToPort;
            if(handler != null)
                handler(this, new BusPortIndexEventArgs(bus, port, busIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> BusDetachedFromPort;
        public void NotifyBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
            foreach(IObserver observer in _observers)
                observer.OnBusDetachedFromPort(bus, port, busIndex);
            EventHandler<BusPortIndexEventArgs> handler = BusDetachedFromPort;
            if(handler != null)
                handler(this, new BusPortIndexEventArgs(bus, port, busIndex));
        }
        #endregion

        #region ValueNode State
        public event EventHandler<SignalEventArgs> SignalValueChanged;
        public void NotifySignalValueChanged(Signal signal)
        {
            foreach(IObserver observer in _observers)
                observer.OnSignalValueChanged(signal);
            EventHandler<SignalEventArgs> handler = SignalValueChanged;
            if(handler != null)
                handler(this, new SignalEventArgs(signal));
        }

        public event EventHandler<BusEventArgs> BusValueChanged;
        public void NotifyBusValueChanged(Bus bus)
        {
            foreach(IObserver observer in _observers)
                observer.OnBusValueChanged(bus);
            EventHandler<BusEventArgs> handler = BusValueChanged;
            if(handler != null)
                handler(this, new BusEventArgs(bus));
        }
        #endregion
    }
}
