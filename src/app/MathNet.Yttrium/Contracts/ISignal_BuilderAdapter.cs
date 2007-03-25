using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    [Obsolete]
    public interface ISignal_BuilderAdapter
    {
        Guid AcceptSystemBuilderBefore(ISystemBuilder builder);
        void AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings);
        void BuilderSetValue(IValueStructure structure);
        void BuilderAppendProperty(IProperty property);
        void BuilderAppendConstraint(IProperty constraint);
    }
}
