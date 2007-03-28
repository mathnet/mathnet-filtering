using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Mediator
{
    public interface ILogWriter
    {
        void WriteEntry(DateTime time, Guid systemId, LogAction type, Guid portId, Guid signalId, Guid busId, string entityId, int? index1);
    }

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
        BusDetachedFromPort,
        SignalValueChanged,
        BusValueChanged
    }
}
