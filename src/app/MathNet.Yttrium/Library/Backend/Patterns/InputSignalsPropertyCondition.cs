using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class InputSignalsPropertyCondition : Condition
    {
        private MathIdentifier _propertyType;
        private CombinationMode _mode;

        public InputSignalsPropertyCondition(MathIdentifier propertyType, CombinationMode mode)
        {
            _propertyType = propertyType;
            _mode = mode;
        }

        public override bool FulfillsCondition(Signal output, Port port)
        {
            if(port == null)
                return false;

            int cnt = 0;
            switch(_mode)
            {
                case CombinationMode.All:
                    foreach(Signal s in port.InputSignals)
                        if(!s.HasProperty(_propertyType))
                            return false;
                    return true;
                case CombinationMode.None:
                    foreach(Signal s in port.InputSignals)
                        if(s.HasProperty(_propertyType))
                            return false;
                    return true;
                case CombinationMode.AtLeastOne:
                    foreach(Signal s in port.InputSignals)
                        if(s.HasProperty(_propertyType))
                            return true;
                    return false;
                case CombinationMode.One:
                    foreach(Signal s in port.InputSignals)
                        if(s.HasProperty(_propertyType))
                            cnt++;
                    return cnt == 1;
                case CombinationMode.AtMostOne:
                    foreach(Signal s in port.InputSignals)
                        if(s.HasProperty(_propertyType) && cnt++ > 0)
                            return false;
                    return true;
                default:
                    throw new NotSupportedException("Pattern Condition doesn't support mode " + _mode.ToString());
            }
        }

        public override bool Equals(Condition other)
        {
            InputSignalsPropertyCondition ot = other as InputSignalsPropertyCondition;
            if(ot == null)
                return false;
            return _propertyType.Equals(ot._propertyType) && _mode.Equals(ot._mode);
        }
    }
}
