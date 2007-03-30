using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The art pallet consists of the often used colors, brushes, font etc. used by the painting methods.
    /// You can set the pallet at starup of the diagram control
    /// </summary>
    public static class ArtPallet
    {

        #region Fields
        private static Font mTitleFont = new Font("Arial", 20F, FontStyle.Bold);

       
        /// <summary>
        /// whether shadows of entities are painted
        /// </summary>
        private static bool mEnableShadows = false;     
        /// <summary>
        /// the font for the marks on the rulers
        /// </summary>
        private static Font mRulerFont = new Font("Arial", 7.0F, FontStyle.Regular);
        /// <summary>
        /// the ruler pen 
        /// </summary>
        private static Pen mRulerPen = Pens.Black;
        /// <summary>
        /// the background of the rulers
        /// </summary>
        private static Brush mRulerFillBrush = Brushes.White;
        /// <summary>
        /// the brush with which to paint the ghosts
        /// </summary>
        private static  Brush mGhostBrush = new SolidBrush(Color.FromArgb(120, Color.LightYellow));
        /// <summary>
        ///  the ghost pen
        /// </summary>
        private static Pen mGhostPen = new Pen(Color.Green, 1.5f);
        /// <summary>
        /// the pen to draw the interconnecting folder lines
        /// </summary>
        private static Pen mFolderLinesPen = new Pen(Color.Gray, 1.0F);
        /// <summary>
        /// the pen used for drawing the tracker
        /// </summary>
        private static Pen mTrackerPen = new Pen(Color.DimGray, 1.5F);
        /// <summary>
        /// the brush used to paint the grips
        /// </summary>
        private static Brush mGripBrush = Brushes.WhiteSmoke;
        /// <summary>
        /// Default font for drawing text
        /// </summary>
        private static Font mDefaultFont = new Font("Tahoma", 8.5F);
        /// <summary>
        /// the font used to  draw bold text
        /// </summary>
        private static Font mDefaultBoldFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
        /// <summary>
        /// randomizer for generating random colors of a certain style
        /// </summary>
        private static Random rnd = new Random();
        /// <summary>
        /// Default black pen
        /// </summary>
        private static Pen mBlackPen = new Pen(Brushes.Black, 1F);  
        /// <summary>
        /// the global shadow brush
        /// </summary>
        private static Brush mShadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black));
        /// <summary>
        /// Default red pen
        /// </summary>
        private static Pen mHighlightPen = new Pen(Brushes.OrangeRed, 1.7F);
        /// <summary>
        /// the global shadow pen
        /// </summary>
        private static Pen mShadowPen = new Pen(Color.FromArgb(30, Color.Black), 1F);
        /// <summary>
        /// the pen used to paint the connections
        /// </summary>
        private static Pen mConnectionPen = new Pen(Color.Silver,1F);
        /// <summary>
        /// the pen used to paint the highlighted connection
        /// </summary>
        private static Pen mConnectionHighlightPen = new Pen(Color.OrangeRed, 1F);

        private static IPaintStyle mTransparentPaintStyle = new SolidPaintStyle(Color.Transparent);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the title font used on the canvas to display the title of the current page.
        /// </summary>
        /// <value>The title font.</value>
        public static Font TitleFont
        {
            get { return mTitleFont; }
            set { mTitleFont = value; }
        }
        /// <summary>
        /// Gets or sets the global value indicating whether to enable entities shadows.
        /// </summary>
        /// <value><c>true</c> to enable shadows; otherwise, <c>false</c>.</value>
        public static bool EnableShadows
        {
            get { return mEnableShadows; }
            set { mEnableShadows = value; }
        }
        /// <summary>
        /// Gets the ruler font.
        /// </summary>
        /// <value>The ruler font.</value>
        public static Font RulerFont
        {
            get { return mRulerFont; }
        }
        /// <summary>
        /// Gets the pen with which the ruler is drawn.
        /// </summary>
        /// <value>The ruler pen.</value>
        public static Pen RulerPen
        {
            get { return mRulerPen; }
        }

        /// <summary>
        /// Gets the ruller fill brush.
        /// </summary>
        /// <value>The ruller fill brush.</value>
        public static Brush RullerFillBrush
        {
            get {
                return mRulerFillBrush;
            }
        }

        /// <summary>
        /// Gets the brush with which the ghosts are painted.
        /// </summary>
        /// <value>The ghost brush.</value>
        public static Brush GhostBrush
        {
            get
            {
                return mGhostBrush; 
            }
        }
        /// <summary>
        /// Gets the pen to draw the multi-point ghost lines.
        /// </summary>
        /// <value>The ghost pen.</value>
        public static Pen GhostPen
        {
            get
            {
                return mGhostPen;
            }
        }
        /// <summary>
        /// Gets the pen used to draw the dashed line between <see cref="FolderMaterial"/> nodes.
        /// </summary>
        /// <value>The folder lines pen.</value>
        public static Pen FolderLinesPen
        {
            get {

                return mFolderLinesPen;
            }
        }
        /// <summary>
        /// Gets or sets the connection pen.
        /// </summary>
        /// <value>The connection pen.</value>
        public static Pen ConnectionPen
        {
            get
            {
                return mConnectionPen;
            }
            set
            {
                mConnectionPen = value;
            }
        }
        /// <summary>
        /// Gets or sets the pen used to paint the highlighted connection.
        /// </summary>
        /// <value>The connection highlight pen.</value>
        public static Pen ConnectionHighlightPen
        {
            get
            {
                return mConnectionHighlightPen;
            }
            set
            {
                mConnectionHighlightPen = value;
            }
        }

        /// <summary>
        /// Gets or sets the grip brush.
        /// </summary>
        /// <value>The grip brush.</value>
        public static Brush GripBrush
        {
            get { return mGripBrush; }
            set { mGripBrush = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Inits the static instance.
        /// </summary>
        public static void Init()
        {
            mTrackerPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            mBlackPen.LineJoin = LineJoin.Round;
            
            //AdjustableArrowCap ccap = new AdjustableArrowCap(5, 7, true);
            //mConnectionPen.EndCap = LineCap.Custom;
            //mConnectionPen.CustomEndCap = ccap;

            //mConnectionHighlightPen.EndCap = LineCap.Custom;
            //mConnectionHighlightPen.CustomEndCap = ccap;

            mFolderLinesPen.DashStyle = DashStyle.Dot;
        }
        /// <summary>
        /// Gets or sets the default font to render text on the canvas.
        /// </summary>
        /// <value>The font.</value>
        public static Font DefaultFont
        {
            get
            {
                return ArtPallet.mDefaultFont;
            }
            set
            {
                ArtPallet.mDefaultFont = value;
            }
        }
        /// <summary>
        /// Gets or sets the default bold font to render text on the canvas.
        /// </summary>
        /// <value>The font.</value>
        public static Font DefaultBoldFont
        {
            get
            {
                return ArtPallet.mDefaultBoldFont;
            }
            set
            {
                ArtPallet.mDefaultBoldFont = value;
            }
        }
        /// <summary>
        /// Gets the black pen.
        /// </summary>
        /// <value>The black pen.</value>
        public static Pen BlackPen
        {
            get
            {
                return ArtPallet.mBlackPen;
            }
            set
            {
                ArtPallet.mBlackPen = value;
            }
        }
        /// <summary>
        /// Gets the tracker pen.
        /// </summary>
        /// <value>The red pen.</value>
        public static Pen TrackerPen
        {
            get { 
                return ArtPallet.mTrackerPen; }
            set { ArtPallet.mTrackerPen = value; }
        }
        /// <summary>
        /// Gets the red pen.
        /// </summary>
        /// <value>The red pen.</value>
        public static Pen HighlightPen
        {
            get
            {
                return ArtPallet.mHighlightPen;
            }
            set
            {
                ArtPallet.mHighlightPen = value;
            }
        }
        /// <summary>
        /// Gets the shadow brush.
        /// </summary>
        /// <value>The shadow brush.</value>
        public static Brush ShadowBrush
        {
            get
            {
                return ArtPallet.mShadowBrush;
            }
            set
            {
                ArtPallet.mShadowBrush = value;
            }
        }
        /// <summary>
        /// Gets the shadow pen to paint the shadow of the connections.
        /// </summary>
        /// <value>The shadow pen.</value>
        public static Pen ConnectionShadow
        {
            get
            {
                return ArtPallet.mShadowPen;
            }
            set
            {
                ArtPallet.mShadowPen = value;
            }
        }
        /// <summary>
        /// Gets a random color from the whole available color spectrum.
        /// </summary>
        /// <value>The random color.</value>
        ///<example file="ArtPallet.RandomColor.xml">
        ///
        /// 
        /// wdfgsdfgsdfs
        ///</example>
        public static Color RandomColor
        {
            get
            {
                return Color.FromArgb(rnd.Next(10, 250), rnd.Next(10, 250), rnd.Next(10, 250));
            }
        }
        /// <summary>
        /// Gets a random blue color.
        /// <remarks>You can generate any variation of a certain color range by specifying the HSV range and a utility function will convert it to 
        /// an RGB value (<see cref="Utils.HSL2RGB"/>).
        /// </remarks>
        /// </summary>
        /// <value>The random blues.</value>
        public static Color RandomBlues
        {
            get
            {
                return (Color) Utils.HSL2RGB((rnd.NextDouble() * 20D + 150D) / 255D, (rnd.NextDouble() * 150D + 100D) / 255D, (rnd.NextDouble() * 50D + 150D) / 255D);
            }
        }
        /// <summary>
        /// Gets a random low saturation color.
        /// </summary>
        /// <value>The random color.</value>
        public static Color RandomLowSaturationColor
        {
            get
            {
                return (Color) Utils.HSL2RGB((rnd.NextDouble() * 255D) / 255D, (rnd.NextDouble() * 20D + 30D) / 255D, (rnd.NextDouble() * 20D + 130D) / 255D);
            }
        }

        /// <summary>
        /// Gets the solid brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns></returns>
        public static Brush GetSolidBrush(Color color, int alpha)
        {
            return new SolidBrush(Color.FromArgb(alpha, color));
        }

        /// <summary>
        /// Gets the gradient brush.
        /// </summary>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Brush GetGradientBrush(Color startColor, Color endColor, Rectangle rectangle, float angle)
        {
            return new LinearGradientBrush(rectangle, startColor, endColor, angle);
        }

        /// <summary>
        /// Gets the default solid <see cref="IPaintStyle"/>.
        /// </summary>
        /// <returns></returns>
        public static IPaintStyle GetDefaultSolidPaintStyle()
        {
           return  new SolidPaintStyle(ArtPallet.RandomLowSaturationColor);
        }
        /// <summary>
        /// Gets the default gradient <see cref="IPaintStyle"/>.
        /// </summary>
        /// <returns></returns>
        public static IPaintStyle GetDefaultGradientPaintStyle()
        {
           return  new GradientPaintStyle();
        }
        /// <summary>
        /// Gets the transparent <see cref="IPaintStyle"/>.
        /// </summary>
        /// <returns></returns>
        public static IPaintStyle GetTransparentPaintStyle()
        {
            return mTransparentPaintStyle;
        }

        /// <summary>
        /// Returns the default paint style for the whole control.
        /// 
        /// </summary>
        /// <returns></returns>
        public static IPaintStyle GetDefaultPaintStyle()
        {
            return GetDefaultGradientPaintStyle();
        }

        /// <summary>
        /// Gets the default <see cref="IPenStyle"/>.
        /// </summary>
        /// <returns></returns>
        public static IPenStyle GetDefaultPenStyle()
        {
            return new PenStyle(); //this returns a black, standard pen.
        }

        #endregion
        
    }
 
}
