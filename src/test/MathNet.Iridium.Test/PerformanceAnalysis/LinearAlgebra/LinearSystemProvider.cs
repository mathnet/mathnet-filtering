using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceAnalysis.LinearAlgebra
{
    class LinearSystemProvider
    {
        private Random _rnd = new Random();

        public double[,] BuildMatrix(int length)
        {
            double[,] a = new double[length, length];
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    a[i, j] = _rnd.NextDouble();
            return a;
        }

        public double[,] BuildVector(int length)
        {
            double[,] x = new double[length, 1];
            for(int i = 0; i < length; i++)
                x[i, 0] = _rnd.NextDouble();
            return x;
        }
    }
}
