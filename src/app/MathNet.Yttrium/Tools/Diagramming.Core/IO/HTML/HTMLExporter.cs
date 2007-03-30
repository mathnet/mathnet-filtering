using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Netron.GraphLib.UI;
using System.Reflection;
namespace Netron.GraphLib.IO.HTML
{
	/// <summary>
	/// Exports a diagram to HTML
	/// includes the diagram as an image with an
	/// imagemap if there are URL's included on shapes.
	/// </summary>
	public class HTMLExporter
	{

		/// <summary>
		/// the graph control
		/// </summary>
		private GraphControl mSite;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="site"></param>
		public HTMLExporter(GraphControl site)
		{
			mSite = site;
		}


		/// <summary>
		/// Creates the necessary files and directories for the export
		/// </summary>
		/// <param name="filePath"></param>
		private void CreateDirAndFiles(string filePath)
		{
			if(!Directory.Exists(Path.GetDirectoryName(filePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}
			if(!Directory.Exists(Path.GetDirectoryName(filePath) + "\\images"))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "\\images");
			}
			Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.GraphLib.Resources.Stripes.gif");
			Bitmap bmp= Bitmap.FromStream(stream) as Bitmap;
			bmp.Save(Path.GetDirectoryName(filePath) + "\\images\\Stripes.gif",ImageFormat.Gif);
			stream.Close();
			stream=null;
/*
			Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.GraphLib.Resources.GradLeft.jpg");
			Bitmap bmp= Bitmap.FromStream(stream) as Bitmap;
			bmp.Save(Path.GetDirectoryName(filePath) + "\\images\\GradLeft.jpg",ImageFormat.Jpeg);
			stream.Close();
			stream=null;

			stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.GraphLib.Resources.GradTop.jpg");
			bmp= Bitmap.FromStream(stream) as Bitmap;
			bmp.Save(Path.GetDirectoryName(filePath) + "\\images\\GradTop.jpg",ImageFormat.Jpeg);
			stream.Close();
			stream=null;
*/
		}

		/// <summary>
		/// Creates the image-map for the clickable areas related to the URL on the shapes
		/// </summary>
		/// <returns></returns>
		private string GetURLMap()
		{
			StringBuilder sb = new StringBuilder();
			string template = @"<area shape=""rect"" coords=""{0},{1},{2},{3}"" href=""{4}"" target=""_blank"" title=""{4}"" >";
			foreach(Shape sh in mSite.Abstract.Shapes)
			{
				if(sh.URL!=string.Empty)
				{
					sb.AppendFormat(template,Convert.ToInt32(sh.Rectangle.Right-16),Convert.ToInt32(sh.Rectangle.Bottom-16),Convert.ToInt32(sh.Rectangle.Right), Convert.ToInt32(sh.Rectangle.Bottom),sh.URL);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Saves an HTML version of the diagram to the given file
		/// </summary>
		/// <param name="filePath">a path</param>
		public void SaveAs(string filePath)
		{
			CreateDirAndFiles(filePath);

			StreamWriter sw = null;
			try
			{
				GraphInformation info = mSite.Abstract.GraphInformation;
				Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.GraphLib.Resources.DefaultHTML.htm");
				StreamReader reader = new StreamReader(stream,System.Text.Encoding.ASCII);
				string template = reader.ReadToEnd();
				
				stream.Close();
				
				stream=null;

				sw = new StreamWriter(filePath);

				if(info.Title==string.Empty)
					template = template.Replace("$title$", "A Netron diagram");
				else
					template = template.Replace("$title$",info.Title);

				if(info.Description==string.Empty)
					template = template.Replace("$description$","Not set");
				else
					template = template.Replace("$description$",info.Description);
				
				if(info.Author==string.Empty)
					template = template.Replace("$author$", "Not set");
				else
					template = template.Replace("$author$",info.Author);

				if(info.Subject==string.Empty)
					template = template.Replace("$subject$", "Not set");
				else
					template = template.Replace("$subject$",info.Subject);
				
				template = template.Replace("$creationdate$",info.CreationDate);
				
				if(mSite.FileName==string.Empty)
					template = template.Replace("$filepath$", "Unsaved diagram");
				else
					template = template.Replace("$filepath$",mSite.FileName);

				string imagename = Path.ChangeExtension(Path.GetFileName(filePath),"jpg");

				mSite.SaveImage(Path.GetDirectoryName(filePath) + "\\images\\" + imagename, true);


				template = template.Replace("$imagepath$","images\\" + imagename);

				template = template.Replace("$urlmap$",GetURLMap());
				

				sw.Write(template);
				sw.Close();
			}
			catch(Exception exc)
			{
				mSite.OutputInfo(exc.Message,OutputInfoLevels.Exception);
				if(sw!=null) sw.Close();
			}
		}
	}
}
