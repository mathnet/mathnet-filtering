#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Patterns.Toolkit
{
    public class OutputSignalsPropertyCondition : Condition
    {
        private MathIdentifier _propertyType;
        private CombinationMode _mode;

        public OutputSignalsPropertyCondition(MathIdentifier propertyType, CombinationMode mode)
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
                    foreach(Signal s in port.OutputSignals)
                        if(!s.HasProperty(_propertyType))
                            return false;
                    return true;
                case CombinationMode.None:
                    foreach(Signal s in port.OutputSignals)
                        if(s.HasProperty(_propertyType))
                            return false;
                    return true;
                case CombinationMode.AtLeastOne:
                    foreach(Signal s in port.OutputSignals)
                        if(s.HasProperty(_propertyType))
                            return true;
                    return false;
                case CombinationMode.One:
                    foreach(Signal s in port.OutputSignals)
                        if(s.HasProperty(_propertyType))
                            cnt++;
                    return cnt == 1;
                case CombinationMode.AtMostOne:
                    foreach(Signal s in port.OutputSignals)
                        if(s.HasProperty(_propertyType) && cnt++ > 0)
                            return false;
                    return true;
                default:
                    throw new NotSupportedException("Pattern Condition doesn't support mode " + _mode.ToString());
            }
        }

        public override bool Equals(Condition other)
        {
            OutputSignalsPropertyCondition ot = other as OutputSignalsPropertyCondition;
            if(ot == null)
                return false;
            return _propertyType.Equals(ot._propertyType) && _mode.Equals(ot._mode);
        }
    }
}
