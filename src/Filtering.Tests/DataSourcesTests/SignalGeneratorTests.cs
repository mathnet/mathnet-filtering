using NUnit.Framework;
using MathNet.Filtering.DataSources;

namespace MathNet.Filtering.UnitTests.DataSourcesTests
{
    [TestFixture]
    public class SignalGeneratorTests
    {
        [Test]
        public void SineSignalGeneratorTest()
        {
            // Arrange
            var sampleRate = 44100;
            var frequency = 1000;
            var amplitude = 1;
            var phase = 0;
            var length = 5;

            // Act
            var sineSignalGenerator = SignalGenerator.Sine(sampleRate, frequency, phase, amplitude, length);

            // Assert
            Assert.IsNotNull(sineSignalGenerator);
            CollectionAssert.AllItemsAreNotNull(sineSignalGenerator);
			Assert.That(sineSignalGenerator.Length, Is.EqualTo(length));
        }

		[Test]
		public void StepSignalGeneratorTest()
		{
            // Arrange
            var stepSignalOffset = 0;
            var stepSignalAmplitude = 1;
            var stepSignalLength = 5;

			// Act
            var stepSignalGenerator = SignalGenerator.Step(stepSignalOffset, stepSignalAmplitude, stepSignalLength);

			// Assert
			Assert.IsNotNull(stepSignalGenerator);
            CollectionAssert.AllItemsAreNotNull(stepSignalGenerator);
            Assert.That(stepSignalGenerator.Length, Is.EqualTo(stepSignalLength));
		}

		[Test]
		public void ImpulseSignalGeneratorWithFrequencyGreaterThanZeroTest()
		{
			// Arrange
			var impulseSignalOffset = 1;
			var impulseSignalAmplitude = 1;
            var impulseSignalFrequency = 2;
			var impulseSignalLength = 5;

			// Act
            var impulseSignalGenerator = SignalGenerator.Impulse(impulseSignalOffset, impulseSignalFrequency, impulseSignalAmplitude, impulseSignalLength);

			// Assert
			Assert.IsNotNull(impulseSignalGenerator);
            CollectionAssert.AllItemsAreNotNull(impulseSignalGenerator);
            Assert.That(impulseSignalGenerator.Length, Is.EqualTo(impulseSignalLength));
            Assert.That(impulseSignalGenerator[impulseSignalOffset], Is.EqualTo(impulseSignalAmplitude));
            Assert.That(impulseSignalGenerator[impulseSignalOffset + impulseSignalFrequency], Is.EqualTo(impulseSignalAmplitude));
		}

		[Test]
		public void ImpulseSignalGeneratorWithFrequencyZeroTest()
		{
			// Arrange
			var impulseSignalOffset = 3;
			var impulseSignalAmplitude = 1;
			var impulseSignalFrequency = 0;
			var impulseSignalLength = 5;

			// Act
			var impulseSignalGenerator = SignalGenerator.Impulse(impulseSignalOffset, impulseSignalFrequency, impulseSignalAmplitude, impulseSignalLength);

			// Assert
			Assert.IsNotNull(impulseSignalGenerator);
            CollectionAssert.AllItemsAreNotNull(impulseSignalGenerator);
			Assert.That(impulseSignalGenerator.Length, Is.EqualTo(impulseSignalLength));
            Assert.That(impulseSignalGenerator[impulseSignalOffset], Is.EqualTo(impulseSignalAmplitude));
		}
    }
}
