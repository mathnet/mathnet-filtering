using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Presentation.Shapes;
using System.Drawing;

namespace MathNet.Symbolics.Presentation.FlyweightShapes
{
    public interface IFlyweightShape<TShape>
    {
        void InitShape(TShape shape);
        void Paint(TShape shape, Graphics g);
        void Reposition(TShape shape, Point shift);
    }
}
