using System;

namespace MathNet.SignalProcessing.DataSources
{
    /// <summary>
    /// Generators for sinusidual and theoretical signal vectors. 
    /// </summary>
    public class SignalGenerator
    {
        /// <summary>
        /// Create a Sine Signal Sample Vector.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="frequency">Frequency in samples per unit.</param>
        /// <param name="phase">Optional phase offset.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Sine(double samplingRate, double frequency, double phase, double amplitude, int length)
        {
            double[] data = new double[length];
            double step = frequency / samplingRate * 2 * Math.PI;
            for(int i = 0; i < length; i++)
                data[i] = amplitude * Math.Sin(phase + i * step);
            return data;
        }

        /// <summary>
        /// Create a Heaviside Step Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Step(int offset, double amplitude, int length)
        {
            double[] data = new double[length];
            int cursor;
            for(cursor = 0; cursor < offset && cursor < length; cursor++)
                data[cursor] = 0d;
            for(; cursor < length; cursor++)
                data[cursor] = amplitude;
            return data;
        }

        /// <summary>
        /// Create a Dirac Delta Impulse Signal Sample Vector.
        /// </summary>
        /// <param name="offset">Offset to the time axis. Zero or positive.</param>
        /// <param name="frequency">impulse sequence frequency. -1 for single impulse only.</param>
        /// <param name="amplitude">The maximal reached peak.</param>
        /// <param name="length">The count of samples to generate.</param>
        public static double[] Impulse(int offset, int frequency, double amplitude, int length)
        {
            double[] data = new double[length];
            if(frequency <= 0)
                data[offset] = amplitude;
            else
                while(offset < length)
                {
                    data[offset] = amplitude;
                    offset += frequency;
                }
            return data;
        }
    }
}
