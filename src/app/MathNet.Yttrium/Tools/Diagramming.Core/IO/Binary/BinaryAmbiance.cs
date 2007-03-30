using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.Serialization;
namespace Netron.GraphLib.IO.Binary
{
	/// <summary>
	/// Collects ambient properties of the GraphControl
	/// </summary>
	[Serializable] public class BinaryAmbiance : ISerializable
	{

		#region Fields

		/// <summary>
		/// the Locked setting
		/// </summary>
		private bool mLocked;
		/// <summary>
		/// the ShowGrid setting
		/// </summary>
		private bool mShowGrid;
		/// <summary>
		/// the bottom gradient
		/// </summary>
		private Color mGradientBottom;
		/// <summary>
		/// the top gradient
		/// </summary>
		private Color mGradientTop;
		/// <summary>
		/// the AllowAddConnection property
		/// </summary>
		private bool mAllowAddConnection;
		/// <summary>
		/// the AllowAddShape property
		/// </summary>
		private bool mAllowAddShape;
		/// <summary>
		/// the AllowDeleteShape property
		/// </summary>
		private bool mAllowDeleteShape;
		/// <summary>
		/// the AllowMoveShape property
		/// </summary>
		private bool mAllowMoveShape;
		/// <summary>
		/// the AutomataPulse property
		/// </summary>
		private int mAutomataPulse;
		/// <summary>
		/// the BackgroundColor property
		/// </summary>
		private Color mBackgroundColor;
		/// <summary>
		/// the BackgroundImagePath property
		/// </summary>
		private string mBackgroundImagePath;
		/// <summary>
		/// the BackgroundType property
		/// </summary>
		private CanvasBackgroundType mBackgroundType;
		/// <summary>
		/// the DefaultConnectionPath property
		/// </summary>
		private string mDefaultConnectionPath;
		/// <summary>
		/// the GradientMode property
		/// </summary>
		private LinearGradientMode mGradientMode;
		/// <summary>
		/// the EnableContextMenu property
		/// </summary>
		private bool mEnableContextMenu;
		/// <summary>
		/// the GridSize property
		/// </summary>
		private int mGridSize;
		/// <summary>
		/// the RestrictToCanvas property
		/// </summary>
		private bool mRestrictToCanvas;
		/// <summary>
		/// the Snap property
		/// </summary>
		private bool mSnap;		
		/// <summary>
		/// the DefaultConnectionEnd property
		/// </summary>
		private ConnectionEnd mDefaultConnectionEnd ;
		/// <summary>
		/// the mEnableLayout property
		/// </summary>
		private bool mEnableLayout;
		/// <summary>
		/// the GraphLayoutAlgorithm property
		/// </summary>
		private  GraphLayoutAlgorithms mGraphLayoutAlgorithm;		
		/// <summary>
		/// the EnableTooltip property
		/// </summary>
		private bool mEnableTooltip;
		/// <summary>
		/// the ShowAutomataController property
		/// </summary>
		private bool mShowAutomataController;
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets or sets whether the internal dataflow is runnning
		/// </summary>
		public bool ShowAutomataController
		{
			get{return mShowAutomataController;}			
			set{mShowAutomataController = value;}
		}

		/// <summary>
		/// Gets or sets the EnableTooltip property of the GraphControl
		/// </summary>
		public bool EnableTooltip
		{
			get{return mEnableTooltip;}
			set{mEnableTooltip = value;}
		}

		/// <summary>
		/// Gets or sets the Locked property of the GraphControl
		/// </summary>
		public bool Locked
		{
			get{return mLocked;}
			set{mLocked = value;}
		}
		/// <summary>
		/// Gets or sets the GraphLayoutAlgorithm property of the GraphControl
		/// </summary>
		public  GraphLayoutAlgorithms GraphLayoutAlgorithm
		{
			get{return mGraphLayoutAlgorithm;}
			set{mGraphLayoutAlgorithm = value;}
		}

		/// <summary>
		/// Gets or sets the EnableLayout property of the GraphControl
		/// </summary>
		public bool EnableLayout
		{
			get{return mEnableLayout ;}
			set{mEnableLayout = value;}
		}

		/// <summary>
		/// Gets or sets the DefaultConnectionEnd property of the GraphControl
		/// </summary>
		public ConnectionEnd DefaultConnectionEnd
		{
			get{return mDefaultConnectionEnd ;}
			set{mDefaultConnectionEnd = value;}
		}

	
		/// <summary>
		/// Gets or sets the Snap property of the GraphControl
		/// </summary>
		public bool Snap
		{
			get{return mSnap ;}
			set{mSnap = value;}
		}

		/// <summary>
		/// Gets or sets the RestrictToCanvas property of the GraphControl
		/// </summary>
		public bool RestrictToCanvas
		{
			get{return mRestrictToCanvas ;}
			set{mRestrictToCanvas = value;}
		}
		/// <summary>
		/// Gets or sets the GridSize property of the GraphControl
		/// </summary>
		public int GridSize
		{
			get{return mGridSize ;}
			set{mGridSize = value;}
		}
		/// <summary>
		/// Gets or sets the GradientMode property of the GraphControl
		/// </summary>
		public LinearGradientMode GradientMode
		{
			get{return mGradientMode ;}
			set{mGradientMode = value;}
		}

		/// <summary>
		/// Gets or sets the EnableContextMenu property of the GraphControl
		/// </summary>
		public bool EnableContextMenu
		{
			get{return mEnableContextMenu ;}
			set{mEnableContextMenu = value;}
		}

		/// <summary>
		/// Gets or sets the DefaultConnectionPath property of the GraphControl
		/// </summary>
		public string DefaultConnectionPath
		{
			get{return mDefaultConnectionPath;}
			set{mDefaultConnectionPath = value;}
		}
		/// <summary>
		/// Gets or sets the BackgroundType property of the GraphControl
		/// </summary>
		public CanvasBackgroundType BackgroundType
		{
			get{return mBackgroundType;}
			set{mBackgroundType = value;}
		}

		/// <summary>
		/// Gets or sets the BackgroundImagePath property of the GraphControl
		/// </summary>
		public string BackgroundImagePath
		{
			get{return mBackgroundImagePath;}
			set{mBackgroundImagePath = value;}
		}
		/// <summary>
		/// Gets or sets the BackgroundColor property of the GraphControl
		/// </summary>
		public Color BackgroundColor
		{
			get{return mBackgroundColor;}
			set{mBackgroundColor = value;}
		}
		/// <summary>
		/// Gets or sets the AutomataPulse property
		/// </summary>
		public int AutomataPulse
		{
			get{return mAutomataPulse;}
			set{mAutomataPulse = value;}
		}
		/// <summary>
		/// Gets or sets the AllowAddShape property of the GraphControl
		/// </summary>
		public bool AllowMoveShape
		{
			get{return mAllowMoveShape;}
			set{mAllowMoveShape = value;}		
		}
		/// <summary>
		/// Gets or sets the AllowAddShape property of the GraphControl
		/// </summary>
		public bool AllowDeleteShape
		{
			get{return mAllowDeleteShape;}
			set{mAllowDeleteShape = value;}		
		}

		/// <summary>
		/// Gets or sets the AllowAddShape property of the GraphControl
		/// </summary>
		public bool AllowAddShape
		{
			get{return mAllowAddShape;}
			set{mAllowAddShape = value;}		
		}

		/// <summary>
		/// Gets or sets the AllowAddConnection property of the GraphControl
		/// </summary>
		public bool AllowAddConnection
		{
			get{return mAllowAddConnection;}
			set{mAllowAddConnection = value;}		
		}

		/// <summary>
		/// Gets or sets the gradient top-color
		/// </summary>
		public Color GradientTop
		{
			get{return mGradientTop;}
			set{mGradientTop = value;}
		}

		/// <summary>
		/// Gets or sets the gradient bottom-color
		/// </summary>
		public Color GradientBottom
		{
			get{return mGradientBottom;}
			set{mGradientBottom = value;}
		}

		/// <summary>
		/// Gets or sets the ShowGrid setting
		/// </summary>
		public bool ShowGrid
		{
			get{return mShowGrid;}
			set{mShowGrid = value;}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public BinaryAmbiance()
		{
			
		}
		/// <summary>
		/// Deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected BinaryAmbiance(SerializationInfo info, StreamingContext context) 
		{
		
		
			#region Incremental serialization fix; delete try-catch when properly saved with all members
			try
			{				
				 mLocked = info.GetBoolean("mLocked");
			}
			catch
			{
				this.mLocked = false;
			}

			try
			{
				mEnableTooltip = info.GetBoolean("mEnableTooltip");
			}
			catch
			{
				mEnableTooltip = true;
			}

			try
			{
			mShowAutomataController = info.GetBoolean("mShowAutomataController");
			}
			catch
			{
				mShowAutomataController = false;
			}
			#endregion
				mShowGrid = info.GetBoolean("mShowGrid");
				mGradientBottom = (Color) info.GetValue("mGradientBottom", typeof(Color));
				mGradientTop = (Color) info.GetValue("mGradientTop", typeof(Color));
				mAllowAddConnection = info.GetBoolean("mAllowAddConnection");
				mAllowAddShape = info.GetBoolean("mAllowAddShape");
				mAllowDeleteShape = info.GetBoolean("mAllowDeleteShape");
				mAllowMoveShape = info.GetBoolean("mAllowMoveShape");
				mAutomataPulse = info.GetInt32("mAutomataPulse");
				mBackgroundColor = (Color) info.GetValue("mBackgroundColor", typeof(Color));
				mBackgroundImagePath = info.GetString("mBackgroundImagePath");
				mBackgroundType = (CanvasBackgroundType) info.GetValue("mBackgroundType", typeof(CanvasBackgroundType));
				mDefaultConnectionPath = info.GetString("mDefaultConnectionPath");
				mGradientMode =(LinearGradientMode)  info.GetValue("mGradientMode", typeof(LinearGradientMode));
				mEnableContextMenu = info.GetBoolean("mEnableContextMenu");
				mGridSize = info.GetInt32("mGridSize");
				mRestrictToCanvas = info.GetBoolean("mRestrictToCanvas");
				mSnap = info.GetBoolean("mSnap");
				mDefaultConnectionEnd  = (ConnectionEnd) info.GetValue("mDefaultConnectionEnd", typeof(ConnectionEnd));
				mEnableLayout = info.GetBoolean("mEnableLayout");
				mGraphLayoutAlgorithm = (GraphLayoutAlgorithms) info.GetValue("mGraphLayoutAlgorithm", typeof(GraphLayoutAlgorithms));
				
			
		}
		#endregion

		#region ISerializable Members

		/// <summary>
		/// Serialization of this class
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
				info.AddValue("mLocked",  mLocked);
				info.AddValue("mShowGrid",  mShowGrid);
				info.AddValue("mGradientBottom",  mGradientBottom);
				info.AddValue("mGradientTop",  mGradientTop);
				info.AddValue("mAllowAddConnection",  mAllowAddConnection);
				info.AddValue("mAllowAddShape",  mAllowAddShape);
				info.AddValue("mAllowDeleteShape",  mAllowDeleteShape);
				info.AddValue("mAllowMoveShape",  mAllowMoveShape);
				info.AddValue("mAutomataPulse",  mAutomataPulse);
				info.AddValue("mBackgroundColor",  mBackgroundColor);
				info.AddValue("mBackgroundImagePath",  mBackgroundImagePath);
				info.AddValue("mBackgroundType",  mBackgroundType);
				info.AddValue("mDefaultConnectionPath",  mDefaultConnectionPath);
				info.AddValue("mGradientMode",  mGradientMode);
				info.AddValue("mEnableContextMenu",  mEnableContextMenu);
				info.AddValue("mGridSize",  mGridSize);
				info.AddValue("mRestrictToCanvas",  mRestrictToCanvas);
				info.AddValue("mSnap",  mSnap);
				info.AddValue("mDefaultConnectionEnd",  mDefaultConnectionEnd);
				info.AddValue("mEnableLayout",  mEnableLayout);
				info.AddValue("mGraphLayoutAlgorithm",  mGraphLayoutAlgorithm);
				info.AddValue("mEnableTooltip", mEnableTooltip);
				info.AddValue("mShowAutomataController", mShowAutomataController);
		}

		#endregion
	}
}
