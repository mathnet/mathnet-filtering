using System;

namespace MathNet.SignalProcessing.Filter
{
	/// <summary>
	/// Frequency Filter Type
	/// </summary>
	public enum FilterType
	{
		/// <summary>LowPass, lets only low frequencies pass.</summary>
		LowPass,
		/// <summary>HighPass, lets only high frequencies pass.</summary>
		HighPass,
		/// <summary>BandPass, lets only frequencies pass that are inside of a band.</summary>
		BandPass,
		/// <summary>BandStop, lets only frequencies pass that are outside of a band.</summary>
		BandStop,
		/// <summary>Other behavior.</summary>
		Other
	}
}
