using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using MathNet.Symbolics.Presentation.Shapes;

namespace MathNet.Symbolics.Presentation.FlyweightShapes
{
    public class DefaultSignalShape : IFlyweightShape<SignalShape>
    {
        public void InitShape(SignalShape shape)
        {
            shape.AutoSize = false;
            shape.Width = 20;
            shape.Height = 20;
            shape.Resizable = false;
            shape.Text = shape.SignalReference.Index.ToString();

            shape.PaintStyle = new GradientPaintStyle(Color.OrangeRed, Color.Yellow, -135);

            Point loc = shape.Location;
            shape.InputConnector.Point = new Point(loc.X, loc.Y + 10);
            shape.OutputConnector.Point = new Point(loc.X + 20, loc.Y + 10);
        }

        public void Paint(SignalShape shape, Graphics g)
        {
            Rectangle rect = shape.Rectangle;

            g.SmoothingMode = SmoothingMode.HighQuality;

            //the shadow
            g.FillEllipse(ArtPallet.ShadowBrush, rect.X + 3, rect.Y + 3, rect.Width, rect.Height);

            //the actual shape
            g.FillEllipse(shape.Brush, rect);

            //the edge of the bundle
            if(shape.Hovered || shape.IsSelected)
                g.DrawEllipse(ArtPallet.HighlightPen, rect);
            else
                g.DrawEllipse(shape.Pen, rect);

            //the connectors
            shape.InputConnector.Paint(g);
            shape.OutputConnector.Paint(g);

            if(!string.IsNullOrEmpty(shape.Text))
                g.DrawString(shape.Text, ArtPallet.DefaultFont, Brushes.Black, shape.TextRectangle.X - 6, shape.TextRectangle.Y - 6);
        }
    }
}
