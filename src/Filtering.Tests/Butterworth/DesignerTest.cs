using MathNet.Filtering.Butterworth;
using NUnit.Framework;

namespace MathNet.Filtering.Tests.Butterworth
{
    [TestFixture]
    public class DesignerTest
    {
        private const double Tolerance = 10e-16;

        private const double SamplingFrequency = 20000;

        [Test]
        public void LowPass()
        {
            const double passbandFreq = 2000 / SamplingFrequency;
            const double stopbandFreq = 2500 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (n, w) = Designer.LowPass(passbandFreq, stopbandFreq, passbandRipple, stopbandAttenuation);

            const int expectedOrder = 1;
            const double expectedCutoff = 0.06830709304786786;

            Assert.AreEqual(expectedOrder, n);
            Assert.AreEqual(expectedCutoff, w, Tolerance);
        }

        [Test]
        public void HighPass()
        {
            const double passbandFreq = 2500 / SamplingFrequency;
            const double stopbandFreq = 2000 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (n, w) = Designer.HighPass(stopbandFreq, passbandFreq, passbandRipple, stopbandAttenuation);

            const int expectedOrder = 1;
            const double expectedCutoff = 0.1811544562026703;

            Assert.AreEqual(expectedOrder, n);
            Assert.AreEqual(expectedCutoff, w, Tolerance);
        }

        [Test]
        public void BandPass()
        {
            const double lowPassbandFreq = 2500 / SamplingFrequency;
            const double lowStopbandFreq = 2000 / SamplingFrequency;
            const double highPassbandFreq = 3000 / SamplingFrequency;
            const double highStopbandFreq = 3500 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (n, w1, w2) = Designer.BandPass(lowStopbandFreq, lowPassbandFreq, highPassbandFreq, highStopbandFreq, passbandRipple, stopbandAttenuation);

            const int expectedOrder = 1;
            const double expectedCutoffLow = 0.1287104733860468;
            const double expectedCutoffHigh = 0.1457165559830869;

            Assert.AreEqual(expectedOrder, n);
            Assert.AreEqual(expectedCutoffLow, w1, Tolerance);
            Assert.AreEqual(expectedCutoffHigh, w2, Tolerance);
        }

        [Test]
        public void BandStop()
        {
            const double lowPassbandFreq = 2000 / SamplingFrequency;
            const double lowStopbandFreq = 2500 / SamplingFrequency;
            const double highPassbandFreq = 3500 / SamplingFrequency;
            const double highStopbandFreq = 3000 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (n, w1, w2) = Designer.BandStop(lowPassbandFreq, lowStopbandFreq, highStopbandFreq, highPassbandFreq, passbandRipple, stopbandAttenuation);

            const int expectedOrder = 1;
            const double expectedCutoffLow = 0.0953139584811423;
            const double expectedCutoffHigh = 0.19518335294151;

            Assert.AreEqual(expectedOrder, n);
            Assert.AreEqual(expectedCutoffLow, w1, Tolerance);
            Assert.AreEqual(expectedCutoffHigh, w2, Tolerance);
        }
    }
}
