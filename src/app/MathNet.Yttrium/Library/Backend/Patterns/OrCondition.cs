using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class OrCondition : Condition
    {
        private IEnumerable<Condition> _conditions;

        public OrCondition(IEnumerable<Condition> conditions)
        {
            _conditions = conditions;
        }
        public OrCondition(params Condition[] conditions)
        {
            _conditions = conditions;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            foreach(Condition c in _conditions)
                if(c.FulfillsCondition(output, port))
                    return true;
            return false;
        }
    }
}
