using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Presentation.FlyweightShapes;

namespace MathNet.Symbolics.Presentation.Shapes
{
    /// <summary>
    /// Math.NET Signal Shape
    /// </summary>
    public partial class SignalShape : SimpleShapeBase
    {
        private Connector outConnector, inConnector;
        private CommandReference sysRef;
        private Signal signal;
        private IFlyweightShape<SignalShape> _fly;

        #region Properties
        public CommandReference SignalReference
        {
            get { return sysRef; }
            set { sysRef = value; }
        }
        public Signal Signal
        {
            get { return signal; }
            set { signal = value; }
        }

        public Connector OutputConnector
        {
            get { return outConnector; }
        }
        public Connector InputConnector
        {
            get { return inConnector; }
        }

        public override string EntityName
        {
            get { return "Math.NET Signal"; }
        }
        #endregion

        public SignalShape(IModel model, CommandReference reference, IFlyweightShape<SignalShape> fly)
            : base(model)
        {
            this.sysRef = reference;

            outConnector = new Connector(Model);
            outConnector.Name = "Output";
            outConnector.Parent = this;
            Connectors.Add(outConnector);

            inConnector = new Connector(Model);
            inConnector.Name = "Input";
            inConnector.Parent = this;
            Connectors.Add(inConnector);

            AssignFly(fly);
        }

        public void AssignFly(IFlyweightShape<SignalShape> fly)
        {
            if(_fly != fly)
            {
                _fly = fly;
                if(_fly != null)
                    _fly.InitShape(this);
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
    }
}
