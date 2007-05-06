using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceAnalysis.FFT
{
    class FftSetProvider
    {
        public double[] BuildSamples(int length)
        {
            int half = length >> 1;
            double[] data = new double[length];
            for(int i = 0; i < length; i++)
            {
                double z = (double)(i - half) / half;
                data[i] = 1.0 / (z * z + 1.0);
            }
            return data;
        }
    }
}
