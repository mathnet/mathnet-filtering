using System;

namespace Netron.GraphLib.IO.NML
{
	/// <summary>
	/// Helps (de)serialize custom collections in shapes.
	/// </summary>
	public interface INMLCollection
	{
		public INMLCollection()
		{
			string WriteXml();
			void ReadXml(string content);
		}
	}
}
