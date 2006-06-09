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
using System.Text;
using System.Collections.Generic;
using System.Globalization;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend.Simulation;
using MathNet.Symbolics.Backend.Events;
using MathNet.Symbolics.Backend.Channels;

namespace MathNet.Symbolics.Backend
{
    public class Context
    {
        private Builder _builder;
        private Library _library;
        private Containers.SignalTable _singletonSignals;
        private Scheduler _scheduler;
        private Random _random;
        private HintChannel _hintChannel;
        private TimeSpan _simulationTime;

        /// <summary>
        /// Raised if the simulation time was shifted to a new value either because
        /// it would have overflown or because <see cref="ResetSimulationTime"/>
        /// was called. The new value is always nearer to zero, thus the time span
        /// in the event args is always negative.
        /// </summary>
        public event EventHandler<SimulationTimeEventArgs> SimulationTimeShifted;

        internal Context()
        {
            _library = new Library(this);
            _builder = new Builder(this);
            _scheduler = new Scheduler(this);
            _singletonSignals = new Containers.SignalTable();
            _random = new Random();
            _hintChannel = new HintChannel();
            _simulationTime = TimeSpan.Zero;
            _scheduler.SimulationTimeProgress += SimulationTimeProgressHandler;
        }

        public Builder Builder
        {
            get { return _builder; }
        }

        public Library Library
        {
            get { return _library; }
        }

        public Containers.SignalTable SingletonSignals
        {
            get { return _singletonSignals; }
        }

        public Scheduler Scheduler
        {
            get { return _scheduler; }
        }

        public Random Random
        {
            get { return _random; }
        }

        internal int GenerateTag()
        {
            return _random.Next();
        }

        public Guid GenerateInstanceId()
        {
            return Guid.NewGuid();
        }

        public HintChannel Hints
        {
            get { return _hintChannel; }
        }

        internal static string YttriumNamespace
        {
            get { return @"http://www.cdrnet.net/projects/nmath/symbolics/yttrium/system/0.50/"; }
        }

        public TimeSpan SimulationTime
        {
            get { return _simulationTime; }
        }

        public void ResetSimulationTime()
        {
            TimeSpan shift = -_simulationTime;
            _simulationTime = TimeSpan.Zero;
            if(SimulationTimeShifted != null && shift != TimeSpan.Zero)
                SimulationTimeShifted(this, new SimulationTimeEventArgs(shift));
        }

        private void SimulationTimeProgressHandler(object sender, SimulationTimeEventArgs e)
        {
            try
            {
                checked
                {
                    _simulationTime += e.TimeSpan;
                }
            }
            catch(OverflowException)
            {
                /* 
                 * This is a realistic scenario, as simulation time is independent
                 * of real time and may progress very fast (eg. 100 years per cycle).
                 */

                TimeSpan shift = -_simulationTime; // doesn't overlow, as TimeSpan.MinValue < -TimeSpan.MaxValue
                _simulationTime = e.TimeSpan;
                if(SimulationTimeShifted != null)
                    SimulationTimeShifted(this, new SimulationTimeEventArgs(shift));
            }
        }

        #region Globalization
        public static CultureInfo Culture
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public static StringComparer IdentifierComparer
        {
            get { return StringComparer.InvariantCulture; }
        }

        public static NumberFormatInfo NumberFormat
        {
            get { return NumberFormatInfo.InvariantInfo; }
        }

        public static Encoding DefaultEncoding
        {
            get { return Encoding.Unicode; }
        }
        #endregion

        #region Format Settings
        private char separatorCharacter = ',';
        public char SeparatorCharacter
        {
            get { return separatorCharacter; }
            set { separatorCharacter = value; }
        }

        private char executorCharacter = ';';
        public char ExecutorCharacter
        {
            get { return executorCharacter; }
            set { executorCharacter = value; }
        }

        private EncapsulationFormat listEncapsulation = new EncapsulationFormat('(', ')');
        public EncapsulationFormat ListEncapsulation
        {
            get {return listEncapsulation;}
            set {listEncapsulation = value;}
        }

