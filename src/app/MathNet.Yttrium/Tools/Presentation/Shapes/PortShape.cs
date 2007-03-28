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
    /// Math.NET Port Shape
    /// </summary>
    public partial class PortShape : SimpleShapeBase
    {
        private CommandReference sysRef;
        private Port port;
        private List<Connector> cIn, cOut, cBus;

        #region Properties
        public CommandReference PortReference
        {
            get { return sysRef; }
            set { sysRef = value; }
        }
        public Port Port
        {
            get { return port; }
            set
            {
                port = value;
                UpdatePort();
            }
        }

        public List<Connector> InputConnectors
        {
            get { return cIn; }
        }
        public List<Connector> OutputConnectors
        {
            get { return cOut; }
        }
        public List<Connector> BusConnectors
        {
            get { return cBus; }
        }

        public override string EntityName
        {
            get { return "Math.NET Port"; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="s"></param>
        public PortShape(IModel s, CommandReference reference)
            : base(s)
        {
            this.sysRef = reference;
            cIn = new List<Connector>();
            cOut = new List<Connector>();
            cBus = new List<Connector>();
            Initialize();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleRectangle"/> class.
        /// </summary>
        public PortShape(CommandReference reference)
            : base()
        {
            this.sysRef = reference;
            cIn = new List<Connector>();
            cOut = new List<Connector>();
            cBus = new List<Connector>();
            Initialize();
        }

        private void UpdatePort()
        {
            Text = port.Entity.ToString();
            int inCnt = port.InputSignalCount;
            int outCnt = port.OutputSignalCount;
            int inoutCnt = Math.Max(inCnt, outCnt);
            int busCnt = port.BusCount;

            Height = 40 + 25 * inoutCnt;
            Width = 60 + 25 * busCnt;

            SyncConnectors(inCnt, cIn, "Input");
            SyncConnectors(outCnt, cOut, "Output");
            SyncConnectors(busCnt, cBus, "Bus");

            for(int i = 0; i < inCnt; i++)
                cIn[i].Point = new Point(Rectangle.Left, Rectangle.Top + 30 + i * 25);
            for(int i = 0; i < outCnt; i++)
                cOut[i].Point = new Point(Rectangle.Right, Rectangle.Top + 30 + i * 25);
            for(int i = 0; i < busCnt; i++)
                cBus[i].Point = new Point(Rectangle.Right - 30 - i * 25, Rectangle.Top);
        }

        private void SyncConnectors(int count, List<Connector> list, string prefix)
        {
            if(count > list.Count)
                for(int i = list.Count; i < count; i++)
                {
                    Connector c = new Connector(Model);
                    c.Name = prefix + i.ToString();
                    list.Add(c);
                    Connectors.Add(c);
                }
            else if(count < list.Count)
                for(int i = list.Count - 1; i > count; i--)
                {
                    Connector c = list[i];
                    Connectors.Remove(c);
                    list.RemoveAt(i);
                }
        }

        private void Initialize()
        {

            foreach(IConnector con in Connectors)
            {
                con.Parent = this;
            }

            AutoSize = false;
            Width = 60;
            Height = 40;
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

            GraphicsPath path = new GraphicsPath();
            path.AddArc(Rectangle.X, Rectangle.Y, 20, 20, -180, 90);
            path.AddLine(Rectangle.X + 10, Rectangle.Y, Rectangle.X + Rectangle.Width - 10, Rectangle.Y);
            path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y, 20, 20, -90, 90);
            path.AddLine(Rectangle.X + Rectangle.Width, Rectangle.Y + 10, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 10);
            path.AddArc(Rectangle.X + Rectangle.Width - 20, Rectangle.Y + Rectangle.Height - 20, 20, 20, 0, 90);
            path.AddLine(Rectangle.X + Rectangle.Width - 10, Rectangle.Y + Rectangle.Height, Rectangle.X + 10, Rectangle.Y + Rectangle.Height);
            path.AddArc(Rectangle.X, Rectangle.Y + Rectangle.Height - 20, 20, 20, 90, 90);
            path.AddLine(Rectangle.X, Rectangle.Y + Rectangle.Height - 10, Rectangle.X, Rectangle.Y + 10);

            //shadow
            Region darkRegion = new Region(path);
            darkRegion.Translate(4, 4);
            g.FillRegion(ArtPallet.ShadowBrush, darkRegion);

            //background
            g.FillPath(Brushes.White, path);

            //the border
            if(Hovered || IsSelected)
                g.DrawPath(ArtPallet.HighlightPen, path);
            else
                g.DrawPath(Pen, path);

            //the connectors
            Brush cBrush = ArtPallet.GetSolidBrush(Color.Gray, 255); //Color.SteelBlue
            foreach(Connector c in cOut)
            {
                g.FillRectangle(cBrush, c.Point.X - 7, c.Point.Y - 12, 7, 24);
                g.FillPie(cBrush, c.Point.X - 18, c.Point.Y - 12, 24, 24, 90, 180);
                //g.FillEllipse(cBrush, c.Point.X - 12, c.Point.Y - 12, 24, 24);
            }
            for(int k = 0; k < Connectors.Count; k++)
            {
                Connectors[k].Paint(g);
            }

            if(port != null)
            {
                g.DrawString(port.Entity.Symbol, ArtPallet.DefaultBoldFont, Brushes.Black, TextRectangle.X, TextRectangle.Y);
                g.DrawString(port.Entity.EntityId.Domain, ArtPallet.DefaultFont, Brushes.Black, TextRectangle.X, TextRectangle.Y + 15);
                g.DrawString(port.Entity.EntityId.Label, ArtPallet.DefaultFont, Brushes.Black, TextRectangle.X, TextRectangle.Y + 30);
            }
        }


        public override void Move(Point p)
        {
            base.Move(p);

            foreach(Connector c in cOut)
                foreach(Connector cnf in c.AttachedConnectors)
                {
                    IConnector c2 = ((IConnection)cnf.Parent).To.AttachedTo;
                    SignalShape ss = c2.Parent as SignalShape;
                    if(ss != null)
                    {
                        ss.Location = new Point(c.Point.X - 16, c.Point.Y - 10);
                    }
                }

        }


        #endregion
    }
}

