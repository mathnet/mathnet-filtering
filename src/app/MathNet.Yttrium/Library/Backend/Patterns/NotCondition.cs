using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class NotCondition : Condition
    {
        private Condition _condition;

        public NotCondition(Condition condition)
        {
            _condition = condition;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            return !_condition.FulfillsCondition(output, port);
        }

        public override bool Equals(Condition other)
        {
            NotCondition ot = other as NotCondition;
            if(ot == null)
                return false;
            return _condition.Equals(ot._condition);
        }
    }
}
