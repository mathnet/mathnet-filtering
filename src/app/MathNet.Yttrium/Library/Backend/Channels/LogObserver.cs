using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Backend.Channels;

namespace MathNet.Symbolics.Backend.Channels
{
    public enum LogAction
    {
        SystemChanged,
        BeginInitialize,
        EndInitialize,
        SignalAdded,
        SignalRemoved,
        BusAdded,
        BusRemoved,
        PortAdded,
        PortRemoved,
        InputAdded,
        InputRemoved,
        OutputAdded,
        OutputRemoved,
        PortDrivesSignal,
        PortUndrivesSignal,
        SignalDrivesPort,
        SignalUndrivesPort,
        BusAttachedToPort,
        BusDetachedFromPort
    }

    public interface ILogWriter
    {
        void WriteEntry(DateTime time, Guid systemId, LogAction type, Guid portId, Guid signalId, Guid busId, string entityId, int index);
    }

    public class TextLogWriter : ILogWriter
    {
        private TextWriter _writer;

        public TextLogWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteEntry(DateTime time, Guid systemId, LogAction type, Guid portId, Guid signalId, Guid busId, string entityId, int index)
        {
            _writer.WriteLine("{0:s} -> {1:N} : {2,-20} -> P:{3:N} S:{4:N} B:{5:N} {7} {6}", time, systemId, type, portId, signalId, busId, entityId, index);
            _writer.Flush();
        }
    }

    public class LogObserver : ISystemObserver
    {
        private ILogWriter _writer;
        private Guid _emptyGuid;
        private string _emptyId;
        private Guid _currentSysId;
   
        public LogObserver(ILogWriter writer)
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

        public void AttachedToSystem(MathSystem system)
        {
            _currentSysId = system.InstanceId;
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SystemChanged, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, -1);
        }

        public void DetachedFromSystem(MathSystem system)
        {
        }

        public void BeginInitialize()
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BeginInitialize, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, -1);
        }

        public void EndInitialize()
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.EndInitialize, _emptyGuid, _emptyGuid, _emptyGuid, _emptyId, -1);
        }

        public void OnSignalAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnSignalRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnSignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        public void OnBusAdded(Bus bus, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusAdded, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, index);
        }

        public void OnBusRemoved(Bus bus, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusRemoved, _emptyGuid, _emptyGuid, bus.InstanceId, _emptyId, index);
        }

        public void OnBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
        }

        public void OnPortAdded(Port port, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortAdded, port.InstanceId, _emptyGuid, _emptyGuid, port.Entity.EntityId.ToString(), index);
        }

        public void OnPortRemoved(Port port, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortRemoved, port.InstanceId, _emptyGuid, _emptyGuid, port.Entity.EntityId.ToString(), index);
        }

        public void OnPortMoved(Port port, int indexBefore, int indexAfter)
        {
        }

        public void OnInputAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.InputAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnInputRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.InputRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnInputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        public void OnOutputAdded(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.OutputAdded, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnOutputRemoved(Signal signal, int index)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.OutputRemoved, _emptyGuid, signal.InstanceId, _emptyGuid, _emptyId, index);
        }

        public void OnOutputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }

        public void OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortDrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        public void OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.PortUndrivesSignal, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), outputIndex);
        }

        public void OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalDrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        public void OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.SignalUndrivesPort, port.InstanceId, signal.InstanceId, _emptyGuid, port.Entity.EntityId.ToString(), inputIndex);
        }

        public void OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusAttachedToPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }

        public void OnBusAttachedToPortNoLonger(Bus bus, Port port, int busIndex)
        {
            _writer.WriteEntry(DateTime.Now, _currentSysId, LogAction.BusDetachedFromPort, port.InstanceId, _emptyGuid, bus.InstanceId, port.Entity.EntityId.ToString(), busIndex);
        }
    }
}
