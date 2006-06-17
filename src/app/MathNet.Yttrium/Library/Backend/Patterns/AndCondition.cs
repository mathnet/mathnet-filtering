using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class AndCondition : Condition
    {
        private IEnumerable<Condition> _conditions;

        public AndCondition(IEnumerable<Condition> conditions)
        {
            _conditions = conditions;
        }
        public AndCondition(params Condition[] conditions)
        {
            _conditions = conditions;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            foreach(Condition c in _conditions)
                if(!c.FulfillsCondition(output, port))
                    return false;
            return true;
        }
    }
}
