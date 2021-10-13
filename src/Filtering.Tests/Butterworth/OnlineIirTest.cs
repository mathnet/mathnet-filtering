using MathNet.Filtering.Butterworth;
using MathNet.Filtering.IIR;
using NUnit.Framework;

namespace MathNet.Filtering.Tests.Butterworth
{
    [TestFixture]
    public class OnlineIirTest
    {
        private const double Tolerance = 10e-14;

        private const double SamplingFrequency = 20000;

        private int InputDataLength => InputData.Length;
        private readonly double[] InputData = { 0.228897927516298, -2.619954349660920,
                                                -17.502123684467897, -2.856509715953298,
                                                -8.313665115676244, -9.792063051673022,
                                                -11.564016556640022, -5.335571093159874,
                                                -20.026357358830605, 9.642294226316274 };


        [Test]
        public void LowPass()
        {
            const double passbandFreq = 2000 / SamplingFrequency;
            const double stopbandFreq = 2500 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;
            double[] ExpectedOutputData = {0.022257300742183576,
                                          -0.21456981840243389,
                                          -2.1294480226855419 ,
                                          -3.6949348755798441 ,
                                          -4.06251962388313   ,
                                          -5.0330098214473757 ,
                                          -6.1308190623358048 ,
                                          -6.58179840726892   ,
                                          -7.7679250740922559 ,
                                          -7.2669818186786257 };

            OnlineIirFilter filter = OnlineIirButterworthFilter.LowPass(passbandFreq, stopbandFreq, passbandRipple, stopbandAttenuation);

            double[] output = RunFilter(filter);
            Assert.That(output, Is.EqualTo(ExpectedOutputData).Within(Tolerance));
        }

        [Test]
        public void HighPass()
        {
            const double passbandFreq = 2500 / SamplingFrequency;
            const double stopbandFreq = 2000 / SamplingFrequency;
            const double passbandRipple = 5;
            const double stopbandAttenuation = 6;
            double[] ExpectedOutputData = { 0.17709781344898412, -2.1072080720443367,
                -12.667777308342028, 4.3969941901531158,
                -1.8152939925238485, -2.1375169628929394,
                -2.5410236580725973, 3.4279893350358845,
                -9.4897624101777485, 17.759915828621352 };

            OnlineIirFilter filter = OnlineIirButterworthFilter.HighPass(stopbandFreq, passbandFreq, passbandRipple, stopbandAttenuation);

            double[] output = RunFilter(filter);
            Assert.That(output, Is.EqualTo(ExpectedOutputData).Within(Tolerance));
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
            double[] ExpectedOutputData = {0.0059568621907906414,
                                          -0.057636016095946464,
                                          -0.56911855227151587,
                                          -0.95907837367120408,
                                          -0.9193187331454562 ,
                                          -0.89887865697280822,
                                          -0.80447803861636846,
                                          -0.45616524702168815,
                                          -0.2652060530769399 ,
                                          0.35269154712711837};

            OnlineIirFilter filter = OnlineIirButterworthFilter.BandPass(lowStopbandFreq, lowPassbandFreq, highPassbandFreq, highStopbandFreq, passbandRipple, stopbandAttenuation);
            double[] output = RunFilter(filter);
            Assert.That(output, Is.EqualTo(ExpectedOutputData).Within(Tolerance));

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
            double[] ExpectedOutputData = { 0.19763688241819988,
                                            -2.3112043872011712,
                                            -14.573275217116462,
                                            1.5480395909989819,
                                            -4.7847154775654985,
                                            -6.5078504178710119,
                                            -8.5307650341308729,
                                            -3.5708389643430767,
                                            -18.305730196535904,
                                            9.0144565837006585};

            OnlineIirFilter filter = OnlineIirButterworthFilter.BandStop(lowPassbandFreq, lowStopbandFreq, highStopbandFreq, highPassbandFreq, passbandRipple, stopbandAttenuation);
            double[] output = RunFilter(filter);
            Assert.That(output, Is.EqualTo(ExpectedOutputData).Within(Tolerance));

        }

        protected double[] RunFilter(OnlineIirFilter filter)
        {
            double[] output = new double[InputDataLength];
            for (int i = 0; i < InputDataLength; i++)
            {
                output[i] = filter.ProcessSample(InputData[i]);
                //double outputSample = filter.ProcessSample(InputData[i]);
                //Assert.AreEqual(outputSample, ExpectedOutputData[i], Tolerance);
            }
            return output;
        }
    }
}
