// <copyright file="SinusoidalSource.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using MathNet.Filtering.Channel;
using MathNet.Numerics;

namespace MathNet.Filtering.DataSources
{
    /// <summary>
    /// Sinus sample source.
    /// </summary>
    public class SinusoidalSource : IChannelSource
    {
        readonly int _delay;
        readonly double _amplitude;
        readonly double _mean;
        readonly double _phaseStep;
        double _nextPhase;

        /// <summary>
        /// Create a new on-demand sinus sample source with the given parameters.
        /// </summary>
        public SinusoidalSource(double samplingRate, double frequency, double amplitude, double phase, double mean, int delay)
        {
            _delay = delay;
            _mean = mean;
            _amplitude = amplitude;
            _phaseStep = frequency/samplingRate*Constants.Pi2;
            _nextPhase = phase - delay*_phaseStep;
        }

        /// <summary>
        /// Create a new on-demand sinus sample source with the given parameters an zero phase and mean.
        /// </summary>
        public SinusoidalSource(double samplingRate, double frequency, double amplitude)
            : this(samplingRate, frequency, amplitude, 0.0, 0.0, 0)
        {
        }

        /// <summary>
        /// Creates a precomputed sinus sample source with the given parameters and zero mean.
        /// </summary>
        public static IChannelSource Precompute(int samplesPerPeriod, double amplitude, double phase)
        {
            double[] samples = SignalGenerator.Sine(
                samplesPerPeriod, // samplingRate
                1.0, // frequency
                phase,
                amplitude,
                samplesPerPeriod); // length

            return new ArbitraryPeriodicSource(samples);
        }

        /// <summary>
        /// Creates a precomputed sinus sample source with the given parameters and zero phase and mean.
        /// </summary>
        public static IChannelSource Precompute(int samplesPerPeriod, double amplitude)
        {
            return Precompute(samplesPerPeriod, amplitude, 0.0);
        }

        /// <summary>
        /// Computes and returns the next sample.
        /// </summary>
        public double ReadNextSample()
        {
            double sample = _mean + _amplitude*Math.Sin(_nextPhase);
            _nextPhase += _phaseStep;
            double pi2 = Constants.Pi2;

            if (_nextPhase > pi2)
            {
                _nextPhase -= pi2;
            }

            return sample;
        }

        /// <summary>
        /// Sample delay of this source in relation to the whole system.
        /// </summary>
        public int Delay
        {
            get { return _delay; }
        }
    }
}
