using MathNet.Filtering.Median;
using NUnit.Framework;

namespace MathNet.Filtering.UnitTests
{
    /// <summary>
    /// OnlineMedian test.
    /// </summary>
    [TestFixture]
    public class OnlineMedianFilterTest
    {
        /// <summary>
        /// Naive transforms real sine correctly.
        /// </summary>
        [Test]
        public void MedianWindowFilling()
        {
            var filter = new OnlineMedianFilter(5);

            Assert.AreEqual(5, filter.ProcessSample(5));
            Assert.AreEqual(3.5, filter.ProcessSample(2));
            Assert.AreEqual(5, filter.ProcessSample(6));
            Assert.AreEqual(4, filter.ProcessSample(3));
            Assert.AreEqual(4, filter.ProcessSample(4));
            Assert.AreEqual(3, filter.ProcessSample(1));
            Assert.AreEqual(4, filter.ProcessSample(7));
        }
    }
}
