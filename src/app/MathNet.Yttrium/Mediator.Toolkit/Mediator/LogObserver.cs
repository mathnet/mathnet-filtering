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
using System.IO;

namespace MathNet.Symbolics.Mediator
{
    public class TextLogWriter : ILogWriter
    {
        private TextWriter _writer;

        public TextLogWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteEntry(DateTime time, Guid systemId, LogAction type, Guid portId, Guid signalId, Guid busId, string entityId, int? index1)
        {
            _writer.WriteLine("{0:s} -> {1:N} : {2,-20} -> P:{3:N} S:{4:N} B:{5:N} {7} {6}", time, systemId, type, portId, signalId, busId, entityId, index1.ToString() ?? "N/A");
            _writer.Flush();
        }
    }

    public class LogObserver : IObserver
    {
        private ILogWriter _writer;
        private Guid _emptyGuid;
        private string _emptyId;

        public LogObserver(ILogWriter writer)
        {
            _writer = writer;
            _emptyGuid = Guid.Empty;
            _emptyId = string.Empty;
        }

        void IObserver.OnNewSignalCreated(Signal signal)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.SignalAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, null);
        }

        void IObserver.OnNewPortCreated(Port port)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.PortAdded, port.InstanceId, _emptyGuid, _emptyGuid, port.Entity.EntityId.ToString(), null);
        }

        void IObserver.OnNewBusCreated(Bus bus)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.BusAdded, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, null);
        }

        void IObserver.OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.PortDrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        void IObserver.OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.PortUndrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        void IObserver.OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.SignalDrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        void IObserver.OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.SignalUndrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        void IObserver.OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.BusAttachedToPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }

        void IObserver.OnBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.BusDetachedFromPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }

        void IObserver.OnSignalValueChanged(Signal signal)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.SignalValueChanged, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, null);
        }

        void IObserver.OnBusValueChanged(Bus bus)
        {
            _writer.WriteEntry(DateTime.Now, _emptyGuid, LogAction.BusValueChanged, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, null);
        }
    }

    public class LogSystemObserver : ISystemObserver
    {
        private ILogWriter _writer;
        private Guid _emptyGuid;
        private string _emptyId;
        private Guid _currentSysId;

        public LogSystemObserver(ILogWriter writer)
        {
            _writer = writer;
            _emptyGuid = Guid.Empty;
            _emptyId = string.Empty;
        }

        public bool AutoDetachOnSystemChanged
        {
            get { return false; }
        }

        public bool AutoInitialize
        {
            get { return true; }
        }

        public void AttachedToSystem(IMathSystem system)
        {
            _currentSysId = system.InstanceId;
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SystemChanged, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, null);
        }

        public void DetachedFromSystem(IMathSystem system)
        {
        }

        void ISystemObserver.BeginInitialize()
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BeginInitialize, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, null);
        }

        void ISystemObserver.EndInitialize()
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.EndInitialize, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, null);
        }

        void ISystemObserver.OnSignalAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnSignalRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnSignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        void ISystemObserver.OnBusAdded(Bus bus, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusAdded, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, index);
        }

        void ISystemObserver.OnBusRemoved(Bus bus, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusRemoved, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, index);
        }

        void ISystemObserver.OnBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
        }

        void ISystemObserver.OnPortAdded(Port port, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortAdded, port.InstanceId, _emptyGuid, _emptyGuid, port.Entity.EntityId.ToString(), index);
        }

        void ISystemObserver.OnPortRemoved(Port port, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortRemoved, port.InstanceId, _emptyGuid, _emptyGuid, port.Entity.EntityId.ToString(), index);
        }

        void ISystemObserver.OnPortMoved(Port port, int indexBefore, int indexAfter)
        {
        }

        void ISystemObserver.OnInputAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.InputAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnInputRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.InputRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnInputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        void ISystemObserver.OnOutputAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.OutputAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnOutputRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.OutputRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        void ISystemObserver.OnOutputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        void ISystemObserver.OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortDrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        void ISystemObserver.OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortUndrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        void ISystemObserver.OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalDrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        void ISystemObserver.OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalUndrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        void ISystemObserver.OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusAttachedToPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }

        void ISystemObserver.OnBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusDetachedFromPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }

        void ISystemObserver.OnSignalValueChanged(Signal signal)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalValueChanged, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, null);
        }

        void ISystemObserver.OnBusValueChanged(Bus bus)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusValueChanged, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, null);
        }
    }
}