        private EncapsulationFormat vectorEncapsulation = new EncapsulationFormat('[', ']');
        public EncapsulationFormat VectorEncapsulation
        {
            get { return vectorEncapsulation; }
            set { vectorEncapsulation = value; }
        }

        private EncapsulationFormat setEncapsulation = new EncapsulationFormat('{', '}');
        public EncapsulationFormat SetEncapsulation
        {
            get { return setEncapsulation; }
            set { setEncapsulation = value; }
        }

        private EncapsulationFormat scalarEncapsulation = new EncapsulationFormat('<', '>');
        public EncapsulationFormat ScalarEncapsulation
        {
            get { return scalarEncapsulation; }
            set { scalarEncapsulation = value; }
        }

        private EncapsulationFormat literalEncapsulation = new EncapsulationFormat('"', '"');
        public EncapsulationFormat LiteralEncapsulation
        {
            get { return literalEncapsulation; }
            set { literalEncapsulation = value; }
        }
        #endregion

        #region Notification (Trivial Mediator)
        #region Units Constructed & Deconstructed
        public event EventHandler<SignalEventArgs> OnNewSignalConstructed;
        internal void NotifyNewSignalConstructed(Signal signal)
        {
            EventHandler<SignalEventArgs> handler = OnNewSignalConstructed;
            if(handler != null)
                handler(this, new SignalEventArgs(signal));
        }

        public event EventHandler<PortEventArgs> OnNewPortConstructed;
        internal void NotifyNewPortConstructed(Port port)
        {
            EventHandler<PortEventArgs> handler = OnNewPortConstructed;
            if(handler != null)
                handler(this, new PortEventArgs(port));
        }

        public event EventHandler<BusEventArgs> OnNewBusConstructed;
        internal void NotifyNewBusConstructed(Bus bus)
        {
            EventHandler<BusEventArgs> handler = OnNewBusConstructed;
            if(handler != null)
                handler(this, new BusEventArgs(bus));
        }
        #endregion

        #region Port <-> Signal|Bus Connections
        public event EventHandler<SignalPortIndexEventArgs> OnSignalDrivenByPort;
        internal void NotifySignalDrivenByPort(Signal signal, Port port, int outputIndex)
        {
            EventHandler<SignalPortIndexEventArgs> handler = OnSignalDrivenByPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal,port,outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> OnSignalNoLongerDrivenByPort;
        internal void NotifySignalNoLongerDrivenByPort(Signal signal, Port port, int outputIndex)
        {
            EventHandler<SignalPortIndexEventArgs> handler = OnSignalNoLongerDrivenByPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, outputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> OnSignalDrivesPort;
        internal void NotifySignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            EventHandler<SignalPortIndexEventArgs> handler = OnSignalDrivesPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<SignalPortIndexEventArgs> OnSignalNoLongerDrivesPort;
        internal void NotifySignalNoLongerDrivesPort(Signal signal, Port port, int inputIndex)
        {
            EventHandler<SignalPortIndexEventArgs> handler = OnSignalNoLongerDrivesPort;
            if(handler != null)
                handler(this, new SignalPortIndexEventArgs(signal, port, inputIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> OnBusAttachedToPort;
        internal void NotifyBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            EventHandler<BusPortIndexEventArgs> handler = OnBusAttachedToPort;
            if(handler != null)
                handler(this, new BusPortIndexEventArgs(bus, port, busIndex));
        }

        public event EventHandler<BusPortIndexEventArgs> OnBusNoLongerAttachedToPort;
        internal void NotifyBusNoLongerAttachedToPort(Bus bus, Port port, int busIndex)
        {
            EventHandler<BusPortIndexEventArgs> handler = OnBusNoLongerAttachedToPort;
            if(handler != null)
                handler(this, new BusPortIndexEventArgs(bus, port, busIndex));
        }
        #endregion

        #region Signal State
        public event EventHandler<SignalEventArgs> OnSignalValueChanged;
        internal void NotifySignalValueChanged(Signal signal)
        {
            EventHandler<SignalEventArgs> handler = OnSignalValueChanged;
            if(handler != null)
                handler(this, new SignalEventArgs(signal));
        }
        #endregion
        #endregion
    }
}
