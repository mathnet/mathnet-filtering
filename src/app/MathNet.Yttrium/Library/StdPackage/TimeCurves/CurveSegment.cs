using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.StdPackage.TimeCurves
{
    public class CurveSegment<T> where T : ValueStructure, IAlgebraicCommutativeRingWithUnity<T>
    {
        private TimeSpan _timeOffset;
        private T _valueOffset;
        private T _slopePerSecond;

        public CurveSegment(TimeSpan timeOffset, T valueOffset, T slopePerSecond)
       { 
        }
    }
}
