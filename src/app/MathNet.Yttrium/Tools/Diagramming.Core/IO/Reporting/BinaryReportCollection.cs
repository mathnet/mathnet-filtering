using System;
using System.Collections;
namespace Netron.GraphLib.IO.Reporting
{
	/// <summary>
	/// STC of BinaryReport objects
	/// </summary>
	public class BinaryReportCollection : CollectionBase
	{

		/// <summary>
		/// Adds a report to the collection
		/// </summary>
		/// <param name="report"></param>
		/// <returns></returns>
		public int Add(BinaryReport report)
		{
			return this.InnerList.Add(report);
		}

		/// <summary>
		/// Integer indexer
		/// </summary>
		public BinaryReport this[int index]
		{
			get{
				return this.InnerList[index] as BinaryReport;
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public BinaryReportCollection()
		{
			
		}
	}
}
