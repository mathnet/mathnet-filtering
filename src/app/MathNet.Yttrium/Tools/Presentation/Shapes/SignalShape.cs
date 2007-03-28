using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.NetronLight;
using Netron.NetronLight.Win;

using MathNet.Symbolics.Mediator;

namespace MathNet.Symbolics.Presentation.Shapes
{
    /// <summary>
    /// Math.NET Signal Shape
    /// </summary>
    public partial class SignalShape : SimpleShapeBase
    {
        protected Connector outConnector, inConnector;
        private CommandReference sysRef;
        private Signal signal;

        #region Properties
        public CommandReference SignalReference
        {
            get { return sysRef; }
            set { sysRef = value; }
        }
        public Signal Signal
        {
            get { return signal; }
            set { signal = value; }
        }

        public Connector OutputConnector
        {
            get { return outConnector; }
        }
        public Connector InputConnector
        {
            get { return inConnector; }
        }

        public override string EntityName
        {
            get { return "Math.NET Signal"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s"></param>
        public SignalShape(IModel s, CommandReference reference)
            : base(s)
        {
            this.sysRef = reference;
            Initialize();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleRectangle"/> class.
        /// </summary>
        public SignalShape(CommandReference reference)
            : base()
        {
            this.sysRef = reference;
            Initialize();
        }

        private void Initialize()
        {
            outConnector = new Connector(new Point(Rectangle.Right, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
            outConnector.Name = "Output";
            Connectors.Add(outConnector);

            inConnector = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)), Model);
            inConnector.Name = "Input";
            Connectors.Add(inConnector);

            foreach(IConnector con in Connectors)
            {
                con.Parent = this;
            }

            AutoSize = false;
            Width = 20;
            Height = 20;
            Resizable = false;
            Text = sysRef.Index.ToString();

            PaintStyle = new GradientPaintStyle(Color.OrangeRed, Color.Yellow, -135);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Tests whether the mouse hits this bundle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(Point p)
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
            g.SmoothingMode = SmoothingMode.HighQuality;

            //the shadow
            g.FillEllipse(ArtPallet.ShadowBrush, Rectangle.X + 3, Rectangle.Y + 3, Rectangle.Width, Rectangle.Height);
            
            //the actual shape
            g.FillEllipse(Brush, Rectangle);
            
            //the edge of the bundle
            if(Hovered || IsSelected)
                g.DrawEllipse(ArtPallet.HighlightPen, Rectangle);
            else
                g.DrawEllipse(Pen, Rectangle);
            
            //the connectors
            for(int k = 0; k < Connectors.Count; k++)
            {
                Connectors[k].Paint(g);
            }

            if(!string.IsNullOrEmpty(Text))
                g.DrawString(Text, ArtPallet.DefaultFont, Brushes.Black, TextRectangle.X - 6, TextRectangle.Y - 6);
        }
        #endregion
    }
}
