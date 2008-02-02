using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using PerformanceAnalysis.FFT;
using PerformanceAnalysis.LinearAlgebra;
using MathNet.Numerics.Transformations;
using MathNet.Numerics.LinearAlgebra;

namespace PerformanceAnalysis
{
    class Program
    {

        static void Main(string[] args)
        {
            //Console.ReadKey();
            //{
            //    Console.WriteLine("=== FFT ANALYSIS ===");
            //    Console.WriteLine("Building Samples...");

            //    int[] sizes = new int[] { 1024, 4096, 16384, 65536, 262144, 1048576, 2097152 };
            //    FftSetProvider provider = new FftSetProvider();
            //    double[][] samples = new double[sizes.Length][];
            //    for(int i = 0; i < sizes.Length; i++)
            //        samples[i] = provider.BuildSamples(sizes[i]);
            //    provider = null;

            //    Console.WriteLine("Warm Up...");

            //    RealFourierTransformation rft = new RealFourierTransformation();
            //    double[] freqReal, freqImag, samplesOut;

            //    rft.TransformForward(samples[sizes.Length - 1], out freqReal, out freqImag);
            //    rft.TransformBackward(freqReal, freqImag, out samplesOut);

            //    Console.WriteLine("Timing...");

            //    Stopwatch stopwatch = new Stopwatch();

            //    for(int i = 0; i < sizes.Length; i++)
            //    {
            //        double[] data = samples[i];
            //        stopwatch.Reset();

            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);

            //        stopwatch.Start();
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        stopwatch.Stop();

            //        Console.WriteLine("  " + sizes[i].ToString() + ": " + (stopwatch.ElapsedMilliseconds / 5).ToString() + " ms");
            //    }

            //    rft = null;
            //}
            //Console.ReadKey();
            //{
            //    Console.WriteLine("=== FFT ANALYSIS ===");

            //    int[] sizes = new int[] { 1024, 4096, 16384, 65536, 262144, 1048576, 2097152 };
            //    FftSetProvider provider = new FftSetProvider();
            //    RealFourierTransformation rft = new RealFourierTransformation();

            //    Stopwatch stopwatch = new Stopwatch();

            //    for(int i = 0; i < sizes.Length; i++)
            //    {
            //        double[] data = provider.BuildSamples(sizes[i]);
            //        double[] freqReal, freqImag, samplesOut;
            //        stopwatch.Reset();

            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);

            //        stopwatch.Start();
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        rft.TransformForward(data, out freqReal, out freqImag);
            //        stopwatch.Stop();

            //        Console.WriteLine("  " + sizes[i].ToString() + ": " + (stopwatch.ElapsedMilliseconds / 5).ToString() + " ms");
            //    }

            //    rft = null;
            //}
            Console.ReadKey();
            {
                Console.WriteLine("=== LINEAR ALGEBRA ===");
                Console.WriteLine("Building Samples...");

                int[] sizes = new int[] { 100, 200, 400, 600, 800, 1000 };
                LinearSystemProvider provider = new LinearSystemProvider();

                double[][,] sampleMatrices = new double[sizes.Length][,];
                double[][,] sampleVectors = new double[sizes.Length][,];
                for(int i = 0; i < sizes.Length; i++)
                {
                    sampleMatrices[i] = provider.BuildMatrix(sizes[i]);
                    sampleVectors[i] = provider.BuildVector(sizes[i]);
                }
                provider = null;

                Console.WriteLine("Warm Up...");

                Matrix maw = Matrix.Create(sampleMatrices[sizes.Length - 1]);
                Matrix mxw = Matrix.Create(sampleVectors[sizes.Length - 1]);

                maw.Solve(mxw);

                Console.WriteLine("Timing...");

                Stopwatch stopwatch = new Stopwatch();

                for(int i = 0; i < sizes.Length; i++)
                {
                    Matrix ma = Matrix.Create(sampleMatrices[i]);
                    Matrix mx = Matrix.Create(sampleVectors[i]);
                    stopwatch.Reset();

                    ma.Solve(mx);
                    ma.Solve(mx);

                    stopwatch.Start();
                    ma.Solve(mx);
                    ma.Solve(mx);
                    ma.Solve(mx);
                    ma.Solve(mx);
                    ma.Solve(mx);
                    stopwatch.Stop();

                    Console.WriteLine("  " + sizes[i].ToString() + ": " + (stopwatch.ElapsedMilliseconds / 5).ToString() + " ms");
                }
            }

            Console.WriteLine();
            Console.WriteLine("=== END ===");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
