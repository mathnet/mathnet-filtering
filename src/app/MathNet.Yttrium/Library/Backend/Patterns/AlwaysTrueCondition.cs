using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class AlwaysTrueCondition : Condition
    {
        private static AlwaysTrueCondition _instance;

        private AlwaysTrueCondition() {}

        public static AlwaysTrueCondition Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new AlwaysTrueCondition();
                return _instance;
            }
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            return true;
        }
    }
}
