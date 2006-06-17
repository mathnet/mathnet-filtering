using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class Pattern
    {
        private static Pattern _alwaysTrueInstance;

        private Condition _condition;

        public Pattern()
        {
            _condition = AlwaysTrueCondition.Instance;
        }
        public Pattern(Condition condition)
        {
            _condition = condition;
        }

        public static Pattern AlwaysTrueInstance
        {
            get
            {
                if(_alwaysTrueInstance == null)
                    _alwaysTrueInstance = new Pattern();
                return _alwaysTrueInstance;
            }
        }

        public virtual bool Match(Signal output, Port port)
        {
            return _condition.FulfillsCondition(output, port);
        }
    }
}
