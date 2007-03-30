using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using MathNet.Symbolics.Presentation.Shapes;

namespace MathNet.Symbolics.Presentation.FlyweightShapes
{
    public class DefaultBusShape : IFlyweightShape<BusShape>
    {
        public void InitShape(BusShape shape)
        {
            shape.AutoSize = false;
            shape.Width = 20;
            shape.Height = 20;
            shape.Resizable = false;
            shape.Text = shape.BusReference.Index.ToString();

            //shape.PaintStyle = new GradientPaintStyle(Color.DarkGreen, Color.Yellow, -135);
            shape.PaintStyle = new GradientPaintStyle(Color.SteelBlue, Color.LightBlue, -135);

            Point loc = shape.Location;
            shape.BusConnector.Point = new Point(loc.X + 10, loc.Y + 20);
        }

        public void Paint(BusShape shape, Graphics g)
        {
            Rectangle rect = shape.Rectangle;

            g.SmoothingMode = SmoothingMode.HighQuality;

            //the shadow
            g.FillRectangle(ArtPallet.ShadowBrush, rect.X + 3, rect.Y + 3, rect.Width, rect.Height);

            //the actual shape
            g.FillRectangle(shape.Brush, rect);

            //the edge of the bundle
            if(shape.Hovered || shape.IsSelected)
                g.DrawRectangle(ArtPallet.HighlightPen, rect);
            else
                g.DrawRectangle(shape.Pen, rect);

            //the connectors
            shape.BusConnector.Paint(g);

            if(!string.IsNullOrEmpty(shape.Text))
                g.DrawString(shape.Text, ArtPallet.DefaultFont, Brushes.Black, shape.TextRectangle.X - 6, shape.TextRectangle.Y - 6);
        }
    }
}
