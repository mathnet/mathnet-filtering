using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    [Obsolete]
    public interface IBus_BuilderAdapter
    {
        Guid AcceptSystemBuilderBefore(ISystemBuilder builder);
        void AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings);
    }
}
