using System;
using System.IO;
using Netron.GraphLib.Interfaces;
using Netron.GraphLib.IO.Binary;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
namespace Netron.GraphLib.IO.Reporting
{
	/// <summary>
	/// Summary description for BinaryReporter.
	/// </summary>
	public class BinaryReporter : IReporter
	{

		/// <summary>
		/// Occurs when a report is found and added to the collection
		/// </summary>
		public event InfoDelegate OnReport;

		#region Fields
		private string path;
		#endregion

		#region Properties
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">the path to either a binary saved diagram or a directory, in the latter case the report will contain a
		/// collection of reports</param>
		public BinaryReporter(string path)
		{
			this.path = path;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a BinaryReport or a collection of BinaryReport objects depending on whether the given path is a file or a directory
		/// </summary>
		/// <returns></returns>
		public object Report()
		{
			if(Directory.Exists(path)) //directory
					return GetReports();
			else
				return GetReport(path);
		}

		/// <summary>
		/// Assuming the given path is a directory this method will return a 
		/// collection of BinaryReports of the binary diagram in the directory
		/// </summary>
		/// <returns></returns>
		private BinaryReportCollection GetReports()
		{
			string[] files = Directory.GetFiles(path,"*.netron");
			BinaryReportCollection col = new BinaryReportCollection();
			BinaryReport report;
			for(int k=0; k<files.Length; k++)
			{
				report = GetReport(files[k]);
				if(report!=null) col.Add(report);
			}
			return col;
		}

		/// <summary>
		/// Returns a BinaryReport of the diagram save in the given path/file
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private BinaryReport GetReport(string filePath)
		{
			BinaryReport report = new BinaryReport();
			BinaryCapsule capsule = LoadCapsule(filePath);
			if(capsule!=null)
			{
				
				report.Thumbnail = capsule.Thumbnail;
				GraphInformation info = capsule.Abstract.GraphInformation;
				report.Author = info.Author;
				report.CreationDate = info.CreationDate;
				report.Description = info.Description;
				report.FileSize = new System.IO.FileInfo(filePath).Length;
				report.Path = filePath;
				report.Subject = info.Subject;
				report.Title = info.Title;
				if(OnReport!=null) OnReport(report,OutputInfoLevels.Info);
				return report;
			}
			else
				return null;
		}

		private BinaryCapsule LoadCapsule(string fileName)
		{
			FileStream fs=null;
					
			try
			{
				fs= File.OpenRead(fileName);
			}
			catch (System.IO.DirectoryNotFoundException exc)
			{
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch(System.IO.FileLoadException exc)
			{				
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch (System.IO.FileNotFoundException exc)
			{
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch
			{				
				Trace.WriteLine("Non-CLS exception caught.","BinarySerializer.SaveAs");
			}
			//donnot open anything if filestream is not there
			if (fs==null) return null;
			try
			{
						
				BinaryFormatter f = new BinaryFormatter();
				
				BinaryCapsule capsule = (BinaryCapsule) f.Deserialize(fs); 						
				
				return capsule;
			}
			catch(System.Runtime.Serialization.SerializationException exc)
			{
				Trace.WriteLine(exc.Message,"BinaryReporter.LoadCapsule");
				return null;
			}
			catch(Exception exc)
			{
				Trace.WriteLine(exc.Message,"BinaryReporter.LoadCapsule");
				return null;
			}

		}

		#endregion
		
	}
}
