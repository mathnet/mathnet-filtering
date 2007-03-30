using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Xml.Serialization;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// <para>
    /// The ambient properties of the canvas (background style, default line style etc) are collected in this class. The ambience class is part of the model and is serialized together with the diagram.
    /// </para>
    /// </summary>
    public partial class Ambience : IDisposable
    {
        #region Events
        /// <summary>
        /// Occurs when the Ambience has changed
        /// </summary>
        public event EventHandler<AmbienceEventArgs> OnAmbienceChanged;
        #endregion

        #region Fields
        /// <summary>
        /// pointer to the model
        /// </summary>
        private IModel mModel;
        /// <summary>
        /// the background type 
        /// </summary>
        private CanvasBackgroundTypes mBackgroundType;
        /// <summary>
        /// the GradientColor1 field
        /// </summary>
        private Color mGradientColor1;
        /// <summary>
        /// the GradientColor2 field
        /// </summary>
        private Color mGradientColor2;
        /// <summary>
        /// the BackgroundColor field
        /// </summary>
        private Color mBackgroundColor;
        /// <summary>
        /// the size of the page
        /// </summary>
        private Rectangle mPageSize = new Rectangle(Point.Empty, new Size(600, 800));

        private string mTitle = "Default page: no title.";

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }
         /// <summary>
        /// the Page field
        /// </summary>
        private IPage mPage;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public Rectangle PageSize
        {
            get { return mPageSize; }
            set { mPageSize = value; }
        }

       
        /// <summary>
        /// Gets or sets the Page to which this ambience belongs
        /// </summary>
        public IPage Page
        {
            get
            {
                return mPage;
            }
            set
            {
                mPage = value;
            }
        }


        /// <summary>
        /// Gets or sets the type of the background.
        /// </summary>
        /// <value>The type of the background.</value>
        public CanvasBackgroundTypes BackgroundType
        {
            get { return this.mBackgroundType; }
            set
            {
                this.mBackgroundType = value;
                RaiseOnAmbienceChanged();
            }
        }

        /// <summary>
        /// Gets or sets the first gradient color
        /// </summary>
        /// <value>The first gradient color.</value>
        public Color GradientColor1
        {
            get { return mGradientColor1; }
            set
            {
                mGradientColor1 = value;
                RaiseOnAmbienceChanged();
            }
        }
        /// <summary>
        /// Gets or sets the second gradient color
        /// </summary>
        /// <value>The second gradient color.</value>
        public Color GradientColor2
        {
            get { return mGradientColor2; }
            set
            {
                mGradientColor2 = value;
                RaiseOnAmbienceChanged();
            }
        }
        /// <summary>
        /// Gets or sets the BackgroundColor
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor
        {
            get { return mBackgroundColor; }
            set
            {
                mBackgroundColor = value;
                //notify the world that things have changed
                RaiseOnAmbienceChanged();

            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public IModel Model
        {
            get
            {
                return mModel;
            }
            internal set
            {
                mModel = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="page">The page.</param>
        public Ambience(IPage page)
        {
            if (page == null)
                throw new ArgumentNullException("The page paramter cannot be 'null'");
            if (page.Model == null)
                throw new ArgumentNullException("The Model is 'null'");

            //set default ambience
            mBackgroundColor = Color.LightSlateGray;
            mBackgroundType = CanvasBackgroundTypes.FlatColor;
            mGradientColor1 = Color.WhiteSmoke;
            mGradientColor2 = Color.LightSlateGray;
            mModel = page.Model;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="OnAmbienceChanged"/> event
        /// </summary>
        protected virtual void RaiseOnAmbienceChanged()
        {
            EventHandler<AmbienceEventArgs> handler = OnAmbienceChanged;
            if (handler != null)
            {
                handler(this, new AmbienceEventArgs(this));
            }
        }

        #endregion

        #region Standard IDispose implementation
        /// <summary>
        /// Disposes the view
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);


        }
        /// <summary>
        /// Part of the dispose implementation
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                #region free managed resources

                #endregion
            }

        }

        #endregion
    }
}
