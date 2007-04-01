using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Presentation.FlyweightShapes;
using MathNet.Symbolics.Presentation.Connectors;

namespace MathNet.Symbolics.Presentation.Shapes
{
    /// <summary>
    /// Math.NET Bus Shape
    /// </summary>
    public partial class BusShape : SimpleShapeBase
    {
        private YttriumConnector busConnector;
        private CommandReference sysRef;
        private Bus bus;
        private IFlyweightShape<BusShape> _fly;

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

        public YttriumConnector BusConnector
        {
            get { return busConnector; }
        }

        public override string EntityName
        {
            get { return "Math.NET Bus"; }
        }
        #endregion

        public BusShape(IModel model, CommandReference reference, IFlyweightShape<BusShape> fly)
            : base(model)
        {
            this.sysRef = reference;

            busConnector = new YttriumConnector(Model, ConnectorType.BusConnector); //new Point((int)(Rectangle.Left + Rectangle.Width / 2), Rectangle.Bottom), 
            busConnector.Name = "Bus";
            busConnector.Parent = this;
            Connectors.Add(busConnector);

            AssignFly(fly);
        }

        public void AssignFly(IFlyweightShape<BusShape> fly)
        {
            if(_fly != fly)
            {
                _fly = fly;
                if(_fly != null)
                {
                    _fly.InitShape(this);
                    _fly.Reposition(this, Point.Empty);
                }
            }

            Invalidate();
        }

        /// <summary>
        /// Tests whether the mouse hits this bundle
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Hit(Point p)
        {
            Rectangle r = new Rectangle(p, new Size(2, 2));
            return Rectangle.Contains(r);
        }

        /// <summary>
        /// Paints the bundle on the canvas
        /// </summary>
        /// <param name="g"></param>
        public override void Paint(Graphics g)
        {
            _fly.Paint(this, g);
        }

        public override void Move(Point p)
        {
            base.Move(p);
            _fly.Reposition(this, p);
        }
    }
}
