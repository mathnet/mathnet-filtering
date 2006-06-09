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
using System.Reflection;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.Backend.Events;
using MathNet.Symbolics.Backend.Parsing;
using MathNet.Symbolics.Backend.Channels;

namespace MathNet.Symbolics.Workplace
{
    public class Project : IContextSensitive
    {
        private readonly Context _context;
        private bool _keepTrack; // = false;
        private MathSystem _currentSystem;
        private Dictionary<Guid, MathSystem> _namedSystems;
        private List<ISystemObserver> _localObservers;
        private Parser _parser;

        public event EventHandler SystemLoaded;

        public Project() : this(new Context()) { }
        public Project(Context context)
        {
            _context = context;
            _context.Library.LoadPackageManager(new MathNet.Symbolics.StdPackage.StdPackageManager(context));

            _namedSystems = new Dictionary<Guid, MathSystem>();
            _localObservers = new List<ISystemObserver>();
            _parser = new Parser();
            MathSystem sys = AddSystem();
            LoadSystem(sys.InstanceId);
        }

        #region Properties
        public Context Context
        {
            get { return _context; }
        }

        public Builder Builder
        {
            get { return _context.Builder; }
        }

        public Parser Parser
        {
            get { return _parser; }
        }

        [Obsolete]
        public bool KeepTrack
        {
            get { return _keepTrack; }
            set
            {
                if(value && !_keepTrack)
                {
                    _context.OnNewSignalConstructed += context_OnNewSignalConstructed;
                    _context.OnNewPortConstructed += context_OnNewPortConstructed;
                    _context.OnNewBusConstructed += context_OnNewBusConstructed;
                    _context.OnSignalDrivenByPort += context_OnSignalDrivenByPort;
                    _context.OnSignalDrivesPort += context_OnSignalDrivesPort;
                    _context.OnSignalNoLongerDrivenByPort += context_OnSignalNoLongerDrivenByPort;
                    _context.OnSignalNoLongerDrivesPort += context_OnSignalNoLongerDrivesPort;
                    _context.OnBusAttachedToPort += context_OnBusAttachedToPort;
                    _context.OnBusNoLongerAttachedToPort += context_OnBusNoLongerAttachedToPort;
                    _keepTrack = true;
                }
                else if(!value && _keepTrack)
                {
                    _keepTrack = false;
                    _context.OnNewSignalConstructed -= context_OnNewSignalConstructed;
                    _context.OnNewPortConstructed -= context_OnNewPortConstructed;
                    _context.OnNewBusConstructed -= context_OnNewBusConstructed;
                    _context.OnSignalDrivenByPort -= context_OnSignalDrivenByPort;
                    _context.OnSignalDrivesPort -= context_OnSignalDrivesPort;
                    _context.OnSignalNoLongerDrivenByPort -= context_OnSignalNoLongerDrivenByPort;
                    _context.OnSignalNoLongerDrivesPort -= context_OnSignalNoLongerDrivesPort;
                    _context.OnBusAttachedToPort -= context_OnBusAttachedToPort;
                    _context.OnBusNoLongerAttachedToPort -= context_OnBusNoLongerAttachedToPort;
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
            if(!system.HasMediator)
                system.Mediator = new Mediator();
            _namedSystems.Add(system.InstanceId, system);
        }
        public MathSystem AddSystem()
        {
            MathSystem system = new MathSystem(_context);
            system.Mediator = new Mediator();
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

            if(!system.HasMediator)
                system.Mediator = new Mediator();

            foreach(ISystemObserver observer in _localObservers)
                system.Mediator.AttachObserver(observer);

            if(SystemLoaded != null)
                SystemLoaded(this, EventArgs.Empty);
        }
        private void UnloadSystem()
        {
            if(_currentSystem == null)
                return;

            foreach(ISystemObserver observer in _localObservers)
                _currentSystem.Mediator.DetachObserver(observer);

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
                _currentSystem.Mediator.AttachObserver(observer);
        }
        public void DetachLocalObserver(ISystemObserver observer)
        {
            _localObservers.Remove(observer);
            if(_currentSystem != null)
                _currentSystem.Mediator.DetachObserver(observer);
        }
        #endregion

        public void PostCommand(ICommand command)
        {
            _currentSystem.Mediator.PostCommand(command);
        }

        public void LoadPackage(Assembly assembly)
        {
            _context.Library.LoadAssembly(assembly);
        }
        public void LoadPackage(IPackageManager manager)
        {
            _context.Library.LoadPackageManager(manager);
        }

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
            return _context.Scheduler.SimulateInstant();
        }
        /// <param name="timespan">The time (in simulation time space) to simulate.</param>
        /// <returns>
        /// Actually simulated time.
        /// If this time is smaller than <see cref="timespan"/>, there were no more events available.
        /// </returns>
        public TimeSpan SimulateFor(TimeSpan timespan)
        {
            return _context.Scheduler.SimulateFor(timespan);
        }
        /// <param name="cycles">The number of cycles to simulate.</param>
        /// <returns>Simulated time.</returns>
        public TimeSpan SimulateFor(int cycles)
        {
            return _context.Scheduler.SimulateFor(cycles);
        }
        #endregion

        #region Tracking (Obsolete, to be removed!)
        void context_OnNewPortConstructed(object sender, PortEventArgs e)
        {
        }

        void context_OnNewSignalConstructed(object sender, SignalEventArgs e)
        {
            _currentSystem.AddSignal(e.Signal);
        }

        void context_OnNewBusConstructed(object sender, BusEventArgs e)
        {
            _currentSystem.AddBus(e.Bus);
        }

        void context_OnBusNoLongerAttachedToPort(object sender, BusPortIndexEventArgs e)
        {
        }

        void context_OnBusAttachedToPort(object sender, BusPortIndexEventArgs e)
        {
            _currentSystem.AddBus(e.Bus);
            _currentSystem.AddPortTree(e.Port, false, true);
        }

        void context_OnSignalNoLongerDrivesPort(object sender, SignalPortIndexEventArgs e)
        {
        }

        void context_OnSignalNoLongerDrivenByPort(object sender, SignalPortIndexEventArgs e)
        {
            _currentSystem.PromoteAsInput(e.Signal);
        }

        void context_OnSignalDrivesPort(object sender, SignalPortIndexEventArgs e)
        {
            _currentSystem.AddSignal(e.Signal);
            _currentSystem.AddPortTree(e.Port, false, true);
        }

        void context_OnSignalDrivenByPort(object sender, SignalPortIndexEventArgs e)
        {
            _currentSystem.AddSignalTree(e.Signal, false, true);
            _currentSystem.UnpromoteAsInput(e.Signal);
        }
        #endregion
    }
}
