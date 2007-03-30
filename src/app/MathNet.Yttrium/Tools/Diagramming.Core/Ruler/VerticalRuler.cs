



using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implements the vertical ruler.
    /// </summary>
    public class VerticalRuler : RulerBase
    {

        #region Properties
        /// <summary>
        /// Gets the bounding rectangle of this vertical ruler.
        /// </summary>
        /// <value>The bounds.</value>
        public override Rectangle Rectangle
        {
            get
            {
                if (View != null)
                    return View.VerticalRulerBounds;
                else
                    return Rectangle.Empty;
            }
        }
        #endregion

        #region Constructor
        public VerticalRuler(IView view)
            : base(view)
        {
        }
        #endregion

        #region Methods
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
            Matrix matrix = new Matrix();
            PointF pointF1 = PointF.Empty;
            RectangleF rectangleF = Rectangle;
            g.FillRectangle(ArtPallet.RullerFillBrush, rectangleF);
            
            PointF pointF2 = View.Origin;
            bool flag1 = false;
            float f3 = (float)rectangleF.Left;
            float f4 = (float)rectangleF.Right;
            bool flag2 = true;
            float f5;
            GraphicsUnit graphicsUnit2;
            MeasurementsUnit measurementsUnit2;
            while (!flag1)
            {
                Measurements.MeasurementsUnitToGraphicsUnit(measurementsUnit1, out graphicsUnit2, out f5);
                float f6 = Measurements.Convert(graphicsUnit2, f5, graphicsUnit1, f2, f1, 1.0F);
                float f7 = View.ViewToDeviceF(View.WorldToView(new SizeF(f6, f6))).Height;
                if (f7 > 4.0F)
                {
                    PointF pointF3 = Measurements.Convert(graphicsUnit1, f2, graphicsUnit2, f5, g, pointF2);
                    int i = (int)Math.Floor((double)pointF3.Y) + 1;
                    PointF pointF4 = new PointF(pointF3.X, (float)i);
                    pointF4 = Measurements.Convert(graphicsUnit2, f5, graphicsUnit1, f2, g, pointF4);
                    float f8 = View.ViewToDeviceF(View.WorldToView(pointF4)).Y;
                    for (float f9 = (float)rectangleF.Bottom; f8 < f9; f8 += f7)
                    {
                        g.DrawLine(ArtPallet.RulerPen, f3, f8, f4, f8);
                        if (flag2)
                        {
                            string str = i.ToString();
                            SizeF sizeF2 = g.MeasureString(str, font);
                            matrix.Reset();
                            matrix.Translate(-(sizeF2.Width / 2.0F), -(sizeF2.Height / 2.0F), MatrixOrder.Append);
                            matrix.Rotate(-90.0F, MatrixOrder.Append);
                            matrix.Translate(sizeF2.Width / 2.0F, sizeF2.Height / 2.0F, MatrixOrder.Append);
                            matrix.Translate(f3 + 1.0F, f8 + 1.0F, MatrixOrder.Append);
                            g.Transform = matrix;
                            g.DrawString(str, font, Brushes.Black, pointF1);
                            g.Transform = new Matrix();
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

            g.DrawRectangle(ArtPallet.RulerPen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);

        }
        
#endregion
        
    }

}
