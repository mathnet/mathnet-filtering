using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class ArchitectureCondition : Condition
    {
        private Predicate<Architecture> _match;

        public ArchitectureCondition(Predicate<Architecture> match)
        {
            _match = match;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            if(port == null)
                return false;

            return port.HasArchitectureLink && _match(port.CurrentArchitecture);
        }

        public override bool Equals(Condition other)
        {
            ArchitectureCondition ot = other as ArchitectureCondition;
            if(ot == null)
                return false;
            return _match.Equals(ot._match);
        }
    }
}
