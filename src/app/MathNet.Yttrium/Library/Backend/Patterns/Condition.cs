using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public abstract class Condition
    {
        public abstract bool FulfillsCondition(Signal output, Port port);
    }
}
