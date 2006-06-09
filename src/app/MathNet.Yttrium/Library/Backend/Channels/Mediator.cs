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
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend.Events;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Channels
{
    /// <summary>
    /// Interface between a <see cref="MathSystem"/> and the outside world, providing
    /// a command and observer infrastructure for controler-based architectures like
    /// Model-View-Controler (MVC) or Presentation-Abstraction-Controler (PAC).
    /// </summary>
    public sealed class Mediator
    {
        private MathSystem _system;
        private CommandChannel _commands;
        private List<ISystemObserver> _observers;

        public event EventHandler SystemChanged;

        public Mediator()
        {
            _commands = new CommandChannel();
            _commands.EntryAvailable += EntryAvailableHandler;
            _commands.Enabled = true;
            _observers = new List<ISystemObserver>();
        }

        public MathSystem System
        {
            get { return _system; }
        }

        public Context Context
        {
            get { return _system.Context; }
        }

        #region Command Infrastructure
        public CommandChannel Commands
        {
            get { return _commands; }
        }

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

        #region Observer Infrastructure
        public void AttachObserver(ISystemObserver observer)
        {
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

        #region Notifications
        #region Add/Remove Signals/Buses/Ports
        internal void OnSignalAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalAdded(signal, index);
        }
        internal void OnSignalRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalRemoved(signal, index);
        }
        internal void OnSignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalMoved(signal, indexBefore, indexAfter);
        }

        internal void OnBusAdded(Bus bus, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusAdded(bus, index);
        }
        internal void OnBusRemoved(Bus bus, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusRemoved(bus, index);
        }
        internal void OnBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusMoved(bus, indexBefore, indexAfter);
        }

        internal void OnPortAdded(Port port, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortAdded(port, index);

            ReadOnlySignalSet allSignals = System.GetAllSignals();
            ReadOnlySignalSet inputs = port.InputSignals;
            for(int i=0;i<inputs.Count;i++)
                if(allSignals.Contains(inputs[i]))
                    OnSignalDrivesPort(inputs[i], port, i);
            ReadOnlySignalSet outputs = port.OutputSignals;
            for(int i=0;i<outputs.Count;i++)
                if(allSignals.Contains(outputs[i]))
                    OnPortDrivesSignal(outputs[i], port, i);
        }
        internal void OnPortRemoved(Port port, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortRemoved(port, index);
        }
        internal void OnPortMoved(Port port, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortMoved(port, indexBefore, indexAfter);
        }
        #endregion
        #region Add/Remove Inputs/Outputs
        internal void OnInputAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputAdded(signal, index);
        }
        internal void OnInputRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputRemoved(signal, index);
        }
        internal void OnInputMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnInputMoved(signal, indexBefore, indexAfter);
        }

        internal void OnOutputAdded(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputAdded(signal, index);
        }
        internal void OnOutputRemoved(Signal signal, int index)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputRemoved(signal, index);
        }
        internal void OnOutputMoved(Signal signal, int indexBefore, int indexAfter)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnOutputMoved(signal, indexBefore, indexAfter);
        }
        #endregion
        #region Add/Remove Connections
        internal void OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortDrivesSignal(signal, port, outputIndex);
        }
        internal void OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnPortDrivesSignalNoLonger(signal, port, outputIndex);
        }

        internal void OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalDrivesPort(signal, port, inputIndex);
        }
        internal void OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnSignalDrivesPortNoLonger(signal, port, inputIndex);
        }

        internal void OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusAttachedToPort(bus, port, busIndex);
        }
        internal void OnBusAttachedToPortNoLonger(Bus bus, Port port, int busIndex)
        {
            foreach(ISystemObserver observer in _observers)
                observer.OnBusAttachedToPortNoLonger(bus, port, busIndex);
        }
        #endregion
        #endregion

        #region System Subscription
        internal void SubscribeSystem(MathSystem system)
        {
            _system = system;
            EnableTracking(system.Context);
            foreach(ISystemObserver observer in _observers)
            {
                observer.AttachedToSystem(system);
                if(observer.AutoInitialize)
                    InitializeObserverWithCurrentSystem(observer);
            }
            if(SystemChanged != null)
                SystemChanged(this, EventArgs.Empty);
        }
        internal void UnsubscribeSystem(MathSystem system)
        {
            DisableTracking(system.Context);
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

        #region Filtered Context Tracking
        private void EnableTracking(Context context)
        {
            context.OnSignalDrivenByPort += context_OnSignalDrivenByPort;
            context.OnSignalDrivesPort += context_OnSignalDrivesPort;
            context.OnSignalNoLongerDrivenByPort += context_OnSignalNoLongerDrivenByPort;
            context.OnSignalNoLongerDrivesPort += context_OnSignalNoLongerDrivesPort;
            context.OnBusAttachedToPort += context_OnBusAttachedToPort;
            context.OnBusNoLongerAttachedToPort += context_OnBusNoLongerAttachedToPort;
        }

        private void DisableTracking(Context context)
        {
            context.OnSignalDrivenByPort -= context_OnSignalDrivenByPort;
            context.OnSignalDrivesPort -= context_OnSignalDrivesPort;
            context.OnSignalNoLongerDrivenByPort -= context_OnSignalNoLongerDrivenByPort;
            context.OnSignalNoLongerDrivesPort -= context_OnSignalNoLongerDrivesPort;
            context.OnBusAttachedToPort -= context_OnBusAttachedToPort;
            context.OnBusNoLongerAttachedToPort -= context_OnBusNoLongerAttachedToPort;
        }

        private void context_OnSignalDrivenByPort(object sender, SignalPortIndexEventArgs e)
        {
            if(_system.ContainsSignal(e.Signal) && _system.ContainsPort(e.Port))
                OnPortDrivesSignal(e.Signal, e.Port, e.Index);
        }
        private void context_OnSignalNoLongerDrivenByPort(object sender, SignalPortIndexEventArgs e)
        {
            if(_system.ContainsSignal(e.Signal) && _system.ContainsPort(e.Port))
                OnPortDrivesSignalNoLonger(e.Signal, e.Port, e.Index);
        }
        private void context_OnSignalDrivesPort(object sender, SignalPortIndexEventArgs e)
        {
            if(_system.ContainsSignal(e.Signal) && _system.ContainsPort(e.Port))
                OnSignalDrivesPort(e.Signal, e.Port, e.Index);
        }
        private void context_OnSignalNoLongerDrivesPort(object sender, SignalPortIndexEventArgs e)
        {
            if(_system.ContainsSignal(e.Signal) && _system.ContainsPort(e.Port))
                OnSignalDrivesPortNoLonger(e.Signal, e.Port, e.Index);
        }
        private void context_OnBusAttachedToPort(object sender, BusPortIndexEventArgs e)
        {
            if(_system.ContainsBus(e.Bus) && _system.ContainsPort(e.Port))
                OnBusAttachedToPort(e.Bus, e.Port, e.Index);
        }
        private void context_OnBusNoLongerAttachedToPort(object sender, BusPortIndexEventArgs e)
        {
            if(_system.ContainsBus(e.Bus) && _system.ContainsPort(e.Port))
                OnBusAttachedToPortNoLonger(e.Bus, e.Port, e.Index);
        }
        #endregion
    }
}
