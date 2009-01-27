#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2008, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using MathNet.SignalProcessing;
using MathNet.SignalProcessing.DataSources;
using MathNet.SignalProcessing.Channel;
using MathNet.Numerics;
using MathNet.Numerics.Transformations;

namespace Neodym.Test
{
    [TestFixture]
    public class NoiseDataSourcesTest
    {
        const int signalLengthWidth = 16;
        const int groupLengthWidth = 8;

        const int signalLength = 1 << signalLengthWidth;
        const int groupLength = 1 << groupLengthWidth;
        const int groupCount = 1 << (signalLengthWidth - groupLengthWidth);

        [Test]
        public void TestWhiteGaussianNoiseIsWhite()
        {
            for(int z = 0; z < 10; z++) // run test 10 times to be sure
            {
                IChannelSource source = new WhiteGaussianNoiseSource();

                double[] signal = new double[signalLength];
                double signalPower = 0d;
                for(int i = 0; i < signal.Length; i++)
                {
                    signal[i] = source.ReadNextSample();
                    signalPower += signal[i] * signal[i];
                }
                signalPower /= signalLength;
                Assert.Less(signalPower, 1.1, "signal power must be less than 1.1");
                Assert.Greater(signalPower, 0.9, "signal power must be greater than 0.9");

                double[] freqReal, freqImag;
                RealFourierTransformation fft = new RealFourierTransformation();
                fft.TransformForward(signal, out freqReal, out freqImag);

                double spectralPower = 0d;
                double[] spectralGroupPower = new double[groupCount];
                for(int i = 0, j = 0; i < spectralGroupPower.Length; i++, j += groupLength)
                {
                    double sum = 0d;
                    for(int k = 0; k < groupLength; k++)
                    {
                        double real = freqReal[j + k];
                        double imag = freqImag[j + k];
                        sum += real * real + imag * imag;
                    }
                    spectralGroupPower[i] = sum / (2 * groupLength);
                    spectralPower += sum;

                    Assert.Less(spectralGroupPower[i], 1.4, "spectral power must be less than 1.4 for each group");
                    Assert.Greater(spectralGroupPower[i], 0.6, "spectral power must be greater than 0.6 for each group");
                }
                spectralPower /= (2 * signalLength);
                Assert.AreEqual(signalPower, spectralPower, 0.0001, "Signal and spectral power must match.");
            }
        }
    }
}
