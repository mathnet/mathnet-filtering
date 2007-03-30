using System;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implements the horizontal ruler.
    /// </summary>
    public class HorizontalRuler : RulerBase
    {



        public override Rectangle Rectangle
        {
            get
            {
                Rectangle rectangle;

                if (View != null)
                {
                    rectangle = View.HorizontalRulerBounds;
                }
                else
                {
                    rectangle = Rectangle.Empty;
                }
                return rectangle;
            }
        }

        public HorizontalRuler(IView view)
            : base(view)
        {
        }

        public override void Paint(Graphics g)
        {
            if (View == null)
            {
                return;
            }
            IModel model = View.Model;
            if (model == null)
            {
                return;
            }
            
            float f1 = g.DpiX;
            MeasurementsUnit measurementsUnit1 = View.RulerUnits;
            GraphicsUnit graphicsUnit1 = model.MeasurementUnits;
            float f2 = model.MeasurementScale;


            Font font = ArtPallet.RulerFont;
            RectangleF rec = Rectangle;
            g.FillRectangle(ArtPallet.RullerFillBrush, rec);
            
            PointF pointF1 = View.Origin;
            bool flag1 = false;
            float f3 = (float)rec.Top;
            float f4 = (float)rec.Bottom;
            float f5;
            bool flag2 = true;
            GraphicsUnit graphicsUnit2;
            MeasurementsUnit measurementsUnit2;
            while (!flag1)
            {
                Measurements.MeasurementsUnitToGraphicsUnit(measurementsUnit1, out graphicsUnit2, out f5);
                float f6 = Measurements.Convert(graphicsUnit2, f5, graphicsUnit1, f2, f1, 1.0F);
                float f7 = View.ViewToDeviceF(View.WorldToView(new SizeF(f6, f6))).Width;
                if (f7 > 4.0F)
                {
                    PointF pointF2 = Measurements.Convert(graphicsUnit1, f2, graphicsUnit2, f5, g, pointF1);
                    int i = (int)Math.Floor((double)pointF2.X) + 1;
                    PointF pointF3 = new PointF((float)i, pointF2.Y);
                    pointF3 = Measurements.Convert(graphicsUnit2, f5, graphicsUnit1, f2, g, pointF3);
                    float f8 = View.ViewToDeviceF(View.WorldToView(pointF3)).X;
                    for (float f9 = (float)rec.Right; f8 < f9; f8 += f7)
                    {
                        g.DrawLine(ArtPallet.RulerPen, f8, f3, f8, f4);
                        if (flag2)
                        {
                            string str = i.ToString();
                            PointF pointF5 = new PointF(f8 + 1.0F, f3 + 1.0F);
                            g.DrawString(str, font, Brushes.Black, pointF5);
                        }
                        i++;
                    }
                    if (Measurements.GetSmallerUnits(measurementsUnit1, out measurementsUnit2))
                    {
                        measurementsUnit1 = measurementsUnit2;
                        f3 = f4 - (f4 - f3) / 2.0F;
                    }
                    else
                    {
                        flag1 = true;
                    }
                    flag2 = false;
                }
                else
                {
                    flag1 = true;
                }
            }
            g.DrawRectangle(ArtPallet.RulerPen, rec.X, rec.Y, rec.Width, rec.Height);


          
        }
    }

}
