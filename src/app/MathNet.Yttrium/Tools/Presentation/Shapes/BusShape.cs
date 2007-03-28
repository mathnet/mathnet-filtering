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
    /// Math.NET Bus Shape
    /// </summary>
    public partial class BusShape : SimpleShapeBase
    {
        protected Connector busConnector;
        private CommandReference sysRef;
        private Bus bus;

        #region Properties
        public CommandReference BusReference
        {
            get { return sysRef; }
            set { sysRef = value; }
        }
        public Bus Bus
        {
            get { return bus; }
            set { bus = value; }
        }

        public Connector BusConnector
        {
            get { return busConnector; }
        }

        public override string EntityName
        {
            get { return "Math.NET Bus"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s"></param>
        public BusShape(IModel s, CommandReference reference)
            : base(s)
        {
            this.sysRef = reference;
            Initialize();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleRectangle"/> class.
        /// </summary>
        public BusShape(CommandReference reference)
            : base()
        {
            this.sysRef = reference;
            Initialize();
        }

        private void Initialize()
        {
            busConnector = new Connector(new Point((int)(Rectangle.Left + Rectangle.Width / 2),Rectangle.Bottom), Model);
            busConnector.Name = "Bus";
            Connectors.Add(busConnector);

            foreach(IConnector con in Connectors)
            {
                con.Parent = this;
            }

            AutoSize = false;
            Width = 20;
            Height = 20;
            Resizable = false;
            Text = sysRef.Index.ToString();

            PaintStyle = new GradientPaintStyle(Color.DarkGreen, Color.Yellow, -135);
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
            g.FillRectangle(ArtPallet.ShadowBrush, Rectangle.X + 3, Rectangle.Y + 3, Rectangle.Width, Rectangle.Height);
            
            //the actual shape
            g.FillRectangle(Brush, Rectangle);
            
            //the edge of the bundle
            if(Hovered || IsSelected)
                g.DrawRectangle(ArtPallet.HighlightPen, Rectangle);
            else
                g.DrawRectangle(Pen, Rectangle);
            
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
