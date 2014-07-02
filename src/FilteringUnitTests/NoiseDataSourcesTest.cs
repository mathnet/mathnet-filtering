using System;
using System.Numerics;
using MathNet.Filtering.Channel;
using MathNet.Filtering.DataSources;
using MathNet.Numerics.IntegralTransforms;
using NUnit.Framework;

namespace MathNet.Filtering.UnitTests
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
