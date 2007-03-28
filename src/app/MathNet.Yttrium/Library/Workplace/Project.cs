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
using System.Reflection;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Interpreter;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Events;
using MathNet.Symbolics.Simulation;

namespace MathNet.Symbolics.Workplace
{
    public class Project : IObserver
    {
        private bool _keepTrack; // = false;
        private MathSystem _currentSystem;
        private Dictionary<Guid, MathSystem> _namedSystems;
        private List<ISystemObserver> _localObservers;
        private IParser _parser;
        private IMediator _mediator;

        public event EventHandler SystemLoaded;

        public Project()
        {
            Service<IPackageLoader>.Instance.LoadDefaultPackages();

            _namedSystems = new Dictionary<Guid, MathSystem>();
            _localObservers = new List<ISystemObserver>();
            _parser = Service<IParser>.Instance;
            _mediator = Service<IMediator>.Instance;
            MathSystem sys = AddSystem();
            LoadSystem(sys.InstanceId);
        }

        #region Properties
        public IParser Parser
        {
            get { return _parser; }
        }

        public bool KeepTrack
        {
            get { return _keepTrack; }
            set
            {
                if(value && !_keepTrack)
                {
                    _mediator.AttachObserver(this);
                    _keepTrack = true;
                }
                else if(!value && _keepTrack)
                {
                    _keepTrack = false;
                    _mediator.DetachObserver(this);
                }
            }
        }

        public MathSystem CurrentSystem
        {
            get { return _currentSystem; }
            //set { currentSystem = value; }
        }
        #endregion

        #region System Management
        public void ImportSystem(MathSystem system)
        {
            if(!system.HasSystemMediator)
                system.SystemMediator = Binder.GetInstance<ISystemMediator>();
            _namedSystems.Add(system.InstanceId, system);
        }
        public MathSystem AddSystem()
        {
            MathSystem system = new MathSystem();
            Binder.GetInstance<ISystemMediator, IMathSystem>(system);
            //system.SystemMediator = Binder.GetInstance<ISystemMediator>();
            _namedSystems.Add(system.InstanceId, system);
            return system;
        }
        public void LoadSystem(int index)
        {

        }
        public void LoadSystem(Guid iid)
        {
            UnloadSystem();   
            LoadSystem(_namedSystems[iid]);
        }
        private void LoadSystem(MathSystem system)
        {
            _currentSystem = system;

            if(!system.HasSystemMediator)
                system.SystemMediator = Binder.GetInstance<ISystemMediator>();

            foreach(ISystemObserver observer in _localObservers)
                system.SystemMediator.AttachObserver(observer);

            if(SystemLoaded != null)
                SystemLoaded(this, EventArgs.Empty);
        }
        private void UnloadSystem()
        {
            if(_currentSystem == null)
                return;

            foreach(ISystemObserver observer in _localObservers)
                _currentSystem.SystemMediator.DetachObserver(observer);

            _currentSystem = null;
        }
        public Dictionary<Guid, MathSystem> Systems
        {
            get { return _namedSystems; }
        }
        #endregion

        #region Local Observers
        public void AttachLocalObserver(ISystemObserver observer)
        {
            _localObservers.Add(observer);
            if(_currentSystem != null)
                _currentSystem.SystemMediator.AttachObserver(observer);
        }
        public void DetachLocalObserver(ISystemObserver observer)
        {
            _localObservers.Remove(observer);
            if(_currentSystem != null)
                _currentSystem.SystemMediator.DetachObserver(observer);
        }
        #endregion

        //public void PostCommand(MathNet.Symbolics.Backend.Channels.ICommand command)
        //{
        //    // TODO: UNCOMMENT
        //    //_currentSystem.SystemMediator.PostCommand(command);
        //}

        //public void LoadPackage(Assembly assembly)
        //{
        //    _context.Library.LoadAssembly(assembly);
        //}
        //public void LoadPackage(IPackageManager manager)
        //{
        //    ((Library)_context.Library).LoadPackageManager(manager);
        //}

        #region Interpreter
        public void Interpret(string expressions)
        {
            _parser.Interpret(expressions, _currentSystem);
        }
        public void Interpret(System.IO.TextReader reader)
        {
            _parser.Interpret(reader, _currentSystem);
        }
        public void Interpret(System.IO.FileInfo file)
        {
            _parser.Interpret(file, _currentSystem);
        }
        #endregion

        #region Simulator
        /// <returns>True if there were any events available.</returns>
        public bool SimulateInstant()
        {
            return Service<ISimulationMediator>.Instance.SimulateInstant();
        }
        /// <param name="timespan">The time (in simulation time space) to simulate.</param>
        /// <returns>
        /// Actually simulated time.
        /// If this time is smaller than <see cref="timespan"/>, there were no more events available.
        /// </returns>
        public TimeSpan SimulateFor(TimeSpan timespan)
        {
            return Service<ISimulationMediator>.Instance.SimulateFor(timespan);
        }
        /// <param name="cycles">The number of cycles to simulate.</param>
        /// <returns>Simulated time.</returns>
        public TimeSpan SimulateFor(int cycles)
        {
            return Service<ISimulationMediator>.Instance.SimulateFor(cycles);
        }
        #endregion

        #region IObserver Members

        void IObserver.OnNewSignalCreated(Signal signal)
        {
            _currentSystem.AddSignal(signal);
        }

        void IObserver.OnNewPortCreated(Port port)
        {
            _currentSystem.AddPort(port);
        }

        void IObserver.OnNewBusCreated(Bus bus)
        {
            _currentSystem.AddBus(bus);
        }

        void IObserver.OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            _currentSystem.AddSignalTree(signal, false, true);
            _currentSystem.UnpromoteAsInput(signal);
        }

        void IObserver.OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            _currentSystem.PromoteAsInput(signal);
        }

        void IObserver.OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            _currentSystem.AddSignal(signal);
            _currentSystem.AddPortTree(port, false, true);
        }

        void IObserver.OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
        }

        void IObserver.OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            _currentSystem.AddBus(bus);
            _currentSystem.AddPortTree(port, false, true);
        }

        void IObserver.OnBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
        }

        void IObserver.OnSignalValueChanged(Signal signal)
        {
        }

        void IObserver.OnBusValueChanged(Bus bus)
        {
        }

        #endregion
    }
}
