using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// SpeedVector vector. 
    /// Is just like a <see cref="Point"/> but the data type has double precision.
    /// </summary>
    struct SpeedVector
    {
        public double X;
        public double Y;
        public SpeedVector(double speedx, double speedy)
        {
            this.X = speedx;
            this.Y = speedy;
        }
    }
}
