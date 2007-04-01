using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Presentation.Connectors
{
    public enum ConnectorType
    {
        NetronConnector = 0,
        SignalInputConnector,
        SignalOutputConnector,
        BusConnector,
        PortInputConnector,
        PortOutputConnector,
        PortBusConnector
    }
}
