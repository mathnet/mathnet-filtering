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
    public class SystemMediator : ISystemMediator, IObserver
    {
        private List<ISystemObserver> _observers;
        private IMathSystem _system;
        private Channel<ICommand> _commands;

        public event EventHandler SystemChanged;

        public SystemMediator()
        {
            _commands = new Channel<ICommand>();
            _commands.EntryAvailable += EntryAvailableHandler;
            _commands.Enabled = true;
            _observers = new List<ISystemObserver>();
        }

        #region Observer Infrastructure
        public void AttachObserver(ISystemObserver observer)
        {
            if(_observers.Contains(observer))
                return;
            _observers.Add(observer);
            if(_system != null)
            {
                observer.AttachedToSystem(_system);
                if(observer.AutoInitialize)
                    InitializeObserverWithCurrentSystem(observer);
            }
        }
        public void DetachObserver(ISystemObserver observer)
        {
            if(!_observers.Contains(observer))
                return;
            _observers.Remove(observer);
            if(_system != null)
                observer.DetachedFromSystem(_system);
        }

        private void InitializeObserverWithCurrentSystem(ISystemObserver observer)
        {
            if(_system == null)
                return;

            observer.BeginInitialize();

            ReadOnlySignalSet allSignals = _system.GetAllSignals();
            for(int i = 0; i < allSignals.Count; i++)
                observer.OnSignalAdded(allSignals[i], i);

            ReadOnlyBusSet allBuses = _system.GetAllBuses();
            for(int i = 0; i < allBuses.Count; i++)
                observer.OnBusAdded(allBuses[i], i);

            ReadOnlyPortSet allPorts = _system.GetAllPorts();
            for(int i = 0; i < allPorts.Count; i++)
                observer.OnPortAdded(allPorts[i], i);

            for(int i = 0; i < allSignals.Count; i++)
            {
                Signal s = allSignals[i];
                if(s.IsDriven && allPorts.Contains(s.DrivenByPort))
                    observer.OnPortDrivesSignal(s, s.DrivenByPort, s.DrivenByPort.OutputSignals.IndexOf(s));
            }
            for(int i = 0; i < allPorts.Count; i++)
            {
                Port p = allPorts[i];
                for(int j = 0; j < p.InputSignalCount; j++)
                {
                    Signal s = p.InputSignals[j];
                    if(allSignals.Contains(s))
                        observer.OnSignalDrivesPort(s, p, j);
                }
            }

            ReadOnlySignalSet inputs = _system.GetAllInputs();
            for(int i = 0; i < inputs.Count; i++)
                observer.OnInputAdded(inputs[i], i);

            ReadOnlySignalSet outputs = _system.GetAllOutputs();
            for(int i = 0; i < outputs.Count; i++)
                observer.OnOutputAdded(outputs[i], i);

            observer.EndInitialize();
        }
        #endregion

        #region System Subscription
        public IMathSystem CurrentSystem
        {
            get { return _system; }
        }
        public void SubscribeSystem(IMathSystem system)
        {
            if(_system != null && _system.InstanceId == system.InstanceId)
                return; // already subscribed
            if(_system != null)
                throw new InvalidOperationException("There's already a system subscribed.");
            _system = system;
            foreach(ISystemObserver observer in _observers)
            {
                observer.AttachedToSystem(system);
                if(observer.AutoInitialize)
                    InitializeObserverWithCurrentSystem(observer);
            }
            Mediator.Instance.AttachObserver(this);
            if(SystemChanged != null)
                SystemChanged(this, EventArgs.Empty);
        }

        public void UnsubscribeSystem(IMathSystem system)
        {
            if(_system == null)
                return; // wasn't registered, no matter (might be programming error, but doesn't help anyone to throw; more robust this way.
                //throw new InvalidOperationException("There's no system subscribed.");
            Mediator.Instance.DetachObserver(this);
            for(int i = _observers.Count - 1; i >= 0; i--)
            {
                if(_observers[i].AutoDetachOnSystemChanged)
                    DetachObserver(_observers[i]);
                else
                    _observers[i].DetachedFromSystem(system);
            }
            _system = null;
        }
        #endregion

        #region Signal/Bus/Port Indices
        public event EventHandler<SignalIndexEventArgs> SignalAdded;
        public void NotifySignalAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalAdded(signal, index);
            EventHandler<SignalIndexEventArgs> handler = SignalAdded;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }
        
        public event EventHandler<SignalIndexEventArgs> SignalRemoved;
        public void NotifySignalRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalRemoved(signal, index);
            EventHandler<SignalIndexEventArgs> handler = SignalRemoved;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }

        public event EventHandler<SignalIndexChangedEventArgs> SignalMoved;
        public void NotifySignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalMoved(signal, indexBefore, indexAfter);
            EventHandler<SignalIndexChangedEventArgs> handler = SignalMoved;
            if(handler != null)
                handler(this, new SignalIndexChangedEventArgs(signal, indexBefore, indexAfter));
        }

        public event EventHandler<BusIndexEventArgs> BusAdded;
        public void NotifyBusAdded(Bus bus, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusAdded(bus, index);
            EventHandler<BusIndexEventArgs> handler = BusAdded;
            if(handler != null)
                handler(this, new BusIndexEventArgs(bus, index));
        }

        public event EventHandler<BusIndexEventArgs> BusRemoved;
        public void NotifyBusRemoved(Bus bus, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusRemoved(bus, index);
            EventHandler<BusIndexEventArgs> handler = BusRemoved;
            if(handler != null)
                handler(this, new BusIndexEventArgs(bus, index));
        }

        public event EventHandler<BusIndexChangedEventArgs> BusMoved;
        public void NotifyBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusMoved(bus, indexBefore, indexAfter);
            EventHandler<BusIndexChangedEventArgs> handler = BusMoved;
            if(handler != null)
                handler(this, new BusIndexChangedEventArgs(bus, indexBefore, indexAfter));
        }


        public event EventHandler<PortIndexEventArgs> PortAdded;
        public void NotifyPortAdded(Port port, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortAdded(port, index);
            EventHandler<PortIndexEventArgs> handler = PortAdded;
            if(handler != null)
                handler(this, new PortIndexEventArgs(port, index));
        }

        public event EventHandler<PortIndexEventArgs> PortRemoved;
        public void NotifyPortRemoved(Port port, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortRemoved(port, index);
            EventHandler<PortIndexEventArgs> handler = PortRemoved;
            if(handler != null)
                handler(this, new PortIndexEventArgs(port, index));
        }

        public event EventHandler<PortIndexChangedEventArgs> PortMoved;
        public void NotifyPortMoved(Port port, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortMoved(port, indexBefore, indexAfter);
            EventHandler<PortIndexChangedEventArgs> handler = PortMoved;
            if(handler != null)
                handler(this, new PortIndexChangedEventArgs(port, indexBefore, indexAfter));
        }
        #endregion

        #region Input/Output Indices
        public event EventHandler<SignalIndexEventArgs> InputAdded;
        public void NotifyInputAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputAdded(signal, index);
            EventHandler<SignalIndexEventArgs> handler = InputAdded;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }

        public event EventHandler<SignalIndexEventArgs> InputRemoved;
        public void NotifyInputRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputRemoved(signal, index);
            EventHandler<SignalIndexEventArgs> handler = InputRemoved;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }

        public event EventHandler<SignalIndexChangedEventArgs> InputMoved;
        public void NotifyInputMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputMoved(signal, indexBefore, indexAfter);
            EventHandler<SignalIndexChangedEventArgs> handler = InputMoved;
            if(handler != null)
                handler(this, new SignalIndexChangedEventArgs(signal, indexBefore, indexAfter));
        }


        public event EventHandler<SignalIndexEventArgs> OutputAdded;
        public void NotifyOutputAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputAdded(signal, index);
            EventHandler<SignalIndexEventArgs> handler = OutputAdded;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }

        public event EventHandler<SignalIndexEventArgs> OutputRemoved;
        public void NotifyOutputRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputRemoved(signal, index);
            EventHandler<SignalIndexEventArgs> handler = OutputRemoved;
            if(handler != null)
                handler(this, new SignalIndexEventArgs(signal, index));
        }

        public event EventHandler<SignalIndexChangedEventArgs> OutputMoved;
        public void NotifyOutputMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputMoved(signal, indexBefore, indexAfter);
            EventHandler<SignalIndexChangedEventArgs> handler = OutputMoved;
            if(handler != null)
                handler(this, new SignalIndexChangedEventArgs(signal, indexBefore, indexAfter));
        }
        #endregion

        #region Port <-> Signal|Bus Connections
        public event EventHandler<SignalPortIndexEventArgs> PortDrivesSignal;
        public void NotifyPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortDrivesSignal(signal, port, outputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = PortDrivesSignal;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> PortDrivesSignalNoLonger;
        public void NotifyPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortDrivesSignalNoLonger(signal, port, outputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = PortDrivesSignalNoLonger;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> SignalDrivesPort;
        public void NotifySignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalDrivesPort(signal, port, inputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = SignalDrivesPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> SignalDrivesPortNoLonger;
        public void NotifySignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalDrivesPortNoLonger(signal, port, inputIndex);
            EventHandler<SignalPortIndexEventArgs> handler = SignalDrivesPortNoLonger;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> BusAttachedToPort;
        public void NotifyBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusAttachedToPort(bus, port, busIndex);
            EventHandler<BusPortIndexEventArgs> handler = BusAttachedToPort;
            if(handler != null)
                handler(this, new BusPortIndexEventArgs(bus, port, busIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> BusDetachedFromPort;
        public void NotifyBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
            foreach(ISystemObserver observer in _observers)
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
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalValueChanged(signal);
            EventHandler<SignalEventArgs> handler = SignalValueChanged;
            if(handler != null)
                handler(this, new SignalEventArgs(signal));
        }

        public event EventHandler<BusEventArgs> BusValueChanged;
        public void NotifyBusValueChanged(Bus bus)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusValueChanged(bus);
            EventHandler<BusEventArgs> handler = BusValueChanged;
            if(handler != null)
                handler(this, new BusEventArgs(bus));
        }
        #endregion

        #region IObserver Members
        void IObserver.OnNewSignalCreated(Signal signal)
        {
        }

        void IObserver.OnNewPortCreated(Port port)
        {
        }

        void IObserver.OnNewBusCreated(Bus bus)
        {
        }

        void IObserver.OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            if(_system.ContainsSignal(signal) && _system.ContainsPort(port))
                NotifyPortDrivesSignal(signal, port, outputIndex);
        }

        void IObserver.OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            if(_system.ContainsSignal(signal) && _system.ContainsPort(port))
                NotifyPortDrivesSignalNoLonger(signal, port, outputIndex);
        }

        void IObserver.OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            if(_system.ContainsSignal(signal) && _system.ContainsPort(port))
                NotifySignalDrivesPort(signal, port, inputIndex);
        }

        void IObserver.OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {

            if(_system.ContainsSignal(signal) && _system.ContainsPort(port))
                NotifySignalDrivesPortNoLonger(signal, port, inputIndex);
        }

        void IObserver.OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            if(_system.ContainsBus(bus) && _system.ContainsPort(port))
                NotifyBusAttachedToPort(bus, port, busIndex);
        }

        void IObserver.OnBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
            if(_system.ContainsBus(bus) && _system.ContainsPort(port))
                NotifyBusDetachedFromPort(bus, port, busIndex);
        }

        void IObserver.OnSignalValueChanged(Signal signal)
        {
            if(_system.ContainsSignal(signal))
                NotifySignalValueChanged(signal);
        }

        void IObserver.OnBusValueChanged(Bus bus)
        {
            if(_system.ContainsBus(bus))
                NotifyBusValueChanged(bus);
        }

        #endregion

        #region Command Infrastructure
        //public CommandChannel Commands
        //{
        //    get { return _commands; }
        //}

        public void PostCommand(ICommand command)
        {
            _commands.PushEntry(command);
        }

        private void EntryAvailableHandler(object sender, EventArgs e)
        {
            while(_commands.HasEntries)
            {
                ICommand cmd = _commands.PopEntry();
                cmd.System = _system;
                cmd.Execute();
            }
        }
        #endregion
    }
}
