using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class EntityCondition : Condition
    {
        private MathIdentifier _entityId;

        public EntityCondition(MathIdentifier entityId)
        {
            _entityId = entityId;
        }
        [Obsolete("Use MathIdentifiers directly",false)]
        public EntityCondition(string entityLabel, string entityDomain)
        {
            _entityId = new MathIdentifier(entityLabel, entityDomain);
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            return _entityId.Equals(port.Entity.EntityId);
        }
    }
}
