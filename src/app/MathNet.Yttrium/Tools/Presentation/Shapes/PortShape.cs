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
    /// Math.NET Port Shape
    /// </summary>
    public partial class PortShape : SimpleShapeBase
    {
        private CommandReference sysRef;
        private Port port;
        private List<YttriumConnector> cIn, cOut, cBus;
        private IFlyweightShape<PortShape> _fly;

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

        public List<YttriumConnector> InputConnectors
        {
            get { return cIn; }
        }
        public List<YttriumConnector> OutputConnectors
        {
            get { return cOut; }
        }
        public List<YttriumConnector> BusConnectors
        {
            get { return cBus; }
        }

        public override string EntityName
        {
            get { return "Math.NET Port"; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SimpleRectangle"/> class.
        /// </summary>
        public PortShape(IModel model, CommandReference reference, IFlyweightShape<PortShape> fly)
            : base(model)
        {
            this.sysRef = reference;

            cIn = new List<YttriumConnector>();
            cOut = new List<YttriumConnector>();
            cBus = new List<YttriumConnector>();

            AssignFly(fly);
        }

        public void AssignFly(IFlyweightShape<PortShape> fly)
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

        private void UpdatePort()
        {
            if(_fly != null)
            {
                _fly.InitShape(this);
                _fly.Reposition(this, Point.Empty);
            }
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

