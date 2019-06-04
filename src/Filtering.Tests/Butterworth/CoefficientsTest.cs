using MathNet.Filtering.Butterworth;
using NUnit.Framework;
using System.Linq;

namespace MathNet.Filtering.Tests.Butterworth
{
    [TestFixture]
    public class CoefficientsTest
    {
        private const double _Tolerance = 10e-16;

        private const double _SamplingFrequency = 20000;

        [Test]
        public void LowPass()
        {
            const double passbandFreq = 2000 / _SamplingFrequency;
            const double stopbandFreq = 2500 / _SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (eb, ea) = IirCoefficients.LowPass(passbandFreq, stopbandFreq, passbandRipple, stopbandAttenuation);

            var b = eb.ToArray();
            var a = ea.ToArray();

            var desiredB = new double[] { 0.09723679451225617, 0.09723679451225617 };
            var desiredA = new double[] { 1, -0.8055264109754877 };

            Assert.AreEqual(desiredB.Length, b.Length);
            Assert.AreEqual(desiredA.Length, a.Length);

            for (int i = 0; i < b.Length; i++)
            {
                Assert.AreEqual(desiredB[i], b[i], _Tolerance);
            }

            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(desiredA[i], a[i], _Tolerance);
            }
        }

        [Test]
        public void HighPass()
        {
            const double passbandFreq = 2500 / _SamplingFrequency;
            const double stopbandFreq = 2000 / _SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (eb, ea) = IirCoefficients.HighPass(stopbandFreq, passbandFreq, passbandRipple, stopbandAttenuation);

            var b = eb.ToArray();
            var a = ea.ToArray();

            var desiredB = new double[] { 0.7736977585189119, -0.7736977585189119 };
            var desiredA = new double[] { 1, -0.5473955170378236 };

            Assert.AreEqual(desiredB.Length, b.Length);
            Assert.AreEqual(desiredA.Length, a.Length);

            for (int i = 0; i < b.Length; i++)
            {
                Assert.AreEqual(desiredB[i], b[i], _Tolerance);
            }

            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(desiredA[i], a[i], _Tolerance);
            }
        }

        [Test]
        public void BandPass()
        {
            const double lowPassbandFreq = 2500 / _SamplingFrequency;
            const double lowStopbandFreq = 2000 / _SamplingFrequency;
            const double highPassbandFreq = 3000 / _SamplingFrequency;
            const double highStopbandFreq = 3500 / _SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (eb, ea) = IirCoefficients.BandPass(lowStopbandFreq, lowPassbandFreq, highPassbandFreq, highStopbandFreq, passbandRipple, stopbandAttenuation);

            var b = eb.ToArray();
            var a = ea.ToArray();

            var desiredB = new double[] { 0.02602409840677345, 0, -0.02602409840677345 };
            var desiredA = new double[] { 1, -1.770384034934973, 0.9479518031864529 };

            Assert.AreEqual(desiredB.Length, b.Length);
            Assert.AreEqual(desiredA.Length, a.Length);

            for (int i = 0; i < b.Length; i++)
            {
                Assert.AreEqual(desiredB[i], b[i], _Tolerance);
            }

            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(desiredA[i], a[i], _Tolerance);
            }
        }

        [Test]
        public void BandStop()
        {
            const double lowPassbandFreq = 2000 / _SamplingFrequency;
            const double lowStopbandFreq = 2500 / _SamplingFrequency;
            const double highPassbandFreq = 3500 / _SamplingFrequency;
            const double highStopbandFreq = 3000 / _SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;

            var (eb, ea) = IirCoefficients.BandStop(lowPassbandFreq, lowStopbandFreq, highStopbandFreq, highPassbandFreq, passbandRipple, stopbandAttenuation);

            var b = eb.ToArray();
            var a = ea.ToArray();

            var desiredB = new double[] { 0.8634280116150362, -1.569442492959457, 0.8634280116150361 };
            var desiredA = new double[] { 1, -1.569442492959457, 0.7268560232300724 };

            Assert.AreEqual(desiredB.Length, b.Length);
            Assert.AreEqual(desiredA.Length, a.Length);

            for (int i = 0; i < b.Length; i++)
            {
                Assert.AreEqual(desiredB[i], b[i], _Tolerance);
            }

            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(desiredA[i], a[i], _Tolerance);
            }
        }
    }
}
