using System;
using System.Drawing;
namespace Netron.GraphLib.IO.Reporting
{
	/// <summary>
	/// Encapsulates the reporting of a single diagram saved to binary file
	/// </summary>
	public class BinaryReport
	{
		#region Fields

		/// <summary>
		/// the size of the file
		/// </summary>
		private long mSize;
		/// <summary>
		/// the path to tht file
		/// </summary>
		private string mPath;
		/// <summary>
		/// the thumbnail
		/// </summary>
		private Image mThumbnail;
		/// <summary>
		/// the description of the graph
		/// </summary>
		private string mDescription = string.Empty;
		/// <summary>
		/// the author of the graph
		/// </summary>
		private string mAuthor = string.Empty;
		/// <summary>
		/// the creation date of the graph
		/// </summary>
		private string mCreationDate = string.Empty;
		/// <summary>
		/// the subject of the graph
		/// </summary>
		private string mSubject = string.Empty;
		/// <summary>
		/// the title of the graph
		/// </summary>
		private string mTitle = string.Empty;

		#endregion
		
		#region Properties

		/// <summary>
		/// Gets or sets the file-size
		/// </summary>
		public long FileSize
		{
			get{return mSize;}
			set{mSize = value;}
		}
		/// <summary>
		/// Gets or sets the path to the file
		/// </summary>
		public string Path
		{
			get{return mPath;}
			set{mPath = value;}
		}

		/// <summary>
		/// Gets or sets the thumbnail
		/// </summary>
		public Image Thumbnail
		{
			get{return mThumbnail;}
			set{mThumbnail = value;}
		}

		/// <summary>
		/// Gets or sets the description of the graph
		/// </summary>
		public string Description
		{
			get{return mDescription;}
			set{mDescription = value;}
		}

		/// <summary>
		/// Gets or sets the author of the graph
		/// </summary>
		public string Author
		{
			get{return mAuthor;}
			set{mAuthor = value;}
		}
		/// <summary>
		/// Gets or sets the creation date of the graph
		/// </summary>
		public string CreationDate
		{
			get{return mCreationDate;}			
			set{mCreationDate = value;}
		}
		/// <summary>
		/// Gets or sets the subject of the graph
		/// </summary>
		public string Subject
		{
			get{return mSubject;}
			set{mSubject = value;}
		}
		/// <summary>
		/// Gets or sets the title of the graph
		/// </summary>
		public string Title
		{
			get{return mTitle;}
			set{mTitle = value;}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public BinaryReport()
		{
			
		}
		#endregion
	}
}
