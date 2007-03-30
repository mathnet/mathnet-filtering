using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Netron.Diagramming.Core
{
    static class Measurements
    {
        [ThreadStaticAttribute()]
        private static float screenDpiX = 96.0F;

        [ThreadStaticAttribute()]
        private static float screenDpiY = 96.0F;

        private static float[,] conversionRatios = new float[,]{
	    {1.0F, 4.0F, 1.3333334e-002F, 0.33866665F, 0.95999998F}, 
	    {0.25F, 1.0F, 3.3333334e-003F, 8.4666662e-002F, 0.23999999F}, 
	    {75.0F, 300.0F, 1.0F, 25.4F, 72.0F}, 
	    {2.9527559F, 11.811024F, 3.9370079e-002F, 1.0F, 2.8346457F}, 
	    {1.0416666F, 4.1666665F, 1.3888889e-002F, 0.35277778F, 1.0F}, 
	    };

        private static MeasurementsUnit[] englishUnits;

        private static MeasurementsUnit[] metricUnits;


        private static int GraphicsUnitIndex(GraphicsUnit graphicsUnit)
        {
            int i = -1;
            switch (graphicsUnit)
            {
                case GraphicsUnit.Display:
                    i = 0;
                    break;

                case GraphicsUnit.Document:
                    i = 1;
                    break;

                case GraphicsUnit.Inch:
                    i = 2;
                    break;

                case GraphicsUnit.Millimeter:
                    i = 3;
                    break;

                case GraphicsUnit.Point:
                    i = 4;
                    break;
            }
            return i;
        }

        public static void InitScreenDPI()
        {
            Graphics graphics = Graphics.FromHwnd((IntPtr)0);
            if (graphics != null)
            {
                screenDpiX = graphics.DpiX;
                screenDpiY = graphics.DpiY;
            }
        }

        public static float UnitsPerInch(GraphicsUnit unit)
        {
            float f = 1.0F;
            switch (unit)
            {
                case GraphicsUnit.Inch:
                    return 1.0F;

                case GraphicsUnit.Millimeter:
                    return 25.4F;

                case GraphicsUnit.Point:
                    return 72.0F;

                case GraphicsUnit.Document:
                    return 300.0F;

                case GraphicsUnit.Display:
                    return 75.0F;

                default:
                    return f;
            }
        }

        public static float Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, float dpi, float value)
        {
            float f1 = value;
            if (fromUnits != toUnits)
            {
                float f2;

                GraphicsUnit graphicsUnit;

                if (fromUnits == GraphicsUnit.Pixel)
                {
                    graphicsUnit = GraphicsUnit.Inch;
                    f2 = value / dpi;
                }
                else
                {
                    f2 = value;
                    graphicsUnit = fromUnits;
                }
                if (graphicsUnit == toUnits)
                {
                    f1 = f2;
                }
                else if (toUnits == GraphicsUnit.Pixel)
                {
                    if (graphicsUnit != GraphicsUnit.Inch)
                    {
                        int i = GraphicsUnitIndex(graphicsUnit);
                        int j = GraphicsUnitIndex(GraphicsUnit.Inch);
                        f2 *= conversionRatios[i, j];
                    }
                    f1 = f2 * dpi;
                }
                else
                {
                    int i = GraphicsUnitIndex(graphicsUnit);
                    int j = GraphicsUnitIndex(toUnits);
                    f1 = f2 * conversionRatios[i, j];
                }
            }
            return f1;
        }

        public static float Convert(GraphicsUnit fromUnits, float fromScale, GraphicsUnit toUnits, float toScale, float dpi, float value)
        {
            float f1 = value;
            if (fromUnits != toUnits || fromScale != toScale)
            {
                float f2;

                GraphicsUnit graphicsUnit;

                if (fromUnits == GraphicsUnit.Pixel)
                {
                    graphicsUnit = GraphicsUnit.Inch;
                    f2 = value / dpi;
                }
                else
                {
                    f2 = value;
                    graphicsUnit = fromUnits;
                }
                f2 *= fromScale;
                if (graphicsUnit == toUnits)
                {
                    f1 = f2;
                }
                else if (toUnits == GraphicsUnit.Pixel)
                {
                    if (graphicsUnit != GraphicsUnit.Inch)
                    {
                        int i = GraphicsUnitIndex(graphicsUnit);
                        int j = GraphicsUnitIndex(GraphicsUnit.Inch);
                        f2 *= conversionRatios[i, j];
                    }
                    f1 = f2 * dpi;
                }
                else
                {
                    int i = GraphicsUnitIndex(graphicsUnit);
                    int j = GraphicsUnitIndex(toUnits);
                    f1 = f2 * conversionRatios[i, j];
                }
                f1 /= toScale;
            }
            return f1;
        }

        public static float Convert(MeasurementsUnit fromUnits, GraphicsUnit toUnits, float toScale, float dpi, float value)
        {
            GraphicsUnit graphicsUnit;
            float f;
            MeasurementsUnitToGraphicsUnit(fromUnits, out graphicsUnit, out f);
            return Convert(graphicsUnit, f, toUnits, toScale, dpi, value);
        }

        public static float Convert(GraphicsUnit fromUnits, float fromScale, MeasurementsUnit toUnits, float dpi, float value)
        {
            GraphicsUnit graphicsUnit;
            float f;
            MeasurementsUnitToGraphicsUnit(toUnits, out graphicsUnit, out f);
            return Convert(fromUnits, fromScale, graphicsUnit, f, dpi, value);
        }

        public static float Convert(MeasurementsUnit fromUnits, MeasurementsUnit toUnits, float dpi, float value)
        {
            GraphicsUnit graphicsUnit1, graphicsUnit2;
            float f1, f2;
            MeasurementsUnitToGraphicsUnit(fromUnits, out graphicsUnit1, out f1);
            MeasurementsUnitToGraphicsUnit(toUnits, out graphicsUnit2, out f2);
            return Convert(graphicsUnit1, f1, graphicsUnit2, f2, dpi, value);
        }

        public static float Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, float value)
        {
            return Convert(fromUnits, toUnits, screenDpiX, value);
        }

        public static float Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, Graphics grfx, float value)
        {
            float f = screenDpiX;
            if (grfx != null)
            {
                f = grfx.DpiX;
            }
            return Convert(fromUnits, toUnits, f, value);
        }

        public static PointF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            float f3 = Convert(fromUnits, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static PointF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, Graphics grfx, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static PointF Convert(GraphicsUnit fromUnits, float fromScale, GraphicsUnit toUnits, float toScale, Graphics grfx, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, fromScale, toUnits, toScale, f1, value.X);
            float f4 = Convert(fromUnits, fromScale, toUnits, toScale, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static PointF Convert(MeasurementsUnit fromUnits, MeasurementsUnit toUnits, Graphics grfx, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static PointF Convert(GraphicsUnit fromUnits, float fromScale, MeasurementsUnit toUnits, Graphics grfx, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, fromScale, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, fromScale, toUnits, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static PointF Convert(MeasurementsUnit fromUnits, GraphicsUnit toUnits, float toScale, Graphics grfx, PointF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, toUnits, toScale, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, toScale, f2, value.Y);
            return new PointF(f3, f4);
        }

        public static SizeF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, SizeF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            float f3 = Convert(fromUnits, toUnits, f1, value.Width);
            float f4 = Convert(fromUnits, toUnits, f2, value.Height);
            return new SizeF(f3, f4);
        }

        public static SizeF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, Graphics grfx, SizeF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, toUnits, f1, value.Width);
            float f4 = Convert(fromUnits, toUnits, f2, value.Height);
            return new SizeF(f3, f4);
        }

        public static SizeF Convert(GraphicsUnit fromUnits, float fromScale, GraphicsUnit toUnits, float toScale, Graphics grfx, SizeF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, fromScale, toUnits, toScale, f1, value.Width);
            float f4 = Convert(fromUnits, fromScale, toUnits, toScale, f2, value.Height);
            return new SizeF(f3, f4);
        }

        public static RectangleF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, RectangleF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            float f3 = Convert(fromUnits, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, f2, value.Y);
            float f5 = Convert(fromUnits, toUnits, f1, value.Width);
            float f6 = Convert(fromUnits, toUnits, f2, value.Height);
            return new RectangleF(f3, f4, f5, f6);
        }

        public static RectangleF Convert(GraphicsUnit fromUnits, GraphicsUnit toUnits, Graphics grfx, RectangleF value)
        {
            float f1 = screenDpiX;
            float f2 = screenDpiY;
            if (grfx != null)
            {
                f1 = grfx.DpiX;
                f2 = grfx.DpiY;
            }
            float f3 = Convert(fromUnits, toUnits, f1, value.X);
            float f4 = Convert(fromUnits, toUnits, f2, value.Y);
            float f5 = Convert(fromUnits, toUnits, f1, value.Width);
            float f6 = Convert(fromUnits, toUnits, f2, value.Height);
            return new RectangleF(f3, f4, f5, f6);
        }

        public static void MeasurementsUnitToGraphicsUnit(MeasurementsUnit unitMeasure, out GraphicsUnit grfxUnit, out float grfxScale)
        {
            switch (unitMeasure)
            {
                case MeasurementsUnit.SixteenthInches:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 6.25e-002F;
                    return;

                case MeasurementsUnit.EighthInches:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 0.125F;
                    return;

                case MeasurementsUnit.QuarterInches:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 0.25F;
                    return;

                case MeasurementsUnit.HalfInches:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 0.5F;
                    return;

                case MeasurementsUnit.Inches:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 1.0F;
                    return;

                case MeasurementsUnit.Feet:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 12.0F;
                    return;

                case MeasurementsUnit.Yards:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 36.0F;
                    return;

                case MeasurementsUnit.Miles:
                    grfxUnit = GraphicsUnit.Inch;
                    grfxScale = 63360.0F;
                    return;

                case MeasurementsUnit.Millimeters:
                    grfxUnit = GraphicsUnit.Millimeter;
                    grfxScale = 1.0F;
                    return;

                case MeasurementsUnit.Centimeters:
                    grfxUnit = GraphicsUnit.Millimeter;
                    grfxScale = 10.0F;
                    return;

                case MeasurementsUnit.Meters:
                    grfxUnit = GraphicsUnit.Millimeter;
                    grfxScale = 1000.0F;
                    return;

                case MeasurementsUnit.Kilometers:
                    grfxUnit = GraphicsUnit.Millimeter;
                    grfxScale = 1000000.0F;
                    return;

                case MeasurementsUnit.Points:
                    grfxUnit = GraphicsUnit.Point;
                    grfxScale = 1.0F;
                    return;

                default:
                    grfxUnit = GraphicsUnit.Pixel;
                    grfxScale = 1.0F;
                    return;
            }
        }

        public static MeasurementSystem GetUnitsSystem(MeasurementsUnit units)
        {
            MeasurementSystem measurementSystem = MeasurementSystem.English;
            MeasurementsUnit[] measurementsUnits = metricUnits;
            for (int i = 0; i < (int)measurementsUnits.Length; i++)
            {
                if (measurementsUnits[i] == units)
                {
                    measurementSystem = MeasurementSystem.Metric;
                    break;
                }
            }
            return measurementSystem;
        }

        public static bool GetLargerUnits(MeasurementsUnit units, out MeasurementsUnit largerUnits)
        {
            bool flag = false;
            largerUnits = units;
            for (int i = 0; !flag && i < (int)englishUnits.Length - 1; i++)
            {
                if (englishUnits[i] == units)
                {
                    largerUnits = englishUnits[i + 1];
                    flag = true;
                }
            }
            for (int j = 0; !flag && j < (int)metricUnits.Length - 1; j++)
            {
                if (metricUnits[j] == units)
                {
                    largerUnits = englishUnits[j + 1];
                }
            }
            return flag;
        }

        public static bool GetSmallerUnits(MeasurementsUnit units, out MeasurementsUnit smallerUnits)
        {
            bool flag = false;
            smallerUnits = units;
            for (int i = (int)englishUnits.Length - 1; !flag && i > 0; i--)
            {
                if (englishUnits[i] == units)
                {
                    smallerUnits = englishUnits[i - 1];
                    flag = true;
                }
            }
            for (int j = (int)metricUnits.Length - 1; !flag && j > 0; j--)
            {
                if (metricUnits[j] == units)
                {
                    smallerUnits = metricUnits[j - 1];
                    flag = true;
                }
            }
            return flag;
        }

        public static MeasurementsUnit GetSystemUnits(MeasurementSystem measureSys, int unitOrdinal)
        {
            MeasurementsUnit measurementsUnit;

            //if (measureSys == null)
            //{
            //  if (unitOrdinal < 0 || unitOrdinal >= (int)englishUnits.Length)
            //  {
            //      throw new InconsistencyException("Oops");
            //  }
            //  measurementsUnit = englishUnits[unitOrdinal];
            //}
            //else
            {
                if (measureSys != MeasurementSystem.Metric)
                {
                    throw new InconsistencyException("Oops");
                }
                if (unitOrdinal < 0 || unitOrdinal >= (int)metricUnits.Length)
                {
                    throw new InconsistencyException("Oops");
                }
                measurementsUnit = metricUnits[unitOrdinal];
            }
            return measurementsUnit;
        }

        static Measurements()
        {
            MeasurementsUnit[] measurementsUnits = new MeasurementsUnit[8];
            measurementsUnits[1] = MeasurementsUnit.EighthInches;
            measurementsUnits[2] = MeasurementsUnit.QuarterInches;
            measurementsUnits[3] = MeasurementsUnit.HalfInches;
            measurementsUnits[4] = MeasurementsUnit.Inches;
            measurementsUnits[5] = MeasurementsUnit.Feet;
            measurementsUnits[6] = MeasurementsUnit.Yards;
            measurementsUnits[7] = MeasurementsUnit.Miles;
            englishUnits = measurementsUnits;
            measurementsUnits = new MeasurementsUnit[] { MeasurementsUnit.Millimeters, MeasurementsUnit.Centimeters, MeasurementsUnit.Meters, MeasurementsUnit.Kilometers };
            metricUnits = measurementsUnits;
        }
    }

}
