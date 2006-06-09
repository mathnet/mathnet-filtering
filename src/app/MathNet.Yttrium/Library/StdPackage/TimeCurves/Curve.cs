using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.StdPackage.TimeCurves
{
    public class Curve
    {

        private class CurveSegment
        {
            public Curve Segment;
            public TimeSpan Begin;
            public ValueStructure Offset;


            public CurveSegment(Curve segment, TimeSpan begin, ValueStructure offset)
            {
                this.Segment = segment;
                this.Begin = begin;
                this.Offset = offset;
            }
        }
    }
}
