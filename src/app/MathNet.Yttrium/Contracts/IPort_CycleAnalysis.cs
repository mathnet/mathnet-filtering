using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    [Obsolete]
    public interface IPort_CycleAnalysis
    {
        bool TagWasTagged(int tag);
        void DeTag(int tag);
    }
}