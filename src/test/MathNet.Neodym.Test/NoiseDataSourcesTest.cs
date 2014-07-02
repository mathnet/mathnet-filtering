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
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NUnit.Framework;
using MathNet.SignalProcessing.DataSources;
using MathNet.SignalProcessing.Channel;

namespace Neodym.Test
{
    [TestFixture]
    public class NoiseDataSourcesTest
    {
        const int SignalLengthWidth = 16;
        const int GroupLengthWidth = 8;

        const int SignalLength = 1 << SignalLengthWidth;
        const int GroupLength = 1 << GroupLengthWidth;
        const int GroupCount = 1 << (SignalLengthWidth - GroupLengthWidth);

        [Test]
        public void TestWhiteGaussianNoiseIsWhite()
        {
            for(int z = 0; z < 10; z++) // run test 10 times to be sure
            {
                IChannelSource source = new WhiteGaussianNoiseSource();

                Complex[] signal = new Complex[SignalLength];
                double signalPower = 0d;
                for(int i = 0; i < signal.Length; i++)
                {
                    signal[i] = source.ReadNextSample();
                    signalPower += Math.Pow(signal[i].Magnitude, 2.0);
                }
                signalPower /= SignalLength;
                Assert.Less(signalPower, 1.1, "signal power must be less than 1.1");
                Assert.Greater(signalPower, 0.9, "signal power must be greater than 0.9");

                Fourier.Forward(signal);

                double spectralPower = 0d;
                double[] spectralGroupPower = new double[GroupCount];
                for(int i = 0, j = 0; i < spectralGroupPower.Length; i++, j += GroupLength)
                {
                    double sum = 0d;
                    for(int k = 0; k < GroupLength; k++)
                    {
                        sum += Math.Pow(signal[j + k].Magnitude, 2.0);
                    }
                    spectralGroupPower[i] = sum / GroupLength;
                    spectralPower += sum;

                    Assert.Less(spectralGroupPower[i], 1.4, "spectral power must be less than 1.4 for each group");
                    Assert.Greater(spectralGroupPower[i], 0.6, "spectral power must be greater than 0.6 for each group");
                }
                spectralPower /= SignalLength;
                Assert.AreEqual(signalPower, spectralPower, 0.0001, "Signal and spectral power must match.");
            }
        }
    }
}
