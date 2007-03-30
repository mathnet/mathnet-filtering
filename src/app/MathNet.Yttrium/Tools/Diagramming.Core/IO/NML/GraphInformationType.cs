using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;
    
namespace Netron.GraphLib.IO.NML {    
    
    /// <summary>
    /// Generic XML wrapper for any diagram data
    /// </summary>
    [XmlType(IncludeInSchema=true, TypeName="GraphInformation.type")]
    [XmlRoot(ElementName="GraphInformation", IsNullable=false, DataType="")]
    public class GraphInformationType {

		#region Fields
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
		/// Gets or sets the description of the graph
		/// </summary>
		[XmlElement("Description",typeof(string))]
		public string Description
		{
			get{return mDescription;}
			set{mDescription = value;}
		}

		/// <summary>
		/// Gets or sets the author of the graph
		/// </summary>
		[XmlElement("Author",typeof(string))]
		public string Author
		{
			get{return mAuthor;}
			set{mAuthor = value;}
		}
		/// <summary>
		/// Gets or sets the creation date of the graph
		/// </summary>
		[XmlElement("CreationDate",typeof(string))]
		public string CreationDate
		{
			get{return mCreationDate;}			
			set{mCreationDate = value;}
		}
		/// <summary>
		/// Gets or sets the subject of the graph
		/// </summary>
		[XmlElement("Subject",typeof(string))]
		public string Subject
		{
			get{return mSubject;}
			set{mSubject = value;}
		}
		/// <summary>
		/// Gets or sets the title of the graph
		/// </summary>
		[XmlElement("Title",typeof(string))]
		public string Title
		{
			get{return mTitle;}
			set{mTitle = value;}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Required XMLSerilization constructor
		/// </summary>
		public GraphInformationType(){}
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="info"></param>		
		public GraphInformationType(GraphInformation info)
		{
			this.mAuthor = info.Author;
			this.mCreationDate = info.CreationDate;
			this.mDescription = info.Description;
			this.mSubject = info.Subject;
			this.mTitle = info.Title;			
		}
		

		#endregion


		/// <summary>
		/// Returns this type as a GraphInformation object.
		/// </summary>
		/// <remarks>Used at deserialization</remarks>
		/// <returns></returns>
		public GraphInformation ToGraphInformation()
		{
			GraphInformation gi = new GraphInformation();
			gi.Author = this.Author;
			gi.CreationDate = this.mCreationDate;
			gi.Description =this.mDescription;
			gi.Subject = this.mSubject;
			gi.Title = this.mTitle;
			return gi;
		}

    }
}
