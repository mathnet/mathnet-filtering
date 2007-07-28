using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    public sealed class NodeEvent
        : EventAspect<MathIdentifier, NodeEvent>
    {
        public static NodeEvent Register(MathIdentifier id, Type handlerType, Type ownerType)
        {
            NodeEvent np = new NodeEvent(id, handlerType, ownerType, null);
            return np;
        }

        private NodeEvent(MathIdentifier id, Type handlerType, Type ownerType, IdentifierService<MathIdentifier> service)
            : base(id, handlerType, ownerType, service)
        {
        }
    }
}
