using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
namespace Netron.GraphLib.IO.Binary
{
	/// <summary>
	/// Encapsulates the GraphAbstract and additional ambient properties of the GraphControl
	/// for binary (de)serialization.
	/// </summary>
	[Serializable] class BinaryCapsule : ISerializable
	{
		#region Fields
		/// <summary>
		/// the actual diagram
		/// </summary>
		private GraphAbstract mGraphAbstract;
		/// <summary>
		/// the ambiant properties
		/// </summary>
		private BinaryAmbiance mBinaryAmbiance;
		/// <summary>
		/// the thumbnail
		/// </summary>
		private Image mThumbnail;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the thumbnail of the diagram
		/// </summary>
		public Image Thumbnail
		{
			get{return mThumbnail;}
			set{mThumbnail = value;}
		}
		/// <summary>
		/// Gets or sets the GraphAbstract
		/// </summary>
		public GraphAbstract Abstract
		{
			get{return mGraphAbstract;}
			set{mGraphAbstract = value;}
		}
		/// <summary>
		/// Gets or sets the ambiance properties
		/// </summary>
		public BinaryAmbiance Ambiance
		{
			get{return mBinaryAmbiance;}
			set{mBinaryAmbiance = value;}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Default Constructor
		/// </summary>
		public BinaryCapsule()
		{
			
		}
		protected  BinaryCapsule(SerializationInfo info, StreamingContext context) 
		{
			try
			{
				this.mBinaryAmbiance = info.GetValue("mBinaryAmbiance", typeof(BinaryAmbiance)) as BinaryAmbiance;
			}
			catch(Exception exc)
			{
				Trace.WriteLine(exc.Message, "BinaryCapsule.DeserializationConstructor");
				//trying to recover the old binaries
				this.mBinaryAmbiance = info.GetValue("mBinaryAmbience", typeof(BinaryAmbiance)) as BinaryAmbiance;
			}
			this.mGraphAbstract = info.GetValue("mGraphAbstract", typeof(GraphAbstract)) as GraphAbstract;
			this.mThumbnail = info.GetValue("mThumbnail", typeof(Image)) as Image;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="graphAbstract"></param>
		/// <param name="ambiance"></param>
		public BinaryCapsule(GraphAbstract graphAbstract, BinaryAmbiance ambiance)
		{
			this.mGraphAbstract = graphAbstract;
			this.mBinaryAmbiance = ambiance;
		}
		#endregion



		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("mGraphAbstract",this.mGraphAbstract);
			info.AddValue("mBinaryAmbiance", this.mBinaryAmbiance);
			info.AddValue("mThumbnail", this.mThumbnail);

		}

	}
}
