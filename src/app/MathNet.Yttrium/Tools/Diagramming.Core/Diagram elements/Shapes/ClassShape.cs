using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Netron.Diagramming.Core;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The typical shape shipping with Visual Studio 2005 in the class designer.
    /// </summary>	
    [Shape("Class shape", "ClassShape", "Special", "The typical shape shipping with Visual Studio 2005 in the class designer.")]
    public partial class ClassShape : ComplexShapeBase
    {

        #region Fields
        private Color mHeadColor = ArtPallet.RandomLowSaturationColor;
        /// <summary>
        /// the xpansion icon
        /// </summary>
        private SwitchIconMaterial xicon;
        /// <summary>
        /// the pen used to draw the edge of the shape, which is switching between the highlighted and normal state 
        /// </summary>
        private Pen pen;
        /// <summary>
        /// holds the connectors
        /// </summary>
        private Connector cBottom, cLeft, cRight, cTop;
        /// <summary>
        /// the height of the body
        /// </summary>
        private int bodyHeight;
        /// <summary>
        /// the type of body
        /// </summary>
        private BodyType mBodyType = BodyType.None;
        /// <summary>
        /// the main text in the head
        /// </summary>
        private string mTitle;
        /// <summary>
        /// the mSubTitle
        /// </summary>
        private string mSubTitle;        
        /// <summary>
        /// the collapse state is a flagged enum
        /// </summary>
        private bool mCollapsed = true;
        /// <summary>
        /// the list of items when set to list-type
        /// </summary>
        private ClassShapeItemCollection mList = new ClassShapeItemCollection();       
        /// <summary>
        /// the <see cref="LabelMaterial"/> to display the free text
        /// </summary>
        private LabelMaterial textMaterial;
        /// <summary>
        /// graphics utility to paint the shape
        /// </summary>
        private GraphicsPath path, gradientPath;
        /// <summary>
        /// graphics utility to paint the shape
        /// </summary>
        private Region darkRegion, gradientRegion;
        /// <summary>
        /// the brush used to paint the gradient of the shape-head
        /// </summary>
        private Brush gradientBrush;
        /// <summary>
        /// typographic string formatting
        /// </summary>
        StringFormat sf = StringFormat.GenericTypographic;
        /// <summary>
        /// the collection of <see cref="FolderMaterial"/> folders displayed when the shape is in <see cref="BodyType.List"/> mode.
        /// </summary>
        private CollectionBase<FolderMaterial> mFolders = new CollectionBase<FolderMaterial>();
        #endregion

        #region Properties

        private FolderMaterial mPropertiesNode;
        private FolderMaterial mMethodsNode;

        public FolderMaterial PropertiesNode
        {
            get { return mPropertiesNode; }
        }
        public FolderMaterial MethodsNode
        {
            get { return mMethodsNode; }
        }

        /// <summary>
        /// Gets or sets the color of the head.
        /// </summary>
        /// <value>The color of the head.</value>
        public Color HeadColor
        {
            get { return mHeadColor; }
            set { mHeadColor = value;
            
                if (PaintStyle is GradientPaintStyle)
                 (PaintStyle as GradientPaintStyle).StartColor = value;
            else if (PaintStyle is SolidPaintStyle)
                 (PaintStyle as SolidPaintStyle).SolidColor = value;
            }
        }

        /// <summary>
        /// Gets the friendly name of the entity to be displayed in the UI
        /// </summary>
        /// <value></value>
        public override string EntityName
        {
            get
            {
                return "Class Shape";
            }
        }

        /// <summary>
        /// Gets or sets the type of content displayed in the body of the bundle
        /// </summary>
        public BodyType BodyType
        {
            get
            {
                return mBodyType;
            }
            set
            {
                mBodyType = value;
                this.UpdateBody();
            }
        }

        /// <summary>
        /// Collapses the shape
        /// </summary>
        public void Collapse()
        {
            this.mCollapsed = true;            
            UpdateBody();
            this.Invalidate();
        }

        /// <summary>
        /// Expands the bundle to display the body
        /// </summary>
        public void Expand()
        {
            mCollapsed = false;

            UpdateBody();
            this.Invalidate();
        }

        /// <summary>
        /// Collapses the bundle and hides the body
        /// </summary>
        public bool Collapsed
        {
            get
            {
                return mCollapsed;
            }
            //set{mCollapsed = value;}
        }

        /// <summary>
        /// Gets the list of items displayed in the body
        /// </summary>
        public ClassShapeItemCollection List
        {
            get
            {
                return this.mList;
            }
        }

        /// <summary>
        /// Gets or sets the mSubTitle of the bundle
        /// </summary>
        public string SubTitle
        {
            get
            {
                return mSubTitle;
            }
            set
            {
                mSubTitle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed if the body-type is set to FreeText
        /// </summary>
        public string FreeText
        {
            get
            {
                return textMaterial.Text;
            }
            set
            {
                textMaterial.Text = value;
                //UpdateBody();
            }
        }
        
        /// <summary>
        /// Gets or sets the mTitle of the bundle
        /// </summary>
        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
            }
        }
        public override string Text
        {
            get
            {
                if (BodyType == BodyType.FreeText)
                    return textMaterial.Text;
                else
                    return string.Empty;


            }
            set
            {
                if (BodyType == BodyType.FreeText)
                    textMaterial.Text = value;
            }
        }

        /// <summary>
        /// Gets the folders of the shape which are displayed when its in <see cref="BodyType.List"/> mode.
        /// </summary>
        /// <value>The folders.</value>
        public CollectionBase<FolderMaterial> Folders
        {
            get { return mFolders; }
            internal set { mFolders = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public ClassShape(IModel model)
            : base(model)
        {
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShape"/> class.
        /// </summary>
        public ClassShape()
            : base()
        {
            Init();
        }
        /// <summary>
        /// Initialize of the bundle
        /// </summary>
        private void Init()
        {
            Name = "Class Shape";
            mTitle = "Class shape";
            mSubTitle = "by The Netron Project";
            this.Resizable = false;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            //the initial size
            Transform(0, 0, 200, 50);

            
            //mList.OnItemAdded += new EventHandler<CollectionEventArgs<ClassShapeItem>>(mList_OnItemAdded);

            #region The icon material
            CreateShapeIcon();
            #endregion

            #region The xpand icon material
            CreateXpansionIcon();
            #endregion

            #region The free text
            textMaterial = new LabelMaterial();
            textMaterial.Transform(new Rectangle(Rectangle.X + 5, Rectangle.Y + 18, Rectangle.Width - 10, bodyHeight));
            textMaterial.Text = GetQuotation();
            textMaterial.Visible = false;
            Children.Add(textMaterial);

            #endregion

            #region The folders

            /* The following code is only an example of what is possible.
             * You can add any shape material here but I guess the properties/methods nodes
             * are quite useful for many purposes.
             */

            string[] props = new string[] { "Rectangle", "Size", "Visible" };
            FolderMaterial folder1 = new FolderMaterial("Properties", props, "Resources.PublicProperty.ico");
            folder1.Transform(new Rectangle(Rectangle.X + 5, Rectangle.Y + 55, Rectangle.Width - 10, FolderMaterial.constHeaderHeight));
            folder1.Visible = false;
            folder1.ShowLines = true;
            mFolders.Add(folder1);
            folder1.OnFolderChanged += new EventHandler<RectangleEventArgs>(folders_OnFolderChanged);
            Children.Add(folder1);
            mPropertiesNode = folder1;

            string[] methds = new string[] { "Invalidate", "Transform", "Update", "Reset", "Delete" };
            FolderMaterial folder2 = new FolderMaterial("Methods", methds, "Resources.PublicMethod.ico");
            folder2.Transform(new Rectangle(Rectangle.X + 5, Rectangle.Y + 55 + FolderMaterial.constHeaderHeight + 10, Rectangle.Width - 10, FolderMaterial.constHeaderHeight));
            folder2.Visible = false;
            folder2.ShowLines = true;
            mFolders.Add(folder2);
            folder2.OnFolderChanged += new EventHandler<RectangleEventArgs>(folders_OnFolderChanged);
            Children.Add(folder2);
            mMethodsNode = folder2;

            #region this calculates the initial body height

            
            foreach (FolderMaterial folder in mFolders)
                bodyHeight += folder.Rectangle.Height + 3;

            //UpdateBody();
            #endregion

            #endregion

            #region Connectors
            cTop = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Top), Model);
            cTop.Name = "Top connector";
            cTop.Parent = this;
            Connectors.Add(cTop);

            cRight = new Connector(new Point(Rectangle.Right, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
            cRight.Name = "Right connector";
            cRight.Parent = this;
            Connectors.Add(cRight);

            cBottom = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), Model);
            cBottom.Name = "Bottom connector";
            cBottom.Parent = this;
            Connectors.Add(cBottom);

            cLeft = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
            cLeft.Name = "Left connector";
            cLeft.Parent = this;
            Connectors.Add(cLeft);
            #endregion



        }

        private void CreateXpansionIcon()
        {
            //we use a custom version of the ClickableIconMaterial
            xicon = new SwitchIconMaterial(SwitchIconType.UpDown);
            //Rectangle rec = new Rectangle(new Point(Rectangle.Right -20, Rectangle.Y + 5), xicon.Icon.Size);            
            xicon.Transform(new Rectangle(new Point(Rectangle.Right - 20, Rectangle.Y + 7), xicon.Icon.Size));
            xicon.Gliding = false;
            xicon.OnExpand += new EventHandler(xicon_OnExpand);
            xicon.OnCollapse += new EventHandler(xicon_OnCollapse);
            Children.Add(xicon);
        }

        private void CreateShapeIcon()
        {
            ClickableIconMaterial icon = new ClickableIconMaterial("Resources.ClassShape.png");
            //IconMaterial icon = new IconMaterial("Resources.idea.gif");
            if(icon.Icon == null)
                throw new InconsistencyException("The icon resource of the class shape could not be found.");
            //Rectangle rec = new Rectangle(new Point(Rectangle.X + 5, Rectangle.Y + 3), icon.Icon.Size);
            icon.Transform(new Rectangle(new Point(Rectangle.X + 5, Rectangle.Y + 5), icon.Icon.Size));
            icon.Gliding = false;
            Children.Add(icon);
        }
        /// <summary>
        /// Handles the <see cref="FolderMaterial.OnFolderChanged"/> event and recalculates the bounding rectangle of the body, which
        /// depends on the state of the folders when the class shape is in <see cref="BodyType.List"/> mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void folders_OnFolderChanged(object sender, RectangleEventArgs e)
        {
            if (mFolders.Count == 0) return;
            bodyHeight = 0;
            if (mFolders.Count > 1) //shifting of the folder with respect to its predecessor
            {
                for (int k = 1; k < mFolders.Count; k++)
                {
                    mFolders[k].Transform(new Rectangle(Rectangle.X + 5, mFolders[k-1].Rectangle.Bottom + 10, Rectangle.Width - 10, FolderMaterial.constHeaderHeight));
                }
            }
            foreach (FolderMaterial folder in mFolders)
                bodyHeight += folder.Rectangle.Height + 3;

            UpdateBody();
        }
        /// <summary>
        /// Handles the <see cref="SwitchIconMaterial.OnCollapse"/> event and sets the state of the class shape to 'collapsed'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xicon_OnCollapse(object sender, EventArgs e)
        {
            Collapse();
        }

        /// <summary>
        /// Handles the <see cref="SwitchIconMaterial.OnCollapse"/> event and sets the state of the class shape to 'expanded'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xicon_OnExpand(object sender, EventArgs e)
        {
            Expand();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates pens and brushes
        /// </summary>
        protected override void UpdatePaintingMaterial()
        {
            base.UpdatePaintingMaterial();
            if (PaintStyle is GradientPaintStyle)
                mHeadColor = (PaintStyle as GradientPaintStyle).StartColor;
            else if (PaintStyle is SolidPaintStyle)
                mHeadColor = (PaintStyle as SolidPaintStyle).SolidColor;
               
        }

        /// <summary>
        /// You can safely delete this method, it's gives some variety and tests different text sizes.
        /// </summary>
        /// <returns></returns>
        private string GetQuotation()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Netron.Diagramming.Core.Resources.Quotations.txt");
            if (stream != null)
            {
                Random rnd = new Random();
                StreamReader reader = new StreamReader(stream);
                string all = reader.ReadToEnd();
                string[] quotes = all.Split((char)10);
                reader.Close();
                stream.Close();
                return quotes[rnd.Next(0, quotes.Length)];

            }
            else
                return "Intelligence is characterized by a natural incomprehension of life.";
        }

        /// <summary>
        /// Tests whether the mouse hits this bundle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(System.Drawing.Point p)
        {
            Rectangle r = new Rectangle(p, new Size(5, 5));
            return Rectangle.Contains(r);
        }
         
        /// <summary>
        /// Paints the bundle on the canvas
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(Graphics g)
        {
            #region Set the quality of the drawing
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            #endregion

            #region Artist's material
            if (IsSelected || Hovered)
                pen = ArtPallet.HighlightPen;
            else
                pen = Pens.Gray;
            #endregion

            #region Shape's shadow and container
            path = new GraphicsPath();
            path.AddArc(Rectangle.X, Rectangle.Y, 20, 20, -180, 90);
            path.AddLine(Rectangle.X + 10, Rectangle.Y, Rectangle.X + Rectangle.Width - 10, Rectangle.Y);
            path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y, 20, 20, -90, 90);
            path.AddLine(Rectangle.X + Rectangle.Width, Rectangle.Y + 10, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 10);
            path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y + Rectangle.Height - 20, 20, 20, 0, 90);
            path.AddLine(Rectangle.X + Rectangle.Width - 10, Rectangle.Y + Rectangle.Height, Rectangle.X + 10, Rectangle.Y + Rectangle.Height);
            path.AddArc(Rectangle.X, Rectangle.Y + Rectangle.Height - 20, 20, 20, 90, 90);
            path.AddLine(Rectangle.X, Rectangle.Y + Rectangle.Height - 10, Rectangle.X, Rectangle.Y + 10);
            //shadow
            if (ArtPallet.EnableShadows)
            {
                darkRegion = new Region(path);
                darkRegion.Translate(5, 5);
                g.FillRegion(ArtPallet.ShadowBrush, darkRegion);
            }
            //background
            g.FillPath(Brushes.White, path);

            #endregion

            #region the header
            if (mCollapsed)
            {
                //paint the gradient                
                using (gradientBrush = new LinearGradientBrush(Rectangle.Location, new Point(Rectangle.X + Rectangle.Width, Rectangle.Y), HeadColor, Color.White))
                {
                    gradientRegion = new Region(path);
                    g.FillRegion(gradientBrush, gradientRegion);
                }
            }
            else
            {
                gradientPath = new GraphicsPath();
                gradientPath.AddArc(Rectangle.X + 1, Rectangle.Y + 1, 18, 18, -180, 90);
                gradientPath.AddLine(Rectangle.X + 11, Rectangle.Y + 1, Rectangle.X + Rectangle.Width - 11, Rectangle.Y + 1);
                gradientPath.AddArc(Rectangle.X + Rectangle.Width - 19, Rectangle.Y + 1, 18, 18, -90, 90);
                gradientPath.AddLine(Rectangle.X + Rectangle.Width - 1, Rectangle.Y + 50, Rectangle.X + 1, Rectangle.Y + 50);
                //gradient
                using (gradientBrush = new LinearGradientBrush(Rectangle.Location, new Point(((int)(Rectangle.X + Rectangle.Width)), ((int)(Rectangle.Y))), HeadColor, Color.White))
                {
                    gradientRegion = new Region(gradientPath);
                    g.FillRegion(gradientBrush, gradientRegion);
                }
            }
            #endregion

            #region Border
            //the border
            g.DrawPath(pen, path);
            #endregion

            #region Text and body

            g.DrawString(mTitle, ArtPallet.DefaultBoldFont, Brushes.Black, new Rectangle(Rectangle.X + 20, Rectangle.Y + 5, Rectangle.Width - 45, 27));
            g.DrawString(mSubTitle, ArtPallet.DefaultFont, Brushes.Black, new RectangleF(Rectangle.X + 20, Rectangle.Y + 35, Rectangle.Width - 10, 30), sf);

           
            #endregion

            #region The material
            foreach (IPaintable material in Children)
            {
                material.Paint(g);
            }
            #endregion

            #region The connectors
            //the connectors
            for (int k = 0; k < Connectors.Count; k++)
            {
                Connectors[k].Paint(g);
            }
            #endregion

        }

        /// <summary>
        /// Updates the collapsible body in function of the added properties and methods
        /// </summary>
        private void UpdateBody()
        {
            if (mCollapsed)
            {

                if (BodyType == BodyType.None)
                {
                    xicon.Visible = false;
                }
                else
                    xicon.Visible = true;
                Transform(Rectangle.X, Rectangle.Y, 200, 50);

                textMaterial.Visible = false;
                foreach (IShapeMaterial material in mFolders)
                    material.Visible = false;
                return;
            }
            else
            {
                
                if (BodyType == BodyType.FreeText)
                {
                    textMaterial.Visible = true;
                    xicon.Visible = true;
                    foreach (IShapeMaterial material in mFolders)
                        material.Visible = false;
                    Transform(Rectangle.X, Rectangle.Y, 200, 90 + 60 + 10);

                }
                else if (BodyType == BodyType.List)
                {
                    xicon.Visible = true;
                    textMaterial.Visible = false;
                    foreach (IShapeMaterial material in mFolders)
                        material.Visible = true;
                    Transform(Rectangle.X, Rectangle.Y, 200, bodyHeight + 60 + 10);
                }
                else if (BodyType == BodyType.None)
                {
                    textMaterial.Visible = false;
                    foreach (IShapeMaterial material in mFolders)
                        material.Visible = false;
                    Transform(Rectangle.X, Rectangle.Y, 200, 60 + 10);
                    xicon.Visible = false;
                }
            }
           

        }

        /// <summary>
        /// Updates the body of the bundle when an item is added to the list
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void mList_OnItemAdded(object sender, CollectionEventArgs<ClassShapeItem> e)
        {
            if (this.mBodyType == BodyType.List)
                this.UpdateBody();
        }
        #endregion
    }

    #region Utility classes
    /// <summary>
    /// The <see cref="ClassShape"/>'s two possible variations
    /// </summary>
    public enum BodyType
    {
        /// <summary>
        /// The class shape displays a series of <see cref="FolderMaterial"/> folders.
        /// </summary>
        List,
        /// <summary>
        /// The class shape displays a simple <see cref="LabelMaterial"/> in its body.
        /// </summary>
        FreeText,
        /// <summary>
        /// The class shape displays only a header and no body content.
        /// </summary>
        None
    }

    [Obsolete]
    public class ClassShapeItemCollection : Netron.Diagramming.Core.CollectionBase<ClassShapeItem>, IEnumerable<ClassShapeItem>
    {


    }




    /// <summary>
    /// An item in the <see cref="ClassShape"/>.
    /// </summary>
    public class ClassShapeItem
    {
        #region Fields
        private string mText;
        private Bitmap mIcon;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Bitmap Icon
        {
            get
            {
                return mIcon;
            }
            set
            {
                mIcon = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShapeItem"/> class.
        /// </summary>
        public ClassShapeItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShapeItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ClassShapeItem(string text)
        {
            this.mText = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShapeItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        public ClassShapeItem(string text, Bitmap icon)
            : this(text)
        {
            this.mIcon = icon;
        }

    }
    /// <summary>
    /// Utility class to pass ClassShape info via events.
    /// </summary>
    public class ClassShapeEventArgs : EventArgs
    {
        private ClassShapeItem item;

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public ClassShapeItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShapeEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ClassShapeEventArgs(ClassShapeItem item)
        {
            this.item = item;
        }
    }
    #endregion
}
