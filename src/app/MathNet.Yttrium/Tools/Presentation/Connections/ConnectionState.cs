using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Presentation.Connections
{
    [Flags]
    public enum ConnectionStates : int
    {
        Unassigned = 0x00,
        SingleAssigned = 0x01,
        SourceAssigned = 0x02 | SingleAssigned,
        DestinationAssigned = 0x04 | SingleAssigned,
        BothAssigned = 0x08,

        HasPort = 0x20,
        HasBus = 0x21,
        HasSignal = 0x22,

        HasPortOut = 0x40 | HasPort,
        HasPortIn = 0x41 | HasPort,
        HasPortBus = 0x42 | HasPort,
        HasSignalOut = 0x44 | HasSignal,
        HasSignalIn = 0x48 | HasSignal,

        OnlyPortOut = HasPortOut | SourceAssigned,
        OnlyPortIn = HasPortIn | DestinationAssigned,
        OnlyPortBus = HasPortBus | DestinationAssigned,
        OnlySignalOut = HasSignalOut | SourceAssigned,
        OnlySignalIn = HasSignalIn | DestinationAssigned,
        OnlyBus = HasBus | SourceAssigned,

        PortToSignal = HasPortOut | HasSignalIn | BothAssigned,
        SignalToPort = HasPortIn | HasSignalOut | BothAssigned,
        BusToPort = HasPortBus | HasBus | BothAssigned
    }
}
