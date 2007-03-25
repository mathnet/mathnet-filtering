using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    [Obsolete]
    public interface IPort_BuilderAdapter
    {
        Guid AcceptSystemBuilder(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings);
        //IInstanceIdSet BuilderMapSignals(ISignalSet signals, Dictionary<Guid, Guid> signalMappings);
        //IInstanceIdSet BuilderMapBuses(IBusSet buses, Dictionary<Guid, Guid> busMappings);
    }
}
