using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics
{
    [Obsolete]
    public interface ISignal_CycleAnalysis
    {
        int AddCycles(Signal source, int tag);
        int RemoveCycles(Signal source, int tag);
    }
}
